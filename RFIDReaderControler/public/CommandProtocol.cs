using System;
using System.Collections.Generic;
using System.Text;

namespace RFIDReaderControler
{
    /// <summary>
    /// 与服务器通讯的协议，通过该协议控制该应用，使得该应用做出相应反应
    /// 其中command为执行的动作，而flag则标记出执行的具体读写器
    /// command 具体值有：open close
    /// flag则根据具体读写器的设置确定具体值
    /// </summary>
    public class CommandProtocol
    {
        public string command = string.Empty;//命令类型，应该根据协议过的命令做出反应
        public string timeStamp = string.Empty;//时间戳
        public string flag = string.Empty;//发送扫描到的标签时附带的标记，是获取扫描到的标签的分类的依据
        public string state = string.Empty;
        public CommandProtocol()
        {

        }
        public CommandProtocol(string _command, string _timeStamp, string _flag)
        {
            this.command = _command;
            this.timeStamp = _timeStamp;
            this.flag = _flag;
        }
    }
}
