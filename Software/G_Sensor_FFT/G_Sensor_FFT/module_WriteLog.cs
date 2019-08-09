using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

//[20180221]Create by Simon
namespace MP_Moudule
{
    /// <summary>
    /// Debug輸出
    /// </summary>
    public static class WriteLog
    {
        /// <summary>
        /// 將字串包成 「HH:mm:ss.fff [header] input」的形式
        /// </summary>
        /// <param name="header">標頭</param>
        /// <param name="input">內容</param>
        /// <returns></returns>
        public static string GetFormatString(string header, string input)
        {
            return DateTime.Now.ToString("HH:mm:ss.fff") + " [" + header + "] " + input;
        }

        /// <summary>
        /// 將byte的內容寫入控制台
        /// </summary>
        /// <param name="header">標頭</param>
        /// <param name="input">輸入資料</param>
        public static void Console(string header, byte input)
        {
            Console(header, input.ToString("X2"));
        }
        /// <summary>
        /// 將byte Array所有內容寫入控制台
        /// </summary>
        /// <param name="header">標頭</param>
        /// <param name="input">輸入資料</param>
        public static void Console(string header, byte[] input)
        {
            Console(header, BitConverter.ToString(input).Replace("-", " "));
        }
        /// <summary>
        /// 將List byte所有內容寫入控制台
        /// </summary>
        /// <param name="header">標頭</param>
        /// <param name="input">輸入資料</param>
        public static void Console(string header, List<byte> input)
        {
            Console(header, BitConverter.ToString(input.ToArray()).Replace("-", " "));
        }
        /// <summary>
        /// 將字串寫入控制台
        /// </summary>
        /// <param name="header">標頭</param>
        /// <param name="input">輸入資料</param>
        public static void Console(string header, string input)
        {
            string temp = GetFormatString(header, input);
            System.Diagnostics.Debug.WriteLine(temp);
            
            //WriteToTxt("Debug Log", temp);
        }

        /// <summary>
        /// 將資料寫入txt檔
        /// </summary>
        /// <param name="name">檔名</param>
        /// <param name="input">內容</param>
        public static void WriteToTxt(string name, string input)
        {
            string direct = Directory.GetCurrentDirectory() + @"\Log\";
            string fileName = name + ".txt";
            string path = direct + fileName;

            rty:
            try
            {
                if (!Directory.Exists(direct)) { Directory.CreateDirectory(direct); }

                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

                sw.WriteLine(input);
                sw.Close();
            }
            catch (IOException/* ioE*/) { /*MessageBox.Show("Please close file " + fileName + " and try again.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);*/ goto rty; }
            catch (Exception/* e*/) { /*MessageBox.Show(e.Message);*/ goto rty; }
        }

        /// <summary>
        /// 將資料以特定副檔名寫入
        /// </summary>
        /// <param name="name">檔名</param>
        /// <param name="expand">副檔名</param>
        /// <param name="input">內容</param>
        public static void WriteToFile(string name, string expand, string input)
        {
            string direct = Directory.GetCurrentDirectory() + @"\Log\";
            string fileName = name + "." + expand;
            string path = direct + fileName;

            rty:
            try
            {
                if (!Directory.Exists(direct)) { Directory.CreateDirectory(direct); }

                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

                sw.WriteLine(input);
                sw.Close();
            }
            catch (IOException /*ioE*/) { MessageBox.Show("Please close file " + fileName + " and try again.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); goto rty; }
            catch (Exception e) { MessageBox.Show(e.Message); goto rty; }
        }

        /// <summary>
        /// 將資料寫入CSV檔，以分號\n作為換行(分行)符號，豆號,作為分列(分欄)符號
        /// </summary>
        /// <param name="name">檔案名稱</param>
        /// <param name="header">標頭</param>
        /// <param name="input">內容</param>
        public static void WriteToCsv(string name, string header, string input)
        {
            string direct = Directory.GetCurrentDirectory() + @"\Log\";
            string fileName = name + ".csv";
            string path = direct + fileName;

            if (!Directory.Exists(direct)) { Directory.CreateDirectory(direct); }
            if (!File.Exists(path))
            {
                WriteToFile(name, "csv", header);
            }
            WriteToFile(name, "csv", input);
        }
        /// <summary>
        /// 將資料寫入CSV檔，以\n作為換行(分行)符號，豆號,作為分列(分欄)符號
        /// </summary>
        /// <param name="name">檔案名稱</param>
        /// <param name="input">內容</param>
        public static void WriteToCsv(string name, string input)
        {
            WriteToFile(name, "csv", input);
        }
    }
}
