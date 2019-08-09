using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Performance_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        PointPairList[] point;
        delegate void dUse();
        private void Form1_Load(object sender, EventArgs e)
        {
            ZedGraphControl tmpZgc;
            GraphPane tmpGP;

            tmpZgc = zgcTest1;
            tmpGP = tmpZgc.GraphPane;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            tmpZgc = zgcTest2;
            tmpGP = tmpZgc.GraphPane;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;

            tmpZgc = zgcTest3;
            tmpGP = tmpZgc.GraphPane;
            tmpZgc.IsEnableVZoom = false;
            tmpZgc.IsEnableHZoom = false;
            tmpZgc.IsEnableVPan = false;
            tmpZgc.IsEnableHPan = false;
            tmpZgc.IsEnableWheelZoom = false;


            double max = 65536;
            point = new PointPairList[3];
            for (int i = 0; i < point.Length; i++)
            {
                point[i] = new PointPairList();
            }
            for (double i = 0; i < max; i += 0.1)
            {
                double angle = i * 2 * Math.PI;
                point[0].Add(i, Math.Sin(angle));
                point[1].Add(i, Math.Cos(angle));
                point[2].Add(i, Math.Tan(angle));
            }
            PaintCurve(point[0], zgcTest1);
            PaintCurve(point[1], zgcTest2);
            PaintCurve(point[2], zgcTest3);

            zgcTest1.GraphPane.XAxis.Scale.Min = 0;
            zgcTest1.GraphPane.XAxis.Scale.Max = 20;
            zgcTest2.GraphPane.XAxis.Scale.Min = 0;
            zgcTest2.GraphPane.XAxis.Scale.Max = 20;
            zgcTest3.GraphPane.XAxis.Scale.Min = 0;
            zgcTest3.GraphPane.XAxis.Scale.Max = 20;
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

        double timeScale;
        private void tmrPlay_Tick(object sender, EventArgs e)
        {
            zgcTest1.GraphPane.XAxis.Scale.Max += timeScale;
            zgcTest1.GraphPane.XAxis.Scale.Min += timeScale;
            zgcTest1.Refresh();
            //zgcTest2.GraphPane.XAxis.Scale.Max += timeScale;
            //zgcTest2.GraphPane.XAxis.Scale.Min += timeScale;
            //zgcTest2.Refresh();
            //zgcTest3.GraphPane.XAxis.Scale.Max += timeScale;
            //zgcTest3.GraphPane.XAxis.Scale.Min += timeScale;
            //zgcTest3.Refresh();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            tmrPlay.Interval = 20;
            timeScale = tmrPlay.Interval / 1000.0;
            DateTime dt = DateTime.Now;
            dUse duse;
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    if ((DateTime.Now - dt).TotalMilliseconds >= tmrPlay.Interval)
                    {
                        zgcTest2.GraphPane.XAxis.Scale.Max += timeScale;
                        zgcTest2.GraphPane.XAxis.Scale.Min += timeScale;
                        try
                        {
                            Invoke(duse = () =>
                            {
                                zgcTest2.Refresh();
                            });
                        }
                        catch { return; }
                        dt = dt.AddMilliseconds(tmrPlay.Interval);
                    }
                };
            });
            t.Start();
            tmrPlay.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmSetAlarm frmTest = new frmSetAlarm();
            frmTest.ShowDialog();
            if (frmTest.DialogResult == DialogResult.OK)
            {
                frmSetAlarm.xyzLimit t = frmTest.rtnValue;
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
            Font = new Font("Consolas", 12.0f);

            Button btnTest = new Button()
            {
                Location = new Point(50, 50),
                Text = "try it"
            };

            btnTest.Click += (o, e) =>
            {
                rtnValue = new xyzLimit(1, 2, 3, 4, 5, 6);
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.Add(btnTest);

        }
    }
}
