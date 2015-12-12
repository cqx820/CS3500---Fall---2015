using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Model;
using Network_Controller;
using Newtonsoft.Json;
using System.Xml;
using System.IO;
using System.Timers;
using System.Drawing;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace Server
{
    /// <summary>
    /// Acubio server console and web server
    /// </summary>
    public class Server
    {
        private World world;
        private Dictionary<Socket, PlayerStatus> socketSet;
        private Dictionary<int, PlayerStatus> playerStatusSet;
        private Dictionary<int, string[]> requestList;
        private Dictionary<int, Tuple<int, int, Cube>> moveSplits;
        private Timer timer;
        private Random rand;
        private List<Cube> changedCubes;
        public const string connectionString = "server=atr.eng.utah.edu;database=cs3500_qixiangc;uid=cs3500_qixiangc;password=serverSC";

        /// <summary>
        /// Contructor that  will build a new world and start the server.
        /// </summary>
        public Server()
        {
            socketSet = new Dictionary<Socket, PlayerStatus>();
            playerStatusSet = new Dictionary<int, PlayerStatus>();
            requestList = new Dictionary<int, string[]>();
            moveSplits = new Dictionary<int, Tuple<int, int, Cube>>();
            changedCubes = new List<Cube>();
            timer = new Timer();
            rand = new Random();
            readWorld();
            start();
        }


        /// <summary>
        /// masvn - creates a new server
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Server server = new Server();
            Console.Read();
        }

        /// <summary>
        /// starts the connection to client and wait for webreqests
        /// </summary>
        private void start()
        {
            Networking.Server_Awaiting_Client_Loop(clientConnectionHandler, 11000);
            Networking.Server_Awaiting_Client_Loop(webConnectionHandler, 11100);
            lock (world)
            {
                for (int i = 0; i < world.maxFood; i++)
                {
                    Cube cube = new Cube(rand.Next(world.Width), rand.Next(world.Height), world.randomColor(), world.incUID(), 0, true, "", world.foodValue);
                    if (i == 10 || i == 200 || i == 3000 || i == 4000)
                        cube.generateVirus();
                    world.addFood(cube);
                }
            }
            timer.Interval = 1000 / world.heartbeatPerSecond;
            timer.Elapsed += update;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        /// <summary>
        /// This method is used to handle the website connection
        /// </summary>
        /// <param name="obj"></param>
        private void webConnectionHandler(Preserved_State obj)
        {
            obj.Callback = handleWeb;
            Networking.i_want_more_data(obj);
        }

        /// <summary>
        /// Send the html string to port 11100
        /// </summary>
        /// <param name="obj"></param>
        private void handleWeb(IAsyncResult obj)
        {
            //Socket socket = ((Preserved_State)obj.AsyncState).socket;
            Preserved_State state = (Preserved_State)obj.AsyncState;
            string message = state.stringProcess().First();
            String html = processMessage(message);
            Console.WriteLine(message);
            string header = "HTTP/1.1 200 OK \r\n Connection: close \r\n Content-Type: text/html; charset=UTF-8 \r\n\r\n";
            Networking.Send(state.socket, header + html);

            System.Threading.Thread.Sleep(2000);
            state.socket.Shutdown(SocketShutdown.Both);
            state.socket.Close();
            //string html = "<html><body>Hello world!</body></html>\n";
            //"<!DOCTYPE html> <html> <body> <h1>Game Status</h1> </body> </html>"
        }

        /// <summary>
        /// Process the message received from the website
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string processMessage(String message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<!DOCTYPE html>");
            builder.Append("<html>");
            builder.Append("<head>");
            builder.Append("<title>");
            builder.Append("Our Page");
            builder.Append("</title>");
            builder.Append("</head>");
            builder.Append("<body>");

            //Extract relevant info
            Regex reg = new Regex(@"(GET\s+/)(.*)(\s+HTTP/1.1)");
            Match m = reg.Match(message);
            message = m.Groups[2].Value;
            string[] info = message.Split('?', '=');
            switch (info[0])
            {
                case "games":
                    builder.Append("<h1>");
                    builder.Append("Games played by " + info[2]);
                    builder.Append("</h1>");
                    builder.Append(getHtml("SELECT * FROM cs3500_qixiangc.Game WHERE (UserName LIKE '%" + info[2] + "%')"));
                    builder.Append("</body>");
                    builder.Append("</html>");
                    break;
                case "eaten":
                    builder.Append("<h1>");
                    builder.Append("All eaten cubes in session " + info[2]);
                    builder.Append("</h1>");
                    builder.Append(getHtml("SELECT * FROM cs3500_qixiangc.Eaten INNER JOIN cs3500_qixiangc.Game ON cs3500_qixiangc.Game.ID = cs3500_qixiangc.Eaten.ID WHERE cs3500_qixiangc.Eaten.ID = " + int.Parse(info[2])));
                    builder.Append("</body>");
                    builder.Append("</html>");
                    break;
                case "players":
                    builder.Append("<h1>");
                    builder.Append("Overall Server Status");
                    builder.Append("</h1>");
                    builder.Append(getHtml("SELECT * FROM cs3500_qixiangc.Game"));
                    builder.Append("</body>");
                    builder.Append("</html>");
                    break;
                default:
                    builder.Append("<h1>");
                    builder.Append("Error: Webpage does not exist");
                    builder.Append("</h1>");
                    builder.Append("<p>");
                    builder.Append("Use http://localhost:11100/players or http://localhost:11100/games?player=Joe or http://localhost:11100/eaten?id=35");
                    builder.Append("</p>");
                    break;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Enter SQL command and get the HTML strings 
        /// </summary>
        /// <param name="commandT"></param>
        /// <returns></returns>
        private string getHtml(String commandT)
        {
            String html = "";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();
                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = commandT;
                    // Execute the command and cycle through the DataReader object
                    MySqlDataAdapter da = new MySqlDataAdapter(command);
                    // this will query your database and return the result to your datatable
                    DataTable dataTable = new DataTable();
                    da.Fill(dataTable);

                    html = convertToHtml(dataTable);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return html;
        }

        /// <summary>
        /// Convert the information in DataTable to HTML strings 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string convertToHtml(DataTable dt)
        {
            //if (dt.Rows.Count == 0)
            //    return "";
            StringBuilder builder = new StringBuilder();
            String address = "http://" + getLocalAddress();
            builder.Append("<p><a href=\"" + address + ":11100/players\">Overall Game Info</a></p>");
            builder.Append("<table border='1px' cellpadding='5' cellspacing='0'");
            builder.Append("style='border: solid 1px Silver; font-size: x-small;'>");
            builder.Append("<tr align='left' valign='top'>");
            foreach (DataColumn c in dt.Columns)
            {
                builder.Append("<td align='left' valign='top'><b>");
                builder.Append(c.ColumnName);
                builder.Append("</b></td>");
            }
            builder.Append("</tr>");
            foreach (DataRow r in dt.Rows)
            {
                builder.Append("<tr align='left' valign='top'>");
                foreach (DataColumn c in dt.Columns)
                {
                    builder.Append("<td align='left' valign='top'>");
                    if (c.ColumnName.Equals("ID"))
                    {
                        builder.Append("<a href=\"" + address + ":11100/eaten?id=" + r[c.ColumnName] + "\">" + r[c.ColumnName] + "</a>");
                    }
                    else if (c.ColumnName.Equals("UserName") || c.ColumnName.Equals("EatenBy") || c.ColumnName.Equals("Eaten"))
                    {
                        builder.Append("<a href=\"" + address + ":11100/games?player=" + r[c.ColumnName] + "\">" + r[c.ColumnName] + "</a>");
                    }
                    else if (c.ColumnName.Equals("Rank"))
                    {
                        if ((int)r[c.ColumnName] < 6)
                            builder.Append(r[c.ColumnName]);
                        else
                            builder.Append("Unranked");
                    }
                    else
                    {
                        builder.Append(r[c.ColumnName]);
                    }
                    builder.Append("</td>");
                }
                builder.Append("</tr>");
            }
            builder.Append("</table>");
            return builder.ToString();
        }

        /// <summary>
        /// Method to get local IP - from stackoverflow
        /// </summary>
        /// <returns></returns>
        private string getLocalAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        /// <summary>
        /// Writes available information to the game and eaten tables
        /// </summary>
        /// <param name="maxMass"></param>
        /// <param name="aliveTime"></param>
        /// <param name="userName"></param>
        /// <param name="deathTime"></param>
        /// <param name="rank"></param>
        /// <param name="eatenBy"></param>
        /// <param name="cubesEaten"></param>
        /// <param name="eatenList"></param>
        public void WriteGameDatabase(double maxMass, TimeSpan aliveTime, string userName, DateTime deathTime, int rank, string eatenBy, int cubesEaten, HashSet<string> eatenList)
        {
            // Connect to the DB
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();
                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO Game (MaxMass, AliveTime, UserName, DeathTime, Rank, EatenBy,CubesEaten) VALUES (@MaxMass, @AliveTime, @UserName, @DeathTime, @Rank, @EatenBy,@CubesEaten)";
                    command.Parameters.AddWithValue("@MaxMass", maxMass);
                    command.Parameters.AddWithValue("@AliveTime", aliveTime);
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@DeathTime", deathTime);
                    command.Parameters.AddWithValue("@Rank", rank);
                    command.Parameters.AddWithValue("@EatenBy", eatenBy);
                    command.Parameters.AddWithValue("@CubesEaten", cubesEaten);
                    command.ExecuteNonQuery();
                    MySqlCommand getCommand = conn.CreateCommand();
                    getCommand.CommandText = "SELECT ID FROM cs3500_qixiangc.Game ORDER BY ID DESC LIMIT 1";
                    int id = 0;
                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = getCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] is int)
                            {
                                id = (int)reader["ID"];
                            }
                        }
                    }
                    foreach (String name in eatenList)
                    {
                        MySqlCommand command2 = conn.CreateCommand();
                        command2.CommandText = "INSERT INTO Eaten (ID,Eaten) VALUES (@ID, @Eaten)";
                        command2.Parameters.AddWithValue("@ID", id);
                        command2.Parameters.AddWithValue("@Eaten", name);
                        command2.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Helper method to connet to client
        /// </summary>
        /// <param name="result"></param>
        public void clientConnectionHandler(Preserved_State result)
        {
            result.Callback = nameReceiver;
            Networking.i_want_more_data(result);
        }

        /// <summary>
        /// Checks if cubes overlap using rectangles
        /// </summary>
        /// <param name="cube"></param>
        /// <returns></returns>
        private bool overlap(Cube cube)
        {
            bool overlap = false;
            Rectangle userRec = new Rectangle((int)cube.getRight(), (int)cube.getBottom(), cube.getWidth(), cube.getWidth());
            foreach (List<Cube> Cl in world.teamCubes.Values)
            {
                foreach (Cube c in Cl)
                {
                    if (isOverlap(cube, c))
                        overlap = true;
                }
            }
            return overlap;
        }

        /// <summary>
        /// 1st time sending cube info across network to client
        /// </summary>
        /// <param name="result"></param>
        public void nameReceiver(IAsyncResult result)
        {
            Preserved_State state = (Preserved_State)result.AsyncState;
            Cube userCube = new Cube(rand.Next(world.Width), rand.Next(world.Height), world.randomColor(), world.incUID(), 0, false, state.stringProcess().First(), world.playerStartMass);
            userCube.team_id = userCube.uid;
            lock (world)
            {
                while (overlap(userCube))
                {
                    userCube.loc_x = rand.Next(world.Width);
                    userCube.loc_y = rand.Next(world.Height);
                }
                //first send all information to the new socket
                Networking.Send(state.socket, JsonConvert.SerializeObject(userCube) + "\n");
                foreach (Cube c in world.foodCubes.Values)
                {
                    Networking.Send(state.socket, JsonConvert.SerializeObject(c) + "\n");
                }
                world.addUserCube(userCube);
                Console.WriteLine("Name has been sent and received");
            }
            PlayerStatus st = new PlayerStatus(userCube.Name, userCube.team_id, state.socket);
            socketSet.Add(state.socket, st);
            playerStatusSet.Add(userCube.team_id, st);
            state.Callback = dataHandler;
            Networking.i_want_more_data(state);
        }

        /// <summary>
        /// Stores requests
        /// </summary>
        /// <param name="result"></param>
        public void dataHandler(IAsyncResult result)
        {
            Preserved_State state = (Preserved_State)result.AsyncState;
            lock (requestList)
            {
                foreach (string s in state.stringProcess())
                {
                    if (s.Equals(""))
                    {
                        continue;
                    }
                    string[] request = s.Split('(', ')', ' ', ',');
                    PlayerStatus st;
                    if (socketSet.TryGetValue(state.socket, out st))
                    {
                        if (requestList.ContainsKey(st.TeamID))
                        {
                            requestList[st.TeamID] = new string[] { request[1], request[3], request[5] };
                        }
                        else
                        {
                            requestList.Add(st.TeamID, new string[] { request[1], request[3], request[5] });
                        }
                    }
                }
            }
            Networking.i_want_more_data(state);
        }

        /// <summary>
        /// Updates the server whenever timer thicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void update(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            lock (world)
            {
                processRequests();
                updateSplits();
                checkCollision();
                generateFood();
                sendToClient();
                requestList.Clear();
                changedCubes.Clear();
            }
            timer.Start();
        }

        /// <summary>
        /// Sends updated info to each socket connect
        /// </summary>
        private void sendToClient()
        {
            lock (world)
            {
                List<Socket> removeSockets = new List<Socket>();
                for (int i = 0; i < socketSet.Keys.Count; i++)
                {
                    Socket socket = socketSet.Keys.ElementAt(i);
                    try
                    {
                        List<Cube> removeList = new List<Cube>();
                        PlayerStatus st;
                        socketSet.TryGetValue(socket, out st);
                        //iterates through all player cubes and mark the onces to remove
                        foreach (List<Cube> Cl in world.teamCubes.Values)
                        {
                            foreach (Cube cube in Cl)
                            {

                                Networking.Send(socket, JsonConvert.SerializeObject(cube) + "\n");
                                if (cube.Mass == 0)
                                {
                                    removeList.Add(cube);
                                }
                            }
                        }
                        bool skip = false;
                        //Remove all 0 value player cubes
                        foreach (Cube cube in removeList)
                        {
                            int id = cube.team_id;
                            if (world.userRemove(cube))
                            {
                                PlayerStatus st1;
                                playerStatusSet.TryGetValue(id, out st1);
                                if (st.EndTime.Ticks != 0)
                                    continue;
                                st.endTime();
                                WriteGameDatabase(st.MaxMass, st.AliveTime, st.name, st.EndTime, st.MaxRank, st.EatenBy, st.CubesEaten, st.EatenNames);
                                socketSet.Remove(socket);
                                i--;
                                skip = true;

                            }
                        }
                        if (!skip)
                        {
                            //update the changed cubes
                            foreach (Cube c in changedCubes)
                            {
                                Networking.Send(socket, JsonConvert.SerializeObject(c) + "\n");
                                if (c.Mass == 0)
                                {
                                    world.foodCubes.Remove(c.uid);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Socket could not be sent" + e.Message);
                        removeSockets.Add(socket);
                    }
                }
                foreach (Socket s in removeSockets)
                {
                    PlayerStatus st;
                    socketSet.TryGetValue(s, out st);
                    if (st.EndTime.Ticks != 0)
                        continue;
                    st.endTime();
                    st.EatenBy = st.name;
                    WriteGameDatabase(st.MaxMass, st.AliveTime, st.name, st.EndTime, st.MaxRank, st.EatenBy, st.CubesEaten, st.EatenNames);
                    socketSet.Remove(s);
                }
            }
        }

        /// <summary>
        /// generates new food, and call attrition
        /// </summary>
        private void generateFood()
        {
            lock (world)
            {
                if (world.foodCubes.Count < world.maxFood)
                {
                    Cube newFood = new Cube(rand.Next(world.Width), rand.Next(world.Height), world.randomColor(), world.incUID(), 0, true, "", world.foodValue);
                    changedCubes.Add(newFood);
                    world.addFood(newFood);
                }
                Attrition();
            }
        }

        /// <summary>
        /// Checks if there are collesion between objects
        /// </summary>
        private void checkCollision()
        {
            lock (world)
            {
                //Creates a list of user cubes for ease of use
                List<Cube> userCubes = new List<Cube>();
                List<KeyValuePair<int, double>> tempList = new List<KeyValuePair<int, double>>();
                foreach (List<Cube> cl in world.teamCubes.Values)
                {
                    double mass = 0;
                    int teamId = -1;
                    if (cl.Count == 0)
                    {
                        continue;
                    }
                    foreach (Cube c in cl)
                    {
                        userCubes.Add(c);
                        mass += c.Mass;
                        teamId = c.team_id;
                    }
                    tempList.Add(new KeyValuePair<int, double>(teamId, mass));
                }
                tempList.Sort((firstPair, nextPair) =>
                {
                    return -1 * firstPair.Value.CompareTo(nextPair.Value);
                }
                );

                for (int i = 0; i < tempList.Count; i++)
                {
                    PlayerStatus st;
                    KeyValuePair<int, double> pair = tempList[i];
                    playerStatusSet.TryGetValue(pair.Key, out st);

                    st.testNewRank(i + 1);
                    st.testNewMass(pair.Value);

                }

                //Check every player cube
                for (int i = 0; i < userCubes.Count(); i++)
                {

                    //decrease counter if cube merge is on timer
                    if (userCubes[i].timer > 0)
                        userCubes[i].timer--;
                    //Check against other player cubes
                    for (int j = i + 1; j < userCubes.Count; j++)
                    {
                        Cube c1 = userCubes[i];
                        Cube c2 = userCubes[j];
                        if (isOverlap(c1, c2))
                        {
                            //If the two cubes belong to the same team is on timer , continue
                            if ((c1.team_id == c2.team_id) && (c1.timer != 0 || c2.timer != 0))
                                continue;
                            //Otherwise merge the two cubes
                            merge(c1, c2);
                            PlayerStatus survivorST;
                            PlayerStatus looserST;
                            if (c1.Mass > c2.Mass)
                            {
                                playerStatusSet.TryGetValue(c1.team_id, out survivorST);
                                playerStatusSet.TryGetValue(c2.team_id, out looserST);
                                survivorST.addEatenName(c2.Name);

                                looserST.EatenBy = c1.Name;

                            }
                            else
                            {
                                playerStatusSet.TryGetValue(c2.team_id, out survivorST);
                                playerStatusSet.TryGetValue(c1.team_id, out looserST);
                                survivorST.addEatenName(c1.Name);
                                looserST.EatenBy = c2.Name;
                            }
                        }
                    }


                    foreach (Cube c2 in world.foodCubes.Values)
                    {
                        Cube c1 = userCubes[i];
                        if (isOverlap(c1, c2))
                        {
                            if (c2.isVirus)
                            {

                                splitCube(c1.team_id, rand.Next(world.Width), rand.Next(world.Height));
                                splitCube(c1.team_id, rand.Next(world.Width), rand.Next(world.Height));
                                c2.Mass = 0;
                                changedCubes.Add(c2);

                            }
                            else
                                //Otherwise merge the two cubes
                                merge(c1, c2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Combines two cubes
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        private void merge(Cube c1, Cube c2)
        {
            if (c1.Mass >= c2.Mass)
            {
                c1.Mass += c2.Mass;
                c2.Mass = 0;
                PlayerStatus st;
                playerStatusSet.TryGetValue(c1.team_id, out st);
                if (c1.team_id != c2.team_id)
                {
                    st.addCubeEaten();
                }
                if (!c2.food && c2.uid == c2.team_id)
                {
                    swapUID(c2.team_id);
                }
            }
            else
            {
                c2.Mass += c1.Mass;
                c1.Mass = 0;
                PlayerStatus st;
                playerStatusSet.TryGetValue(c2.team_id, out st);
                if (c1.team_id != c2.team_id)
                {
                    st.addCubeEaten();
                }
                if (!c1.food && c1.uid == c1.team_id)
                {
                    swapUID(c1.team_id);
                }
            }
            changedCubes.Add(c2);
            changedCubes.Add(c1);
        }

        /// <summary>
        /// Randomly assigns the teamID to a cube in the team
        /// </summary>
        /// <param name="team_id"></param>
        private void swapUID(int team_id)
        {
            List<Cube> cList;
            if (!world.teamCubes.TryGetValue(team_id, out cList))
                return;
            int temp = 0;
            Cube tempC = null;
            for (int i = 0; i < cList.Count; i++)
            {
                if (cList[i].uid == team_id)
                {
                    tempC = cList[i];
                }
            }
            for (int i = 0; i < cList.Count; i++)
            {
                if (cList[i].Mass != 0)
                {
                    temp = cList[i].uid;
                    cList[i].uid = team_id;
                    tempC.uid = temp;
                    break;
                }
            }
        }

        /// <summary>
        /// Checks if two rectangles are overlapping using absorbDIstance Delta
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private bool isOverlap(Cube c1, Cube c2)
        {
            Rectangle rect1 = new Rectangle((int)c1.getRight(), (int)c1.getBottom(), c1.getWidth(), c1.getWidth());
            Rectangle rect2 = new Rectangle((int)c2.getRight(), (int)c2.getBottom(), c2.getWidth(), c2.getWidth());
            Rectangle intersect = Rectangle.Intersect(rect1, rect2);
            if (intersect.Width > world.absorbDistanceDelta || intersect.Height > world.absorbDistanceDelta)
                return true;
            else
                return false;
        }

        /// <summary>
        /// move the cubes that just split
        /// </summary>
        private void updateSplits()
        {
            //Move the split cubes
            List<int> removeList = new List<int>();
            lock (moveSplits)
            {
                foreach (Tuple<int, int, Cube> set in moveSplits.Values)
                {
                    int x = set.Item1;
                    int y = set.Item2;
                    Cube cube = set.Item3;
                    moveCube(cube, x, y, world.topSpeed);
                    if (withinStopDistance(cube, x, y))
                    {
                        {
                            if (!removeList.Contains(cube.team_id))
                                removeList.Add(cube.team_id);
                        }
                        cube.spliting = false;

                    }
                }
                foreach (int teamID in removeList)
                {
                    for (int i = 0; i < moveSplits.Count(); i++)
                    {
                        moveSplits.Values.ElementAt(i).Item3.spliting = false;
                        if (moveSplits.Values.ElementAt(i).Item3.team_id == teamID)
                            moveSplits.Remove(moveSplits.Values.ElementAt(i).Item3.uid);
                    }
                }
            }
        }

        private bool withinStopDistance(Cube cube, int x, int y)
        {
            if (cube.getLeft() > x && cube.getRight() < x && cube.getTop() > y && cube.getBottom() < y)
                return true;
            return false;
        }

        private void processRequests()
        {
            lock (world)
            {
                lock (requestList)
                {
                    //for every request
                    foreach (int team_id in requestList.Keys)
                    {

                        string[] temp;
                        requestList.TryGetValue(team_id, out temp);

                        int x = Int32.Parse(temp[1]);
                        int y = Int32.Parse(temp[2]);
                        //update move position according to move request
                        if (temp[0].Equals("move"))
                        {
                            move(team_id, x, y);
                        }

                        //if there's a split command
                        else if (temp[0].Equals("split"))
                        {
                            splitCube(team_id, x, y);

                        }
                    }
                }
            }
        }

        private void splitCube(int team_id, int x, int y)
        {
            //Create new cubes and put them in a list
            List<Cube> cubesToSplit = new List<Cube>();
            foreach (Cube c in Split(team_id, x, y))
                if (c.Mass > world.minSplitMass)
                    cubesToSplit.Add(c);

            if (cubesToSplit.Count * 2 > world.maxSplits)
                return;

            //iterates through the list, check distance, then add to permanemnt request list
            foreach (Cube cube in cubesToSplit)
            {

                if (Math.Abs(x - cube.loc_x) > world.maxSplitDistance)
                    if (cube.loc_x > x)
                        x = ((int)(cube.loc_x - world.maxSplitDistance));
                    else
                        x = ((int)(cube.loc_x + world.maxSplitDistance));
                if (Math.Abs(y - cube.loc_y) > world.maxSplitDistance)
                    if (cube.loc_y > y)
                        y = ((int)(cube.loc_y - world.maxSplitDistance));
                    else
                        y = ((int)(cube.loc_y + world.maxSplitDistance));
                lock (moveSplits)
                {
                    //add new cubes to permenant list in order to move cube
                    if (!moveSplits.ContainsKey(cube.uid))
                        moveSplits.Add(cube.uid, new Tuple<int, int, Cube>(x, y, cube));
                    else
                    {
                        moveSplits.Remove(cube.uid);
                        moveSplits.Add(cube.uid, new Tuple<int, int, Cube>(x, y, cube));
                    }
                }

            }
        }
        /// <summary>
        /// helper method to find the right cubes to split
        /// </summary>
        /// <param name="team_id"></param>
        /// <returns></returns>
        private IEnumerable<Cube> SplitHelper(int team_id)
        {
            List<Cube> temp;
            world.teamCubes.TryGetValue(team_id, out temp);
            temp = new List<Cube>(temp);
            foreach (Cube cube in temp)
            {
                if (cube.Mass >= world.minSplitMass)
                {
                    yield return cube;
                }
            }
            yield break;
        }


        /// <summary>
        /// Splits the cube in half
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private IEnumerable<Cube> Split(int teamID, int x, int y)
        {
            foreach (Cube cube in SplitHelper(teamID))
            {
                int id = world.incUID();
                Cube newCube = new Cube(cube.loc_x, cube.loc_y, cube.argb_color, id, teamID, false, cube.Name, cube.Mass / 2);
                newCube.timer = 500;
                newCube.spliting = true;
                world.addUserCube(newCube);
                cube.Mass /= 2;
                yield return newCube;
            }
        }

        /// <summary>
        /// moves a team of cubes
        /// </summary>
        /// <param name="team_id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void move(int team_id, int x, int y)
        {
            List<Cube> temp;
            world.teamCubes.TryGetValue(team_id, out temp);
            foreach (Cube cube in temp)
            {
                double speed = Math.Min(world.topSpeed / (cube.Mass / world.playerStartMass), world.topSpeed);
                if (!cube.spliting)
                    moveCube(cube, x, y, speed);

            }
        }

        /// <summary>
        /// helper method to move a single cube
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="speed"></param>
        private void moveCube(Cube cube, int x, int y, double speed)
        {

            if (speed < world.lowSpeed)
            {
                speed = world.lowSpeed;
            }
            if (cube.loc_x > x + world.topSpeed)
            {
                cube.loc_x -= speed;
            }
            else if (cube.loc_x < x - world.topSpeed)
            {
                cube.loc_x += speed;
            }
            if (cube.loc_y > y + world.topSpeed)
            {
                cube.loc_y -= speed;
            }
            else if (cube.loc_y < y - world.topSpeed)
            {
                cube.loc_y += speed;
            }
            if (cube.getLeft() > world.Width)
            {
                cube.loc_x = world.Width - (cube.getWidth() / 2);
            }
            else if (cube.getRight() < 0)
            {
                cube.loc_x = cube.getWidth() / 2;
            }
            if (cube.getTop() > world.Height)
            {
                cube.loc_y = world.Height - (cube.getWidth() / 2);
            }
            else if (cube.getBottom() < 0)
            {
                cube.loc_y = cube.getWidth() / 2;
            }
        }


        /// <summary>
        /// reduce cube mass accorind to attrition
        /// </summary>
        public void Attrition()
        {
            foreach (List<Cube> Cl in world.teamCubes.Values)
            {
                foreach (Cube cube in Cl)
                {
                    if (cube.Mass > world.playerStartMass)
                        cube.Mass -= world.attritionRate;
                }
            }
        }

        /// <summary>
        /// read world parameters from xml file in resources
        /// </summary>
        private void readWorld()
        {
            StreamReader reader = null;
            XmlReader xmlReader = null;
            try
            {
                int Width, Height, heartbeatPerSecond, foodValue, playerStartMass, maxFood, maxSplits;
                double topSpeed, lowSpeed, attritionRate, absorbDistanceDelta, minSplitMass, maxSplitDistance;
                //Create a new xml reader
                reader = new StreamReader(@"..\..\..\Resources\Packages\World_Param.xml");
                xmlReader = XmlReader.Create(reader);
                //read the elements
                xmlReader.ReadToFollowing("width");
                Width = xmlReader.ReadElementContentAsInt();
                xmlReader.ReadToFollowing("height");
                Height = xmlReader.ReadElementContentAsInt();
                xmlReader.ReadToFollowing("max_split_distance");
                maxSplitDistance = xmlReader.ReadElementContentAsDouble();
                xmlReader.ReadToFollowing("top_speed");
                topSpeed = xmlReader.ReadElementContentAsDouble();
                xmlReader.ReadToFollowing("low_speed");
                lowSpeed = xmlReader.ReadElementContentAsDouble();
                xmlReader.ReadToFollowing("attrition_rate");
                attritionRate = xmlReader.ReadElementContentAsDouble();
                xmlReader.ReadToFollowing("food_value");
                foodValue = xmlReader.ReadElementContentAsInt();
                xmlReader.ReadToFollowing("player_start_mass");
                playerStartMass = xmlReader.ReadElementContentAsInt();
                xmlReader.ReadToFollowing("max_food");
                maxFood = xmlReader.ReadElementContentAsInt();
                xmlReader.ReadToFollowing("min_split_mass");
                minSplitMass = xmlReader.ReadElementContentAsDouble();
                xmlReader.ReadToFollowing("absorb_constant");
                absorbDistanceDelta = xmlReader.ReadElementContentAsDouble();
                xmlReader.ReadToFollowing("max_splits");
                maxSplits = xmlReader.ReadElementContentAsInt();
                xmlReader.ReadToFollowing("heartbeats_per_second");
                heartbeatPerSecond = xmlReader.ReadElementContentAsInt();
                //Create new world;
                world = new World(Width, Height, heartbeatPerSecond, topSpeed, lowSpeed, attritionRate,
                           foodValue, playerStartMass, maxFood, minSplitMass, maxSplitDistance, maxSplits, absorbDistanceDelta);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to initialize world" + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (xmlReader != null)
                    xmlReader.Dispose();
            }
        }

    }

    /// <summary>
    /// Stores the stats of each cube for easy access when writing to databases
    /// </summary>
    public class PlayerStatus
    {/// <summary>
     /// Name of the cube that ate this
     /// </summary>
        public string EatenBy
        {
            get; set;
        }

        /// <summary>
        /// Max rank, initialize to high number
        /// </summary>
        public int MaxRank
        {
            get; set;
        }
        /// <summary>
        /// name of the cube
        /// </summary>
        public string name
        {
            get; set;
        }

        //time the cube is alive
        public TimeSpan AliveTime
        {
            get; set;
        }
        //Time game starts
        public DateTime StartTime
        {
            get; set;
        }
        //time game ends
        public DateTime EndTime
        {
            get; set;
        }
        //max mass
        public double MaxMass
        {
            get; set;
        }
        //List of all cubes eaten
        public HashSet<string> EatenNames
        {
            get; set;
        }
        public Socket Socket
        {
            get; set;
        }

        public int TeamID { get; set; }
        private int cubesEaten;
        public int CubesEaten
        {
            get
            {
                return cubesEaten;
            }
        }
        public PlayerStatus(string name, int teamID, Socket socket)
        {
            this.StartTime = DateTime.Now;
            this.MaxMass = 0;
            this.EatenNames = new HashSet<string>();
            this.name = name;
            this.TeamID = teamID;
            this.Socket = socket;
            cubesEaten = 0;
            EndTime = new DateTime(0);
            MaxRank = 1000;
            EatenBy = name;
        }
        /// <summary>
        /// increases the number of cubes the player has eaten
        /// </summary>
        public void addCubeEaten()
        {
            cubesEaten++;
        }
        /// <summary>
        /// sets when the game end and how long the player was alive
        /// </summary>
        public void endTime()
        {
            this.EndTime = DateTime.Now;
            this.AliveTime = EndTime.Subtract(StartTime);
        }

        /// <summary>
        /// change maxmass if newMess is larger
        /// </summary>
        /// <param name="newMass"></param>
        public void testNewMass(double newMass)
        {
            if (newMass > MaxMass)
            {
                MaxMass = newMass;
            }
        }

        /// <summary>
        /// change max rank if new rank is lower
        /// </summary>
        /// <param name="rank"></param>
        public void testNewRank(int rank)
        {
            if (rank < MaxRank)
            {
                MaxRank = rank;
            }
        }

        /// <summary>
        /// adds player cubes eaten
        /// </summary>
        /// <param name="name"></param>
        public void addEatenName(string name)
        {
            if (!this.EatenNames.Contains(name))
            {
                EatenNames.Add(name);
            }
        }
    }
}
