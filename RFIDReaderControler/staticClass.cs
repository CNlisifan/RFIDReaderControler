using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace RFIDReaderControler
{
    public static class staticClass
    {
        public static DateTime timeBase = DateTime.Now;
        public static string configFilePath = "app.yap";
        public static string PicturePath = @"商品图片\";
        public static string currentDBConnectString = string.Empty;
        //public static IObjectContainer db = Db4oFactory.OpenFile(configFilePath);
        public static string strServerIP = "192.168.1.100";
        public static int iServePort = 5000;

        public static string restServerIP = "192.168.1.100";
        public static string restServerPort = "9002";
        public static int interval = 15000;

        public static string readerTableName = "tbreader";

        public static Dictionary<string, ReaderInfo> readerDic = new Dictionary<string, ReaderInfo>();

        public static void refresh_reader_dic()
        {
            try
            {
                readerDic.Clear();
                DataTable dt = nsConfigDB.ConfigDB.getTable(staticClass.readerTableName);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    ReaderInfo ri = new ReaderInfo(
                                                   dr["key"].ToString(),
                                                   dr["ip"].ToString(),
                                                   dr["port"].ToString(),
                                                   dr["flag"].ToString(),
                                                   dr["sendType"].ToString(),
                                                   dr["interval"].ToString(),
                                                   dr["targetIP"].ToString()
                                                   );
                    staticClass.readerDic.Add(dr["key"].ToString(), ri);
                }
            }
            catch
            {

            }
        }
    }

}
