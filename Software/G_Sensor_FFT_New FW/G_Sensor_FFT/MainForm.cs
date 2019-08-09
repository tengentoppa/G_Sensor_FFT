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
        #region Parameter
        enum Version { Advance, Normal };
        Version ToolVersion = Version.Advance;
        string logPath = Application.StartupPath + @"\log";
        string iniPath = Application.StartupPath + @"\setting.ini";
        int rawDataLen = 32768;
        const int defPrintDataLen = 1024;
        int printDataLen = 1024;
        double writeLogInterval = 60.0;
        double hz = 0.0f, m_hz = 8192.0;
        int refreshInterval = 50;
        double TimeScale = 1; //sec/dev
        bool flagIsOldFw = true;

        #endregion

        #region Declare Zone
        bool flagLockMac = false;
        bool flagRecord = false;
        bool flagAddMark = false;
        const double oneSecondScale = 1000; //ms
        double speedMulti = 1.0;

        Size formSize;

        UART serial_port_dongle;
        BleDevice curDevice;
        delegate void dUse();
        List<string> _connectedMac = new List<string>();
        Queue<SourceXYX> point = new Queue<SourceXYX>();
        List<string> portList = new List<string>();
        List<PointXYX> logXyz;
        bool flagReceiving = false, flagPause = false, flagPlayRecord = false, flagRecordPause = false;
        string filePath;
        Thread PaintXYZ;

        int lastPos = 0;
        //FFT_Complex[] m_fft_in = null/*, m_fft_out = null, m_fft_out2 = null*/;

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
                catch { /*WriteLog.Console("Wtrite File", e.ToString());*/ goto reWrite; }
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
            public int _x, _y, _z;
            public SourceXYX() { }
            public SourceXYX(int x, int y, int z)
            {
                _x = x;
                _y = y;
                _z = z;
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

            btnScan.Enabled = false;
            lbMac.Enabled = false;
            btnPause.Enabled = false;
            btnAddMark.Enabled = false;
            EnableRecordController(false);

            btnCom.Top = cbxCom.Bottom + 10;
            btnScan.Top = btnCom.Bottom + 30;
            lbMac.Top = btnScan.Bottom + 10;
            btnPause.Top = lbMac.Bottom + 15;
            btnRecord.Top = btnPause.Bottom + 5;
            btnAddMark.Top = btnRecord.Bottom + 5;
            lblShowRange.Top = btnAddMark.Bottom + 10;
            cbxTimeScale.Top = lblShowRange.Bottom + 1;

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

            #region InitGraphPane

            zgcAxisY.Top = zgcAxisX.Bottom + 5;
            zgcAxisZ.Top = zgcAxisY.Bottom + 5;
            zgcFftY.Top = zgcFftX.Bottom + 5;
            zgcFftZ.Top = zgcFftY.Bottom + 5;

            zgcAxisX.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);
            zgcAxisY.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);
            zgcAxisZ.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);

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
            tmpGP = tmpZgc.GraphPane;
            tmpGP.Title.Text = "X軸 FFT";
            tmpGP.XAxis.Title.Text = "頻率(Hz)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.YAxis.Scale.Max = 1.0;
            tmpGP.YAxis.Scale.Min = 0.0;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            tmpZgc = zgcFftY;
            tmpZgc.Left = zgcAxisX.Right + 10;
            tmpZgc.Width = Right - tmpZgc.Left - 25;
            tmpGP = tmpZgc.GraphPane;
            tmpGP.Title.Text = "Y軸 FFT";
            tmpGP.XAxis.Title.Text = "頻率(Hz)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.YAxis.Scale.Max = 1.0;
            tmpGP.YAxis.Scale.Min = 0.0;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            tmpZgc = zgcFftZ;
            tmpZgc.Left = zgcAxisX.Right + 10;
            tmpZgc.Width = Right - tmpZgc.Left - 25;
            tmpGP = tmpZgc.GraphPane;
            tmpGP.Title.Text = "Z軸 FFT";
            tmpGP.XAxis.Title.Text = "頻率(Hz)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.YAxis.Scale.Max = 1.0;
            tmpGP.YAxis.Scale.Min = 0.0;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            #endregion

            btnSpeedController.Top = zgcAxisZ.Bottom - btnSpeedController.Height;
            btnPlayPause.Top = btnSpeedController.Top - btnPlayPause.Height - 5;
            btnLoadFile.Top = btnPlayPause.Top - btnLoadFile.Height - 5;
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
        PointPairList[] pLogList = new PointPairList[3];
        PointPairList[] pLogFftList = new PointPairList[3];

        TimeSpan receiveTimeOut = TimeSpan.FromMilliseconds(5000.0);
        DateTime lastReceiveTime = DateTime.Now;

        /// <summary>
        /// Com Port變更時自動更新Com Port清單
        /// </summary>
        /// <param name="m">系統回報的訊息</param>
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
                    //else if (cbxCom.Text == serial_port_dongle.getPort.PortName && btnCom.Text == text.OpenCom)
                    //{
                    //    btnOpenCom_Click(null, null);
                    //    Thread.Sleep(100);
                    //    StopScan();
                    //    Thread.Sleep(100);
                    //    StartScan();
                    //    return;
                    //}

                }
            }
            catch (Exception ex)
            {
                WriteLog.Console("WndProc", ex.Message);
            }
            finally { }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 將搜尋到的Com Port置入清單
        /// </summary>
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

                logXyz = null;

                serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.SetUuid, ByteConverter.HexStringToListByte("3D46D155ECA19BA18BA6B4E10100DE72")));

                serial_port_dongle.StartReceiveData(ProccessDongleData);
                if (ToolVersion == Version.Normal) { }
                else
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
                        filePath = logPath + $"\\{ByteConverter.ToHexString(curDevice.mac).Replace(' ', '-')}_{writeLogTiming.ToString("MMdd_HHmmss")}.csv";
                    }
                    else
                    {
                        string regex = "([\\/:*?\"<>|])";
                        if (Regex.IsMatch(logFileName, regex))
                        {
                            MessageBox.Show("File Name not access,please edit .ini file and try again."); return;
                        }
                        filePath = logPath + $"\\{logFileName}_{writeLogTiming.ToString("MMdd_HHmmss")}.csv";
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
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            dUse duse;
            flagPlayRecord = false;
            string[] s_arr = null;
            //arrPos = 0;

            //m_fft_in = null;
            //m_fft_out = null;
            //m_fft_out2 = null;

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
            for (int i = 0; i < pLogList.Length; i++)
            {
                pLogList[i] = new PointPairList();
            }
            lastPos = s_arr.Length - 1;
            //btnLoadFile.Enabled = false;
            //btnLoadFile.Text = text.LoadingFile;
            btnLoadFile.Text = text.LoadingFile;
            Thread t = new Thread(() =>
            {
                logXyz = LogDataProccess(s_arr);
                double h = 0.0;
                foreach (PointXYX p in logXyz)
                {
                    pLogList[0].Add(h, p._x);
                    pLogList[1].Add(h, p._y);
                    pLogList[2].Add(h, p._z);
                    h += 0.001;
                }
                zgcAxisX.GraphPane.XAxis.Scale.Min = 0;
                zgcAxisY.GraphPane.XAxis.Scale.Min = 0;
                zgcAxisZ.GraphPane.XAxis.Scale.Min = 0;
                zgcAxisX.GraphPane.XAxis.Scale.Max = 4;
                zgcAxisY.GraphPane.XAxis.Scale.Max = 4;
                zgcAxisZ.GraphPane.XAxis.Scale.Max = 4;

                PaintCurve(pLogList[0], zgcAxisX);
                PaintCurve(pLogList[1], zgcAxisY);
                PaintCurve(pLogList[2], zgcAxisZ);

                Invoke(duse = () => { btnLoadFile.Text = text.LoadFile; });
            })
            //Thread t = new Thread(() =>
            //{
            //    logXyz = LogDataProccess(s_arr);
            //    Invoke(duse = () =>
            //    {
            //        switch (btnPlayPause.Text)
            //        {
            //            case text.RecordPlay:
            //                flgRecordPause = true;
            //                break;
            //            case text.RecordPuase:
            //                flgRecordPause = false;
            //                tmrPaintLog.Start();
            //                break;
            //        }
            //        btnLoadFile.Text = text.LoadFile;
            //    });
            //    PaintLog();
            //})
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

                        double hz = 0.0f, m_hz = 2048.0;
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

                                pLogFftList[0] = new PointPairList();
                                pLogFftList[1] = new PointPairList();
                                pLogFftList[2] = new PointPairList();
                                for (int i = 1; i < m_amplitude_x.Length; i++)
                                {
                                    pLogFftList[0].Add(hz * i, m_amplitude_x[i]);
                                    pLogFftList[1].Add(hz * i, m_amplitude_y[i]);
                                    pLogFftList[2].Add(hz * i, m_amplitude_z[i]);
                                }
                                PaintBar(zgcFftX, pLogFftList[0]);
                                PaintBar(zgcFftY, pLogFftList[1]);
                                PaintBar(zgcFftZ, pLogFftList[2]);
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
                //if (curDevice != null)
                //{
                //    if (curDevice == BleDevice.DeviceList[lbMac.SelectedItem.ToString()]) { return; }
                //    serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StartScan, 0x01));
                //    Thread.Sleep(200);
                //    //serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.Disconnect, curDevice.mac));
                //    //Thread.Sleep(200);
                //}
                //else
                //{
                //    serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StartScan, 0x01));
                //    Thread.Sleep(200);
                //}

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

            formSize = new Size(Width, Height);
        }

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
            List<byte> sendData = new List<byte>(/*new List<byte>() { 0x01, 0x01 }*/);
            sendData.AddRange(mac);
            //serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.SetAutoList, sendData));
            serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.Connect, sendData));
            return true;
        }
        bool Reconnect(List<byte> mac)
        {
            if (mac.Count != 6) return false;
            serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.Disconnect, mac));
            Thread.Sleep(200);
            serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StartScan, 0x00));
            //serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StartScan, 0x01));
            Thread.Sleep(200);
            Connect(mac);
            Thread.Sleep(200);

            return true;
        }

        void CloseAll()
        {
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
                //WriteLog.Console("Dongle Data", $"CMD:{dongle.cmd.ToString("X2")},Data:{ByteConverter.ToHexString(dongle.data)}");

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
                        if (flagIsOldFw)
                        {
                            curDevice.receiveA5 = true;
                            curDevice.connecting = true;
                            flagReceiving = true;
                        }
                        //End
                        if (!PaintXYZ.IsAlive)
                        {
                            PaintXYZ = new Thread(PaintData) { IsBackground = true };
                            PaintXYZ.Start();
                        }
                        if (curDevice.receiveA5 == false) break;
                        byte[] tData = dongle.data.GetRange(6, dongle.data.Count - 6).ToArray();
                        if (tData[0] == 0x25 && tData.Length == 20)
                        {
                            for (int i = 1; i < 19; i += 6)
                            {
                                try
                                {
                                    point.Enqueue(new SourceXYX(
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
                        //if (dongle.data[0] == 0x01)
                        //{
                        //    curDevice.inConnectList = true;
                        //}
                        //else { curDevice.inConnectList = false; }
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

        //int arrPos = 0;
        //FFT_Complex[] logFftX, logFftY, logFftZ;
        //private void tmrPaintLog_Tick(object sender, EventArgs e)
        //{
        //    double t = 0.001 * tmrPaintLog.Interval;

        //    ZgcMoveHorize(zgcAxisX, t);
        //    ZgcMoveHorize(zgcAxisY, t);
        //    ZgcMoveHorize(zgcAxisZ, t);

        //    if (logFftX is null)
        //    {
        //        logFftX = new FFT_Complex[printDataLen];
        //        logFftY = new FFT_Complex[printDataLen];
        //        logFftZ = new FFT_Complex[printDataLen];
        //    }
        //    if (logXyz is null) { return; }
        //    if (logXyz.Count == 0) { return; }

        //    double hz = 0.0f, m_hz = 2048.0;
        //    double[] m_amplitude_x = new double[printDataLen], m_amplitude_y = new double[printDataLen], m_amplitude_z = new double[printDataLen];
        //    FFT_Complex[] m_fft_out_x = new FFT_Complex[printDataLen], m_fft_out_y = new FFT_Complex[printDataLen], m_fft_out_z = new FFT_Complex[printDataLen];

        //    //Console.WriteLine(DateTime.Now.ToString("HHmmss.fff"));
        //    int fftArrayCount = 0;

        //    FFT_Complex_Rotate.ArrRotate(logFftX, fftArrayCount);
        //    FFT_Complex_Rotate.ArrRotate(logFftY, fftArrayCount);
        //    FFT_Complex_Rotate.ArrRotate(logFftZ, fftArrayCount);
        //    FFT_translateToSpectrum(out hz, out m_amplitude_x[0], out m_fft_out_x[0], logFftX, logFftX.Length, m_hz);
        //    FFT_translateToSpectrum(out hz, out m_amplitude_y[0], out m_fft_out_y[0], logFftY, logFftY.Length, m_hz);
        //    FFT_translateToSpectrum(out hz, out m_amplitude_z[0], out m_fft_out_z[0], logFftZ, logFftZ.Length, m_hz);

        //    pLogFftList[0] = new PointPairList();
        //    pLogFftList[1] = new PointPairList();
        //    pLogFftList[2] = new PointPairList();
        //    for (int i = 1; i < m_amplitude_x.Length; i++)
        //    {
        //        pLogFftList[0].Add(hz * i, m_amplitude_x[i]);
        //        pLogFftList[1].Add(hz * i, m_amplitude_y[i]);
        //        pLogFftList[2].Add(hz * i, m_amplitude_z[i]);
        //    }
        //}
        //private void tmrPaintLog_Tick(object sender, EventArgs e)
        //{
        //    if (logFftX is null)
        //    {
        //        logFftX = new FFT_Complex[printDataLen];
        //        logFftY = new FFT_Complex[printDataLen];
        //        logFftZ = new FFT_Complex[printDataLen];
        //    }
        //    if (logXyz is null) { return; }
        //    if (logXyz.Count == 0) { return; }
        //    double h;
        //    int maxLen = logXyz.Count;
        //    int range = arrPos + defaultInterval;

        //    //FFT_Complex[] logFftX, logFftY, logFftZ;

        //    //logFftX = new FFT_Complex[dataLen];
        //    //logFftY = new FFT_Complex[dataLen];
        //    //logFftZ = new FFT_Complex[dataLen];
        //    double hz = 0.0f, m_hz = 2048.0;
        //    double[] m_amplitude_x = new double[printDataLen], m_amplitude_y = new double[printDataLen], m_amplitude_z = new double[printDataLen];
        //    FFT_Complex[] m_fft_out_x = new FFT_Complex[printDataLen], m_fft_out_y = new FFT_Complex[printDataLen], m_fft_out_z = new FFT_Complex[printDataLen];

        //    //Console.WriteLine(DateTime.Now.ToString("HHmmss.fff"));
        //    int fftArrayCount = 0;
        //    do
        //    {
        //        if (arrPos >= maxLen)
        //        {
        //            flgPlayRecord = false;
        //            tmrPaintLog.Stop();
        //            break;
        //        }
        //        h = arrPos / timeScale;
        //        pLogList[0].Add(h, logXyz[arrPos]._x);
        //        pLogList[1].Add(h, logXyz[arrPos]._y);
        //        pLogList[2].Add(h, logXyz[arrPos]._z);
        //        logFftX[fftArrayCount] = new FFT_Complex(logXyz[arrPos]._x, 0.0f);
        //        logFftY[fftArrayCount] = new FFT_Complex(logXyz[arrPos]._y, 0.0f);
        //        logFftZ[fftArrayCount] = new FFT_Complex(logXyz[arrPos]._z, 0.0f);
        //        if (++fftArrayCount >= printDataLen) fftArrayCount = 0;
        //    } while (arrPos++ < range);
        //    int dataInterval = pLogList[0].Count - printDataLen;
        //    if (dataInterval > 0)
        //    {
        //        pLogList[0].RemoveRange(0, dataInterval);
        //        pLogList[1].RemoveRange(0, dataInterval);
        //        pLogList[2].RemoveRange(0, dataInterval);
        //    }

        //    FFT_Complex_Rotate.ArrRotate(logFftX, fftArrayCount);
        //    FFT_Complex_Rotate.ArrRotate(logFftY, fftArrayCount);
        //    FFT_Complex_Rotate.ArrRotate(logFftZ, fftArrayCount);
        //    FFT_translateToSpectrum(out hz, out m_amplitude_x[0], out m_fft_out_x[0], logFftX, logFftX.Length, m_hz);
        //    FFT_translateToSpectrum(out hz, out m_amplitude_y[0], out m_fft_out_y[0], logFftY, logFftY.Length, m_hz);
        //    FFT_translateToSpectrum(out hz, out m_amplitude_z[0], out m_fft_out_z[0], logFftZ, logFftZ.Length, m_hz);

        //    pLogFftList[0] = new PointPairList();
        //    pLogFftList[1] = new PointPairList();
        //    pLogFftList[2] = new PointPairList();
        //    for (int i = 1; i < m_amplitude_x.Length; i++)
        //    {
        //        pLogFftList[0].Add(hz * i, m_amplitude_x[i]);
        //        pLogFftList[1].Add(hz * i, m_amplitude_y[i]);
        //        pLogFftList[2].Add(hz * i, m_amplitude_z[i]);
        //    }
        //}

        string LoadIniFile()
        {
            //WritePrivateProfileString("log", "test", "ttt", iniPath);
            StringBuilder result = new StringBuilder(255);
            GetPrivateProfileString("log", "file name", "123", result, 255, iniPath);
            return result.ToString();
        }
        void PaintData()
        {
            #region Declare Zone
            dUse duse;
            UInt32 SN = 0;
            DateTime startTime = DateTime.Now;
            writeLogTiming = DateTime.Now;
            filePath = logPath + "\\" + $"{ByteConverter.ToHexString(curDevice.mac)}_{writeLogTiming.ToString("MMdd_HHmmss")}.csv";

            int count = 0;
            double axisH = 0;
            FFT_Complex[] fftX, fftY, fftZ;
            double[] m_amplitude_x = new double[printDataLen], m_amplitude_y = new double[printDataLen], m_amplitude_z = new double[printDataLen];
            FFT_Complex[] m_fft_out_x = new FFT_Complex[printDataLen], m_fft_out_y = new FFT_Complex[printDataLen], m_fft_out_z = new FFT_Complex[printDataLen];

            #endregion

            #region Init Zone
            fftX = new FFT_Complex[printDataLen];
            fftY = new FFT_Complex[printDataLen];
            fftZ = new FFT_Complex[printDataLen];

            for (int i = 0; i < pLogList.Length; i++)
            {
                pLogList[i] = new PointPairList();
            }

            axisH = 0;
            zgcFftX.GraphPane.XAxis.Scale.Min = -2;
            zgcFftX.GraphPane.XAxis.Scale.Max = printDataLen + 20;
            zgcFftY.GraphPane.XAxis.Scale.Min = -2;
            zgcFftY.GraphPane.XAxis.Scale.Max = printDataLen + 20;
            zgcFftZ.GraphPane.XAxis.Scale.Min = -2;
            zgcFftZ.GraphPane.XAxis.Scale.Max = printDataLen + 20;
            #endregion

            while (flagReceiving)
            {
                int fftArrayCount = 0;

                if (point.Count > 0)
                {
                    SourceXYX p;
                    double h = 0.0;
                    //double multi = /*0.001*/1 / 8192.0;
                    double div = 8192.0;
                    string log = "";
                    double x, y, z;
                    DateTime recordTime = DateTime.Now, tempTime = DateTime.Now;
                    string rcTime = "";
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

                        //由Tool來調整時間間距
                        if (tempTime == recordTime)
                        {
                            rcTime = UNIXTime.DatetimeToUnix(recordTime).ToString() + recordTime.ToString(".fff") + (++timeCount);
                            //recordTime = recordTime.AddMilliseconds(++timeCount);
                        }
                        else
                        {
                            rcTime = UNIXTime.DatetimeToUnix(recordTime).ToString() + recordTime.ToString(".fff") + "0";
                            timeCount = 0;
                            tempTime = recordTime;
                        }

                        fftX[fftArrayCount] = new FFT_Complex(x, 0.0f);
                        fftY[fftArrayCount] = new FFT_Complex(y, 0.0f);
                        fftZ[fftArrayCount] = new FFT_Complex(z, 0.0f);
                        if (++fftArrayCount >= printDataLen) fftArrayCount = 0;

                        pLogList[0].Add(h, x);
                        pLogList[1].Add(h, y);
                        pLogList[2].Add(h, z);

                        if (flagRecord)
                        {
                            log += $"{(++SN).ToString("0000000")},{rcTime},{x.ToString("F3")},{y.ToString("F3")},{z.ToString("F3")},";
                            if (flagAddMark)
                            {
                                flagAddMark = false;
                                log += "●";
                            }
                            log += "\n";
                        }
                    }
                    if (flagRecord)
                    {
                        if ((DateTime.Now - writeLogTiming).TotalMinutes >= writeLogInterval)
                        {
                            writeLogTiming = writeLogTiming.AddMinutes(writeLogInterval);
                        }
                        AsyncFileAccess.WriteText(filePath, log);
                    }
                    FFT_Complex_Rotate.ArrRotate(fftX, fftArrayCount);
                    FFT_Complex_Rotate.ArrRotate(fftY, fftArrayCount);
                    FFT_Complex_Rotate.ArrRotate(fftZ, fftArrayCount);

                    for (int i = 0; i < pLogList.Length; i++)
                    {
                        if (pLogList[i].Count > rawDataLen)
                        {
                            pLogList[i].RemoveRange(0, pLogList[i].Count - rawDataLen);
                        }
                    }
                    #endregion

                    double
                        max = h,
                        min = h - printDataLen / oneSecondScale;
                    if (!flagPause)
                    {
                        int pStart = pLogList[0].Count - printDataLen;
                        int amount = printDataLen;
                        if (pStart < 0)
                        {
                            pStart = 0;
                            amount = pLogList[0].Count;
                        }
                        PointPairList[] a = new PointPairList[3];
                        for (int i = 0; i < a.Length; i++) { a[i] = new PointPairList(); a[i].AddRange(pLogList[i]); }
                        zgcAxisX.GraphPane.XAxis.Scale.Max = max;
                        zgcAxisX.GraphPane.XAxis.Scale.Min = min;
                        zgcAxisY.GraphPane.XAxis.Scale.Max = max;
                        zgcAxisY.GraphPane.XAxis.Scale.Min = min;
                        zgcAxisZ.GraphPane.XAxis.Scale.Max = max;
                        zgcAxisZ.GraphPane.XAxis.Scale.Min = min;
                        //PaintCurve(pLogList[0], zgcAxisX);
                        //PaintCurve(pLogList[1], zgcAxisY);
                        //PaintCurve(pLogList[2], zgcAxisZ);
                        PaintCurve(a[0], zgcAxisX);
                        PaintCurve(a[1], zgcAxisY);
                        PaintCurve(a[2], zgcAxisZ);

                        if (count++ > 10)
                        {
                            #region FFT calculate and paint
                            FFT_translateToSpectrum(out hz, out m_amplitude_x[0], out m_fft_out_x[0], fftX, fftX.Length, m_hz);
                            FFT_translateToSpectrum(out hz, out m_amplitude_y[0], out m_fft_out_y[0], fftY, fftY.Length, m_hz);
                            FFT_translateToSpectrum(out hz, out m_amplitude_z[0], out m_fft_out_z[0], fftZ, fftZ.Length, m_hz);
                            GraphPane pane;
                            PointPairList[] list = new PointPairList[3];
                            list[0] = new PointPairList();
                            list[1] = new PointPairList();
                            list[2] = new PointPairList();
                            for (int i = 1; i < m_amplitude_x.Length; i++)
                            {
                                list[0].Add(hz * i, m_amplitude_x[i]);
                                list[1].Add(hz * i, m_amplitude_y[i]);
                                list[2].Add(hz * i, m_amplitude_z[i]);
                            }

                            try
                            {
                                PaintBar(zgcFftX, list[0]);
                                PaintBar(zgcFftY, list[1]);
                                PaintBar(zgcFftZ, list[2]);
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
        void PaintLog()
        {
            flagPlayRecord = true;
            //int fftCount = 0;
            while (flagPlayRecord)
            {
                Thread.Sleep(20);

                #region Paint Grapgh
                //if (pLogList is null) { return; }
                //if (flgRecordPause) { continue; }
                //if (pLogList[0].Count == 0 || pLogList[1].Count == 0 || pLogList[2].Count == 0) { continue; }
                //PointPair a = pLogList[0].Last();
                //double h = pLogList[0].Last().X;
                //double
                //    max = h,
                //    min = h - printDataLen / timeScale;

                ////dUse duse;
                //zgcAxisX.GraphPane.XAxis.Scale.Max = max;
                //zgcAxisX.GraphPane.XAxis.Scale.Min = min;
                //zgcAxisY.GraphPane.XAxis.Scale.Max = max;
                //zgcAxisY.GraphPane.XAxis.Scale.Min = min;
                //zgcAxisZ.GraphPane.XAxis.Scale.Max = max;
                //zgcAxisZ.GraphPane.XAxis.Scale.Min = min;
                //try
                //{
                //    PaintCurve(pLogList[0], zgcAxisX);
                //    PaintCurve(pLogList[1], zgcAxisY);
                //    PaintCurve(pLogList[2], zgcAxisZ);
                //}
                //catch
                //{
                //    return;
                //}
                #endregion

                PaintBar(zgcFftX, pLogFftList[0]);
                PaintBar(zgcFftY, pLogFftList[1]);
                PaintBar(zgcFftZ, pLogFftList[2]);
            }
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

        private void btnSetAlarm_Click(object sender, EventArgs e)
        {
            frmSetAlarm frmSet = new frmSetAlarm();
            frmSet.ShowDialog();
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
                //Invoke(duse = () =>
                //{
                //    CloseAll();
                //});
            }
            return true;
        }
        bool PaintBar(ZedGraphControl zgc, PointPairList ppl)
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

            pane = zgcFftX.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcFftX.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcFftX.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcAxisX.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcAxisY.GraphPane;
            pane.GraphObjList.Clear();
            pane.CurveList.Clear();

            pane = zgcAxisZ.GraphPane;
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
                    if (split2.Length != 6) continue;
                    result.Add(new PointXYX(double.Parse(split2[2]), double.Parse(split2[3]), double.Parse(split2[4])));
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
    }

    public partial class frmSetAlarm : Form
    {
        public class xyzLimit
        {
            double[]
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
            }
        }
        public xyzLimit rtnValue;
        public frmSetAlarm()
        {
            initForm();
            TextBox[][] tbxInputArray = new TextBox[3][];
            Point startLoc = new Point(20, 30);
            for (int i = 0; i < tbxInputArray.GetLength(0); i++)
            {
                tbxInputArray[i] = new TextBox[2];
                for (int j = 0; j < tbxInputArray[i].Length; j++)
                {
                    tbxInputArray[i][j] = new TextBox()
                    {
                        Text = (i * 2 + j).ToString()
                    };
                    tbxInputArray[i][j].Location = new Point(j * (tbxInputArray[0][0].Width + 10) + startLoc.X, i * 50 + startLoc.Y);
                    Controls.Add(tbxInputArray[i][j]);
                }
            }

            Button btnTest = new Button()
            {
                Location = new Point(50, 50),
                Text = "try it"
            };

            btnTest.Click += (o, e) =>
            {
                double[] input = new double[tbxInputArray.Length];
                //for(int i = 0;i<)

                rtnValue = new xyzLimit(1, 2, 3, 4, 5, 6);
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.Add(btnTest);
        }
        void initForm()
        {
            Font = new Font("Consolas", 12.0f);
        }
    }
}
