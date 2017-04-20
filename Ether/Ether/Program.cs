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
    static Int64 baseA = 0x7FF61B87A870;
    static double[,] FC_N_R = new double[14,58]; //Freecourt normal right side
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
        int b = ((int) ((upDown / divisor)) - 1138);
       
        
     
        Console.WriteLine("x " + ((int)((upDown / divisor)) - 1138) + " y" + (int)((rightLeft / divisor) - 3234));
        //Thread.Sleep(5000);
        if(b > 1 && a > 1 && a < 58)
        {
            return FC_N_R[b, a];
        }
        return 0;
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
        FC_N_R[7, 57] = 570;
        //1147 3292
        FC_N_R[8, 57] = 570;
        //1148 3292
        FC_N_R[9, 57] = 570;
        //CHECK
        FC_N_R[10, 57] = 550;
        //CHECK
        FC_N_R[11, 57] = 545;
        //1150 3292
        FC_N_R[12, 57] = 545;
        //1151 3292
        FC_N_R[13, 57] = 545;
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
        FC_N_R[7, 56] = 570;
        //Check
        FC_N_R[8, 56] = 570;
        //Check
        FC_N_R[9, 56] = 570;
        FC_N_R[10, 56] = 550;
        FC_N_R[11, 56] = 545;
        FC_N_R[12, 56] = 545;
        FC_N_R[13, 56] = 545;
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
        FC_N_R[7, 55] = 570;
        FC_N_R[8, 55] = 570;
        FC_N_R[9, 55] = 570;
        FC_N_R[10, 55] = 550;
        FC_N_R[11, 55] = 545;
        FC_N_R[12, 55] = 545;
        FC_N_R[13, 55] = 545;
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
        FC_N_R[7, 54] = 570;
        FC_N_R[8, 54] = 570;
        FC_N_R[9, 54] = 570;
        FC_N_R[10, 54] = 540;
        FC_N_R[11, 54] = 545;
        FC_N_R[12, 54] = 545;
        FC_N_R[13, 54] = 545;
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
        FC_N_R[7, 53] = 570;
        FC_N_R[8, 53] = 570;
        FC_N_R[9, 53] = 570;
        FC_N_R[10, 53] = 530;
        FC_N_R[11, 53] = 545;
        FC_N_R[12, 53] = 545;
        FC_N_R[13, 53] = 545;
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
        FC_N_R[7, 52] = 570;
        FC_N_R[8, 52] = 570;
        FC_N_R[9, 52] = 570;
        FC_N_R[10, 52] = 520;
        FC_N_R[11, 52] = 545;
        FC_N_R[12, 52] = 545;
        FC_N_R[13, 52] = 545;
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
        FC_N_R[7, 51] = 570;
        FC_N_R[8, 51] = 570;
        FC_N_R[9, 51] = 570;
        FC_N_R[10, 51] = 510;
        FC_N_R[11, 51] = 545;
        FC_N_R[12, 51] = 545;
        FC_N_R[13, 51] = 545;
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
        FC_N_R[7, 50] = 570;
        FC_N_R[8, 50] = 570;
        FC_N_R[9, 50] = 570;
        FC_N_R[10, 50] = 550;
        FC_N_R[11, 50] = 545;
        FC_N_R[12, 50] = 545;
        FC_N_R[13, 50] = 545;
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
        FC_N_R[7, 49] = 570;
        FC_N_R[8, 49] = 570;
        FC_N_R[9, 49] = 570;
        FC_N_R[10, 49] = 550;
        FC_N_R[11, 49] = 545;
        FC_N_R[12, 49] = 545;
        FC_N_R[13, 49] = 545;
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
        FC_N_R[7, 43] = 570;
        //Check
        FC_N_R[8, 43] = 570;
        //Check
        FC_N_R[9, 43] = 570;
        FC_N_R[10, 43] = 459;
        FC_N_R[11, 43] = 444;
        FC_N_R[12, 43] = 444;
        FC_N_R[13, 43] = 444;
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
        FC_N_R[7, 44] = 570;
        FC_N_R[8, 44] = 570;
        FC_N_R[9, 44] = 570;
        FC_N_R[10, 44] = 459;
        FC_N_R[11, 44] = 444;
        FC_N_R[12, 44] = 444;
        FC_N_R[13, 44] = 444;
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
        FC_N_R[7, 45] = 570;
        FC_N_R[8, 45] = 570;
        FC_N_R[9, 45] = 570;
        FC_N_R[10, 45] = 436;
        FC_N_R[11, 45] = 444;
        FC_N_R[12, 45] = 444;
        FC_N_R[13, 45] = 444;
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
        FC_N_R[7, 46] = 570;
        FC_N_R[8, 46] = 570;
        FC_N_R[9, 46] = 570;
        FC_N_R[10, 46] = 460;
        FC_N_R[11, 46] = 444;
        FC_N_R[12, 46] = 444;
        FC_N_R[13, 46] = 444;
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
        FC_N_R[7, 47] = 570;
        FC_N_R[8, 47] = 570;
        FC_N_R[9, 47] = 570;
        FC_N_R[10, 47] = 470;
        FC_N_R[11, 47] = 444;
        FC_N_R[12, 47] = 444;
        FC_N_R[13, 47] = 444;
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
        FC_N_R[7, 48] = 570;
        FC_N_R[8, 48] = 570;
        FC_N_R[9, 48] = 570;
        FC_N_R[10, 48] = 480;
        FC_N_R[11, 48] = 444;
        FC_N_R[12, 48] = 444;
        FC_N_R[13, 48] = 444;
        //*****************************

        //42
        //***************************
        FC_N_R[0, 42] = 377;
        FC_N_R[1, 42] = 377;
        FC_N_R[2, 42] = 375;
        FC_N_R[3, 42] = 375;
        FC_N_R[4, 42] = 375;
        FC_N_R[5, 42] = 375;
        FC_N_R[6, 42] = 377;
        //Check
        FC_N_R[7, 42] = 570;
        //Check
        FC_N_R[8, 42] = 570;
        //Check
        FC_N_R[9, 42] = 570;
        FC_N_R[10, 42] = 410;
        FC_N_R[11, 42] = 405;
        FC_N_R[12, 42] = 405;
        FC_N_R[13, 42] = 405;
        //*****************************

        //41 
        //***************************
        FC_N_R[0, 41] = 377;
        FC_N_R[1, 41] = 377;
        FC_N_R[2, 41] = 375;
        FC_N_R[3, 41] = 375;
        FC_N_R[4, 41] = 375;
        FC_N_R[5, 41] = 375;
        FC_N_R[6, 41] = 377;
        FC_N_R[7, 41] = 570;
        FC_N_R[8, 41] = 570;
        FC_N_R[9, 41] = 570;
        FC_N_R[10, 41] = 410;
        FC_N_R[11, 41] = 405;
        FC_N_R[12, 41] = 405;
        FC_N_R[13, 41] = 405;
        //*****************************

        //40 
        //***************************
        FC_N_R[0, 40] = 377;
        FC_N_R[1, 40] = 377;
        FC_N_R[2, 40] = 375;
        FC_N_R[3, 40] = 375;
        FC_N_R[4, 40] = 375;
        FC_N_R[5, 40] = 375;
        FC_N_R[6, 40] = 377;
        FC_N_R[7, 40] = 570;
        FC_N_R[8, 40] = 570;
        FC_N_R[9, 40] = 570;
        FC_N_R[10, 40] = 400;
        FC_N_R[11, 40] = 405;
        FC_N_R[12, 40] = 405;
        FC_N_R[13, 40] = 405;
        //*****************************


        //39 
        //***************************
        FC_N_R[0, 39] = 377;
        FC_N_R[1, 39] = 377;
        FC_N_R[2, 39] = 375;
        FC_N_R[3, 39] = 375;
        FC_N_R[4, 39] = 375;
        FC_N_R[5, 39] = 375;
        FC_N_R[6, 39] = 377;
        FC_N_R[7, 39] = 570;
        FC_N_R[8, 39] = 570;
        FC_N_R[9, 39] = 570;
        FC_N_R[10, 39] = 390;
        FC_N_R[11, 39] = 405;
        FC_N_R[12, 39] = 405;
        FC_N_R[13, 39] = 405;
        //*****************************

        //38 
        //***************************
        FC_N_R[0, 38] = 377;
        FC_N_R[1, 38] = 377;
        FC_N_R[2, 38] = 375;
        FC_N_R[3, 38] = 375;
        FC_N_R[4, 38] = 375;
        FC_N_R[5, 38] = 375;
        FC_N_R[6, 38] = 377;
        FC_N_R[7, 38] = 570;
        FC_N_R[8, 38] = 570;
        FC_N_R[9, 38] = 570;
        FC_N_R[10, 38] = 380;
        FC_N_R[11, 38] = 405;
        FC_N_R[12, 38] = 405;
        FC_N_R[13, 38] = 405;
        //*****************************
        //37 
        //***************************
        FC_N_R[0, 37] = 377;
        FC_N_R[1, 37] = 377;
        FC_N_R[2, 37] = 375;
        FC_N_R[3, 37] = 375;
        FC_N_R[4, 37] = 375;
        FC_N_R[5, 37] = 375;
        FC_N_R[6, 37] = 377;
        FC_N_R[7, 37] = 570;
        FC_N_R[8, 37] = 570;
        FC_N_R[9, 37] = 570;
        FC_N_R[10, 37] = 370;
        FC_N_R[11, 37] = 405;
        FC_N_R[12, 37] = 405;
        FC_N_R[13, 37] = 405;
        //*****************************


        //36
        //***************************
        FC_N_R[0, 36] = 377;
        FC_N_R[1, 36] = 377;
        FC_N_R[2, 36] = 375;
        FC_N_R[3, 36] = 375;
        FC_N_R[4, 36] = 375;
        FC_N_R[5, 36] = 375;
        FC_N_R[6, 36] = 377;
        FC_N_R[7, 36] = 570;
        FC_N_R[8, 36] = 570;
        FC_N_R[9, 36] = 570;
        FC_N_R[10, 36] = 410;
        FC_N_R[11, 36] = 405;
        FC_N_R[12, 36] = 405;
        FC_N_R[13, 36] = 405;
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
        FC_N_R[7, 35] = 570;
        FC_N_R[8, 35] = 570;
        FC_N_R[9, 35] = 570;
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
        FC_N_R[7, 34] = 570;
        FC_N_R[8, 34] = 570;
        FC_N_R[9, 34] = 570;
        FC_N_R[10, 34] = 340;
        FC_N_R[11, 34] = 345;
        FC_N_R[12, 34] = 345;
        FC_N_R[13, 34] = 345;
        //*****************************

    }

    public static double getTiming()
    {
        //   if(BitConverter.ToInt64(buff, 0) > 3293000000 && BitConverter.ToInt32(buff2, 0) > 1154000000)
        //    {
        //        //Console.WriteLine(GamepadButtonFlags.X);
        //       // Thread.Sleep(1000);
        //        return 530;
        //    }
        //    if (BitConverter.ToInt64(buff, 0) > 3285000000 && BitConverter.ToInt64(buff, 0) < 3288000000 && BitConverter.ToInt32(buff2, 0) > 1141000000 && BitConverter.ToInt32(buff2, 0) < 1144000000)
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





