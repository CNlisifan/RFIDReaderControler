using System;
using System.Collections.Generic;
using System.Text;

namespace invokePhpRestDemo
{
    public class InventoryCommand
    {
        public static string 扫描入库 = "inventory_input";
        public static string 扫描出库 = "inventory_output";
        public static string 盘点 = "inventory_pandian";
    }
    public class tagID
    {
        public string tag = string.Empty;
        public string startTime = string.Empty;
        public string cmd = string.Empty;
        public string state;
        public tagID() { }
        //public tagID(string _tag, string _time)
        public tagID(string _tag, string _time,string _cmd)
        {
            this.tag = _tag;
            this.startTime = _time;
            this.cmd = _cmd;
            this.state = string.Empty;
        }
    }
    public class scanTagPara
    {
        public string cmd = string.Empty;
        public string startTime = string.Empty;
        public scanTagPara(string _cmd, string _startTime)
        {
            this.cmd = _cmd;
            this.startTime = _startTime;
        }
    }

}
