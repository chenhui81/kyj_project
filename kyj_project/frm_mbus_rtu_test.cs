using kyj_project.DAL;
using Modbus.Device;
using System;
using System.IO.Ports;
using System.Windows.Forms;


namespace kyj_project
{
    public partial class frm_mbus_rtu_test : Form
    {

        // ModbusMaster _mm;
        public frm_mbus_rtu_test()
        {
            InitializeComponent();
            this.Text = biz_cls.xiangmu_mingcheng;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] s = this.textBox1.Text.Split('|');
            string portName = s[0];  // 串口名称
            int baudRate = int.Parse(s[1]);       // 波特率
            int parity = int.Parse(s[2]);            // 校验位，0: 无校验，1: 偶校验，2: 奇校验
            int dataBits = int.Parse(s[3]);          // 数据位
            int stopBits = int.Parse(s[4]);          // 停止位

            //1、打开串口连接
            var serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);
            try
            {
                serialPort.Open();

                // 2. 创建 Modbus RTU 主机对象
                var modbusRtuMaster = ModbusSerialMaster.CreateRtu(serialPort);
                this.lb_zt.Text = "Modbus RTU已连接";

                serialPort.Close();
            }
            catch (Exception ex)
            {
                this.lb_zt.Text = ex.Message;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] s = this.textBox1.Text.Split('|');
            string portName = s[0];  // 串口名称
            int baudRate = int.Parse(s[1]);       // 波特率
            int parity = int.Parse(s[2]);            // 校验位，0: 无校验，1: 偶校验，2: 奇校验
            int dataBits = int.Parse(s[3]);          // 数据位
            int stopBits = int.Parse(s[4]);          // 停止位



            try
            {
                //1、打开串口连接
                var serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);
                serialPort.Open();

                // 2. 创建 Modbus RTU 主机对象
                var modbusRtuMaster = ModbusSerialMaster.CreateRtu(serialPort);

                // 3. 读取从站设备的寄存器
                string s1 = mrtu_cls.get_mrtu_value(modbusRtuMaster, this.textBox2.Text);
                this.lb_zt.Text = "读取值：" + s1;

                // 4. 关闭串口连接
                //  modbusRtuMaster.Dispose();
                serialPort.Close();
            }
            catch (Exception ex)
            {
                this.lb_zt.Text = ex.Message;
            }
        }

        private void frm_mbus_rtu_test_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] s = this.textBox1.Text.Split('|');
            string portName = s[0];  // 串口名称
            int baudRate = int.Parse(s[1]);       // 波特率
            int parity = int.Parse(s[2]);            // 校验位，0: 无校验，1: 偶校验，2: 奇校验
            int dataBits = int.Parse(s[3]);          // 数据位
            int stopBits = int.Parse(s[4]);          // 停止位

            //1、打开串口连接
            var serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);

            try
            {
                serialPort.Open();

                // 2. 创建 Modbus RTU 主机对象
                var modbusRtuMaster = ModbusSerialMaster.CreateRtu(serialPort);


                // 3. 读取从站设备的寄存器
                string s1 = mrtu_cls.set_mrtu_value(modbusRtuMaster, this.textBox2.Text, this.textBox4.Text);
                if (s1 == "")
                {
                    this.lb_zt.Text = "写入成功";
                }
                else
                {
                    this.lb_zt.Text = s1;
                }

                // 4. 关闭串口连接
                //  modbusRtuMaster.Dispose();
                serialPort.Close();
            }
            catch
            {
                serialPort.Close();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
