using System;
using System.Collections.Generic;
using System.Text;
using RFIDReaderControler;

namespace RestAPI
{
    public delegate void deleControlInvoke(object o);

    public class RestUrl
    {
        public static string RestAddress = "http://" + staticClass.restServerIP + ":" + staticClass.restServerPort + "/index.php/";
        //public static string RestAddress = "http://192.168.0.82:9002/index.php/";

        public static string getCommand = RestAddress + "RFIDReader/Communication/getCommand";
        public static string addCommand = RestAddress + "RFIDReader/Communication/addCommand";

        public static string addScanedTag = RestAddress + "RFIDReader/Reader/addScanTag";
        public static string addScanedTags = RestAddress + "RFIDReader/Reader/addScanTags";
        public static string getScanedTags = RestAddress + "RFIDReader/Reader/getScanTags";

    }
}
