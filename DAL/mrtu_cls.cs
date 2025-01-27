using kyj_project.Common;
using Modbus.Device;
using System;
using System.Linq;

namespace kyj_project.DAL
{
    public class mrtu_cls
    {


        /// <summary>
        /// 获取MODBUS TCP相应地址位数据
        /// </summary>
        /// <param name="_mm">MODBUS TCP实例</param>
        /// <param name="dizhi">地址参数</param>
        /// <returns></returns>
        public static string get_mrtu_value(ModbusSerialMaster _mm, string dizhi)
        {
            try
            {
                //地址参数 从站地址|寄存器|寄存器数量|数据类型（Int/DInt/Real/Bool）|高低位互换（0/1）|位数（-1代表没有）
                //地址参数样式： 1|4100|1|Int|0|-1
                string[] s = dizhi.Split('|');
                if (s.Length == 6)
                {
                    byte s0 = byte.Parse(s[0]);//从站地址
                    ushort s1 = ushort.Parse(s[1]);//寄存器
                    ushort s2 = ushort.Parse(s[2]);//寄存器数量(1：16位；2：32位)
                    string s3 = s[3];//数据类型（Int/DInt/Real/Bool）
                    int s4 = int.Parse(s[4]);//高低位互换（0/1）
                    int s5 = int.Parse(s[5]);//位数（-1代表没有）

                    if (s2 == 1)
                    {
                        ushort[] runtime = new ushort[16];
                        runtime = _mm.ReadHoldingRegisters(s0, s1, s2);

                        if (s3.ToLower() == "int")
                        {
                            return runtime[0].ToString();
                        }
                        else if (s3.ToLower() == "bool")
                        {
                            if (s5 >= 0)
                            {
                                bool bvalue = ((runtime[0] >> s5) & 0x01) == 1;
                                if (bvalue == true) { return "1"; } else { return "0"; }
                            }
                        }
                    }

                    if (s2 == 2)
                    {
                        ushort[] runtime = new ushort[32];
                        runtime = _mm.ReadHoldingRegisters(s0, s1, s2);

                        if (s3.ToLower() == "dint")
                        {
                            int result = ushorts2int(runtime, s4);
                            return result.ToString();
                        }
                        else if (s3.ToLower() == "real")
                        {
                            float result = ushort2float(runtime, s4);
                            return result.ToString();
                            //if (s5 >= 0)
                            //{
                            //    ushort result = runtime[s5];
                            //    return result.ToString();
                            //}
                        }
                    }
                }
                else
                {
                    return "";
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 写入MODBUS TCP相应地址数据
        /// </summary>
        /// <param name="_mm"></param>
        /// <param name="dizhi"></param>
        /// <param name="modbus_value"></param>
        /// <returns></returns>
        public static string set_mrtu_value(ModbusSerialMaster _mm, string dizhi, string modbus_value)
        {
            try
            {
                //地址参数 从站地址|寄存器|寄存器数量|数据类型（Int/DInt/Real/Bool）|高低位互换（0/1）|位数（-1代表没有）
                //地址参数样式： 1|4100|1|Int|0|-1
                string[] s = dizhi.Split('|');
                if (s.Length == 6)
                {
                    byte s0 = byte.Parse(s[0]);//从站地址
                    ushort s1 = ushort.Parse(s[1]);//寄存器
                    ushort s2 = ushort.Parse(s[2]);//寄存器数量(1：16位；2：32位)
                    string s3 = s[3];//数据类型（Int/DInt/Real/Bool）
                    int s4 = int.Parse(s[4]);//高低位互换（0/1）
                    int s5 = int.Parse(s[5]);//位数（-1代表没有）

                    if (s2 == 1)
                    {
                        //ushort[] runtime = new ushort[16];
                        //runtime = _mm.ReadHoldingRegisters(s0, s1, s2);

                        if (s3.ToLower() == "int")
                        {
                            ushort runtime = ushort.Parse(modbus_value);
                            _mm.WriteSingleRegister(s0, s1, runtime);

                        }
                        else if (s3.ToLower() == "bool")
                        {
                            if (s5 >= 0)
                            {
                                // 读取当前寄存器的值
                                ushort currentValue = _mm.ReadHoldingRegisters(s0, s1, s2)[0];

                                // 根据布尔值更新特定的位
                                if (modbus_value == "1")
                                {
                                    // 设置特定的位为1
                                    currentValue |= (ushort)(1 << s5);
                                }
                                else
                                {
                                    // 设置特定的位为0
                                    currentValue &= (ushort)~(1 << s5);
                                }

                                // 写入更新后的寄存器值
                                _mm.WriteSingleRegister(s0, s1, currentValue);

                            }
                        }
                    }

                    if (s2 == 2)
                    {
                        //ushort[] runtime = new ushort[32];
                        //runtime = _mm.ReadHoldingRegisters(s0, s1, s2);

                        if (s3.ToLower() == "dint")
                        {
                            int dint = Utility.ToInt(modbus_value);
                            ushort[] runtime = DintToUshortArray(dint, s4);
                            _mm.WriteMultipleRegisters(s0, s1, runtime);
                        }
                        else if (s3.ToLower() == "real")
                        {
                            ushort[] runtime = FloatToUshortArray(float.Parse(modbus_value), s4);
                            _mm.WriteMultipleRegisters(s0, s1, runtime);
                        }
                    }
                }
                else
                {
                    return "";
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public static int ushorts2int(ushort[] res, int gao_di)
        {
            if (gao_di == 0)
            {
                int high = res[0];
                int low = res[1];
                int value = (high << 16) + low;
                return value;
            }
            else
            {
                int high = res[0];
                int low = res[1];
                int value = (low << 16) + high;
                return value;
            }
        }

        public static ushort[] DintToUshortArray(int dint, int gao_di)
        {
            if (gao_di == 1)
            {
                // 要发送的 DINT 数据
                //int dintValue = 123456789; 
                // 示例 DINT 值 
                // 将 DINT 转换为两个寄存器 (MODBUS 中的寄存器是 16 位的)
                ushort[] ushorts = new ushort[2];
                ushorts[0] = (ushort)(dint & 0xFFFF); // 低位
                ushorts[1] = (ushort)((dint >> 16) & 0xFFFF); // 高位
                return ushorts;
            }
            else
            {
                // 要发送的 DINT 数据
                //int dintValue = 123456789; 
                // 示例 DINT 值 
                // 将 DINT 转换为两个寄存器 (MODBUS 中的寄存器是 16 位的)
                ushort[] ushorts = new ushort[2];
                ushorts[1] = (ushort)(dint & 0xFFFF); // 低位
                ushorts[0] = (ushort)((dint >> 16) & 0xFFFF); // 高位
                return ushorts;
            }
        }

        public static float ushort2float(ushort[] data, int gao_di)
        {
            if (gao_di == 0)
            {
                ushort floatValue1 = data[0];
                ushort floatValue2 = data[1];
                float result = BitConverter.ToSingle(BitConverter.GetBytes(floatValue1).Concat(BitConverter.GetBytes(floatValue2)).ToArray(), 0);
                return result;
            }
            else
            {
                ushort floatValue1 = data[0];
                ushort floatValue2 = data[1];
                float result = BitConverter.ToSingle(BitConverter.GetBytes(floatValue2).Concat(BitConverter.GetBytes(floatValue1)).ToArray(), 0);
                return result;
            }
        }

        public static ushort[] FloatToUshortArray(float value, int gao_di)
        {
            if (gao_di == 0)
            {
                // 将 decimal 转换为两个寄存器 (MODBUS 中的寄存器是 16 位的)
                ushort[] ushorts = new ushort[2];
                byte[] floatBytes = BitConverter.GetBytes(value);
                // 将 decimal 的字节拆分到两个 16 位寄存器
                ushorts[0] = BitConverter.ToUInt16(floatBytes, 0); // 低位
                ushorts[1] = BitConverter.ToUInt16(floatBytes, 2); // 高位 
                return ushorts;
            }
            else
            {
                // 将 decimal 转换为两个寄存器 (MODBUS 中的寄存器是 16 位的)
                ushort[] ushorts = new ushort[2];
                byte[] floatBytes = BitConverter.GetBytes(value);
                // 将 decimal 的字节拆分到两个 16 位寄存器
                ushorts[1] = BitConverter.ToUInt16(floatBytes, 0); // 低位
                ushorts[0] = BitConverter.ToUInt16(floatBytes, 2); // 高位 
                return ushorts;
            }

        }

        /// <summary>
        /// 地址校验
        /// </summary>
        /// <param name="dizhi"></param>
        /// <returns></returns>
        public static string modbus_check_dizhi(string dizhi)
        {
            //地址参数 从站地址| 寄存器 | 寄存器数量 | 数据类型（Int / DInt / Real / Bool）| 高低位互换（0 / 1）| 位数（-1代表没有）
            //地址参数样式： 1 | 4100 | 1 | Int | 0 | -1
            string[] s = dizhi.Split('|');
            if (s.Length == 6)
            {
                if (s[3].ToLower() == "dint" || s[3].ToLower() == "real")
                {
                    if (s[2] != "2") { return "地址格式不正确"; }
                }
                if (s[3].ToLower() == "int" || s[3].ToLower() == "bool")
                {
                    if (s[2] != "1") { return "地址格式不正确"; }
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
