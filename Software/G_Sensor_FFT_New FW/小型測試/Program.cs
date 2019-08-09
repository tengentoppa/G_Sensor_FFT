using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 小型測試
{
    class Program
    {
        static SerialPort com;
        delegate void d();
        static void Main(string[] args)
        {
            List<string> comList = new List<string>();
            Dictionary<string, d> listTestFunction = new Dictionary<string, d>() {
                { "Read Com Use Thread Loop", readComUseThreadLoop},
                { "Read Com Use Event DataReceive", readComUseEventDataReceive } };

            comList.AddRange(SerialPort.GetPortNames());


            if (comList.Count == 0) { Console.WriteLine("Can't find any com port\nPress any key to leave."); Console.ReadKey(); return; }

            Console.WriteLine("\nCom List:");
            for (int i = 0; i < comList.Count; i++)
            {
                Console.WriteLine($"{i + 1}.{comList[i]}");
            }
            Console.WriteLine("\nPlease select the com that you want to open.");
            if (!int.TryParse(Console.ReadLine(), out int pCom)) { return; }
            if (pCom <= 0 || pCom - 1 >= comList.Count) { return; }


            int p = 0;
            Console.WriteLine("\nTest Function List:");
            foreach (KeyValuePair<string , d> f in listTestFunction)
            {
                Console.WriteLine($"{++p}.{f.Key}");
            }
            Console.WriteLine("\nPlease select the Test Function that you want to use.");
            if (!int.TryParse(Console.ReadLine(), out int pTestFunc)) { return; }
            if (pTestFunc <= 0 || pTestFunc - 1 >= listTestFunction.Count) { return; }

            com = new SerialPort(comList[pCom - 1], 921600, Parity.None, 8) { ReadBufferSize = 4096 };
            try
            {
                com.Open();
            }
            catch (Exception e) { Console.WriteLine($"Open com fail:{e.ToString()}"); }

            listTestFunction.ToArray()[pTestFunc-1].Value();

            Console.WriteLine("Press any key to leave.");
            Console.ReadKey();
            return;
        }

        static DateTime dtTest;
        static void readComUseThreadLoop()
        {
            dtTest = DateTime.Now;
            Thread t = new Thread(() =>
            {
                List<byte> data = new List<byte>();
                while (true)
                {
                    byte[] b = new byte[com.BytesToRead];
                    com.Read(b, 0, b.Length);
                    data.AddRange(b);

                    if ((DateTime.Now - dtTest).TotalMilliseconds >= 1000)
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}-{data.Count}");
                        dtTest = dtTest.AddMilliseconds(1000.0);

                        data.Clear();
                    }
                }
            })
            { IsBackground = true };
            t.Start();
        }

        static void readComUseEventDataReceive()
        {
            dtTest = DateTime.Now;
            List<byte> data = new List<byte>();

            com.DataReceived += (o, e) =>
            {
                byte[] b = new byte[com.BytesToRead];
                com.Read(b, 0, b.Length);
                data.AddRange(b);

                if ((DateTime.Now - dtTest).TotalMilliseconds >= 1000)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}-{data.Count}");
                    dtTest = dtTest.AddMilliseconds(1000.0);

                    data.Clear();
                }
            };
        }

        static void convertTest()
        {
            string printout = "";
            int result;
            for (byte b1 = 0x00; b1 <= 0xFF; b1++)
            {
                for (byte b2 = 0x00; b2 <= 0xFF; b2++)
                {
                    object a = ((sbyte)b1) * 256;
                    Type b = a.GetType();
                    result = ((sbyte)b1) * 256 + b2;
                    printout += $"{b1.ToString("X2")}{b2.ToString("X2")} = {result.ToString("D4")}{"\t"}";
                    if (b2 % 16 == 15)
                    {
                        printout += "\n";
                    }
                    if (printout.Length > 2000)
                    {
                        Console.Write(printout);
                        printout = "";
                    }
                    if ((b1 & b2) == 0xFF)
                    {
                        Console.Write(printout);
                        Console.ReadKey();
                        return;
                    }
                    if (b2 == 0xFF) { break; }
                }
            }
        }
    }
}
