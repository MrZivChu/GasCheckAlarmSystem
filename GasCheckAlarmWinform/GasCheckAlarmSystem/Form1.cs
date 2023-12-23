using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GasCheckAlarmSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigHandleHelper.InitConfig();
            SqlHelper.InitSqlConnection(ConfigHandleHelper.GetConfig().sqlIP, ConfigHandleHelper.GetConfig().sqlDatabase, ConfigHandleHelper.GetConfig().sqlUserId, ConfigHandleHelper.GetConfig().sqlUserPwd);
            ProbeDAL.ResetGasValue();
            InitControl();
            InitSerialPort();
        }

        void InitControl()
        {
            WindowState = ConfigHandleHelper.GetConfig().isLog ? FormWindowState.Normal : FormWindowState.Minimized;
            ShowInTaskbar = ConfigHandleHelper.GetConfig().isLog;
            ControlBox = ConfigHandleHelper.GetConfig().isLog;
        }

        SerialPortHelper serialPortHelper_ = null;
        void InitSerialPort()
        {
            serialPortHelper_ = new SerialPortHelper();
            serialPortHelper_.Run();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPortHelper_ != null)
            {
                serialPortHelper_.FixedUpdate(timer1.Interval);
            }
            TimerHelper.GetInstance().Update(timer1.Interval);
        }

        private void saveLogBtn_Click(object sender, EventArgs e)
        {
            LogHelper.SaveLog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(HexHelper.GetTxtSendText(textBox1.Text));
        }
    }
}
