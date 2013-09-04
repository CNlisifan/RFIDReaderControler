using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace RFIDReaderControler
{
    public class sysSettingSerialPortBase : ISysSettingItem
    {
        bool bChanged = false;
        public string caption = string.Empty;
        ISerialPortConfigItem serialPortConfigItem = null;
        EventHandler changeHandler = null;
        System.Windows.Forms.Control.ControlCollection Controls;

        private System.Windows.Forms.ComboBox cmbStopBits;
        private System.Windows.Forms.ComboBox cmbBaudRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbPortName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbDataBits;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.GroupBox groupBox2;

        public sysSettingSerialPortBase(System.Windows.Forms.Control.ControlCollection _controls,
            string _caption, ISerialPortConfigItem configItem, EventHandler _handler)
        {
            if (_caption != null)
            {
                this.caption = _caption;
            }
            else
            {
                this.caption = ((SerialPortConfigItem)configItem).ConfigName;
            }
            this.serialPortConfigItem = (ISerialPortConfigItem)configItem;
            this.changeHandler = _handler;
            this.Controls = _controls;

            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbStopBits = new System.Windows.Forms.ComboBox();
            this.cmbBaudRate = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbPortName = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbDataBits = new System.Windows.Forms.ComboBox();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbStopBits);
            this.groupBox2.Controls.Add(this.cmbBaudRate);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.cmbPortName);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.cmbDataBits);
            this.groupBox2.Controls.Add(this.cmbParity);
            this.groupBox2.Location = new System.Drawing.Point(224, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(572, 380);
            this.groupBox2.TabIndex = 39;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = this.caption;
            // 
            // cmbStopBits
            // 
            this.cmbStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStopBits.FormattingEnabled = true;
            this.cmbStopBits.Items.AddRange(new object[] {
            "1",
            "2"});
            this.cmbStopBits.Location = new System.Drawing.Point(18, 251);
            this.cmbStopBits.Name = "cmbStopBits";
            this.cmbStopBits.Size = new System.Drawing.Size(144, 20);
            this.cmbStopBits.TabIndex = 32;
            // 
            // cmbBaudRate
            // 
            this.cmbBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBaudRate.FormattingEnabled = true;
            this.cmbBaudRate.Items.AddRange(new object[] {
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "28800",
            "36000",
            "57600",
            "115200"});
            this.cmbBaudRate.Location = new System.Drawing.Point(18, 95);
            this.cmbBaudRate.Name = "cmbBaudRate";
            this.cmbBaudRate.Size = new System.Drawing.Size(144, 20);
            this.cmbBaudRate.TabIndex = 28;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 29;
            this.label6.Text = "波特率";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 236);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 35;
            this.label7.Text = "停止位";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 26;
            this.label5.Text = "名称";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 185);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 34;
            this.label8.Text = "数据位";
            // 
            // cmbPortName
            // 
            this.cmbPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPortName.FormattingEnabled = true;
            this.cmbPortName.Location = new System.Drawing.Point(18, 43);
            this.cmbPortName.Name = "cmbPortName";
            this.cmbPortName.Size = new System.Drawing.Size(144, 20);
            this.cmbPortName.TabIndex = 27;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 133);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 33;
            this.label9.Text = "校验位";
            // 
            // cmbDataBits
            // 
            this.cmbDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataBits.FormattingEnabled = true;
            this.cmbDataBits.Items.AddRange(new object[] {
            "7",
            "8",
            "9"});
            this.cmbDataBits.Location = new System.Drawing.Point(18, 200);
            this.cmbDataBits.Name = "cmbDataBits";
            this.cmbDataBits.Size = new System.Drawing.Size(144, 20);
            this.cmbDataBits.TabIndex = 31;
            // 
            // cmbParity
            // 
            this.cmbParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParity.FormattingEnabled = true;
            this.cmbParity.Items.AddRange(new object[] {
            "None",
            "Even",
            "Odd"});
            this.cmbParity.Location = new System.Drawing.Point(18, 148);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(144, 20);
            this.cmbParity.TabIndex = 30;
        }

        void SelectedIndexChanged(object sender, EventArgs e)
        {
            this.bChanged = true;
            if (this.changeHandler != null)
            {
                this.changeHandler(sender, e);
            }
        }
        #region ISysSettingItem 成员

        void ISysSettingItem.addControls()
        {
            this.LoadConfig();
            this.cmbStopBits.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            this.cmbParity.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            this.cmbPortName.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            this.cmbBaudRate.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            this.cmbDataBits.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);

            this.Controls.Add(groupBox2);

        }

        void ISysSettingItem.removeControls()
        {
            this.Controls.Remove(this.groupBox2);
        }

        bool ISysSettingItem.isChanged()
        {
            return this.bChanged;
        }

        bool ISysSettingItem.saveChanges()
        {
            return true;
            this.serialPortConfigItem.SaveConfigItem(cmbPortName.Text,
                                 cmbBaudRate.Text,
                                 cmbParity.Text,
                                 cmbDataBits.Text,
                                 cmbStopBits.Text);
        }

        #endregion
        void LoadConfig()
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            cmbPortName.Items.Clear();
            cmbPortName.Items.AddRange(ports);
            try
            {
                string portname = serialPortConfigItem.GetItemValue("PortName");
                //string portname = ConfigManager.GetItemValue("PortName");
                if (null == portname)
                {
                    cmbPortName.SelectedIndex = -1;
                }
                else
                {

                    cmbPortName.SelectedIndex = cmbPortName.Items.IndexOf(portname);
                }
                string baudRate = serialPortConfigItem.GetItemValue("BaudRate");
                //string baudRate = ConfigManager.GetItemValue("BaudRate");
                if (null != baudRate)
                {
                    cmbBaudRate.SelectedIndex = cmbBaudRate.Items.IndexOf(baudRate);
                }
                else
                {
                    cmbBaudRate.SelectedIndex = -1;
                }
                string parity = serialPortConfigItem.GetItemValue("Parity");
                //string parity = ConfigManager.GetItemValue("Parity");
                if (null != parity)
                {
                    cmbParity.SelectedIndex = cmbParity.Items.IndexOf(parity);
                }
                else
                {
                    cmbParity.SelectedIndex = -1;
                }
                string stopbites = serialPortConfigItem.GetItemValue("StopBits");
                //string stopbites = ConfigManager.GetItemValue("StopBits");
                if (null != stopbites)
                {
                    cmbStopBits.SelectedIndex = cmbStopBits.Items.IndexOf(stopbites);
                }
                else
                {
                    cmbStopBits.SelectedIndex = -1;
                }
                string databits = serialPortConfigItem.GetItemValue("DataBits");
                //string databits = ConfigManager.GetItemValue("DataBits");
                if (null != databits)
                {
                    cmbDataBits.SelectedIndex = cmbDataBits.Items.IndexOf(databits);
                }
                else
                {
                    cmbDataBits.SelectedIndex = -1;
                }
            }
            catch (System.Exception ex)
            {
            }
        }
    }
}
