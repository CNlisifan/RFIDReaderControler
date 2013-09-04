using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace RFIDReaderControler
{
    public class sysParaConfig : ISysSettingItem
    {
        bool bChanged = false;
        public string caption = string.Empty;

        public string __ip = string.Empty;
        public string __port = string.Empty;
        public string __interval = string.Empty;


        EventHandler changeHandler = null;
        System.Windows.Forms.Control.ControlCollection Controls;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPort;


        public sysParaConfig(System.Windows.Forms.Control.ControlCollection _controls,
            string _caption, EventHandler _handler)
        {

            if (_caption != null)
            {
                this.caption = _caption;
            }
            this.changeHandler = _handler;
            this.Controls = _controls;

            this.label1 = new System.Windows.Forms.Label();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();

            // 
            // label1
            // 
            //this.label1.AutoSize = true;
            //this.label1.Location = new System.Drawing.Point(236, 49);
            //this.label1.Name = "label1";
            //this.label1.Size = new System.Drawing.Size(89, 12);
            //this.label1.TabIndex = 3;
            //this.label1.Text = "标签发送间隔：";
            //// 
            //// txtInterval
            //// 
            //this.txtInterval.Location = new System.Drawing.Point(357, 45);
            //this.txtInterval.Name = "txtInterval";
            //this.txtInterval.Size = new System.Drawing.Size(241, 21);
            //this.txtInterval.TabIndex = 4;
            //this.txtInterval.TextChanged += new EventHandler(txtInterval_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(238, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "接收数据服务器IP：";
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(357, 42);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(241, 21);
            this.txtServerIP.TabIndex = 4;
            this.txtServerIP.TextChanged += new EventHandler(txtIP_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(613, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "端口：";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(653, 42);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(108, 21);
            this.txtPort.TabIndex = 4;
            this.txtPort.TextChanged += new EventHandler(txtPort_TextChanged);

            try
            {
                object o = nsConfigDB.ConfigDB.getConfig("restPort");
                if (o != null)
                {
                    this.__port = (string)o;
                }
                o = nsConfigDB.ConfigDB.getConfig("restIP");
                if (o != null)
                {
                    this.__ip = (string)o;
                }
                o = nsConfigDB.ConfigDB.getConfig("tagBufferTime");
                if (o != null)
                {
                    this.__interval = (string)o;
                }
                //IList<sysParaConfig> list = staticClass.db.Query<sysParaConfig>(delegate(sysParaConfig uc)
                //{
                //    return uc.caption == this.caption;
                //}
                //);
                //if (list.Count <= 0)
                //{
                //}
                //else
                //{
                //    this.__ip = list[0].__ip;
                //    this.__port = list[0].__port;
                //    this.__interval = list[0].__interval;
                //}
            }
            catch
            {

            }
        }

        void txtInterval_TextChanged(object sender, EventArgs e)
        {
            if (this.txtInterval.Text != this.__interval)
            {
                this.bChanged = true;
                if (this.changeHandler != null)
                {
                    this.changeHandler(sender, e);
                }
            }
        }

        void txtPort_TextChanged(object sender, EventArgs e)
        {
            if (this.txtPort.Text != this.__port)
            {
                this.bChanged = true;
                if (this.changeHandler != null)
                {
                    this.changeHandler(sender, e);
                }
            }
        }

        void txtIP_TextChanged(object sender, EventArgs e)
        {
            if (this.txtServerIP.Text != this.__ip)
            {
                this.bChanged = true;
                if (this.changeHandler != null)
                {
                    this.changeHandler(sender, e);
                }
            }

        }


        public void addControls()
        {
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.txtInterval);

            this.txtServerIP.Text = this.__ip;
            this.txtPort.Text = this.__port;
            this.txtInterval.Text = this.__interval;

        }

        public void removeControls()
        {
            this.Controls.Remove(this.label3);
            this.Controls.Remove(this.label2);
            this.Controls.Remove(this.txtPort);
            this.Controls.Remove(this.txtServerIP);
            this.Controls.Remove(this.txtInterval);
            this.Controls.Remove(this.label1);

        }

        public bool isChanged()
        {
            return this.bChanged;
        }

        public bool saveChanges()
        {
            bool bR = true;

            string strPort = this.txtPort.Text;
            try
            {
                int iport = int.Parse(strPort);
                if (iport < 80)
                {
                    bR = false;
                    MessageBox.Show("端口设置不符合规定，请重新设置！");
                    goto end;
                }
                strPort = iport.ToString();
                this.__port = iport.ToString();
            }
            catch (System.Exception ex)
            {
                bR = false;
                MessageBox.Show("端口设置不符合规定，请重新设置！");
                goto end;
            }
            string strIP = this.txtServerIP.Text;
            try
            {
                IPAddress _ip = IPAddress.Parse(strIP);
                this.__ip = this.txtServerIP.Text;

            }
            catch (System.Exception ex)
            {
                bR = false;
                MessageBox.Show("IP地址设置不符合规定，请重新设置！");
                goto end;
            }
            //try
            //{
            //    int iInterval = int.Parse(this.txtInterval.Text);
            //    if (iInterval < 1000)
            //    {
            //        bR = false;
            //        MessageBox.Show("间隔时间太短，可能数据量会很大！");
            //    }
            //    this.__interval = iInterval.ToString();
            //}
            //catch (System.Exception ex)
            //{
            //    bR = false;
            //    MessageBox.Show("间隔时间设置不符合规定，请重新设置！");
            //    goto end;
            //}
            try
            {
                nsConfigDB.ConfigDB.saveConfig("restPort", this.__port);
                nsConfigDB.ConfigDB.saveConfig("restIP", this.__ip);
                //nsConfigDB.ConfigDB.saveConfig("tagBufferTime", this.__interval);
                //IList<sysParaConfig> list = staticClass.db.Query<sysParaConfig>(delegate(sysParaConfig uc)
                //{
                //    return uc.caption == this.caption;
                //}
                //);
                //if (list.Count <= 0)
                //{
                //    staticClass.db.Store(this);
                //}
                //else
                //{
                //    list[0].__ip = this.__ip;
                //    list[0].__interval = this.__interval;
                //    list[0].__port = this.__port;
                //    staticClass.db.Store(list[0]);
                //}

            }
            catch
            {

            }
        end:
            return bR;
        }
    }
}
