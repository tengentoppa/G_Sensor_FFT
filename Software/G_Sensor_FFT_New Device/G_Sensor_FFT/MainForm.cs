using MP_Moudule;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using ZedGraph;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace G_Sensor_FFT
{
    public struct FFT_Complex
    {
        public System.Double _real;
        public System.Double _imag;
        public FFT_Complex(double real, double imag)
        {
            _real = real;
            _imag = imag;
        }
    }
    public partial class MainForm : Form
    {
#if DEBUG
        public static System.Windows.Forms.Label lblShowByteAmount;
#endif
        #region Parameter
        enum Version { Advance, Normal };
        Version ToolVersion = Version.Advance;
        string logPath = Application.StartupPath + @"\log";
        string iniPath = Application.StartupPath + @"\setting.ini";
        string logHeader = $"SN,Receive Time,X,Y,Z,TimeStamp,Mark";
        int rawDataLen = 32768;
        const int defPrintDataLen = 1024;
        int printDataLen = 512;
        double hz = 0.0f, m_hz = 1024.0F;
        int refreshInterval = 50;
        double TimeScale = 1; //sec/dev
        bool flagIsOldFw = true, flagIsOldDevice = false;
        CalMode calculateMode = CalMode.FFT;

        #endregion
        #region Declare Zone
        bool flagLockMac = false;
        bool flagRecord = false;
        bool flagAddMark = false;
        bool flagReceiving = false, flagPause = false, flagPlayRecord = false, flagRecordPause = false;
        bool flagClosing = false;
        bool flagWarnX = false, flagWarnY = false, flagWarnZ = false;
        const double oneSecondScale = 1000; //ms
        double speedMulti = 1.0;

        Size formSize;
        frmSetAlarm.xyzLimit limit = new frmSetAlarm.xyzLimit(new double[] { -1, 1, -1, 1, -1, 1 });
        UART serial_port_dongle;
        BleDevice curDevice;
        delegate void dUse();
        List<string> _connectedMac = new List<string>();
        Queue<SourceXYX> point = new Queue<SourceXYX>();
        List<string> portList = new List<string>();
        List<PointXYX> logXyz;
        string filePath;
        Thread PaintXYZ;
        int lastPos = 0;
        #endregion
        #region Import DLL
        [DllImport("MP-FFT.dll", EntryPoint = "FFT_translate", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        extern static System.Int32 FFT_translate(out FFT_Complex p_dst, FFT_Complex[] p_src, System.Int32 num_of_points);
        [DllImport("MP-FFT.dll", EntryPoint = "FFT_convertToMagnitude", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        extern static System.Int32 FFT_convertToMagnitude(out System.Double p_dst, FFT_Complex[] p_src, System.Int32 num_of_points);
        [DllImport("MP-FFT.dll", EntryPoint = "FFT_translateToSpectrum", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        extern static System.Int32 FFT_translateToSpectrum(out System.Double p_delte_hz, out System.Double p_amplitude, out FFT_Complex p_dst, FFT_Complex[] p_src, System.Int32 num_of_points, System.Double hz);
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WritePrivateProfileString(string sectionName, string keyName, string keyValue, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetPrivateProfileString(string sectionName, string keyName, string defaultReturnString, StringBuilder returnString, int returnStringLength, string filePath);
        #endregion
        enum CalMode
        {
            FFT,
            Velocity,
            Deplacement,
        }
        public static class FFT_Complex_Rotate
        {
            public static void ArrRotate(FFT_Complex[] data, int pStart)
            {
                FFT_Complex[] temp = new FFT_Complex[pStart];
                Array.Copy(data, 0, temp, 0, pStart);
                Array.Copy(data, pStart, data, 0, data.Count() - pStart);
                Array.Copy(temp, 0, data, data.Count() - pStart, pStart);
            }

        }
        public static class AsyncFileAccess
        {
            public static async Task WriteText(string filePath, string text)
            {
                reWrite:
                try
                {
                    using (FileStream sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                    {
                        using (StreamWriter sw = new StreamWriter(sourceStream))
                        {
                            await sw.WriteAsync(text);
                        }
                    };
                }
                catch { goto reWrite; }
            }
        }

        class text
        {
            public const string
                OpenCom = "Open Com",
                CloseAll = "Close All",
                Scan = "Scan",
                StopScan = "Stop Scan",
                Scanning = "Scanning",
                Connect = "Connect",
                Connecting = "Connecting",
                Disconnect = "Disconnect",
                Start = "Start",
                Testing = "Testing",
                Stop = "Stop",
                Pause = "Pause",
                Resume = "Resume",
                TestTime = "Test Times",
                TagSuccess = "Read Tag Success",
                AllPass = "All Pass",
                Fail = "Fail",
                NoneData = "None Data",
                LoadFile = "Load File",
                LoadingFile = "Loading",
                RecordPlay = "►",
                RecordPuase = "❚❚",
                StartRecord = "Start Record",
                StopRecord = "Stop Record",
                AddMark = "Mark";
        }
        class SourceXYX
        {
            public DateTime _recordTime;
            public int _timeStamp;
            public int _x, _y, _z;
            public SourceXYX() { }
            public SourceXYX(int x, int y, int z)
            {
                _x = x;
                _y = y;
                _z = z;
                _recordTime = DateTime.Now;
            }
            public SourceXYX(int ts, int x, int y, int z)
            {
                _x = x;
                _y = y;
                _z = z;
                _timeStamp = ts;
                _recordTime = DateTime.Now;
            }
        }
        class PointXYX
        {
            public double _x, _y, _z;
            public PointXYX() { }
            public PointXYX(double x, double y, double z)
            {
                _x = x;
                _y = y;
                _z = z;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            InitInterface();
            InitSubscrib();
            SearchPort();

            PaintXYZ = new Thread(PaintData) { IsBackground = true };
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }

        void InitInterface()
        {
            Text = $"Yoke Reader Production Tool  v{Application.ProductVersion}_{ToolVersion.ToString()}";
            if (ToolVersion == Version.Normal)
            {
                btnScan.Visible = false;
                lbMac.Visible = false;
            }

            foreach (Control c in Controls)
            {
                c.Font = new Font("Consolas", 10f);
            }
            tmrPaintLog.Interval = refreshInterval;

            btnCom.Text = text.OpenCom;
            btnRecord.Text = text.StartRecord;
            btnPlayPause.Text = text.RecordPlay;
            btnAddMark.Text = text.AddMark;
            cbxCalMode.SelectedIndex = 0;

            btnScan.Enabled = false;
            lbMac.Enabled = false;
            btnPause.Enabled = false;
            btnAddMark.Enabled = false;
            EnableRecordController(false);

            btnCom.Top = cbxCom.Bottom + 5;
            btnScan.Top = btnCom.Bottom + 7;
            lbMac.Top = btnScan.Bottom + 5;
            btnPause.Top = lbMac.Bottom + 7;
            btnRecord.Top = btnPause.Bottom + 5;
            btnAddMark.Top = btnRecord.Bottom + 5;
            lblCalMode.Top = btnAddMark.Bottom + 7;
            cbxCalMode.Top = lblCalMode.Bottom + 1;
            lblTimeScale.Top = cbxCalMode.Bottom + 3;
            cbxTimeScale.Top = lblTimeScale.Bottom + 1;
            btnSetAlarm.Top = cbxTimeScale.Bottom + 5;
            btnResetAlarm.Top = btnSetAlarm.Bottom + 5;

            Height = zgcFftZ.Bottom + 50;
            Width = zgcFftZ.Right + 30;

            MinimumSize = new Size(Width, Height);
            formSize = MinimumSize;

            cbxTimeScale.Items.AddRange(new object[] { 0.05, 0.1, 0.5 });
            for (int i = 1; i <= 8; i *= 2)
            {
                cbxTimeScale.Items.Add(i);
            }
            cbxTimeScale.SelectedIndex = cbxTimeScale.Items.Count / 2;

            #region Init GraphPane

            zgcAxisY.Top = zgcAxisX.Bottom + 5;
            zgcAxisZ.Top = zgcAxisY.Bottom + 5;
            zgcFftY.Top = zgcFftX.Bottom + 5;
            zgcFftZ.Top = zgcFftY.Bottom + 5;

            zgcAxisX.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);
            zgcAxisY.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);
            zgcAxisZ.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);
            zgcFftX.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);
            zgcFftY.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);
            zgcFftZ.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);

            ZedGraphControl tmpZgc;
            GraphPane tmpGP;

            tmpZgc = zgcAxisX;
            tmpGP = tmpZgc.GraphPane;
            tmpGP.Title.Text = "X軸";
            tmpGP.XAxis.Title.Text = "時間(s)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.XAxis.Scale.Max = 1.0;
            tmpGP.XAxis.Scale.Min = 0.0;
            tmpGP.YAxis.Scale.Max = 5.0;
            tmpGP.YAxis.Scale.Min = -5.0;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            tmpZgc = zgcAxisY;
            tmpGP = tmpZgc.GraphPane;
            tmpGP.Title.Text = "Y軸";
            tmpGP.XAxis.Title.Text = "時間(s)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.XAxis.Scale.Max = 1.0;
            tmpGP.XAxis.Scale.Min = 0.0;
            tmpGP.YAxis.Scale.Max = 5.0;
            tmpGP.YAxis.Scale.Min = -5.0;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            tmpZgc = zgcAxisZ;
            tmpGP = tmpZgc.GraphPane;
            tmpGP.Title.Text = "Z軸";
            tmpGP.XAxis.Title.Text = "時間(s)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.XAxis.Scale.Max = 1.0;
            tmpGP.XAxis.Scale.Min = 0.0;
            tmpGP.YAxis.Scale.Max = 5.0;
            tmpGP.YAxis.Scale.Min = -5.0;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            tmpZgc = zgcFftX;
            tmpZgc.Left = zgcAxisX.Right + 10;
            tmpZgc.Width = Right - tmpZgc.Left - 25;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            tmpZgc = zgcFftY;
            tmpZgc.Left = zgcAxisX.Right + 10;
            tmpZgc.Width = Right - tmpZgc.Left - 25;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            tmpZgc = zgcFftZ;
            tmpZgc.Left = zgcAxisX.Right + 10;
            tmpZgc.Width = Right - tmpZgc.Left - 25;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;
            #endregion

            #region Init Warn Label
            System.Windows.Forms.Label tmpLbl;
            int lblW = zgcFftX.Left - zgcAxisX.Right;
            int lblH = zgcAxisX.Height;
            tmpLbl = lblWarnX;
            tmpLbl.Location = new Point(zgcAxisX.Right, zgcAxisX.Top);
            tmpLbl.BackColor = Color.LawnGreen;
            tmpLbl.Size = new Size(lblW, lblH);

            tmpLbl = lblWarnY;
            tmpLbl.Location = new Point(zgcAxisY.Right, zgcAxisY.Top);
            tmpLbl.BackColor = Color.LawnGreen;
            tmpLbl.Size = new Size(lblW, lblH);

            tmpLbl = lblWarnZ;
            tmpLbl.Location = new Point(zgcAxisZ.Right, zgcAxisZ.Top);
            tmpLbl.BackColor = Color.LawnGreen;
            tmpLbl.Size = new Size(lblW, lblH);
            #endregion

            btnSpeedController.Top = zgcAxisZ.Bottom - btnSpeedController.Height;
            btnPlayPause.Top = btnSpeedController.Top - btnPlayPause.Height - 5;
            btnLoadFile.Top = btnPlayPause.Top - btnLoadFile.Height - 5;

#if DEBUG
            lblShowByteAmount = new System.Windows.Forms.Label()
            {
                AutoSize = false,
                Location = new Point(btnSetAlarm.Left, btnSetAlarm.Bottom + 5),
                Width = btnSetAlarm.Width,
                Text = "Byte Amount",
                BackColor = Color.PaleVioletRed
            };

            Controls.Add(lblShowByteAmount);
#endif
        }

        void SetPaneToFftMode()
        {
            GraphPane tmpGP;

            tmpGP = zgcFftX.GraphPane;
            tmpGP.Title.Text = "X軸 FFT";
            tmpGP.XAxis.Title.Text = "頻率(Hz)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.XAxis.Scale.Max = defPrintDataLen / 2;
            tmpGP.XAxis.Scale.Min = 0.0;
            tmpGP.YAxis.Scale.Max = 1.0;
            tmpGP.YAxis.Scale.Min = 0.0;

            tmpGP = zgcFftY.GraphPane;
            tmpGP.Title.Text = "Y軸 FFT";
            tmpGP.XAxis.Title.Text = "頻率(Hz)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.XAxis.Scale.Max = defPrintDataLen / 2;
            tmpGP.XAxis.Scale.Min = 0.0;
            tmpGP.YAxis.Scale.Max = 1.0;
            tmpGP.YAxis.Scale.Min = 0.0;

            tmpGP = zgcFftZ.GraphPane;
            tmpGP.Title.Text = "Z軸 FFT";
            tmpGP.XAxis.Title.Text = "頻率(Hz)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.XAxis.Scale.Max = defPrintDataLen / 2;
            tmpGP.XAxis.Scale.Min = 0.0;
            tmpGP.YAxis.Scale.Max = 1.0;
            tmpGP.YAxis.Scale.Min = 0.0;
        }
        void SetPaneToVelocityMode()
        {
            GraphPane tmpGP;

            tmpGP = zgcFftX.GraphPane;
            tmpGP.Title.Text = "X軸 Velocity";
            tmpGP.XAxis.Title.Text = "Time(S)";
            tmpGP.YAxis.Title.Text = "Velocity(m/s)";
            tmpGP.YAxis.Scale.Max = 0.5;
            tmpGP.YAxis.Scale.Min = -0.5;

            tmpGP = zgcFftY.GraphPane;
            tmpGP.Title.Text = "Y軸 Velocity";
            tmpGP.XAxis.Title.Text = "Time(S)";
            tmpGP.YAxis.Title.Text = "Velocity(m/s)";
            tmpGP.YAxis.Scale.Max = 0.5;
            tmpGP.YAxis.Scale.Min = -0.5;

            tmpGP = zgcFftZ.GraphPane;
            tmpGP.Title.Text = "Z軸 Velocity";
            tmpGP.XAxis.Title.Text = "Time(S)";
            tmpGP.YAxis.Title.Text = "Velocity(m/s)";
            tmpGP.YAxis.Scale.Max = 0.5;
            tmpGP.YAxis.Scale.Min = -0.5;
        }
        void SetPaneToDeplacementMode()
        {
            GraphPane tmpGP;

            tmpGP = zgcFftX.GraphPane;
            tmpGP.Title.Text = "X軸 Deplacement";
            tmpGP.XAxis.Title.Text = "Time(S)";
            tmpGP.YAxis.Title.Text = "Deplacement(m/s)";
            tmpGP.YAxis.Scale.Max = 0.05;
            tmpGP.YAxis.Scale.Min = -0.05;

            tmpGP = zgcFftY.GraphPane;
            tmpGP.Title.Text = "Y軸 Deplacement";
            tmpGP.XAxis.Title.Text = "Time(S)";
            tmpGP.YAxis.Title.Text = "Deplacement(m/s)";
            tmpGP.YAxis.Scale.Max = 0.05;
            tmpGP.YAxis.Scale.Min = -0.05;

            tmpGP = zgcFftZ.GraphPane;
            tmpGP.Title.Text = "Z軸 Deplacement";
            tmpGP.XAxis.Title.Text = "Time(S)";
            tmpGP.YAxis.Title.Text = "Deplacement(m/s)";
            tmpGP.YAxis.Scale.Max = 0.05;
            tmpGP.YAxis.Scale.Min = -0.05;
        }
        void InitSubscrib()
        {
            btnCom.TextChanged += (s, e) =>
            {
                if (btnCom.Text == text.CloseAll)
                {
                    cbxCom.Enabled = false;
                    btnScan.Enabled = true;
                    lbMac.Enabled = true;
                    btnPause.Enabled = true;
                    btnLoadFile.Enabled = false;
                    btnPlayPause.Enabled = false;
                    btnSpeedController.Enabled = false;
                }
                else
                {
                    cbxCom.Enabled = true;
                    btnScan.Enabled = false;
                    lbMac.Enabled = false;
                    btnPause.Enabled = false;
                    btnLoadFile.Enabled = true;
                    btnPlayPause.Enabled = true;
                    btnSpeedController.Enabled = true;
                }
            };
            cbxTimeScale.SelectedIndexChanged += (o, e) =>
            {
                double.TryParse(cbxTimeScale.Items[cbxTimeScale.SelectedIndex].ToString(), out double scale);
                printDataLen = (int)(defPrintDataLen * scale);
                TimeScale = scale;
            };

            Resize += MainForm_Resize;
            zgcAxisX.DoubleClick += Zgc_DoubleClick;
            zgcAxisY.DoubleClick += Zgc_DoubleClick;
            zgcAxisZ.DoubleClick += Zgc_DoubleClick;
            zgcFftX.DoubleClick += Zgc_DoubleClick;
            zgcFftY.DoubleClick += Zgc_DoubleClick;
            zgcFftZ.DoubleClick += Zgc_DoubleClick;
        }

        bool inZoom = false;
        Size originSize;
        Point originLocation;
        private void Zgc_DoubleClick(object sender, EventArgs e)
        {
            ZedGraphControl zgc = (ZedGraphControl)sender;
            if (inZoom)
            {
                zgc.Size = originSize;
                zgc.Location = originLocation;
                inZoom = false;
            }
            else
            {
                zgc.BringToFront();
                originSize = zgc.Size;
                originLocation = zgc.Location;
                zgc.Location = new Point(0, 0);
                zgc.Size = new Size(Width, Height - 30);
                inZoom = true;
            }
        }

        DateTime writeLogTiming;
        PointPairList[] pList = new PointPairList[3];
        PointPairList[] pFftList = new PointPairList[3];
        PointPairList[] pVelocityList = new PointPairList[3];
        PointPairList[] pDisplacementList = new PointPairList[3];

        TimeSpan receiveTimeOut = TimeSpan.FromMilliseconds(5000.0);
        DateTime lastReceiveTime = DateTime.Now;

        #region LayoutEvent
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseAll();
        }
        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            if (btnCom.Text == text.OpenCom)
            {
                UART.Statue comStatue = UART.Statue.Null;

                if (!cbxCom.Text.Contains("COM")) return;

                //serial_port_dongle = new UART(cbxCom.Text, 115200);
                serial_port_dongle = new UART(cbxCom.Text, 921600);

                if ((comStatue = serial_port_dongle.Open()) != UART.Statue.Success)
                { return; }
                if (btnPlayPause.Text == text.RecordPuase) { btnPlayPause_Click(null, null); }

                flagClosing = false;
                logXyz = null;

                serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.SetUuid, ByteConverter.HexStringToListByte("3D46D155ECA19BA18BA6B4E10100DE72")));

                serial_port_dongle.StartReceiveData(ProccessDongleData);
                if (ToolVersion == Version.Advance)
                {
                    Thread.Sleep(200);
                    StartScan();
                }
                btnCom.Text = text.CloseAll;
            }
            else
            {
                btnScan.Text = text.StopScan;
                btnScan_Click(null, null);
                Thread.Sleep(30);
                btnCom.Text = text.OpenCom;
                CloseAll();
            }
        }
        private void btnScan_Click(object sender, EventArgs e)
        {
            if (btnScan.Text == text.Scan)
            {
                StartScan();
            }
            else
            {
                StopScan();
            }
        }
        private void btnPause_Click(object sender, EventArgs e)
        {
            serial_port_dongle.ReceiveData();

            if (btnPause.Text == text.Resume)
            {
                flagPause = false;
                btnPause.Text = text.Pause;
            }
            else
            {
                flagPause = true;
                btnPause.Text = text.Resume;
            }
        }
        private void btnRecord_Click(object sender, EventArgs e)
        {
            switch (btnRecord.Text)
            {
                case text.StartRecord:
                    if (curDevice is null) { return; }
                    if (curDevice.connecting == false) { return; }
                    string logFileName;
                    logFileName = LoadIniFile();
                    if (string.IsNullOrEmpty(logFileName))
                    {
                        filePath = logPath + $"\\{ByteConverter.ToHexString(curDevice.mac).Replace(' ', '-')}_{writeLogTiming.ToString("yyyyMMdd_HHmmss")}.txt";
                    }
                    else
                    {
                        string regex = "([\\/:*?\"<>|])";
                        if (Regex.IsMatch(logFileName, regex))
                        {
                            MessageBox.Show("File Name not access,please edit .ini file and try again."); return;
                        }
                        filePath = logPath + $"\\{logFileName}_{writeLogTiming.ToString("yyyyMMdd_HHmmss")}.txt";
                    }
                    btnRecord.Text = text.StopRecord;
                    writeLogTiming = DateTime.Now;
                    flagRecord = true;
                    btnAddMark.Enabled = true;
                    break;
                case text.StopRecord:
                    btnRecord.Text = text.StartRecord;
                    flagRecord = false;
                    btnAddMark.Enabled = false;
                    break;
            }
        }
        private void btnAddMark_Click(object sender, EventArgs e)
        {
            flagAddMark = true;
        }
        private void btnSetAlarm_Click(object sender, EventArgs e)
        {
            frmSetAlarm frmSet = new frmSetAlarm(limit);
            frmSet.ShowDialog();
            if (frmSet.DialogResult == DialogResult.OK)
            {
                limit = frmSet.rtnValue;
            }
        }
        private void btnResetAlarm_Click(object sender, EventArgs e)
        {
            flagWarnX = false;
            flagWarnY = false;
            flagWarnZ = false;
            lblWarnX.ResetWarn();
            lblWarnY.ResetWarn();
            lblWarnZ.ResetWarn();
        }
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            dUse duse;
            flagPlayRecord = false;
            string[] s_arr = null;
            if (Directory.Exists(logPath)) { ofdInput.InitialDirectory = logPath; } else { ofdInput.InitialDirectory = Application.StartupPath; }
            if (ofdInput.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            s_arr = File.ReadAllLines(ofdInput.FileName);
            if (s_arr.Length < printDataLen)
            {
                MessageBox.Show($"請引入筆數大於等於{printDataLen}筆的資料");
                return;
            }
            for (int i = 0; i < pList.Length; i++)
            {
                pList[i] = new PointPairList();
            }
            lastPos = s_arr.Length - 1;
            btnLoadFile.Text = text.LoadingFile;
            Thread t = new Thread(() =>
            {
                logXyz = LogDataProccess(s_arr);
                double h = 0.0;
                foreach (PointXYX p in logXyz)
                {
                    pList[0].Add(h, p._x);
                    pList[1].Add(h, p._y);
                    pList[2].Add(h, p._z);
                    h += 0.001;
                }
                zgcAxisX.GraphPane.XAxis.Scale.Min = 0;
                zgcAxisY.GraphPane.XAxis.Scale.Min = 0;
                zgcAxisZ.GraphPane.XAxis.Scale.Min = 0;
                zgcAxisX.GraphPane.XAxis.Scale.Max = TimeScale;
                zgcAxisY.GraphPane.XAxis.Scale.Max = TimeScale;
                zgcAxisZ.GraphPane.XAxis.Scale.Max = TimeScale;

                PaintCurve(pList[0], zgcAxisX);
                PaintCurve(pList[1], zgcAxisY);
                PaintCurve(pList[2], zgcAxisZ);

                Invoke(duse = () => { btnLoadFile.Text = text.LoadFile; });
            })
            { IsBackground = true };
            t.Start();
        }
        int arrPos = 0;
        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            dUse duse;
            switch (btnPlayPause.Text)
            {
                case text.RecordPlay:
                    //tmrPaintLog.Start();
                    flagRecordPause = false;
                    Thread t = new Thread(() =>
                    {
                        if (logXyz is null) { return; }
                        if (logXyz.Count == 0) { return; }

                        DateTime dt = DateTime.Now;
                        double scale = refreshInterval / 1000.0;
                        FFT_Complex[] logFftX, logFftY, logFftZ;

                        logFftX = new FFT_Complex[printDataLen];
                        logFftY = new FFT_Complex[printDataLen];
                        logFftZ = new FFT_Complex[printDataLen];

                        //double hz = 0.0f, m_hz = 2048.0;
                        double[] m_amplitude_x = new double[printDataLen], m_amplitude_y = new double[printDataLen], m_amplitude_z = new double[printDataLen];
                        FFT_Complex[] m_fft_out_x = new FFT_Complex[printDataLen], m_fft_out_y = new FFT_Complex[printDataLen], m_fft_out_z = new FFT_Complex[printDataLen];
                        int maxLen = logXyz.Count;
                        if (arrPos >= maxLen)
                        {
                            ZgcResetHorize(zgcAxisX);
                            ZgcResetHorize(zgcAxisY);
                            ZgcResetHorize(zgcAxisZ);
                            arrPos = 0;
                        }

                        while (flagRecordPause == false)
                        {
                            if ((DateTime.Now - dt).TotalMilliseconds > refreshInterval)
                            {
                                double len = scale * speedMulti;
                                dt = dt.AddMilliseconds(refreshInterval);
                                ZgcMoveHorize(zgcAxisX, len);
                                ZgcMoveHorize(zgcAxisY, len);
                                ZgcMoveHorize(zgcAxisZ, len);
                                int rangeMax = (int)(arrPos + (len * 1000));
                                int fftArrayCount = 0;
                                do
                                {
                                    if (arrPos >= maxLen)
                                    {
                                        btnPlayPause_Click(null, null);
                                        //tmrPaintLog.Stop();
                                        return;
                                    }
                                    logFftX[fftArrayCount] = new FFT_Complex(logXyz[arrPos]._x, 0.0f);
                                    logFftY[fftArrayCount] = new FFT_Complex(logXyz[arrPos]._y, 0.0f);
                                    logFftZ[fftArrayCount] = new FFT_Complex(logXyz[arrPos]._z, 0.0f);
                                    if (++fftArrayCount >= printDataLen) fftArrayCount = 0;
                                } while (arrPos++ < rangeMax);
                                Console.WriteLine(DateTime.Now.ToString("HHmmss.fff"));

                                FFT_Complex_Rotate.ArrRotate(logFftX, fftArrayCount);
                                FFT_Complex_Rotate.ArrRotate(logFftY, fftArrayCount);
                                FFT_Complex_Rotate.ArrRotate(logFftZ, fftArrayCount);
                                FFT_translateToSpectrum(out hz, out m_amplitude_x[0], out m_fft_out_x[0], logFftX, logFftX.Length, m_hz);
                                FFT_translateToSpectrum(out hz, out m_amplitude_y[0], out m_fft_out_y[0], logFftY, logFftY.Length, m_hz);
                                FFT_translateToSpectrum(out hz, out m_amplitude_z[0], out m_fft_out_z[0], logFftZ, logFftZ.Length, m_hz);

                                pFftList[0] = new PointPairList();
                                pFftList[1] = new PointPairList();
                                pFftList[2] = new PointPairList();
                                for (int i = 1; i < m_amplitude_x.Length; i++)
                                {
                                    pFftList[0].Add(hz * i, m_amplitude_x[i]);
                                    pFftList[1].Add(hz * i, m_amplitude_y[i]);
                                    pFftList[2].Add(hz * i, m_amplitude_z[i]);
                                }
                                PaintCurve(pFftList[0], zgcFftX);
                                PaintCurve(pFftList[1], zgcFftY);
                                PaintCurve(pFftList[2], zgcFftZ);
                            }
                        }
                    })
                    { IsBackground = true };
                    t.Start();
                    btnPlayPause.Text = text.RecordPuase;
                    break;
                case text.RecordPuase:
                    //tmrPaintLog.Stop();
                    flagRecordPause = true;
                    Invoke(duse = () =>
                    {
                        btnPlayPause.Text = text.RecordPlay;
                    });
                    break;
            }
        }
        private void btnSpeedController_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (speedMulti < 8) { speedMulti *= 2; }
                    break;
                case MouseButtons.Right:
                    if (speedMulti > 0.125) { speedMulti /= 2; }
                    break;
            }
            tmrPaintLog.Interval = 20; /*(int)(defaultInterval / speedMulti);*/
            btnSpeedController.Text = $"{speedMulti}X";
        }
        private void lbMac_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lbMac.SelectedItem != null)
            {
                curDevice = BleDevice.DeviceList[lbMac.SelectedItem.ToString()];
                if (!Connect(curDevice.mac))
                {
                    curDevice = null;
                    return;
                }
                point.Clear();
                lastReceiveTime = DateTime.Now;
                btnScan.Text = text.Scan;
            }
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            int width = (Width - formSize.Width) / 2;
            int height = (Height - formSize.Height) / 3;
            ZedGraphControl tmpZgc;

            #region Size Configure
            tmpZgc = zgcAxisX;
            tmpZgc.Width += width;
            tmpZgc.Height += height;

            tmpZgc = zgcAxisY;
            tmpZgc.Width += width;
            tmpZgc.Height += height;

            tmpZgc = zgcAxisZ;
            tmpZgc.Width += width;
            tmpZgc.Height += height;

            tmpZgc = zgcFftX;
            tmpZgc.Width += width;
            tmpZgc.Height += height;

            tmpZgc = zgcFftY;
            tmpZgc.Width += width;
            tmpZgc.Height += height;

            tmpZgc = zgcFftZ;
            tmpZgc.Width += width;
            tmpZgc.Height += height;
            #endregion

            #region Location Configure
            zgcFftX.Left = zgcAxisX.Right + 10;
            zgcFftY.Left = zgcAxisX.Right + 10;
            zgcFftZ.Left = zgcAxisX.Right + 10;

            zgcFftX.Left = zgcAxisX.Right + 10;
            zgcFftY.Left = zgcAxisX.Right + 10;
            zgcFftZ.Left = zgcAxisX.Right + 10;

            zgcAxisY.Top = zgcAxisX.Bottom + 5;
            zgcAxisZ.Top = zgcAxisY.Bottom + 5;
            zgcFftY.Top = zgcFftX.Bottom + 5;
            zgcFftZ.Top = zgcFftY.Bottom + 5;
            #endregion

            #region Label Resize

            int lblW = zgcFftX.Left - zgcAxisX.Right;
            int lblH = zgcAxisX.Height;

            lblWarnX.Size = new Size(lblW, lblH);
            lblWarnY.Size = new Size(lblW, lblH);
            lblWarnZ.Size = new Size(lblW, lblH);

            lblWarnX.Location = new Point(zgcAxisX.Right, zgcAxisX.Top);
            lblWarnY.Location = new Point(zgcAxisY.Right, zgcAxisY.Top);
            lblWarnZ.Location = new Point(zgcAxisZ.Right, zgcAxisZ.Top);

            #endregion

            formSize = new Size(Width, Height);
        }
        private void CbxCalMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculateMode = (CalMode)cbxCalMode.SelectedIndex;
            switch (calculateMode)
            {
                case CalMode.FFT:
                    SetPaneToFftMode();
                    break;
                case CalMode.Velocity:
                    SetPaneToVelocityMode();
                    break;
                case CalMode.Deplacement:
                    SetPaneToDeplacementMode();
                    break;
            }

        }
        #endregion
        #region UART Control
        void StartScan()
        {
            if (flagIsOldFw)
            {
                serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StartScan, null));
            }
            else
            {
                serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StartScan, 0x00));
            }
            btnScan.Text = text.StopScan;
        }
        void StopScan()
        {
            serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StopScan, null));
            btnScan.Text = text.Scan;
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_DEVICECHANGE = 0x219;

            try
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    SearchPort();
                    if (serial_port_dongle is null) { return; }
                    if (cbxCom.Text != serial_port_dongle.getPort.PortName && btnCom.Text == text.CloseAll)
                    {
                        btnOpenCom_Click(null, null);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.Console("WndProc", ex.Message);
            }
            finally { }

            base.WndProc(ref m);
        }
        void SearchPort()
        {
            string[] tmpPortList = UART.SearchPort();
            cbxCom.Items.Clear();
            cbxCom.Items.AddRange(tmpPortList);
            portList.Clear();
            portList.AddRange(tmpPortList);
            if (serial_port_dongle != null)
            {
                if (portList.Contains(serial_port_dongle.getPort.PortName))
                {
                    cbxCom.Text = serial_port_dongle.getPort.PortName;
                    return;
                }
            }
            if (cbxCom.Text == "" && cbxCom.Items.Count > 0)
            {
                cbxCom.SelectedIndex = 0;
            }
        }
        void Connect(List<List<byte>> macList)
        {
            int macAmount = macList.Count;
            List<byte> sendData;
            for (int i = 0; i < macAmount; i++)
            {
                if (macList[i].Count != 6) { continue; }
                sendData = new List<byte>() { (byte)macAmount, (byte)(i + 1) };
                sendData.AddRange(macList[i]);
                serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.SetAutoList, sendData));
            }
        }
        bool Connect(List<byte> mac)
        {
            if (mac.Count != 6) { return false; }
            List<byte> sendData = new List<byte>();
            sendData.AddRange(mac);
            serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.Connect, sendData));
            return true;
        }
        bool Reconnect(List<byte> mac)
        {
            if (mac.Count != 6) return false;
            serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.Disconnect, mac));
            Thread.Sleep(200);
            serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StartScan, 0x00));
            Thread.Sleep(200);
            Connect(mac);
            Thread.Sleep(200);

            return true;
        }
        void CloseAll()
        {
            flagClosing = true;
            flagReceiving = false;
            flagPause = false;
            if (btnRecord.Text == text.StopRecord)
            {
                btnRecord_Click(null, null);
            }
            btnPause.Text = text.Pause;
            EnableRecordController(false);
            lbMac.Items.Clear();
            BleDevice.DeviceList.Clear();
            if (serial_port_dongle != null)
            {
                if (ToolVersion == Version.Normal) { flagLockMac = false; }
                else
                {
                    if (curDevice != null)
                    {
                        serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.Disconnect, curDevice.mac));
                        Thread.Sleep(100);
                    }
                }
                serial_port_dongle.StopReceiveData();
                serial_port_dongle.Close();
            }
        }
        #endregion

        void EnableRecordController(bool sw)
        {
            if (sw)
            {
                btnRecord.Enabled = true;

                flagRecord = false;
                flagAddMark = false;
            }
            else
            {
                btnRecord.Enabled = false;
            }
        }
        void ProccessDongleData(List<byte> data)
        {
            dUse duse;
            BLEDongle dongle;
            List<List<byte>> tl = BLEDongle.ParseData(data);
            if (tl is null) return;
            foreach (List<byte> l in tl)
            {
                dongle = new BLEDongle(l);

                switch (dongle.cmd)
                {
                    case (byte)BLEDongle.CMD.rAvdReport:
                        string address = ByteConverter.ToHexString(dongle.data.GetRange(0, 6));
                        if (BleDevice.SetDevice(address))
                        {
                            Invoke(duse = () =>
                            {
                                lbMac.Items.Add(address);
                            });
                        }
                        if (dongle.data.Count > 8)
                        {
                            BleDevice.DeviceList[address].name =
                              ByteConverter.ToAsciiString(dongle.data.GetRange(6, dongle.data.Count - 7));
                        }
                        else
                        { BleDevice.DeviceList[address].name = "None"; }

                        BleDevice.DeviceList[address].rssi = dongle.data[dongle.data.Count - 1];

                        break;

                    case (byte)BLEDongle.CMD.Connect:
                        break;

                    case (byte)BLEDongle.CMD.Disconnect:
                        if (curDevice is null) break;
                        if (ByteConverter.ToHexString(curDevice.mac) == ByteConverter.ToHexString(dongle.data))
                        {
                            curDevice.receiveA5 = false;
                            curDevice.connecting = false;
                        }
                        break;

                    case (byte)BLEDongle.CMD.rRxData:
                        #region RX Data
                        if (ToolVersion == Version.Normal)
                        {
                            if (flagLockMac == false)
                            {
                                Invoke(duse = () => { EnableRecordController(true); });
                                flagLockMac = true;
                                curDevice = new BleDevice();
                                curDevice.mac = dongle.data.GetRange(0, 6);
                                curDevice.receiveA5 = true;
                                curDevice.connecting = true;
                                flagReceiving = true;
                                if (PaintXYZ.IsAlive) { PaintXYZ.Abort(); Thread.Sleep(10); }
                                PaintXYZ = new Thread(PaintData) { IsBackground = true };
                                PaintXYZ.Start();
                                WriteLog.Console("Connecting", curDevice.name);
                            }
                        }
                        if (curDevice is null) break;

                        //Start,對舊版FW，如改回新版FW請將本段移除或註解
                        if (flagIsOldFw && !flagReceiving)
                        {
                            if (btnCom.Text == text.OpenCom) { return; }
                            if (flagClosing) { return; }
                            curDevice.receiveA5 = true;
                            curDevice.connecting = true;
                            flagReceiving = true;

                            Invoke((dUse)(() => { btnRecord.Enabled = true; }));
                        }
                        //End
                        if (!PaintXYZ.IsAlive)
                        {
                            PaintXYZ = new Thread(PaintData) { IsBackground = true };
                            PaintXYZ.Start();
                        }
                        if (curDevice.receiveA5 == false) break;
                        byte[] tData = dongle.data.GetRange(6, dongle.data.Count - 6).ToArray();
                        if (!flagIsOldDevice)
                        {

                            if (tData.Length != 20) { return; }
                            if (BLEDongle.MakeCrc(tData.ToList(), 0, 19) != tData[19]) { Console.WriteLine("CRC Fail"); return; }

                            for (int i = 1; i < 19; i += 6)
                            {
                                try
                                {
                                    point.Enqueue(new SourceXYX(
                                        tData[0],
                                        ((((sbyte)tData[0 + i]) << 8) + tData[1 + i]),
                                        ((((sbyte)tData[2 + i]) << 8) + tData[3 + i]),
                                        ((((sbyte)tData[4 + i]) << 8) + tData[5 + i])));
                                }
                                catch (Exception) { }
                            }

                        }
                        else
                        {
                            for (int i = 1; i < 19; i += 6)
                            {
                                try
                                {
                                    point.Enqueue(new SourceXYX(
                                        tData[0],
                                        ((((sbyte)tData[0 + i]) << 8) + tData[1 + i]),
                                        ((((sbyte)tData[2 + i]) << 8) + tData[3 + i]),
                                        ((((sbyte)tData[4 + i]) << 8) + tData[5 + i])));
                                }
                                catch (Exception) { }
                            }
                        }
                        break;

                    case (byte)BLEDongle.CMD.TxData:
                        if (dongle.data[6] == 0x01)
                        {
                            curDevice.connecting = true;
                            curDevice.receiveA5 = true;
                        }
                        else
                        {
                            curDevice.connecting = false;
                            curDevice.receiveA5 = false;
                            PaintXYZ.Abort();
                        }
                        break;
                    #endregion

                    case (byte)BLEDongle.CMD.rConnectedAck:
                        if (curDevice is null)
                        {
                            return;
                        }
                        if (curDevice is null) break;
                        if (ByteConverter.ToHexString(curDevice.mac) == ByteConverter.ToHexString(dongle.data))
                        {
                            StopScan();
                            Invoke(duse = () => { EnableRecordController(true); });
                            curDevice.receiveA5 = true;
                            curDevice.connecting = true;
                            flagReceiving = true;
                            if (PaintXYZ.IsAlive) { PaintXYZ.Abort(); Thread.Sleep(10); }
                            PaintXYZ = new Thread(PaintData) { IsBackground = true };
                            PaintXYZ.Start();
                            WriteLog.Console("Connecting", curDevice.name);
                        }
                        break;

                    case (byte)BLEDongle.CMD.SetAutoList:
                        break;
                    case (byte)BLEDongle.CMD.GetConnectList:
                        if (dongle.data[2] != 0x01) return;
                        string mac = ByteConverter.ToHexString(dongle.data.GetRange(3, 6));
                        if (!_connectedMac.Contains(mac))
                            _connectedMac.Add(mac);
                        break;
                }
            }

        }

        void ZgcResetHorize(ZedGraphControl zgc)
        {
            dUse duse;
            zgc.GraphPane.XAxis.Scale.Min -= zgc.GraphPane.XAxis.Scale.Min;
            zgc.GraphPane.XAxis.Scale.Max -= zgc.GraphPane.XAxis.Scale.Max + TimeScale;
            try
            {
                Invoke(duse = () =>
                {
                    zgc.Refresh();
                });
            }
            catch { }
        }
        void ZgcMoveHorize(ZedGraphControl zgc, double displacement)
        {
            dUse duse;
            zgc.GraphPane.XAxis.Scale.Max += displacement;
            zgc.GraphPane.XAxis.Scale.Min = zgc.GraphPane.XAxis.Scale.Max - TimeScale;
            try
            {
                Invoke(duse = () =>
                {
                    zgc.Refresh();
                });
            }
            catch { }
        }

        string LoadIniFile()
        {
            //WritePrivateProfileString("log", "test", "ttt", iniPath);
            StringBuilder result = new StringBuilder(255);
            GetPrivateProfileString("log", "file name", "None", result, 255, iniPath);
            return result.ToString();
        }

        void initZedFft()
        {
            zgcFftX.GraphPane.XAxis.Scale.Min = -2;
            zgcFftX.GraphPane.XAxis.Scale.Max = printDataLen + 1;
            zgcFftY.GraphPane.XAxis.Scale.Min = -2;
            zgcFftY.GraphPane.XAxis.Scale.Max = printDataLen + 1;
            zgcFftZ.GraphPane.XAxis.Scale.Min = -2;
            zgcFftZ.GraphPane.XAxis.Scale.Max = printDataLen + 1;
        }

        const double TwoPI = Math.PI * 2;
        void PaintData()
        {
            #region Declare Zone
            dUse duse;
            UInt32 SN = 0;
            DateTime startTime = DateTime.Now;
            writeLogTiming = DateTime.Now;
            //filePath = logPath + "\\" + $"{ByteConverter.ToHexString(curDevice.mac)}_{writeLogTiming.ToString("MMdd_HHmmss")}.csv";

            int count = 0;
            double axisH = 0;
            FFT_Complex[] fftX, fftY, fftZ;
            double[] m_amplitude_x = new double[defPrintDataLen], m_amplitude_y = new double[defPrintDataLen], m_amplitude_z = new double[defPrintDataLen];
            FFT_Complex[] m_fft_out_x = new FFT_Complex[defPrintDataLen], m_fft_out_y = new FFT_Complex[defPrintDataLen], m_fft_out_z = new FFT_Complex[defPrintDataLen];

            #endregion

            #region Init Zone
            fftX = new FFT_Complex[defPrintDataLen];
            fftY = new FFT_Complex[defPrintDataLen];
            fftZ = new FFT_Complex[defPrintDataLen];

            for (int i = 0; i < pList.Length; i++)
            {
                pList[i] = new PointPairList();
                pVelocityList[i] = new PointPairList();
                pDisplacementList[i] = new PointPairList();
            }

            axisH = 0;
            initZedFft();
            #endregion

            while (flagReceiving)
            {
                int fftArrayCount = 0;

                if (point.Count > 0)
                {
                    SourceXYX p;
                    double h = 0.0;
                    double div = 8192.0;
                    string log = "";
                    double x = 0, y = 0, z = 0;
                    string rcTime = "";
                    DateTime recordTime = DateTime.Now, tempTime = DateTime.Now;
                    int timeCount = 0;

                    #region Data Proccess
                    while (point.Count > 0)
                    {
                        p = point.Dequeue();
                        if (p is null) { continue; }
                        axisH++;
                        h = axisH / oneSecondScale;
                        //x = p._x * multi; y = p._y * multi; z = p._z * multi;

                        x = p._x / div; y = p._y / div; z = p._z / div;
                        recordTime = p._recordTime;

                        if (!flagWarnX)
                        {
                            if (limit.x[0] > x || limit.x[1] < x) { lblWarnX.SetWarn(); flagWarnX = true; }
                        }
                        if (!flagWarnY)
                        {
                            if (limit.y[0] > y || limit.y[1] < y) { lblWarnY.SetWarn(); flagWarnY = true; }
                        }
                        if (!flagWarnZ)
                        {
                            if (limit.z[0] > z || limit.z[1] < z) { lblWarnZ.SetWarn(); flagWarnZ = true; }
                        }

                        //由Tool來調整時間間距
                        if (tempTime == recordTime)
                        {
                            rcTime = UNIXTime.DatetimeToUnix(recordTime).ToString() + recordTime.AddMilliseconds((++timeCount) / 10).ToString(".fff") + (timeCount % 10);
                        }
                        else
                        {
                            timeCount = 0;
                            rcTime = UNIXTime.DatetimeToUnix(recordTime).ToString() + recordTime.ToString(".fff") + '0';
                            tempTime = recordTime;
                        }

                        fftX[fftArrayCount] = new FFT_Complex(x, 0.0f);
                        fftY[fftArrayCount] = new FFT_Complex(y, 0.0f);
                        fftZ[fftArrayCount] = new FFT_Complex(z, 0.0f);
                        if (++fftArrayCount >= defPrintDataLen) fftArrayCount = 0;

                        pList[0].Add(h, x);
                        pList[1].Add(h, y);
                        pList[2].Add(h, z);

                        if (flagRecord)
                        {
                            log += $"{(++SN).ToString()}\t{x.ToString("F6")}\t{y.ToString("F6")}\t{z.ToString("F6")}";
                            if (flagAddMark)
                            {
                                flagAddMark = false;
                                log += "Mark";
                            }
                            log += "\n";
                        }
                    }
                    if (flagRecord)
                    {
                        AsyncFileAccess.WriteText(filePath, log);
                    }
                    FFT_Complex_Rotate.ArrRotate(fftX, fftArrayCount);
                    FFT_Complex_Rotate.ArrRotate(fftY, fftArrayCount);
                    FFT_Complex_Rotate.ArrRotate(fftZ, fftArrayCount);

                    for (int i = 0; i < pList.Length; i++)
                    {
                        if (pList[i].Count > rawDataLen)
                        {
                            pList[i].RemoveRange(0, pList[i].Count - rawDataLen);
                        }
                    }
                    #endregion

                    double
                        max = h,
                        min = h - printDataLen / oneSecondScale;
                    if (!flagPause)
                    {
                        int pStart = pList[0].Count - printDataLen;
                        int amount = printDataLen;
                        if (pStart < 0)
                        {
                            pStart = 0;
                            amount = pList[0].Count;
                        }
                        PointPairList[] a = new PointPairList[3];
                        for (int i = 0; i < a.Length; i++) { a[i] = new PointPairList(); a[i].AddRange(pList[i]); }
                        zgcAxisX.GraphPane.XAxis.Scale.Max = max;
                        zgcAxisX.GraphPane.XAxis.Scale.Min = min;
                        zgcAxisY.GraphPane.XAxis.Scale.Max = max;
                        zgcAxisY.GraphPane.XAxis.Scale.Min = min;
                        zgcAxisZ.GraphPane.XAxis.Scale.Max = max;
                        zgcAxisZ.GraphPane.XAxis.Scale.Min = min;
                        PaintCurve(a[0], zgcAxisX);
                        PaintCurve(a[1], zgcAxisY);
                        PaintCurve(a[2], zgcAxisZ);

                        if (count++ > 10)
                        {
                            #region FFT calculate and paint
                            FFT_translateToSpectrum(out hz, out m_amplitude_x[0], out m_fft_out_x[0], fftX, fftX.Length, m_hz);
                            FFT_translateToSpectrum(out hz, out m_amplitude_y[0], out m_fft_out_y[0], fftY, fftY.Length, m_hz);
                            FFT_translateToSpectrum(out hz, out m_amplitude_z[0], out m_fft_out_z[0], fftZ, fftZ.Length, m_hz);
                            PointPairList[] list = new PointPairList[3];
                            list[0] = new PointPairList();
                            list[1] = new PointPairList();
                            list[2] = new PointPairList();

                            #region FFT Point
                            for (int i = 1; i < m_amplitude_x.Length / 2; i++)
                            {
                                list[0].Add(hz * i, m_amplitude_x[i]);
                                list[1].Add(hz * i, m_amplitude_y[i]);
                                list[2].Add(hz * i, m_amplitude_z[i]);
                            }
                            #endregion

                            #region Prepare Data
                            double oX, oY, oZ;
                            double vX, vY, vZ;
                            double dX, dY, dZ;
                            oX = list[0].First(t => t.Y == list[0].Max(n => n.Y)).X * TwoPI;
                            oY = list[1].First(t => t.Y == list[1].Max(n => n.Y)).X * TwoPI;
                            oZ = list[2].First(t => t.Y == list[2].Max(n => n.Y)).X * TwoPI;

                            vX = x / oX;
                            vY = y / oY;
                            vZ = z / oZ;

                            dX = vX / oX;
                            dY = vY / oY;
                            dZ = vZ / oZ;
                            #endregion

                            #region Velocity
                            pVelocityList[0].Add(h, vX);
                            pVelocityList[1].Add(h, vY);
                            pVelocityList[2].Add(h, vZ);
                            #endregion

                            #region Deplacement
                            pDisplacementList[0].Add(h, dX);
                            pDisplacementList[1].Add(h, dY);
                            pDisplacementList[2].Add(h, dZ);
                            #endregion

                            for (int i = 0; i < 3; i++)
                            {
                                if (pVelocityList[i].Count > rawDataLen)
                                {
                                    pVelocityList[i].RemoveRange(0, pVelocityList[i].Count - rawDataLen);
                                }
                            }

                            try
                            {
                                switch (calculateMode)
                                {
                                    case CalMode.FFT:
                                        PaintCurve(list[0], zgcFftX);
                                        PaintCurve(list[1], zgcFftY);
                                        PaintCurve(list[2], zgcFftZ);
                                        break;
                                    case CalMode.Velocity:
                                        MoveZedFftRange(max, min);
                                        PaintCurve(pVelocityList[0], zgcFftX);
                                        PaintCurve(pVelocityList[1], zgcFftY);
                                        PaintCurve(pVelocityList[2], zgcFftZ);
                                        break;
                                    case CalMode.Deplacement:
                                        MoveZedFftRange(max, min);
                                        PaintCurve(pDisplacementList[0], zgcFftX);
                                        PaintCurve(pDisplacementList[1], zgcFftY);
                                        PaintCurve(pDisplacementList[2], zgcFftZ);
                                        break;

                                }
                            }
                            catch { }
                            #endregion
                        }

                        Thread.Sleep(10);
                    }
                    lastReceiveTime = DateTime.Now;
                }
                else
                {
                    Thread.Sleep(10);
                }
                if (DateTime.Now - lastReceiveTime > receiveTimeOut)
                {
                    Reconnect(curDevice.mac);
                    Invoke(duse = () => { EnableRecordController(false); });
                    flagReceiving = false;
                }
            }
        }
        void MoveZedFftRange(double max, double min)
        {
            zgcFftX.GraphPane.XAxis.Scale.Max = max;
            zgcFftX.GraphPane.XAxis.Scale.Min = min;
            zgcFftY.GraphPane.XAxis.Scale.Max = max;
            zgcFftY.GraphPane.XAxis.Scale.Min = min;
            zgcFftZ.GraphPane.XAxis.Scale.Max = max;
            zgcFftZ.GraphPane.XAxis.Scale.Min = min;
        }

        bool PaintCurve(PointPairList[] pointListArray, ZedGraphControl paneArea)
        {
            dUse duse;
            if (pointListArray.Length > 6) return false;
            Color[] colorArray = { Color.Gray, Color.Black, Color.DarkBlue, Color.Beige, Color.BlanchedAlmond, Color.BurlyWood };
            int count = 0;
            paneArea.GraphPane.CurveList.Clear();
            foreach (PointPairList pointList in pointListArray)
            {
                paneArea.GraphPane.AddCurve("", pointList, colorArray[count++], SymbolType.None);
            }
            paneArea.AxisChange();
            if (flagReceiving)
            {
                Invoke(duse = () =>
                {
                    paneArea.Refresh();
                });
            }
            return true;
        }
        bool PaintCurve(PointPairList pointList, ZedGraphControl paneArea)
        {
            dUse duse;
            Color color = Color.LawnGreen;
            paneArea.GraphPane.CurveList.Clear();
            paneArea.GraphPane.AddCurve("", pointList, color, SymbolType.None);
            paneArea.AxisChange();
            try
            {
                Invoke(duse = () =>
                {
                    paneArea.Refresh();
                });
            }
            catch
            {
            }
            return true;
        }
        bool PaintBar(PointPairList ppl, ZedGraphControl zgc)
        {
            dUse duse;
            GraphPane pane = zgc.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();
            pane.AddBar("", ppl, Color.Gray);
            zgc.AxisChange();
            try
            {
                Invoke(duse = () =>
                {
                    zgc.Refresh();
                });
            }
            catch { }
            return true;
        }
        void ClearPane()
        {
            GraphPane pane;

            pane = zgcAxisX.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcAxisY.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcAxisZ.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcFftX.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcFftX.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcFftX.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();
        }

        List<PointXYX> LogDataProccess(string[] logData)
        {
            List<PointXYX> result = null;
            ClearPane();
            try
            {
                result = new List<PointXYX>();
                foreach (string t in logData)
                {
                    string[] split2 = t.Replace("\r", "").Split(',');
                    if (split2.Length < 4) continue;
                    try { result.Add(new PointXYX(double.Parse(split2[2]), double.Parse(split2[3]), double.Parse(split2[4]))); } catch { continue; }
                }
                return result;
            }
            catch
            {
                return result;
            }
        }
    }

    class LabelWarn : System.Windows.Forms.Label
    {
        delegate void dUse();
        static Color colorPass = Color.LawnGreen;
        static Color colorWarn = Color.MediumVioletRed;
        public void ResetWarn()
        {
            Invoke((dUse)(() =>
            {
                BackColor = colorPass;
            }));
        }

        public void SetWarn()
        {
            Invoke((dUse)(() =>
            {
                BackColor = colorWarn;
            }));
        }
    }

    public partial class frmSetAlarm : Form
    {
        TextBox[][] tbxInputArray = new TextBox[3][];
        Button btnTest;
        xyzLimit limit;
        public class xyzLimit
        {
            public bool valueVerify = false;
            public double[]
                x = new double[2],
                y = new double[2],
                z = new double[2];

            public xyzLimit(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax)
            {
                x[0] = xMin;
                x[1] = xMax;
                y[0] = yMin;
                y[1] = yMax;
                z[0] = zMin;
                z[1] = zMax;
                verify();
            }
            public xyzLimit(double[] input)
            {
                x[0] = input[0];
                x[1] = input[1];
                y[0] = input[2];
                y[1] = input[3];
                z[0] = input[4];
                z[1] = input[5];
                verify();
            }
            void verify()
            {
                if (x[0] > x[1]) { return; }
                if (y[0] > y[1]) { return; }
                if (z[0] > z[1]) { return; }
                valueVerify = true;
            }
        }
        public xyzLimit rtnValue;
        public frmSetAlarm(xyzLimit limit)
        {
            this.limit = limit;
            initForm();
            initInterface();

            btnTest.Click += (o, e) =>
            {
                double[] input = new double[tbxInputArray.Length * tbxInputArray[0].Length];
                for (int i = 0; i < tbxInputArray.GetLength(0); i++)
                {
                    for (int j = 0; j < tbxInputArray[i].Length; j++)
                    {
                        if (!double.TryParse(tbxInputArray[i][j].Text, out input[i * 2 + j])) { MessageBox.Show("請輸入有效數字"); return; }
                    }
                }

                rtnValue = new xyzLimit(input);
                if (!rtnValue.valueVerify)
                {
                    MessageBox.Show("請驗證資料是否無誤");
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.Add(btnTest);
        }
        void initForm()
        {
            Font = new Font("Consolas", 12.0f);
        }
        void initInterface()
        {
            Point startLoc = new Point(40, 50);
            for (int i = 0; i < tbxInputArray.Length; i++)
            {
                tbxInputArray[i] = new TextBox[2];
                for (int j = 0; j < tbxInputArray[i].Length; j++)
                {
                    double t = 0.0;
                    switch (i)
                    {
                        case 0:
                            t = limit.x[j];
                            break;
                        case 1:
                            t = limit.y[j];
                            break;
                        case 2:
                            t = limit.z[j];
                            break;
                    }
                    tbxInputArray[i][j] = new TextBox()
                    {
                        Text = t.ToString()
                    };
                    tbxInputArray[i][j].Location = new Point(j * (tbxInputArray[0][0].Width + 10) + startLoc.X, i * 50 + startLoc.Y);
                    Controls.Add(tbxInputArray[i][j]);
                }
            }

            System.Windows.Forms.Label lblX, lblY, lblZ, lblMinium, lblMaxium;
            lblX = new System.Windows.Forms.Label() { Text = "X", Location = new Point(tbxInputArray[0][0].Left - 25, tbxInputArray[0][0].Top) };
            lblY = new System.Windows.Forms.Label() { Text = "Y", Location = new Point(tbxInputArray[1][0].Left - 25, tbxInputArray[1][0].Top) };
            lblZ = new System.Windows.Forms.Label() { Text = "Z", Location = new Point(tbxInputArray[2][0].Left - 25, tbxInputArray[2][0].Top) };

            lblMinium = new System.Windows.Forms.Label() { Text = "Minium", Location = new Point(tbxInputArray[0][0].Left, tbxInputArray[0][0].Top - 25) };
            lblMaxium = new System.Windows.Forms.Label() { Text = "Maxium", Location = new Point(tbxInputArray[0][1].Left, tbxInputArray[0][1].Top - 25) };

            Controls.Add(lblX);
            Controls.Add(lblY);
            Controls.Add(lblZ);
            Controls.Add(lblMinium);
            Controls.Add(lblMaxium);

            btnTest = new Button()
            {
                Size = new Size(200, 50),
                Text = "Confirm"
            };
            btnTest.Location = new Point((Width - btnTest.Width) / 2, tbxInputArray.Last().Last().Bottom + 20);

            TextBox tmpTbx = tbxInputArray.Last().Last();
            Point tmpPoint = new Point(tmpTbx.Right, tmpTbx.Bottom);
            Size = new Size(tmpPoint.X + 30, btnTest.Bottom + 60);
            MinimumSize = Size;
        }
    }
}
