using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpDX.XInput;


/**
 * 
 * 
 * https://social.msdn.microsoft.com/Forums/vstudio/en-US/cf429cdf-146d-48c9-a8a2-cec9fc4b298a/sendinput-wont-work-when-compiled-with-x64-option?forum=csharpgeneral
 * 
 * */


public class MemoryRead
{


    const int PROCESS_WM_READ = 0x0010;
    const UInt32 WM_KEYDOWN = 0x1000;
    const int VK_F5 = 0x74;
    static int bytesRead2 = 0;
    static int bytesRead = 0;
    static byte[] buff;
    static byte[] buff2;
    static double divisor = 1000000;
    static Int64 baseA = 0x7FF60ED1E1B0;
    static double[,] FC_N_R = new double[14,58]; //Freecourt normal right side
    static double[,] FC_N_L = new double[14, 41]; //Freecourt normal right side
    static double upDown;
    static double rightLeft;
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    static extern short VkKeyScan(char ch);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);


    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(int hProcess, Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    [DllImport("user32.dll")]
    static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);



    [DllImport("user32.dll")]
    static extern IntPtr GetMessageExtraInfo();

    [DllImport("user32.dll",
    CallingConvention = CallingConvention.StdCall,
    CharSet = CharSet.Unicode,
    EntryPoint = "MapVirtualKey",
    SetLastError = true,
    ThrowOnUnmappableChar = false)]
    private static extern uint MapVirtualKey(
    uint uCode,
    uint uMapType);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct KEYBDINPUT
    {
        public ushort wVk;
        public short wScan;
        public uint dwFlags;
        public int time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public int mouseData;
        public uint dwFlags;
        public int time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct INPUT
    {
        [FieldOffset(0)]
        public uint type;
        [FieldOffset(8)]
        public MOUSEINPUT mi;
        [FieldOffset(8)]
        public KEYBDINPUT ki;
        [FieldOffset(8)]
        public HARDWAREINPUT hi;
    }

    [STAThread]
    public static void Main()
    {
        uint MAPVK_VK_TO_VSC = 0;

        XInputController x = new XInputController();
        sHook h = new sHook();
        Thread oThread = new Thread(new ThreadStart(x.Update));
        Thread oThread2 = new Thread(new ThreadStart(h.Update));
        Controller controller;
        Gamepad gamepad;
        controller = new Controller(UserIndex.One);
        bool connected = false;
        Keystroke xbutton;
        connected = controller.IsConnected;
        oThread2.Start();
        //h.Update();
        //uint ucode = (uint)Keys.A;
        //uint scancode = MapVirtualKey(ucode, MAPVK_VK_TO_VSC);
        //its doing backsapce
        setRefs();
        while (0 < 1)
        {
            //if (!connected)
            //{
            //    Console.WriteLine("error");
            //    Thread.Sleep(3000);
            //    return;
            //}

            gamepad = controller.GetState().Gamepad;
            //if (gamepad.Buttons == GamepadButtonFlags.A)
            //{
            //Console.WriteLine(Math.Abs((int)getTiming() + 15));
            //Console.WriteLine(getTimingA());
            //Thread.Sleep(5000);
            if (gamepad.Buttons == GamepadButtonFlags.RightThumb)
            //if(Keyboard.IsKeyDown(Key.))
            {

                Thread.Sleep(200);
                Send_Key(0x4C, 0 | 0x0008);
               
                //Thread.Sleep(Math.Abs((int)getTiming() + 15));
                Thread.Sleep((int)getTimingA());
                Send_Key(0x4C, 0x0002 | 0x0008);
            }





            //}

        }


    }

    public static double getTimingA()
    {
        Thread.Sleep(1);
        Console.Clear();
        Process process = Process.GetProcessesByName("NBA2K17")[0];
        IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
        bytesRead = 0;
        byte[] buffer = new byte[24]; //'Hello World!' takes 12*2 bytes because of Unicode 
        buff = new byte[8];

        bytesRead2 = 0;
        byte[] buffer2 = new byte[24]; //'Hello World!' takes 12*2 bytes because of Unicode 
        buff2 = new byte[8];

        try
        {
            ReadProcessMemory((int)processHandle, baseA - 0x10, buffer, buffer.Length, ref bytesRead);
            ReadProcessMemory((int)processHandle, baseA - 0x08, buffer2, buffer2.Length, ref bytesRead2);
        }
        catch (System.AccessViolationException e)
        {

        }
        for (int i = 0; i < 8; i++)
        {
            buff[i] = buffer[i];
            buff2[i] = buffer2[i];
        }

        rightLeft = BitConverter.ToInt64(buff, 0);
        upDown = BitConverter.ToInt32(buff2, 0);
        //b should be right left
        int a = (int)((rightLeft / divisor) - 3234);
        int aLeft = (int)((rightLeft / divisor) - 1105);
        int b = ((int) ((upDown / divisor)) - 1138);
       
        
     
        Console.WriteLine("x " + ((int)((upDown / divisor)) - 1138) + "  a " + a + " left " + aLeft);
        
        //Thread.Sleep(5000);
        if (b > 1 && a > 1 && a < 58 && a > 0)
        {
            Console.WriteLine("on right side ");
            return FC_N_R[b, a];
        }
        else if (b > 1 && aLeft < 39 && aLeft > 0 ) {
            Console.WriteLine("on left side ");
            return FC_N_L[b, aLeft];
        }
        return 0;
    }
    public static void setReftsLeft()
    {
        //14 
        //***************************
        FC_N_L[0, 14] = 545;;
        FC_N_L[1, 14] = 545;;
        FC_N_L[2, 14] = 530; 
        FC_N_L[3, 14] = 530;
        FC_N_L[4, 14] = 530;
        FC_N_L[5, 14] = 530;
        FC_N_L[6, 14] = 545;;
        FC_N_L[7, 14] = 545;;
        FC_N_L[8, 14] = 545;;
        FC_N_L[9, 14] = 545;;
        FC_N_L[10, 14] = 545;;
        FC_N_L[11, 14] = 545;;
        FC_N_L[12, 14] = 545;;
        FC_N_L[13, 14] = 545;;
        //*****************************


        //13 
        //***************************
        FC_N_L[0, 13] = 545;;
        FC_N_L[1, 13] = 545;;
        FC_N_L[2, 13] = 530;
        FC_N_L[3, 13] = 530;
        FC_N_L[4, 13] = 530;
        FC_N_L[5, 13] = 530;
        FC_N_L[6, 13] = 545;;
        FC_N_L[7, 13] = 545;;
        FC_N_L[8, 13] = 545;;
        FC_N_L[9, 13] = 545;;
        FC_N_L[10, 13] = 530;
        FC_N_L[11, 13] = 545;;
        FC_N_L[12, 13] = 545;;
        FC_N_L[13, 13] = 545;;
        //*****************************

        //12 
        //***************************
        FC_N_L[0, 12] = 545;
        FC_N_L[1, 12] = 545;
        FC_N_L[2, 12] = 530;
        FC_N_L[3, 12] = 530;
        FC_N_L[4, 12] = 530;
        FC_N_L[5, 12] = 530;
        FC_N_L[6, 12] = 545;
        FC_N_L[7, 12] = 545;;
        FC_N_L[8, 12] = 545;;
        FC_N_L[9, 12] = 545;;
        FC_N_L[10, 12] = 545;
        FC_N_L[11, 12] = 545;;
        FC_N_L[12, 12] = 545;;
        FC_N_L[13, 12] = 545;;
        //*****************************
        //11 
        //***************************
        FC_N_L[0, 11] = 545;
        FC_N_L[1, 11] = 545;
        FC_N_L[2, 11] = 530;
        FC_N_L[3, 11] = 530;
        FC_N_L[4, 11] = 530;
        FC_N_L[5, 11] = 530;
        FC_N_L[6, 11] = 545;
        FC_N_L[7, 11] = 545;;
        FC_N_L[8, 11] = 545;;
        FC_N_L[9, 11] = 545;;
        FC_N_L[10, 11] = 545;
        FC_N_L[11, 11] = 545;;
        FC_N_L[12, 11] = 545;;
        FC_N_L[13, 11] = 545;;
        //*****************************


        //10
        //***************************
        FC_N_L[0, 10] = 545;
        FC_N_L[1, 10] = 545;
        FC_N_L[2, 10] = 530;
        FC_N_L[3, 10] = 530;
        FC_N_L[4, 10] = 530;
        FC_N_L[5, 10] = 530;
        FC_N_L[6, 10] = 545;
        FC_N_L[7, 10] = 545;;
        FC_N_L[8, 10] = 545;;
        FC_N_L[9, 10] = 545;;
        FC_N_L[10, 10] = 545;
        FC_N_L[11, 10] = 545;;
        FC_N_L[12, 10] = 545;;
        FC_N_L[13, 10] = 545;;
        //*****************************

        //8
        //***************************
        FC_N_L[0, 8] = 545;
        FC_N_L[1, 8] = 545;
        FC_N_L[2, 8] = 530;
        FC_N_L[3, 8] = 530;
        FC_N_L[4, 8] = 530;
        FC_N_L[5, 8] = 530;
        FC_N_L[6, 8] = 545;
        FC_N_L[7, 8] = 545;;
        FC_N_L[8, 8] = 545;;
        FC_N_L[9, 8] = 545;;
        FC_N_L[10, 8] = 545;
        FC_N_L[11, 8] = 545;;
        FC_N_L[12, 8] = 545;;
        FC_N_L[13, 8] = 545;;
        //*****************************

        //
        //***************************
        FC_N_L[0, 3] = 545;
        FC_N_L[1, 3] = 545;
        FC_N_L[2, 3] = 530;
        FC_N_L[3, 3] = 530;
        FC_N_L[4, 3] = 530;
        FC_N_L[5, 3] = 530;
        FC_N_L[6, 3] = 545;
        //Check
        FC_N_L[7, 3] = 545;;
        //Check
        FC_N_L[8, 3] = 545;;
        //Check
        FC_N_L[9, 3] = 545;;
        FC_N_L[10, 3] = 545;
        FC_N_L[11, 3] = 545;
        FC_N_L[12, 3] = 545;
        FC_N_L[13, 3] = 545;
        //*****************************

        //
        //***************************
        FC_N_L[0, 4] = 545;
        FC_N_L[1, 4] = 545;
        FC_N_L[2, 4] = 530;
        FC_N_L[3, 4] = 530;
        FC_N_L[4, 4] = 530;
        FC_N_L[5, 4] = 530;
        FC_N_L[6, 4] = 545;
        FC_N_L[7, 4] = 545;;
        FC_N_L[8, 4] = 545;;
        FC_N_L[9, 4] = 545;;
        FC_N_L[10, 4] = 545;
        FC_N_L[11, 4] = 545;
        FC_N_L[12, 4] = 545;
        FC_N_L[13, 4] = 545;
        //*****************************

        //5 
        //***************************
        FC_N_L[0, 5] = 545;
        FC_N_L[1, 5] = 545;
        FC_N_L[2, 5] = 530;
        FC_N_L[3, 5] = 530;
        FC_N_L[4, 5] = 530;
        FC_N_L[5, 5] = 530;
        FC_N_L[6, 5] = 545;
        FC_N_L[7, 5] = 545;;
        FC_N_L[8, 5] = 545;;
        FC_N_L[9, 5] = 545;;
        FC_N_L[10, 5] = 545;
        FC_N_L[11, 5] = 545;
        FC_N_L[12, 5] = 545;
        FC_N_L[13, 5] = 545;
        //*****************************


        //6 
        //***************************
        FC_N_L[0, 6] = 545;
        FC_N_L[1, 6] = 545;
        FC_N_L[2, 6] = 530;
        FC_N_L[3, 6] = 530;
        FC_N_L[4, 6] = 530;
        FC_N_L[5, 6] = 530;
        FC_N_L[6, 6] = 545;
        FC_N_L[7, 6] = 545;;
        FC_N_L[8, 6] = 545;;
        FC_N_L[9, 6] = 545;;
        FC_N_L[10, 6] = 545;
        FC_N_L[11, 6] = 545;
        FC_N_L[12, 6] = 545;
        FC_N_L[13, 6] = 545;
        //*****************************

        //7 
        //***************************
        FC_N_L[0, 7] = 545;
        FC_N_L[1, 7] = 545;
        FC_N_L[2, 7] = 530;
        FC_N_L[3, 7] = 530;
        FC_N_L[4, 7] = 530;
        FC_N_L[5, 7] = 530;
        FC_N_L[6, 7] = 545;
        FC_N_L[7, 7] = 545;;
        FC_N_L[8, 7] = 545;;
        FC_N_L[9, 7] = 545;;
        FC_N_L[10, 7] = 545;
        FC_N_L[11, 7] = 545;
        FC_N_L[12, 7] = 545;
        FC_N_L[13, 7] = 545;
        //*****************************
        //9 
        //***************************
        FC_N_L[0, 9] = 545;
        FC_N_L[1, 9] = 545;
        FC_N_L[2, 9] = 530;
        FC_N_L[3, 9] = 530;
        FC_N_L[4, 9] = 530;
        FC_N_L[5, 9] = 530;
        FC_N_L[6, 9] = 545;
        FC_N_L[7, 9] = 545;;
        FC_N_L[8, 9] = 545;;
        FC_N_L[9, 9] = 545;;
        FC_N_L[10, 9] = 545;;
        FC_N_L[11, 9] = 545;
        FC_N_L[12, 9] = 545;
        FC_N_L[13, 9] = 545;
        //*****************************
        
        //2 
        //***************************
        FC_N_L[0, 2] = 545;
        FC_N_L[1, 2] = 545;
        FC_N_L[2, 2] = 545;
        FC_N_L[3, 2] = 545;
        FC_N_L[4, 2] = 545;
        FC_N_L[5, 2] = 545;
        FC_N_L[6, 2] = 545;
        FC_N_L[7, 2] = 545;;
        FC_N_L[8, 2] = 545;;
        FC_N_L[9, 2] = 545;;
        FC_N_L[10, 2] = 545;
        FC_N_L[11, 2] = 545;
        FC_N_L[12, 2] = 545;
        FC_N_L[13, 2] = 545;
        //*****************************

        //1 
        //***************************
        FC_N_L[0, 1] = 545;
        FC_N_L[1, 1] = 545;
        FC_N_L[2, 1] = 545;
        FC_N_L[3, 1] = 545;
        FC_N_L[4, 1] = 545;
        FC_N_L[5, 1] = 545;
        FC_N_L[6, 1] = 545;
        FC_N_L[7, 1] = 545;;
        FC_N_L[8, 1] = 545;;
        FC_N_L[9, 1] = 545;;
        FC_N_L[10, 1] = 10;
        FC_N_L[11, 1] = 545;
        FC_N_L[12, 1] = 545;
        FC_N_L[13, 1] = 545;
        //*****************************


        //0 
        //***************************
        FC_N_L[0, 0] = 545;
        FC_N_L[1, 0] = 545;
        FC_N_L[2, 0] = 545;
        FC_N_L[3, 0] = 545;
        FC_N_L[4, 0] = 545;
        FC_N_L[5, 0] = 545;
        FC_N_L[6, 0] = 545;
        FC_N_L[7, 0] = 545;;
        FC_N_L[8, 0] = 545;;
        FC_N_L[9, 0] = 545;;
        FC_N_L[10, 0] = 00;
        FC_N_L[11, 0] = 545;
        FC_N_L[12, 0] = 545;
        FC_N_L[13, 0] = 545;
        //*****************************

        //38 
        //***************************
        FC_N_L[0, 38] = 545;
        FC_N_L[1, 38] = 545;
        FC_N_L[2, 38] = 545;
        FC_N_L[3, 38] = 545;
        FC_N_L[4, 38] = 545;
        FC_N_L[5, 38] = 545;
        FC_N_L[6, 38] = 545;
        FC_N_L[7, 38] = 545;;
        FC_N_L[8, 38] = 545;;
        FC_N_L[9, 38] = 545;;
        FC_N_L[10, 38] = 545;;
        FC_N_L[11, 38] = 545;
        FC_N_L[12, 38] = 545;
        FC_N_L[13, 38] = 545;
        //*****************************
        //37 
        //***************************
        FC_N_L[0, 37] = 545;
        FC_N_L[1, 37] = 545;
        FC_N_L[2, 37] = 545;
        FC_N_L[3, 37] = 545;
        FC_N_L[4, 37] = 545;
        FC_N_L[5, 37] = 545;
        FC_N_L[6, 37] = 545;
        FC_N_L[7, 37] = 545;;
        FC_N_L[8, 37] = 545;;
        FC_N_L[9, 37] = 545;;
        FC_N_L[10, 37] = 545;
        FC_N_L[11, 37] = 545;
        FC_N_L[12, 37] = 545;
        FC_N_L[13, 37] = 545;
        //*****************************


        //36
        //***************************
        FC_N_L[0, 36] = 545;
        FC_N_L[1, 36] = 545;
        FC_N_L[2, 36] = 545;
        FC_N_L[3, 36] = 545;
        FC_N_L[4, 36] = 545;
        FC_N_L[5, 36] = 545;
        FC_N_L[6, 36] = 545;
        FC_N_L[7, 36] = 545;;
        FC_N_L[8, 36] = 545;;
        FC_N_L[9, 36] = 545;;
        FC_N_L[10, 36] = 545;
        FC_N_L[11, 36] = 545;
        FC_N_L[12, 36] = 545;
        FC_N_L[13, 36] = 545;
        //*****************************


        //35 
        //***************************
        FC_N_L[0, 35] = 545;
        FC_N_L[1, 35] = 545;
        FC_N_L[2, 35] = 530;
        FC_N_L[3, 35] = 530;
        FC_N_L[4, 35] = 530;
        FC_N_L[5, 35] = 530;
        FC_N_L[6, 35] = 545;
        FC_N_L[7, 35] = 545;;
        FC_N_L[8, 35] = 545;;
        FC_N_L[9, 35] = 545;;
        FC_N_L[10, 35] = 545;
        FC_N_L[11, 35] = 545;
        FC_N_L[12, 35] = 545;
        FC_N_L[13, 35] = 545;
        //*****************************


        //34 
        //***************************
        FC_N_L[0, 34] = 545;
        FC_N_L[1, 34] = 545;
        FC_N_L[2, 34] = 530;
        FC_N_L[3, 34] = 530;
        FC_N_L[4, 34] = 530;
        FC_N_L[5, 34] = 530;
        FC_N_L[6, 34] = 545;
        FC_N_L[7, 34] = 545;;
        FC_N_L[8, 34] = 545;;
        FC_N_L[9, 34] = 545;;
        FC_N_L[10, 34] = 545;
        FC_N_L[11, 34] = 545;
        FC_N_L[12, 34] = 545;
        FC_N_L[13, 34] = 545;
        //*****************************



        //33 [Done]
        //***************************
        FC_N_L[0, 33] = 545;
        FC_N_L[1, 33] = 545;
        FC_N_L[2, 33] = 545;
        FC_N_L[3, 33] = 545;
        FC_N_L[4, 33] = 545;
        FC_N_L[5, 33] = 545;
        FC_N_L[6, 33] = 545;
        FC_N_L[7, 33] = 545;
        FC_N_L[8, 33] = 545;
        FC_N_L[9, 33] = 545;
        FC_N_L[10, 33] = 545;
        FC_N_L[11, 33] = 545;
        FC_N_L[12, 33] = 545;
        FC_N_L[13, 33] = 545;
        //*****************************

        //32 
        //***************************
        FC_N_L[0, 32] = 545;
        FC_N_L[1, 32] = 545;
        FC_N_L[2, 32] = 545;
        FC_N_L[3, 32] = 545;
        FC_N_L[4, 32] = 545;
        FC_N_L[5, 32] = 545;
        FC_N_L[6, 32] = 545;
        FC_N_L[7, 32] = 545;
        FC_N_L[8, 32] = 545;
        FC_N_L[9, 32] = 545;
        FC_N_L[10, 32] = 545;
        FC_N_L[11, 32] = 545;
        FC_N_L[12, 32] = 545;
        FC_N_L[13, 32] = 545;
        //*****************************


        //31 
        //***************************
        FC_N_L[0, 31] = 545;
        FC_N_L[1, 31] = 545;
        FC_N_L[2, 31] = 545;
        FC_N_L[3, 31] = 545;
        FC_N_L[4, 31] = 545;
        FC_N_L[5, 31] = 545;
        FC_N_L[6, 31] = 545;
        FC_N_L[7, 31] = 545;
        FC_N_L[8, 31] = 545;
        FC_N_L[9, 31] = 545;
        FC_N_L[10, 31] = 545;
        FC_N_L[11, 31] = 545;
        FC_N_L[12, 31] = 545;
        FC_N_L[13, 31] = 545;
        //*****************************

        //30 
        //***************************
        FC_N_L[0, 30] = 545;
        FC_N_L[1, 30] = 545;
        FC_N_L[2, 30] = 545;
        FC_N_L[3, 30] = 545;
        FC_N_L[4, 30] = 545;
        FC_N_L[5, 30] = 545;
        FC_N_L[6, 30] = 545;
        FC_N_L[7, 30] = 545;
        FC_N_L[8, 30] = 545;
        FC_N_L[9, 30] = 545;
        FC_N_L[10, 30] = 545;
        FC_N_L[11, 30] = 545;
        FC_N_L[12, 30] = 545;
        FC_N_L[13, 30] = 545;
        //*****************************
        //29 
        //***************************
        FC_N_L[0, 29] = 545;
        FC_N_L[1, 29] = 545;
        FC_N_L[2, 29] = 545;
        FC_N_L[3, 29] = 545;
        FC_N_L[4, 29] = 545;
        FC_N_L[5, 29] = 545;
        FC_N_L[6, 29] = 545;
        FC_N_L[7, 29] = 545;
        FC_N_L[8, 29] = 545;
        FC_N_L[9, 29] = 545;
        FC_N_L[10, 29] = 545;
        FC_N_L[11, 29] = 545;
        FC_N_L[12, 29] = 545;
        FC_N_L[13, 29] = 545;
        //*****************************


        //28
        //***************************
        FC_N_L[0, 28] = 545;
        FC_N_L[1, 28] = 545;
        FC_N_L[2, 28] = 545;
        FC_N_L[3, 28] = 545;
        FC_N_L[4, 28] = 545;
        FC_N_L[5, 28] = 545;
        FC_N_L[6, 28] = 545;
        FC_N_L[7, 28] = 545;
        FC_N_L[8, 28] = 545;
        FC_N_L[9, 28] = 545;
        FC_N_L[10, 28] = 545;
        FC_N_L[11, 28] = 545;
        FC_N_L[12, 28] = 545;
        FC_N_L[13, 28] = 545;
        //*****************************

        //27
        //***************************
        FC_N_L[0, 27] = 545;
        FC_N_L[1, 27] = 545;
        FC_N_L[2, 27] = 545;
        FC_N_L[3, 27] = 545;
        FC_N_L[4, 27] = 545;
        FC_N_L[5, 27] = 545;
        FC_N_L[6, 27] = 545;
        FC_N_L[7, 27] = 545;
        FC_N_L[8, 27] = 545;
        FC_N_L[9, 27] = 545;
        FC_N_L[10, 27] = 545;
        FC_N_L[11, 27] = 545;
        FC_N_L[12, 27] = 545;
        FC_N_L[13, 27] = 545;
        //*****************************

        //
        //***************************
        FC_N_L[0, 21] = 545;
        FC_N_L[1, 21] = 545;
        FC_N_L[2, 21] = 545;
        FC_N_L[3, 21] = 545;
        FC_N_L[4, 21] = 545;
        FC_N_L[5, 21] = 545;
        FC_N_L[6, 21] = 545;
        //Check
        FC_N_L[7, 21] = 545;
        //Check
        FC_N_L[8, 21] = 545;
        //Check
        FC_N_L[9, 21] = 545;
        FC_N_L[10, 21] = 545;
        FC_N_L[11, 21] = 545;
        FC_N_L[12, 21] = 545;
        FC_N_L[13, 21] = 545;
        //*****************************

        //
        //***************************
        FC_N_L[0, 22] = 545;
        FC_N_L[1, 22] = 545;
        FC_N_L[2, 22] = 545;
        FC_N_L[3, 22] = 545;
        FC_N_L[4, 22] = 545;
        FC_N_L[5, 22] = 545;
        FC_N_L[6, 22] = 545;
        FC_N_L[7, 22] = 545;
        FC_N_L[8, 22] = 545;
        FC_N_L[9, 22] = 545;
        FC_N_L[10, 22] = 545;
        FC_N_L[11, 22] = 545;
        FC_N_L[12, 22] = 545;
        FC_N_L[13, 22] = 545;
        //*****************************

        //23 
        //***************************
        FC_N_L[0, 23] = 545;
        FC_N_L[1, 23] = 545;
        FC_N_L[2, 23] = 545;
        FC_N_L[3, 23] = 545;
        FC_N_L[4, 23] = 545;
        FC_N_L[5, 23] = 545;
        FC_N_L[6, 23] = 545;
        FC_N_L[7, 23] = 545;
        FC_N_L[8, 23] = 545;
        FC_N_L[9, 23] = 545;
        FC_N_L[10, 23] = 545;
        FC_N_L[11, 23] = 545;
        FC_N_L[12, 23] = 545;
        FC_N_L[13, 23] = 545;
        //*****************************


        //24 
        //***************************
        FC_N_L[0, 24] = 545;
        FC_N_L[1, 24] = 545;
        FC_N_L[2, 24] = 545;
        FC_N_L[3, 24] = 545;
        FC_N_L[4, 24] = 545;
        FC_N_L[5, 24] = 545;
        FC_N_L[6, 24] = 545;
        FC_N_L[7, 24] = 545;
        FC_N_L[8, 24] = 545;
        FC_N_L[9, 24] = 545;
        FC_N_L[10, 24] = 545;
        FC_N_L[11, 24] = 545;
        FC_N_L[12, 24] = 545;
        FC_N_L[13, 24] = 545;
        //*****************************

        //25 
        //***************************
        FC_N_L[0, 25] = 545;
        FC_N_L[1, 25] = 545;
        FC_N_L[2, 25] = 545;
        FC_N_L[3, 25] = 545;
        FC_N_L[4, 25] = 545;
        FC_N_L[5, 25] = 545;
        FC_N_L[6, 25] = 545;
        FC_N_L[7, 25] = 545;
        FC_N_L[8, 25] = 545;
        FC_N_L[9, 25] = 545;
        FC_N_L[10, 25] = 545;
        FC_N_L[11, 25] = 545;
        FC_N_L[12, 25] = 545;
        FC_N_L[13, 25] = 545;
        //*****************************
        //26 
        //***************************
        FC_N_L[0, 26] = 545;
        FC_N_L[1, 26] = 545;
        FC_N_L[2, 26] = 545;
        FC_N_L[3, 26] = 545;
        FC_N_L[4, 26] = 545;
        FC_N_L[5, 26] = 545;
        FC_N_L[6, 26] = 545;
        FC_N_L[7, 26] = 545;
        FC_N_L[8, 26] = 545;
        FC_N_L[9, 26] = 545;
        FC_N_L[10, 26] = 545;
        FC_N_L[11, 26] = 545;
        FC_N_L[12, 26] = 545;
        FC_N_L[13, 26] = 545;
        //*****************************

        //20
        //***************************
        FC_N_L[0, 20] = 545;
        FC_N_L[1, 20] = 545;
        FC_N_L[2, 20] = 545;
        FC_N_L[3, 20] = 545;
        FC_N_L[4, 20] = 545;
        FC_N_L[5, 20] = 545;
        FC_N_L[6, 20] = 545;
        //Check
        FC_N_L[7, 20] = 545;
        //Check
        FC_N_L[8, 20] = 545;
        //Check
        FC_N_L[9, 20] = 545;
        FC_N_L[10, 20] = 545;
        FC_N_L[11, 20] = 545;
        FC_N_L[12, 20] = 545;
        FC_N_L[13, 20] = 545;
        //*****************************

        //19 
        //***************************
        FC_N_L[0, 19] = 545;
        FC_N_L[1, 19] = 545;
        FC_N_L[2, 19] = 545;
        FC_N_L[3, 19] = 545;
        FC_N_L[4, 19] = 545;
        FC_N_L[5, 19] = 545;
        FC_N_L[6, 19] = 545;
        FC_N_L[7, 19] = 545;
        FC_N_L[8, 19] = 545;
        FC_N_L[9, 19] = 545;
        FC_N_L[10, 19] = 545;
        FC_N_L[11, 19] = 545;
        FC_N_L[12, 19] = 545;
        FC_N_L[13, 19] = 545;
        //*****************************

        //18 
        //***************************
        FC_N_L[0, 18] = 545;
        FC_N_L[1, 18] = 545;
        FC_N_L[2, 18] = 545;
        FC_N_L[3, 18] = 545;
        FC_N_L[4, 18] = 545;
        FC_N_L[5, 18] = 545;
        FC_N_L[6, 18] = 545;
        FC_N_L[7, 18] = 545;
        FC_N_L[8, 18] = 545;
        FC_N_L[9, 18] = 545;
        FC_N_L[10, 18] = 545;
        FC_N_L[11, 18] = 545;
        FC_N_L[12, 18] = 545;
        FC_N_L[13, 18] = 545;
        //*****************************


        //17 
        //***************************
        FC_N_L[0, 17] = 545;
        FC_N_L[1, 17] = 545;
        FC_N_L[2, 17] = 545;
        FC_N_L[3, 17] = 545;
        FC_N_L[4, 17] = 545;
        FC_N_L[5, 17] = 545;
        FC_N_L[6, 17] = 545;
        FC_N_L[7, 17] = 545;
        FC_N_L[8, 17] = 545;
        FC_N_L[9, 17] = 545;
        FC_N_L[10, 17] = 545;
        FC_N_L[11, 17] = 545;
        FC_N_L[12, 17] = 545;
        FC_N_L[13, 17] = 545;
        //*****************************

        //16 
        //***************************
        FC_N_L[0, 16] = 545;
        FC_N_L[1, 16] = 545;
        FC_N_L[2, 16] = 545;
        FC_N_L[3, 16] = 545;
        FC_N_L[4, 16] = 545;
        FC_N_L[5, 16] = 545;
        FC_N_L[6, 16] = 545;
        FC_N_L[7, 16] = 545;
        FC_N_L[8, 16] = 545;
        FC_N_L[9, 16] = 545;
        FC_N_L[10, 16] = 545;
        FC_N_L[11, 16] = 545;
        FC_N_L[12, 16] = 545;
        FC_N_L[13, 16] = 545;
        //*****************************
        //15 
        //***************************
        FC_N_L[0, 15] = 545;
        FC_N_L[1, 15] = 545;
        FC_N_L[2, 15] = 545;
        FC_N_L[3, 15] = 545;
        FC_N_L[4, 15] = 545;
        FC_N_L[5, 15] = 545;
        FC_N_L[6, 15] = 545;
        FC_N_L[7, 15] = 545;
        FC_N_L[8, 15] = 545;
        FC_N_L[9, 15] = 545;
        FC_N_L[10, 15] = 545;
        FC_N_L[11, 15] = 545;
        FC_N_L[12, 15] = 545;
        FC_N_L[13, 15] = 545;

        FC_N_L[0, 39] = 545;
        FC_N_L[1, 39] = 545;
        FC_N_L[2, 39] = 545;
        FC_N_L[3, 39] = 545;
        FC_N_L[4, 39] = 545;
        FC_N_L[5, 39] = 545;
        FC_N_L[6, 39] = 545;
        FC_N_L[7, 39] = 545;
        FC_N_L[8, 39] = 545;
        FC_N_L[9, 39] = 545;
        FC_N_L[10, 39] = 545;;;
        FC_N_L[12, 39] = 545;
        FC_N_L[13, 39] = 545;
    }
    public static void setRefs()
    {

        //57 [Done[
        //***************************
        FC_N_R[0, 57] = 517;
        FC_N_R[1, 57] = 517;
        FC_N_R[2, 57] = 530;
        FC_N_R[3, 57] = 530;
        FC_N_R[4, 57] = 530;
        FC_N_R[5, 57] = 530;
        FC_N_R[6, 57] = 517;
        //Check
        FC_N_R[7, 57] = 545;;
        //1147 3292
        FC_N_R[8, 57] = 545;;
        //1148 3292
        FC_N_R[9, 57] = 545;;
        //CHECK
        FC_N_R[10, 57] = 550;
        //CHECK
        FC_N_R[11, 57] = 545;;
        //1150 3292
        FC_N_R[12, 57] = 545;;
        //1151 3292
        FC_N_R[13, 57] = 545;;
        //*****************************

        //56 [Done]
        //***************************
        FC_N_R[0, 56] = 517;
        FC_N_R[1, 56] = 517;
        FC_N_R[2, 56] = 530;
        FC_N_R[3, 56] = 530;
        FC_N_R[4, 56] = 530;
        FC_N_R[5, 56] = 530;
        FC_N_R[6, 56] = 517;
        //Check
        FC_N_R[7, 56] = 545;;
        //Check
        FC_N_R[8, 56] = 545;;
        //Check
        FC_N_R[9, 56] = 545;;
        FC_N_R[10, 56] = 550;
        FC_N_R[11, 56] = 545;;
        FC_N_R[12, 56] = 545;;
        FC_N_R[13, 56] = 545;;
        //*****************************

        //55 [Done]
        //***************************
        FC_N_R[0, 55] = 517;
        FC_N_R[1, 55] = 517;
        FC_N_R[2, 55] = 530;
        FC_N_R[3, 55] = 530;
        FC_N_R[4, 55] = 530;
        FC_N_R[5, 55] = 530;
        FC_N_R[6, 55] = 517;
        FC_N_R[7, 55] = 545;;
        FC_N_R[8, 55] = 545;;
        FC_N_R[9, 55] = 545;;
        FC_N_R[10, 55] = 550;
        FC_N_R[11, 55] = 545;;
        FC_N_R[12, 55] = 545;;
        FC_N_R[13, 55] = 545;;
        //*****************************

        //54 
        //***************************
        FC_N_R[0, 54] = 517;
        FC_N_R[1, 54] = 517;
        FC_N_R[2, 54] = 530;
        FC_N_R[3, 54] = 530;
        FC_N_R[4, 54] = 530;
        FC_N_R[5, 54] = 530;
        FC_N_R[6, 54] = 517;
        FC_N_R[7, 54] = 545;;
        FC_N_R[8, 54] = 545;;
        FC_N_R[9, 54] = 545;;
        FC_N_R[10, 54] = 540;
        FC_N_R[11, 54] = 545;;
        FC_N_R[12, 54] = 545;;
        FC_N_R[13, 54] = 545;;
        //*****************************


        //53 
        //***************************
        FC_N_R[0, 53] = 517;
        FC_N_R[1, 53] = 517;
        FC_N_R[2, 53] = 530;
        FC_N_R[3, 53] = 530;
        FC_N_R[4, 53] = 530;
        FC_N_R[5, 53] = 530;
        FC_N_R[6, 53] = 517;
        FC_N_R[7, 53] = 545;;
        FC_N_R[8, 53] = 545;;
        FC_N_R[9, 53] = 545;;
        FC_N_R[10, 53] = 530;
        FC_N_R[11, 53] = 545;;
        FC_N_R[12, 53] = 545;;
        FC_N_R[13, 53] = 545;;
        //*****************************

        //52 
        //***************************
        FC_N_R[0, 52] = 517;
        FC_N_R[1, 52] = 517;
        FC_N_R[2, 52] = 530;
        FC_N_R[3, 52] = 530;
        FC_N_R[4, 52] = 530;
        FC_N_R[5, 52] = 530;
        FC_N_R[6, 52] = 517;
        FC_N_R[7, 52] = 545;;
        FC_N_R[8, 52] = 545;;
        FC_N_R[9, 52] = 545;;
        FC_N_R[10, 52] = 5545;
        FC_N_R[11, 52] = 545;;
        FC_N_R[12, 52] = 545;;
        FC_N_R[13, 52] = 545;;
        //*****************************
        //51 
        //***************************
        FC_N_R[0, 51] = 517;
        FC_N_R[1, 51] = 517;
        FC_N_R[2, 51] = 530;
        FC_N_R[3, 51] = 530;
        FC_N_R[4, 51] = 530;
        FC_N_R[5, 51] = 530;
        FC_N_R[6, 51] = 517;
        FC_N_R[7, 51] = 545;;
        FC_N_R[8, 51] = 545;;
        FC_N_R[9, 51] = 545;;
        FC_N_R[10, 51] = 510;
        FC_N_R[11, 51] = 545;;
        FC_N_R[12, 51] = 545;;
        FC_N_R[13, 51] = 545;;
        //*****************************

      
        //50
        //***************************
        FC_N_R[0, 50] = 517;
        FC_N_R[1, 50] = 517;
        FC_N_R[2, 50] = 530;
        FC_N_R[3, 50] = 530;
        FC_N_R[4, 50] = 530;
        FC_N_R[5, 50] = 530;
        FC_N_R[6, 50] = 517;
        FC_N_R[7, 50] = 545;;
        FC_N_R[8, 50] = 545;;
        FC_N_R[9, 50] = 545;;
        FC_N_R[10, 50] = 550;
        FC_N_R[11, 50] = 545;;
        FC_N_R[12, 50] = 545;;
        FC_N_R[13, 50] = 545;;
        //*****************************

        //49
        //***************************
        FC_N_R[0, 49] = 517;
        FC_N_R[1, 49] = 517;
        FC_N_R[2, 49] = 530;
        FC_N_R[3, 49] = 530;
        FC_N_R[4, 49] = 530;
        FC_N_R[5, 49] = 530;
        FC_N_R[6, 49] = 517;
        FC_N_R[7, 49] = 545;;
        FC_N_R[8, 49] = 545;;
        FC_N_R[9, 49] = 545;;
        FC_N_R[10, 49] = 550;
        FC_N_R[11, 49] = 545;;
        FC_N_R[12, 49] = 545;;
        FC_N_R[13, 49] = 545;;
        //*****************************

        //
        //***************************
        FC_N_R[0, 43] = 487;
        FC_N_R[1, 43] = 487;
        FC_N_R[2, 43] = 530;
        FC_N_R[3, 43] = 530;
        FC_N_R[4, 43] = 530;
        FC_N_R[5, 43] = 530;
        FC_N_R[6, 43] = 487;
        //Check
        FC_N_R[7, 43] = 545;;
        //Check
        FC_N_R[8, 43] = 545;;
        //Check
        FC_N_R[9, 43] = 545;;
        FC_N_R[10, 43] = 545;;
        FC_N_R[11, 43] = 545;;
        FC_N_R[12, 43] = 545;;
        FC_N_R[13, 43] = 545;;
        //*****************************

        //
        //***************************
        FC_N_R[0, 44] = 487;
        FC_N_R[1, 44] = 487;
        FC_N_R[2, 44] = 530;
        FC_N_R[3, 44] = 530;
        FC_N_R[4, 44] = 530;
        FC_N_R[5, 44] = 530;
        FC_N_R[6, 44] = 487;
        FC_N_R[7, 44] = 545;;
        FC_N_R[8, 44] = 545;;
        FC_N_R[9, 44] = 545;;
        FC_N_R[10, 44] = 545;;
        FC_N_R[11, 44] = 545;;
        FC_N_R[12, 44] = 545;;
        FC_N_R[13, 44] = 545;;
        //*****************************

        //45 
        //***************************
        FC_N_R[0, 45] = 487;
        FC_N_R[1, 45] = 487;
        FC_N_R[2, 45] = 530;
        FC_N_R[3, 45] = 530;
        FC_N_R[4, 45] = 530;
        FC_N_R[5, 45] = 530;
        FC_N_R[6, 45] = 487;
        FC_N_R[7, 45] = 545;;
        FC_N_R[8, 45] = 545;;
        FC_N_R[9, 45] = 545;;
        FC_N_R[10, 45] = 545;;
        FC_N_R[11, 45] = 545;;
        FC_N_R[12, 45] = 545;;
        FC_N_R[13, 45] = 545;;
        //*****************************


        //46 
        //***************************
        FC_N_R[0, 46] = 487;
        FC_N_R[1, 46] = 487;
        FC_N_R[2, 46] = 530;
        FC_N_R[3, 46] = 530;
        FC_N_R[4, 46] = 530;
        FC_N_R[5, 46] = 530;
        FC_N_R[6, 46] = 487;
        FC_N_R[7, 46] = 545;;
        FC_N_R[8, 46] = 545;;
        FC_N_R[9, 46] = 545;;
        FC_N_R[10, 46] = 545;;
        FC_N_R[11, 46] = 545;;
        FC_N_R[12, 46] = 545;;
        FC_N_R[13, 46] = 545;;
        //*****************************

        //47 
        //***************************
        FC_N_R[0, 47] = 487;
        FC_N_R[1, 47] = 487;
        FC_N_R[2, 47] = 530;
        FC_N_R[3, 47] = 530;
        FC_N_R[4, 47] = 530;
        FC_N_R[5, 47] = 530;
        FC_N_R[6, 47] = 487;
        FC_N_R[7, 47] = 545;;
        FC_N_R[8, 47] = 545;;
        FC_N_R[9, 47] = 545;;
        FC_N_R[10, 47] = 545;;
        FC_N_R[11, 47] = 545;;
        FC_N_R[12, 47] = 545;;
        FC_N_R[13, 47] = 545;;
        //*****************************
        //48 
        //***************************
        FC_N_R[0, 48] = 487;
        FC_N_R[1, 48] = 487;
        FC_N_R[2, 48] = 530;
        FC_N_R[3, 48] = 530;
        FC_N_R[4, 48] = 530;
        FC_N_R[5, 48] = 530;
        FC_N_R[6, 48] = 487;
        FC_N_R[7, 48] = 545;;
        FC_N_R[8, 48] = 545;;
        FC_N_R[9, 48] = 545;;
        FC_N_R[10, 48] = 545;;
        FC_N_R[11, 48] = 545;;
        FC_N_R[12, 48] = 545;;
        FC_N_R[13, 48] = 545;;
        //*****************************

        //42
        //***************************
        FC_N_R[0, 42] = 545;
        FC_N_R[1, 42] = 545;
        FC_N_R[2, 42] = 545;
        FC_N_R[3, 42] = 545;
        FC_N_R[4, 42] = 545;
        FC_N_R[5, 42] = 545;
        FC_N_R[6, 42] = 545;
        //Check
        FC_N_R[7, 42] = 545;;
        //Check
        FC_N_R[8, 42] = 545;;
        //Check
        FC_N_R[9, 42] = 545;;
        FC_N_R[10, 42] = 545;;
        FC_N_R[11, 42] = 545;;
        FC_N_R[12, 42] = 545;;
        FC_N_R[13, 42] = 545;;
        //*****************************

        //41 
        //***************************
        FC_N_R[0, 41] = 545;
        FC_N_R[1, 41] = 545;
        FC_N_R[2, 41] = 545;
        FC_N_R[3, 41] = 545;
        FC_N_R[4, 41] = 545;
        FC_N_R[5, 41] = 545;
        FC_N_R[6, 41] = 545;
        FC_N_R[7, 41] = 545;;
        FC_N_R[8, 41] = 545;;
        FC_N_R[9, 41] = 545;;
        FC_N_R[10, 41] = 545;;
        FC_N_R[11, 41] = 545;;
        FC_N_R[12, 41] = 545;;
        FC_N_R[13, 41] = 545;;
        //*****************************

        //40 
        //***************************
        FC_N_R[0, 40] = 545;
        FC_N_R[1, 40] = 545;
        FC_N_R[2, 40] = 545;
        FC_N_R[3, 40] = 545;
        FC_N_R[4, 40] = 545;
        FC_N_R[5, 40] = 545;
        FC_N_R[6, 40] = 545;
        FC_N_R[7, 40] = 545;;
        FC_N_R[8, 40] = 545;;
        FC_N_R[9, 40] = 545;;
        FC_N_R[10, 40] = 545;;
        FC_N_R[11, 40] = 545;;
        FC_N_R[12, 40] = 545;;
        FC_N_R[13, 40] = 545;;
        //*****************************


        //39 
        //***************************
        FC_N_R[0, 39] = 545;
        FC_N_R[1, 39] = 545;
        FC_N_R[2, 39] = 545;
        FC_N_R[3, 39] = 545;
        FC_N_R[4, 39] = 545;
        FC_N_R[5, 39] = 545;
        FC_N_R[6, 39] = 545;
        FC_N_R[7, 39] = 545;;
        FC_N_R[8, 39] = 545;;
        FC_N_R[9, 39] = 545;;
        FC_N_R[10, 39] = 545;;
        FC_N_R[11, 39] = 545;;
        FC_N_R[12, 39] = 545;;
        FC_N_R[13, 39] = 545;;
        //*****************************

        //38 
        //***************************
        FC_N_R[0, 38] = 545;
        FC_N_R[1, 38] = 545;
        FC_N_R[2, 38] = 545;
        FC_N_R[3, 38] = 545;
        FC_N_R[4, 38] = 545;
        FC_N_R[5, 38] = 545;
        FC_N_R[6, 38] = 545;
        FC_N_R[7, 38] = 545;;
        FC_N_R[8, 38] = 545;;
        FC_N_R[9, 38] = 545;;
        FC_N_R[10, 38] = 545;;
        FC_N_R[11, 38] = 545;;
        FC_N_R[12, 38] = 545;;
        FC_N_R[13, 38] = 545;;
        //*****************************
        //37 
        //***************************
        FC_N_R[0, 37] = 545;
        FC_N_R[1, 37] = 545;
        FC_N_R[2, 37] = 545;
        FC_N_R[3, 37] = 545;
        FC_N_R[4, 37] = 545;
        FC_N_R[5, 37] = 545;
        FC_N_R[6, 37] = 545;
        FC_N_R[7, 37] = 545;;
        FC_N_R[8, 37] = 545;;
        FC_N_R[9, 37] = 545;;
        FC_N_R[10, 37] =545;
        FC_N_R[11, 37] = 545;;
        FC_N_R[12, 37] = 545;;
        FC_N_R[13, 37] = 545;;
        //*****************************


        //36
        //***************************
        FC_N_R[0, 36] = 545;
        FC_N_R[1, 36] = 545;
        FC_N_R[2, 36] = 545;
        FC_N_R[3, 36] = 545;
        FC_N_R[4, 36] = 545;
        FC_N_R[5, 36] = 545;
        FC_N_R[6, 36] = 545;
        FC_N_R[7, 36] = 545;;
        FC_N_R[8, 36] = 545;;
        FC_N_R[9, 36] = 545;;
        FC_N_R[10, 36] = 545;;
        FC_N_R[11, 36] = 545;;
        FC_N_R[12, 36] = 545;;
        FC_N_R[13, 36] = 545;;
        //*****************************


        //35 
        //***************************
        FC_N_R[0, 35] = 517;
        FC_N_R[1, 35] = 517;
        FC_N_R[2, 35] = 530;
        FC_N_R[3, 35] = 530;
        FC_N_R[4, 35] = 530;
        FC_N_R[5, 35] = 530;
        FC_N_R[6, 35] = 517;
        FC_N_R[7, 35] = 545;;
        FC_N_R[8, 35] = 545;;
        FC_N_R[9, 35] = 545;;
        FC_N_R[10, 35] = 350;
        FC_N_R[11, 35] = 345;
        FC_N_R[12, 35] = 345;
        FC_N_R[13, 35] = 345;
        //*****************************


        //34 
        //***************************
        FC_N_R[0, 34] = 517;
        FC_N_R[1, 34] = 517;
        FC_N_R[2, 34] = 530;
        FC_N_R[3, 34] = 530;
        FC_N_R[4, 34] = 530;
        FC_N_R[5, 34] = 530;
        FC_N_R[6, 34] = 517;
        FC_N_R[7, 34] = 545;;
        FC_N_R[8, 34] = 545;;
        FC_N_R[9, 34] = 545;;
        FC_N_R[10, 34] = 340;
        FC_N_R[11, 34] = 345;
        FC_N_R[12, 34] = 345;
        FC_N_R[13, 34] = 345;
        //*****************************

    }

    public static double getTiming()
    {
        //   if(BitConverter.ToInt64(buff, 0) > 3293000000 && BitConverter.ToInt32(buff2, 0) > 115545;0000)
        //    {
        //        //Console.WriteLine(GamepadButtonFlags.X);
        //       // Thread.Sleep(1000);
        //        return 530;
        //    }
        //    if (BitConverter.ToInt64(buff, 0) > 3285000000 && BitConverter.ToInt64(buff, 0) < 3288000000 && BitConverter.ToInt32(buff2, 0) > 11545;00000 && BitConverter.ToInt32(buff2, 0) < 11545;;0000)
        //    {
        //        Console.WriteLine(GamepadButtonFlags.X);
        //        // Thread.Sleep(1000);
        //        return 540;
        //    }
        //    if (BitConverter.ToInt64(buff, 0) > 3290000000 && BitConverter.ToInt64(buff, 0) < 3293000000 && BitConverter.ToInt32(buff2, 0) > 1146000000 && BitConverter.ToInt32(buff2, 0) < 1149000000)
        //    {
        //        Console.WriteLine(GamepadButtonFlags.X);
        //        // Thread.Sleep(1000);
        //        return 530;
        //    }
        if (((((double)BitConverter.ToInt64(buff, 0) / BitConverter.ToInt32(buff2, 0)) * 2.08)) * 100 > 0)
        {
            if (BitConverter.ToInt64(buff, 0) > 3200000000)
            {
                return ((((double)BitConverter.ToInt64(buff, 0) / BitConverter.ToInt32(buff2, 0)) * 2.08)) * 100;
            }
            return ((((double)BitConverter.ToInt64(buff, 0) * 3 / BitConverter.ToInt32(buff2, 0)) * 2)) * 100; ;
        }
        if (BitConverter.ToInt64(buff, 0) > 3200000000)
        {
            return ((((double)BitConverter.ToInt64(buff, 0) / BitConverter.ToInt32(buff2, 0)) * 2.08)) * 100;
        }
        return ((((double)BitConverter.ToInt32(buff2, 0) * 3 / BitConverter.ToInt64(buff, 0)) * 2)) * 100;
    }

    class XInputController
    {
        Controller controller;
        Gamepad gamepad;
        public bool connected = false;
        Keystroke xbutton;





        public XInputController()
        {
            controller = new Controller(UserIndex.One);
            connected = controller.IsConnected;
        }

        // Call this method to update all class values
        public void Update()
        {


            while (0 < 1)
            {
                if (!connected)
                {
                    Console.WriteLine("error");
                    Thread.Sleep(3000);
                    return;
                }

                gamepad = controller.GetState().Gamepad;
                if (gamepad.Buttons == GamepadButtonFlags.X)
                {

                    // Console.WriteLine(controller.GetKeystroke(DeviceQueryType.Gamepad, out xbutton));


                    Send_Key(0x4C, 0 | 0x0008);
                    Thread.Sleep(615);
                    //Console.WriteLine((short)0x4C);
                    Send_Key(0x4C, 0x0002 | 0x0008);
                    Thread.Sleep(100);

                }


            }
        }

    }

    public static void Send_Key(short Keycode, int dwFlag)

    {
        INPUT[] InputData = new INPUT[1];



        InputData[0].type = 1;    //keyboard input

        InputData[0].ki.wScan = (short)Keycode;

        InputData[0].ki.dwFlags = (uint)dwFlag;

        InputData[0].ki.time = 0;

        InputData[0].ki.dwExtraInfo = IntPtr.Zero;


        SendInput((uint)1, InputData, Marshal.SizeOf(typeof(INPUT)));


    }

    public class sHook
    {
        public int xPos;
        public Int64 yPos;
        public sHook()
        {


        }
        public void Update()
        {
            Process process = Process.GetProcessesByName("NBA2K17")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
            Controller controller;
            Gamepad gamepad;
            controller = new Controller(UserIndex.One);
            bool connected = false;
            Keystroke xbutton;
            connected = controller.IsConnected;
            bytesRead = 0;
            byte[] buffer = new byte[24]; //'Hello World!' takes 12*2 bytes because of Unicode 
            buff = new byte[8];

            bytesRead2 = 0;
            byte[] buffer2 = new byte[24]; //'Hello World!' takes 12*2 bytes because of Unicode 
            buff2 = new byte[8];
            // x.Update();
            setRefs();
            setReftsLeft();
            Thread.Sleep(100);
            Console.WriteLine(buffer2[0] + " (" + bytesRead2.ToString() + "bytes)");
            gamepad = controller.GetState().Gamepad;
            while (1 == 1)
            //
            { 
                Console.Clear();

                // 0x0046A3B8 is the address where I found the string, replace it with what you found
                try
                {
                    ReadProcessMemory((int)processHandle, baseA - 0x10, buffer, buffer.Length, ref bytesRead);
                    ReadProcessMemory((int)processHandle, baseA - 0x08, buffer2, buffer2.Length, ref bytesRead2);
                }
                catch (System.AccessViolationException e)
                {

                }
                for (int i = 0; i < 8; i++)
                {
                    buff[i] = buffer[i];
                    buff2[i] = buffer2[i];
                }
                Console.WriteLine(buffer2[0] + " (" + bytesRead2.ToString() + "bytes)");
                xPos = BitConverter.ToInt32(buff2, 0);
                yPos = BitConverter.ToInt64(buff, 0);
                Console.WriteLine("Up Down " + BitConverter.ToInt32(buff2, 0));
                Console.WriteLine(buffer[0] + " (" + bytesRead.ToString() + "bytes)");
                Console.WriteLine("Left Right " + BitConverter.ToInt64(buff, 0));

                Console.WriteLine(getTimingA());


                Thread.Sleep(500);
            }

        }
    }

}





