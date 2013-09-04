using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;

namespace RFIDReaderControler
{
    public class IP_info
    {
        public IPAddress ipaddress;
        public int port;
        public IP_info(IPAddress _ip, int _port)
        {
            this.ipaddress = _ip;
            this.port = _port;
        }
    }
    public class ReaderInfo
    {
        public static string sendTypeUDP = "UDP";
        public static string sendTypeREST = "REST服务";
        public string name;
        public bool bRunning = false;
        public IPAddress ip;
        public int port;
        public string flag;
        public string sendType;
        public int interval;//ms
        public string ips;
        public List<IP_info> ipList = new List<IP_info>();
        public Socket socket_server;//接收指定读写器信息的udp服务端，不用的时候要关闭

        public ReaderInfo(string _name, string _ip, string _port, string _flag, string _sendtype,
                            string _interval, string _ips)
        {
            this.name = _name;
            try
            {
                this.ip = IPAddress.Parse(_ip);
                if (_port != null && _port.Length > 0)
                {
                    this.port = int.Parse(_port);
                }
                this.flag = _flag;
                this.sendType = _sendtype;
                if (_interval != null && _interval.Length > 0)
                {
                    this.interval = int.Parse(_interval);
                    this.interval *= 1000;
                }
                this.ips = _ips;
                string[] ips = _ips.Split(';');
                for (int i = 0; i < ips.Length; i++)
                {
                    if (ips[i].Length > 0)
                    {
                        string[] ip_and_port_s = ips[i].Split(':');
                        if (ip_and_port_s.Length < 2)
                        {
                            continue;
                        }
                        IPAddress ip = IPAddress.Parse(ip_and_port_s[0]);
                        int port = int.Parse(ip_and_port_s[1]);
                        IP_info ipinfo = new IP_info(ip, port);
                        //IPAddress ip = IPAddress.Parse(ips[i]);
                        this.ipList.Add(ipinfo);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(
                    string.Format("ReaderInfo.ReaderInfo  ->  = {0}"
                    , ex.Message));
            }
        }
    }
}
