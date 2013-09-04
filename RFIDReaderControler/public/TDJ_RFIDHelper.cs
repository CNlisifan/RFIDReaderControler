using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Timers;
using System.Threading;
using RFIDReaderControler;


namespace Server
{
    public enum emuTagInfoFormat
    {
        标准, 简化, 无
    }
    /// <summary>
    /// 将标签信息解析后通过此类中转处理
    /// </summary>
    public class TagInfo
    {
        public int readCount = 0;
        public string antennaID = string.Empty;
        public string tagType = string.Empty;
        public string epc = string.Empty;
        public string getTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //public int milliSecond = DateTime.Now.Millisecond;

        public long milliSecond = 0;

        public string toString()
        {
            string str = string.Empty;
            str = string.Format("ant -> {0} | count -> {1} | epc -> {2}",
                                this.antennaID, this.readCount, this.epc);

            return str;
        }
        public static TagInfo Parse(string tagInfo)
        {
            //Debug.WriteLine("TagInfo -> Parse  " + tagInfo);
            TagInfo ti = new TagInfo();
            try
            {

                emuTagInfoFormat format = emuTagInfoFormat.标准;
                if (tagInfo.Substring(0, 4) == "Disc")
                {
                    format = emuTagInfoFormat.标准;
                }
                else
                {
                    format = emuTagInfoFormat.简化;
                }
                /*
    Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:42, Count:00019, Ant:02, Type:04, Tag:300833B2DDD906C000000000 
             
                 */
                if (format == emuTagInfoFormat.标准)
                {
                    string[] arrays = tagInfo.Split(',');
                    if (arrays.Length < 6)//信息不全
                    {
                        return null;
                    }

                    string temp = arrays[1];
                    ti.getTime = temp.Substring(temp.IndexOf(':') + 1).Trim();

                    temp = arrays[2];
                    string strCount = arrays[2].Substring(temp.IndexOf(':') + 1);
                    try
                    {
                        ti.readCount = int.Parse(strCount);

                    }
                    catch (System.Exception ex)
                    {

                    }
                    temp = arrays[3];
                    ti.antennaID = temp.Substring(temp.IndexOf(':') + 1).Trim();

                    //不关注位置或者误读，只关注有没有,所以注释掉
                    //if (ti.antennaID != "01" && ti.antennaID != "02" && ti.antennaID != "04" && ti.antennaID != "08")
                    //{
                    //    return null;
                    //}


                    //int iAntenna = int.Parse(ti.antennaID);
                    //if (iAntenna==3||iAntenna==0||iAntenna==5)
                    //{
                    //}
                    //过滤无效天线编号
                    //if (ti.antennaID == "03" || ti.antennaID == "00" || ti.antennaID == "05" || ti.antennaID == "06" || ti.antennaID == "07")
                    //{
                    //    return null;
                    //}
                    temp = arrays[4];
                    ti.tagType = temp.Substring(temp.IndexOf(':') + 1).Trim();
                    temp = arrays[5];
                    ti.epc = temp.Substring(temp.IndexOf(':') + 1).Trim();
                    DateTime dt = DateTime.Now;
                    TimeSpan ts = dt - staticClass.timeBase;
                    //ti.getTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    //ti.milliSecond = dt.Millisecond;
                    ti.milliSecond = (long)ts.TotalMilliseconds;

                    //  Debug.WriteLine(ti.toString());
                }

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("TagInfo <- Parse error   " + ex.Message);

            }
            return ti;

        }

    }
    //板状读写器操作类
    //实时获取标签信息

    //标签转发最重要的工作就是将读到的标签尽可能快的解析出来
    //可能按天线，也可能不按照天线
    public class TDJ_RFIDHelper
    {
        //两个天线可能有误读，在此时间范围内更正
        int milliSecondDelay = 500;//缓冲时间，毫秒，

        //标签取走的缓冲时间，如果间隔该段时间没有该标签信息更新，则认为标签已经丢失
        int lostDelay = 20 * 60 * 1000;//单位毫秒

        int minReadCount = 0;//最少读取到的标签次数，少于此数目，则认为为误读
        //该表将存储读到的标签信息
        private DataTable tagsInfo = new DataTable("tagsInfo");
        System.Timers.Timer refreshTimer = new System.Timers.Timer();
        //public bool bBusy = false;

        ManualResetEvent resetState = new ManualResetEvent(true);

        public TDJ_RFIDHelper()
        {
            this.tagsInfo.CaseSensitive = true;

            tagsInfo.Columns.Add("epc", typeof(string));
            tagsInfo.Columns.Add("antennaID", typeof(string));
            tagsInfo.Columns.Add("tagType", typeof(string));
            tagsInfo.Columns.Add("readCount", typeof(string));
            tagsInfo.Columns.Add("milliSecond", typeof(long));
            tagsInfo.Columns.Add("getTime", typeof(string));
            tagsInfo.Columns.Add("state", typeof(string));
        }
        public List<TagInfo> getTagList()
        {
            this.resetState.WaitOne();
            this.resetState.Reset();
            List<TagInfo> list = new List<TagInfo>();
            DataRowCollection rows = null;
            rows = this.tagsInfo.Rows;

            foreach (DataRow dr in rows)
            {
                TagInfo ti = new TagInfo();
                ti.epc = dr["epc"].ToString();
                ti.antennaID = dr["antennaID"].ToString();

                list.Add(ti);
            }

            this.tagsInfo.Rows.Clear();
            this.resetState.Set();

            return list;
        }
        public void clearTags()
        {
            this.resetState.WaitOne();
            this.resetState.Reset();

            this.tagsInfo.Rows.Clear();

            this.resetState.Set();
        }
        public void tagDeleted()
        {
            //this.resetState.WaitOne();
            //this.resetState.Reset();
            DataRow[] rows = null;
            TimeSpan ts = DateTime.Now - staticClass.timeBase;
            long span = (long)(ts.TotalMilliseconds - this.lostDelay);
            if (lostDelay > 0)
            {
                rows = tagsInfo.Select("milliSecond < " + span.ToString() + "");
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        Debug.WriteLine(string.Format("tagDeleted :  span -> {0}   millis -> {1} epc -> {2}", span.ToString(), rows[i]["milliSecond"], rows[i]["epc"]));

                        //rows[i]["state"] = "deleted";
                        this.tagsInfo.Rows.Remove(rows[i]);
                    }

                    this.outputTagTable();
                }
            }
            //this.resetState.Set();

        }

        //接收串口或者其它方式接收到的标签信息，
        public void ParseDataToTag(string data)
        {
            this.tagDeleted();
            //this.bBusy = true;
            if (data == null || data.Length <= 0)
            {
                return;
            }
            this.stringBuilder.Append(data);
            int tagLength = 110;//每条数据的标准长度为110
            string temp1 = this.stringBuilder.ToString();
            //Debug.WriteLine(temp1);
            int start = temp1.IndexOf("Disc:");
            if (start < 0)
            {
                return;
            }
            int tempStart = start;
            int lastDiscIndex = start;
            while (true)//找到最后一个Disc，并且其后有满格式的数据，即长度为110
            {
                int DiscIndex = temp1.IndexOf("Disc:", lastDiscIndex + 1);
                if (DiscIndex == -1)
                {
                    break;
                }
                else
                {
                    if (temp1.Length < DiscIndex + tagLength)
                    {
                        break;
                    }
                }
                lastDiscIndex = DiscIndex;
            }
            //int tail = lastDiscIndex + 110;
            int tail = lastDiscIndex - 1;

            string temp = this.stringBuilder.ToString(start, tail - start + 1);
            //string temp = this.stringBuilder.ToString(start, tail + 2 - start + 1);

            this.stringBuilder.Remove(0, tail + 1);//正确数据之前的数据已经没用了

            for (int i = 0; i < temp.Length; i++)
            {
                string tagInfo = string.Empty;
                int startIndex = temp.IndexOf("Disc", i);
                string restStr = "no rest";
                if (startIndex >= 0)
                {
                    restStr = temp.Substring(startIndex);
                }
                //Debug.WriteLine(
                //    string.Format("TDJ_RFIDHelper.ParseDataToTag  -> startIndex = {0} lastDiscIndex = {1}  rest temp = {2}"
                //    , startIndex, lastDiscIndex, restStr));
                if (startIndex == -1)
                {
                    return;
                }
                if (temp.Length - startIndex >= tagLength)
                {

                    tagInfo = temp.Substring(startIndex, tagLength);
                }
                else
                {
                    return;
                }

                TagInfo ti = TagInfo.Parse(tagInfo);
                if (null != ti)
                {

                    this.AddNewTag2Table(ti);
                }

                i = startIndex + tagLength;
            }
        }
        public void ReceiveNewTag()
        {
            string temp1 = this.stringBuilder.ToString();
            int start = temp1.IndexOf("Disc:");
            int end = temp1.IndexOf("\r\n");
            if (start >= 0 && end > 110 && start < end)
            {
                string temp = this.stringBuilder.ToString(start, end + 2);
                this.stringBuilder.Remove(0, end + 2);

                for (int i = 0; i < temp.Length; i++)
                {
                    string tagInfo = string.Empty;
                    int startIndex = temp.IndexOf("Disc", i);
                    if (startIndex == -1)
                    {
                        return;
                    }
                    if (temp.Length - startIndex > 110)
                    {
                        tagInfo = temp.Substring(startIndex, 110);
                    }
                    else
                    {
                        return;
                    }
                    TagInfo ti = TagInfo.Parse(tagInfo);
                    if (null == ti)
                    {
                        return;
                    }
                    this.AddNewTag2Table(ti);
                    i = startIndex + 110;
                }
            }
        }
        public void ReceiveNewTag(string tagInfo)
        {
            TagInfo ti = TagInfo.Parse(tagInfo);
            if (null == ti)
            {
                return;
            }
            this.AddNewTag2Table(ti);
        }
        public void outputTagTable()
        {
            Debug.WriteLine("*******************************************************************");
            DataRowCollection rows = this.tagsInfo.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow row = rows[i];
                Debug.WriteLine(string.Format("epc -> {0}  antennaID -> {1}  readCount = {2}", row["epc"].ToString(), row["antennaID"].ToString(), row["readCount"].ToString()));
            }

        }

        /// <summary>
        /// 将新解析完的标签尝试添加到列表中
        /// 首先要检查列表中是否已经有新标签的epc，如果已经有标签epc，查看天线编号是否一致，如果天线编号一致，则替换原有的
        /// 标签信息，如果天线编号不一致，则查看是否在缓冲时间段内，如果是则表明这可能是误读，要用读取次数多的标签信息代替
        /// 读取次数少的标签信息；如果不在缓冲时间段内，则认为标签已经改变了位置
        /// 因此，导致表内信息改变的情况有以下几种：
        /// 1 epc不存在，加到表中
        /// 2 epc存在，且天线编号一致，新的代替旧的
        /// 3 epc存在，缓冲时间段内，天线编号不一致，用读取次数多的代替少的
        /// 4 epc存在，非缓冲时间段内，天线编号不一致，新的代替旧的
        /// </summary>
        /// <param name="ti"></param>
        public void AddNewTag2Table(TagInfo ti)
        {
            //
            DataRow[] rows = null;

            if (ti.readCount < this.minReadCount)
            {
                return;
            }
            this.resetState.WaitOne();
            this.resetState.Reset();
            rows = tagsInfo.Select("epc = '" + ti.epc + "'");
            if (rows.Length <= 0)//epc不存在，加到表中
            {
                this.tagsInfo.Rows.Add(new object[] { ti.epc, ti.antennaID, ti.tagType, ti.readCount, ti.milliSecond, ti.getTime, "new" });
            }
            else
            {
                rows[0]["milliSecond"] = ti.milliSecond;
                //if (ti.antennaID == rows[0]["antennaID"].ToString())//天线编号一致
                //{
                //    this.tagsInfo.Rows.Remove(rows[0]);
                //    this.tagsInfo.Rows.Add(new object[] { ti.epc, ti.antennaID, ti.tagType, ti.readCount, ti.milliSecond, ti.getTime, "new" });
                //}
                //else//天线编号不一致
                //{
                //    {
                //        //没超出一秒，有可能超出缓冲时间，需要比较毫秒
                //        long oldM = 0;
                //        try
                //        {
                //            oldM = long.Parse(rows[0]["milliSecond"].ToString());
                //        }
                //        catch (System.Exception ex)
                //        {
                //            Debug.WriteLine(ex.Message);
                //        }
                //        if ((ti.milliSecond - oldM) > this.milliSecondDelay)//超出缓冲时间
                //        {
                //            this.tagsInfo.Rows.Remove(rows[0]);
                //            this.tagsInfo.Rows.Add(new object[] { ti.epc, ti.antennaID, ti.tagType, ti.readCount, ti.milliSecond, ti.getTime, "new" });
                //        }
                //        else
                //        {
                //            //读取次数多的代替少的
                //            int oldC = 0;
                //            try
                //            {
                //                oldC = int.Parse(rows[0]["readCount"].ToString());
                //            }
                //            catch (System.Exception ex)
                //            {
                //                Debug.WriteLine(ex.Message);

                //            }
                //            if (ti.readCount > oldC)
                //            {
                //                this.tagsInfo.Rows.Remove(rows[0]);
                //                this.tagsInfo.Rows.Add(new object[] { ti.epc, ti.antennaID, ti.tagType, ti.readCount, ti.milliSecond, ti.getTime, "new" });
                //            }
                //        }
                //    }
                //}
            }
            this.resetState.Set();

            //this.outputTagTable();
        }

        List<byte> maxbuf = new List<byte>();
        StringBuilder stringBuilder = new StringBuilder();

    }
}
