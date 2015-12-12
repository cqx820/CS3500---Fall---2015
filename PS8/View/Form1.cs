using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;
using System.Net.Sockets;
using Network_Controller;
using Newtonsoft.Json;
using VirtualRealm.Mdx.Common;


namespace View
{
    /// <summary>
    /// This is the partial class AgCubioForm, this finished the painting and connect job
    /// </summary>
    public partial class AgCubioForm : Form
    {
        /// <summary>
        /// The object of World class, contains width and height field and some cube processing methods
        /// </summary>
        private World world;

        /// <summary>
        /// The user name which entered by users themselves
        /// </summary>
        private string userName;

        /// <summary>
        /// The server name
        /// </summary>
        private string serverName;

        /// <summary>
        /// The socket
        /// </summary>
        private Socket socket;

        /// <summary>
        /// The object type lock
        /// </summary>
        private object thisLock;

        /// <summary>
        /// The food count on the screen
        /// </summary>
        private int foodCount;

        /// <summary>
        /// The mass of user's cube
        /// </summary>
        private int massNumer;

        /// <summary>
        /// The width of user's cube
        /// </summary>
        private int cubeWidth;

        /// <summary>
        /// For zooming
        /// </summary>
        private const int scaleFactor = 2;

        /// <summary>
        /// Player's x-coordinate
        /// </summary>
        private double myLocX;

        /// <summary>
        /// Player's y-coordinate
        /// </summary>
        private double myLocY;

        /// <summary>
        /// The constructor of the form
        /// </summary>
        public AgCubioForm()
        {
            ///Initializing all components
            InitializeComponent();
            DoubleBuffered = true;
            world = new World(1000, 1500);
            world.OnPlayerDeath += World_OnPlayerDeath;
            userName = "";
            serverName = "";
            thisLock = new object();
            foodCount = 0;
            massNumer = 0;
            cubeWidth = 0;
            myLocX = 0;
            myLocY = 0;
        }

        /// <summary>
        /// New added method
        /// The judgement is in World class and when you lose, the message box would show you information
        /// </summary>
        private void World_OnPlayerDeath()
        {
            Invoke(new Action(() =>
            {
                gameStarted(false);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }));
            //MessageBox.Show("You died!");
            DialogResult boxResult = MessageBox.Show("You died, would you like to start a new game?", "", MessageBoxButtons.YesNo);
            if (boxResult == DialogResult.Yes)
            {
                
                FormApplicationContext.getAppContext().RunForm(new AgCubioForm());
            }
            else
            {
                this.Close();
                return;
            }
        }

        /// <summary>
        /// A call back method which is used to connect the server
        /// </summary>
        /// <param name="result">The Async Result</param>
        private void getServerInfo(IAsyncResult result)
        {
            Preserved_State ps = (Preserved_State)result.AsyncState;
            if (ps.exception != null)
            {
                // Handle server endconnect throwing exception
                MessageBox.Show("Failed to connect, please check the server name");
                gameStarted(false);
                return;
            }
            gameStarted(true);
            userName = nameBox.Text;
            //Send the user name to server
            Networking.Send(socket, userName + '\n');
            //Store the next call back function in Preserved_State class
            (result.AsyncState as Preserved_State).Callback = ReceiveData;
            //Request for more data from server
            Networking.i_want_more_data((Preserved_State)result.AsyncState);
        }

        //private void ReceivePlayerData(IAsyncResult result)
        //{
        //    lock (thisLock)
        //    {
        //        string[] temp = (result.AsyncState as Preserved_State).sb.ToString().Split(new string[] { "\n" }, StringSplitOptions.None);
        //        string playerInfo = temp[0];
        //        Cube rebuilt = JsonConvert.DeserializeObject<Cube>(playerInfo);
        //        world.addCube(rebuilt.uid, rebuilt);
        //        world.playerID = rebuilt.uid;
        //        int i = (result.AsyncState as Preserved_State).sb.ToString().IndexOf("\n");
        //        (result.AsyncState as Preserved_State).sb.Remove(0, i);
        //        // Deserialize string to convert to cube
        //        // Save player info
        //        (result.AsyncState as Preserved_State).Callback = ReceiveData;
        //        Networking.i_want_more_data((Preserved_State)result.AsyncState);
        //    }
        //}

        /// <summary>
        /// Call back function which is used to process the received data
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveData(IAsyncResult result)
        {
            //Lock added
            lock (thisLock)
            {
                Preserved_State ps = (Preserved_State)result.AsyncState;
                if (ps.exception != null)
                {
                    // Handle server endconnect throwing exception
                    MessageBox.Show("Failed to connect, please check the server name");
                    gameStarted(false);
                    return;
                }
                //Use the "stringProcess" method in the Preserved_State class to process JSON strings
                foreach (string jsonInfo in (result.AsyncState as Preserved_State).stringProcess())
                {
                    //When empty string sent, continue the loop
                    if (jsonInfo.Equals(""))
                    {
                        continue;
                    }
                    //Deserializing JSON strings
                    Cube rebuilt = JsonConvert.DeserializeObject<Cube>(jsonInfo);
                    //The first returned JSON strings must the user's cube information
                    if (world.team_id == 0)
                    {
                        //Player's id
                        world.team_id = rebuilt.uid;
                    }
                    if (world.team_id == rebuilt.uid)
                    {
                        myLocX = rebuilt.getRight();
                        myLocY = rebuilt.getBottom();
                    }
                    //Add the deserialized cube into the "World" class
                    world.addCube(rebuilt);
                }
                //Request for more data from the server
                Networking.i_want_more_data((Preserved_State)result.AsyncState);
            }
        }

        /// <summary>
        /// Event handler for textBox key press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                //After entering user name and press enter key, the serverBox would be focus
                serverBox.Focus();
            }
        }

        /// <summary>
        /// The most important part in the class. The paint method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Lock added
            lock (thisLock)
            {
                //Begin painting if panel1 is invisible
                if (!panel1.Visible)
                {
                    //Using uid to get the foodCubes from World class
                    foreach (Cube cube in world.foodCubes.Values)
                    {
                        //If a cube is food, then food count would increase
                        if (cube.food)
                        {
                            foodCount++;
                        }
                        //If cube is not a food and belongs user's, then the mass number and cube width would be changed
                        if (!cube.food && cube.uid == world.team_id)
                        {
                            massNumer = (int)cube.Mass;
                            cubeWidth = cube.getWidth();
                        }
                        //Create a new rectangle, the position is from the cube's
                        Rectangle r = new Rectangle((int)((scaleFactor * cube.getRight() - scaleFactor * myLocX) + (world.Width / scaleFactor)),
                            (int)((scaleFactor * cube.getBottom() - scaleFactor * myLocY) + (world.Width / scaleFactor)),
                            scaleFactor * cube.getWidth(), scaleFactor * cube.getWidth());
                        //Fill the rectangle
                        e.Graphics.FillRectangle(new System.Drawing.SolidBrush(Color.FromArgb(cube.argb_color)), r);
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        //The name would added on the cube
                        e.Graphics.DrawString(cube.Name, new Font("Times New Roman", 12),
                            new System.Drawing.SolidBrush(Color.Black), r, format);
                    }
                    //The cursor position
                    Point p = PointToClient(Cursor.Position);
                    //If the world received more than one foodCubes, the move request would be sent to server
                    if (world.getUids().Count() > 1)
                    {
                        Networking.Send(socket, "(move, " + p.X + ", " + p.Y + ")\n");
                    }
                    //The cube's status would be update on the panel2
                    statusUpdating(foodCount, massNumer, cubeWidth);
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Helper method which is used to update the cube's status on panel2
        /// </summary>
        /// <param name="foodCount">The food's count</param>
        /// <param name="massNumber">User's cube mass</param>
        /// <param name="cubeWidth">User's cube width</param>
        private void statusUpdating(int foodCount, int massNumber, int cubeWidth)
        {
            FoodValue.Text = foodCount + "";
            WidthValue.Text = cubeWidth + "";
            MassValue.Text = massNumer + "";
            FPSValue.Text = Utility.CalculateFrameRate().ToString();
            Application.DoEvents();
        }

        /// <summary>
        /// When finish entering the server name and press the enter key, the connection would be built
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxServer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Return || e.KeyChar == (char)Keys.Enter) && panel1.Visible)
            {
                serverName = serverBox.Text;
                try
                {
                    socket = Networking.Connect_to_Server(getServerInfo, serverName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        /// <summary>
        /// Split request sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AgCubio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space && !panel1.Visible)
            {
                Point p = Cursor.Position;
                //Send the split request to server
                Networking.Send(socket, "(split, " + p.X + ", " + p.Y + ")\n");
            }
        }

        /// <summary>
        /// Helper method which is used to reset the visbility and enability of the panel and form
        /// </summary>
        /// <param name="set"></param>
        private void gameStarted(bool set)
        {
            Invoke(new Action(() =>
            {
                if (set)
                {
                    panel1.Visible = false;
                    panel2.Enabled = true;
                }
                else
                {
                    panel1.Visible = true;
                    panel2.Enabled = false;
                    serverBox.Text = "localhost";
                    world.team_id = 0;
                    MassValue.Refresh();
                    WidthValue.Refresh();
                    FoodValue.Refresh();
                    FPSValue.Refresh();
                }
                this.Invalidate();
            }));
        }

        /// <summary>
        /// When user click the "Restart" button, the connection would be disconnected 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restartButton_Click(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                gameStarted(false);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }));
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

namespace VirtualRealm.Mdx.Common
{
    /// <summary>
    /// Reference:
    /// http://forum.codecall.net/topic/65487-calculating-frames-per-second/
    /// </summary>
    public class Utility
    {
        #region Basic Frame Counter
        private static int lastTick;
        private static int lastFrameRate;
        private static int frameRate;

        /// <summary>
        /// The method calculate the FPS. Refered from Internet resource
        /// </summary>
        /// <returns></returns>
        public static int CalculateFrameRate()
        {
            if (System.Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
            return lastFrameRate;
        }
        #endregion
    }
}