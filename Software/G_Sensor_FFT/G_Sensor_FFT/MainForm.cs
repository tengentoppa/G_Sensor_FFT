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
        UART serial_port_dongle;
        BleDevice curDevice;
        delegate void dUse();
        List<string> _connectedMac = new List<string>();
        Queue<PointXYX> point = new Queue<PointXYX>();
        List<string> portList = new List<string>();
        bool flgReceiving = false, flgPause = false;
        string logPath = Application.StartupPath + @"\log";
        string filePath;
        Thread PaintXYZ;

        int rangeMax = 0, samples = 64, startPos = 0, lastPos = 0;
        FFT_Complex[] m_fft_in = null, m_fft_out = null, m_fft_out2 = null;

        [System.Runtime.InteropServices.DllImport("MP-FFT.dll", EntryPoint = "FFT_translate", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        extern static System.Int32 FFT_translate(out FFT_Complex p_dst, FFT_Complex[] p_src, System.Int32 num_of_points);

        [System.Runtime.InteropServices.DllImport("MP-FFT.dll", EntryPoint = "FFT_convertToMagnitude", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        extern static System.Int32 FFT_convertToMagnitude(out System.Double p_dst, FFT_Complex[] p_src, System.Int32 num_of_points);

        [System.Runtime.InteropServices.DllImport("MP-FFT.dll", EntryPoint = "FFT_translateToSpectrum", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        extern static System.Int32 FFT_translateToSpectrum(out System.Double p_delte_hz, out System.Double p_amplitude, out FFT_Complex p_dst, FFT_Complex[] p_src, System.Int32 num_of_points, System.Double hz);

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
                NoneData = "None Data";
        }

        public MainForm()
        {
            InitializeComponent();
            InitInterface();
            SubscribInit();
            SearchPort();

            PaintXYZ = new Thread(PaintToGrapgh) { IsBackground = true };
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }

        void InitInterface()
        {
            Text = "Yoke Reader Production Tool  v" + Application.ProductVersion;

            foreach (Control c in Controls)
            {
                c.Font = new Font("Consolas", 10f);
            }
            btnCom.Text = text.OpenCom;
            btnScan.Enabled = false;
            lbMac.Enabled = false;
            btnPause.Enabled = false;

            btnCom.Top = cbxCom.Bottom + 10;
            btnScan.Top = btnCom.Bottom + 30;
            lbMac.Top = btnScan.Bottom + 10;
            btnPause.Top = lbMac.Bottom + 30;

            btnLoadFile.Top = btnPause.Bottom + 50;
            tbxStartPos.Top = btnLoadFile.Bottom + 10;
            cbxSamples.Top = tbxStartPos.Bottom + 10;

            Height = zgcFftZ.Bottom + 50;
            Width = zgcFftX.Right + 30;

            MinimumSize = new Size(Width, Height);

            zgcAxisX.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);
            zgcAxisY.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);
            zgcAxisZ.GraphPane.Chart.Fill = new Fill(Color.Gray, Color.Gray);

            GraphPane tmpGP;
            tmpGP = zgcAxisX.GraphPane;
            tmpGP.Title.Text = "X軸";
            tmpGP.XAxis.Title.Text = "時間(s)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.YAxis.Scale.Max = 15.0;
            tmpGP.YAxis.Scale.Min = -15.0;

            tmpGP = zgcAxisY.GraphPane;
            tmpGP.Title.Text = "Y軸";
            tmpGP.XAxis.Title.Text = "時間(s)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.YAxis.Scale.Max = 15.0;
            tmpGP.YAxis.Scale.Min = -15.0;

            tmpGP = zgcAxisZ.GraphPane;
            tmpGP.Title.Text = "Z軸";
            tmpGP.XAxis.Title.Text = "時間(s)";
            tmpGP.YAxis.Title.Text = "震幅";
            tmpGP.YAxis.Scale.Max = 15.0;
            tmpGP.YAxis.Scale.Min = -15.0;
        }

        void SubscribInit()
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
                    cbxSamples.Enabled = false;
                    tbxStartPos.Enabled = false;
                }
                else
                {
                    cbxCom.Enabled = true;
                    btnScan.Enabled = false;
                    lbMac.Enabled = false;
                    btnPause.Enabled = false;
                    btnLoadFile.Enabled = true;
                    cbxSamples.Enabled = true;
                    tbxStartPos.Enabled = true;
                }
            };
        }

        DateTime writeLogTiming;
        double writeLogInterval = 3.0;
        int dataLen = 1024;
        PointPairList[] pList = new PointPairList[3];
        void PaintToGrapgh()
        {
            writeLogTiming = DateTime.Now;
            filePath = logPath + "\\" + $"{ByteConverter.ToHexString(curDevice.mac)}_{writeLogTiming.ToString("MMdd_HHmm")}.csv";

            double axisH = 0;
            FFT_Complex[] fftX, fftY, fftZ;

            fftX = new FFT_Complex[dataLen];
            fftY = new FFT_Complex[dataLen];
            fftZ = new FFT_Complex[dataLen];

            for (int i = 0; i < pList.Length; i++)
            {
                pList[i] = new PointPairList();
            }
            int count = 0;

            axisH = 0;
            double hz = 0.0f, m_hz = 2048.0;
            double[] m_amplitude_x = new double[dataLen], m_amplitude_y = new double[dataLen], m_amplitude_z = new double[dataLen];
            FFT_Complex[] m_fft_out_x = new FFT_Complex[dataLen], m_fft_out_y = new FFT_Complex[dataLen], m_fft_out_z = new FFT_Complex[dataLen];

            zgcFftX.GraphPane.XAxis.Scale.Min = -2;
            zgcFftX.GraphPane.XAxis.Scale.Max = dataLen + 20;
            zgcFftY.GraphPane.XAxis.Scale.Min = -2;
            zgcFftY.GraphPane.XAxis.Scale.Max = dataLen + 20;
            zgcFftZ.GraphPane.XAxis.Scale.Min = -2;
            zgcFftZ.GraphPane.XAxis.Scale.Max = dataLen + 20;

            while (flgReceiving)
            {
                int fftArrayCount = 0;


                if (point.Count > 0)
                {
                    PointXYX p;
                    double h = 0.0;
                    double timeScale = 1000; //ms
                    double multi = 0.001;
                    string log = "";
                    double x, y, z;

                    while (point.Count > 0)
                    {
                        p = point.Dequeue();
                        if (p is null) { continue; }
                        axisH++;
                        h = axisH / timeScale;
                        x = p._x * multi; y = p._y * multi; z = p._z * multi;

                        fftX[fftArrayCount] = new FFT_Complex(x, 0.0f);
                        fftY[fftArrayCount] = new FFT_Complex(y, 0.0f);
                        fftZ[fftArrayCount] = new FFT_Complex(z, 0.0f);
                        if (++fftArrayCount >= dataLen) fftArrayCount = 0;

                        pList[0].Add(h, x);
                        pList[1].Add(h, y);
                        pList[2].Add(h, z);

                        log += $"{x.ToString("F3")},{y.ToString("F3")},{z.ToString("F3")}\n";
                    }
                    if ((DateTime.Now - writeLogTiming).TotalMinutes >= writeLogInterval)
                    {
                        writeLogTiming = writeLogTiming.AddMinutes(writeLogInterval);
                        filePath = logPath + "\\" + $"{ByteConverter.ToHexString(curDevice.mac)}_{writeLogTiming.ToString("MMdd_HHmm")}.csv";
                    }
                    AsyncWriteFile.WriteText(filePath, log);
                    FFT_Complex_Rotate.ArrRotate(fftX, fftArrayCount);
                    FFT_Complex_Rotate.ArrRotate(fftY, fftArrayCount);
                    FFT_Complex_Rotate.ArrRotate(fftZ, fftArrayCount);

                    for (int i = 0; i < pList.Length; i++)
                    {
                        if (pList[i].Count > dataLen)
                        {
                            pList[i].RemoveRange(0, pList[i].Count - dataLen);
                        }
                    }

                    double
                        max = h + 0.05,
                        min = h - dataLen / timeScale;
                    if (!flgPause)
                    {
                        dUse duse;
                        zgcAxisX.GraphPane.XAxis.Scale.Max = max;
                        zgcAxisX.GraphPane.XAxis.Scale.Min = min;
                        zgcAxisY.GraphPane.XAxis.Scale.Max = max;
                        zgcAxisY.GraphPane.XAxis.Scale.Min = min;
                        zgcAxisZ.GraphPane.XAxis.Scale.Max = max;
                        zgcAxisZ.GraphPane.XAxis.Scale.Min = min;
                        paintCurve(pList[0], zgcAxisX);
                        paintCurve(pList[1], zgcAxisY);
                        paintCurve(pList[2], zgcAxisZ);

                        if (count++ > 10)
                        {

                            if (fftX.Length % 1024 == 0)
                            { FFT_translateToSpectrum(out hz, out m_amplitude_x[0], out m_fft_out_x[0], fftX, fftX.Length, m_hz); }
                            if (fftY.Length % 1024 == 0)
                            { FFT_translateToSpectrum(out hz, out m_amplitude_y[0], out m_fft_out_y[0], fftY, fftY.Length, m_hz); }
                            if (fftZ.Length % 1024 == 0)
                            { FFT_translateToSpectrum(out hz, out m_amplitude_z[0], out m_fft_out_z[0], fftZ, fftZ.Length, m_hz); }
                            GraphPane pane;
                            PointPairList[] list = new PointPairList[3];
                            list[0] = new PointPairList();
                            list[1] = new PointPairList();
                            list[2] = new PointPairList();
                            for (int i = 1; i < m_amplitude_x.Length; i++)
                            {
                                list[0].Add(hz * i, m_amplitude_x[i]);
                            }
                            for (int i = 1; i < m_amplitude_y.Length; i++)
                            {
                                list[1].Add(hz * i, m_amplitude_y[i]);
                            }
                            for (int i = 1; i < m_amplitude_z.Length; i++)
                            {
                                list[2].Add(hz * i, m_amplitude_z[i]);
                            }

                            try
                            {
                                pane = zgcFftX.GraphPane;
                                pane.GraphObjList.Clear();
                                pane.CurveList.Clear();
                                pane.AddBar("", list[0], Color.Gray);
                                zgcFftX.AxisChange();
                                Invoke(duse = () =>
                                {
                                    zgcFftX.Refresh();
                                });

                                pane = zgcFftY.GraphPane;
                                pane.GraphObjList.Clear();
                                pane.CurveList.Clear();
                                pane.AddBar("", list[1], Color.Gray);
                                zgcFftY.AxisChange();
                                Invoke(duse = () =>
                                {
                                    zgcFftY.Refresh();
                                });

                                pane = zgcFftZ.GraphPane;
                                pane.GraphObjList.Clear();
                                pane.CurveList.Clear();
                                pane.AddBar("", list[2], Color.Gray);
                                zgcFftZ.AxisChange();
                                Invoke(duse = () =>
                                {
                                    zgcFftZ.Refresh();
                                });
                            }
                            catch { }
                        }

                        Thread.Sleep(10);
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

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
                    if(serial_port_dongle is null) { return; }
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

        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            if (btnCom.Text == text.OpenCom)
            {
                UART.Statue comStatue = UART.Statue.Null;

                if (!cbxCom.Text.Contains("COM")) return;

                serial_port_dongle = new UART(cbxCom.Text, 921600);

                if ((comStatue = serial_port_dongle.Open()) != UART.Statue.Success)
                { /*MessageBox.Show(comStatue.ToString(), "Open Com Denied");*/ return; }

                serial_port_dongle.StartReceiveData(ProccessDongleData);
                StartScan();
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

        void CloseAll()
        {
            flgReceiving = false;
            flgPause = false;
            btnPause.Text = text.Pause;
            lbMac.Items.Clear();
            BleDevice.DeviceList.Clear();
            if (serial_port_dongle != null)
            {
                if (curDevice != null)
                {
                    serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.Disconnect, curDevice.mac));
                }
                serial_port_dongle.StopReceiveData();
                serial_port_dongle.Close();
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

        void StartScan()
        {
            serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StartScan, null));
            btnScan.Text = text.StopScan;
        }
        void StopScan()
        {
            serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.StopScan, null));
            btnScan.Text = text.Scan;
        }

        private void lbMac_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lbMac.SelectedItem != null)
            {
                if (curDevice != null)
                {
                    PaintXYZ.Abort();
                    serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.Disconnect, curDevice.mac));
                    Thread.Sleep(100);
                    StartScan();
                    Thread.Sleep(100);
                }
                //lbMac.Enabled = false;
                //btnScan.Enabled = false;
                curDevice = BleDevice.DeviceList[lbMac.SelectedItem.ToString()];
                serial_port_dongle.SendData(BLEDongle.PackageData((byte)BLEDongle.CMD.Connect, curDevice.mac));
                btnScan.Text = text.Scan;
                flgReceiving = true;
                //PaintXYZ = new Thread(PaintToGrapgh) { IsBackground = true };
                //PaintXYZ.Start();
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
                WriteLog.Console("Dongle Data", $"CMD:{dongle.cmd.ToString("X2")},Data:{ByteConverter.ToHexString(dongle.data)}");

                switch (dongle.cmd)
                {
                    case 0xA2:
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

                    case 0xA3:
                        if (curDevice is null) break;
                        if (ByteConverter.ToHexString(curDevice.mac) == ByteConverter.ToHexString(dongle.data))
                        {
                            curDevice.receiveA5 = true;
                            curDevice.connecting = true;
                            if (PaintXYZ.IsAlive) { PaintXYZ.Abort(); Thread.Sleep(10); }
                            PaintXYZ = new Thread(PaintToGrapgh) { IsBackground = true };
                            PaintXYZ.Start();
                        }
                        break;

                    case 0xA4:
                        if (curDevice is null) break;
                        if (ByteConverter.ToHexString(curDevice.mac) == ByteConverter.ToHexString(dongle.data))
                        {
                            curDevice.receiveA5 = false;
                            curDevice.connecting = false;
                        }
                        break;

                    case 0xA5:
                        if (curDevice is null) break;
                        if (curDevice.receiveA5 == false) break;
                        byte[] tData = dongle.data.GetRange(6, dongle.data.Count - 6).ToArray();
                        if (tData[0] == 0x25 && tData.Length == 20)
                        {
                            for (int i = 1; i < 19; i += 6)
                            {
                                try
                                {
                                    point.Enqueue(new PointXYX(
                                        (((sbyte)tData[0 + i]) * 256 + tData[1 + i]),
                                        (((sbyte)tData[2 + i]) * 256 + tData[3 + i]),
                                        (((sbyte)tData[4 + i]) * 256 + tData[5 + i])));
                                }
                                catch (Exception) { }
                            }
                        }
                        break;

                    case 0xA6:
                        if (dongle.data[6] == 0x01)
                        {
                            curDevice.connecting = true;
                            curDevice.receiveA5 = true;
                            PaintXYZ.Start();
                        }
                        else
                        {
                            curDevice.connecting = false;
                            curDevice.receiveA5 = false;
                            PaintXYZ.Abort();
                        }
                        break;

                        //case 0xAA:
                        //    if (curDevice is null)
                        //    {
                        //        return;
                        //    }
                        //    if (ByteConverter.ToHexString(dongle.data.GetRange(0, 6)) == ByteConverter.ToHexString(curDevice.mac))
                        //    {
                        //        PaintXYZ.Start();
                        //        curDevice.connecting = true;
                        //        WriteLog.Console("Connecting", curDevice.name);
                        //    }
                        //    break;

                        //case 0xB1:
                        //    if (dongle.data[0] == 0x01)
                        //    {
                        //        curDevice.inConnectList = true;
                        //    }
                        //    else { curDevice.inConnectList = false; }
                        //    break;
                        //case 0xC1:
                        //    if (dongle.data[2] != 0x01) return;
                        //    string mac = ByteConverter.ToHexString(dongle.data.GetRange(3, 6));
                        //    if (!_connectedMac.Contains(mac))
                        //        _connectedMac.Add(mac);
                        //    break;
                }
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseAll();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (btnPause.Text == text.Resume)
            {
                flgPause = false;
                btnPause.Text = text.Pause;
            }
            else
            {
                flgPause = true;
                btnPause.Text = text.Resume;
            }
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            string[] s_arr = null;

            m_fft_in = null;
            m_fft_out = null;
            m_fft_out2 = null;

            if (ofdInput.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            s_arr = File.ReadAllLines(ofdInput.FileName);
            if (s_arr.Length < 64)
            {
                MessageBox.Show("請引入筆數大於等於64筆的資料");
                return;
            }
            lastPos = s_arr.Length - 1;
            //m_hz = double.Parse(s_arr[0]);
            //list = new PointPairList[2];
            //m_fft_in = new FFT_Complex[s_arr.Length - 1];
            //m_fft_out = new FFT_Complex[s_arr.Length - 1];
            //m_fft_out2 = new FFT_Complex[s_arr.Length - 1];
            //m_real = new double[s_arr.Length - 1];
            //m_amplitude = new double[(s_arr.Length - 1) / 2];

            //GraphPane pane = zgcOrigin.GraphPane;

            //list[0] = new PointPairList();
            //list[1] = new PointPairList();
            //for (int i = 0; i < (s_arr.Length - 1); i++)
            //{
            //    string[] arr = s_arr[i + 1].Split(',');

            //    m_fft_in[i].real = double.Parse(arr[0]);
            //    m_fft_in[i].imag = double.Parse(arr[1]);

            //    list[0].Add(i + 1, m_fft_in[i].real);
            //    list[1].Add(i + 1, m_fft_in[i].imag);
            //}

            //paintCurve(list, zgcOrigin);
            //FFTtoGraph(m_fft_in.ToList().GetRange(startPos, samples).ToArray());

            //int sampleAmountList = 64;
            //cbxSamples.Items.Clear();
            //while (sampleAmountList <= s_arr.Length)
            //{
            //    cbxSamples.Items.Add(sampleAmountList);
            //    sampleAmountList *= 2;
            //}
            //cbxSamples.SelectedIndex = 0;

            //rangeMax = lastPos - int.Parse(cbxSamples.Text);
            //tbxStartPos.Text = "0";
        }

        private void tbxStartPos_TextChanged(object sender, EventArgs e)
        {
            if (tbxStartPos.Text == "")
            {
                return;
            }

            if (m_fft_in is null)
            {
                tbxStartPos.Text = 0.ToString();
                return;
            }

            if (!int.TryParse(tbxStartPos.Text, out startPos))
            {
                tbxStartPos.Text = startPos.ToString();
                return;
            }

            if (startPos < 0)
            {
                startPos = 0;
            }

            if (startPos > rangeMax)
            {
                startPos = rangeMax;
            }

            //tbxStartPos.Text = startPos.ToString();
            //List<PointPairList> tmpPointList = new List<PointPairList>();
            //foreach (PointPairList t in list)
            //{
            //    PointPairList ttt = new PointPairList();
            //    ttt.AddRange(t.GetRange(startPos, samples));
            //    tmpPointList.Add(ttt);
            //}

            //zgcOrigin.GraphPane.CurveList.Clear();
            //paintCurve(tmpPointList.ToArray(), zgcOrigin);
            //FFTtoGraph(m_fft_in.ToList().GetRange(startPos, samples).ToArray());
        }

        private void cbxSample_SelectedIndexChanged(object sender, EventArgs e)
        {
            int temp = lastPos - int.Parse(cbxSamples.Text);
            if (temp < 0)
            {
                MessageBox.Show("取樣數設定超過引入檔案大小");
                return;
            }
            samples = int.Parse(cbxSamples.Text);
            rangeMax = temp;
            tbxStartPos_TextChanged(null, null);
        }

        bool paintCurve(PointPairList[] pointListArray, ZedGraphControl paneArea)
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
            if (flgReceiving)
            {
                Invoke(duse = () =>
                {
                    paneArea.Refresh();
                });
            }
            return true;
        }

        bool paintCurve(PointPairList pointList, ZedGraphControl paneArea)
        {
            dUse duse;
            Color color = Color.LawnGreen;
            paneArea.GraphPane.CurveList.Clear();
            paneArea.GraphPane.AddCurve("", pointList, color, SymbolType.None);
            paneArea.AxisChange();
            if (flgReceiving)
            {
                try
                {
                    Invoke(duse = () =>
                    {
                        paneArea.Refresh();
                    });
                }
                catch
                {
                    Invoke(duse = () =>
                    {
                        CloseAll();
                    });
                }
                return true;
            }
            return false;
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

        public static class AsyncWriteFile
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
                catch (Exception e) { WriteLog.Console("Wtrite File", e.ToString()); goto reWrite; }
            }

        }

        class PointXYX
        {
            public /*UInt16*/int _x, _y, _z;
            public PointXYX(/*UInt16 x, UInt16 y, UInt16 z*/int x, int y, int z)
            {
                _x = x;
                _y = y;
                _z = z;
            }
        }
    }
}
