using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace RFIDReaderControler
{
    public partial class frmReaderMngment : Form
    {
        string __port = string.Empty;
        string __IP = string.Empty;
        string __flag = string.Empty;
        public frmReaderMngment()
        {
            InitializeComponent();
            this.cmbSendType.Items.Clear();
            this.cmbSendType.Items.Add(ReaderInfo.sendTypeUDP);
            this.cmbSendType.Items.Add(ReaderInfo.sendTypeREST);
            this.cmbSendType.SelectedIndex = 0;

            this.Load += new EventHandler(frmReaderMngment_Load);
        }

        void frmReaderMngment_Load(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            DataTable dt = nsConfigDB.ConfigDB.getTable(staticClass.readerTableName);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                this.listBox1.Items.Add(dt.Rows[i]["key"].ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.checkValidation())
            {
                // 业务逻辑
                // 当前读写器设置必须分为不同的IP，并且端口必须不同，否则本系统无法区分各个读写器
                // 发过来的数据
                DataTable dt = nsConfigDB.ConfigDB.getTable(staticClass.readerTableName);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (dr["key"].ToString() == this.txtName.Text
                        || dr["ip"].ToString() == this.__IP
                        || dr["port"].ToString() == this.__port)
                    {
                        MessageBox.Show("读写器的名称、IP和端口不能重复！", "信息提示");
                        return;
                    }
                }
                //

                nsConfigDB.ConfigDB.saveConfig(staticClass.readerTableName, this.txtName.Text,
                                            new string[] { this.__IP, this.__port, this.__flag,
                                                            this.cmbSendType.Text,this.txtInterval.Text, this.txtTargetIP.Text });

                this.listBox1.Items.Add(this.txtName.Text);

                //this.btnAdd.Enabled = false;

                staticClass.refresh_reader_dic();
            }
        }
        bool checkValidation()
        {
            if (this.txtName.Text == null || this.txtName.Text == string.Empty)
            {
                return false;
            }
            else
            {

            }
            if (this.txtFlag.Text == null || this.txtFlag.Text == string.Empty)
            {
                //return false;
            }
            else
            {
                this.__flag = this.txtFlag.Text;
            }
            if (this.txtIP.Text == null || this.txtIP.Text == string.Empty)
            {
                MessageBox.Show("必须填写读写器IP地址!", "异常提示");
                return false;
            }
            else
            {
                try
                {
                    string str = this.txtIP.Text;
                    IPAddress ip = IPAddress.Parse(str);
                    this.__IP = str;
                    //MessageBox.Show("IP地址填写不符合规定!", "异常提示");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("IP地址填写不符合规定，" + ex.Message, "异常提示");
                    return false;
                }

            }
            if (this.txtPort.Text == null || this.txtPort.Text == string.Empty)
            {
                MessageBox.Show("必须填写读写器IP地址!", "异常提示");
                return false;
            }
            else
            {
                try
                {
                    string str = this.txtPort.Text;
                    int port = int.Parse(str);
                    this.__port = str;
                    //MessageBox.Show("端口填写不符合规定!", "异常提示");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("端口填写不符合规定，" + ex.Message, "异常提示");
                    return false;
                }

            }
            if (this.txtTargetIP.Text != null && this.txtTargetIP.Text.Length > 0)
            {
                try
                {
                    string[] ips = this.txtTargetIP.Text.Split(';');
                    for (int i = 0; i < ips.Length; i++)
                    {
                        string[] ip_and_port_s = ips[i].Split(':');
                        if (ip_and_port_s.Length < 2)
                        {
                            continue;
                        }
                        IPAddress ip = IPAddress.Parse(ip_and_port_s[0]);
                        int port = int.Parse(ip_and_port_s[1]);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("目标IP地址格式错误，多个IP之间使用分号隔开", "异常提示");
                    return false;
                }
            }
            if (this.txtInterval.Text != null && this.txtInterval.Text.Length > 0)
            {
                try
                {
                    int interval = int.Parse(this.txtInterval.Text);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("发送数据时间间隔设置有误，" + ex.Message, "异常提示");
                    return false;
                }
            }
            return true;
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            //if (this.checkValidation())
            //{
            //    // 业务逻辑
            //    // 当前读写器设置必须分为不同的IP，并且端口必须不同，否则本系统无法区分各个读写器
            //    // 发过来的数据
            //    DataTable dt = nsConfigDB.ConfigDB.getTable("tbreader");
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        DataRow dr = dt.Rows[i];
            //        if (dr["key"].ToString() == this.txtName.Text
            //            || dr["ip"].ToString() == this.__IP
            //            || dr["port"].ToString() == this.__port)
            //        {
            //            //this.btnAdd.Enabled = false;
            //            //this.btnSave.Enabled = false;
            //            return;
            //        }
            //    }

            //}
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.txtName.Text == null
                || this.txtName.Text == string.Empty)
            {
                return;
            }
            DialogResult result = MessageBox.Show("确定删除名称为" + this.txtName.Text + "的读写器吗？", "信息提示", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                DataTable dt = nsConfigDB.ConfigDB.getTable(staticClass.readerTableName);
                DataRow[] rows = dt.Select(string.Format("key = '{0}'", this.txtName.Text));
                if (rows.Length > 0)
                {
                    dt.Rows.Remove(rows[0]);
                }
                frmReaderMngment_Load(null, null);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.checkValidation())
            {
                DataTable dt = nsConfigDB.ConfigDB.getTable(staticClass.readerTableName);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if ((dr["ip"].ToString() == this.__IP || dr["port"].ToString() == this.__port)
                        && dr["key"].ToString() != this.txtName.Text)
                    {
                        MessageBox.Show("读写器的名称、IP和端口不能重复！", "信息提示");
                        return;
                    }
                }
                //

                bool b = nsConfigDB.ConfigDB.saveConfig(staticClass.readerTableName, this.txtName.Text,
                            new string[] { this.__IP, this.__port, this.__flag,
                                        this.cmbSendType.Text,this.txtInterval.Text, this.txtTargetIP.Text });
                if (b == true)
                {
                    MessageBox.Show("修改保存成功！", "信息提示");
                }
                else
                {
                    MessageBox.Show("修改保存时出现异常！", "信息提示");

                }
                staticClass.refresh_reader_dic();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex >= 0)
            {
                string itemName = this.listBox1.Items[this.listBox1.SelectedIndex].ToString();
                string[] values = nsConfigDB.ConfigDB.getConfig(staticClass.readerTableName, itemName);
                if (values != null && values.Length >= 3)
                {
                    this.txtName.Text = values[0];
                    this.txtIP.Text = values[1];
                    this.txtPort.Text = values[2];
                    this.txtFlag.Text = values[3];
                    this.cmbSendType.Text = values[4];
                    this.txtInterval.Text = values[5];
                    this.txtTargetIP.Text = values[6];
                }
            }
        }

        private void cmbSendType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbSendType.SelectedIndex == 0)//udp方式
            {
                this.txtTargetIP.Enabled = true;
                this.txtInterval.Text = string.Empty;
                this.txtInterval.Enabled = false;

                this.txtFlag.Text = string.Empty;
                this.txtFlag.Enabled = false;
            }
            else//rest方式
            {
                this.txtTargetIP.Text = string.Empty;
                this.txtTargetIP.Enabled = false;

                this.txtInterval.Enabled = true;

                this.txtFlag.Enabled = true;
            }
        }
    }
}
