using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using RFIDReaderControler;

namespace Server
{
    //public delegate void invokeCtlString(string str);

    public class UDPServer
    {
        public ManualResetEvent Manualstate = new ManualResetEvent(true);
        public StringBuilder sbuilder = new StringBuilder();
        public Socket serverSocket;
        byte[] byteData = new byte[1024];
        int port = 9001;
        public void stopUDPListening()
        {
            if (serverSocket != null)
            {
                try
                {
                    serverSocket.Close();
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(
                        string.Format("UDPServer.stopUDPListening  ->  = {0}"
                        , ex.Message));
                }
            }
        }
        public Socket startUDPListening(int iPort)
        {
            this.port = iPort;
            try
            {
                //if (serverSocket == null)
                {
                    //We are using UDP sockets
                    serverSocket = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram, ProtocolType.Udp);
                    IPAddress ip = IPAddress.Parse(this.GetLocalIP4());
                    IPEndPoint ipEndPoint = new IPEndPoint(ip, this.port);
                    //                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);
                    Debug.WriteLine(
                        string.Format("UDPServer.startUDPListening  -> ip = {0}   port = {1}"
                        , staticClass.strServerIP, staticClass.iServePort));
                    //Bind this address to the server
                    serverSocket.Bind(ipEndPoint);
                    //防止客户端强行中断造成的异常
                    long IOC_IN = 0x80000000;
                    long IOC_VENDOR = 0x18000000;
                    long SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

                    byte[] optionInValue = { Convert.ToByte(false) };
                    byte[] optionOutValue = new byte[4];
                    serverSocket.IOControl((int)SIO_UDP_CONNRESET, optionInValue, optionOutValue);
                }

                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                //The epSender identifies the incoming clients
                EndPoint epSender = (EndPoint)ipeSender;

                //Start receiving data
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
                    SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);


                //**********************************************************************

            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    string.Format("UDPServer.startUDPListening  -> error = {0}"
                    , ex.Message));
            }
            return serverSocket;
        }
        public void OnReceive(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = (EndPoint)ipeSender;

                serverSocket.EndReceiveFrom(ar, ref epSender);

                string strReceived = Encoding.UTF8.GetString(byteData);

                //Debug.WriteLine(
                //    string.Format("UDPServer.OnReceive  -> received = {0}"
                //    , strReceived));

                Array.Clear(byteData, 0, byteData.Length);
                int i = strReceived.IndexOf("\0");
                Manualstate.WaitOne();
                Manualstate.Reset();
                //todo here should deal with the received string
                sbuilder.Append(strReceived.Substring(0, i));
                Manualstate.Set();

                //Start listening to the message send by the user
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender,
                    new AsyncCallback(OnReceive), epSender);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    string.Format("UDPServer.OnReceive  -> error = {0}"
                    , ex.Message));
            }
        }
        string GetLocalIP4()
        {
            IPAddress ipAddress = null;
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
            {
                ipAddress = ipHostInfo.AddressList[i];
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    break;
                }
                else
                {
                    ipAddress = null;
                }
            }
            if (null == ipAddress)
            {
                return null;
            }
            return ipAddress.ToString();
        }
    }
}
