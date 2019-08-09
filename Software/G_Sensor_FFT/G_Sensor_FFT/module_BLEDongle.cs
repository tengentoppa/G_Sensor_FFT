using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//[20180221]Create by Simon
namespace MP_Moudule
{
    class BLEDongle
    {
        //STX(1) + CMD(1) + Data(N) + CRC(1){ XOR From STX to Data } + ETX(1)
        public enum DataStatue { Success, DataNull, NoData, LengthNotEnough, SETXFail, DataFail, CrcFail, SRNotMatch };
        public enum CMD { StartScan = 0xA1, Connect = 0xA3, Disconnect = 0xA4,SendData = 0xA6, StopScan = 0xA7, SetUuid = 0xA8 };
        const byte SSTX = 0xC1, SETX = 0xC2;
        const byte RSTX = 0xD1, RETX = 0xD2;

        //靜態區域
        /// <summary>
        /// 打包資料
        /// </summary>
        /// <param name="cmd">CMD</param>
        /// <param name="data">資料，無資料則填入null</param>
        /// <returns>打包後的資料</returns>
        public static List<byte> PackageData(byte cmd, List<byte> data)
        {
            byte len = 1;
            if (data != null) len += (byte)data.Count;
            List<byte> result = new List<byte>();
            result.Add(SSTX);
            result.Add(len);
            result.Add(cmd);
            if (data != null) result.AddRange(data);
            result.Add(MakeCrc(result));
            result.Add(SETX);
            return result;
        }
        public static List<byte> PackageData(byte cmd, byte data)
        {
            byte len = 1;
            len++;
            List<byte> result = new List<byte>();
            result.Add(SSTX);
            result.Add(len);
            result.Add(cmd);
            result.Add(data);
            result.Add(MakeCrc(result));
            result.Add(SETX);
            return result;
        }
        /// <summary>
        /// 切割資料
        /// </summary>
        /// <param name="data">待切割資料</param>
        /// <param name="output">切割後的資料</param>
        /// <returns>資料狀態</returns>
        public static List<List<byte>> ParseData(List<byte> data)
        {
            if (data == null) return null;
            List<List<byte>> output = new List<List<byte>>();

            int pStx = 0, len, pEtx, totalLen;
            while ((pStx = data.IndexOf(RSTX, pStx)) != -1)
            {
                if (data.Count <= pStx + 1) { WriteLog.Console("Parse1", DataStatue.LengthNotEnough.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " ")); return null; }
                len = data[pStx + 1];
                pEtx = pStx + len + 3;
                totalLen = len + 4;
                if (data.Count <= pEtx) { WriteLog.Console("Parse2", DataStatue.LengthNotEnough.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " ")); return null; }
                if (data[pEtx] != RETX) { WriteLog.Console("Parse3", DataStatue.SETXFail.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " ")); /*pStx += 1*/data.RemoveAt(0); continue; }
                if (data[pEtx - 1] != MakeCrc(data, pStx, totalLen - 2)) { WriteLog.Console("Parse4", DataStatue.CrcFail.ToString() + " " + ByteConverter.ToHexString(data)); pStx += 1; continue; }
                output.Add(data.GetRange(pStx, totalLen));
                if (pStx != 0)
                {
                    data.RemoveRange(0, pStx);
                    WriteLog.Console("Parse5", DataStatue.DataFail.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " "));
                    pStx = 0;
                }

                data.RemoveRange(pStx, totalLen);
            }
            return output;
        }

        /// <summary>
        /// 以指定範圍的List<byte>製作CRC驗證碼</byte>
        /// </summary>
        /// <param name="input">輸入List</param>
        /// <param name="pStart">起使位置</param>
        /// <param name="length">納入運算的長度</param>
        /// <returns>CRC驗證碼</returns>
        public static byte MakeCrc(List<byte> input, int pStart, int length)
        { return MakeCrc(input.GetRange(pStart, length)); }
        /// <summary>
        /// 將輸入清單的每個元素作XOR運算
        /// </summary>
        /// <param name="input">輸入清單</param>
        /// <returns>CRC驗證碼</returns>
        public static byte MakeCrc(List<byte> input)
        {
            byte result = 0x00;
            foreach (byte data in input) result ^= data;
            return result;
        }


        //動態區域
        byte m_cmd;
        List<byte> m_data = null;
        int m_data_len = 0;
        bool m_data_statue = false;

        public byte cmd { get { return m_cmd; } }
        public List<byte> data { get { return m_data; } }
        public int len { get { return m_data_len; } }
        public bool dataStatue { get { return m_data_statue; } }

        public BLEDongle(List<byte> source)
        {
            FormatData(source);
        }

        public void FormatData(List<byte> source)
        {
            if (source.Count < 5) { return; }
            if (source[0] != RSTX) { return; }
            int total_len = source.Count;
            if (source[total_len - 1] != RETX) { return; }

            m_data_len = source[1];
            m_cmd = source[2];
            if (source.Count > 5)
            {
                m_data = source.GetRange(3, m_data_len - 1);
            }
            m_data_statue = true;
        }
    }

    class BleDevice
    {
        public static Dictionary<string, BleDevice> DeviceList = new Dictionary<string, BleDevice>();
        //public YokeReaderInBLE yokeReader;
        List<byte> _mac = new List<byte>();
        String _name;
        int _rssi;
        float _volte;
        public bool inConnectList = false;
        public bool connecting = false;
        public bool receiveA5 = false;

        public static bool SetDevice(List<byte> macAddress)
        {
            string address = ByteConverter.ToHexString(macAddress);
            if (!DeviceList.ContainsKey(address))
            {
                DeviceList.Add(address, new BleDevice());
                DeviceList[address]._mac = macAddress;
                return true;
            }
            return false;
        }
        public static bool SetDevice(string macAddress)
        {
            if (!DeviceList.ContainsKey(macAddress))
            {
                DeviceList.Add(macAddress, new BleDevice());
                DeviceList[macAddress]._mac = ByteConverter.HexStringToByteArray(macAddress).ToList();
                return true;
            }
            return false;
        }
        public List<byte> mac { set { _mac = value; } get { return _mac; } }
        public string name { set { _name = value; } get { return _name; } }
        public int rssi { set { _rssi = value; } get { return _rssi; } }
        public float volte { set { _volte = value; } get { return _volte; } }

        public BleDevice() { }
    }
}
