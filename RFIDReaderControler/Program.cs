using System;
using System.Collections.Generic;
using System.Windows.Forms;

/*
 本项目的目的：
 * 控制与本应用相关联的rfid读写设备（打开，关闭等等）
 * 
 * 将从设备读到的标签按照协议要求发送到指定目的地
 * 
 * 
 
 
 */


namespace RFIDReaderControler
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            programInitial();

            Application.Run(new frmMain());
        }

        private static void programInitial()
        {
            nsConfigDB.ConfigItem item = new nsConfigDB.ConfigItem(staticClass.readerTableName);
            item.AddColumn("ip");
            item.AddColumn("port");
            item.AddColumn("flag");
            item.AddColumn("sendType");
            item.AddColumn("interval");
            item.AddColumn("targetIP");
            nsConfigDB.ConfigDB.addConfigItem(item);


            object o = nsConfigDB.ConfigDB.getConfig("restPort");
            if (o != null)
            {
                staticClass.restServerPort = o.ToString();
            }
            o = nsConfigDB.ConfigDB.getConfig("restIP");
            if (o != null)
            {
                staticClass.restServerIP = o.ToString();
            }


        }
    }
}
