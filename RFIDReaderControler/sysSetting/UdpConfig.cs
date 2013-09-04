using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace RFIDReaderControler
{
    public class UdpConfig : ISysSettingItem
    {
        bool bChanged = false;
        public string caption = string.Empty;
        public string port = string.Empty;
        public string ip = string.Empty;

        EventHandler changeHandler = null;
        System.Windows.Forms.Control.ControlCollection Controls;


        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;


        public UdpConfig(System.Windows.Forms.Control.ControlCollection _controls,
            string _caption, EventHandler _handler)
        {

            if (_caption != null)
            {
                this.caption = _caption;
            }
            this.changeHandler = _handler;
            this.Controls = _controls;

            this.label1 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();

            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(255, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "监听IP地址：";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(338, 23);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(259, 21);
            this.txtIP.TabIndex = 4;
            this.txtIP.TextChanged += new EventHandler(txtIP_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "监听IP端口：";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(338, 66);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(259, 21);
            this.txtPort.TabIndex = 4;
            this.txtPort.TextChanged += new EventHandler(txtPort_TextChanged);
            try
            {
                object o = nsConfigDB.ConfigDB.getConfig("uddIP");
                if (o != null)
                {
                    this.ip = (string)o;
                }
                o = nsConfigDB.ConfigDB.getConfig("udpPort");
                if (o != null)
                {
                    this.port = (string)o;
                }
                //IList<UdpConfig> list = staticClass.db.Query<UdpConfig>(delegate(UdpConfig uc)
                //{
                //    return uc.caption == this.caption;
                //}
                //);
                //if (list.Count <= 0)
                //{
                //}
                //else
                //{
                //    this.ip = list[0].ip;
                //    this.port = list[0].port;

                //}

            }
            catch
            {

            }
        }

        void txtPort_TextChanged(object sender, EventArgs e)
        {
            if (this.txtPort.Text != this.port)
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
            if (this.txtIP.Text != this.ip)
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
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);

            this.txtIP.Text = this.ip;
            this.txtPort.Text = this.port;

        }

        public void removeControls()
        {
            this.Controls.Remove(this.txtPort);
            this.Controls.Remove(this.txtIP);
            this.Controls.Remove(this.label2);
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
                this.port = iport.ToString();
            }
            catch (System.Exception ex)
            {
                bR = false;
                MessageBox.Show("端口设置不符合规定，请重新设置！");
            }
            string strIP = this.txtIP.Text;
            try
            {
                IPAddress _ip = IPAddress.Parse(strIP);
                this.ip = this.txtIP.Text;

            }
            catch (System.Exception ex)
            {
                bR = false;
                MessageBox.Show("IP地址设置不符合规定，请重新设置！");
                goto end;
            }
            try
            {
                nsConfigDB.ConfigDB.saveConfig("uddIP", this.ip);
                nsConfigDB.ConfigDB.saveConfig("udpPort", this.port);

                //IList<UdpConfig> list = staticClass.db.Query<UdpConfig>(delegate(UdpConfig uc)
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
                //    list[0].ip = this.ip;
                //    list[0].port = this.port;
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
