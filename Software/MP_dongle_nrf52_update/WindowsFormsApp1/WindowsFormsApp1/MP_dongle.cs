using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class MP_dongle
    {
        public enum CMD
        {
           TX_STX                = 0xC1,
           TX_ETX                = 0xC2,
           RX_STX                = 0xD1,
           RX_ETX                = 0xD2,
           Scan_Start            = 0xA1,
           AVD_Report            = 0xA2,
           Connect_ack           = 0xA3,
           Disconnect            = 0xA4,
           BLE_RX                = 0xA5,
           BLE_TX                = 0xA6,
           Scan_Stop             = 0xA7,
           Set_UUID              = 0xA8,
           Get_UUID              = 0xA9,
           Connect               = 0xAA,
           Set_Auto_connect_list = 0xB1,
           Get_Auto_connect_list = 0xB2,
           Get_Connect_list      = 0xC1,
           Start                 = 0xD0,
           Reset                 = 0xD1,
           Version               = 0xD2
        };

        public enum GET_LIST
        {
            First_Get = 0x01,
            Continue  = 0x02
        };

        public enum SET_LIST
        {
            First_Set = 0x01,
            Continue = 0x02
        };

        public enum SCAN_TYPE
        {
            Scan_Mode = 0x00,
            Auto_Mode = 0x01
        };

        public List<device_info> scan_device_info           = new List<device_info>();
        public List<device_info> connected_device_info      = new List<device_info>();
        public List<device_info> Auto_connect_list          = new List<device_info>();
        public List<device_info> Set_Auto_connect_list      = new List<device_info>();
        public List<device_info> Get_Auto_connect_list      = new List<device_info>();
        public List<device_info> Set_Auto_connect_list_info = new List<device_info>();

        public List<string> ble_device_mac     = new List<string>();
        public List<string> ble_device         = new List<string>();
        public List<string> ble_device_connect = new List<string>();
        public List<payload> ble_rx_payload    = new List<payload>();

        public string str_UUID = "";
        public byte[] select_scan_device               = new byte[6];
        public byte[] select_connect_device            = new byte[6];
        public bool Flag_is_change_device_show         = false;
        public bool Flag_is_change_device_connect_show = false;
        public bool Flag_is_auto_connect_list_show     = false;
        public bool Flag_is_connect_success            = false;
        public bool Flag_is_UUID_show = false;
        public bool Flag_is_no_show_log = false;

        public bool Flag_get_auto_connect_list = false;
        public bool Flag_get_connected_list    = false;

        public bool Flag_set_auto_list_scan    = false;
        public bool Flag_set_auto_list         = false;
        int index_set_auto_list = 0;

        public string str_version = "";
        public bool Flag_get_version           = false;
        public bool Flag_show_log = true;
        public byte[] scan_start(byte type)
        {
            byte[] data = new byte[6];
            data[0] = (byte)CMD.TX_STX;
            data[1] = 0x01;
            data[2] = (byte)CMD.Scan_Start;
            data[3] = type;
            data[4] = creat_crc(data, 4);
            data[5] = (byte)CMD.TX_ETX;

            return data;
        }

        public byte[] scan_stop()
        {
            byte[] data = new byte[5];
            data[0] = (byte)CMD.TX_STX;
            data[1] = 0x01;
            data[2] = (byte)CMD.Scan_Stop;
            data[3] = creat_crc(data, 3);
            data[4] = (byte)CMD.TX_ETX;

            return data;
        }

        public byte[] connect()
        {
            byte[] data = new byte[11];
            data[0]  = (byte)CMD.TX_STX;
            data[1]  = 0x07;
            data[2]  = (byte)CMD.Connect_ack;
            for (int i = 0; i < 6; i++)
                data[i + 3] = select_scan_device[i];
            data[9]  = creat_crc(data, 9);
            data[10] = (byte)CMD.TX_ETX;

            return data;
        }

        public byte[] disconnect()
        {
            byte[] data = new byte[11];
            data[0] = (byte)CMD.TX_STX;
            data[1] = 0x07;
            data[2] = (byte)CMD.Disconnect;
            for (int i = 0; i < 6; i++)
                data[i + 3] = select_connect_device[i];
            data[9] = creat_crc(data, 9);
            data[10] = (byte)CMD.TX_ETX;

            return data;
        }

        public byte[] ble_tx(byte [] payload)
        {
            byte[] data = new byte[11+payload.Length];
            data[0] = (byte)CMD.TX_STX;
            data[1] = (byte)(7 + payload.Length);
            data[2] = (byte)CMD.BLE_TX;
            for (int i = 0; i < 6; i++)
                data[i + 3] = select_connect_device[i];
            for (int i = 0; i < payload.Length; i++)
                data[i + 9] = payload[i];
            data[9 + payload.Length] = creat_crc(data, 9 + payload.Length);
            data[10 + payload.Length] = (byte)CMD.TX_ETX;

            return data;
        }

        public byte[] set_UUID(byte[] data_uuid)
        {
            byte[] data = new byte[5 + data_uuid.Length];
            data[0] = (byte)CMD.TX_STX;
            data[1] = 0x11;
            data[2] = (byte)CMD.Set_UUID;
            for (int i = 0; i < 16; i++)
                data[i + 3] = data_uuid[i];
            data[19] = creat_crc(data, 19);
            data[20] = (byte)CMD.TX_ETX;

            return data;
        }

        public byte[] get_UUID()
        {
            byte[] data = new byte[5];
            data[0] = (byte)CMD.TX_STX;
            data[1] = 0x01;
            data[2] = (byte)CMD.Get_UUID;
            data[3] = creat_crc(data, 3);
            data[4] = (byte)CMD.TX_ETX;

            return data;
        }

        public byte[] get_auto_connect_list(byte type)
        {
            byte[] data = new byte[6];
            data[0] = (byte)CMD.TX_STX;
            data[1] = 0x02;
            data[2] = (byte)CMD.Get_Auto_connect_list;
            data[3] = type;
            data[4] = creat_crc(data, 4);
            data[5] = (byte)CMD.TX_ETX;
            return data;
        }

        public byte[] get_connected_list(byte type)
        {
            byte[] data = new byte[6];
            data[0] = (byte)CMD.TX_STX;
            data[1] = 0x02;
            data[2] = (byte)CMD.Get_Connect_list;
            data[3] = type;
            data[4] = creat_crc(data, 4);
            data[5] = (byte)CMD.TX_ETX;
            return data;
        }

        public byte[] set_auto_connected_list(byte type)
        {
            if ((byte)SET_LIST.First_Set == type)
                index_set_auto_list = 1;
            else
                index_set_auto_list++;
            
            byte[] data = new byte[13];
            data[0]  = (byte)CMD.TX_STX;
            data[1]  = 0x09;
            data[2]  = (byte)CMD.Set_Auto_connect_list;
            data[3]  = (byte)Set_Auto_connect_list.Count;
            data[4]  = (byte)index_set_auto_list;
            if (index_set_auto_list <= Set_Auto_connect_list.Count)
            {
                data[5] = Set_Auto_connect_list[index_set_auto_list - 1].peer[0];
                data[6] = Set_Auto_connect_list[index_set_auto_list - 1].peer[1];
                data[7] = Set_Auto_connect_list[index_set_auto_list - 1].peer[2];
                data[8] = Set_Auto_connect_list[index_set_auto_list - 1].peer[3];
                data[9] = Set_Auto_connect_list[index_set_auto_list - 1].peer[4];
                data[10] = Set_Auto_connect_list[index_set_auto_list - 1].peer[5];
            }
            data[11] = creat_crc(data, 11);
            data[12] = (byte)CMD.TX_ETX;
            return data;
        }

        public byte[] reset()
        {
            byte[] data = new byte[5];
            data[0] = (byte)CMD.TX_STX;
            data[1] = 0x01;
            data[2] = (byte)CMD.Reset;
            data[3] = creat_crc(data, 3);
            data[4] = (byte)CMD.TX_ETX;
            return data;
        }

        public byte[] version()
        {
            byte[] data = new byte[5];
            data[0] = (byte)CMD.TX_STX;
            data[1] = 0x01;
            data[2] = (byte)CMD.Version;
            data[3] = creat_crc(data, 3);
            data[4] = (byte)CMD.TX_ETX;
            return data;
        }

        public string serial_Receive(byte[] rx_data, string time)
        {
            string log = "";
            string str = "";
            device_info info = new device_info();
            Flag_show_log = true;
            try
            {
                if (rx_data[0] == (byte)CMD.RX_STX &&
                    rx_data[rx_data.Length - 1] == (byte)CMD.RX_ETX &&
                    check_crc(rx_data, rx_data.Length - 2)
                   )
                {
                    Flag_is_change_device_show = false;
                    switch (rx_data[2])
                    {
                        case (byte)CMD.Scan_Start:
                            log += "Scan start" + Environment.NewLine;
                            ble_device_mac.Clear();
                            ble_device.Clear();
                            scan_device_info.Clear();
                            //Flag_is_change_device_show = true;
                            //Flag_is_change_device_connect_show = true;
                            break;

                        case (byte)CMD.AVD_Report:
                            Flag_show_log = false;
                            string str_deviec_name = "";
                            info.peer = new byte[6];
                            for (int i = 0; i < 6; i++)
                            {
                                str += String.Format("{0:X2}", rx_data[i + 3]);
                                if (i != 5) str += ":";
                                info.peer[i] = rx_data[i + 3];
                            }

                            for (int i = 0; i < rx_data[1] - 6 - 1 - 1; i++)
                            {
                                if(rx_data[i + 9] != 0x00)
                                    str_deviec_name += Convert.ToChar(rx_data[i + 9]);
                            }
                            info.str_device_name = str_deviec_name;
                            info.str_device_mac = str;
                            if (Flag_set_auto_list_scan == false)
                            {
                                Flag_is_change_device_show = false;
                                for (int i = 0; i < ble_device_mac.Count; i++)
                                {
                                    if (ble_device_mac[i] == str)
                                    {
                                        Flag_is_change_device_show = true;
                                        int rssi = rx_data[rx_data[1] + 2];
                                        rssi = rssi % 127;
                                        ble_device[i] = str_deviec_name + " " + str + "(-" + (rssi).ToString() + "dBm)";
                                        scan_device_info[i] = info;
                                    }
                                }
                                if (Flag_is_change_device_show == false)
                                {
                                    Flag_is_change_device_show = true;
                                    ble_device_mac.Add(str);
                                    int rssi = rx_data[rx_data[1] + 2];
                                    rssi = rssi % 127;
                                    ble_device.Add(str_deviec_name + " " + str + "(-" + (rssi).ToString() + "dBm)");
                                    scan_device_info.Add(info);
                                }
                            }
                            else
                            {
                                Flag_is_change_device_show = false;
                                for (int i = 0; i < Set_Auto_connect_list_info.Count; i++)
                                {
                                    if (Set_Auto_connect_list_info[i].str_device_mac == str)
                                    {
                                        Flag_is_change_device_show = true;
                                    }
                                }
                                if (Flag_is_change_device_show == false)
                                {
                                    Flag_is_change_device_show = true;
                                    ble_device_mac.Add(str);
                                    info.str_device_name = "Scan";
                                    Set_Auto_connect_list_info.Add(info);
                                }
                            }
                            break;

                        case (byte)CMD.Connect:
                            info.peer = new byte[6];
                            for (int i = 0; i < 6; i++)
                            {
                                info.peer[i] = rx_data[i+3];
                                str += String.Format("{0:X2}", rx_data[i + 3]);
                                if (i != 5) str += ":";
                            }

                            for (int i = 0; i < scan_device_info.Count; i++)
                            {
                                if (scan_device_info[i].str_device_mac == str)
                                    info = scan_device_info[i];
                            }
                            info.last_number = 0;
                            info.error_counter = 0;
                            info.catch_counter = 0;
                            info.str_device_mac = str;
                            //ble_device_connect.Add(info.str_device_name + " " + info.str_device_mac);
                            for (int i = 0; i < scan_device_info.Count; i++)
                            {
                                if (scan_device_info[i].str_device_mac == str)
                                {
                                    log += "Connect:" + str + Environment.NewLine;
                                    Flag_is_change_device_connect_show = true;
                                    Flag_is_connect_success = true;
                                    break;
                                }
                            }
                            connected_device_info.Add(info);
                            log += "Connect:" + str + Environment.NewLine;
                            Flag_is_change_device_connect_show = true;
                            Flag_is_connect_success = true;
                            break;

                        case (byte)CMD.Disconnect:
                            for (int i = 0; i < 6; i++)
                            {
                                str += String.Format("{0:X2}", rx_data[i + 3]);
                                if (i != 5) str += ":";
                            }
                            for (int i = 0; i < connected_device_info.Count; i++)
                            {
                                if (str == connected_device_info[i].str_device_mac)
                                {
                                    connected_device_info.RemoveAt(i);
                                    break;
                                }
                            }
                            Flag_is_change_device_connect_show = true;
                            log += "Disconnect" + Environment.NewLine;
                            break;

                        case (byte)CMD.BLE_RX:
                            Flag_show_log = false;
                            payload payload_tmp = new payload();
                            string str_detail = "";
                            int number = 0;
                            int index = 0;
                            string str_ASCII = "";
                            for (int i = 0; i < 6; i++)
                            {
                                str += String.Format("{0:X2}", rx_data[i + 3]);
                                if (i != 5) str += ":";
                            }

                            for (int i = 0; i < connected_device_info.Count; i++)
                            {
                                if (connected_device_info[i].str_device_mac == str)
                                {
                                    if(connected_device_info[i].str_device_name != null)
                                        str = connected_device_info[i].str_device_name+"_"+ str;
                                    else
                                        str = "n_a" + "_" + str;
                                    index = i;
                                }
                            }

                            for (int i = 0; i < rx_data[1] - 7; i++)
                            {
                                str_detail += String.Format("{0:X2}", rx_data[i + 9])+" ";
                                str_ASCII += (char)rx_data[i + 9];
                            }

                            //try
                            //{
                            //    number = Int32.Parse(str_ASCII);
                            //    int buffer = number - connected_device_info[index].last_number;
                            //    if (number == 0 && connected_device_info[index].last_number == 65535)
                            //    {
                            //        //nothing
                            //    }
                            //    else if (buffer != 1)
                            //    {
                            //        if(buffer > 0)
                            //            connected_device_info[index].error_counter += buffer-1;
                            //        else
                            //            connected_device_info[index].error_counter ++;
                            //    }
                            //    connected_device_info[index].last_number = number;
                            //    connected_device_info[index].catch_counter++;
                            //    //Flag_is_change_device_connect_show = true;
                            //}
                            //catch (Exception ex)
                            //{

                            //}

                            //try
                            //{
                            //    connected_device_info[index].catch_counter++;
                            //    payload_tmp.doc_name = str.Replace(':', '-') + ".txt";
                            //    payload_tmp.content = "[" + time + "] " + str_detail + " " + str_ASCII + " " + connected_device_info[index].error_counter + "/" + connected_device_info[index].catch_counter;
                            //}
                            //catch (Exception ex)
                            //{
                            //    payload_tmp.doc_name = str.Replace(':', '-') + ".txt";
                            //    payload_tmp.content = "[" + time + "] " + str_detail + " " + str_ASCII;
                            //}
                            //ble_rx_payload.Add(payload_tmp);
                            //Flag_is_no_show_log = true;
                            //log += "[" + time + "] " + str + " RX: " + str_detail+Environment.NewLine;
                            break;

                        case (byte)CMD.BLE_TX:
                      
                            for (int i = 0; i < 6; i++)
                            {
                                str += String.Format("{0:X2}", rx_data[i + 3]);
                                if (i != 5) str += ":";
                            }
                            if(rx_data[9] == 0)
                                log += "[" + time + "] " + str + " TX: fail"+ Environment.NewLine;
                            else
                                log += "[" + time + "] " + str + " TX: ok" + Environment.NewLine;
                            Flag_is_no_show_log = true;
                            break;

                        case (byte)CMD.Scan_Stop:
                            if (rx_data[3] == 0x01)
                                log += "Set Scan stop (ok)" + Environment.NewLine;
                            else
                                log += "Set Scan stop (fail)" + Environment.NewLine;
                            break;

                        case (byte)CMD.Set_UUID:
                                if (rx_data[3] == 0x01)
                                    log += "Set UUID ok" + Environment.NewLine;
                                else
                                    log += "Set UUID error(busy)" + Environment.NewLine;
                            break;

                        case (byte)CMD.Get_UUID:
                                for (int i = 0; i < 16; i++)
                                    str += String.Format("{0:X2}", rx_data[18 - i]);
                                str_UUID = str;
                                Flag_is_UUID_show = true;
                                log = "Get UUID :" + str + Environment.NewLine;
                            break;

                        case (byte)CMD.Set_Auto_connect_list:
                            if (rx_data[3] == 0x01)
                                Flag_set_auto_list = true;
                            break;

                        case (byte)CMD.Get_Auto_connect_list:
                            if (rx_data[4] == 0x01 || rx_data[4] == 0x00) Get_Auto_connect_list.Clear();
                            if (rx_data[4] == 0x00) break;
                            info.peer = new byte[6];
                            for (int i = 0; i < 6; i++)
                            {
                                info.peer[i] = rx_data[i + 5];
                                info.str_device_mac += String.Format("{0:X2}", rx_data[i + 5]);
                                if (i != 5) info.str_device_mac += ":";
                            }
                            Flag_get_auto_connect_list = true;
                            Flag_is_auto_connect_list_show = true;
                            Get_Auto_connect_list.Add(info);
                            Flag_is_change_device_connect_show = true;
                            break;

                        case (byte)CMD.Get_Connect_list:
                            bool flag = false;
                            if (rx_data[4] == 0x01) connected_device_info.Clear();
                            info.peer = new byte[6];
                            if (rx_data[5] == 1) flag = true;
                            for (int i = 0; i < 6; i++)
                            {
                                info.peer[i] = rx_data[i + 6];
                                info.str_device_mac += String.Format("{0:X2}", rx_data[i + 6]);
                                if (i != 5) info.str_device_mac += ":";
                                
                            }
                            if (flag)
                            {
                                connected_device_info.Add(info);
                            }
                            Flag_is_change_device_connect_show = true;
                            Flag_get_connected_list = true;
                            break;

                        case (byte)CMD.Start:
                            log = "START" + Environment.NewLine;
                            break;

                        case (byte)CMD.Reset:
                            log = "Reset" + Environment.NewLine;
                            break;

                        case (byte)CMD.Version:
                            str_version = "";
                            for (int i = 0; i < rx_data[1] - 1; i++)
                                str_version += (char)rx_data[i+3];
                            if (rx_data[1] > 1) Flag_get_version = true;
                            break;
                    }
                }
            }
            catch (Exception ex) { }
            return log;
        }


        public byte creat_crc(byte[] b, int length)
        {
            byte crc = 0;
            for (int i = 0; i < length; i++)
                crc ^= b[i];
            return crc;
        }

        public bool check_crc(byte[] b, int length)
        {
            byte crc = 0;
            for (int i = 0; i < length; i++)
                crc ^= b[i];
            if (crc == b[length])
                return true;
            else
                return false;
        }

    }
}
