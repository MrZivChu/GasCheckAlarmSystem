using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GasCheckAlarmSystem
{
    public struct SGameConfig
    {
        public bool isEnterPosDir;
        public bool isLog;
        public double mainLight;
        public double subLight;
        public bool isSetLightByUI;
        public string commName;
        public string productName;
    }
    public partial class Form1 : Form
    {
        public static Form1 instance;
        public Form1()
        {
            instance = this;
            InitializeComponent();
        }

        SerialPortHelper serialPortHelper = null;
        public static SGameConfig gameConfig_;
        private void Form1_Load(object sender, EventArgs e)
        {
            RealtimeDataDAL.ResetGasValue();
            ReadConfig();
            InitControl();
            InitSerialPort();
        }

        string PrintSendData(string startProbe, string endProbe, string machineAddress)
        {
            int firstDec = Convert.ToInt32(startProbe);
            int endDec = Convert.ToInt32(endProbe);
            string firstHex = firstDec.ToString("X4");
            string endHex = endDec.ToString("X4");

            int machineDec = Convert.ToInt32(machineAddress);
            string machineHex = machineDec.ToString("X2");

            StringBuilder sb = new StringBuilder();
            sb.Append(machineHex).Append("03").Append(firstHex).Append(endHex);
            return HexHelper.GetTxtSendText(sb.ToString());
        }

        void ReadConfig()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "/config.txt";
            if (File.Exists(configPath))
            {
                string content = File.ReadAllText(configPath);
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        gameConfig_ = LitJson.JsonMapper.ToObject<SGameConfig>(content);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("配置文件转换成Json格式失败：" + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("配置文件内容为空");
                }
            }
            else
            {
                MessageBox.Show("配置文件不存在：" + configPath);
            }
        }

        void InitControl()
        {
            if (gameConfig_.isLog)
            {
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
                ControlBox = true;
            }
            else
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
                ControlBox = false;
            }
        }

        void InitSerialPort()
        {
            try
            {
                serialPortHelper = new SerialPortHelper();
                serialPortHelper.StartConnect(gameConfig_.commName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化串口失败：" + ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPortHelper != null)
            {
                serialPortHelper.FixedUpdate(timer1.Interval);
            }
            TimerHelper.GetInstance().Update(timer1.Interval);
        }

        string log_ = string.Empty;
        public void AddLog(string log)
        {
            if (gameConfig_.isLog)
            {
                log_ += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + log + "\n";
            }
        }
        public void AddChangeLineLog()
        {
            if (gameConfig_.isLog)
            {
                log_ += "\n";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllText("d:\\" + DateTime.Now.ToString("MM-dd HH-mm") + ".txt", log_);
            MessageBox.Show("保存成功");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            button1_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox4.Text = PrintSendData(textBox1.Text, textBox2.Text, textBox3.Text);
        }
    }
}
