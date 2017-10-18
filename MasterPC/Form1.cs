using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.IO.Ports;
using Microsoft.Win32;
using ZedGraph;

namespace MasterPC
{
    public partial class MasterPC_Form_Main : Form
    {

        #region 变量初始化
        //串口调试
        public string[] SerialPortName;//串口名称
        public long SerialPort_ReceiveData_Count = 0;//接收数据计数
        public long SerialPort_SendData_Count = 0;//发送数据计数
        public bool SerialPort_Listening_Flag = false;//侦听Invoke执行标志
        public bool SerialPort_Closing_Flag = false;//串口正在关闭标志
        public static volatile bool SerialPort_OpenClose_Sleep = false;//开关串口线程睡眠标志
        public static volatile bool SerialPort_ReceiveData_Finish = false;//串口接收完成
        public static long SerialPort_ReceiveData_Last = 0;//串口接收速度
        public static string ReceiveDataString = null;//串口接收字符串


        //实时曲线
        //串口曲线
        public string[] SerialPortName_Curve;//串口名称
        public long SerialPort_Curve_ReceiveData_Count = 0;//接收数据计数
        public static int SerialPort_ReadBuffSize = 256;//串口缓冲数组大小(256 byte)
        public static byte[] SerialPort_ReadBuff = new byte[SerialPort_ReadBuffSize];//串口缓冲数组
        public static volatile byte[] SerialPort_CheckDataBuff = new byte[20];//拆分串口数组数据
        public static volatile int[] SerialPort_Curve_Data = new int[8];//串口曲线数据
        public static volatile bool SerialPort_DataCheck_Finish = false;//数据拆分完成
        public static volatile bool SerialPort_CCD_Check_Finish = false;//CCD数据拆分完成
        public static volatile string TabControl_Curve_SelectedTabNow = "串口曲线";//选择页当前选项

        //ZedGraph
        public static LineItem ZedGraph_Curve_1;//曲线1
        public static LineItem ZedGraph_Curve_2;//曲线2
        public static LineItem ZedGraph_Curve_3;//曲线3
        public static LineItem ZedGraph_Curve_4;//曲线4
        public static LineItem ZedGraph_Curve_5;//曲线5
        public static LineItem ZedGraph_Curve_6;//曲线6
        public static LineItem ZedGraph_Curve_7;//曲线7
        public static LineItem ZedGraph_Curve_8;//曲线8

        public static volatile PointPairList ZedGraph_List_1 = new PointPairList();//曲线1数据
        public static volatile PointPairList ZedGraph_List_2 = new PointPairList();//曲线2数据
        public static volatile PointPairList ZedGraph_List_3 = new PointPairList();//曲线3数据
        public static volatile PointPairList ZedGraph_List_4 = new PointPairList();//曲线4数据
        public static volatile PointPairList ZedGraph_List_5 = new PointPairList();//曲线5数据
        public static volatile PointPairList ZedGraph_List_6 = new PointPairList();//曲线6数据
        public static volatile PointPairList ZedGraph_List_7 = new PointPairList();//曲线7数据
        public static volatile PointPairList ZedGraph_List_8 = new PointPairList();//曲线8数据

        //CCD曲线
        //Chart
        public static volatile List<int> ChartLine_List_1 = new List<int>();//ChartLine1
        public static volatile List<int> ChartLine_List_2 = new List<int>();//ChartLine2
        public static volatile int Curve_CCD_Number_Flag = 0;//CCD数量
        public static volatile int Curve_CCD_Sample_Bit_Flag = 0;//CCD采样精度标志

        //数据分析
        //图形Image
        private List<byte> File_Data_List = new List<byte>();//数据线性链表
        private List<int> File_Data_List_Index_First = new List<int>();//数据线性链表起始
        private List<int> File_Data_List_Index_Last = new List<int>();//数据线性链表末尾
        private List<Bitmap> File_Data_BitmapList = new List<Bitmap>();//图形库链表
        private List<bool> File_Data_List_ImageFlag = new List<bool>();//图形标志
        private int Image_Index = 0;//图形指针索引(当前帧)
        private int Image_Frame = 0;//图形帧数(总帧数)
        private int Image_Color_Now = 1;//图形当前颜色(Black:1,White:0)
        private int Image_Row = 102;//图形行号
        private int Image_Col = 220;//图形列号
        private int Image_Jump = 25;//图形跳变点
        private int Image_Line_Now = 0;//图形当前绘图行
        private int Image_Cell_Width = 1;//图形单元宽度
        private int Image_Cell_Height = 2;//图形单元高度
        private int Image_Play_Order = 0;//图形播放顺序(0:正序;1:反序)

        //图标Chart
        private int Chart_Amplitude = 150;//图表幅值(图表Y轴最大值)
        //变量参数
        private List<int> Curve_Data_List_1 = new List<int>();//曲线1链表
        private List<int> Curve_Data_List_2 = new List<int>();//曲线2链表
        private List<int> Curve_Data_List_3 = new List<int>();//曲线3链表
        private List<int> Curve_Data_List_4 = new List<int>();//曲线4链表
        private List<int> Curve_Data_List_5 = new List<int>();//曲线5链表
        private List<int> Curve_Data_List_6 = new List<int>();//曲线6链表
        private List<int> Curve_Data_List_7 = new List<int>();//曲线7链表
        private List<int> Curve_Data_List_8 = new List<int>();//曲线8链表
        private List<int> Curve_Data_List_9 = new List<int>();//曲线9链表
        
        //控制参数
        private List<int> Curve_Data_List_10 = new List<int>();//曲线10链表
        private List<int> Curve_Data_List_11 = new List<int>();//曲线11链表
        private List<int> Curve_Data_List_12 = new List<int>();//曲线12链表
        private List<int> Curve_Data_List_13 = new List<int>();//曲线13链表
        private List<int> Curve_Data_List_14 = new List<int>();//曲线14链表
        private List<int> Curve_Data_List_15 = new List<int>();//曲线15链表
        private List<int> Curve_Data_List_16 = new List<int>();//曲线16链表
        private List<int> Curve_Data_List_17 = new List<int>();//曲线17链表
        private List<int> Curve_Data_List_18 = new List<int>();//曲线18链表

        //HideCaret函数引用
        [DllImport("user32", EntryPoint = "HideCaret")]
        private static extern bool HideCaret(IntPtr hWnd);

        #endregion

        #region 窗体初始化加载与关闭
        public MasterPC_Form_Main()//窗体初始化
        {
            InitializeComponent();
        }

        private void MasterPC_Form_Main_Load(object sender, EventArgs e)//窗体加载
        {
            #region 窗体初始化
            /*
             * Form
             */
            //Form窗体设置
            this.DoubleBuffered = true;//窗体开启双缓冲
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;//窗体禁止改变大小(固定的单行边框)
            this.MaximizeBox = false;//窗体禁止最大化
            #endregion

            #region 串口调试选项卡初始化
            /*
             * TabControl_TabPage(串口调试)
             */
            //ComboBox设置
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉

            //RadioButton设置
            this.MasterPC_TabPage_SerialPort_Receive_RadioButton_String.Checked = true;//RadioButton接收默认选择ASCII字符
            this.MasterPC_TabPage_SerialPort_Receive_RadioButton_Hex.Checked = false;//RadioButton接收默认不选择Hex字符
            this.MasterPC_TabPage_SerialPort_Send_RadioButton_String.Checked = true;//RadioButton发送默认选择ASCII字符
            this.MasterPC_TabPage_SerialPort_Send_RadioButton_Hex.Checked = false;//RadioButton发送默认不选择Hex字符

            //CheckBox设置
            this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Enabled = false;//CheckBox默认禁止
            this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Checked = false;//CheckBox默认不选择定时发送
            this.MasterPC_TabPage_SerialPort_CheckBox_ReceiveNewLine.Enabled = false;//CheckBox默认禁止
            this.MasterPC_TabPage_SerialPort_CheckBox_ReceiveNewLine.Checked = false;//CheckBox默认不选择接收换行

            //NumberUpDown设置
            this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Maximum = 10000;//定时发送最大时间10s
            this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Minimum = 100;//定时发送最小时间0.1s
            this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Value = 1000;//定时发送默认时间1s
            this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Increment = 100;//定时发送默认改变时间0.1s

            //TextBox设置
            this.MasterPC_TabPage_SerialPort_TextBox_Receive.ReadOnly = true;//TextBox接收默认只读
            this.MasterPC_TabPage_SerialPort_TextBox_Send.ReadOnly = false;//TextBox发送默认读写
            this.MasterPC_TabPage_SerialPort_TextBox_Information.ReadOnly = true;//TextBox信息提示默认只读
            this.MasterPC_TabPage_SerialPort_TextBox_Information_SPNumber.ReadOnly = true;//TextBox串口号信息默认只读
            this.MasterPC_TabPage_SerialPort_TextBox_Information_SPBaud.ReadOnly = true;//TextBox波特率信息默认只读
            this.MasterPC_TabPage_SerialPort_TextBox_Receive.BackColor = Color.White;//TextBox接收背景白色
            this.MasterPC_TabPage_SerialPort_TextBox_Information.BackColor = Color.White;//TextBox信息提示背景白色
            this.MasterPC_TabPage_SerialPort_TextBox_Information_SPNumber.BackColor = Color.White;//TextBox串口号信息背景白色
            this.MasterPC_TabPage_SerialPort_TextBox_Information_SPBaud.BackColor = Color.White;//TextBox波特率信息背景白色
            this.MasterPC_TabPage_SerialPort_TextBox_Receive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;//TextBox接收滚动条
            this.MasterPC_TabPage_SerialPort_TextBox_Send.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;//TextBox发送滚动条

            //Timer设置
            this.Timer_Timing_ReceiveSpeed.Enabled = false;//Timer定时

            /*
             * 串口配置
             */
            //串口号配置
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Items.Clear();//清空列表中的项目
            SerialPortName = SerialPort.GetPortNames();//检测串口

            if (SerialPortName.Length != 0)//检测到COM口
            {
                foreach (string SerialPort_n in System.IO.Ports.SerialPort.GetPortNames())
                {
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Items.Add(SerialPort_n);//添加COM口
                }
                this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.SelectedIndex = 0;//默认选择第一个COM口
            }
            else//未检测到COM口
            {
                this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.SelectedIndex = -1;//默认为COM口为空
            }

            //波特率配置
            //SerialPort
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Items.Clear();//清空列表中的项目
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Items.Add("9600");//串口波特率9600
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Items.Add("14400");//串口波特率14400
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Items.Add("19200");//串口波特率19200
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Items.Add("38400");//串口波特率38400
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Items.Add("57600");//串口波特率57600
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Items.Add("115200");//串口波特率115200
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Items.Add("128000");//串口波特率128000
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Items.Add("256000");//串口波特率256000
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.SelectedIndex = 5;//默认选择第一个波特率配置(115200bps)

            //数据位配置
            //SerialPort
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Items.Clear();//清空列表中的项目
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Items.Add("5");//数据位5位
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Items.Add("6");//数据位6位
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Items.Add("7");//数据位7位
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Items.Add("8");//数据位8位
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.SelectedIndex = 3;//默认选择第五个数据位配置(数据位:8Bit)

            //停止位配置
            //SerialPort
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Items.Clear();//清空列表中的项目
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Items.Add("1");//停止位1位
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Items.Add("2");//停止位2位
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.SelectedIndex = 0;//默认选择第一个停止位配置(停止位:1Bit)

            //校验位配置
            //SerialPort
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Items.Clear();//清空列表中的项目
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Items.Add("无校验");//无校验
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Items.Add("奇校验");//奇校验
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Items.Add("偶校验");//偶校验
            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.SelectedIndex = 0;//默认选择无校验

            /*
             * 串口提示信息
             */
            if (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.SelectedIndex == 0)//检测到串口
            {
                //SerialPort串口助手显示
                this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "已检测到串口!";
                this.MasterPC_TabPage_SerialPort_TextBox_Information_SPNumber.Text = this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Text;
                this.MasterPC_TabPage_SerialPort_TextBox_Information_SPBaud.Text = this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Text;
            }
            else//未检测到串口
            {
                //SerialPort串口助手显示
                this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "未检测到串口,请刷新重试!";
                this.MasterPC_TabPage_SerialPort_TextBox_Information_SPNumber.Text = "";
                this.MasterPC_TabPage_SerialPort_TextBox_Information_SPBaud.Text = "";
            }

            //关闭串口
            this.SerialPortNow.Close();//关闭串口

            #endregion

            #region 实时曲线选项卡初始化
            /*
             * TabControl_TabPage(实时曲线)
             */
            //串口曲线
            //ComboBox设置
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉

            //TextBox设置
            this.MasterPC_TabPage_Curve_TextBox_Information.ReadOnly = true;//TextBox信息提示默认只读
            this.MasterPC_TabPage_Curve_TextBox_Information_SPNumber.ReadOnly = true;//TextBox串口号信息默认只读
            this.MasterPC_TabPage_Cruve_TextBox_Information_SPBaud.ReadOnly = true;//TextBox波特率信息默认只读
            this.MasterPC_TabPage_Curve_TextBox_Information.BackColor = Color.White;//TextBox信息提示背景白色
            this.MasterPC_TabPage_Curve_TextBox_Information_SPNumber.BackColor = Color.White;//TextBox串口号信息背景白色
            this.MasterPC_TabPage_Cruve_TextBox_Information_SPBaud.BackColor = Color.White;//TextBox波特率信息背景白色

            //ZegGraph设置
            MasterPC_TabControl_Curve_ZedGraph_Init();//ZedGraph初始化

            //NumberUpDown设置
            this.MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Minimum = 1;//曲线数量最小值1
            this.MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Maximum = 8;//曲线数量最大值8
            this.MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Value = 4;//当前曲线数量4
            this.MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Increment = 1;//曲线数量变化步长1

            //CheckBox设置
            this.MasterPC_TabControl_Curve_CheckBox1.Enabled = true;//曲线1可用
            this.MasterPC_TabControl_Curve_CheckBox2.Enabled = true;//曲线2可用
            this.MasterPC_TabControl_Curve_CheckBox3.Enabled = true;//曲线3可用
            this.MasterPC_TabControl_Curve_CheckBox4.Enabled = true;//曲线4可用
            this.MasterPC_TabControl_Curve_CheckBox5.Enabled = false;//曲线5不可用
            this.MasterPC_TabControl_Curve_CheckBox6.Enabled = false;//曲线6不可用
            this.MasterPC_TabControl_Curve_CheckBox7.Enabled = false;//曲线7不可用
            this.MasterPC_TabControl_Curve_CheckBox8.Enabled = false;//曲线8不可用

            this.MasterPC_TabControl_Curve_CheckBox1.Checked = true;//曲线1选中
            this.MasterPC_TabControl_Curve_CheckBox2.Checked = true;//曲线2选中
            this.MasterPC_TabControl_Curve_CheckBox3.Checked = true;//曲线3选中
            this.MasterPC_TabControl_Curve_CheckBox4.Checked = true;//曲线4选中
            this.MasterPC_TabControl_Curve_CheckBox5.Checked = false;//曲线5不选中
            this.MasterPC_TabControl_Curve_CheckBox6.Checked = false;//曲线6不选中
            this.MasterPC_TabControl_Curve_CheckBox7.Checked = false;//曲线7不选中
            this.MasterPC_TabControl_Curve_CheckBox8.Checked = false;//曲线8不选中

            //Label设置
            this.MasterPC_TabControl_Curve_Label1_Value.Enabled = true;//曲线1数值可用
            this.MasterPC_TabControl_Curve_Label2_Value.Enabled = true;//曲线2数值可用
            this.MasterPC_TabControl_Curve_Label3_Value.Enabled = true;//曲线3数值可用
            this.MasterPC_TabControl_Curve_Label4_Value.Enabled = true;//曲线4数值可用
            this.MasterPC_TabControl_Curve_Label5_Value.Enabled = false;//曲线5数值不可用
            this.MasterPC_TabControl_Curve_Label6_Value.Enabled = false;//曲线6数值不可用
            this.MasterPC_TabControl_Curve_Label7_Value.Enabled = false;//曲线7数值不可用
            this.MasterPC_TabControl_Curve_Label8_Value.Enabled = false;//曲线8数值不可用

            //CCD曲线
            //ComboBox设置
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.Items.Clear();//ComboBox清空
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.Items.Add("1");//ComboBox添加项
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.Items.Add("2");//ComboBox添加项
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.SelectedIndex = 0;//ComboBox默认选中"1"
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.Items.Clear();//ComboBox清空
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.Items.Add("8Bit");//ComboBox添加项
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.Items.Add("10Bit");//ComboBox添加项
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.Items.Add("12Bit");//ComboBox添加项
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.SelectedIndex = 0;//ComboBox默认选中"8Bit"

            //CCD_Chart1
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisX.Minimum = 0;//Chart1X轴最小值
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisX.Maximum = 127;//Chart1X轴最大值
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisY.Minimum = 0;//Chart1Y轴最小值
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisY.Maximum = 255;//Chart1Y轴最大值
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].CursorX.LineDashStyle = ChartDashStyle.Dash;//设置虚线游标
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].CursorX.LineColor = Color.LightCoral;//游标红色
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].CursorX.Position = 64;

            //CCD_Chart2
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisX.Minimum = 0;//Chart2X轴最小值
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisX.Maximum = 127;//Chart2X轴最大值
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisY.Minimum = 0;//Chart2Y轴最小值
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisY.Maximum = 255;//Chart2Y轴最大值
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].CursorX.LineDashStyle = ChartDashStyle.Dash;//设置虚线游标
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].CursorX.LineColor = Color.LightCoral;//游标红色
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].CursorX.Position = 64;

            //添加示例曲线
            for (int i = 0; i < 128; i++)
            {
                ChartLine_List_1.Add(i);
                ChartLine_List_2.Add(i);
            }
            ChartLine_List_1[63] = 0;
            ChartLine_List_2[63] = 0;

            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.Series[0].Points.DataBindY(ChartLine_List_1);
            this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.Series[0].Points.DataBindY(ChartLine_List_2);

            //摄像头图像
            //ComboBox设置
            this.MasterPC_TabPage_Curve_ComboBox_Camera_Type.DropDownStyle = ComboBoxStyle.DropDownList;//ComboBox默认只能下拉
            this.MasterPC_TabPage_Curve_ComboBox_Camera_Type.Items.Clear();//ComboBox清空列表内容
            this.MasterPC_TabPage_Curve_ComboBox_Camera_Type.Items.Add("二值化图像");//ComboBox添加项
            this.MasterPC_TabPage_Curve_ComboBox_Camera_Type.SelectedIndex = 0;//ComboBox选中二值化图像
            //RadioButton设置
            this.MasterPC_TabPage_Curve_RadioButton_Camera_Type1.Checked = true;//RadioButton选择原始图像
            this.MasterPC_TabPage_Curve_RadioButton_Camera_Type2.Checked = false;//RadioButton不选择矫正图像
            //TextBox设置

            //功能区控件可见性
            //串口曲线可见
            this.MasterPC_TabControl_Curve_CheckBox1.Visible = true;//CheckBox1可见
            this.MasterPC_TabControl_Curve_CheckBox2.Visible = true;//CheckBox2可见
            this.MasterPC_TabControl_Curve_CheckBox3.Visible = true;//CheckBox3可见
            this.MasterPC_TabControl_Curve_CheckBox4.Visible = true;//CheckBox4可见
            this.MasterPC_TabControl_Curve_CheckBox5.Visible = true;//CheckBox5可见
            this.MasterPC_TabControl_Curve_CheckBox6.Visible = true;//CheckBox6可见
            this.MasterPC_TabControl_Curve_CheckBox7.Visible = true;//CheckBox7可见
            this.MasterPC_TabControl_Curve_CheckBox8.Visible = true;//CheckBox8可见

            this.MasterPC_TabControl_Curve_Label1_Value.Visible = true;//Label1可见
            this.MasterPC_TabControl_Curve_Label2_Value.Visible = true;//Label2可见
            this.MasterPC_TabControl_Curve_Label3_Value.Visible = true;//Label3可见
            this.MasterPC_TabControl_Curve_Label4_Value.Visible = true;//Label4可见
            this.MasterPC_TabControl_Curve_Label5_Value.Visible = true;//Label5可见
            this.MasterPC_TabControl_Curve_Label6_Value.Visible = true;//Label6可见
            this.MasterPC_TabControl_Curve_Label7_Value.Visible = true;//Label7可见
            this.MasterPC_TabControl_Curve_Label8_Value.Visible = true;//Label8可见

            this.MasterPC_TabControl_Curve_Label_CurveNum.Visible = true;//Label_CurveNum可见
            this.MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Visible = true;//NumberUpDown可见
            this.MasterPC_TabControl_Curve_Button_CurveClear.Visible = true;//Button_CurveClear可见

            //CCD曲线不可见
            this.MasterPC_TabPage_Curve_GropBox_CCD_Function.Visible = false;//GroupBox线性CCD配置不可见
            this.MasterPC_TabPage_Curve_GropBox_CCD_Value.Visible = false;//GroupBox线性CCD数值不可见

            this.MasterPC_TabPage_Curve_Label_CCD_Number.Visible = false;//LabelCCD数量不可见
            this.MasterPC_TabPage_Curve_Label_CCD_Bit.Visible = false;//LabelCCD精度不可见
            this.MasterPC_TabPage_Curve_Label_CCD1_Max.Visible = false;//LabelCCD1最大值不可见
            this.MasterPC_TabPage_Curve_Label_CCD1_Min.Visible = false;//LabelCCD1最小值不可见
            this.MasterPC_TabPage_Curve_Label_CCD2_Max.Visible = false;//LabelCCD2最大值不可见
            this.MasterPC_TabPage_Curve_Label_CCD2_Min.Visible = false;//LabelCCD2最小值不可见

            this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.Visible = false;//ComboBoxCCD数量不可见
            this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.Visible = false;//ComboBoxCCD精度不可见

            this.MasterPC_TabPage_Curve_CheckBox_CCD_Grid.Visible = false;//CheckBox网格不可见
            this.MasterPC_TabPage_Curve_CheckBox_CCD_Point.Visible = false;//CheckBox数据点不可见

            //摄像头图像不可见
            this.MasterPC_TabPage_Curve_GropBox_Camera_Function.Visible = false;//GroupBox摄像头配置不可见

            this.MasterPC_TabPage_Curve_ComboBox_Camera_Type.Visible = false;//ComboBox摄像头类型不可见

            this.MasterPC_TabPage_Curve_TextBox_Camera_Row.Visible = false;//TextBox摄像头行号不可见
            this.MasterPC_TabPage_Curve_TextBox_Camera_Col.Visible = false;//TextBox摄像头列号不可见

            this.MasterPC_TabPage_Curve_RadioButton_Camera_Type1.Visible = false;//RadioButton摄像头行号不可见
            this.MasterPC_TabPage_Curve_RadioButton_Camera_Type2.Visible = false;//RadioButton摄像头列号不可见

            this.MasterPC_TabPage_Curve_Label_Camera_Type.Visible = false;//Label摄像头类型不可见
            this.MasterPC_TabPage_Curve_Label_Camera_Row.Visible = false;//Label摄像头行号不可见
            this.MasterPC_TabPage_Curve_Label_Camera_Col.Visible = false;//Label摄像头列号不可见

          /*
           * 串口配置
           */
            //串口号配置
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Items.Clear();//清空列表中的项目
            SerialPortName_Curve = SerialPort.GetPortNames();//检测串口

            if (SerialPortName_Curve.Length != 0)//检测到COM口
            {
                foreach (string SerialPort_n in System.IO.Ports.SerialPort.GetPortNames())
                {
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Items.Add(SerialPort_n);//添加COM口
                }
                this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.SelectedIndex = 0;//默认选择第一个COM口
            }
            else//未检测到COM口
            {
                this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.SelectedIndex = -1;//默认为COM口为空
            }

            //波特率配置
            //Curve
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Items.Clear();//清空列表中的项目
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Items.Add("9600");//串口波特率9600
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Items.Add("14400");//串口波特率14400
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Items.Add("19200");//串口波特率19200
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Items.Add("38400");//串口波特率38400
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Items.Add("57600");//串口波特率57600
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Items.Add("115200");//串口波特率115200
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Items.Add("128000");//串口波特率128000
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Items.Add("256000");//串口波特率256000
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.SelectedIndex = 5;//默认选择第一个波特率配置(115200bps)

            //数据位配置
            //Curve
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Items.Clear();//清空列表中的项目
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Items.Add("5");//数据位5位
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Items.Add("6");//数据位6位
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Items.Add("7");//数据位7位
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Items.Add("8");//数据位8位
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.SelectedIndex = 3;//默认选择第五个数据位配置(数据位:8Bit)

            //停止位配置
            //Curve
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Items.Clear();//清空列表中的项目
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Items.Add("1");//停止位1位
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Items.Add("2");//停止位2位
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.SelectedIndex = 0;//默认选择第一个停止位配置(停止位:1Bit)

            //校验位配置
            //Curve
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Items.Clear();//清空列表中的项目
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Items.Add("无校验");//无校验
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Items.Add("奇校验");//奇校验
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Items.Add("偶校验");//偶校验
            this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.SelectedIndex = 0;//默认选择无校验

            /*
             * 串口提示信息
             */
            if (this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.SelectedIndex == 0)//检测到串口
            {
                //Curve实时曲线显示
                this.MasterPC_TabPage_Curve_TextBox_Information.Text = "已检测到串口!";
                this.MasterPC_TabPage_Curve_TextBox_Information_SPNumber.Text = this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Text;
                this.MasterPC_TabPage_Cruve_TextBox_Information_SPBaud.Text = this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Text;
            }
            else//未检测到串口
            {
                //Curve实时曲线显示
                this.MasterPC_TabPage_Curve_TextBox_Information.Text = "未检测到串口,请刷新重试!";
                this.MasterPC_TabPage_Curve_TextBox_Information_SPNumber.Text = "";
                this.MasterPC_TabPage_Cruve_TextBox_Information_SPBaud.Text = "";
            }

            //关闭串口
            this.SerialPortCurve.Close();//关闭串口

            #endregion

            #region 数据分析选项卡初始化
            /*
             * TabControl_TabPage(数据分析)
             */
            RegistryKey MyReg1, MyReg2;

            MyReg1 = Registry.CurrentUser;
            MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项

            //Chart设置
            //图表区域设置
            this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisX.Minimum = 0;//ChartX轴最小值
            this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisX.Maximum = 100;//ChartX轴最大值
            this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisY.Minimum = 0;//ChartY轴最小值
            this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisY.Maximum = 150;//ChartY轴最大值
            this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].CursorX.LineDashStyle = ChartDashStyle.Dash;//设置虚线游标
            this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].CursorX.LineColor = Color.Red;//游标红色
            this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].CursorX.LineWidth = 2;//游标线条宽度
            //this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].CursorX.Position = 50;//游标显示
            //图表曲线设置
            this.MasterPC_TabPage_Data_Chart_Chart.Series[0].Color = Color.LightCoral;//Chart曲线1颜色浅红色
            this.MasterPC_TabPage_Data_Chart_Chart.Series[1].Color = Color.LightBlue;//Chart曲线2颜色浅蓝色
            this.MasterPC_TabPage_Data_Chart_Chart.Series[2].Color = Color.Orange;//Chart曲线3颜色橙色
            this.MasterPC_TabPage_Data_Chart_Chart.Series[3].Color = Color.LightGreen;//Chart曲线4颜色浅绿色
            this.MasterPC_TabPage_Data_Chart_Chart.Series[4].Color = Color.LightSeaGreen;//Chart曲线5颜色浅海绿
            this.MasterPC_TabPage_Data_Chart_Chart.Series[5].Color = Color.Gold;//Chart曲线6颜色金黄色
            this.MasterPC_TabPage_Data_Chart_Chart.Series[6].Color = Color.LawnGreen;//Chart曲线7颜色浅草绿色
            this.MasterPC_TabPage_Data_Chart_Chart.Series[7].Color = Color.Plum;//Chart曲线8颜色浅紫色
            this.MasterPC_TabPage_Data_Chart_Chart.Series[8].Color = Color.LightPink;//Chart曲线9颜色浅粉红色
            this.MasterPC_TabPage_Data_Chart_Chart.Series[0].BorderWidth = 2;//Chart曲线1线条宽度2
            this.MasterPC_TabPage_Data_Chart_Chart.Series[1].BorderWidth = 2;//Chart曲线2线条宽度2
            this.MasterPC_TabPage_Data_Chart_Chart.Series[2].BorderWidth = 2;//Chart曲线3线条宽度2
            this.MasterPC_TabPage_Data_Chart_Chart.Series[3].BorderWidth = 2;//Chart曲线4线条宽度2
            this.MasterPC_TabPage_Data_Chart_Chart.Series[4].BorderWidth = 2;//Chart曲线5线条宽度2
            this.MasterPC_TabPage_Data_Chart_Chart.Series[5].BorderWidth = 2;//Chart曲线6线条宽度2
            this.MasterPC_TabPage_Data_Chart_Chart.Series[6].BorderWidth = 2;//Chart曲线7线条宽度2
            this.MasterPC_TabPage_Data_Chart_Chart.Series[7].BorderWidth = 2;//Chart曲线8线条宽度2
            this.MasterPC_TabPage_Data_Chart_Chart.Series[8].BorderWidth = 2;//Chart曲线9线条宽度2

            //NumberUpDown设置
            this.MasterPC_TabPage_Data_NumberUpDown_Jump.Maximum = 1000;//暂定最大帧数1000
            this.MasterPC_TabPage_Data_NumberUpDown_Jump.Minimum = 0;//暂定最小帧数0
            this.MasterPC_TabPage_Data_NumberUpDown_Jump.Value = 0;//暂定当前帧数0
            this.MasterPC_TabPage_Data_NumberUpDown_Jump.Increment = 1;//帧数增量1

            //RadioButton设置
            this.MasterPC_TabPage_Data_RadioButton_Original.Checked = true;//初始选择原始图形
            this.MasterPC_TabPage_Data_RadioButton_Rectify.Checked = false;//初始不选择矫正图形

            //CheckBox设置
            this.MasterPC_TabPage_Data_CheckBox_Center.Checked = true;//初始选择显示中心边

            //Timer设置
            this.Timer_Timing_ImagePlay.Enabled = false;//图形播放定时器关闭

            //TextBox设置
            //功能设置
            this.MasterPC_TabPage_Data_TextBox_Page.ReadOnly = true;//图像帧只读
            this.MasterPC_TabPage_Data_TextBox_Page.BackColor = Color.White;//图像帧背景色白色

            //图形设置
            this.MasterPC_TabPage_Data_TextBox_Row.MaxLength = 3;//图形行号限制
            this.MasterPC_TabPage_Data_TextBox_Col.MaxLength = 3;//图形列号限制
            this.MasterPC_TabPage_Data_TextBox_Jump.MaxLength = 2;//图形跳变限制
            this.MasterPC_TabPage_Data_TextBox_YMaxValue.MaxLength = 3;//图表幅值限制

            //控制参数
            this.MasterPC_TabPage_Data_TextBox_Angle_P.ReadOnly = true;//控制参数只读
            this.MasterPC_TabPage_Data_TextBox_Angle_D.ReadOnly = true;//控制参数只读
            this.MasterPC_TabPage_Data_TextBox_Speed_P.ReadOnly = true;//控制参数只读
            this.MasterPC_TabPage_Data_TextBox_Speed_I.ReadOnly = true;//控制参数只读
            this.MasterPC_TabPage_Data_TextBox_Direc_P.ReadOnly = true;//控制参数只读
            this.MasterPC_TabPage_Data_TextBox_Direc_D.ReadOnly = true;//控制参数只读
            this.MasterPC_TabPage_Data_TextBox_Cotrl_1.ReadOnly = true;//控制参数只读
            this.MasterPC_TabPage_Data_TextBox_Cotrl_2.ReadOnly = true;//控制参数只读
            this.MasterPC_TabPage_Data_TextBox_Cotrl_3.ReadOnly = true;//控制参数只读
            this.MasterPC_TabPage_Data_TextBox_Angle_P.BackColor = Color.SkyBlue;//控制参数背景色天蓝色
            this.MasterPC_TabPage_Data_TextBox_Angle_D.BackColor = Color.SkyBlue;//控制参数背景色天蓝色
            this.MasterPC_TabPage_Data_TextBox_Speed_P.BackColor = Color.SkyBlue;//控制参数背景色天蓝色
            this.MasterPC_TabPage_Data_TextBox_Speed_I.BackColor = Color.SkyBlue;//控制参数背景色天蓝色
            this.MasterPC_TabPage_Data_TextBox_Direc_P.BackColor = Color.SkyBlue;//控制参数背景色天蓝色
            this.MasterPC_TabPage_Data_TextBox_Direc_D.BackColor = Color.SkyBlue;//控制参数背景色天蓝色
            this.MasterPC_TabPage_Data_TextBox_Cotrl_1.BackColor = Color.SkyBlue;//控制参数背景色天蓝色
            this.MasterPC_TabPage_Data_TextBox_Cotrl_2.BackColor = Color.SkyBlue;//控制参数背景色天蓝色
            this.MasterPC_TabPage_Data_TextBox_Cotrl_3.BackColor = Color.SkyBlue;//控制参数背景色天蓝色

            //变量参数
            this.MasterPC_TabPage_Data_TextBox_Var1.BackColor = Color.LightCoral;//实时参数曲线1浅红色
            this.MasterPC_TabPage_Data_TextBox_Var2.BackColor = Color.LightBlue;//实时参数曲线2浅蓝色
            this.MasterPC_TabPage_Data_TextBox_Var3.BackColor = Color.Orange;//实时参数曲线3橙色
            this.MasterPC_TabPage_Data_TextBox_Var4.BackColor = Color.LightGreen;//实时参数曲线4浅绿色
            this.MasterPC_TabPage_Data_TextBox_Var5.BackColor = Color.LightSeaGreen;//实时参数曲线5浅海绿
            this.MasterPC_TabPage_Data_TextBox_Var6.BackColor = Color.Gold;//实时参数曲线6金黄色
            this.MasterPC_TabPage_Data_TextBox_Var7.BackColor = Color.LawnGreen;//实时参数曲线7浅草绿色
            this.MasterPC_TabPage_Data_TextBox_Var8.BackColor = Color.Plum;//实时参数曲线8浅紫色
            this.MasterPC_TabPage_Data_TextBox_Var9.BackColor = Color.LightPink;//实时参数曲线9浅粉红色

            this.MasterPC_TabPage_Data_TextBox_Value_Var1.BackColor = Color.LightCoral;//实时参数曲线1浅红色
            this.MasterPC_TabPage_Data_TextBox_Value_Var2.BackColor = Color.LightBlue;//实时参数曲线2浅蓝色
            this.MasterPC_TabPage_Data_TextBox_Value_Var3.BackColor = Color.Orange;//实时参数曲线3橙色
            this.MasterPC_TabPage_Data_TextBox_Value_Var4.BackColor = Color.LightGreen;//实时参数曲线4浅绿色
            this.MasterPC_TabPage_Data_TextBox_Value_Var5.BackColor = Color.LightSeaGreen;//实时参数曲线5浅海绿
            this.MasterPC_TabPage_Data_TextBox_Value_Var6.BackColor = Color.Gold;//实时参数曲线6金黄色
            this.MasterPC_TabPage_Data_TextBox_Value_Var7.BackColor = Color.LawnGreen;//实时参数曲线7浅草绿色
            this.MasterPC_TabPage_Data_TextBox_Value_Var8.BackColor = Color.Plum;//实时参数曲线8浅紫色
            this.MasterPC_TabPage_Data_TextBox_Value_Var9.BackColor = Color.LightPink;//实时参数曲线9浅粉红色
            this.MasterPC_TabPage_Data_TextBox_Value_Var1.ReadOnly = true;//实时参数曲线1只读
            this.MasterPC_TabPage_Data_TextBox_Value_Var2.ReadOnly = true;//实时参数曲线2只读
            this.MasterPC_TabPage_Data_TextBox_Value_Var3.ReadOnly = true;//实时参数曲线3只读
            this.MasterPC_TabPage_Data_TextBox_Value_Var4.ReadOnly = true;//实时参数曲线4只读
            this.MasterPC_TabPage_Data_TextBox_Value_Var5.ReadOnly = true;//实时参数曲线5只读
            this.MasterPC_TabPage_Data_TextBox_Value_Var6.ReadOnly = true;//实时参数曲线6只读
            this.MasterPC_TabPage_Data_TextBox_Value_Var7.ReadOnly = true;//实时参数曲线7只读
            this.MasterPC_TabPage_Data_TextBox_Value_Var8.ReadOnly = true;//实时参数曲线8只读
            this.MasterPC_TabPage_Data_TextBox_Value_Var9.ReadOnly = true;//实时参数曲线9只读

            //图形设置参数读取
            //图形行号
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Row.Text = MyReg2.GetValue("Image_Row").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Row.Text = "102";
            }

            Image_Row = Convert.ToInt16(this.MasterPC_TabPage_Data_TextBox_Row.Text);
            //图形列号
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Col.Text = MyReg2.GetValue("Image_Col").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Col.Text = "220";
            }

            Image_Col = Convert.ToInt16(this.MasterPC_TabPage_Data_TextBox_Col.Text);
            //图形跳变
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Jump.Text = MyReg2.GetValue("Image_Jump").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Jump.Text = "25";
            }

            Image_Jump = Convert.ToInt16(this.MasterPC_TabPage_Data_TextBox_Jump.Text);
            //图表幅值
            try
            {
                this.MasterPC_TabPage_Data_TextBox_YMaxValue.Text = MyReg2.GetValue("Chart_Amplitude").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_YMaxValue.Text = "150";
            }

            Chart_Amplitude = Convert.ToInt16(this.MasterPC_TabPage_Data_TextBox_YMaxValue.Text);

            //变量参数名称读取
            //曲线1
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Var1.Text = MyReg2.GetValue("Var1").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Var1.Text = "Var1";
            }
            //曲线2
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Var2.Text = MyReg2.GetValue("Var2").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Var2.Text = "Var2";
            }
            //曲线3
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Var3.Text = MyReg2.GetValue("Var3").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Var3.Text = "Var3";
            }
            //曲线4
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Var4.Text = MyReg2.GetValue("Var4").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Var4.Text = "Var4";
            }
            //曲线5
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Var5.Text = MyReg2.GetValue("Var5").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Var5.Text = "Var5";
            }
            //曲线6
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Var6.Text = MyReg2.GetValue("Var6").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Var6.Text = "Var6";
            }
            //曲线7
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Var7.Text = MyReg2.GetValue("Var7").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Var7.Text = "Var7";
            }
            //曲线8
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Var8.Text = MyReg2.GetValue("Var8").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Var8.Text = "Var8";
            }
            //曲线9
            try
            {
                this.MasterPC_TabPage_Data_TextBox_Var9.Text = MyReg2.GetValue("Var9").ToString();
            }
            catch (Exception)
            {
                this.MasterPC_TabPage_Data_TextBox_Var9.Text = "Var9";
            }


            #endregion

            #region 帮助选项卡初始化
            //ComboBox设置
            this.MasterPC_TabPage_Help_ComboBox_Items.DropDownStyle = ComboBoxStyle.DropDownList;
            this.MasterPC_TabPage_Help_ComboBox_Do.DropDownStyle = ComboBoxStyle.DropDownList;
            this.MasterPC_TabPage_Help_ComboBox_Items.Items.Clear();
            this.MasterPC_TabPage_Help_ComboBox_Items.Items.Add("串口曲线");
            this.MasterPC_TabPage_Help_ComboBox_Items.Items.Add("CCD曲线");
            this.MasterPC_TabPage_Help_ComboBox_Items.SelectedIndex = 0;
            this.MasterPC_TabPage_Help_ComboBox_Do.Items.Clear();
            this.MasterPC_TabPage_Help_ComboBox_Do.Items.Add("代码");
            this.MasterPC_TabPage_Help_ComboBox_Do.SelectedIndex = 0;

            //TextBox设置
            this.MasterPC_TabPage_Help_TextBox_Help_SerialPort.ReadOnly = true;
            this.MasterPC_TabPage_Help_TextBox_Help_SerialPort.BackColor = Color.White;
            this.MasterPC_TabPage_Help_TextBox_Help_SerialPort.ScrollBars = ScrollBars.Vertical;

            this.MasterPC_TabPage_Help_TextBox_Help_CCD.ReadOnly = true;
            this.MasterPC_TabPage_Help_TextBox_Help_CCD.BackColor = Color.White;
            this.MasterPC_TabPage_Help_TextBox_Help_CCD.ScrollBars = ScrollBars.Vertical;

            this.MasterPC_TabPage_Help_TextBox_Help_CCD.Visible = false;
            this.MasterPC_TabPage_Help_TextBox_Help_SerialPort.Visible = true;

            #endregion

            #region 关于选项卡初始化
            #endregion
        }

        private void MasterPC_Form_Main_FormClosed(object sender, FormClosedEventArgs e)//窗体关闭
        {
        }

        #endregion

        #region 选项卡页面
        private void MasterPC_TabControl_Main_SelectedIndexChanged(object sender, EventArgs e)//主页面选项卡改变
        {
            //当前TabControl_Main选项卡选中的选项页
            switch (this.MasterPC_TabControl_Main.SelectedTab.Text)
            {
                case "串口调试"://当前TabPage选中"串口调试"页面
                    break;
                case "实时曲线"://当前TabPage选中"实时曲线"页面
                    break;
                case "数据分析"://当前TabPage选中"数据分析"页面
                    break;
                case "帮助":    //当前TabPage选中"帮助"页面
                    break;
                case "关于":    //当前TabPage选中"关于"页面
                    break;
                default:
                    MessageBox.Show("选项卡选择发生错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void MasterPC_TabControl_Curve_SelectedIndexChanged(object sender, EventArgs e)//实时曲线选项卡改变
        {
            //当前TabControl_Curve选项卡选中的选项页
            switch (this.MasterPC_TabControl_Curve.SelectedTab.Text)
            {
                case "串口曲线"://当前TabPage选中"串口曲线"页面
                    TabControl_Curve_SelectedTabNow = "串口曲线";
                    //串口曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Visible = true;//CheckBox1可见
                    this.MasterPC_TabControl_Curve_CheckBox2.Visible = true;//CheckBox2可见
                    this.MasterPC_TabControl_Curve_CheckBox3.Visible = true;//CheckBox3可见
                    this.MasterPC_TabControl_Curve_CheckBox4.Visible = true;//CheckBox4可见
                    this.MasterPC_TabControl_Curve_CheckBox5.Visible = true;//CheckBox5可见
                    this.MasterPC_TabControl_Curve_CheckBox6.Visible = true;//CheckBox6可见
                    this.MasterPC_TabControl_Curve_CheckBox7.Visible = true;//CheckBox7可见
                    this.MasterPC_TabControl_Curve_CheckBox8.Visible = true;//CheckBox8可见

                    this.MasterPC_TabControl_Curve_Label1_Value.Visible = true;//Label1可见
                    this.MasterPC_TabControl_Curve_Label2_Value.Visible = true;//Label2可见
                    this.MasterPC_TabControl_Curve_Label3_Value.Visible = true;//Label3可见
                    this.MasterPC_TabControl_Curve_Label4_Value.Visible = true;//Label4可见
                    this.MasterPC_TabControl_Curve_Label5_Value.Visible = true;//Label5可见
                    this.MasterPC_TabControl_Curve_Label6_Value.Visible = true;//Label6可见
                    this.MasterPC_TabControl_Curve_Label7_Value.Visible = true;//Label7可见
                    this.MasterPC_TabControl_Curve_Label8_Value.Visible = true;//Label8可见

                    this.MasterPC_TabControl_Curve_Label_CurveNum.Visible = true;//Label_CurveNum可见
                    this.MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Visible = true;//NumberUpDown可见
                    this.MasterPC_TabControl_Curve_Button_CurveClear.Visible = true;//Button_CurveClear可见

                    //CCD曲线
                    this.MasterPC_TabPage_Curve_GropBox_CCD_Function.Visible = false;//GroupBox线性CCD配置不可见
                    this.MasterPC_TabPage_Curve_GropBox_CCD_Value.Visible = false;//GroupBox线性CCD数值不可见

                    this.MasterPC_TabPage_Curve_Label_CCD_Number.Visible = false;//LabelCCD数量不可见
                    this.MasterPC_TabPage_Curve_Label_CCD_Bit.Visible = false;//LabelCCD精度不可见
                    this.MasterPC_TabPage_Curve_Label_CCD1_Max.Visible = false;//LabelCCD1最大值不可见
                    this.MasterPC_TabPage_Curve_Label_CCD1_Min.Visible = false;//LabelCCD1最小值不可见
                    this.MasterPC_TabPage_Curve_Label_CCD2_Max.Visible = false;//LabelCCD2最大值不可见
                    this.MasterPC_TabPage_Curve_Label_CCD2_Min.Visible = false;//LabelCCD2最小值不可见

                    this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.Visible = false;//ComboBoxCCD数量不可见
                    this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.Visible = false;//ComboBoxCCD精度不可见

                    this.MasterPC_TabPage_Curve_CheckBox_CCD_Grid.Visible = false;//CheckBox网格不可见
                    this.MasterPC_TabPage_Curve_CheckBox_CCD_Point.Visible = false;//CheckBox数据点不可见

                    //摄像头图像不可见
                    this.MasterPC_TabPage_Curve_GropBox_Camera_Function.Visible = false;//GroupBox摄像头配置不可见

                    this.MasterPC_TabPage_Curve_ComboBox_Camera_Type.Visible = false;//ComboBox摄像头类型不可见

                    this.MasterPC_TabPage_Curve_TextBox_Camera_Row.Visible = false;//TextBox摄像头行号不可见
                    this.MasterPC_TabPage_Curve_TextBox_Camera_Col.Visible = false;//TextBox摄像头列号不可见

                    this.MasterPC_TabPage_Curve_RadioButton_Camera_Type1.Visible = false;//RadioButton摄像头行号不可见
                    this.MasterPC_TabPage_Curve_RadioButton_Camera_Type2.Visible = false;//RadioButton摄像头列号不可见

                    this.MasterPC_TabPage_Curve_Label_Camera_Type.Visible = false;//Label摄像头类型不可见
                    this.MasterPC_TabPage_Curve_Label_Camera_Row.Visible = false;//Label摄像头行号不可见
                    this.MasterPC_TabPage_Curve_Label_Camera_Col.Visible = false;//Label摄像头列号不可见

                    break;
                case "线性CCD"://当前TabPage选中"CCD"页面
                    TabControl_Curve_SelectedTabNow = "线性CCD";
                    //串口曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Visible = false;//CheckBox1不可见
                    this.MasterPC_TabControl_Curve_CheckBox2.Visible = false;//CheckBox2不可见
                    this.MasterPC_TabControl_Curve_CheckBox3.Visible = false;//CheckBox3不可见
                    this.MasterPC_TabControl_Curve_CheckBox4.Visible = false;//CheckBox4不可见
                    this.MasterPC_TabControl_Curve_CheckBox5.Visible = false;//CheckBox5不可见
                    this.MasterPC_TabControl_Curve_CheckBox6.Visible = false;//CheckBox6不可见
                    this.MasterPC_TabControl_Curve_CheckBox7.Visible = false;//CheckBox7不可见
                    this.MasterPC_TabControl_Curve_CheckBox8.Visible = false;//CheckBox8不可见

                    this.MasterPC_TabControl_Curve_Label1_Value.Visible = false;//Label1不可见
                    this.MasterPC_TabControl_Curve_Label2_Value.Visible = false;//Label2不可见
                    this.MasterPC_TabControl_Curve_Label3_Value.Visible = false;//Label3不可见
                    this.MasterPC_TabControl_Curve_Label4_Value.Visible = false;//Label4不可见
                    this.MasterPC_TabControl_Curve_Label5_Value.Visible = false;//Label5不可见
                    this.MasterPC_TabControl_Curve_Label6_Value.Visible = false;//Label6不可见
                    this.MasterPC_TabControl_Curve_Label7_Value.Visible = false;//Label7不可见
                    this.MasterPC_TabControl_Curve_Label8_Value.Visible = false;//Label8不可见

                    this.MasterPC_TabControl_Curve_Label_CurveNum.Visible = false;//Label_CurveNum不可见
                    this.MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Visible = false;//NumberUpDown不可见
                    this.MasterPC_TabControl_Curve_Button_CurveClear.Visible = false;//Button_CurveClear不可见

                    //CCD曲线
                    this.MasterPC_TabPage_Curve_GropBox_CCD_Function.Visible = true;//GroupBox线性CCD配置可见
                    this.MasterPC_TabPage_Curve_GropBox_CCD_Value.Visible = true;//GroupBox线性CCD数值可见

                    this.MasterPC_TabPage_Curve_Label_CCD_Number.Visible = true;//LabelCCD数量可见
                    this.MasterPC_TabPage_Curve_Label_CCD_Bit.Visible = true;//LabelCCD精度可见
                    this.MasterPC_TabPage_Curve_Label_CCD1_Max.Visible = true;//LabelCCD1最大值可见
                    this.MasterPC_TabPage_Curve_Label_CCD1_Min.Visible = true;//LabelCCD1最小值可见
                    this.MasterPC_TabPage_Curve_Label_CCD2_Max.Visible = true;//LabelCCD2最大值可见
                    this.MasterPC_TabPage_Curve_Label_CCD2_Min.Visible = true;//LabelCCD2最小值可见

                    this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.Visible = true;//ComboBoxCCD数量可见
                    this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.Visible = true;//ComboBoxCCD精度可见

                    this.MasterPC_TabPage_Curve_CheckBox_CCD_Grid.Visible = true;//CheckBox网格可见
                    this.MasterPC_TabPage_Curve_CheckBox_CCD_Point.Visible = true;//CheckBox数据点可见

                    //摄像头图像不可见
                    this.MasterPC_TabPage_Curve_GropBox_Camera_Function.Visible = false;//GroupBox摄像头配置不可见

                    this.MasterPC_TabPage_Curve_ComboBox_Camera_Type.Visible = false;//ComboBox摄像头类型不可见

                    this.MasterPC_TabPage_Curve_TextBox_Camera_Row.Visible = false;//TextBox摄像头行号不可见
                    this.MasterPC_TabPage_Curve_TextBox_Camera_Col.Visible = false;//TextBox摄像头列号不可见

                    this.MasterPC_TabPage_Curve_RadioButton_Camera_Type1.Visible = false;//RadioButton摄像头行号不可见
                    this.MasterPC_TabPage_Curve_RadioButton_Camera_Type2.Visible = false;//RadioButton摄像头列号不可见

                    this.MasterPC_TabPage_Curve_Label_Camera_Type.Visible = false;//Label摄像头类型不可见
                    this.MasterPC_TabPage_Curve_Label_Camera_Row.Visible = false;//Label摄像头行号不可见
                    this.MasterPC_TabPage_Curve_Label_Camera_Col.Visible = false;//Label摄像头列号不可见

                    break;
                case "摄像头"://当前TabPage选中"摄像头"页面
                    TabControl_Curve_SelectedTabNow = "摄像头";
                    //串口曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Visible = false;//CheckBox1不可见
                    this.MasterPC_TabControl_Curve_CheckBox2.Visible = false;//CheckBox2不可见
                    this.MasterPC_TabControl_Curve_CheckBox3.Visible = false;//CheckBox3不可见
                    this.MasterPC_TabControl_Curve_CheckBox4.Visible = false;//CheckBox4不可见
                    this.MasterPC_TabControl_Curve_CheckBox5.Visible = false;//CheckBox5不可见
                    this.MasterPC_TabControl_Curve_CheckBox6.Visible = false;//CheckBox6不可见
                    this.MasterPC_TabControl_Curve_CheckBox7.Visible = false;//CheckBox7不可见
                    this.MasterPC_TabControl_Curve_CheckBox8.Visible = false;//CheckBox8不可见

                    this.MasterPC_TabControl_Curve_Label1_Value.Visible = false;//Label1不可见
                    this.MasterPC_TabControl_Curve_Label2_Value.Visible = false;//Label2不可见
                    this.MasterPC_TabControl_Curve_Label3_Value.Visible = false;//Label3不可见
                    this.MasterPC_TabControl_Curve_Label4_Value.Visible = false;//Label4不可见
                    this.MasterPC_TabControl_Curve_Label5_Value.Visible = false;//Label5不可见
                    this.MasterPC_TabControl_Curve_Label6_Value.Visible = false;//Label6不可见
                    this.MasterPC_TabControl_Curve_Label7_Value.Visible = false;//Label7不可见
                    this.MasterPC_TabControl_Curve_Label8_Value.Visible = false;//Label8不可见

                    this.MasterPC_TabControl_Curve_Label_CurveNum.Visible = false;//Label_CurveNum不可见
                    this.MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Visible = false;//NumberUpDown不可见
                    this.MasterPC_TabControl_Curve_Button_CurveClear.Visible = false;//Button_CurveClear不可见

                    //CCD曲线
                    this.MasterPC_TabPage_Curve_GropBox_CCD_Function.Visible = false;//GroupBox线性CCD配置不可见
                    this.MasterPC_TabPage_Curve_GropBox_CCD_Value.Visible = false;//GroupBox线性CCD数值不可见

                    this.MasterPC_TabPage_Curve_Label_CCD_Number.Visible = false;//LabelCCD数量不可见
                    this.MasterPC_TabPage_Curve_Label_CCD_Bit.Visible = false;//LabelCCD精度不可见
                    this.MasterPC_TabPage_Curve_Label_CCD1_Max.Visible = false;//LabelCCD1最大值不可见
                    this.MasterPC_TabPage_Curve_Label_CCD1_Min.Visible = false;//LabelCCD1最小值不可见
                    this.MasterPC_TabPage_Curve_Label_CCD2_Max.Visible = false;//LabelCCD2最大值不可见
                    this.MasterPC_TabPage_Curve_Label_CCD2_Min.Visible = false;//LabelCCD2最小值不可见

                    this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.Visible = false;//ComboBoxCCD数量不可见
                    this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.Visible = false;//ComboBoxCCD精度不可见

                    this.MasterPC_TabPage_Curve_CheckBox_CCD_Grid.Visible = false;//CheckBox网格不可见
                    this.MasterPC_TabPage_Curve_CheckBox_CCD_Point.Visible = false;//CheckBox数据点不可见

                    //摄像头图像不可见
                    this.MasterPC_TabPage_Curve_GropBox_Camera_Function.Visible = true;//GroupBox摄像头配置可见

                    this.MasterPC_TabPage_Curve_ComboBox_Camera_Type.Visible = true;//ComboBox摄像头类型可见

                    this.MasterPC_TabPage_Curve_TextBox_Camera_Row.Visible = true;//TextBox摄像头行号可见
                    this.MasterPC_TabPage_Curve_TextBox_Camera_Col.Visible = true;//TextBox摄像头列号可见

                    this.MasterPC_TabPage_Curve_RadioButton_Camera_Type1.Visible = true;//RadioButton摄像头行号可见
                    this.MasterPC_TabPage_Curve_RadioButton_Camera_Type2.Visible = true;//RadioButton摄像头列号可见

                    this.MasterPC_TabPage_Curve_Label_Camera_Type.Visible = true;//Label摄像头类型可见
                    this.MasterPC_TabPage_Curve_Label_Camera_Row.Visible = true;//Label摄像头行号可见
                    this.MasterPC_TabPage_Curve_Label_Camera_Col.Visible = true;//Label摄像头列号可见

                    break;
                default:
                    MessageBox.Show("选项卡选择发生错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        #endregion

        #region 串口调试

        #region 检查串口设置
        private bool SerialPort_SettingCheck()//检查串口设置
        {
            //串口名未设置
            if (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Text.Trim() == "")
            {
                return false;
            }
            //串口波特率未设置
            if (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Text.Trim() == "")
            {
                return false;
            }
            //串口数据位未设置
            if (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Text.Trim() == "")
            {
                return false;
            }
            //串口停止位未设置
            if (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Text.Trim() == "")
            {
                return false;
            }
            //串口校验位未设置
            if (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Text.Trim() == "")
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 开关串口
        private void MasterPC_TabPage_SerialPort_Button_SerialPortOpenClose_Click(object sender, EventArgs e)//开关串口
        {
            //当前串口状态获取,串口打开:1,串口关闭:0.
            if (this.SerialPortNow.IsOpen)//串口已打开,单击关闭串口
            {
                //关闭串口
                try
                {
                    SerialPort_Closing_Flag = true;//串口正在关闭
                    SerialPort_OpenClose_Sleep = true;//串口线程睡眠
                    while (SerialPort_Listening_Flag) System.Windows.Forms.Application.DoEvents();//等待监听串口状态标志
                    this.SerialPortNow.Close();//关闭串口
                    this.MasterPC_TabPage_SerialPort_Button_SerialPortOpenClose.Text = "打开串口";//SerialPort串口开关Button显示打开串口
                    SerialPort_Closing_Flag = false;//串口已经关闭

                    //串口设置按钮打开
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Enabled = true;  //SerialPort串口号ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Enabled = true;       //SerialPort波特率ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Enabled = true;   //SerialPort数据位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Enabled = true;    //SerialPort停止位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Enabled = true; //SerialPort校验位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_Receive_RadioButton_Hex.Enabled = true;        //SerialPort接收Hex RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Receive_RadioButton_String.Enabled = true;     //SerialPort接收String RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Send_RadioButton_Hex.Enabled = true;             //SerialPort发送Hex RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Send_RadioButton_String.Enabled = true;          //SerialPort发送String RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Button_SerialPortRefresh.Enabled = true;          //SerialPort串口刷新Button使能
                    this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Enabled = false;               //SerialPort定时发送CheckBox禁止使能
                    this.MasterPC_TabPage_SerialPort_CheckBox_ReceiveNewLine.Enabled = false;      //SerialPort换行接收CheckBox禁止使能
                    this.MasterPC_TabPage_SerialPort_Button_Send.Enabled = false;                              //SerialPort串口开关Button禁止使能

                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Enabled = true;         //Curve串口号ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Enabled = true;              //Curve波特率ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Enabled = true;          //Curve数据位ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Enabled = true;           //Curve停止位ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Enabled = true;        //Curve校验位ComboBox使能
                    this.MasterPC_TabPage_Curve_Button_SerialPortRefresh.Enabled = true;                //Curve串口刷新Button使能
                    this.MasterPC_TabPage_Curve_Button_SerialPortOpenClose.Enabled = true;           //Curve串口开关Button使能
                    this.MasterPC_TabControl_Curve_Button_Export.Enabled = true;                              //Curve导出数据Button使能
                    

                    //关闭定时器
                    this.Timer_Timing_RefreshSerialPort.Stop();//停止刷新
                    this.Timer_Timing_RefreshSerialPort.Enabled = false;//禁止定时器

                    this.Timer_Timing_ReceiveSpeed.Stop();//SerialPort
                    this.Timer_Timing_ReceiveSpeed.Enabled = false;//禁止定时器

                    //串口提示信息
                    this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口已关闭!";//SerialPort
                }
                //异常处理
                catch (Exception)
                {
                    //串口提示信息
                    this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "关闭串口时发生错误!";//SerialPort

                    //关闭串口失败
                    MessageBox.Show("关闭串口时发生错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else//串口已关闭,单击打开串口
            {
                if (this.MasterPC_TabPage_SerialPort_Button_SerialPortOpenClose.Text == "关闭串口")//串口发送数据时断开
                {
                    //刷新串口
                    //串口号配置
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Items.Clear();//清空列表中的项目
                    SerialPortName = SerialPort.GetPortNames();//检测串口

                    if (SerialPortName.Length != 0)//检测到COM口
                    {
                        foreach (string SerialPort_n in System.IO.Ports.SerialPort.GetPortNames())
                        {
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Items.Add(SerialPort_n);//添加COM口
                        }
                        this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.SelectedIndex = 0;//默认选择第一个COM口
                    }
                    else//未检测到COM口
                    {
                        this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.SelectedIndex = -1;//默认为COM口为空
                    }

                    //刷新串口设置
                    this.MasterPC_TabPage_SerialPort_Button_SerialPortOpenClose.Text = "打开串口";//SerialPort串口开关Button显示打开串口

                    //串口设置按钮打开
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Enabled = true;  //SerialPort串口号ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Enabled = true;       //SerialPort波特率ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Enabled = true;   //SerialPort数据位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Enabled = true;    //SerialPort停止位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Enabled = true; //SerialPort校验位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_Receive_RadioButton_Hex.Enabled = true;        //SerialPort接收Hex RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Receive_RadioButton_String.Enabled = true;     //SerialPort接收String RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Send_RadioButton_Hex.Enabled = true;             //SerialPort发送Hex RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Send_RadioButton_String.Enabled = true;          //SerialPort发送String RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Button_SerialPortRefresh.Enabled = true;          //SerialPort串口刷新Button使能
                    this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Enabled = false;               //SerialPort定时发送CheckBox禁止使能
                    this.MasterPC_TabPage_SerialPort_CheckBox_ReceiveNewLine.Enabled = false;      //SerialPort换行接收CheckBox禁止使能
                    this.MasterPC_TabPage_SerialPort_Button_Send.Enabled = false;                              //SerialPort串口开关Button禁止使能

                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Enabled = true;         //Curve串口号ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Enabled = true;              //Curve波特率ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Enabled = true;          //Curve数据位ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Enabled = true;           //Curve停止位ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Enabled = true;        //Curve校验位ComboBox使能
                    this.MasterPC_TabPage_Curve_Button_SerialPortRefresh.Enabled = true;                 //Curve串口刷新Button使能
                    this.MasterPC_TabPage_Curve_Button_SerialPortOpenClose.Enabled = true;           //Curve串口开关Button使能
                    this.MasterPC_TabControl_Curve_Button_Export.Enabled = true;                              //Curve导出数据Button使能

                    //关闭定时器
                    this.Timer_Timing_RefreshSerialPort.Stop();//停止刷新
                    this.Timer_Timing_RefreshSerialPort.Enabled = false;//禁止定时器

                    this.Timer_Timing_ReceiveSpeed.Stop();//SerialPort
                    this.Timer_Timing_ReceiveSpeed.Enabled = false;//禁止定时器

                    //串口提示信息
                    this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口已关闭!";//SerialPort
                }
                else//串口已关闭,单击打开串口
                {
                    if (!SerialPort_SettingCheck())//串口未设置成功
                    {
                        //串口提示信息
                        this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口未设置!";//SerialPort

                        //串口未设置
                        MessageBox.Show("串口未设置,请检查串口设置!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else//串口设置成功
                    {
                        //设置串口相关数据
                        //设置串口名称
                        this.SerialPortNow.PortName = this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Text;
                        //设置串口波特率
                        switch (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Text)
                        {
                            case "9600":
                                this.SerialPortNow.BaudRate = 9600;//串口波特率9600
                                break;
                            case "14400":
                                this.SerialPortNow.BaudRate = 14400;//串口波特率14400
                                break;
                            case "19200":
                                this.SerialPortNow.BaudRate = 19200;//串口波特率19200
                                break;
                            case "38400":
                                this.SerialPortNow.BaudRate = 38400;//串口波特率38400
                                break;
                            case "57600":
                                this.SerialPortNow.BaudRate = 57600;//串口波特率57600
                                break;
                            case "115200":
                                this.SerialPortNow.BaudRate = 115200;//串口波特率115200
                                break;
                            case "128000":
                                this.SerialPortNow.BaudRate = 128000;//串口波特率128000
                                break;
                            case "256000":
                                this.SerialPortNow.BaudRate = 256000;//串口波特率256000
                                break;
                            default:
                                this.SerialPortNow.BaudRate = 115200;//串口波特率115200
                                break;
                        }
                        //设置串口数据位
                        switch (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Text)
                        {
                            case "5":
                                this.SerialPortNow.DataBits = 5;//串口数据位5Bit
                                break;
                            case "6":
                                this.SerialPortNow.DataBits = 6;//串口数据位6Bit
                                break;
                            case "7":
                                this.SerialPortNow.DataBits = 7;//串口数据位7Bit
                                break;
                            case "8":
                                this.SerialPortNow.DataBits = 8;//串口数据位8Bit
                                break;
                            default:
                                this.SerialPortNow.DataBits = 8;//串口数据位8Bit
                                break;
                        }
                        //设置串口停止位
                        switch (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Text)
                        {
                            case "1":
                                this.SerialPortNow.StopBits = StopBits.One;//串口停止位1Bit
                                break;
                            case "2":
                                this.SerialPortNow.StopBits = StopBits.Two;//串口停止位2Bit
                                break;
                            default:
                                this.SerialPortNow.StopBits = StopBits.One;//串口停止位1Bit
                                break;
                        }
                        //设置校验位
                        switch (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Text)
                        {
                            case "无校验":
                                this.SerialPortNow.Parity = Parity.None;//串口校验位(无校验)
                                break;
                            case "奇校验":
                                this.SerialPortNow.Parity = Parity.Even;//串口校验位(奇校验)
                                break;
                            case "偶校验":
                                this.SerialPortNow.Parity = Parity.Odd;//串口校验位(偶校验)
                                break;
                            default:
                                this.SerialPortNow.Parity = Parity.None;//串口校验位(无校验)
                                break;
                        }
                        //设置SerialPort串口
                        this.SerialPortNow.ReadTimeout = -1;//设置超出读取时间
                        this.SerialPortNow.RtsEnable = true;

                        //打开串口
                        try
                        {
                            //打开串口
                            SerialPort_OpenClose_Sleep = true;//串口线程睡眠
                            this.SerialPortNow.Open();//打开串口
                            this.MasterPC_TabPage_SerialPort_Button_SerialPortOpenClose.Text = "关闭串口";//SerialPort

                            //串口设置按钮关闭
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Enabled = false;     //SerialPort串口号ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Enabled = false;          //SerialPort波特率ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Enabled = false;      //SerialPort数据位ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Enabled = false;       //SerialPort停止位ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Enabled = false;    //SerialPort校验位ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_Receive_RadioButton_Hex.Enabled = false;          //SerialPort接收Hex RadioButton禁止使能
                            this.MasterPC_TabPage_SerialPort_Receive_RadioButton_String.Enabled = false;       //SerialPort接收String RadioButton禁止使能
                            this.MasterPC_TabPage_SerialPort_Send_RadioButton_Hex.Enabled = false;               //SerialPort发送Hex RadioButton禁止使能
                            this.MasterPC_TabPage_SerialPort_Send_RadioButton_String.Enabled = false;            //SerialPort发送String RadioButton禁止使能
                            this.MasterPC_TabPage_SerialPort_Button_SerialPortRefresh.Enabled = false;            //SerialPort串口刷新Button禁止使能
                            this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Enabled = true;                   //SerialPort定时发送CheckBox使能
                            this.MasterPC_TabPage_SerialPort_CheckBox_ReceiveNewLine.Enabled = true;      //SerialPort换行接收CheckBox使能
                            this.MasterPC_TabPage_SerialPort_Button_Send.Enabled = true;                                  //SerialPort串口开关Button使能

                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Enabled = false;           //Curve串口号ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Enabled = false;                //Curve波特率ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Enabled = false;            //Curve数据位ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Enabled = false;             //Curve停止位ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Enabled = false;          //Curve校验位ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_Button_SerialPortRefresh.Enabled = false;                   //Curve串口刷新Button禁止使能
                            this.MasterPC_TabPage_Curve_Button_SerialPortOpenClose.Enabled = false;               //Curve串口开关Button禁止使能
                            this.MasterPC_TabControl_Curve_Button_Export.Enabled = false;                                 //Curve导出数据Button禁止使能

                            //打开定时器
                            this.Timer_Timing_RefreshSerialPort.Enabled = true;//打开定时器
                            this.Timer_Timing_RefreshSerialPort.Start();//开始刷新

                            this.Timer_Timing_ReceiveSpeed.Enabled = true;//使能定时器
                            this.Timer_Timing_ReceiveSpeed.Start();//SerialPort

                            //串口提示信息
                            this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口已打开!";//SerialPort
                        }
                        //异常处理
                        catch (Exception)
                        {
                            //串口提示信息
                            this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口无效或已被占用!";//SerialPort

                            //打开串口失败
                            MessageBox.Show("串口无效或已被占用!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                }
            }
        }

        #endregion

        #region 刷新串口
        private void MasterPC_TabPage_SerialPort_Button_SerialPortRefresh_Click(object sender, EventArgs e)//刷新串口
        {
            //当前串口状态获取,串口打开:1,串口关闭:0.
            if (!this.SerialPortNow.IsOpen)//串口关闭时才可以刷新串口
            {
                //串口号配置
                this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Items.Clear();//清空列表中的项目
                SerialPortName = SerialPort.GetPortNames();//检测串口

                if (SerialPortName.Length != 0)//检测到串口
                {
                    foreach (string SerialPort_n in System.IO.Ports.SerialPort.GetPortNames())
                    {
                        this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Items.Add(SerialPort_n);//添加串口
                    }
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.SelectedIndex = 0;//默认选择第一个串口
                }
                else//未检测到串口
                {
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.SelectedIndex = -1;//默认为串口为空
                }

                //串口信息提示
                if (this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.SelectedIndex == 0)//检测到串口
                {
                    this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "已检测到串口!";
                    this.MasterPC_TabPage_SerialPort_TextBox_Information_SPNumber.Text = this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Text;
                    this.MasterPC_TabPage_SerialPort_TextBox_Information_SPBaud.Text = this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Text;
                }
                else//未检测到串口
                {
                    this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "未检测到串口,请刷新重试!";
                    this.MasterPC_TabPage_SerialPort_TextBox_Information_SPNumber.Text = "";
                    this.MasterPC_TabPage_SerialPort_TextBox_Information_SPBaud.Text = "";
                }
            }
        }

        #endregion

        #region 收发缓冲区清空
        private void MasterPC_TabPage_SerialPort_Button_ReceiveClear_Click(object sender, EventArgs e)//清除接收缓冲区
        {
            this.MasterPC_TabPage_SerialPort_TextBox_Receive.Clear();//清除接收缓冲区文本框
            this.MasterPC_TabPage_SerialPort_Label_Information_Receive.Text = "已接收:0";//接收数据清零
            SerialPort_ReceiveData_Count = 0;//接收数据量累加清零
        }

        private void MasterPC_TabPage_SerialPort_Button_SendClear_Click(object sender, EventArgs e)//清除发送缓冲区
        {
            this.MasterPC_TabPage_SerialPort_TextBox_Send.Clear();//清除发送缓冲区文本框
            this.MasterPC_TabPage_SerialPort_Label_Information_Send.Text = "已发送:0";//发送数据清零
            SerialPort_SendData_Count = 0;//发送数据量累加清零
        }

        #endregion

        #region 收发缓冲区滚动条设置
        private void MasterPC_TabPage_SerialPort_TextBox_Receive_TextChanged(object sender, EventArgs e)//接收TextBox滚动条设置
        {
            this.MasterPC_TabPage_SerialPort_TextBox_Receive.SelectionStart = this.MasterPC_TabPage_SerialPort_TextBox_Receive.Text.Length;//TextBox起始点为文本框长度
            this.MasterPC_TabPage_SerialPort_TextBox_Receive.SelectionLength = 0;//文本框的选定字符
            this.MasterPC_TabPage_SerialPort_TextBox_Receive.ScrollToCaret();//当前内容滚动到插入符号位置
        }

        private void MasterPC_TabPage_SerialPort_TextBox_Send_TextChanged(object sender, EventArgs e)//发送TextBox滚动条设置
        {
            this.MasterPC_TabPage_SerialPort_TextBox_Send.SelectionStart = this.MasterPC_TabPage_SerialPort_TextBox_Send.Text.Length;//TextBox起始点为文本框长度
            this.MasterPC_TabPage_SerialPort_TextBox_Send.SelectionLength = 0;//文本框的选定字符
            this.MasterPC_TabPage_SerialPort_TextBox_Send.ScrollToCaret();//当前内容滚动到插入符号位置
        }

        #endregion

        #region 串口数据接收发送
        private void SerialPortNow_DataReceived(object sender, SerialDataReceivedEventArgs e)//串口数据接收
        {
            //当前串口状态获取,串口打开:1,串口关闭:0.
            if (this.SerialPortNow.IsOpen)//如果串口打开
            {
                //如果正在关闭,忽略操作
                if (SerialPort_Closing_Flag)
                {
                    Thread.Sleep(100);//睡眠等待
                    SerialPort_OpenClose_Sleep = false;//设置标志
                    return;
                }

                //关闭串口
                try
                {
                    SerialPort_Listening_Flag = true;//设置标志

                    //串口线程睡眠等待
                    if (SerialPort_OpenClose_Sleep == true)//串口线程睡眠等待UI刷新
                    {
                        Thread.Sleep(100);//睡眠等待
                        SerialPort_OpenClose_Sleep = false;//设置标志
                    }

                    Byte[] ReceiveDataArray = new Byte[this.SerialPortNow.BytesToRead];    //ReceiveDataArray数组表示COM口接受的数据量
                    SerialPort_ReceiveData_Count += this.SerialPortNow.BytesToRead;//接收数据累加
                    this.SerialPortNow.Read(ReceiveDataArray, 0, ReceiveDataArray.Length);//读取数据到ReceiveDataArray数组
                    this.SerialPortNow.DiscardInBuffer();//清空SerialPort数据缓冲Buff

                    if (this.MasterPC_TabPage_SerialPort_Receive_RadioButton_String.Checked)//RadioButton选中接收ASCII字符
                    {
                        SerialPort_ReceiveData_Finish = false;//数据接收处理未完成
                        ReceiveDataString = null;
                        for (int i = 0; i < ReceiveDataArray.Length; i++)
                        {
                            ReceiveDataString += Convert.ToChar(ReceiveDataArray[i]);//接收到的字符数据
                        }
                        ReceiveDataString += " ";//字符串结尾加空格
                        SerialPort_ReceiveData_Finish = true;//数据接收处理完成
                    }
                    else//RadioButton选中接收16进制字符
                    {
                        SerialPort_ReceiveData_Finish = false;//数据接收处理未完成
                        ReceiveDataString = null;
                        for (int i = 0; i < ReceiveDataArray.Length; i++)
                        {
                            ReceiveDataString += ("0x" + ReceiveDataArray[i].ToString("X2") + " ");//将接收到的字符数据转换成16进制数据(形式0xAA)
                        }
                        SerialPort_ReceiveData_Finish = true;//数据接收处理完成
                    }
                }
                //异常处理
                catch (Exception)
                {
                    //串口信息提示
                    this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口传输出现异常!";
                    MessageBox.Show("串口传输出现异常!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //串口接收结束
                finally
                {
                    SerialPort_Listening_Flag = false;//设置标志
                    SerialPort_OpenClose_Sleep = false;//设置标志
                }
            }
            else//如果串口关闭
            {
                //串口信息提示
                this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口未打开!";
                MessageBox.Show("串口未打开!", "警告",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
        }
        private void Timer_Timing_RefreshSerialPort_Tick(object sender, EventArgs e)//定时刷新串口数据
        {
            //刷新UI界面
            if (SerialPort_ReceiveData_Finish)//串口接收数据完成
            {
                this.Invoke((EventHandler)(delegate
                {
                    if (this.MasterPC_TabPage_SerialPort_CheckBox_ReceiveNewLine.Checked)//接收换行CheckBox选中
                    {
                        if (ReceiveDataString != null)//字符串不为空,AppendText方法接收
                        {
                            this.MasterPC_TabPage_SerialPort_TextBox_Receive.AppendText(ReceiveDataString + "\n");//TextBox接收字符串(在上次字符串尾部添加)
                        }
                    }
                    else//接收换行CheckBox未选中
                    {
                        if (ReceiveDataString != null)//字符串不为空,AppendText方法接收
                        {
                            this.MasterPC_TabPage_SerialPort_TextBox_Receive.AppendText(ReceiveDataString);//TextBox接收字符串(在上次字符串尾部添加)
                        }
                    }

                    //接收数据限幅
                    if (SerialPort_ReceiveData_Count > 32767)//如果接收数据量超过32767
                    {
                        SerialPort_ReceiveData_Count = 32767;//保持接收数据为32767
                    }
                    this.MasterPC_TabPage_SerialPort_Label_Information_Receive.Text = "已接收:" + SerialPort_ReceiveData_Count.ToString();//显示已接收字符串数量
                }
                ));
                SerialPort_ReceiveData_Finish = false;//等待串口接收
            }
        }
        private void MasterPC_TabPage_SerialPort_Button_Send_Click(object sender, EventArgs e)//单击发送Button
        {
            //当前串口状态获取,串口打开:1,串口关闭:0.
            if (this.SerialPortNow.IsOpen)//如果串口打开
            {
                //串口发送数据
                try
                {
                    if (this.MasterPC_TabPage_SerialPort_Send_RadioButton_String.Checked)//RadioButton选中发送ASCII字符
                    {
                        this.SerialPortNow.Write(this.MasterPC_TabPage_SerialPort_TextBox_Send.Text);//串口发送文本
                    }
                    else//RadioButton选中发送Hex字符
                    {
                        char[] SendDataArray = this.MasterPC_TabPage_SerialPort_TextBox_Send.Text.ToCharArray();//发送字符串转换为文本
                        foreach (byte i in SendDataArray)
                        {
                            this.SerialPortNow.Write(i.ToString("X2"));//发送字符串为Hex形式
                        }
                    }
                    SerialPort_SendData_Count += this.MasterPC_TabPage_SerialPort_TextBox_Send.Text.Length;//已发送数据长度
                    this.MasterPC_TabPage_SerialPort_Label_Information_Send.Text = "已发送:" + SerialPort_SendData_Count.ToString();//已发送数据
                    
                    //发送数据限幅
                    if (SerialPort_SendData_Count > 32767)//如果发送的数据量超过32767
                    {
                        SerialPort_SendData_Count = 32767;//保持发送的数据量为32767
                    }
                }
                //发送数据失败
                catch (Exception)
                {
                    //串口信息提示
                    this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口发送数据时出现错误!";
                    MessageBox.Show("串口发送数据时出现错误!", "错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else//如果串口关闭
            {
                //串口信息提示
                this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口未打开!";
                MessageBox.Show("串口未打开!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void MasterPC_TabPage_SerialPort_CheckBox_TimerSend_CheckedChanged(object sender, EventArgs e)//串口定时发送选项
        {
            if (this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Checked)//串口定时发送选中
            {
                this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Enabled = false;//禁止NumberUpDown
                this.Timer_Timing_Send.Interval = Convert.ToInt16(this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Value);//发送间隔
                this.Timer_Timing_Send.Enabled = true;//启用Timer
                this.Timer_Timing_Send.Start();//开始计时
            }
            else//串口定时发送取消
            {
                this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Enabled = true;//启用NumberUpDown
                this.Timer_Timing_Send.Stop();//停止计时
                this.Timer_Timing_Send.Enabled = false;//关闭Timer
            }
        }
        private void Timer_Timing_Send_Tick(object sender, EventArgs e)//串口定时发送
        {
            //当前串口状态获取,串口打开:1,串口关闭:0.
            if (this.SerialPortNow.IsOpen)//如果串口打开
            {
                //如果当前准备关闭串口,不发送数据,直接返回
                if (SerialPort_Closing_Flag)
                {
                    this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Checked = false;//CheckBox取消选中
                    this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Enabled = true;//启用NumberUpDown
                    this.Timer_Timing_Send.Stop();//停止计时
                    this.Timer_Timing_Send.Enabled = false;//关闭Timer
                    return;
                }

                //串口发送数据
                try
                {
                    if (this.MasterPC_TabPage_SerialPort_Send_RadioButton_String.Checked)//RadioButton选中发送ASCII字符
                    {
                        this.SerialPortNow.Write(this.MasterPC_TabPage_SerialPort_TextBox_Send.Text);//串口发送文本
                    }
                    else//RadioButton选中发送Hex字符
                    {
                        char[] SendDataArray = this.MasterPC_TabPage_SerialPort_TextBox_Send.Text.ToCharArray();//发送字符串转换为文本
                        foreach (byte i in SendDataArray)
                        {
                            this.SerialPortNow.Write(i.ToString("X2"));//发送字符串为Hex形式
                        }
                    }
                    SerialPort_SendData_Count += this.MasterPC_TabPage_SerialPort_TextBox_Send.Text.Length;//已发送数据长度
                    this.MasterPC_TabPage_SerialPort_Label_Information_Send.Text = "已发送:" + SerialPort_SendData_Count.ToString();//已发送数据
                    //发送数据限幅
                    if (SerialPort_SendData_Count > 32767)//如果发送的数据量超过32767
                    {
                        SerialPort_SendData_Count = 32767;//保持发送的数据量为32767
                    }
                }
                //发送数据失败
                catch (Exception)
                {
                    //串口信息提示
                    this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Checked = false;//CheckBox取消选中
                    this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Enabled = true;//启用NumberUpDown
                    this.Timer_Timing_Send.Stop();//停止计时
                    this.Timer_Timing_Send.Enabled = false;//关闭Timer
                    this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口发送数据时出现错误!";
                    MessageBox.Show("串口发送数据时出现错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else//如果串口关闭
            {
                //串口信息提示
                this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Checked = false;//CheckBox取消选中
                this.MasterPC_TabPage_SerialPort_NumberUpDown_Time.Enabled = true;//启用NumberUpDown
                this.Timer_Timing_Send.Stop();//停止计时
                this.Timer_Timing_Send.Enabled = false;//关闭Timer
                this.MasterPC_TabPage_SerialPort_TextBox_Information.Text = "串口未打开!";
                MessageBox.Show("串口未打开!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void Timer_Timing_ReceiveSpeed_Tick(object sender, EventArgs e)//接收数据速度
        {
            long SerialPort_ReceiveSpeed_Now = 0;//当前接收速度
            SerialPort_ReceiveSpeed_Now = SerialPort_ReceiveData_Count - SerialPort_ReceiveData_Last;//计算1s内的传输速度
            if (SerialPort_ReceiveSpeed_Now < 0)//限制最小速度为0
            {
                SerialPort_ReceiveSpeed_Now = 0;
            }
            this.MasterPC_TabPage_SerialPort_Label_ReceiveSpeed.Text = "速度:" + SerialPort_ReceiveSpeed_Now.ToString() + "字节/秒";
            SerialPort_ReceiveData_Last = SerialPort_ReceiveData_Count;
        }

        #endregion

        #endregion

        #region 实时曲线

        #region 串口曲线ZedGraph初始化
        private void MasterPC_TabControl_Curve_ZedGraph_Init()//串口曲线ZedGraph初始化
        {
            //ZedGraph坐标轴标题标签初始化
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.Title.Text = "串口曲线";
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.XAxis.Title.Text = "时间";
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.YAxis.Title.Text = "数值";
            //this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.XAxis.MajorGrid.IsVisible = true;//X轴虚线
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.YAxis.MajorGrid.IsVisible = true;//Y轴虚线
            //this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.Chart.Border.IsVisible = false;

            //ZedGraph坐标轴值设置初始化
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.XAxis.Scale.Min = 1;  //X轴坐标最小值
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.XAxis.Scale.Max = 2; //X轴坐标最大值
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.YAxis.Scale.Min = 0;  //Y轴坐标最小值
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.YAxis.Scale.Max = 1; //Y轴坐标最大值

            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.XAxis.Scale.MinAuto = true;
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.XAxis.Scale.MaxAuto = true;
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.YAxis.Scale.MinAuto = true;
            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.YAxis.Scale.MaxAuto = true;

            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.XAxis.Type = ZedGraph.AxisType.Ordinal;

            this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.CurveList.Clear();//清空曲线

            //ZedGraph创建曲线
            ZedGraph_Curve_1 = this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.AddCurve("曲线1", ZedGraph_List_1, Color.LightCoral, SymbolType.None);
            ZedGraph_Curve_2 = this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.AddCurve("曲线2", ZedGraph_List_2, Color.Teal, SymbolType.None);
            ZedGraph_Curve_3 = this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.AddCurve("曲线3", ZedGraph_List_3, Color.Orange, SymbolType.None);
            ZedGraph_Curve_4 = this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.AddCurve("曲线4", ZedGraph_List_4, Color.LawnGreen, SymbolType.None);
            ZedGraph_Curve_5 = this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.AddCurve("曲线5", ZedGraph_List_5, Color.LightSeaGreen, SymbolType.None);
            ZedGraph_Curve_6 = this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.AddCurve("曲线6", ZedGraph_List_6, Color.LightSkyBlue, SymbolType.None);
            ZedGraph_Curve_7 = this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.AddCurve("曲线7", ZedGraph_List_7, Color.RoyalBlue, SymbolType.None);
            ZedGraph_Curve_8 = this.MasterPC_TabControl_Curve_ZedGraph.GraphPane.AddCurve("曲线8", ZedGraph_List_8, Color.BlueViolet, SymbolType.None);

            //ZedGraph曲线样式设置
            ZedGraph_Curve_1.Line.Width = 2;
            ZedGraph_Curve_2.Line.Width = 2;
            ZedGraph_Curve_3.Line.Width = 2;
            ZedGraph_Curve_4.Line.Width = 2;
            ZedGraph_Curve_5.Line.Width = 2;
            ZedGraph_Curve_6.Line.Width = 2;
            ZedGraph_Curve_7.Line.Width = 2;
            ZedGraph_Curve_8.Line.Width = 2;

            this.MasterPC_TabControl_Curve_ZedGraph.Refresh();//刷新曲线
        }

        #endregion

        #region 检查串口设置
        private bool SerialPort_Curve_SettingCheck()//检查串口设置
        {
            //串口名未设置
            if (this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Text.Trim() == "")
            {
                return false;
            }
            //串口波特率未设置
            if (this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Text.Trim() == "")
            {
                return false;
            }
            //串口数据位未设置
            if (this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Text.Trim() == "")
            {
                return false;
            }
            //串口停止位未设置
            if (this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Text.Trim() == "")
            {
                return false;
            }
            //串口校验位未设置
            if (this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Text.Trim() == "")
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 开关串口
        private void MasterPC_TabPage_Curve_Button_SerialPortOpenClose_Click(object sender, EventArgs e)//开关串口
        {
            //当前串口状态获取,串口打开:1,串口关闭:0.
            if (this.SerialPortCurve.IsOpen)//串口已打开,单击关闭串口
            {
                //关闭串口
                try
                {
                    SerialPort_Closing_Flag = true;//串口正在关闭
                    SerialPort_OpenClose_Sleep = true;//串口线程睡眠等待UI更新
                    while (SerialPort_Listening_Flag) System.Windows.Forms.Application.DoEvents();//等待监听串口状态标志
                    this.SerialPortCurve.Close();//关闭串口
                    this.MasterPC_TabPage_Curve_Button_SerialPortOpenClose.Text = "打开串口";//Curve串口开关Button显示打开串口
                    SerialPort_Closing_Flag = false;//串口已经关闭

                    //串口设置按钮打开
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Enabled = true;         //Curve串口号ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Enabled = true;              //Curve波特率ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Enabled = true;          //Curve数据位ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Enabled = true;           //Curve停止位ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Enabled = true;        //Curve校验位ComboBox使能
                    this.MasterPC_TabPage_Curve_Button_SerialPortRefresh.Enabled = true;                 //Curve串口刷新Button使能
                    this.MasterPC_TabControl_Curve_Button_Export.Enabled = true;                               //Curve导出数据Button使能

                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Enabled = true;  //SerialPort串口号ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Enabled = true;       //SerialPort波特率ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Enabled = true;   //SerialPort数据位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Enabled = true;    //SerialPort停止位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Enabled = true; //SerialPort校验位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_Receive_RadioButton_Hex.Enabled = true;        //SerialPort接收Hex RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Receive_RadioButton_String.Enabled = true;     //SerialPort接收String RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Send_RadioButton_Hex.Enabled = true;             //SerialPort发送Hex RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Send_RadioButton_String.Enabled = true;          //SerialPort发送String RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Button_SerialPortRefresh.Enabled = true;          //SerialPort串口刷新Button使能
                    this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Enabled = false;               //SerialPort定时发送CheckBox禁止使能
                    this.MasterPC_TabPage_SerialPort_CheckBox_ReceiveNewLine.Enabled = false;      //SerialPort换行接收CheckBox禁止使能
                    this.MasterPC_TabPage_SerialPort_Button_Send.Enabled = false;                              //SerialPort串口开关发送Button禁止使能
                    this.MasterPC_TabPage_SerialPort_Button_SerialPortOpenClose.Enabled = true;     //SerialPort串口开关Button使能

                    //定时刷新曲线定时器Timer
                    this.Timer_Timing_RefreshCurve.Stop();//停止定时
                    this.Timer_Timing_RefreshCurve.Enabled = false;//关闭定时刷新

                    //串口提示信息
                    this.MasterPC_TabPage_Curve_TextBox_Information.Text = "串口已关闭!";//Curve
                }
                //异常处理
                catch (Exception)
                {
                    //串口提示信息
                    this.MasterPC_TabPage_Curve_TextBox_Information.Text = "关闭串口时发生错误!";//Curve

                    //关闭串口失败
                    MessageBox.Show("关闭串口时发生错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else//串口已关闭,单击打开串口
            {
                if (this.MasterPC_TabPage_Curve_Button_SerialPortOpenClose.Text == "关闭串口")//串口发送数据时断开
                {
                    //刷新串口
                    //串口号配置
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Items.Clear();//清空列表中的项目
                    SerialPortName_Curve = SerialPort.GetPortNames();//检测串口

                    if (SerialPortName_Curve.Length != 0)//检测到COM口
                    {
                        foreach (string SerialPort_n in System.IO.Ports.SerialPort.GetPortNames())
                        {
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Items.Add(SerialPort_n);//添加COM口
                        }
                        this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.SelectedIndex = 0;//默认选择第一个COM口
                    }
                    else//未检测到COM口
                    {
                        this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.SelectedIndex = -1;//默认为COM口为空
                    }

                    //刷新串口设置
                    this.MasterPC_TabPage_Curve_Button_SerialPortOpenClose.Text = "打开串口";//Curve串口开关Button显示打开串口

                    //串口设置按钮打开
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Enabled = true;         //Curve串口号ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Enabled = true;              //Curve波特率ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Enabled = true;          //Curve数据位ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Enabled = true;           //Curve停止位ComboBox使能
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Enabled = true;        //Curve校验位ComboBox使能
                    this.MasterPC_TabPage_Curve_Button_SerialPortRefresh.Enabled = true;                 //Curve串口刷新Button使能
                    this.MasterPC_TabControl_Curve_Button_Export.Enabled = true;                               //Curve导出数据Button使能

                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Enabled = true;  //SerialPort串口号ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Enabled = true;       //SerialPort波特率ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Enabled = true;   //SerialPort数据位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Enabled = true;    //SerialPort停止位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Enabled = true; //SerialPort校验位ComboBox使能
                    this.MasterPC_TabPage_SerialPort_Receive_RadioButton_Hex.Enabled = true;        //SerialPort接收Hex RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Receive_RadioButton_String.Enabled = true;     //SerialPort接收String RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Send_RadioButton_Hex.Enabled = true;             //SerialPort发送Hex RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Send_RadioButton_String.Enabled = true;          //SerialPort发送String RadioButton使能
                    this.MasterPC_TabPage_SerialPort_Button_SerialPortRefresh.Enabled = true;          //SerialPort串口刷新Button使能
                    this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Enabled = false;               //SerialPort定时发送CheckBox禁止使能
                    this.MasterPC_TabPage_SerialPort_CheckBox_ReceiveNewLine.Enabled = false;      //SerialPort换行接收CheckBox禁止使能
                    this.MasterPC_TabPage_SerialPort_Button_Send.Enabled = false;                              //SerialPort串口开关发送Button禁止使能
                    this.MasterPC_TabPage_SerialPort_Button_SerialPortOpenClose.Enabled = true;     //SerialPort串口开关Button使能

                    //定时刷新曲线定时器Timer
                    this.Timer_Timing_RefreshCurve.Stop();//停止定时
                    this.Timer_Timing_RefreshCurve.Enabled = false;//关闭定时刷新

                    //串口提示信息
                    this.MasterPC_TabPage_Curve_TextBox_Information.Text = "串口已关闭!";//Curve
                }
                else//串口已关闭,单击打开串口
                {
                    if (!SerialPort_Curve_SettingCheck())//串口未设置成功
                    {
                        //串口提示信息
                        this.MasterPC_TabPage_Curve_TextBox_Information.Text = "串口未设置!";//Curve

                        //串口未设置
                        MessageBox.Show("串口未设置,请检查串口设置!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else//串口设置成功
                    {
                        //设置串口相关数据
                        //设置串口名称
                        this.SerialPortCurve.PortName = this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Text;
                        //设置串口波特率
                        switch (this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Text)
                        {
                            case "9600":
                                this.SerialPortCurve.BaudRate = 9600;//串口波特率9600
                                break;
                            case "14400":
                                this.SerialPortCurve.BaudRate = 14400;//串口波特率14400
                                break;
                            case "19200":
                                this.SerialPortCurve.BaudRate = 19200;//串口波特率19200
                                break;
                            case "38400":
                                this.SerialPortCurve.BaudRate = 38400;//串口波特率38400
                                break;
                            case "57600":
                                this.SerialPortCurve.BaudRate = 57600;//串口波特率57600
                                break;
                            case "115200":
                                this.SerialPortCurve.BaudRate = 115200;//串口波特率115200
                                break;
                            case "128000":
                                this.SerialPortCurve.BaudRate = 128000;//串口波特率128000
                                break;
                            case "256000":
                                this.SerialPortCurve.BaudRate = 256000;//串口波特率256000
                                break;
                            default:
                                this.SerialPortCurve.BaudRate = 115200;//串口波特率115200
                                break;
                        }
                        //设置串口数据位
                        switch (this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Text)
                        {
                            case "5":
                                this.SerialPortCurve.DataBits = 5;//串口数据位5Bit
                                break;
                            case "6":
                                this.SerialPortCurve.DataBits = 6;//串口数据位6Bit
                                break;
                            case "7":
                                this.SerialPortCurve.DataBits = 7;//串口数据位7Bit
                                break;
                            case "8":
                                this.SerialPortCurve.DataBits = 8;//串口数据位8Bit
                                break;
                            default:
                                this.SerialPortCurve.DataBits = 8;//串口数据位8Bit
                                break;
                        }
                        //设置串口停止位
                        switch (this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Text)
                        {
                            case "1":
                                this.SerialPortCurve.StopBits = StopBits.One;//串口停止位1Bit
                                break;
                            case "2":
                                this.SerialPortCurve.StopBits = StopBits.Two;//串口停止位2Bit
                                break;
                            default:
                                this.SerialPortCurve.StopBits = StopBits.One;//串口停止位1Bit
                                break;
                        }
                        //设置校验位
                        switch (this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Text)
                        {
                            case "无校验":
                                this.SerialPortCurve.Parity = Parity.None;//串口校验位(无校验)
                                break;
                            case "奇校验":
                                this.SerialPortCurve.Parity = Parity.Even;//串口校验位(奇校验)
                                break;
                            case "偶校验":
                                this.SerialPortCurve.Parity = Parity.Odd;//串口校验位(偶校验)
                                break;
                            default:
                                this.SerialPortCurve.Parity = Parity.None;//串口校验位(无校验)
                                break;
                        }
                        //设置SerialPort串口
                        this.SerialPortCurve.ReadTimeout = -1;//设置超出读取时间
                        this.SerialPortCurve.RtsEnable = true;

                        //打开串口
                        try
                        {
                            //打开串口
                            SerialPort_OpenClose_Sleep = true;//串口线程睡眠等待UI更新
                            this.SerialPortCurve.Open();//打开串口
                            this.MasterPC_TabPage_Curve_Button_SerialPortOpenClose.Text = "关闭串口";//Curve

                            //串口设置按钮关闭
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Enabled = false;         //Curve串口号ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Enabled = false;              //Curve波特率ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortDataBit.Enabled = false;          //Curve数据位ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortStopBit.Enabled = false;           //Curve停止位ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_ComboBox_SerialPortCheckBit.Enabled = false;        //Curve校验位ComboBox禁止使能
                            this.MasterPC_TabPage_Curve_Button_SerialPortRefresh.Enabled = false;                 //Curve串口刷新Button禁止使能
                            this.MasterPC_TabControl_Curve_Button_Export.Enabled = false;                               //Curve导出数据Button禁止使能

                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortNumber.Enabled = false;  //SerialPort串口号ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortBaud.Enabled = false;       //SerialPort波特率ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortDataBit.Enabled = false;   //SerialPort数据位ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortStopBit.Enabled = false;    //SerialPort停止位ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_ComboBox_SerialPortCheckBit.Enabled = false; //SerialPort校验位ComboBox禁止使能
                            this.MasterPC_TabPage_SerialPort_Receive_RadioButton_Hex.Enabled = false;        //SerialPort接收Hex RadioButton禁止使能
                            this.MasterPC_TabPage_SerialPort_Receive_RadioButton_String.Enabled = false;     //SerialPort接收String RadioButton禁止使能
                            this.MasterPC_TabPage_SerialPort_Send_RadioButton_Hex.Enabled = false;             //SerialPort发送Hex RadioButton禁止使能
                            this.MasterPC_TabPage_SerialPort_Send_RadioButton_String.Enabled = false;          //SerialPort发送String RadioButton禁止使能
                            this.MasterPC_TabPage_SerialPort_Button_SerialPortRefresh.Enabled = false;          //SerialPort串口刷新Button禁止使能
                            this.MasterPC_TabPage_SerialPort_CheckBox_TimerSend.Enabled = false;               //SerialPort定时发送CheckBox使能
                            this.MasterPC_TabPage_SerialPort_CheckBox_ReceiveNewLine.Enabled = false;      //SerialPort换行接收CheckBox禁止使能
                            this.MasterPC_TabPage_SerialPort_Button_Send.Enabled = false;                              //SerialPort串口开关发送Button使能
                            this.MasterPC_TabPage_SerialPort_Button_SerialPortOpenClose.Enabled = false;     //SerialPort串口开关Button禁止使能

                            //定时刷新曲线定时器Timer
                            this.Timer_Timing_RefreshCurve.Enabled = true;//打开定时刷新
                            this.Timer_Timing_RefreshCurve.Start();//开始定时

                            //串口提示信息
                            this.MasterPC_TabPage_Curve_TextBox_Information.Text = "串口已打开!";//Curve
                        }
                        //异常处理
                        catch (Exception)
                        {
                            //串口提示信息
                            this.MasterPC_TabPage_Curve_TextBox_Information.Text = "串口无效或已被占用!";//Curve

                            //打开串口失败
                            MessageBox.Show("串口无效或已被占用!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                }
            }
        }

        #endregion

        #region 刷新串口
        private void MasterPC_TabPage_Curve_Button_SerialPortRefresh_Click(object sender, EventArgs e)//刷新串口
        {
            //当前串口状态获取,串口打开:1,串口关闭:0.
            if (!this.SerialPortCurve.IsOpen)//串口关闭时才可以刷新串口
            {
                //串口号配置
                this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Items.Clear();//清空列表中的项目
                SerialPortName_Curve = SerialPort.GetPortNames();//检测串口

                if (SerialPortName_Curve.Length != 0)//检测到COM口
                {
                    foreach (string SerialPort_n in System.IO.Ports.SerialPort.GetPortNames())
                    {
                        this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Items.Add(SerialPort_n);//添加COM口
                    }
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.SelectedIndex = 0;//默认选择第一个COM口
                }
                else//未检测到COM口
                {
                    this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.SelectedIndex = -1;//默认为COM口为空
                }

                //串口信息提示
                if (this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.SelectedIndex == 0)//检测到串口
                {
                    this.MasterPC_TabPage_Curve_TextBox_Information.Text = "已检测到串口!";
                    this.MasterPC_TabPage_Curve_TextBox_Information_SPNumber.Text = this.MasterPC_TabPage_Curve_ComboBox_SerialPortNumber.Text;
                    this.MasterPC_TabPage_Cruve_TextBox_Information_SPBaud.Text = this.MasterPC_TabPage_Curve_ComboBox_SerialPortBaud.Text;
                }
                else//未检测到串口
                {
                    this.MasterPC_TabPage_Curve_TextBox_Information.Text = "未检测到串口,请刷新重试!";
                    this.MasterPC_TabPage_Curve_TextBox_Information_SPNumber.Text = "";
                    this.MasterPC_TabPage_Cruve_TextBox_Information_SPBaud.Text = "";
                }
            }
        }

        #endregion

        #region 串口数据接收发送
        private void SerialPortCurve_DataReceived(object sender, SerialDataReceivedEventArgs e)//串口数据接收
        {
            //当前串口状态获取,串口打开:1,串口关闭:0.
            if (this.SerialPortCurve.IsOpen)//如果串口打开
            {
                //如果正在关闭,忽略操作
                if (SerialPort_Closing_Flag)
                {
                    Thread.Sleep(100);//睡眠等待
                    SerialPort_OpenClose_Sleep = false;//设置标志
                    return;
                }

                //关闭串口
                try
                {
                    SerialPort_Listening_Flag = true;//设置标志

                    //串口线程睡眠等待
                    if (SerialPort_OpenClose_Sleep == true)//串口线程睡眠等待UI刷新
                    {
                        Thread.Sleep(100);//睡眠等待
                        SerialPort_OpenClose_Sleep = false;//设置标志
                    }

                    int i, j, k;
                    Byte[] ReceiveDataArray = new Byte[this.SerialPortCurve.BytesToRead];    //ReceiveDataArray数组表示COM口接受的数据量
                    this.SerialPortCurve.Read(ReceiveDataArray, 0, ReceiveDataArray.Length);//读取数据到ReceiveDataArray数组
                    this.SerialPortCurve.DiscardInBuffer();//清空SerialPort数据缓冲Buff

                    //串口曲线页面拆分数据
                    if (TabControl_Curve_SelectedTabNow == "串口曲线")
                    {
                        //对串口数据进行拆分
                        for (i = 0; i < ReceiveDataArray.Length - (Int16)MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Value * 2 - 3; i++)//搜索数据格式
                        {
                            //数据格式(0xFF,0x00,...,0xAA,0x55)
                            if (ReceiveDataArray[i] == (byte)(0xFF) && ReceiveDataArray[i + 1] == (byte)(0x00) && ReceiveDataArray[i + (Int16)MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Value * 2 + 2] == (byte)(0xAA) && ReceiveDataArray[i + (Int16)MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Value * 2 + 3] == (byte)(0x55))
                            {
                                for (j = 0, k = i; j < (Int16)MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Value; j++, k += 2)
                                {
                                    SerialPort_Curve_Data[j] = BitConverter.ToInt16(ReceiveDataArray, k + 2);//数据拆分
                                }
                                i += (Int16)MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Value * 2 + 3;
                                SerialPort_DataCheck_Finish = true;//数据拆分完成
                            }
                        }
                    }
                    //线性CCD曲线页面拆分数据
                    else if (TabControl_Curve_SelectedTabNow == "线性CCD")
                    {
                        if (SerialPort_CCD_Check_Finish == true)
                        {
                            //Thread.Sleep(100);//睡眠等待
                            return;
                        }
                        else
                        {
                            Thread.Sleep(100);//睡眠等待

                            //对串口数据进行拆分
                            if (Curve_CCD_Sample_Bit_Flag == 0)//ADC8Bit
                            {
                                if (Curve_CCD_Number_Flag == 0)//CCD:1
                                {
                                    for (i = 0; i < ReceiveDataArray.Length - 1 * 128 - 3; i++)
                                    {
                                        if (ReceiveDataArray[i] == (byte)(0xFF) && ReceiveDataArray[i + 1] == (byte)(0x00) && ReceiveDataArray[i + 1 * 128 + 2] == (byte)(0xAA) && ReceiveDataArray[i + 1 * 128 + 3] == (byte)(0x99))
                                        {
                                            for (j = 0, k = i + 2; j < 1 * 128; j++, k++)
                                            {
                                                ChartLine_List_1[j] = ReceiveDataArray[k];
                                            }
                                            i += 1 * 128 + 3;
                                            SerialPort_CCD_Check_Finish = true;//数据拆分完成
                                            Thread.Sleep(20);//睡眠等待
                                        }
                                    }
                                }
                                else//CCD:2
                                {
                                    for (i = 0; i < ReceiveDataArray.Length - 2 * 128 - 3; i++)
                                    {
                                        if (ReceiveDataArray[i] == (byte)(0xFF) && ReceiveDataArray[i + 1] == (byte)(0x00) && ReceiveDataArray[i + 2 * 128 + 2] == (byte)(0xAA) && ReceiveDataArray[i + 2 * 128 + 3] == (byte)(0x99))
                                        {
                                            for (j = 0, k = i + 2; j < 128; j++, k++)
                                            {
                                                ChartLine_List_1[j] = ReceiveDataArray[k];
                                            }
                                            for (j = 0, k = i + 128 + 2; j < 128; j++, k++)
                                            {
                                                ChartLine_List_2[j] = ReceiveDataArray[k];
                                            }
                                            i += 2 * 128 + 3;
                                            SerialPort_CCD_Check_Finish = true;//数据拆分完成
                                            Thread.Sleep(20);//睡眠等待
                                        }
                                    }
                                }
                            }
                            else//ADC10Bit/ADC12Bit
                            {
                                if (Curve_CCD_Number_Flag == 0)//CCD:1
                                {
                                    for (i = 0; i < ReceiveDataArray.Length - 1 * 256 - 3; i++)
                                    {
                                        if (ReceiveDataArray[i] == (byte)(0xFF) && ReceiveDataArray[i + 1] == (byte)(0x00) && ReceiveDataArray[i + 1 * 256 + 2] == (byte)(0xAA) && ReceiveDataArray[i + 1 * 256 + 3] == (byte)(0x99))
                                        {
                                            for (j = 0, k = i; j < 1 * 128; j++, k += 2)
                                            {
                                                ChartLine_List_1[j] = BitConverter.ToInt16(ReceiveDataArray, k + 2);//数据拆分;
                                            }
                                            i += 1 * 256 + 3;
                                            SerialPort_CCD_Check_Finish = true;//数据拆分完成
                                            Thread.Sleep(20);//睡眠等待
                                        }
                                    }
                                }
                                else //CCD:2
                                {
                                    for (i = 0; i < ReceiveDataArray.Length - 2 * 256 - 3; i++)
                                    {
                                        if (ReceiveDataArray[i] == (byte)(0xFF) && ReceiveDataArray[i + 1] == (byte)(0x00) && ReceiveDataArray[i + 2 * 256 + 2] == (byte)(0xAA) && ReceiveDataArray[i + 2 * 256 + 3] == (byte)(0x99))
                                        {
                                            for (j = 0, k = i; j < 1 * 128; j++, k += 2)
                                            {
                                                ChartLine_List_1[j] = BitConverter.ToInt16(ReceiveDataArray, k + 2);//数据拆分;
                                            }
                                            for (j = 0, k = i + 256 + 2; j < 128; j++, k += 2)
                                            {
                                                ChartLine_List_2[j] = BitConverter.ToInt16(ReceiveDataArray, k + 2);//数据拆分;
                                            }
                                            i += 2 * 256 + 3;
                                            SerialPort_CCD_Check_Finish = true;//数据拆分完成
                                            Thread.Sleep(20);//睡眠等待
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //异常处理
                catch (Exception)
                {
                    //串口信息提示
                    this.MasterPC_TabPage_Curve_TextBox_Information.Text = "串口传输出现异常!";
                    MessageBox.Show("串口传输出现异常!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //串口接收结束
                finally
                {
                    SerialPort_Listening_Flag = false;//设置标志
                    SerialPort_OpenClose_Sleep = false;//设置标志
                }
            }
            else//如果串口关闭
            {
                //串口信息提示
                this.MasterPC_TabPage_Curve_TextBox_Information.Text = "串口未打开!";
                MessageBox.Show("串口未打开!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        #endregion

        #region 定时刷新曲线
        private void Timer_Timing_RefreshCurve_Tick(object sender, EventArgs e)//定时刷新曲线
        {
            if (SerialPort_DataCheck_Finish)//数据拆分完成
            {
                //刷新UI界面
                this.Invoke((EventHandler)(delegate
                    {
                        this.MasterPC_TabControl_Curve_Label1_Value.Text = "数值:" + SerialPort_Curve_Data[0].ToString();//曲线1数值显示
                        this.MasterPC_TabControl_Curve_Label2_Value.Text = "数值:" + SerialPort_Curve_Data[1].ToString();//曲线2数值显示
                        this.MasterPC_TabControl_Curve_Label3_Value.Text = "数值:" + SerialPort_Curve_Data[2].ToString();//曲线3数值显示
                        this.MasterPC_TabControl_Curve_Label4_Value.Text = "数值:" + SerialPort_Curve_Data[3].ToString();//曲线4数值显示
                        this.MasterPC_TabControl_Curve_Label5_Value.Text = "数值:" + SerialPort_Curve_Data[4].ToString();//曲线5数值显示
                        this.MasterPC_TabControl_Curve_Label6_Value.Text = "数值:" + SerialPort_Curve_Data[5].ToString();//曲线6数值显示
                        this.MasterPC_TabControl_Curve_Label7_Value.Text = "数值:" + SerialPort_Curve_Data[6].ToString();//曲线7数值显示
                        this.MasterPC_TabControl_Curve_Label8_Value.Text = "数值:" + SerialPort_Curve_Data[7].ToString();//曲线8数值显示

                        //坐标点数多余300后保持坐标中存在300点
                        if (ZedGraph_List_1.Count >= 300)
                        {
                            ZedGraph_List_1.RemoveAt(0);
                        }
                        if (ZedGraph_List_2.Count >= 300)
                        {
                            ZedGraph_List_2.RemoveAt(0);
                        }
                        if (ZedGraph_List_3.Count >= 300)
                        {
                            ZedGraph_List_3.RemoveAt(0);
                        }
                        if (ZedGraph_List_4.Count >= 300)
                        {
                            ZedGraph_List_4.RemoveAt(0);
                        }
                        if (ZedGraph_List_5.Count >= 300)
                        {
                            ZedGraph_List_5.RemoveAt(0);
                        }
                        if (ZedGraph_List_6.Count >= 300)
                        {
                            ZedGraph_List_6.RemoveAt(0);
                        }
                        if (ZedGraph_List_7.Count >= 300)
                        {
                            ZedGraph_List_7.RemoveAt(0);
                        }
                        if (ZedGraph_List_8.Count >= 300)
                        {
                            ZedGraph_List_8.RemoveAt(0);
                        }

                        ZedGraph_List_1.Add(0, SerialPort_Curve_Data[0]);//ZedGraph_List_1添加数据
                        ZedGraph_List_2.Add(0, SerialPort_Curve_Data[1]);//ZedGraph_List_2添加数据
                        ZedGraph_List_3.Add(0, SerialPort_Curve_Data[2]);//ZedGraph_List_3添加数据
                        ZedGraph_List_4.Add(0, SerialPort_Curve_Data[3]);//ZedGraph_List_4添加数据
                        ZedGraph_List_5.Add(0, SerialPort_Curve_Data[4]);//ZedGraph_List_5添加数据
                        ZedGraph_List_6.Add(0, SerialPort_Curve_Data[5]);//ZedGraph_List_6添加数据
                        ZedGraph_List_7.Add(0, SerialPort_Curve_Data[6]);//ZedGraph_List_7添加数据
                        ZedGraph_List_8.Add(0, SerialPort_Curve_Data[7]);//ZedGraph_List_8添加数据

                        this.MasterPC_TabControl_Curve_ZedGraph.AxisChange();//坐标轴自动适应
                        this.MasterPC_TabControl_Curve_ZedGraph.Invalidate();//重绘控件

                        //接收曲线限幅
                        SerialPort_Curve_ReceiveData_Count +=1;//接收曲线数加一
                        if (SerialPort_Curve_ReceiveData_Count > 32767)
                        {
                            SerialPort_Curve_ReceiveData_Count = 32767;
                        }
                        this.MasterPC_TabPage_Curve_Label_Information_Receive.Text = "已接收数据:" + SerialPort_Curve_ReceiveData_Count.ToString();
                    }
                    ));
                SerialPort_DataCheck_Finish = false;//等待拆分数据
            }
            else if (SerialPort_CCD_Check_Finish)//CCD数据拆分完成
            {
                int CCD_Value_Min = 0;
                int CCD_Value_Max = 0;

                //刷新UI界面
                this.Invoke((EventHandler)(delegate
                {
                    if (Curve_CCD_Number_Flag == 0)//单CCD曲线
                    {
                        this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.Series[0].Points.DataBindY(ChartLine_List_1);

                        CCD_Value_Min = ChartLine_List_1[0];
                        CCD_Value_Max = ChartLine_List_1[0];
                        for (int i = 0; i < 128; i++)
                        {
                            if (ChartLine_List_1[i] < CCD_Value_Min)
                            {
                                CCD_Value_Min = ChartLine_List_1[i];
                            }

                            if (ChartLine_List_1[i] > CCD_Value_Max)
                            {
                                CCD_Value_Max = ChartLine_List_1[i];
                            }
                        }

                        this.MasterPC_TabPage_Curve_Label_CCD1_Min.Text = "CCD1最小值:" + CCD_Value_Min.ToString();
                        this.MasterPC_TabPage_Curve_Label_CCD1_Max.Text = "CCD1最大值:" + CCD_Value_Max.ToString();

                        //接收曲线限幅
                        SerialPort_Curve_ReceiveData_Count += 1;//接收曲线数加一
                        if (SerialPort_Curve_ReceiveData_Count > 32767)
                        {
                            SerialPort_Curve_ReceiveData_Count = 32767;
                        }
                        this.MasterPC_TabPage_Curve_Label_Information_Receive.Text = "已接收数据:" + SerialPort_Curve_ReceiveData_Count.ToString();
                    }
                    else//双CCD曲线
                    {
                        this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.Series[0].Points.DataBindY(ChartLine_List_1);
                        this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.Series[0].Points.DataBindY(ChartLine_List_2);

                        CCD_Value_Min = ChartLine_List_1[0];
                        CCD_Value_Max = ChartLine_List_1[0];
                        for (int i = 0; i < 128; i++)
                        {
                            if (ChartLine_List_1[i] < CCD_Value_Min)
                            {
                                CCD_Value_Min = ChartLine_List_1[i];
                            }

                            if (ChartLine_List_1[i] > CCD_Value_Max)
                            {
                                CCD_Value_Max = ChartLine_List_1[i];
                            }
                        }

                        this.MasterPC_TabPage_Curve_Label_CCD1_Min.Text = "CCD1最小值:" + CCD_Value_Min.ToString();
                        this.MasterPC_TabPage_Curve_Label_CCD1_Max.Text = "CCD1最大值:" + CCD_Value_Max.ToString();

                        CCD_Value_Min = ChartLine_List_2[0];
                        CCD_Value_Max = ChartLine_List_2[0];
                        for (int i = 0; i < 128; i++)
                        {
                            if (ChartLine_List_2[i] < CCD_Value_Min)
                            {
                                CCD_Value_Min = ChartLine_List_2[i];
                            }

                            if (ChartLine_List_2[i] > CCD_Value_Max)
                            {
                                CCD_Value_Max = ChartLine_List_2[i];
                            }
                        }

                        this.MasterPC_TabPage_Curve_Label_CCD2_Min.Text = "CCD2最小值:" + CCD_Value_Min.ToString();
                        this.MasterPC_TabPage_Curve_Label_CCD2_Max.Text = "CCD2最大值:" + CCD_Value_Max.ToString();

                        //接收曲线限幅
                        SerialPort_Curve_ReceiveData_Count += 2;//接收曲线数加一
                        if (SerialPort_Curve_ReceiveData_Count > 32767)
                        {
                            SerialPort_Curve_ReceiveData_Count = 32767;
                        }
                        this.MasterPC_TabPage_Curve_Label_Information_Receive.Text = "已接收数据:" + SerialPort_Curve_ReceiveData_Count.ToString();
                    }
                }
                ));

                SerialPort_CCD_Check_Finish = false;//等待拆分CCD数据
            }
        }

        #endregion

        #region 清除曲线
        private void MasterPC_TabControl_Curve_Button_CurveClear_Click(object sender, EventArgs e)//清除当前曲线
        {
            //清除绘图曲线
            ZedGraph_List_1.RemoveRange(0, ZedGraph_List_1.Count);//清除曲线1数据
            ZedGraph_List_2.RemoveRange(0, ZedGraph_List_2.Count);//清除曲线2数据
            ZedGraph_List_3.RemoveRange(0, ZedGraph_List_3.Count);//清除曲线3数据
            ZedGraph_List_4.RemoveRange(0, ZedGraph_List_4.Count);//清除曲线4数据
            ZedGraph_List_5.RemoveRange(0, ZedGraph_List_5.Count);//清除曲线5数据
            ZedGraph_List_6.RemoveRange(0, ZedGraph_List_6.Count);//清除曲线6数据
            ZedGraph_List_7.RemoveRange(0, ZedGraph_List_7.Count);//清除曲线7数据
            ZedGraph_List_8.RemoveRange(0, ZedGraph_List_8.Count);//清除曲线8数据

            ZedGraph_Curve_1.Clear();//清除曲线1数据
            ZedGraph_Curve_2.Clear();//清除曲线2数据
            ZedGraph_Curve_3.Clear();//清除曲线3数据
            ZedGraph_Curve_4.Clear();//清除曲线4数据
            ZedGraph_Curve_5.Clear();//清除曲线5数据
            ZedGraph_Curve_6.Clear();//清除曲线6数据
            ZedGraph_Curve_7.Clear();//清除曲线7数据
            ZedGraph_Curve_8.Clear();//清除曲线8数据

            this.MasterPC_TabControl_Curve_ZedGraph.Refresh();//ZedGraph刷新
            
            //清除接收曲线数据
            SerialPort_Curve_ReceiveData_Count = 0;
            this.MasterPC_TabPage_Curve_Label_Information_Receive.Text = "已接收数据:0"; 
        }

        #endregion

        #region 曲线导出
        private void MasterPC_TabControl_Curve_Button_Export_Click(object sender, EventArgs e)
        {
            //当前TabControl_Curve选项卡选中的选项页
            switch (this.MasterPC_TabControl_Curve.SelectedTab.Text)
            {
                case "串口曲线"://当前TabPage选中"串口曲线"页面
                    if (ZedGraph_List_1.Count != 0 || ZedGraph_List_2.Count != 0
                        || ZedGraph_List_3.Count != 0 || ZedGraph_List_4.Count != 0
                        || ZedGraph_List_5.Count != 0 || ZedGraph_List_6.Count != 0
                        || ZedGraph_List_7.Count != 0 || ZedGraph_List_8.Count != 0)
                    {
                        //导出当前数据
                        try
                        {
                            FolderBrowserDialog FilePath = new FolderBrowserDialog();//创建保存文件对话框
                            if (FilePath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                //创建TXT文件
                                FileStream FileData = File.Create(FilePath.SelectedPath + "\\" + "CurveData" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                                StreamWriter FileWriter = new StreamWriter(FileData);//写文件

                                FileData.SetLength(0);//设置数据流长度
                                FileWriter.Flush();//清除缓冲区

                                //写入日期
                                FileWriter.WriteLine("串口曲线数据");
                                FileWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));//写日期
                                FileWriter.WriteLine("\n");

                                //写入数据
                                if (this.MasterPC_TabControl_Curve_CheckBox1.Checked)//曲线1
                                {
                                    FileWriter.WriteLine("曲线1:");
                                    for (int i = 0; i < ZedGraph_List_1.Count; i++)
                                    {
                                        FileWriter.WriteLine(ZedGraph_List_1[i].Y.ToString());
                                    }
                                    FileWriter.WriteLine("\n");
                                }

                                if (this.MasterPC_TabControl_Curve_CheckBox2.Checked)//曲线2
                                {
                                    FileWriter.WriteLine("曲线2:");
                                    for (int i = 0; i < ZedGraph_List_2.Count; i++)
                                    {
                                        FileWriter.WriteLine(ZedGraph_List_2[i].Y.ToString());
                                    }
                                    FileWriter.WriteLine("\n");
                                }

                                if (this.MasterPC_TabControl_Curve_CheckBox3.Checked)//曲线3
                                {
                                    FileWriter.WriteLine("曲线3:");
                                    for (int i = 0; i < ZedGraph_List_3.Count; i++)
                                    {
                                        FileWriter.WriteLine(ZedGraph_List_3[i].Y.ToString());
                                    }
                                    FileWriter.WriteLine("\n");
                                }

                                if (this.MasterPC_TabControl_Curve_CheckBox4.Checked)//曲线4
                                {
                                    FileWriter.WriteLine("曲线4:");
                                    for (int i = 0; i < ZedGraph_List_4.Count; i++)
                                    {
                                        FileWriter.WriteLine(ZedGraph_List_4[i].Y.ToString());
                                    }
                                    FileWriter.WriteLine("\n");
                                }

                                if (this.MasterPC_TabControl_Curve_CheckBox5.Checked)//曲线5
                                {
                                    FileWriter.WriteLine("曲线5:");
                                    for (int i = 0; i < ZedGraph_List_5.Count; i++)
                                    {
                                        FileWriter.WriteLine(ZedGraph_List_5[i].Y.ToString());
                                    }
                                    FileWriter.WriteLine("\n");
                                }

                                if (this.MasterPC_TabControl_Curve_CheckBox6.Checked)//曲线6
                                {
                                    FileWriter.WriteLine("曲线6:");
                                    for (int i = 0; i < ZedGraph_List_6.Count; i++)
                                    {
                                        FileWriter.WriteLine(ZedGraph_List_6[i].Y.ToString());
                                    }
                                    FileWriter.WriteLine("\n");
                                }

                                if (this.MasterPC_TabControl_Curve_CheckBox7.Checked)//曲线7
                                {
                                    FileWriter.WriteLine("曲线7:");
                                    for (int i = 0; i < ZedGraph_List_7.Count; i++)
                                    {
                                        FileWriter.WriteLine(ZedGraph_List_7[i].Y.ToString());
                                    }
                                    FileWriter.WriteLine("\n");
                                }

                                if (this.MasterPC_TabControl_Curve_CheckBox8.Checked)//曲线8
                                {
                                    FileWriter.WriteLine("曲线8:");
                                    for (int i = 0; i < ZedGraph_List_8.Count; i++)
                                    {
                                        FileWriter.WriteLine(ZedGraph_List_8[i].Y.ToString());
                                    }
                                    FileWriter.WriteLine("\n");
                                }

                                FileWriter.Close();//关闭数据流
                                FileData.Close();//关闭文件

                                MessageBox.Show("数据导出成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        //异常处理
                        catch (IOException)
                        {
                            MessageBox.Show("数据导出失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("曲线数据不存在,无法导出!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                case "线性CCD"://当前TabPage选中"CCD"页面
                    //导出当前数据
                    try
                    {
                        FolderBrowserDialog FilePath = new FolderBrowserDialog();//创建保存文件对话框
                        if (FilePath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            //创建TXT文件
                            FileStream FileData = File.Create(FilePath.SelectedPath + "\\" + "CCDData" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                            StreamWriter FileWriter = new StreamWriter(FileData);//写文件

                            FileData.SetLength(0);//设置数据流长度
                            FileWriter.Flush();//清除缓冲区

                            //写入日期
                            FileWriter.WriteLine("CCD曲线数据");
                            FileWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));//写日期
                            FileWriter.WriteLine("\n");

                            //CCD曲线1
                            FileWriter.WriteLine("CCD曲线1:");
                            for (int i = 0; i < ChartLine_List_1.Count; i++)
                            {
                                FileWriter.WriteLine(ChartLine_List_1[i].ToString());
                            }
                            FileWriter.WriteLine("\n");

                            //CCD曲线2
                            FileWriter.WriteLine("CCD曲线2:");
                            for (int i = 0; i < ChartLine_List_2.Count; i++)
                            {
                                FileWriter.WriteLine(ChartLine_List_2[i].ToString());
                            }
                            FileWriter.WriteLine("\n");

                            FileWriter.Close();//关闭数据流
                            FileData.Close();//关闭文件

                            MessageBox.Show("数据导出成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                    //异常处理
                    catch (IOException)
                    {
                        MessageBox.Show("数据导出失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                case "摄像头"://当前TabPage选中"摄像头"页面
                    break;
                default:
                    MessageBox.Show("导出时发生错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        #endregion

        #region 曲线数量变化
        private void MasterPC_TabControl_Curve_NumberUpDown_CurveNum_ValueChanged(object sender, EventArgs e)//曲线数量发生变化时
        {
            //当前曲线数量
            switch ((int)this.MasterPC_TabControl_Curve_NumberUpDown_CurveNum.Value)
            {
                case 1://当前选中1条曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Enabled = true;//曲线1可用
                    this.MasterPC_TabControl_Curve_CheckBox2.Enabled = false;//曲线2不可用
                    this.MasterPC_TabControl_Curve_CheckBox3.Enabled = false;//曲线3不可用
                    this.MasterPC_TabControl_Curve_CheckBox4.Enabled = false;//曲线4不可用
                    this.MasterPC_TabControl_Curve_CheckBox5.Enabled = false;//曲线5不可用
                    this.MasterPC_TabControl_Curve_CheckBox6.Enabled = false;//曲线6不可用
                    this.MasterPC_TabControl_Curve_CheckBox7.Enabled = false;//曲线7不可用
                    this.MasterPC_TabControl_Curve_CheckBox8.Enabled = false;//曲线8不可用

                    this.MasterPC_TabControl_Curve_CheckBox1.Checked = true;//曲线1选中
                    this.MasterPC_TabControl_Curve_CheckBox2.Checked = false;//曲线2不选中
                    this.MasterPC_TabControl_Curve_CheckBox3.Checked = false;//曲线3不选中
                    this.MasterPC_TabControl_Curve_CheckBox4.Checked = false;//曲线4不选中
                    this.MasterPC_TabControl_Curve_CheckBox5.Checked = false;//曲线5不选中
                    this.MasterPC_TabControl_Curve_CheckBox6.Checked = false;//曲线6不选中
                    this.MasterPC_TabControl_Curve_CheckBox7.Checked = false;//曲线7不选中
                    this.MasterPC_TabControl_Curve_CheckBox8.Checked = false;//曲线8不选中

                    this.MasterPC_TabControl_Curve_Label1_Value.Enabled = true;//曲线1数值可用
                    this.MasterPC_TabControl_Curve_Label2_Value.Enabled = false;//曲线2数值不可用
                    this.MasterPC_TabControl_Curve_Label3_Value.Enabled = false;//曲线3数值不可用
                    this.MasterPC_TabControl_Curve_Label4_Value.Enabled = false;//曲线4数值不可用
                    this.MasterPC_TabControl_Curve_Label5_Value.Enabled = false;//曲线5数值不可用
                    this.MasterPC_TabControl_Curve_Label6_Value.Enabled = false;//曲线6数值不可用
                    this.MasterPC_TabControl_Curve_Label7_Value.Enabled = false;//曲线7数值不可用
                    this.MasterPC_TabControl_Curve_Label8_Value.Enabled = false;//曲线8数值不可用
                    break;
                case 2://当前选中2条曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Enabled = true;//曲线1可用
                    this.MasterPC_TabControl_Curve_CheckBox2.Enabled = true;//曲线2可用
                    this.MasterPC_TabControl_Curve_CheckBox3.Enabled = false;//曲线3不可用
                    this.MasterPC_TabControl_Curve_CheckBox4.Enabled = false;//曲线4不可用
                    this.MasterPC_TabControl_Curve_CheckBox5.Enabled = false;//曲线5不可用
                    this.MasterPC_TabControl_Curve_CheckBox6.Enabled = false;//曲线6不可用
                    this.MasterPC_TabControl_Curve_CheckBox7.Enabled = false;//曲线7不可用
                    this.MasterPC_TabControl_Curve_CheckBox8.Enabled = false;//曲线8不可用

                    this.MasterPC_TabControl_Curve_CheckBox1.Checked = true;//曲线1选中
                    this.MasterPC_TabControl_Curve_CheckBox2.Checked = true;//曲线2选中
                    this.MasterPC_TabControl_Curve_CheckBox3.Checked = false;//曲线3不选中
                    this.MasterPC_TabControl_Curve_CheckBox4.Checked = false;//曲线4不选中
                    this.MasterPC_TabControl_Curve_CheckBox5.Checked = false;//曲线5不选中
                    this.MasterPC_TabControl_Curve_CheckBox6.Checked = false;//曲线6不选中
                    this.MasterPC_TabControl_Curve_CheckBox7.Checked = false;//曲线7不选中
                    this.MasterPC_TabControl_Curve_CheckBox8.Checked = false;//曲线8不选中

                    this.MasterPC_TabControl_Curve_Label1_Value.Enabled = true;//曲线1数值可用
                    this.MasterPC_TabControl_Curve_Label2_Value.Enabled = true;//曲线2数值可用
                    this.MasterPC_TabControl_Curve_Label3_Value.Enabled = false;//曲线3数值不可用
                    this.MasterPC_TabControl_Curve_Label4_Value.Enabled = false;//曲线4数值不可用
                    this.MasterPC_TabControl_Curve_Label5_Value.Enabled = false;//曲线5数值不可用
                    this.MasterPC_TabControl_Curve_Label6_Value.Enabled = false;//曲线6数值不可用
                    this.MasterPC_TabControl_Curve_Label7_Value.Enabled = false;//曲线7数值不可用
                    this.MasterPC_TabControl_Curve_Label8_Value.Enabled = false;//曲线8数值不可用
                    break;
                case 3://当前选中3条曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Enabled = true;//曲线1可用
                    this.MasterPC_TabControl_Curve_CheckBox2.Enabled = true;//曲线2可用
                    this.MasterPC_TabControl_Curve_CheckBox3.Enabled = true;//曲线3可用
                    this.MasterPC_TabControl_Curve_CheckBox4.Enabled = false;//曲线4不可用
                    this.MasterPC_TabControl_Curve_CheckBox5.Enabled = false;//曲线5不可用
                    this.MasterPC_TabControl_Curve_CheckBox6.Enabled = false;//曲线6不可用
                    this.MasterPC_TabControl_Curve_CheckBox7.Enabled = false;//曲线7不可用
                    this.MasterPC_TabControl_Curve_CheckBox8.Enabled = false;//曲线8不可用

                    this.MasterPC_TabControl_Curve_CheckBox1.Checked = true;//曲线1选中
                    this.MasterPC_TabControl_Curve_CheckBox2.Checked = true;//曲线2选中
                    this.MasterPC_TabControl_Curve_CheckBox3.Checked = true;//曲线3选中
                    this.MasterPC_TabControl_Curve_CheckBox4.Checked = false;//曲线4不选中
                    this.MasterPC_TabControl_Curve_CheckBox5.Checked = false;//曲线5不选中
                    this.MasterPC_TabControl_Curve_CheckBox6.Checked = false;//曲线6不选中
                    this.MasterPC_TabControl_Curve_CheckBox7.Checked = false;//曲线7不选中
                    this.MasterPC_TabControl_Curve_CheckBox8.Checked = false;//曲线8不选中

                    this.MasterPC_TabControl_Curve_Label1_Value.Enabled = true;//曲线1数值可用
                    this.MasterPC_TabControl_Curve_Label2_Value.Enabled = true;//曲线2数值可用
                    this.MasterPC_TabControl_Curve_Label3_Value.Enabled = true;//曲线3数值可用
                    this.MasterPC_TabControl_Curve_Label4_Value.Enabled = false;//曲线4数值不可用
                    this.MasterPC_TabControl_Curve_Label5_Value.Enabled = false;//曲线5数值不可用
                    this.MasterPC_TabControl_Curve_Label6_Value.Enabled = false;//曲线6数值不可用
                    this.MasterPC_TabControl_Curve_Label7_Value.Enabled = false;//曲线7数值不可用
                    this.MasterPC_TabControl_Curve_Label8_Value.Enabled = false;//曲线8数值不可用
                    break;
                case 4://当前选中4条曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Enabled = true;//曲线1可用
                    this.MasterPC_TabControl_Curve_CheckBox2.Enabled = true;//曲线2可用
                    this.MasterPC_TabControl_Curve_CheckBox3.Enabled = true;//曲线3可用
                    this.MasterPC_TabControl_Curve_CheckBox4.Enabled = true;//曲线4可用
                    this.MasterPC_TabControl_Curve_CheckBox5.Enabled = false;//曲线5不可用
                    this.MasterPC_TabControl_Curve_CheckBox6.Enabled = false;//曲线6不可用
                    this.MasterPC_TabControl_Curve_CheckBox7.Enabled = false;//曲线7不可用
                    this.MasterPC_TabControl_Curve_CheckBox8.Enabled = false;//曲线8不可用

                    this.MasterPC_TabControl_Curve_CheckBox1.Checked = true;//曲线1选中
                    this.MasterPC_TabControl_Curve_CheckBox2.Checked = true;//曲线2选中
                    this.MasterPC_TabControl_Curve_CheckBox3.Checked = true;//曲线3选中
                    this.MasterPC_TabControl_Curve_CheckBox4.Checked = true;//曲线4选中
                    this.MasterPC_TabControl_Curve_CheckBox5.Checked = false;//曲线5不选中
                    this.MasterPC_TabControl_Curve_CheckBox6.Checked = false;//曲线6不选中
                    this.MasterPC_TabControl_Curve_CheckBox7.Checked = false;//曲线7不选中
                    this.MasterPC_TabControl_Curve_CheckBox8.Checked = false;//曲线8不选中

                    this.MasterPC_TabControl_Curve_Label1_Value.Enabled = true;//曲线1数值可用
                    this.MasterPC_TabControl_Curve_Label2_Value.Enabled = true;//曲线2数值可用
                    this.MasterPC_TabControl_Curve_Label3_Value.Enabled = true;//曲线3数值可用
                    this.MasterPC_TabControl_Curve_Label4_Value.Enabled = true;//曲线4数值可用
                    this.MasterPC_TabControl_Curve_Label5_Value.Enabled = false;//曲线5数值不可用
                    this.MasterPC_TabControl_Curve_Label6_Value.Enabled = false;//曲线6数值不可用
                    this.MasterPC_TabControl_Curve_Label7_Value.Enabled = false;//曲线7数值不可用
                    this.MasterPC_TabControl_Curve_Label8_Value.Enabled = false;//曲线8数值不可用
                    break;
                case 5://当前选中5条曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Enabled = true;//曲线1可用
                    this.MasterPC_TabControl_Curve_CheckBox2.Enabled = true;//曲线2可用
                    this.MasterPC_TabControl_Curve_CheckBox3.Enabled = true;//曲线3可用
                    this.MasterPC_TabControl_Curve_CheckBox4.Enabled = true;//曲线4可用
                    this.MasterPC_TabControl_Curve_CheckBox5.Enabled = true;//曲线5可用
                    this.MasterPC_TabControl_Curve_CheckBox6.Enabled = false;//曲线6不可用
                    this.MasterPC_TabControl_Curve_CheckBox7.Enabled = false;//曲线7不可用
                    this.MasterPC_TabControl_Curve_CheckBox8.Enabled = false;//曲线8不可用

                    this.MasterPC_TabControl_Curve_CheckBox1.Checked = true;//曲线1选中
                    this.MasterPC_TabControl_Curve_CheckBox2.Checked = true;//曲线2选中
                    this.MasterPC_TabControl_Curve_CheckBox3.Checked = true;//曲线3选中
                    this.MasterPC_TabControl_Curve_CheckBox4.Checked = true;//曲线4选中
                    this.MasterPC_TabControl_Curve_CheckBox5.Checked = true;//曲线5选中
                    this.MasterPC_TabControl_Curve_CheckBox6.Checked = false;//曲线6不选中
                    this.MasterPC_TabControl_Curve_CheckBox7.Checked = false;//曲线7不选中
                    this.MasterPC_TabControl_Curve_CheckBox8.Checked = false;//曲线8不选中

                    this.MasterPC_TabControl_Curve_Label1_Value.Enabled = true;//曲线1数值可用
                    this.MasterPC_TabControl_Curve_Label2_Value.Enabled = true;//曲线2数值可用
                    this.MasterPC_TabControl_Curve_Label3_Value.Enabled = true;//曲线3数值可用
                    this.MasterPC_TabControl_Curve_Label4_Value.Enabled = true;//曲线4数值可用
                    this.MasterPC_TabControl_Curve_Label5_Value.Enabled = true;//曲线5数值可用
                    this.MasterPC_TabControl_Curve_Label6_Value.Enabled = false;//曲线6数值不可用
                    this.MasterPC_TabControl_Curve_Label7_Value.Enabled = false;//曲线7数值不可用
                    this.MasterPC_TabControl_Curve_Label8_Value.Enabled = false;//曲线8数值不可用
                    break;
                case 6://当前选中6条曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Enabled = true;//曲线1可用
                    this.MasterPC_TabControl_Curve_CheckBox2.Enabled = true;//曲线2可用
                    this.MasterPC_TabControl_Curve_CheckBox3.Enabled = true;//曲线3可用
                    this.MasterPC_TabControl_Curve_CheckBox4.Enabled = true;//曲线4可用
                    this.MasterPC_TabControl_Curve_CheckBox5.Enabled = true;//曲线5可用
                    this.MasterPC_TabControl_Curve_CheckBox6.Enabled = true;//曲线6可用
                    this.MasterPC_TabControl_Curve_CheckBox7.Enabled = false;//曲线7不可用
                    this.MasterPC_TabControl_Curve_CheckBox8.Enabled = false;//曲线8不可用

                    this.MasterPC_TabControl_Curve_CheckBox1.Checked = true;//曲线1选中
                    this.MasterPC_TabControl_Curve_CheckBox2.Checked = true;//曲线2选中
                    this.MasterPC_TabControl_Curve_CheckBox3.Checked = true;//曲线3选中
                    this.MasterPC_TabControl_Curve_CheckBox4.Checked = true;//曲线4选中
                    this.MasterPC_TabControl_Curve_CheckBox5.Checked = true;//曲线5选中
                    this.MasterPC_TabControl_Curve_CheckBox6.Checked = true;//曲线6选中
                    this.MasterPC_TabControl_Curve_CheckBox7.Checked = false;//曲线7不选中
                    this.MasterPC_TabControl_Curve_CheckBox8.Checked = false;//曲线8不选中

                    this.MasterPC_TabControl_Curve_Label1_Value.Enabled = true;//曲线1数值可用
                    this.MasterPC_TabControl_Curve_Label2_Value.Enabled = true;//曲线2数值可用
                    this.MasterPC_TabControl_Curve_Label3_Value.Enabled = true;//曲线3数值可用
                    this.MasterPC_TabControl_Curve_Label4_Value.Enabled = true;//曲线4数值可用
                    this.MasterPC_TabControl_Curve_Label5_Value.Enabled = true;//曲线5数值可用
                    this.MasterPC_TabControl_Curve_Label6_Value.Enabled = true;//曲线6数值可用
                    this.MasterPC_TabControl_Curve_Label7_Value.Enabled = false;//曲线7数值不可用
                    this.MasterPC_TabControl_Curve_Label8_Value.Enabled = false;//曲线8数值不可用
                    break;
                case 7://当前选中7条曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Enabled = true;//曲线1可用
                    this.MasterPC_TabControl_Curve_CheckBox2.Enabled = true;//曲线2可用
                    this.MasterPC_TabControl_Curve_CheckBox3.Enabled = true;//曲线3可用
                    this.MasterPC_TabControl_Curve_CheckBox4.Enabled = true;//曲线4可用
                    this.MasterPC_TabControl_Curve_CheckBox5.Enabled = true;//曲线5可用
                    this.MasterPC_TabControl_Curve_CheckBox6.Enabled = true;//曲线6可用
                    this.MasterPC_TabControl_Curve_CheckBox7.Enabled = true;//曲线7可用
                    this.MasterPC_TabControl_Curve_CheckBox8.Enabled = false;//曲线8不可用

                    this.MasterPC_TabControl_Curve_CheckBox1.Checked = true;//曲线1选中
                    this.MasterPC_TabControl_Curve_CheckBox2.Checked = true;//曲线2选中
                    this.MasterPC_TabControl_Curve_CheckBox3.Checked = true;//曲线3选中
                    this.MasterPC_TabControl_Curve_CheckBox4.Checked = true;//曲线4选中
                    this.MasterPC_TabControl_Curve_CheckBox5.Checked = true;//曲线5选中
                    this.MasterPC_TabControl_Curve_CheckBox6.Checked = true;//曲线6选中
                    this.MasterPC_TabControl_Curve_CheckBox7.Checked = true;//曲线7选中
                    this.MasterPC_TabControl_Curve_CheckBox8.Checked = false;//曲线8不选中

                    this.MasterPC_TabControl_Curve_Label1_Value.Enabled = true;//曲线1数值可用
                    this.MasterPC_TabControl_Curve_Label2_Value.Enabled = true;//曲线2数值可用
                    this.MasterPC_TabControl_Curve_Label3_Value.Enabled = true;//曲线3数值可用
                    this.MasterPC_TabControl_Curve_Label4_Value.Enabled = true;//曲线4数值可用
                    this.MasterPC_TabControl_Curve_Label5_Value.Enabled = true;//曲线5数值可用
                    this.MasterPC_TabControl_Curve_Label6_Value.Enabled = true;//曲线6数值可用
                    this.MasterPC_TabControl_Curve_Label7_Value.Enabled = true;//曲线7数值可用
                    this.MasterPC_TabControl_Curve_Label8_Value.Enabled = false;//曲线8数值不可用
                    break;
                case 8://当前选中8条曲线
                    this.MasterPC_TabControl_Curve_CheckBox1.Enabled = true;//曲线1可用
                    this.MasterPC_TabControl_Curve_CheckBox2.Enabled = true;//曲线2可用
                    this.MasterPC_TabControl_Curve_CheckBox3.Enabled = true;//曲线3可用
                    this.MasterPC_TabControl_Curve_CheckBox4.Enabled = true;//曲线4可用
                    this.MasterPC_TabControl_Curve_CheckBox5.Enabled = true;//曲线5可用
                    this.MasterPC_TabControl_Curve_CheckBox6.Enabled = true;//曲线6可用
                    this.MasterPC_TabControl_Curve_CheckBox7.Enabled = true;//曲线7可用
                    this.MasterPC_TabControl_Curve_CheckBox8.Enabled = true;//曲线8可用

                    this.MasterPC_TabControl_Curve_CheckBox1.Checked = true;//曲线1选中
                    this.MasterPC_TabControl_Curve_CheckBox2.Checked = true;//曲线2选中
                    this.MasterPC_TabControl_Curve_CheckBox3.Checked = true;//曲线3选中
                    this.MasterPC_TabControl_Curve_CheckBox4.Checked = true;//曲线4选中
                    this.MasterPC_TabControl_Curve_CheckBox5.Checked = true;//曲线5选中
                    this.MasterPC_TabControl_Curve_CheckBox6.Checked = true;//曲线6选中
                    this.MasterPC_TabControl_Curve_CheckBox7.Checked = true;//曲线7选中
                    this.MasterPC_TabControl_Curve_CheckBox8.Checked = true;//曲线8选中

                    this.MasterPC_TabControl_Curve_Label1_Value.Enabled = true;//曲线1数值可用
                    this.MasterPC_TabControl_Curve_Label2_Value.Enabled = true;//曲线2数值可用
                    this.MasterPC_TabControl_Curve_Label3_Value.Enabled = true;//曲线3数值可用
                    this.MasterPC_TabControl_Curve_Label4_Value.Enabled = true;//曲线4数值可用
                    this.MasterPC_TabControl_Curve_Label5_Value.Enabled = true;//曲线5数值可用
                    this.MasterPC_TabControl_Curve_Label6_Value.Enabled = true;//曲线6数值可用
                    this.MasterPC_TabControl_Curve_Label7_Value.Enabled = true;//曲线7数值可用
                    this.MasterPC_TabControl_Curve_Label8_Value.Enabled = true;//曲线8数值可用
                    break;
                default://默认情况
                    MessageBox.Show("曲线数量输入错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        #endregion

        #region 曲线复选框事件

        #region 曲线复选框Checked变化
        private void MasterPC_TabControl_Curve_CheckBox1_CheckedChanged(object sender, EventArgs e)//曲线1复选框选中变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox1.Checked)//曲线1复选框选中
            {
                ZedGraph_Curve_1.IsVisible = true;//曲线1可见
            }
            else//曲线1复选框未选中
            {
                ZedGraph_Curve_1.IsVisible = false;//曲线1不可见
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox2_CheckedChanged(object sender, EventArgs e)//曲线2复选框选中变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox2.Checked)//曲线2复选框选中
            {
                ZedGraph_Curve_2.IsVisible = true;//曲线2可见
            }
            else//曲线2复选框未选中
            {
                ZedGraph_Curve_2.IsVisible = false;//曲线2不可见
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox3_CheckedChanged(object sender, EventArgs e)//曲线3复选框选中变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox3.Checked)//曲线3复选框选中
            {
                ZedGraph_Curve_3.IsVisible = true;//曲线3可见
            }
            else//曲线3复选框未选中
            {
                ZedGraph_Curve_3.IsVisible = false;//曲线3不可见
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox4_CheckedChanged(object sender, EventArgs e)//曲线4复选框选中变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox4.Checked)//曲线4复选框选中
            {
                ZedGraph_Curve_4.IsVisible = true;//曲线4可见
            }
            else//曲线4复选框未选中
            {
                ZedGraph_Curve_4.IsVisible = false;//曲线4不可见
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox5_CheckedChanged(object sender, EventArgs e)//曲线5复选框选中变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox5.Checked)//曲线5复选框选中
            {
                ZedGraph_Curve_5.IsVisible = true;//曲线5可见
            }
            else//曲线5复选框未选中
            {
                ZedGraph_Curve_5.IsVisible = false;//曲线5不可见
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox6_CheckedChanged(object sender, EventArgs e)//曲线6复选框选中变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox6.Checked)//曲线6复选框选中
            {
                ZedGraph_Curve_6.IsVisible = true;//曲线6可见
            }
            else//曲线6复选框未选中
            {
                ZedGraph_Curve_6.IsVisible = false;//曲线6不可见
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox7_CheckedChanged(object sender, EventArgs e)//曲线7复选框选中变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox7.Checked)//曲线7复选框选中
            {
                ZedGraph_Curve_7.IsVisible = true;//曲线7可见
            }
            else//曲线7复选框未选中
            {
                ZedGraph_Curve_7.IsVisible = false;//曲线7不可见
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox8_CheckedChanged(object sender, EventArgs e)//曲线8复选框选中变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox8.Checked)//曲线8复选框选中
            {
                ZedGraph_Curve_8.IsVisible = true;//曲线8可见
            }
            else//曲线8复选框未选中
            {
                ZedGraph_Curve_8.IsVisible = false;//曲线8不可见
            }
        }

        #endregion

        #region 曲线复选框Enabled变化

        private void MasterPC_TabControl_Curve_CheckBox1_EnabledChanged(object sender, EventArgs e)//曲线1复选框Enabled变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox1.Enabled == false)//曲线1复选框禁止
            {
                ZedGraph_Curve_1.IsVisible = false;//曲线1不可见
            }
            else//曲线1复选框可用
            {
                if (this.MasterPC_TabControl_Curve_CheckBox1.Checked)//曲线1复选框选中
                {
                    ZedGraph_Curve_1.IsVisible = true;//曲线1可见
                }
                else
                {
                    ZedGraph_Curve_1.IsVisible = false;//曲线1不可见
                }
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox2_EnabledChanged(object sender, EventArgs e)//曲线2复选框Enabled变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox2.Enabled == false)//曲线2复选框禁止
            {
                ZedGraph_Curve_2.IsVisible = false;//曲线2不可见
            }
            else//曲线2复选框可用
            {
                if (this.MasterPC_TabControl_Curve_CheckBox2.Checked)//曲线2复选框选中
                {
                    ZedGraph_Curve_2.IsVisible = true;//曲线2可见
                }
                else
                {
                    ZedGraph_Curve_2.IsVisible = false;//曲线2不可见
                }
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox3_EnabledChanged(object sender, EventArgs e)//曲线3复选框Enabled变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox3.Enabled == false)//曲线3复选框禁止
            {
                ZedGraph_Curve_3.IsVisible = false;//曲线3不可见
            }
            else//曲线3复选框可用
            {
                if (this.MasterPC_TabControl_Curve_CheckBox3.Checked)//曲线3复选框选中
                {
                    ZedGraph_Curve_3.IsVisible = true;//曲线3可见
                }
                else
                {
                    ZedGraph_Curve_3.IsVisible = false;//曲线3不可见
                }
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox4_EnabledChanged(object sender, EventArgs e)//曲线4复选框Enabled变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox4.Enabled == false)//曲线4复选框禁止
            {
                ZedGraph_Curve_4.IsVisible = false;//曲线4不可见
            }
            else//曲线4复选框可用
            {
                if (this.MasterPC_TabControl_Curve_CheckBox4.Checked)//曲线4复选框选中
                {
                    ZedGraph_Curve_4.IsVisible = true;//曲线4可见
                }
                else
                {
                    ZedGraph_Curve_4.IsVisible = false;//曲线4不可见
                }
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox5_EnabledChanged(object sender, EventArgs e)//曲线5复选框Enabled变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox5.Enabled == false)//曲线5复选框禁止
            {
                ZedGraph_Curve_5.IsVisible = false;//曲线5不可见
            }
            else//曲线5复选框可用
            {
                if (this.MasterPC_TabControl_Curve_CheckBox5.Checked)//曲线5复选框选中
                {
                    ZedGraph_Curve_5.IsVisible = true;//曲线5可见
                }
                else
                {
                    ZedGraph_Curve_5.IsVisible = false;//曲线5不可见
                }
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox6_EnabledChanged(object sender, EventArgs e)//曲线6复选框Enabled变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox6.Enabled == false)//曲线6复选框禁止
            {
                ZedGraph_Curve_6.IsVisible = false;//曲线6不可见
            }
            else//曲线6复选框可用
            {
                if (this.MasterPC_TabControl_Curve_CheckBox6.Checked)//曲线6复选框选中
                {
                    ZedGraph_Curve_6.IsVisible = true;//曲线6可见
                }
                else
                {
                    ZedGraph_Curve_6.IsVisible = false;//曲线6不可见
                }
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox7_EnabledChanged(object sender, EventArgs e)//曲线7复选框Enabled变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox7.Enabled == false)//曲线7复选框禁止
            {
                ZedGraph_Curve_7.IsVisible = false;//曲线7不可见
            }
            else//曲线7复选框可用
            {
                if (this.MasterPC_TabControl_Curve_CheckBox7.Checked)//曲线7复选框选中
                {
                    ZedGraph_Curve_7.IsVisible = true;//曲线7可见
                }
                else
                {
                    ZedGraph_Curve_7.IsVisible = false;//曲线7不可见
                }
            }
        }

        private void MasterPC_TabControl_Curve_CheckBox8_EnabledChanged(object sender, EventArgs e)//曲线8复选框Enabled变化
        {
            if (this.MasterPC_TabControl_Curve_CheckBox8.Enabled == false)//曲线8复选框禁止
            {
                ZedGraph_Curve_8.IsVisible = false;//曲线8不可见
            }
            else//曲线8复选框可用
            {
                if (this.MasterPC_TabControl_Curve_CheckBox8.Checked)//曲线8复选框选中
                {
                    ZedGraph_Curve_8.IsVisible = true;//曲线8可见
                }
                else
                {
                    ZedGraph_Curve_8.IsVisible = false;//曲线8不可见
                }
            }
        }

        #endregion

        #endregion

        #region CCD采用功能选择
        private void MasterPC_TabPage_Curve_ComboBox_CCD_Number_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.MasterPC_TabPage_Curve_ComboBox_CCD_Number.SelectedIndex)
            {
                case 0 :
                    //CCD数量1
                    Curve_CCD_Number_Flag = 0;
                    break;
                case 1:
                    //CCD数量2
                    Curve_CCD_Number_Flag = 1;
                    break;
                default:
                    break;
            }
        }

        private void MasterPC_TabPage_Curve_ComboBox_CCD_Bit_SelectedIndexChanged(object sender, EventArgs e)//CCD采集精度改变
        {
            switch (this.MasterPC_TabPage_Curve_ComboBox_CCD_Bit.SelectedIndex)
            {
                case 0:
                    //8位精度
                    Curve_CCD_Sample_Bit_Flag = 0;
                    this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisY.Maximum = 255;
                    this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisY.Maximum = 255;
                    break;
                case 1:
                    //10位精度
                    Curve_CCD_Sample_Bit_Flag = 1;
                    this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisY.Maximum = 1023;
                    this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisY.Maximum = 1023;
                    break;
                case 2:
                    //12位精度
                    Curve_CCD_Sample_Bit_Flag = 2;
                    this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisY.Maximum = 4095;
                    this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisY.Maximum = 4095;
                    break;
                default:
                    break;
            }
        }

        private void MasterPC_TabPage_Curve_CheckBox_CCD_Grid_CheckedChanged(object sender, EventArgs e)
        {
            if (this.MasterPC_TabPage_Curve_CheckBox_CCD_Grid.Checked)//CCD_Grid选中
            {
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;//网格设置为虚线
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;//网格可见
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;//网格设置为虚线
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisY.MajorGrid.Enabled = true;//网格可见
            }
            else//CCD_Grid未选中
            {
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;//网格不可见
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.ChartAreas[0].AxisY.MajorGrid.Enabled = false;//网格不可见
            }
        }

        private void MasterPC_TabPage_Curve_CheckBox_CCD_Point_CheckedChanged(object sender, EventArgs e)
        {
            if (this.MasterPC_TabPage_Curve_CheckBox_CCD_Point.Checked)//CCD_Point选中
            {
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.Series[0].MarkerBorderColor = Color.Transparent;//Chart1曲线标记点无边框
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.Series[0].MarkerColor = Color.SkyBlue;//Chart1曲线标记点颜色为天蓝色
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.Series[0].MarkerStyle = MarkerStyle.Circle;//Chart1曲线标记点圆形
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.Series[0].MarkerBorderColor = Color.Transparent;//Chart2曲线标记点无边框
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.Series[0].MarkerColor = Color.SkyBlue;//Chart2曲线标记点颜色为天蓝色
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.Series[0].MarkerStyle = MarkerStyle.Circle;//Chart2曲线标记点圆形
            }
            else//CCD_Point未选中
            {
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart1.Series[0].MarkerStyle = MarkerStyle.None;//Chart1曲线标记点不显示
                this.MasterPC_TabPage_Curve_TabPage_CCD_Chart2.Series[0].MarkerStyle = MarkerStyle.None;//Chart2曲线标记点不显示
            }
        }

        private void MasterPC_TabPage_Curve_TabPage_CCD_Chart1_GetToolTipText(object sender, ToolTipEventArgs e)//Chart1鼠标坐标显示
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                System.Windows.Forms.DataVisualization.Charting.DataPoint dp = e.HitTestResult.Series.Points[i];
                e.Text = dp.YValues[0].ToString();
            }
        }

        private void MasterPC_TabPage_Curve_TabPage_CCD_Chart2_GetToolTipText(object sender, ToolTipEventArgs e)//Chart2鼠标坐标显示
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                System.Windows.Forms.DataVisualization.Charting.DataPoint dp = e.HitTestResult.Series.Points[i];
                e.Text = dp.YValues[0].ToString();
            }
        }

        #endregion

        #endregion

        #region 数据分析

        #region 打开文件
        private void MasterPC_TabPage_Data_Button_OpenFile_Click(object sender, EventArgs e)//单击打开文件按钮,读取SD卡
        {
            OpenFileDialog File_New = new OpenFileDialog();//打开文件
            File_New.Filter = "文本文档|*.txt";//文件筛选,寻找".txt"文件
            if (File_New.ShowDialog() == DialogResult.OK)//打开文件完成
            {
                FileInfo File_New_Information = new FileInfo(File_New.FileName);//FileInfor获取文件信息
                FileStream File_New_Stream = new FileStream(File_New.FileName, FileMode.Open);//FileStream创建读写文件
                long File_New_Length = File_New_Information.Length;//文件数组长度
                byte[] File_New_Data_Buff = new byte[File_New_Length];//文件缓冲数组

                Image_Index = 0;//图形指针索引清零
                Image_Frame = 0;//图形总帧数清零
                File_Data_List.Clear();//清除数据线性链表
                File_Data_List_Index_First.Clear();//清除数据线性链表起始
                File_Data_List_Index_Last.Clear();//清除数据线性链表末尾
                File_Data_BitmapList.Clear();//清除图形库链表
                File_Data_List_ImageFlag.Clear();//清除图形标志
                Curve_Clear();//清除曲线数据链表

                File_New_Stream.Read(File_New_Data_Buff, 0, (int)File_New_Length);//文件数据读入缓冲数组
                for (int i = 0; i < File_New_Data_Buff.Length; i++)
                {
                    File_Data_List.Add(File_New_Data_Buff[i]);//数据链表添加元素
                }
                File_New_Stream.Close();//关闭文件
                File_Data_Calculate_Frame(File_Data_List);//图形数据分帧
                Curve_Draw();//绘制曲线
                Image_Draw(0);//绘制图形
                Parameter_Draw(0);//绘制实时曲线
                Curve_Frame_Draw(0);//绘制曲线
            }

        }

        #endregion

        #region 导出保存
        private void MasterPC_TabPage_Data_Button_SaveCurve_Click(object sender, EventArgs e)//导出曲线数据
        {
            if (Image_Index > 0)
            {
                //导出当前数据
                try
                {
                    FolderBrowserDialog FilePath = new FolderBrowserDialog();//创建保存文件对话框
                    if (FilePath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //创建TXT文件
                        FileStream FileData = File.Create(FilePath.SelectedPath + "\\" + "ImageData" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                        StreamWriter FileWriter = new StreamWriter(FileData);//写文件

                        FileData.SetLength(0);//设置数据流长度
                        FileWriter.Flush();//清除缓冲区

                        //写入日期
                        FileWriter.WriteLine("SD卡曲线数据");
                        FileWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));//写日期
                        FileWriter.WriteLine("\n");

                        //写入数据
                        FileWriter.WriteLine("曲线" + this.MasterPC_TabPage_Data_TextBox_Var1.Text);//曲线1
                        for (int i = 0; i < Curve_Data_List_1.Count; i++)
                        {
                            FileWriter.WriteLine(Curve_Data_List_1[i].ToString());
                        }
                        FileWriter.WriteLine("\n");

                        FileWriter.WriteLine("曲线" + this.MasterPC_TabPage_Data_TextBox_Var2.Text);//曲线2
                        for (int i = 0; i < Curve_Data_List_2.Count; i++)
                        {
                            FileWriter.WriteLine(Curve_Data_List_2[i].ToString());
                        }
                        FileWriter.WriteLine("\n");

                        FileWriter.WriteLine("曲线" + this.MasterPC_TabPage_Data_TextBox_Var3.Text);//曲线3
                        for (int i = 0; i < Curve_Data_List_3.Count; i++)
                        {
                            FileWriter.WriteLine(Curve_Data_List_3[i].ToString());
                        }
                        FileWriter.WriteLine("\n");

                        FileWriter.WriteLine("曲线" + this.MasterPC_TabPage_Data_TextBox_Var4.Text);//曲线4
                        for (int i = 0; i < Curve_Data_List_4.Count; i++)
                        {
                            FileWriter.WriteLine(Curve_Data_List_4[i].ToString());
                        }
                        FileWriter.WriteLine("\n");

                        FileWriter.WriteLine("曲线" + this.MasterPC_TabPage_Data_TextBox_Var5.Text);//曲线5
                        for (int i = 0; i < Curve_Data_List_5.Count; i++)
                        {
                            FileWriter.WriteLine(Curve_Data_List_5[i].ToString());
                        }
                        FileWriter.WriteLine("\n");

                        FileWriter.WriteLine("曲线" + this.MasterPC_TabPage_Data_TextBox_Var6.Text);//曲线6
                        for (int i = 0; i < Curve_Data_List_6.Count; i++)
                        {
                            FileWriter.WriteLine(Curve_Data_List_6[i].ToString());
                        }
                        FileWriter.WriteLine("\n");

                        FileWriter.WriteLine("曲线" + this.MasterPC_TabPage_Data_TextBox_Var7.Text);//曲线7
                        for (int i = 0; i < Curve_Data_List_7.Count; i++)
                        {
                            FileWriter.WriteLine(Curve_Data_List_7[i].ToString());
                        }
                        FileWriter.WriteLine("\n");

                        FileWriter.WriteLine("曲线" + this.MasterPC_TabPage_Data_TextBox_Var8.Text);//曲线8
                        for (int i = 0; i < Curve_Data_List_8.Count; i++)
                        {
                            FileWriter.WriteLine(Curve_Data_List_8[i].ToString());
                        }
                        FileWriter.WriteLine("\n");

                        FileWriter.WriteLine("曲线" + this.MasterPC_TabPage_Data_TextBox_Var9.Text);//曲线9
                        for (int i = 0; i < Curve_Data_List_9.Count; i++)
                        {
                            FileWriter.WriteLine(Curve_Data_List_9[i].ToString());
                        }
                        FileWriter.WriteLine("\n");


                        FileWriter.Close();//关闭数据流
                        FileData.Close();//关闭文件

                        MessageBox.Show("数据导出成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                //异常处理
                catch (IOException)
                {
                    MessageBox.Show("数据导出失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("曲线数据不存在,无法导出!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        #endregion

        #region 图形图表设置
        private void MasterPC_TabPage_Data_TextBox_Page_MouseDown(object sender, MouseEventArgs e)//图像帧文本设置
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Row_KeyPress(object sender, KeyPressEventArgs e)//图形行号输入判断
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Enter)//限制只能输入数字,退格和回车
            {
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Enter)//按下回车"Enter"
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项

                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Row.Text == "")//未输入数字
                    {
                        MessageBox.Show("请输入有效数字!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else//输入数字
                    {
                        Image_Row = Convert.ToInt16(this.MasterPC_TabPage_Data_TextBox_Row.Text);
                        MyReg2.SetValue("Image_Row", this.MasterPC_TabPage_Data_TextBox_Row.Text.ToString());
                        MessageBox.Show("参数已保存修改", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("设置保存失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Col_KeyPress(object sender, KeyPressEventArgs e)//图形列号输入判断
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Enter)//限制只能输入数字,退格和回车
            {
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Enter)//按下回车"Enter"
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项

                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Col.Text == "")//未输入数字
                    {
                        MessageBox.Show("请输入有效数字!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else//输入数字
                    {
                        Image_Col = Convert.ToInt16(this.MasterPC_TabPage_Data_TextBox_Col.Text);
                        MyReg2.SetValue("Image_Col", this.MasterPC_TabPage_Data_TextBox_Col.Text.ToString());
                        MessageBox.Show("参数已保存修改", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("设置保存失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Jump_KeyPress(object sender, KeyPressEventArgs e)//图形跳变输入判断
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Enter)//限制只能输入数字,退格和回车
            {
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Enter)//按下回车"Enter"
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项

                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Jump.Text == "")//未输入数字
                    {
                        MessageBox.Show("请输入有效数字!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else//输入数字
                    {
                        Image_Jump = Convert.ToInt16(this.MasterPC_TabPage_Data_TextBox_Jump.Text);
                        MyReg2.SetValue("Image_Jump", this.MasterPC_TabPage_Data_TextBox_Jump.Text.ToString());
                        MessageBox.Show("参数已保存修改", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("设置保存失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_YMaxValue_KeyPress(object sender, KeyPressEventArgs e)//图形幅值输入判断
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Enter)//限制只能输入数字,退格和回车
            {
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Enter)//按下回车"Enter"
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项

                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_YMaxValue.Text == "")//未输入数字
                    {
                        MessageBox.Show("请输入有效数字!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else//输入数字
                    {
                        Chart_Amplitude = Convert.ToInt16(this.MasterPC_TabPage_Data_TextBox_YMaxValue.Text);
                        this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisY.Maximum = Chart_Amplitude;
                        MyReg2.SetValue("Chart_Amplitude", this.MasterPC_TabPage_Data_TextBox_YMaxValue.Text.ToString());
                        MessageBox.Show("参数已保存修改", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("设置保存失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region 图形帧切换
        private void MasterPC_TabPage_Data_Button_Up_Click(object sender, EventArgs e)//上一帧
        {
            if (Image_Index > 0)//当前数据图形有效,可以按键切换帧
            {
                Image_Index -= 1;

                if (Image_Index < 1)
                {
                    Image_Index = 1;
                }
                else if (Image_Index > Image_Frame)
                {
                    Image_Index = Image_Frame;
                }
                else
                {
                    Image_Index = Image_Index;
                }
                this.MasterPC_TabPage_Data_TextBox_Page.Text = Image_Index.ToString() + "/" + Image_Frame.ToString();//列表页数刷新
                Image_Draw(Image_Index - 1);//绘制图形
                Parameter_Draw(Image_Index - 1);//绘制实时曲线
                Curve_Frame_Draw(Image_Index - 1);//绘制曲线
            }
        }

        private void MasterPC_TabPage_Data_Button_Down_Click(object sender, EventArgs e)//下一帧
        {
            if (Image_Index > 0)//当前数据图形有效,可以按键切换帧
            {
                Image_Index += 1;

                if (Image_Index < 1)
                {
                    Image_Index = 1;
                }
                else if (Image_Index > Image_Frame)
                {
                    Image_Index = Image_Frame;
                }
                else
                {
                    Image_Index = Image_Index;
                }
                this.MasterPC_TabPage_Data_TextBox_Page.Text = Image_Index.ToString() + "/" + Image_Frame.ToString();//列表页数刷新
                Image_Draw(Image_Index - 1);//绘制图形
                Parameter_Draw(Image_Index - 1);//绘制实时曲线
                Curve_Frame_Draw(Image_Index - 1);//绘制曲线
            }
        }

        private void MasterPC_TabPage_Data_Button_Uping_Click(object sender, EventArgs e)//连续后退
        {
            if (Image_Index > 0)//当前数据图形有效,可以按键切换帧
            {
                if (Image_Index == 1)//到达第一帧
                {
                    MessageBox.Show("当前帧已为第一帧!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else//未达到第一帧
                {
                    if (this.Timer_Timing_ImagePlay.Enabled == false)//当前定时器关闭
                    {
                        Image_Play_Order = 1;//连续后退
                        this.MasterPC_TabPage_Data_Button_Uping.Text = "暂停";
                        this.MasterPC_TabPage_Data_Button_Downing.Enabled = false;//关闭连续按钮
                        this.Timer_Timing_ImagePlay.Enabled = true;//定时器打开
                        this.Timer_Timing_ImagePlay.Start();//开始计时
                    }
                    else//当前定时器打开
                    {
                        this.Timer_Timing_ImagePlay.Stop();//停止计时
                        this.Timer_Timing_ImagePlay.Enabled = false;//定时器关闭
                        this.MasterPC_TabPage_Data_Button_Uping.Text = "后退";
                        this.MasterPC_TabPage_Data_Button_Downing.Enabled = true;//开启连续按钮
                    }
                }
            }
        }

        private void MasterPC_TabPage_Data_Button_Downing_Click(object sender, EventArgs e)//连续播放
        {
            if (Image_Index > 0)//当前数据图形有效,可以按键切换帧
            {
                if (Image_Index == Image_Frame)//已到达最后一帧
                {
                    MessageBox.Show("当前帧已为最后一帧!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    if (this.Timer_Timing_ImagePlay.Enabled == false)//当前定时器关闭
                    {
                        Image_Play_Order = 0;//连续播放
                        this.MasterPC_TabPage_Data_Button_Downing.Text = "暂停";
                        this.MasterPC_TabPage_Data_Button_Uping.Enabled = false;//关闭后退按钮
                        this.Timer_Timing_ImagePlay.Enabled = true;//定时器打开
                        this.Timer_Timing_ImagePlay.Start();//开始计时
                    }
                    else//当前定时器打开
                    {
                        this.Timer_Timing_ImagePlay.Stop();//停止计时
                        this.Timer_Timing_ImagePlay.Enabled = false;//定时器关闭
                        this.MasterPC_TabPage_Data_Button_Downing.Text = "连续";
                        this.MasterPC_TabPage_Data_Button_Uping.Enabled = true;//开启后退按钮
                    }
                } 
            }        
        }

        private void MasterPC_TabPage_Data_Button_Jump_Click(object sender, EventArgs e)//帧跳转
        {
            if (Image_Index > 0)//当前数据图形有效,可以按键切换帧
            {
                Image_Index = Convert.ToInt16(this.MasterPC_TabPage_Data_NumberUpDown_Jump.Value);

                if (Image_Index < 1)
                {
                    Image_Index = 1;
                }
                else if (Image_Index > Image_Frame)
                {
                    Image_Index = Image_Frame;
                }
                else
                {
                    Image_Index = Image_Index;
                }

                this.MasterPC_TabPage_Data_TextBox_Page.Text = Image_Index.ToString() + "/" + Image_Frame.ToString();//列表页数刷新
                Image_Draw(Image_Index - 1);//绘制图形
                Parameter_Draw(Image_Index - 1);//绘制实时曲线
                Curve_Frame_Draw(Image_Index - 1);//绘制曲线
            }
        }

        private void Timer_Timing_ImagePlay_Tick(object sender, EventArgs e)//图形帧定时器
        {
            if (Image_Play_Order == 0)//连续播放
            {
                Image_Index += 1;

                if (Image_Index < 1)
                {
                    Image_Index = 1;
                }
                else if (Image_Index > Image_Frame)
                {
                    Image_Index = Image_Frame;
                }
                else
                {
                    Image_Index = Image_Index;
                }

                if (Image_Index == Image_Frame)//到达最后一帧,关闭播放
                {
                    this.Timer_Timing_ImagePlay.Stop();//停止计时
                    this.Timer_Timing_ImagePlay.Enabled = false;//定时器关闭
                    this.MasterPC_TabPage_Data_Button_Downing.Text = "连续";
                    this.MasterPC_TabPage_Data_Button_Uping.Enabled = true;//开启后退按钮
                }

                this.MasterPC_TabPage_Data_TextBox_Page.Text = Image_Index.ToString() + "/" + Image_Frame.ToString();//列表页数刷新
                Image_Draw(Image_Index - 1);//绘制图形
                Parameter_Draw(Image_Index - 1);//绘制实时曲线
                Curve_Frame_Draw(Image_Index - 1);//绘制曲线
            }
            else if (Image_Play_Order == 1)//后退播放
            {
                Image_Index -= 1;

                if (Image_Index < 1)
                {
                    Image_Index = 1;
                }
                else if (Image_Index > Image_Frame)
                {
                    Image_Index = Image_Frame;
                }
                else
                {
                    Image_Index = Image_Index;
                }

                if (Image_Index == 1)//到达第一帧,关闭播放
                {
                    this.Timer_Timing_ImagePlay.Stop();//停止计时
                    this.Timer_Timing_ImagePlay.Enabled = false;//定时器关闭
                    this.MasterPC_TabPage_Data_Button_Uping.Text = "后退";
                    this.MasterPC_TabPage_Data_Button_Downing.Enabled = true;//开启连续按钮
                }

                this.MasterPC_TabPage_Data_TextBox_Page.Text = Image_Index.ToString() + "/" + Image_Frame.ToString();//列表页数刷新
                Image_Draw(Image_Index - 1);//绘制图形
                Parameter_Draw(Image_Index - 1);//绘制实时曲线
                Curve_Frame_Draw(Image_Index - 1);//绘制曲线
            }
        }

        #endregion

        #region 图形数据分帧
        private void File_Data_Calculate_Frame(List<byte> File_Data_Array_List)//图形数据分帧
        {
            int Frame_Count = 0;//帧计数

            try
            {
                //图形数据分帧
                for (int i = 0; i < File_Data_Array_List.Count - 5; i++)
                {
                    //搜索"AAAA"针头
                    if (File_Data_Array_List[i] == 65 && File_Data_Array_List[i + 1] == 65
                       && File_Data_Array_List[i + 2] == 65 && File_Data_Array_List[i + 3] == 65
                        && File_Data_Array_List[i + 4] != 65 && File_Data_Array_List[i + 5] != 65)//寻找"AAAA"帧头
                    {
                        Frame_Count++;//帧计数加一
                        File_Data_List_Index_First.Add(i);//列表添加帧计数
                        Image_Create();//图形库创建
                        Curve_Handle(i, File_Data_Array_List);//曲线数据预处理

                    }
                }

                if (Frame_Count > 0)//判断图形数据分帧符合要求
                {
                    Image_Index = 1;//当前图形帧
                    Image_Frame = Frame_Count;//图形总帧数
                    this.MasterPC_TabPage_Data_TextBox_Page.Text = "1/" + Frame_Count.ToString();//列表页数刷新
                    this.MasterPC_TabPage_Data_NumberUpDown_Jump.Maximum = Frame_Count;//暂定最大帧数1000
                    this.MasterPC_TabPage_Data_NumberUpDown_Jump.Minimum = 1;//暂定最小帧数0
                    this.MasterPC_TabPage_Data_NumberUpDown_Jump.Value = 1;//暂定当前帧数0
                }
                else//图形数据分帧不符合要求
                {
                    MessageBox.Show("图形数据文件格式不符合要求!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception)
            {
                //提示分帧错误
                MessageBox.Show("图形数据文件处理发生错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 图形库创建
        private void Image_Create()//图形库创建
        {
            Bitmap Bitmap_New = new Bitmap(this.MasterPC_TabPage_Data_PictureBox_Image.Width, this.MasterPC_TabPage_Data_PictureBox_Image.Height);//图形库创建
            File_Data_BitmapList.Add(Bitmap_New);//添加图形
            File_Data_List_ImageFlag.Add(false);//添加图形标志
        }

        #endregion

        #region 图形帧绘制
        private void Image_Draw(int Index)//图形帧绘制
        {
            int FrameCount = 0;//图形帧计数
            int Number = 0;//图形跳变点计数
            int Point = 0;//图形数据点
            int Last = 0;//图形上次数据点
            int Line1 = 0;
            int Line2 = 0;
            int Line3 = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int x = 0;
            int y = 0;

            Image_Line_Now = 0;//当前行清零
            Point = 0;//当前跳变点清零
            Last = 0;//上次跳变点清零

            try
            {
                //绘制图形帧
                if (File_Data_List_ImageFlag[Index] == false)
                {
                    this.MasterPC_TabPage_Data_PictureBox_Image_Orige.Width = Image_Col;
                    this.MasterPC_TabPage_Data_PictureBox_Image_Orige.Height = Image_Row * 2;
                    Bitmap Bitmap_New = new Bitmap(this.MasterPC_TabPage_Data_PictureBox_Image_Orige.Width, this.MasterPC_TabPage_Data_PictureBox_Image_Orige.Height);//创建图形库
                    for (FrameCount = File_Data_List_Index_First[Index] + 4; FrameCount < File_Data_List_Index_First[Index + 1]; FrameCount++)
                    {
                        if (Image_Line_Now < Image_Row)//当前行号小于最大行号(图形绘制)
                        {
                            Number++;//跳变点加一
                            /*if (Number >= Image_Jump)//跳变点数量满足
                            {
                                for (; Point < Image_Col; Point++)
                                {
                                    if (Image_Color_Now == 1)//当前点为黑色
                                    {
                                        for (x = 0; x < Image_Cell_Width; x++)
                                        {
                                            for (y = 0; y < Image_Cell_Height; y++)
                                            {
                                                Bitmap_New.SetPixel(Image_Cell_Width * Point + x, Image_Cell_Height * Image_Line_Now + y, Color.Black);
                                            }
                                        }
                                    }
                                    else if (Image_Color_Now == 0)//当前点为白色
                                    {
                                        for (x = 0; x < Image_Cell_Width; x++)
                                        {
                                            for (y = 0; y < Image_Cell_Height; y++)
                                            {
                                                Bitmap_New.SetPixel(Image_Cell_Width * Point + x, Image_Cell_Height * Image_Line_Now + y, Color.White);
                                            }
                                        }
                                    }
                                }
                                Number = 0;//跳变点清除
                                Point = 0;
                                Last = 0;
                                Image_Color_Now = 1;
                                Image_Line_Now++;
                            }*/

                            for (Point = Last; Point < File_Data_List[FrameCount]; Point++)//当前跳变点
                            {
                                if (Image_Color_Now == 1)//当前点为黑色
                                {
                                    for (x = 0; x < Image_Cell_Width; x++)
                                    {
                                        for (y = 0; y < Image_Cell_Height; y++)
                                        {
                                            Bitmap_New.SetPixel(Image_Cell_Width * Point + x, Image_Cell_Height * Image_Line_Now + y, Color.Black);
                                        }
                                    }
                                }
                                else if (Image_Color_Now == 0)//当前点为白色
                                {
                                    for (x = 0; x < Image_Cell_Width; x++)
                                    {
                                        for (y = 0; y < Image_Cell_Height; y++)
                                        {
                                            Bitmap_New.SetPixel(Image_Cell_Width * Point + x, Image_Cell_Height * Image_Line_Now + y, Color.White);
                                        }
                                    }
                                }
                            }

                            if (File_Data_List[FrameCount] != Image_Col && Number < Image_Jump)
                            {
                                Image_Color_Now = 1 - Image_Color_Now;//变化颜色
                                Last = Point;//记录上次跳变
                            }
                            else
                            {
                                FrameCount += Image_Jump - Number;
                                Number = 0;//跳变点清除
                                Point = 0;
                                Last = 0;
                                Image_Color_Now = 1;
                                Image_Line_Now++;
                            }
                        }
                        else
                        {
                            /*for (k = 0; k < Image_Row; k++)//曲线绘制
                            {
                                Line1 = File_Data_List[FrameCount + k] * 256 + File_Data_List[FrameCount + Image_Row + k];
                                Line2 = File_Data_List[FrameCount + Image_Row * 2 + k] * 256 + File_Data_List[FrameCount + Image_Row * 2 + Image_Row + k];
                                Line3 = File_Data_List[FrameCount + Image_Row * 4 + k] * 256 + File_Data_List[FrameCount + Image_Row * 4 + Image_Row + k];

                                for (i = 0; i < Image_Cell_Width; i++)
                                {
                                    for (j = 0; j < Image_Cell_Height; j++)
                                    {
                                        Bitmap_New.SetPixel(Image_Cell_Width * Line1 + i, Image_Cell_Height * k + j, Color.LightCoral);
                                        Bitmap_New.SetPixel(Image_Cell_Width * Line2 + i, Image_Cell_Height * k + j, Color.LightSkyBlue);
                                        Bitmap_New.SetPixel(Image_Cell_Width * Line3 + i, Image_Cell_Height * k + j, Color.LightGreen);
                                    }
                                }
                            }*/
                            break;
                        }
                    }

                    //this.MasterPC_TabPage_Data_PictureBox_Image.BackgroundImage = Bitmap_New;

                    Bitmap Bitmap_PictureBox = new Bitmap(Bitmap_New, this.MasterPC_TabPage_Data_PictureBox_Image.Width, this.MasterPC_TabPage_Data_PictureBox_Image.Height);//创建图形库
                    File_Data_BitmapList[Index] = Bitmap_PictureBox;
                    File_Data_List_ImageFlag[Index] = true;
                }
                this.MasterPC_TabPage_Data_PictureBox_Image.BackgroundImage = File_Data_BitmapList[Index];
            }

            catch(Exception)
            {
 
            }

        }

        #endregion

        #region 曲线预处理
        private void Curve_Handle(int Number,List<byte> File_Data_Array_List)//曲线预处理
        {
            int Curve_Data = 0;//曲线数据

            try
            {
                //变量参数添加数据
                //曲线1链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 7 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 7];
                Curve_Data_List_1.Add(Curve_Data);
                //曲线2链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 8 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 8];
                Curve_Data_List_2.Add(Curve_Data);
                //曲线3链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 9 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 9];
                Curve_Data_List_3.Add(Curve_Data);
                //曲线4链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 10 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 10];
                Curve_Data_List_4.Add(Curve_Data);
                //曲线5链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 11 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 11];
                Curve_Data_List_5.Add(Curve_Data);
                //曲线6链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 12 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 12];
                Curve_Data_List_6.Add(Curve_Data);
                //曲线7链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 13 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 13];
                Curve_Data_List_7.Add(Curve_Data);
                //曲线8链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 14 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 14];
                Curve_Data_List_8.Add(Curve_Data);
                //曲线9链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 15 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 15];
                Curve_Data_List_9.Add(Curve_Data);

                //控制参数添加数据
                //曲线10链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 16 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 16];
                Curve_Data_List_10.Add(Curve_Data);
                //曲线11链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 17 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 17];
                Curve_Data_List_11.Add(Curve_Data);
                //曲线12链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 18 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 18];
                Curve_Data_List_12.Add(Curve_Data);
                //曲线13链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 19 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 19];
                Curve_Data_List_13.Add(Curve_Data);
                //曲线14链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 20 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 20];
                Curve_Data_List_14.Add(Curve_Data);
                //曲线15链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 21 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 21];
                Curve_Data_List_15.Add(Curve_Data);
                //曲线16链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 22 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 22];
                Curve_Data_List_16.Add(Curve_Data);
                //曲线17链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 23 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 23];
                Curve_Data_List_17.Add(Curve_Data);
                //曲线18链表添加数据
                Curve_Data = File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 24 - 20] * 256 + File_Data_Array_List[Number + 4 + (Image_Row + 27 - 2) * Image_Jump + 24];
                Curve_Data_List_18.Add(Curve_Data);

            }
            catch (Exception)
            {
                //添加数据错误
                MessageBox.Show("曲线添加数据发生错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #endregion

        #region 曲线清除
        private void Curve_Clear()//曲线数据清除
        {
            Curve_Data_List_1.Clear();//曲线1链表清除
            Curve_Data_List_2.Clear();//曲线2链表清除
            Curve_Data_List_3.Clear();//曲线3链表清除
            Curve_Data_List_4.Clear();//曲线4链表清除
            Curve_Data_List_5.Clear();//曲线5链表清除
            Curve_Data_List_6.Clear();//曲线6链表清除
            Curve_Data_List_7.Clear();//曲线7链表清除
            Curve_Data_List_8.Clear();//曲线8链表清除
            Curve_Data_List_9.Clear();//曲线9链表清除
            Curve_Data_List_10.Clear();//曲线10链表清除
            Curve_Data_List_11.Clear();//曲线11链表清除
            Curve_Data_List_12.Clear();//曲线12链表清除
            Curve_Data_List_13.Clear();//曲线13链表清除
            Curve_Data_List_14.Clear();//曲线14链表清除
            Curve_Data_List_15.Clear();//曲线15链表清除
            Curve_Data_List_16.Clear();//曲线16链表清除
            Curve_Data_List_17.Clear();//曲线17链表清除
            Curve_Data_List_18.Clear();//曲线18链表清除
        }

        #endregion

        #region 曲线绘制
        private void Curve_Draw()//曲线绘制
        {
            this.MasterPC_TabPage_Data_Chart_Chart.Series[0].Points.DataBindY(Curve_Data_List_1);//曲线1绘制
            this.MasterPC_TabPage_Data_Chart_Chart.Series[1].Points.DataBindY(Curve_Data_List_2);//曲线2绘制
            this.MasterPC_TabPage_Data_Chart_Chart.Series[2].Points.DataBindY(Curve_Data_List_3);//曲线3绘制
            this.MasterPC_TabPage_Data_Chart_Chart.Series[3].Points.DataBindY(Curve_Data_List_4);//曲线4绘制
            this.MasterPC_TabPage_Data_Chart_Chart.Series[4].Points.DataBindY(Curve_Data_List_5);//曲线5绘制
            this.MasterPC_TabPage_Data_Chart_Chart.Series[5].Points.DataBindY(Curve_Data_List_6);//曲线6绘制
            this.MasterPC_TabPage_Data_Chart_Chart.Series[6].Points.DataBindY(Curve_Data_List_7);//曲线7绘制
            this.MasterPC_TabPage_Data_Chart_Chart.Series[7].Points.DataBindY(Curve_Data_List_8);//曲线8绘制
            this.MasterPC_TabPage_Data_Chart_Chart.Series[8].Points.DataBindY(Curve_Data_List_9);//曲线9绘制
        }

        #endregion

        #region 曲线帧绘制
        private void Curve_Frame_Draw(int Index)//曲线帧绘制
        {
            if (Index < 50)
            {
                this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisX.Minimum = 0;//ChartX轴最小值
                this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisX.Maximum = 100;//ChartX轴最大值
                this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].CursorX.Position = Index + 1;//游标显示
            }
            else if (Index + 50 > Image_Frame)
            {
                this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisX.Minimum = Image_Frame - 100;//ChartX轴最小值
                this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisX.Maximum = Image_Frame;//ChartX轴最大值
                this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].CursorX.Position = Index;//游标显示
            }
            else
            {
                this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisX.Minimum = Index - 50;//ChartX轴最小值
                this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].AxisX.Maximum = Index + 50;//ChartX轴最大值
                this.MasterPC_TabPage_Data_Chart_Chart.ChartAreas[0].CursorX.Position = Index;//游标显示
            }
        }

        #endregion

        #region 曲线可视化
        private void MasterPC_TabPage_Data_TextBox_Value_Var1_Click(object sender, EventArgs e)//曲线1单击
        {
            if (this.MasterPC_TabPage_Data_TextBox_Value_Var1.BackColor == Color.LightCoral)//曲线1颜色是浅红色
            {
                this.MasterPC_TabPage_Data_TextBox_Var1.BackColor = Color.White;
                this.MasterPC_TabPage_Data_TextBox_Value_Var1.BackColor = Color.White;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[0].Enabled = false;
            }
            else//曲线1颜色是白色
            {
                this.MasterPC_TabPage_Data_TextBox_Var1.BackColor = Color.LightCoral;
                this.MasterPC_TabPage_Data_TextBox_Value_Var1.BackColor = Color.LightCoral;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[0].Enabled = true;
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var2_Click(object sender, EventArgs e)//曲线2单击
        {
            if (this.MasterPC_TabPage_Data_TextBox_Value_Var2.BackColor == Color.LightBlue)//曲线2颜色是浅红色
            {
                this.MasterPC_TabPage_Data_TextBox_Var2.BackColor = Color.White;
                this.MasterPC_TabPage_Data_TextBox_Value_Var2.BackColor = Color.White;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[1].Enabled = false;
            }
            else//曲线2颜色是白色
            {
                this.MasterPC_TabPage_Data_TextBox_Var2.BackColor = Color.LightBlue;
                this.MasterPC_TabPage_Data_TextBox_Value_Var2.BackColor = Color.LightBlue;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[1].Enabled = true;
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var3_Click(object sender, EventArgs e)//曲线3单击
        {
            if (this.MasterPC_TabPage_Data_TextBox_Value_Var3.BackColor == Color.Orange)//曲线3颜色是橙色
            {
                this.MasterPC_TabPage_Data_TextBox_Var3.BackColor = Color.White;
                this.MasterPC_TabPage_Data_TextBox_Value_Var3.BackColor = Color.White;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[2].Enabled = false;
            }
            else//曲线3颜色是白色
            {
                this.MasterPC_TabPage_Data_TextBox_Var3.BackColor = Color.Orange;
                this.MasterPC_TabPage_Data_TextBox_Value_Var3.BackColor = Color.Orange;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[2].Enabled = true;
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var4_Click(object sender, EventArgs e)//曲线4单击
        {
            if (this.MasterPC_TabPage_Data_TextBox_Value_Var4.BackColor == Color.LightGreen)//曲线4颜色是浅绿色
            {
                this.MasterPC_TabPage_Data_TextBox_Var4.BackColor = Color.White;
                this.MasterPC_TabPage_Data_TextBox_Value_Var4.BackColor = Color.White;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[3].Enabled = false;
            }
            else//曲线4颜色是白色
            {
                this.MasterPC_TabPage_Data_TextBox_Var4.BackColor = Color.LightGreen;
                this.MasterPC_TabPage_Data_TextBox_Value_Var4.BackColor = Color.LightGreen;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[3].Enabled = true;
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var5_Click(object sender, EventArgs e)//曲线5单击
        {
            if (this.MasterPC_TabPage_Data_TextBox_Value_Var5.BackColor == Color.LightSeaGreen)//曲线5颜色是海绿色
            {
                this.MasterPC_TabPage_Data_TextBox_Var5.BackColor = Color.White;
                this.MasterPC_TabPage_Data_TextBox_Value_Var5.BackColor = Color.White;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[4].Enabled = false;
            }
            else//曲线5颜色是白色
            {
                this.MasterPC_TabPage_Data_TextBox_Var5.BackColor = Color.LightSeaGreen;
                this.MasterPC_TabPage_Data_TextBox_Value_Var5.BackColor = Color.LightSeaGreen;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[4].Enabled = true;
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var6_Click(object sender, EventArgs e)//曲线6单击
        {
            if (this.MasterPC_TabPage_Data_TextBox_Value_Var6.BackColor == Color.Gold)//曲线6颜色是金色
            {
                this.MasterPC_TabPage_Data_TextBox_Var6.BackColor = Color.White;
                this.MasterPC_TabPage_Data_TextBox_Value_Var6.BackColor = Color.White;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[5].Enabled = false;
            }
            else//曲线6颜色是白色
            {
                this.MasterPC_TabPage_Data_TextBox_Var6.BackColor = Color.Gold;
                this.MasterPC_TabPage_Data_TextBox_Value_Var6.BackColor = Color.Gold;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[5].Enabled = true;
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var7_Click(object sender, EventArgs e)//曲线7单击
        {
            if (this.MasterPC_TabPage_Data_TextBox_Value_Var7.BackColor == Color.LawnGreen)//曲线7颜色是草绿色
            {
                this.MasterPC_TabPage_Data_TextBox_Var7.BackColor = Color.White;
                this.MasterPC_TabPage_Data_TextBox_Value_Var7.BackColor = Color.White;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[6].Enabled = false;
            }
            else//曲线7颜色是白色
            {
                this.MasterPC_TabPage_Data_TextBox_Var7.BackColor = Color.LawnGreen;
                this.MasterPC_TabPage_Data_TextBox_Value_Var7.BackColor = Color.LawnGreen;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[6].Enabled = true;
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var8_Click(object sender, EventArgs e)//曲线8单击
        {
            if (this.MasterPC_TabPage_Data_TextBox_Value_Var8.BackColor == Color.Plum)//曲线8颜色是浅紫色
            {
                this.MasterPC_TabPage_Data_TextBox_Var8.BackColor = Color.White;
                this.MasterPC_TabPage_Data_TextBox_Value_Var8.BackColor = Color.White;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[7].Enabled = false;
            }
            else//曲线8颜色是白色
            {
                this.MasterPC_TabPage_Data_TextBox_Var8.BackColor = Color.Plum;
                this.MasterPC_TabPage_Data_TextBox_Value_Var8.BackColor = Color.Plum;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[7].Enabled = true;
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var9_Click(object sender, EventArgs e)//曲线9单击
        {
            if (this.MasterPC_TabPage_Data_TextBox_Value_Var9.BackColor == Color.LightPink)//曲线9颜色是浅粉色
            {
                this.MasterPC_TabPage_Data_TextBox_Var9.BackColor = Color.White;
                this.MasterPC_TabPage_Data_TextBox_Value_Var9.BackColor = Color.White;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[8].Enabled = false;
            }
            else//曲线9颜色是白色
            {
                this.MasterPC_TabPage_Data_TextBox_Var9.BackColor = Color.LightPink;
                this.MasterPC_TabPage_Data_TextBox_Value_Var9.BackColor = Color.LightPink;
                this.MasterPC_TabPage_Data_Chart_Chart.Series[8].Enabled = true;
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var1_MouseDown(object sender, MouseEventArgs e)//曲线1隐藏光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var2_MouseDown(object sender, MouseEventArgs e)//曲线2隐藏光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var3_MouseDown(object sender, MouseEventArgs e)//曲线3隐藏光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var4_MouseDown(object sender, MouseEventArgs e)//曲线4隐藏光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var5_MouseDown(object sender, MouseEventArgs e)//曲线5隐藏光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var6_MouseDown(object sender, MouseEventArgs e)//曲线6隐藏光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var7_MouseDown(object sender, MouseEventArgs e)//曲线7隐藏光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var8_MouseDown(object sender, MouseEventArgs e)//曲线8隐藏光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Value_Var9_MouseDown(object sender, MouseEventArgs e)//曲线9隐藏光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        #endregion

        #region 曲线数据提示
        private void MasterPC_TabPage_Data_Chart_Chart_GetToolTipText(object sender, ToolTipEventArgs e)//曲线坐标提示
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                System.Windows.Forms.DataVisualization.Charting.DataPoint dp = e.HitTestResult.Series.Points[i];
                e.Text = dp.YValues[0].ToString();
            }
        }

        #endregion

        #region 参数可视化
        private void MasterPC_TabPage_Data_TextBox_Angle_P_MouseDown(object sender, MouseEventArgs e)//控制参数1不显示光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Angle_D_MouseDown(object sender, MouseEventArgs e)//控制参数2不显示光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Speed_P_MouseDown(object sender, MouseEventArgs e)//控制参数3不显示光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Speed_I_MouseDown(object sender, MouseEventArgs e)//控制参数4不显示光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Direc_P_MouseDown(object sender, MouseEventArgs e)//控制参数5不显示光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Direc_D_MouseDown(object sender, MouseEventArgs e)//控制参数6不显示光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Cotrl_1_MouseDown(object sender, MouseEventArgs e)//控制参数7不显示光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Cotrl_2_MouseDown(object sender, MouseEventArgs e)//控制参数8不显示光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        private void MasterPC_TabPage_Data_TextBox_Cotrl_3_MouseDown(object sender, MouseEventArgs e)//控制参数9不显示光标
        {
            HideCaret((sender as TextBox).Handle);
        }

        #endregion

        #region 实时参数帧绘制
        private void Parameter_Draw(int Index)//实时参数帧绘制
        {
            //实时参数绘制
            this.MasterPC_TabPage_Data_TextBox_Value_Var1.Text = Curve_Data_List_1[Index].ToString();//曲线1实时参数
            this.MasterPC_TabPage_Data_TextBox_Value_Var2.Text = Curve_Data_List_2[Index].ToString();//曲线2实时参数
            this.MasterPC_TabPage_Data_TextBox_Value_Var3.Text = Curve_Data_List_3[Index].ToString();//曲线3实时参数
            this.MasterPC_TabPage_Data_TextBox_Value_Var4.Text = Curve_Data_List_4[Index].ToString();//曲线4实时参数
            this.MasterPC_TabPage_Data_TextBox_Value_Var5.Text = Curve_Data_List_5[Index].ToString();//曲线5实时参数
            this.MasterPC_TabPage_Data_TextBox_Value_Var6.Text = Curve_Data_List_6[Index].ToString();//曲线6实时参数
            this.MasterPC_TabPage_Data_TextBox_Value_Var7.Text = Curve_Data_List_7[Index].ToString();//曲线7实时参数
            this.MasterPC_TabPage_Data_TextBox_Value_Var8.Text = Curve_Data_List_8[Index].ToString();//曲线8实时参数
            this.MasterPC_TabPage_Data_TextBox_Value_Var9.Text = Curve_Data_List_9[Index].ToString();//曲线9实时参数

            //控制参数绘制
            this.MasterPC_TabPage_Data_TextBox_Angle_P.Text = Curve_Data_List_10[Index].ToString();//曲线10实时参数
            this.MasterPC_TabPage_Data_TextBox_Angle_D.Text = Curve_Data_List_11[Index].ToString();//曲线11实时参数
            this.MasterPC_TabPage_Data_TextBox_Speed_P.Text = Curve_Data_List_12[Index].ToString();//曲线12实时参数
            this.MasterPC_TabPage_Data_TextBox_Speed_I.Text = Curve_Data_List_13[Index].ToString();//曲线13实时参数
            this.MasterPC_TabPage_Data_TextBox_Direc_P.Text = Curve_Data_List_14[Index].ToString();//曲线14实时参数
            this.MasterPC_TabPage_Data_TextBox_Direc_D.Text = Curve_Data_List_15[Index].ToString();//曲线15实时参数
            this.MasterPC_TabPage_Data_TextBox_Cotrl_1.Text = Curve_Data_List_16[Index].ToString();//曲线16实时参数
            this.MasterPC_TabPage_Data_TextBox_Cotrl_2.Text = Curve_Data_List_17[Index].ToString();//曲线17实时参数
            this.MasterPC_TabPage_Data_TextBox_Cotrl_3.Text = Curve_Data_List_18[Index].ToString();//曲线18实时参数
        }

        #endregion

        #region 修改实时参数名称
        private void MasterPC_TabPage_Data_TextBox_Var1_KeyPress(object sender, KeyPressEventArgs e)//按键修改参数曲线1名称
        {
            if (e.KeyChar == (char)Keys.Enter)//按下回车键Enter
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项
                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Var1.Text.ToString() != "")//参数名称不为空
                    {
                        MyReg2.SetValue("Var1", this.MasterPC_TabPage_Data_TextBox_Var1.Text.ToString());
                        MessageBox.Show("参数名称已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else//参数名称为空
                    {
                        try
                        {
                            MyReg2.SetValue("Var1", MyReg2.GetValue("Var1"));
                        }
                        catch (Exception)
                        {
                            MyReg2.SetValue("Var1", "Var1");
                        }
                        MessageBox.Show("参数名称不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch(Exception)
                {
                    MyReg2.SetValue("Var1", "Var1");
                    MessageBox.Show("参数名称保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Var2_KeyPress(object sender, KeyPressEventArgs e)//按键修改参数曲线2名称
        {
            if (e.KeyChar == (char)Keys.Enter)//按下回车键Enter
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项
                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Var2.Text.ToString() != "")//参数名称不为空
                    {
                        MyReg2.SetValue("Var2", this.MasterPC_TabPage_Data_TextBox_Var2.Text.ToString());
                        MessageBox.Show("参数名称已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else//参数名称为空
                    {
                        try
                        {
                            MyReg2.SetValue("Var2", MyReg2.GetValue("Var2"));
                        }
                        catch (Exception)
                        {
                            MyReg2.SetValue("Var2", "Var2");
                        }
                        MessageBox.Show("参数名称不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception)
                {
                    MyReg2.SetValue("Var2", "Var2");
                    MessageBox.Show("参数名称保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Var3_KeyPress(object sender, KeyPressEventArgs e)//按键修改参数曲线3名称
        {
            if (e.KeyChar == (char)Keys.Enter)//按下回车键Enter
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项
                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Var3.Text.ToString() != "")//参数名称不为空
                    {
                        MyReg2.SetValue("Var3", this.MasterPC_TabPage_Data_TextBox_Var3.Text.ToString());
                        MessageBox.Show("参数名称已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else//参数名称为空
                    {
                        try
                        {
                            MyReg2.SetValue("Var3", MyReg2.GetValue("Var3"));
                        }
                        catch (Exception)
                        {
                            MyReg2.SetValue("Var3", "Var3");
                        }
                        MessageBox.Show("参数名称不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception)
                {
                    MyReg2.SetValue("Var3", "Var3");
                    MessageBox.Show("参数名称保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Var4_KeyPress(object sender, KeyPressEventArgs e)//按键修改参数曲线4名称
        {
            if (e.KeyChar == (char)Keys.Enter)//按下回车键Enter
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项
                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Var4.Text.ToString() != "")//参数名称不为空
                    {
                        MyReg2.SetValue("Var4", this.MasterPC_TabPage_Data_TextBox_Var4.Text.ToString());
                        MessageBox.Show("参数名称已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else//参数名称为空
                    {
                        try
                        {
                            MyReg2.SetValue("Var4", MyReg2.GetValue("Var4"));
                        }
                        catch (Exception)
                        {
                            MyReg2.SetValue("Var4", "Var4");
                        }
                        MessageBox.Show("参数名称不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception)
                {
                    MyReg2.SetValue("Var4", "Var4");
                    MessageBox.Show("参数名称保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Var5_KeyPress(object sender, KeyPressEventArgs e)//按键修改参数曲线5名称
        {
            if (e.KeyChar == (char)Keys.Enter)//按下回车键Enter
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项
                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Var5.Text.ToString() != "")//参数名称不为空
                    {
                        MyReg2.SetValue("Var5", this.MasterPC_TabPage_Data_TextBox_Var5.Text.ToString());
                        MessageBox.Show("参数名称已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else//参数名称为空
                    {
                        try
                        {
                            MyReg2.SetValue("Var5", MyReg2.GetValue("Var5"));
                        }
                        catch (Exception)
                        {
                            MyReg2.SetValue("Var5", "Var5");
                        }
                        MessageBox.Show("参数名称不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception)
                {
                    MyReg2.SetValue("Var5", "Var5");
                    MessageBox.Show("参数名称保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Var6_KeyPress(object sender, KeyPressEventArgs e)//按键修改参数曲线6名称
        {
            if (e.KeyChar == (char)Keys.Enter)//按下回车键Enter
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项
                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Var6.Text.ToString() != "")//参数名称不为空
                    {
                        MyReg2.SetValue("Var6", this.MasterPC_TabPage_Data_TextBox_Var6.Text.ToString());
                        MessageBox.Show("参数名称已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else//参数名称为空
                    {
                        try
                        {
                            MyReg2.SetValue("Var6", MyReg2.GetValue("Var6"));
                        }
                        catch (Exception)
                        {
                            MyReg2.SetValue("Var6", "Var6");
                        }
                        MessageBox.Show("参数名称不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception)
                {
                    MyReg2.SetValue("Var6", "Var6");
                    MessageBox.Show("参数名称保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Var7_KeyPress(object sender, KeyPressEventArgs e)//按键修改参数曲线7名称
        {
            if (e.KeyChar == (char)Keys.Enter)//按下回车键Enter
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项
                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Var7.Text.ToString() != "")//参数名称不为空
                    {
                        MyReg2.SetValue("Var7", this.MasterPC_TabPage_Data_TextBox_Var7.Text.ToString());
                        MessageBox.Show("参数名称已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else//参数名称为空
                    {
                        try
                        {
                            MyReg2.SetValue("Var7", MyReg2.GetValue("Var7"));
                        }
                        catch (Exception)
                        {
                            MyReg2.SetValue("Var7", "Var7");
                        }
                        MessageBox.Show("参数名称不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception)
                {
                    MyReg2.SetValue("Var7", "Var7");
                    MessageBox.Show("参数名称保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Var8_KeyPress(object sender, KeyPressEventArgs e)//按键修改参数曲线8名称
        {
            if (e.KeyChar == (char)Keys.Enter)//按下回车键Enter
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项
                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Var8.Text.ToString() != "")//参数名称不为空
                    {
                        MyReg2.SetValue("Var8", this.MasterPC_TabPage_Data_TextBox_Var8.Text.ToString());
                        MessageBox.Show("参数名称已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else//参数名称为空
                    {
                        try
                        {
                            MyReg2.SetValue("Var8", MyReg2.GetValue("Var8"));
                        }
                        catch (Exception)
                        {
                            MyReg2.SetValue("Var8", "Var8");
                        }
                        MessageBox.Show("参数名称不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception)
                {
                    MyReg2.SetValue("Var8", "Var8");
                    MessageBox.Show("参数名称保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MasterPC_TabPage_Data_TextBox_Var9_KeyPress(object sender, KeyPressEventArgs e)//按键修改参数曲线9名称
        {
            if (e.KeyChar == (char)Keys.Enter)//按下回车键Enter
            {
                RegistryKey MyReg1, MyReg2;//声明注册表对象

                MyReg1 = Registry.CurrentUser;//获取当前注册表项
                MyReg2 = MyReg1.CreateSubKey("Software\\MySoft");//在注册表项中创建子项
                try
                {
                    if (this.MasterPC_TabPage_Data_TextBox_Var8.Text.ToString() != "")//参数名称不为空
                    {
                        MyReg2.SetValue("Var9", this.MasterPC_TabPage_Data_TextBox_Var9.Text.ToString());
                        MessageBox.Show("参数名称已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else//参数名称为空
                    {
                        try
                        {
                            MyReg2.SetValue("Var9", MyReg2.GetValue("Var9"));
                        }
                        catch (Exception)
                        {
                            MyReg2.SetValue("Var9", "Var9");
                        }
                        MessageBox.Show("参数名称不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    
                }
                catch (Exception)
                {
                    MyReg2.SetValue("Var9", "Var9");
                    MessageBox.Show("参数名称保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #endregion

        #region 帮助
        private void MasterPC_TabPage_Help_ComboBox_Items_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.MasterPC_TabPage_Help_ComboBox_Items.SelectedIndex)
            {
                case 0:
                    this.MasterPC_TabPage_Help_TextBox_Help_SerialPort.Visible = true;
                    this.MasterPC_TabPage_Help_TextBox_Help_CCD.Visible = false;
                    break;
                case 1:
                    this.MasterPC_TabPage_Help_TextBox_Help_SerialPort.Visible = false;
                    this.MasterPC_TabPage_Help_TextBox_Help_CCD.Visible = true;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 关于
        #endregion

    }
}
