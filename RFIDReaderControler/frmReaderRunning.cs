using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using httpHelper;
using Server;
using invokePhpRestDemo;
using RestAPI;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace RFIDReaderControler
{
    public partial class frmReaderRunning : Form
    {
        #region 成员
        string __reader_name;
        UDPServer __udpServer = new UDPServer();
        //存储接收到命令信息
        List<string> flagList = new List<string>();
        HttpWebConnect __httpHelperAddTags = new HttpWebConnect();
        //要上传的标签的缓存池
        DataTable __dtTagTemp = new DataTable();
        TDJ_RFIDHelper __2300helper = new TDJ_RFIDHelper();

        Timer __reader2300Timer;//操作 reader2300 之用
        ReaderInfo __reader_info = null;
        frmStartReader __frmReader = null;
        public Socket clientSocket = null; //The main client socket
        //public EndPoint epServer;   //The EndPoint of the server
        List<EndPoint> endpoint_list = new List<EndPoint>();
        #endregion
        public frmReaderRunning(string _reader_name, frmStartReader frmReader)
        {
            InitializeComponent();
            this.__reader_name = _reader_name;
            this.__frmReader = frmReader;
            this.Text = "阅读器" + this.__reader_name;
            this.Shown += new EventHandler(frmReaderRunning_Shown);
            this.FormClosing += new FormClosingEventHandler(frmReaderRunning_FormClosing);

            __reader2300Timer = new Timer();
            __reader2300Timer.Interval = 500;
            __reader2300Timer.Tick += new EventHandler(_timer_get2300Tag);

            __dtTagTemp.Columns.Add("tag", typeof(string));
            __dtTagTemp.Columns.Add("time", typeof(long));


        }
        void _timer_get2300Tag(object sender, EventArgs e)
        {
            __udpServer.Manualstate.WaitOne();
            __udpServer.Manualstate.Reset();
            string str = __udpServer.sbuilder.ToString();
            __udpServer.sbuilder.Remove(0, str.Length);
            if (this.__reader_info.sendType == ReaderInfo.sendTypeUDP)
            {
                byte[] byteData = Encoding.UTF8.GetBytes(str);
                foreach (EndPoint ep in this.endpoint_list)
                {
                    clientSocket.BeginSendTo(byteData, 0,
                                                byteData.Length, SocketFlags.None,
                                                ep, new AsyncCallback(OnSend), null);
                }
                string log = "接收到读写器数据";
                this.appendLog(log);

            }
            else
            {
                List<TagInfo> taglist = __2300helper.getTagList();
                foreach (TagInfo ti in taglist)
                {
                    string log = "检测到标签 " + ti.epc;
                    this.appendLog(log);

                    this.addTagsToServer(ti.epc);
                }
                __2300helper.ParseDataToTag(str);
                if (str != null && str.Length > 0)
                {
                    //Debug.WriteLine(
                    //    string.Format(".  _timer_get2300Tag -> string = {0}"
                    //    , str));
                }
            }

            __udpServer.Manualstate.Set();
        }
        private void OnSend(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    string.Format("frmReaderRunning.OnSend  ->  = {0}"
                    , ex.Message));
            }
        }
        void addTagsToServer(string tag)
        {

            //Debug.WriteLine("addTagsToServer -> " + tag);
            //读到一个新标签后，检查缓存池，会有三种情况：
            //1 该epc尚未加入到缓存池中
            //2 该epc已经加入到缓存池中，但是在缓冲时间之内
            //3 epc在缓冲池中，且储存时间已经超过缓冲时间
            DataRow[] rows = null;
            TimeSpan tsGap = DateTime.Now - staticClass.timeBase;
            long gap = (long)tsGap.TotalMilliseconds - this.__reader_info.interval;//距离现在差距缓冲时间间隔的时间点
            rows = __dtTagTemp.Select("time > " + gap + " and tag = '" + tag + "'");//只要大于这个时间点说明离现在近
            if (rows.Length > 0)//说明tag等于epc的那个标签已经尚在缓冲时间之内，不能重新上传
            {
                return;
            }
            else
            {
                //如果上不存在epc，则要添加到缓冲池中
                rows = null;
                rows = this.__dtTagTemp.Select("tag = '" + tag + "'");
                if (rows.Length <= 0)
                {
                    this.__dtTagTemp.Rows.Add(new object[] { tag, tsGap.TotalMilliseconds });
                }
                else
                {
                    //这里还剩下的只有 已经加入了缓冲池，但是已经超过缓冲时间的标签，因此只需更新读取时间即可
                    rows[0]["time"] = tsGap.TotalMilliseconds;//记录从应用启动到现在的毫秒数
                }
            }
            tagID tagIDTag = new tagID(tag, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), this.__reader_info.flag);
            string log = "发送标签 " + tagIDTag.tag + " " + tagIDTag.startTime;
            this.appendLog(log);

            string jsonString = fastJSON.JSON.Instance.ToJSON(tagIDTag);
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted);
            string url = RestUrl.addScanedTag;
            helper.TryPostData(url, jsonString);
            //foreach (string flag in this.flagList)
            //{
            //    //Debug.WriteLine(
            //    //    string.Format("frmMain.addTagstoServer  -> web add  = flag = {0}  epc = {1}"
            //    //    , flag, tag));
            //    tagID tagIDTag = new tagID(tag, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), flag);
            //    string log = "发送标签 " + tagIDTag.tag + " " + tagIDTag.startTime;
            //    this.appendLog(log);

            //    string jsonString = fastJSON.JSON.Instance.ToJSON(tagIDTag);
            //    HttpWebConnect helper = new HttpWebConnect();
            //    helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted);
            //    string url = RestUrl.addScanedTag;
            //    helper.TryPostData(url, jsonString);
            //}

        }
        void helper_RequestCompleted(object o)
        {
            deleControlInvoke dele = delegate(object otag)
            {
                try
                {
                    tagID tag = (tagID)fastJSON.JSON.Instance.ToObject((string)otag, typeof(tagID));
                    string log = "发送标签 " + tag.tag + " 成功" + "  " + tag.startTime;

                    this.appendLog(log);

                }
                catch (System.Exception ex)
                {
                }
            };
            this.Invoke(dele, o);
        }
        void appendLog(string log)
        {
            if (this.txtLog.Text != null && this.txtLog.Text.Length > 4096)
            {
                this.txtLog.Text = string.Empty;
            }
            this.txtLog.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + log + "\r\n" + this.txtLog.Text;
        }
        void frmReaderRunning_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.__reader2300Timer.Enabled = false;
            ReaderInfo ri = staticClass.readerDic[this.__reader_name];
            if (ri.socket_server != null)
            {
                ri.socket_server.Close();
            }
            if (ri != null)
            {
                ri.bRunning = false;
            }
            if (this.__frmReader != null)
            {
                this.__frmReader.refreshButtonStart(this.__reader_name);
            }

        }

        void frmReaderRunning_Shown(object sender, EventArgs e)
        {
            this.matrixCircularProgressControl1.Start();
            ReaderInfo ri = staticClass.readerDic[this.__reader_name];
            if (ri != null)
            {
                ri.bRunning = true;
                this.__reader_info = ri;

                if (ri.sendType == ReaderInfo.sendTypeUDP)
                {
                    clientSocket = new Socket(AddressFamily.InterNetwork,
                                SocketType.Dgram, ProtocolType.Udp);
                    List<IP_info> infos = ri.ipList;
                    foreach (IP_info ii in infos)
                    {
                        //IP address of the server machine
                        IPEndPoint ipEndPoint = new IPEndPoint(ii.ipaddress, ii.port);
                        EndPoint epServer = (EndPoint)ipEndPoint;
                        this.endpoint_list.Add(epServer);
                    }
                }
                try
                {
                    Socket socket = __udpServer.startUDPListening(this.__reader_info.port);
                    ri.socket_server = socket;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK);
                    return;
                }
                this.__reader2300Timer.Enabled = true;
            }
        }
    }
}
