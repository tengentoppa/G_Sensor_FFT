using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZedGraph;

namespace FFT_Test
{
    struct FFT_Complex
    {
        public System.Double real;
        public System.Double imag;
    }

    public partial class FmMain : Form
    {
        [System.Runtime.InteropServices.DllImport("MP-FFT.dll", EntryPoint = "FFT_translate", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        extern static System.Int32 FFT_translate(out FFT_Complex p_dst, FFT_Complex[] p_src, System.Int32 num_of_points);

        [System.Runtime.InteropServices.DllImport("MP-FFT.dll", EntryPoint = "FFT_convertToMagnitude", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        extern static System.Int32 FFT_convertToMagnitude(out System.Double p_dst, FFT_Complex[] p_src, System.Int32 num_of_points);

        [System.Runtime.InteropServices.DllImport("MP-FFT.dll", EntryPoint = "FFT_translateToSpectrum", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        extern static System.Int32 FFT_translateToSpectrum(out System.Double p_delte_hz, out System.Double p_amplitude, out FFT_Complex p_dst, FFT_Complex[] p_src, System.Int32 num_of_points, System.Double hz);


        FFT_Complex[] m_fft_in = null, m_fft_out = null, m_fft_out2 = null;
        double[] m_real = null, m_amplitude = null;
        double m_hz = 0.0f;

        public FmMain()
        {
            InitializeComponent();
        }

        private void FmMain_Load(object sender, EventArgs e)
        {
            #region Graph_sin_Setting
            GraphPane gpTest = zgcTest.GraphPane;

            gpTest.CurveList.Clear();
            gpTest.GraphObjList.Clear();
            gpTest.CurveList.Clear();
            //myPane.XAxis.CrossAuto = true;

            // Set the titles and axis labels
            gpTest.Title.FontSpec.Size = 16f;
            gpTest.Title.Text = "輸入數據";

            gpTest.XAxis.Title.Text = "t(s)";
            gpTest.XAxis.Title.FontSpec.Size = 18f;
            gpTest.YAxis.Title.FontSpec.Size = 18f;
            gpTest.XAxis.Title.FontSpec.Family = "Times New Roman";
            gpTest.YAxis.Title.FontSpec.Family = "Times New Roman";
            gpTest.YAxis.Title.Text = "sin(2πft)";

            gpTest.XAxis.Scale.FontSpec.Size = 18;
            gpTest.YAxis.Scale.FontSpec.Size = 18;

            gpTest.Border.Color = Color.Gray;
            gpTest.Border.Color = Color.FromArgb(0, 255, 255, 255);


            //顯示格點            
            gpTest.XAxis.MajorGrid.IsVisible = true;

            // Fill the axis background with a color gradient
            gpTest.Chart.Fill = new Fill(Color.White, SystemColors.Control, 45F);

            // Fill the pane background with a color gradient           
            gpTest.Fill = new Fill(SystemColors.Control);
            #endregion
        }

        private void btnFileRead_Click(object sender, EventArgs e)
        {
            string[] s_arr = null;


            lvInput.Items.Clear();
            lvOutput.Items.Clear();
            m_fft_in = null;
            m_fft_out = null;
            m_fft_out2 = null;
            m_real = null;

            chartInput.Series["實數"].Points.Clear();
            chartInput.Series["虛數"].Points.Clear();


            if (ofdInput.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            s_arr = System.IO.File.ReadAllLines(ofdInput.FileName);
            m_fft_in = new FFT_Complex[s_arr.Length - 1];
            m_fft_out = new FFT_Complex[s_arr.Length - 1];
            m_fft_out2 = new FFT_Complex[s_arr.Length - 1];
            m_real = new double[s_arr.Length - 1];
            m_amplitude = new double[(s_arr.Length - 1) / 2];

            m_hz = double.Parse(s_arr[0]);
            switch (cmbFileRead.SelectedIndex)
            {
                case 0:
                    PointPairList[] list = new PointPairList[2];
                    GraphPane pane = zgcTest.GraphPane;

                    list[0] = new PointPairList();
                    list[1] = new PointPairList();
                    for (int i = 0; i < (s_arr.Length - 1); i++)
                    {
                        string[] arr = s_arr[i + 1].Split(',');

                        m_fft_in[i].real = double.Parse(arr[0]);
                        m_fft_in[i].imag = double.Parse(arr[1]);

                        ListViewItem item = lvInput.Items.Add((i + 1).ToString());
                        item.SubItems.Add(m_fft_in[i].real.ToString());
                        item.SubItems.Add(m_fft_in[i].imag.ToString());

                        chartInput.Series["實數"].Points.AddY(m_fft_in[i].real);
                        chartInput.Series["虛數"].Points.AddY(m_fft_in[i].imag);
                        list[0].Add(i + 1, m_fft_in[i].real);
                        list[1].Add(i + 1, m_fft_in[i].imag);
                    }
                    pane.AddCurve("", list[0], Color.IndianRed, SymbolType.None);
                    pane.AddCurve("", list[1], Color.CadetBlue, SymbolType.None);
                    zgcTest.AxisChange();
                    zgcTest.Refresh();
                    break;

                case 1:
                    for (int i = 0; i < (s_arr.Length - 1); i++)
                    {

                        m_fft_in[i].real = double.Parse(s_arr[i + 1]);
                        m_fft_in[i].imag = 0.0f;

                        ListViewItem item = lvInput.Items.Add((i + 1).ToString());
                        item.SubItems.Add(m_fft_in[i].real.ToString());
                        item.SubItems.Add(m_fft_in[i].imag.ToString());

                        chartInput.Series["實數"].Points.AddY(m_fft_in[i].real);
                        chartInput.Series["虛數"].Points.AddY(m_fft_in[i].imag);
                    }
                    break;

                case 2:
                    for (int i = 0; i < (s_arr.Length - 1); i++)
                    {

                        m_fft_in[i].real = 0.0f;
                        m_fft_in[i].imag = double.Parse(s_arr[i + 1]); ;

                        ListViewItem item = lvInput.Items.Add((i + 1).ToString());
                        item.SubItems.Add(m_fft_in[i].real.ToString());
                        item.SubItems.Add(m_fft_in[i].imag.ToString());

                        chartInput.Series["實數"].Points.AddY(m_fft_in[i].real);
                        chartInput.Series["虛數"].Points.AddY(m_fft_in[i].imag);
                    }
                    break;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            double hz = 0.0f;
            lvOutput.Items.Clear();
            chartOutputComplex.Series["實數"].Points.Clear();
            chartOutputComplex.Series["虛數"].Points.Clear();
            chartOutputMagnitude.Series["數值"].Points.Clear();

            if ((m_fft_in == null) || (m_fft_out == null) || (m_fft_out2 == null) || (m_real == null))
            {
                return;
            }
            int t = System.Environment.TickCount;
            FFT_translateToSpectrum(out hz, out m_amplitude[0], out m_fft_out[0], m_fft_in, m_fft_in.Length, m_hz);
            //FFT_translate(out m_fft_out[0], m_fft_in, m_fft_in.Length);
            FFT_convertToMagnitude(out m_real[0], m_fft_out, m_fft_out.Length);
            t = System.Environment.TickCount - t;
            lstripTimeSpend.Text = "花費時間(ms): " + t.ToString();

            for (int i = 0; i < m_fft_out.Length; i++)
            {
                ListViewItem item = lvOutput.Items.Add((i + 1).ToString());
                item.SubItems.Add(m_fft_out[i].real.ToString());
                item.SubItems.Add(m_fft_out[i].imag.ToString());
                item.SubItems.Add(m_real[i].ToString());

                chartOutputComplex.Series["實數"].Points.AddY(m_fft_out[i].real);
                chartOutputComplex.Series["虛數"].Points.AddY(m_fft_out[i].imag);
                chartOutputMagnitude.Series["數值"].Points.AddY(m_real[i]);
            }


            chartOutputFreq.ChartAreas[0].AxisX.Interval = 10;
            for (int i = 0; i < m_amplitude.Length; i++)
            {

                chartOutputFreq.Series["數值"].Points.AddXY(hz * i, m_amplitude[i]);
            }

        }
    }
}
