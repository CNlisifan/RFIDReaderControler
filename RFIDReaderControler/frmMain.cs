using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using httpHelper;
using RestAPI;
using System.Diagnostics;
using invokePhpRestDemo;
using RfidReader;
using System.IO.Ports;
using Server;

namespace RFIDReaderControler
{
    public partial class frmMain : Form
    {


        #region 成员
        //public static DateTime timeBase = DateTime.Now;
        UDPServer __udpServer = new UDPServer();
        //存储接收到命令信息
        List<string> flagList = new List<string>();
        Timer __timer = null;//控制器运作的引擎
        HttpWebConnect __httpHelper = new HttpWebConnect();
        HttpWebConnect __httpHelperAddTags = new HttpWebConnect();
        bool bRunning = false;//是否正在运行
        string lastUpdateTimeStamp = string.Empty;
        //要上传的标签的缓存池
        DataTable __dtTagTemp = new DataTable();
        long __tagUploadInterval = 15000;// 对标签进行缓存，同一个标签要在一定时间之后才能重新上传
        TDJ_RFIDHelper __2300helper = new TDJ_RFIDHelper();

        rfidOperateUnitBase operaterGetTag = null;
        rfidOperateUnitBase operaterStopGetTag = null;
        IDataTransfer dataTransfer = null;
        SerialPort comport = null;
        Timer __reader2300Timer;//操作 reader2300 之用

        #endregion


        public frmMain()
        {
            InitializeComponent();

            this.FormClosing += new FormClosingEventHandler(frmMain_FormClosing);
            this.lblStatus.Text = string.Empty;

            __timer = new Timer();
            __timer.Interval = 1000;
            __timer.Tick += new EventHandler(__timer_Tick_getCommand);

            __reader2300Timer = new Timer();
            __reader2300Timer.Interval = 500;
            __reader2300Timer.Tick += new EventHandler(_timer_get2300Tag);

            __httpHelper.RequestCompleted += new deleGetRequestObject(__httpHelper_RequestCompleted_getCommand);
            this.__httpHelperAddTags.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted);


            __dtTagTemp.Columns.Add("tag", typeof(string));
            __dtTagTemp.Columns.Add("time", typeof(long));
            //__dtTagTemp.Columns.Add("tagtime", typeof(long));

            this.Shown += new EventHandler(frmMain_Shown);

            //this.flagList.Add("test");
        }

        void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool bQuit = true;
            Dictionary<string, ReaderInfo>.ValueCollection items = staticClass.readerDic.Values;
            foreach (ReaderInfo ri in items)
            {
                if (ri.bRunning == true)
                {
                    bQuit = false;
                    break;
                }
            }
            if (bQuit==false)
            {
                DialogResult result = MessageBox.Show("正在接收读写器数据，确定退出吗？", "信息提示", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        void frmMain_Shown(object sender, EventArgs e)
        {
            try
            {
                staticClass.refresh_reader_dic();

                //sysParaConfig sysPara = new sysParaConfig(this.Controls, "系统参数", null);
                //staticClass.restServerIP = sysPara.__ip;
                //staticClass.restServerPort = sysPara.__port;
                //staticClass.interval = int.Parse(sysPara.__interval);
                //this.__tagUploadInterval = staticClass.interval;


                //UdpConfig udpconfig = new UdpConfig(this.Controls, "UDP参数设置", null);
                //staticClass.iServePort = int.Parse(udpconfig.port);
                //staticClass.strServerIP = udpconfig.ip;
                //__udpServer.startUDPListening(staticClass.iServePort+1);
            }
            catch
            {

            }

        }
        
        void _timer_get2300Tag(object sender, EventArgs e)
        {
            __udpServer.Manualstate.WaitOne();
            __udpServer.Manualstate.Reset();
            string str = __udpServer.sbuilder.ToString();
            __udpServer.sbuilder.Remove(0, str.Length);
            List<TagInfo> taglist = __2300helper.getTagList();
            foreach (TagInfo ti in taglist)
            {
                this.lblStatus.Text = "检测到标签 " + ti.epc + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.appendLog(this.lblStatus.Text);

                this.addTagsToServer(ti.epc);
            }
            __2300helper.ParseDataToTag(str);
            if (str != null && str.Length > 0)
            {
                //Debug.WriteLine(
                //    string.Format(".  _timer_get2300Tag -> string = {0}"
                //    , str));
            }
            __udpServer.Manualstate.Set();



        }
        //通过网络读取到命令后，对命令进行解析
        void __httpHelper_RequestCompleted_getCommand(object o)
        {
            deleControlInvoke dele = delegate(object oc)
            {
                string cmds = (string)oc;
                object olist = fastJSON.JSON.Instance.ToObjectList(cmds, typeof(List<CommandProtocol>), typeof(CommandProtocol));
                foreach (CommandProtocol c in (List<CommandProtocol>)olist)
                {
                    //这里要针对命令做出相应的动作
                    commandDispose(c);
                }
            };
            this.Invoke(dele, o);
        }
        void commandDispose(CommandProtocol _cmd)
        {
            if (_cmd == null)
            {
                return;
            }
            if (string.Compare(this.lastUpdateTimeStamp, _cmd.timeStamp, true) < 0)//如果命令的时间较晚
            {
                this.lastUpdateTimeStamp = _cmd.timeStamp;
            }
            switch (_cmd.command.ToLower().Trim())
            {
                #region 之前处理命令的代码

                case "open_rmu900":
                    if (!this.flagList.Contains(_cmd.flag))
                    {
                        this.flagList.Add(_cmd.flag);
                    }
                    this.lblStatus.Text = "接收到命令 " + _cmd.command + "  " + _cmd.timeStamp + "  flag = " + _cmd.flag;
                    this.appendLog(this.lblStatus.Text);

                    if (this.operaterGetTag == null)
                    {
                        operaterGetTag = new rfidOperateUnitInventoryTag(dataTransfer);
                        operaterGetTag.registeCallback(new deleRfidOperateCallback(rfidCallback));
                    }

                    operaterGetTag.OperateStart();
                    break;
                case "close_rmu900":
                    if (this.flagList.Contains(_cmd.flag))
                    {
                        this.flagList.Remove(_cmd.flag);
                    }
                    this.lblStatus.Text = "接收到命令 " + _cmd.command + "  " + _cmd.timeStamp + "  flag = " + _cmd.flag;
                    this.appendLog(this.lblStatus.Text);

                    if (null == this.operaterStopGetTag)
                    {
                        operaterStopGetTag = new rfidOperateUnitStopInventoryTag(dataTransfer);
                    }
                    this.operaterStopGetTag.OperateStart();
                    break;
                case "open_reader2300":
                    if (!this.flagList.Contains(_cmd.flag))
                    {
                        this.flagList.Add(_cmd.flag);
                    }
                    try
                    {
                        this.lblStatus.Text = "接收到命令 " + _cmd.command + "  " + _cmd.timeStamp + "  flag = " + _cmd.flag;
                        this.appendLog(this.lblStatus.Text);
                        //UdpConfig udpconfig = new UdpConfig(this.Controls, "UDP参数设置", null);
                        //staticClass.iServePort = int.Parse(udpconfig.port);
                        //staticClass.strServerIP = udpconfig.ip;
                        //UDPServer.startUDPListening();

                        //清空之前的缓存
                        __udpServer.Manualstate.WaitOne();
                        __udpServer.Manualstate.Reset();
                        string str = __udpServer.sbuilder.ToString();
                        __udpServer.sbuilder.Remove(0, str.Length);
                        __2300helper.clearTags();
                        __udpServer.Manualstate.Set();


                        __reader2300Timer.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "SGSServerUDP",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case "close_reader2300":
                    if (this.flagList.Contains(_cmd.flag))
                    {
                        this.flagList.Remove(_cmd.flag);
                    }
                    try
                    {
                        this.lblStatus.Text = "接收到命令 " + _cmd.command + "  " + _cmd.timeStamp + "  flag = " + _cmd.flag;
                        this.appendLog(this.lblStatus.Text);

                        __reader2300Timer.Enabled = false;
                        //UDPServer.stopUDPListening();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "SGSServerUDP",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                #endregion

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
            long gap = (long)tsGap.TotalMilliseconds - this.__tagUploadInterval;//距离现在差距缓冲时间间隔的时间点
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
            foreach (string flag in this.flagList)
            {
                //Debug.WriteLine(
                //    string.Format("frmMain.addTagstoServer  -> web add  = flag = {0}  epc = {1}"
                //    , flag, tag));
                tagID tagIDTag = new tagID(tag, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), flag);
                this.lblStatus.Text = "发送标签 " + tagIDTag.tag + " " + tagIDTag.startTime;
                this.appendLog(this.lblStatus.Text);

                string jsonString = fastJSON.JSON.Instance.ToJSON(tagIDTag);
                HttpWebConnect helper = new HttpWebConnect();
                helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted);
                string url = RestUrl.addScanedTag;
                helper.TryPostData(url, jsonString);
            }

        }

        void helper_RequestCompleted(object o)
        {
            deleControlInvoke dele = delegate(object otag)
            {
                try
                {
                    tagID tag = (tagID)fastJSON.JSON.Instance.ToObject((string)otag, typeof(tagID));
                    this.lblStatus.Text = "发送标签 " + tag.tag + " 成功" + "  " + tag.startTime;
                    Debug.WriteLine("helper_RequestCompleted -> " + this.lblStatus.Text);

                    this.appendLog(this.lblStatus.Text);

                }
                catch (System.Exception ex)
                {
                }
            };
            this.Invoke(dele, o);
        }
        //将扫描到的标签数据上传到服务器
        void rfidCallback(object o)
        {
            operateMessage msg = (operateMessage)o;
            if (msg.status != "fail")
            {
                string epc = (string)msg.message;
                Debug.WriteLine(
                    string.Format("frmMain.addTagstoServer  ->  = {0}"
                    , epc));
                this.addTagsToServer(epc);
            }
        }

        void helper_RequestCompleted_addCommand(object o)
        {
            string strProduct = (string)o;
            Debug.WriteLine(
                string.Format("frmMain.helper_RequestCompleted_addCommand-> response = {0}"
                , strProduct));
            CommandProtocol u2 = fastJSON.JSON.Instance.ToObject<CommandProtocol>(strProduct);
            if (u2.state == "ok")
            {
                Debug.WriteLine(
                    string.Format("frmMain.helper_RequestCompleted_addCommand  ->  = {0}"
                    , "ok"));
            }
            else
            {
                Debug.WriteLine(
                    string.Format("frmMain.helper_RequestCompleted_addCommand  ->  = {0}"
                    , "failed"));
            }
        }


        void appendLog(string log)
        {

        }
        #region button事件处理
        private void btnStart_Click(object sender, EventArgs e)
        {
            //启动控制器
            //启动以后，控制器会不断的访问网络以获取最新的命令
            if (this.bRunning)
            {
                this.__reader2300Timer.Enabled = false;
                this.__timer.Enabled = false;
                this.bRunning = false;
                this.flagList.Clear();
            }
            else
            {
                this.bRunning = true;
                //重置状态
                this.lastUpdateTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.flagList.Clear();

                this.__timer.Enabled = true;
                this.lblStatus.Text = "";

                this.__reader2300Timer.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CommandProtocol cp = new CommandProtocol("open_reader2300", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "inventory");
            //CommandProtocol cp = new CommandProtocol("open_rmu900", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "inventory");
            string jsonString = fastJSON.JSON.Instance.ToJSON(cp);
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addCommand);
            string url = RestUrl.addCommand;
            helper.TryPostData(url, jsonString);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CommandProtocol cp = new CommandProtocol("close_reader2300", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "inventory");
            //CommandProtocol cp = new CommandProtocol("close_rmu900", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "inventory");
            string jsonString = fastJSON.JSON.Instance.ToJSON(cp);
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addCommand);
            string url = RestUrl.addCommand;
            helper.TryPostData(url, jsonString);
        }
        void __timer_Tick_getCommand(object sender, EventArgs e)
        {
            string url = RestUrl.getCommand;
            CommandProtocol cmd = new CommandProtocol(string.Empty, this.lastUpdateTimeStamp, string.Empty);
            string jsonString = fastJSON.JSON.Instance.ToJSON(cmd);
            __httpHelper.TryPostData(url, jsonString);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //this.addTagstoServer();
        }

        private void 退出TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 系统配置CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSysSettings frm = new frmSysSettings();
            frm.ShowDialog();
        }

        private void 打开读写器SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStartReader frm = new frmStartReader();
            frm.ShowDialog();
            //CommandProtocol cp = new CommandProtocol("open_reader2300", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "inventory");
            ////CommandProtocol cp = new CommandProtocol("open_rmu900", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "inventory");
            //string jsonString = fastJSON.JSON.Instance.ToJSON(cp);
            //HttpWebConnect helper = new HttpWebConnect();
            //helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addCommand);
            //string url = RestUrl.addCommand;
            //helper.TryPostData(url, jsonString);
        }

        private void 关闭读写器CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandProtocol cp = new CommandProtocol("close_reader2300", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "inventory");
            //CommandProtocol cp = new CommandProtocol("close_rmu900", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "inventory");
            string jsonString = fastJSON.JSON.Instance.ToJSON(cp);
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addCommand);
            string url = RestUrl.addCommand;
            helper.TryPostData(url, jsonString);
        }

        private void 读写器管理MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReaderMngment frm = new frmReaderMngment();
            frm.ShowDialog();
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

    }
}
