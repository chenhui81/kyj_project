using kyj_project.Common;
using S7.Net;
using System;


namespace kyj_project.DAL
{
    public class s7_cls
    {
        /// <summary>
        /// 获取PLC实例
        /// </summary>
        /// <param name="cpu_type">CPUtype 规格型号</param>
        /// <param name="ip">IP地址</param>
        /// <param name="jijiahao">机架号</param>
        /// <param name="chacaohao">插槽号</param>
        /// <returns></returns>
        public static Plc get_plc(string cpu_type, string ip, short jijiahao, short chacaohao)
        {
            try
            {
                Plc MyPLC = new Plc(s7_cls.get_plc_type(cpu_type), ip, jijiahao, chacaohao);
                return MyPLC;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取PLC实例
        /// </summary>
        /// <param name="conn_str"> 规格型号|IP|机架号|插槽号</param>
        /// <returns></returns>
        public static Plc get_plc(string conn_str)
        {
            try
            {
                string[] s = conn_str.Split('|');
                if (s.Length == 4)
                {
                    Plc MyPLC = new Plc(s7_cls.get_plc_type(s[0]), s[1], short.Parse(s[2]), short.Parse(s[3]));
                    return MyPLC;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw null;
            }
        }

        public static CpuType get_plc_type(string cpu_type)
        {
            //选择的PLC型号 默认S7-1200
            switch (cpu_type)
            {
                case "S7-200":
                    return CpuType.S7200;
                case "LOGO0BA8":
                    return CpuType.Logo0BA8;
                case "S7-200Smart":
                    return CpuType.S7200Smart;
                case "S7-300":
                    return CpuType.S7300;
                case "S7-1200":
                    return CpuType.S71200;
                case "S7-1500":
                    return CpuType.S71500;
                default:
                    return CpuType.S71200;
            }
        }

        /// <summary>
        /// 获取PLC相关地址数据
        /// </summary>
        /// <param name="_plc"></param>
        /// <param name="dizhi"></param>
        /// <returns></returns>
        public static string get_plc_value(Plc _plc, string dizhi)
        {
            try
            {
                if (dizhi.Contains("DB"))
                {
                    //除bool类型之外的地址
                    string[] s1 = dizhi.Split('|');
                    if (s1.Length == 2)
                    {
                        string[] s = s1[0].Split('.');
                        if (s.Length == 2)
                        {
                            if (s1[1].ToLower() == "real")
                            {

                                return ((uint)_plc.Read(s1[0])).ConvertToFloat().ToString();
                            }
                            else
                            {
                                return Utility.ToObjectString(_plc.Read(s1[0]));
                            }
                        }
                        else
                        {
                            throw new Exception("数据类型格式错误");
                        }
                    }
                    else
                    {
                        throw new Exception("数据类型格式错误");
                    }
                }
                else
                {
                    //bool类型地址读取
                    string[] s1 = dizhi.Split('|');
                    if (s1.Length == 2)
                    {
                        string[] s = s1[0].Split('.');
                        if (s.Length == 3)
                        {
                            int dbNumber = Convert.ToInt32(s[0]); // 数据块号
                            int startByte = Convert.ToInt32(s[1]); // 开始字节
                            int size = 1; // 读取的字节数

                            byte[] data = _plc.ReadBytes(DataType.DataBlock, dbNumber, startByte, size);

                            // 提取点的状态
                            bool bitValue = ((data[0] >> Convert.ToInt32(s[2])) & 0x01) == 1;
                            if (bitValue == true) { return "1"; } else { return "0"; }
                        }
                        else
                        {
                            throw new Exception("bool类型格式错误");
                        }
                    }
                    else
                    {
                        throw new Exception("bool类型格式错误");
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }



        /// <summary>
        /// 写入PLC相关地址数据
        /// </summary>
        /// <param name="_plc">PLC实例</param>
        /// <param name="dizhi_type">地址类型Word/Int/DWord/DInt/Real</param>
        /// <param name="dizhi">地址</param>
        /// <param name="plc_value">数据值</param>
        public static void set_plc_value(Plc _plc, string dizhi, string plc_value)
        {
            try
            {
                string[] s1 = dizhi.Split('|');
                if (s1.Length == 2)
                {
                    //写数据
                    switch (s1[1].ToLower())
                    {
                        case "word":
                            ushort value1 = ushort.Parse(plc_value);
                            _plc.Write(s1[0], value1);
                            break;
                        case "int":
                            short value2 = short.Parse(plc_value);
                            _plc.Write(s1[0], value2);
                            break;
                        case "dword":
                            uint value3 = uint.Parse(plc_value);
                            _plc.Write(s1[0], value3);
                            break;
                        case "dint":
                            int value4 = int.Parse(plc_value);
                            _plc.Write(s1[0], value4);
                            break;
                        case "real":
                            decimal value5 = decimal.Parse(plc_value);
                            _plc.Write(s1[0], value5);
                            break;
                        case "bool":
                            string[] s = s1[0].Split('.');
                            if (s.Length == 3)
                            {
                                if (plc_value == "1")
                                {
                                    _plc.Write("DB" + s[0] + ".DBX" + s[1] + "." + s[2], true);
                                }
                                else
                                {
                                    _plc.Write("DB" + s[0] + ".DBX" + s[1] + "." + s[2], false);
                                }
                            }
                            else
                            {
                                throw new Exception("输入的地址格式不正确");
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// S7PLC 连接字符串校验
        /// </summary>
        /// <param name="constr"></param>
        /// <returns></returns>
        public static string s7_check_constr(string constr)
        {

            //规格型号 | IP | 机架号 | 插槽号
            //S7 - 1200 | 192.168.0.1 | 0 | 1
            string[] s = constr.Split('|');
            if (s.Length == 4)
            {
                if (s[1].Split('.').Length != 4)
                {
                    return "连接字符串格式不正确";
                }
            }
            else
            {
                return "连接字符串格式不正确";
            }

            return "";
        }

        /// <summary>
        /// 地址校验
        /// </summary>
        /// <param name="dizhi"></param>
        /// <returns></returns>
        public static string s7_check_dizhi(string dizhi)
        {
            //非Bool类型：DB1.DB15 | Real
            //Bool类型：1.15.2 | Bool
            string[] s = dizhi.Split('|');
            if (s.Length == 2)
            {
                if (s[0].Contains("DB"))
                {
                    string[] s1 = s[0].Split('.');
                    if (s1.Length != 2) { return "地址格式不正确"; }
                    if (s[1].ToLower() == "real" || s[1].ToLower() == "dint")
                    {
                        if (s1[1].Contains("DBD") == false) { return "地址格式不正确"; }
                    }
                }
                else
                {
                    if (s[0].Split('.').Length != 3)
                    {
                        return "地址格式不正确";
                    }
                    else
                    {
                        if (s[1].ToLower() != "bool") { return "地址格式不正确"; }
                    }
                }
            }
            else
            {
                return "地址格式不正确";
            }

            return "";

        }
    }
}
