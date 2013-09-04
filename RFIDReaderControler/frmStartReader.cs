using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RFIDReaderControler
{
    public partial class frmStartReader : Form
    {
        public frmStartReader()
        {
            InitializeComponent();
            this.cmbSendType.Items.Clear();
            this.cmbSendType.Items.Add(ReaderInfo.sendTypeUDP);
            this.cmbSendType.Items.Add(ReaderInfo.sendTypeREST);
            this.Shown += new EventHandler(frmStartReader_Shown);
        }

        void frmStartReader_Shown(object sender, EventArgs e)
        {
            this.cmbReaders.Items.Clear();
            Dictionary<string, ReaderInfo>.KeyCollection keys = staticClass.readerDic.Keys;
            foreach (string s in keys)
            {
                this.cmbReaders.Items.Add(s);
            }
            if (this.cmbReaders.Items.Count > 0)
            {
                this.cmbReaders.SelectedIndex = 0;
            }
        }
        public void refreshButtonStart(string _reader_name)
        {
            if (this.cmbReaders.Text == _reader_name)
            {
                ReaderInfo ri = staticClass.readerDic[_reader_name];
                if (ri.bRunning == true)
                {
                    this.btnStart.Enabled = false;
                }
                else
                {
                    this.btnStart.Enabled = true;
                }
            }
        }
        private void cmbReaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReaderInfo ri = staticClass.readerDic[this.cmbReaders.Text];
            if (ri != null)
            {
                this.txtFlag.Text = ri.flag;
                this.txtIP.Text = ri.ip.ToString();
                this.txtPort.Text = ri.port.ToString();
                this.cmbSendType.Text = ri.sendType;
                this.txtTargetIP.Text = ri.ips;
                this.txtInterval.Text = ri.interval.ToString();

                if (ri.bRunning == true)
                {
                    this.btnStart.Enabled = false;
                    //this.btnStop.Enabled = true;
                }
                else
                {
                    this.btnStart.Enabled = true;
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            frmReaderRunning frm = new frmReaderRunning(this.cmbReaders.Text, this);

            this.btnStart.Enabled = false;

            frm.Show();
        }
    }
}
