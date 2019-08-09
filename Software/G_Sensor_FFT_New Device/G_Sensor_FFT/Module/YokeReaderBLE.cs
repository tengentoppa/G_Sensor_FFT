using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//[20180221]Create by Simon
namespace MP_Moudule
{
    class YokeReaderInBLE
    {
        //STX(1) + 
        //Count(1){ TotalSN(4bit)&0xF0 + CurSN(4bit)&0x0F } + 
        //Len(1){ CMD.Length + Data.Length } + 
        //CMD(1) + 
        //Data(N) + 
        //ETX(1)
        public enum DataStatue { Success, DataNull, NoData, LengthNotEnough, SETXFail, DataFail, CrcFail, SRNotMatch };
        const byte SSTX = 0x23, SETX = 0x2A;
        const byte RSTX = 0x24, RETX = 0x2A;

        //靜態區域
        public static List<byte> PackageData(byte cmd, List<byte> data)
        {
            List<byte> result = new List<byte>();
            result.Add(SSTX);
            result.Add(0x00);
            if (data != null)
            {
                result.Add((byte)(data.Count + 1));
                result.Add(cmd);
                result.AddRange(data);
            }
            else
            {
                result.Add((byte)(1));
                result.Add(cmd);
            }
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

            int pStx, len, pEtx, totalLen;
            while ((pStx = data.IndexOf(RSTX)) != -1)
            {
                if (data.Count <= pStx + 1) { WriteLog.Console("Parse1", DataStatue.LengthNotEnough.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " ")); return null; }
                len = data[pStx + 1];
                pEtx = pStx + len + 2;
                totalLen = len + 3;
                if (data.Count <= pEtx) { WriteLog.Console("Parse2", DataStatue.LengthNotEnough.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " ")); return null; }
                if (data[pEtx] != RETX) { WriteLog.Console("Parse3", DataStatue.SETXFail.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " ")); data.RemoveRange(0, pStx + 1); continue; }

                //scWriteLog.Console("Parse4", data.GetRange(pStx, totalLen));
                output.Add(data.GetRange(pStx, totalLen));
                if (pStx != 0) WriteLog.Console("Parse5", DataStatue.DataFail.ToString() + " " + BitConverter.ToString(data.ToArray()).Replace("-", " "));

                data.RemoveRange(pStx, totalLen);
            }
            return output;
        }


        //動態區域
        byte m_total_SN, m_cur_SN;
        int m_data_len;
        List<byte> m_data = null;
        bool m_data_statue = false;

        public int dataLen { get { return m_data_len; } }
        public List<byte> data { get { return m_data; } }
        public bool dataStatue { get { return m_data_statue; } }

        public YokeReaderInBLE(List<byte> source)
        {
            FormatData(source);
        }

        public void FormatData(List<byte> source)
        {
            if (source.Count < 4) { return; }
            if (source[0] != RSTX) { return; }

            m_data_len = source[2];
            if (m_data_len != source.Count - 4) { return; }

            int total_len = source.Count;
            if (source[total_len - 1] != RETX) { return; }

            m_total_SN = (byte)(source[1] & 0x0F);
            m_cur_SN = (byte)(source[1] & 0xF0);
            if (m_data_len > 0)
            {
                m_data = source.GetRange(3, m_data_len);
            }
            m_data_statue = true;
        }
    }
}
