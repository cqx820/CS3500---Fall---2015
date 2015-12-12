using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Network_Controller
{
    /// <summary>
    /// This is Preserved_State class, mainly used to store state
    /// </summary>
    public class Preserved_State
    {
        /// <summary>
        /// The constant integer which is the size of buffer
        /// </summary>
        public const int bufferSize = 1024;

        /// <summary>
        /// The preserved socket
        /// </summary>
        public Socket socket;

        /// <summary>
        /// The preserved buffer
        /// </summary>
        public byte[] buffer;

        /// <summary>
        /// The string builder
        /// </summary>
        public StringBuilder sb;

        /// <summary>
        /// IasuncResult type delegate "Callback" function
        /// </summary>
        public Action<IAsyncResult> Callback { get; set; }

        public Action<Preserved_State> stateCallback { get; set; }

        /// <summary>
        /// The preserved exception
        /// </summary>
        public Exception exception;

        /// <summary>
        /// 
        /// </summary>
        public TcpListener listener;

        public IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];

        /// <summary>
        /// The Preserved_state constructor
        /// </summary>
        /// <param name="result"></param>
        public Preserved_State(Action<IAsyncResult> result)
        {
            Callback = result;
            sb = new StringBuilder();
            buffer = new byte[bufferSize];
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listener = new TcpListener(ip, 11000);
        }

        /// <summary>
        /// The helper method in Preserved_state class
        /// I chose the option which is processing received information in this class
        /// Add deserilized foodCubes into World class in View class and get foodCubes from World class into View class
        /// </summary>
        /// <returns>The IEnumerable strings</returns>
        public IEnumerable<string> stringProcess()
        {
            string[] infoList = sb.ToString().Split(new string[] { "\n" }, StringSplitOptions.None);
            sb.Clear();
            //string last = infoList[infoList.Length - 1];
            if (!infoList[infoList.Length - 1].EndsWith("\n") || !infoList[infoList.Length - 1].EndsWith(""))
            {
                //sb = new StringBuilder();
                sb.Append(infoList[infoList.Length - 1]);
                infoList[infoList.Length - 1] = "";
            }
            // Return everything in infoList if the final entry ends with \n
            // Else return everything EXCEPT the final entry
            return infoList;
        }
    }
}