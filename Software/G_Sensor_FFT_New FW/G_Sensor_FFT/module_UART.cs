using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

//[20180221]Create by Simon
namespace MP_Moudule
{
    public class UART
    {
        public bool KeepReceiveFlag = false;
        public enum Statue { Success, Null, Denied, CantOpen, Exception };
        public enum Communicate { Success, UartNull, UartNotOpen, NoData, Exception, Timeout }
        public delegate void KeepProccess(List<byte> data);
        public event KeepProccess keepProccessFunc;
        public List<KeepProccess> eventList = new List<KeepProccess>();
        SerialPort m_serialPort;

        public SerialPort getPort { get { return m_serialPort; } }

        /// <summary>
        /// 取得目前電腦上所有能被辨識的Com Port
        /// </summary>
        /// <returns>Com Port陣列</returns>
        public static string[] SearchPort()
        {
            return SerialPort.GetPortNames();
        }


        public UART(string comPort, int baudRate, Parity parity, int dataBits, StopBits stopBits, int readBufferSize, int writeBufferSize)
        {
            m_serialPort = new SerialPort(comPort, baudRate, parity, dataBits, stopBits)
            {
                ReadBufferSize = readBufferSize,
                WriteBufferSize = writeBufferSize
            };
        }

        public UART(string comPort, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            m_serialPort = new SerialPort(comPort, baudRate, parity, dataBits, stopBits);
        }

        public UART(string comPort, int baudRate)
        {
            m_serialPort = new SerialPort(comPort, baudRate);
        }

        public Communicate GetUartStatue()
        {
            if (m_serialPort == null) return Communicate.UartNull;
            if (!m_serialPort.IsOpen) return Communicate.UartNotOpen;
            return Communicate.Success;
        }

        /// <summary>
        /// 開啟指定的連接埠
        /// </summary>
        /// <param name="com_port">指定的連接埠</param>
        /// <param name="baud_rate">鮑率</param>
        /// <returns>UART狀態</returns>
        public Statue Open()
        {
            try
            {
                if (null == m_serialPort) { return Statue.Null; }

                if (m_serialPort.IsOpen) m_serialPort.Close();

                if (m_serialPort.IsOpen) return Statue.Denied;

                m_serialPort.Open();

                if (!m_serialPort.IsOpen) return Statue.CantOpen;

                ClearBuffer();
                return Statue.Success;
            }
            catch (Exception ex)
            {
                WriteLog.Console("Open Com Exp", ex.Message);
            }
            finally
            {
            }

            return Statue.Exception;
        }

        public void ClearBuffer()
        {
            if (m_serialPort == null || !m_serialPort.IsOpen) return;
            try
            {
                m_serialPort.DiscardInBuffer();
                m_serialPort.DiscardOutBuffer();
            }
            catch { }
        }

        /// <summary>
        /// 關閉目前開啟的連接埠
        /// </summary>
        /// <returns>UART狀態</returns>
        public Statue Close()
        {
            try
            {
                if (m_serialPort.IsOpen) m_serialPort.Close();

                if (m_serialPort.IsOpen) return Statue.Denied;

                return Statue.Success;
            }
            catch (Exception ex)
            {
                WriteLog.Console("Close Com Exp", ex.Message);
            }
            return Statue.Exception;
        }

        /// <summary>
        /// 將byte清單送出
        /// </summary>
        /// <param name="data">要送出的資料</param>
        /// <returns>發送狀態</returns>
        public Communicate SendData(List<byte> data)
        {
            if (data.Count == 0) return Communicate.NoData;
            if (m_serialPort == null) return Communicate.UartNull;
            if (!m_serialPort.IsOpen) return Communicate.UartNotOpen;

            ClearBuffer();

            WriteLog.Console("    Send    ", BitConverter.ToString(data.ToArray()).Replace("-", " "));
            try
            {
                m_serialPort.Write(data.ToArray(), 0, data.Count);
            } catch { }

            return Communicate.Success;
        }

#if DEBUG
        DateTime dt = DateTime.Now;
        List<byte> t = new List<byte>();
#endif
        /// <summary>
        /// 從UART接收資料，單次執行
        /// </summary>
        /// <returns>UART接收到的資料</returns>
        public byte[] ReceiveData()
        {
            try
            {
                if (!m_serialPort.IsOpen) return null;
                int dataLen = m_serialPort.BytesToRead;
                if (dataLen == 0) return new byte[0];

                byte[] buff = new byte[dataLen];

                m_serialPort.Read(buff, 0, dataLen);
#if DEBUG
                t.AddRange(buff);
                if ((DateTime.Now - dt).TotalMilliseconds >= 1000)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}-{t.Count}");
                    t.Clear();
                    dt = dt.AddMilliseconds(1000);
                }
#endif

                //WriteLog.Console("    Buff    ", buff.ToList());
                return buff;
            }
            catch { }
            return null;
        }

        List<byte> bufferData = new List<byte>();
        /// <summary>
        /// 持續接收資料
        /// </summary>
        public void KeepReceiveData(object sender, SerialDataReceivedEventArgs e)
        {

            if (!KeepReceiveFlag)
            {
                StopReceiveData();
                return;
            }
            bufferData.AddRange(ReceiveData());
            keepProccessFunc?.Invoke(bufferData);
        }

        /// <summary>
        /// 開啟ComPort持續接收事件，請插入委派函式處理收到的資料
        /// </summary>
        /// <param name="func"></param>
        /// <returns>如目前已開啟ComPort持續接收事件，將不執行命令並回傳False</returns>
        public bool StartReceiveData(KeepProccess func)
        {
            if (KeepReceiveFlag) return false;

            KeepReceiveFlag = true;
            if (!eventList.Contains(func))
            {
                keepProccessFunc += func;
                eventList.Add(func);
            }
            m_serialPort.DataReceived += KeepReceiveData;
            return true;
        }
        public void StopReceiveData()
        {
            foreach (KeepProccess func in eventList) { keepProccessFunc -= func; }
            eventList.Clear();
            KeepReceiveFlag = false;
            m_serialPort.DataReceived -= KeepReceiveData;
        }
    }
}
