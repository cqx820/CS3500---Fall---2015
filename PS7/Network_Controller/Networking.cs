using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Network_Controller
{
    /// <summary>
    /// The static class Networking
    /// </summary>
    /// Author: TFBoys
    public static class Networking
    {
        /// <summary>
        /// Lock
        /// </summary>
        private static object thisLock = new object();
        /// <summary>
        /// The constant integer buffer size
        /// </summary>
        private const int bufferSize = 1024;
        /// <summary>
        /// The constant port number
        /// </summary>
        private const int port = 11000;
        /// <summary>
        /// We are using UTF-8 encoding
        /// </summary>
        public static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

        /// <summary>
        /// This method is used to connect the server
        /// </summary>
        /// <param name="callbackFunction">A function inside the View to be called when a connection is made</param>
        /// <param name="hostname">The name of the server to connect to</param>
        /// <returns></returns>
        public static Socket Connect_to_Server(Action<IAsyncResult> callbackFunction, string hostname)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            //Store the callback function into the Preserved_State class
            Preserved_State state = new Preserved_State(callbackFunction);
            state.socket = socket;
            //Begin connecting
            socket.BeginConnect(hostname, port, new AsyncCallback(Connected_to_Server), state);
            return socket;
        }

        /// <summary>
        /// This function is reference by the BeginConnect method above and is "called" by the OS when the socket connects to the server. 
        /// The "state_in_an_ar_object" object contains a field "AsyncState" which contains the "state" object saved away in the above function.
        /// Once a connection is established the "saved away" callback function needs to called.
        /// Additionally, the network connection should "BeginReceive" expecting more data to arrive(and provide the ReceiveCallback function for this purpose)
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        public static void Connected_to_Server(IAsyncResult state_in_an_ar_object)
        {
            Preserved_State state = (Preserved_State)state_in_an_ar_object.AsyncState;
            Socket client = state.socket;
            try
            {
                client.EndConnect(state_in_an_ar_object);
            }
            catch (Exception e)
            {
                state.exception = e;
            }
            state.Callback(state_in_an_ar_object);
        }

        /// <summary>
        /// The ReceiveCallback method is called by the OS when new data arrives. 
        /// This method should check to see how much data has arrived. If 0, the connection has been closed (presumably by the server). 
        /// On greater than zero data, this method should call the callback function provided above.
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        public static void ReceiveCallback(IAsyncResult state_in_an_ar_object)
        {
            Preserved_State state = (Preserved_State)state_in_an_ar_object.AsyncState;
            byte[] incomingBuffer = state.buffer;
            Socket socket = state.socket;
            try
            {
                int bytesReceived = socket.EndReceive(state_in_an_ar_object);
                //If no byte received, the socket would shut down and return
                if (bytesReceived == 0)
                {
                    socket.Shutdown(SocketShutdown.Send);
                    socket.Close();
                    return;
                }
                else
                {
                    //Decoding the received bytes to strings and append these strings by using string builder
                    state.sb.Append(encoding.GetString(incomingBuffer, 0, bytesReceived));
                    state.Callback(state_in_an_ar_object);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// This is a small helper function that the client View code will call whenever it wants more data. 
        /// Note: the client will probably want more data every time it gets data.
        /// </summary>
        /// <param name="state"></param>
        public static void i_want_more_data(Preserved_State state)
        {
            try
            {
                state.socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// This function (along with it's helper 'SendCallback') will allow a program to send data over a socket. 
        /// This function needs to convert the data into bytes and then send them using socket.BeginSend.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        public static void Send(Socket socket, String data)
        {
            byte[] byteData = encoding.GetBytes(data);
            //Send request to the server and wait for response
            socket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);
        }

        /// <summary>
        /// This function "assists" the Send function. If all the data has been sent, then life is good and nothing needs to be done
        /// </summary>
        /// <param name="result"></param>
        private static void SendCallBack(IAsyncResult result)
        {
            Socket client = (Socket)result.AsyncState;
            int byteSent = client.EndSend(result);
        }
    }
}