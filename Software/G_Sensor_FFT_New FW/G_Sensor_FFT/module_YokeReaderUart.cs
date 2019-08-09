using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//[20180221]Create by Simon
namespace MP_Moudule
{
    class YokeReaderInUart
    {
        //STX(1) + CMD(1) + Data(N) + CRC(1){ XOR From STX to Data } + ETX(1)
        public enum DataStatue { Success, DataNull, NoData, LengthNotEnough, SETXFail, DataFail, CrcFail, SRNotMatch };
        const byte SSTX = 0x23, SETX = 0x2A;
        const byte KRSTX = 0x23, RSTX = 0x24, RETX = 0x2A;

        //靜態區域
        public static List<byte> PackageData(byte cmd, List<byte> data)
        {
            List<byte> result = new List<byte>();
            result.Add(SSTX);
            result.Add(cmd);
            if (data != null) result.AddRange(data);
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

            int pStx, pEtx, totalLen;
            while ((pStx = data.IndexOf(RSTX)) != -1)
            {
                totalLen = 0;
                if (data.Count <= pStx + 3) { WriteLog.Console("Parse1", DataStatue.LengthNotEnough.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " ")); return null; }
                pEtx = pStx + 2;
                while ((pEtx = data.IndexOf(RETX, pEtx + 1)) != -1)
                {
                    totalLen = pEtx - pStx + 1;
                    if (data[pEtx - 1] != MakeCrcInRange(data, pStx, totalLen - 2))
                    {
                        WriteLog.Console("CRC Fail", data.GetRange(pStx, totalLen));
                        data.RemoveRange(0, pStx + 1);
                    }
                    else { break; }
                }

                if (totalLen == 0 || totalLen > 30 || pEtx == -1) { continue; }
                output.Add(data.GetRange(pStx, totalLen));

                data.RemoveRange(pStx, totalLen);
            }

            while ((pStx = data.IndexOf(KRSTX)) != -1)
            {
                totalLen = 0;
                if (data.Count <= pStx + 3) { WriteLog.Console("Parse1", DataStatue.LengthNotEnough.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " ")); return null; }
                pEtx = pStx + 2;
                while ((pEtx = data.IndexOf(RETX, pEtx + 1)) != -1)
                {
                    totalLen = pEtx - pStx + 1;
                    if (data[pEtx - 1] != MakeCrcInRange(data, pStx, totalLen - 2))
                    {
                        WriteLog.Console("CRC Fail", data.GetRange(pStx, totalLen));
                        data.RemoveRange(0, pStx + 1);
                    }
                    else { break; }
                }

                if (totalLen == 0 || totalLen > 30 || pEtx == -1) { continue; }
                output.Add(data.GetRange(pStx, totalLen));

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
        public static byte MakeCrcInRange(List<byte> input, int pStart, int length)
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
        bool m_data_statue = false;

        public byte cmd { get { return m_cmd; } }
        public List<byte> data { get { return m_data; } }
        public bool dataStatue { get { return m_data_statue; } }

        public YokeReaderInUart(List<byte> source)
        {
            FormatData(source);
        }

        public void FormatData(List<byte> source)
        {
            if (source.Count < 4) { return; }
            if (!(source[0] == RSTX || source[0] == KRSTX)) { return; }
            int total_len = source.Count;
            if (source[total_len - 1] != RETX) { return; }

            m_cmd = source[1];
            if (source.Count > 4)
            {
                m_data = source.GetRange(2, total_len - 4);
            }
            m_data_statue = true;
        }
    }

}
