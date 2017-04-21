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
    static Int64 baseA = 0x7FF63E557400;
    static double[,] FC_N_R = new double[14,58]; //Freecourt normal right side
    static double[,] FC_N_L = new double[14, 41]; //Freecourt normal left side
    static double upDown;
    static double rightLeft;

    //Messed up here :p
    //static double[,] FC_B_R = new double[21, 47];
    static double[,] FC_B_R = new double[21, 12];
    static double[,] FC_B_L = new double[21, 35];
    static double upDownFlipped;
    static double rightLeftFlipped;
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
        int a;
        int aLeft;
        int b;
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
       a = (int)((rightLeft / divisor) - 3234);
        aLeft = (int)((rightLeft / divisor) - 1105);
        b = ((int) ((upDown / divisor)) - 1138);



        //Console.WriteLine("x " + ((int)((upDown / divisor)) - 1138) + "  a " + a + " left " + aLeft);
        
        if (((int)((upDown / divisor)) < 0)){
            //Were on the flipped side of the court
            a = (int)(Math.Abs(upDown / divisor) - 995);
            if ((int)((rightLeft / divisor)) < 3200)
            {
                //We're on the right side of the court
                b = ((int)(Math.Abs(upDown / divisor))-995);
                Console.WriteLine("on right side flipped");
                Console.WriteLine("  up to down " + a  + "  Left to Right " + b);
                if (a < 20 && a > 0 && b < 12 && b > 0)
                {
                    
                    return FC_B_R[a, b];
                }
            }
            else
            {
                //were on the left side of the court
                b = ((int)((rightLeft / divisor)) - 3258);
                Console.WriteLine("on left side flipped");
                if (a < 20 && a > 0 && b < 33 && b > 0)
                {
                    Console.WriteLine("b is " + b + " a is " + a);
                    return FC_B_L[a ,b];
                }
            }
            aLeft = (int)((rightLeft / divisor) - 1105);
            b = ((int)((upDown / divisor)) - 1138);
        }
        else
        {
            //Thread.Sleep(5000);
            if (b > 1 && b < 14 && a > 1 && a < 58 && a > 0)
            {
                Console.WriteLine("on right side ");
                return FC_N_R[b, a];
            }
            else if (b > 1 && b < 14 && aLeft < 39 && aLeft > 0)
            {
                Console.WriteLine("on left side ");
                return FC_N_L[b, aLeft];
            }
        }
        return 0;
    }
    public static void setReftsLeft()
    {
        //14 
        //***************************
        FC_N_L[0, 14] = 545;;
        FC_N_L[1, 14] = 545;;
        FC_N_L[2, 14] = 545; 
        FC_N_L[3, 14] = 545;
        FC_N_L[4, 14] = 545;
        FC_N_L[5, 14] = 545;
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
        FC_N_L[2, 13] = 545;
        FC_N_L[3, 13] = 545;
        FC_N_L[4, 13] = 545;
        FC_N_L[5, 13] = 545;
        FC_N_L[6, 13] = 545;;
        FC_N_L[7, 13] = 545;;
        FC_N_L[8, 13] = 545;;
        FC_N_L[9, 13] = 545;;
        FC_N_L[10, 13] = 545;
        FC_N_L[11, 13] = 545;;
        FC_N_L[12, 13] = 545;;
        FC_N_L[13, 13] = 545;;
        //*****************************

        //12 
        //***************************
        FC_N_L[0, 12] = 545;
        FC_N_L[1, 12] = 545;
        FC_N_L[2, 12] = 545;
        FC_N_L[3, 12] = 545;
        FC_N_L[4, 12] = 545;
        FC_N_L[5, 12] = 545;
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
        FC_N_L[2, 11] = 545;
        FC_N_L[3, 11] = 545;
        FC_N_L[4, 11] = 545;
        FC_N_L[5, 11] = 545;
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
        FC_N_L[2, 10] = 545;
        FC_N_L[3, 10] = 545;
        FC_N_L[4, 10] = 545;
        FC_N_L[5, 10] = 545;
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
        FC_N_L[2, 8] = 545;
        FC_N_L[3, 8] = 545;
        FC_N_L[4, 8] = 545;
        FC_N_L[5, 8] = 545;
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
        FC_N_L[2, 3] = 545;
        FC_N_L[3, 3] = 545;
        FC_N_L[4, 3] = 545;
        FC_N_L[5, 3] = 545;
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
        FC_N_L[2, 4] = 545;
        FC_N_L[3, 4] = 545;
        FC_N_L[4, 4] = 545;
        FC_N_L[5, 4] = 545;
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
        FC_N_L[2, 5] = 545;
        FC_N_L[3, 5] = 545;
        FC_N_L[4, 5] = 545;
        FC_N_L[5, 5] = 545;
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
        FC_N_L[2, 6] = 545;
        FC_N_L[3, 6] = 545;
        FC_N_L[4, 6] = 545;
        FC_N_L[5, 6] = 545;
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
        FC_N_L[2, 7] = 545;
        FC_N_L[3, 7] = 545;
        FC_N_L[4, 7] = 545;
        FC_N_L[5, 7] = 545;
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
        FC_N_L[2, 9] = 545;
        FC_N_L[3, 9] = 545;
        FC_N_L[4, 9] = 545;
        FC_N_L[5, 9] = 545;
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
        FC_N_L[2, 35] = 545;
        FC_N_L[3, 35] = 545;
        FC_N_L[4, 35] = 545;
        FC_N_L[5, 35] = 545;
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
        FC_N_L[2, 34] = 545;
        FC_N_L[3, 34] = 545;
        FC_N_L[4, 34] = 545;
        FC_N_L[5, 34] = 545;
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
        FC_N_R[0, 57] = 545;;
        FC_N_R[1, 57] = 545;;
        FC_N_R[2, 57] = 545;
        FC_N_R[3, 57] = 545;
        FC_N_R[4, 57] = 545;
        FC_N_R[5, 57] = 545;
        FC_N_R[6, 57] = 545;;
        //Check
        FC_N_R[7, 57] = 545;;
        //1147 3292
        FC_N_R[8, 57] = 545;;
        //1148 3292
        FC_N_R[9, 57] = 545;;
        //CHECK
        FC_N_R[10, 57] = 545;
        //CHECK
        FC_N_R[11, 57] = 545;;
        //1150 3292
        FC_N_R[12, 57] = 545;;
        //1151 3292
        FC_N_R[13, 57] = 545;;
        //*****************************

        //56 [Done]
        //***************************
        FC_N_R[0, 56] = 545;;
        FC_N_R[1, 56] = 545;;
        FC_N_R[2, 56] = 545;
        FC_N_R[3, 56] = 545;
        FC_N_R[4, 56] = 545;
        FC_N_R[5, 56] = 545;
        FC_N_R[6, 56] = 545;;
        //Check
        FC_N_R[7, 56] = 545;;
        //Check
        FC_N_R[8, 56] = 545;;
        //Check
        FC_N_R[9, 56] = 545;;
        FC_N_R[10, 56] = 545;
        FC_N_R[11, 56] = 545;;
        FC_N_R[12, 56] = 545;;
        FC_N_R[13, 56] = 545;;
        //*****************************

        //55 [Done]
        //***************************
        FC_N_R[0, 55] = 545;;
        FC_N_R[1, 55] = 545;;
        FC_N_R[2, 55] = 545;
        FC_N_R[3, 55] = 545;
        FC_N_R[4, 55] = 545;
        FC_N_R[5, 55] = 545;
        FC_N_R[6, 55] = 545;;
        FC_N_R[7, 55] = 545;;
        FC_N_R[8, 55] = 545;;
        FC_N_R[9, 55] = 545;;
        FC_N_R[10, 55] = 545;
        FC_N_R[11, 55] = 545;;
        FC_N_R[12, 55] = 545;;
        FC_N_R[13, 55] = 545;;
        //*****************************

        //54 
        //***************************
        FC_N_R[0, 54] = 545;;
        FC_N_R[1, 54] = 545;;
        FC_N_R[2, 54] = 545;
        FC_N_R[3, 54] = 545;
        FC_N_R[4, 54] = 545;
        FC_N_R[5, 54] = 545;
        FC_N_R[6, 54] = 545;;
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
        FC_N_R[0, 53] = 545;;
        FC_N_R[1, 53] = 545;;
        FC_N_R[2, 53] = 545;
        FC_N_R[3, 53] = 545;
        FC_N_R[4, 53] = 545;
        FC_N_R[5, 53] = 545;
        FC_N_R[6, 53] = 545;;
        FC_N_R[7, 53] = 545;;
        FC_N_R[8, 53] = 545;;
        FC_N_R[9, 53] = 545;;
        FC_N_R[10, 53] = 545;
        FC_N_R[11, 53] = 545;;
        FC_N_R[12, 53] = 545;;
        FC_N_R[13, 53] = 545;;
        //*****************************

        //52 
        //***************************
        FC_N_R[0, 52] = 545;;
        FC_N_R[1, 52] = 545;;
        FC_N_R[2, 52] = 545;
        FC_N_R[3, 52] = 545;
        FC_N_R[4, 52] = 545;
        FC_N_R[5, 52] = 545;
        FC_N_R[6, 52] = 545;;
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
        FC_N_R[0, 51] = 545;;
        FC_N_R[1, 51] = 545;;
        FC_N_R[2, 51] = 545;
        FC_N_R[3, 51] = 545;
        FC_N_R[4, 51] = 545;
        FC_N_R[5, 51] = 545;
        FC_N_R[6, 51] = 545;;
        FC_N_R[7, 51] = 545;;
        FC_N_R[8, 51] = 545;;
        FC_N_R[9, 51] = 545;;
        FC_N_R[10, 51] = 545;;
        FC_N_R[11, 51] = 545;;
        FC_N_R[12, 51] = 545;;
        FC_N_R[13, 51] = 545;;
        //*****************************

      
        //50
        //***************************
        FC_N_R[0, 50] = 545;;
        FC_N_R[1, 50] = 545;;
        FC_N_R[2, 50] = 545;
        FC_N_R[3, 50] = 545;
        FC_N_R[4, 50] = 545;
        FC_N_R[5, 50] = 545;
        FC_N_R[6, 50] = 545;;
        FC_N_R[7, 50] = 545;;
        FC_N_R[8, 50] = 545;;
        FC_N_R[9, 50] = 545;;
        FC_N_R[10, 50] = 545;
        FC_N_R[11, 50] = 545;;
        FC_N_R[12, 50] = 545;;
        FC_N_R[13, 50] = 545;;
        //*****************************

        //49
        //***************************
        FC_N_R[0, 49] = 545;;
        FC_N_R[1, 49] = 545;;
        FC_N_R[2, 49] = 545;
        FC_N_R[3, 49] = 545;
        FC_N_R[4, 49] = 545;
        FC_N_R[5, 49] = 545;
        FC_N_R[6, 49] = 545;;
        FC_N_R[7, 49] = 545;;
        FC_N_R[8, 49] = 545;;
        FC_N_R[9, 49] = 545;;
        FC_N_R[10, 49] = 545;
        FC_N_R[11, 49] = 545;;
        FC_N_R[12, 49] = 545;;
        FC_N_R[13, 49] = 545;;
        //*****************************

        //
        //***************************
        FC_N_R[0, 43] = 545;;
        FC_N_R[1, 43] = 545;;
        FC_N_R[2, 43] = 545;
        FC_N_R[3, 43] = 545;
        FC_N_R[4, 43] = 545;
        FC_N_R[5, 43] = 545;
        FC_N_R[6, 43] = 545;;
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
        FC_N_R[0, 44] = 545;;
        FC_N_R[1, 44] = 545;;
        FC_N_R[2, 44] = 545;
        FC_N_R[3, 44] = 545;
        FC_N_R[4, 44] = 545;
        FC_N_R[5, 44] = 545;
        FC_N_R[6, 44] = 545;;
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
        FC_N_R[0, 45] = 545;;
        FC_N_R[1, 45] = 545;;
        FC_N_R[2, 45] = 545;
        FC_N_R[3, 45] = 545;
        FC_N_R[4, 45] = 545;
        FC_N_R[5, 45] = 545;
        FC_N_R[6, 45] = 545;;
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
        FC_N_R[0, 46] = 545;;
        FC_N_R[1, 46] = 545;;
        FC_N_R[2, 46] = 545;
        FC_N_R[3, 46] = 545;
        FC_N_R[4, 46] = 545;
        FC_N_R[5, 46] = 545;
        FC_N_R[6, 46] = 545;;
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
        FC_N_R[0, 47] = 545;;
        FC_N_R[1, 47] = 545;;
        FC_N_R[2, 47] = 545;
        FC_N_R[3, 47] = 545;
        FC_N_R[4, 47] = 545;
        FC_N_R[5, 47] = 545;
        FC_N_R[6, 47] = 545;;
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
        FC_N_R[0, 48] = 545;;
        FC_N_R[1, 48] = 545;;
        FC_N_R[2, 48] = 545;
        FC_N_R[3, 48] = 545;
        FC_N_R[4, 48] = 545;
        FC_N_R[5, 48] = 545;
        FC_N_R[6, 48] = 545;;
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
        FC_N_R[0, 35] = 545;;
        FC_N_R[1, 35] = 545;;
        FC_N_R[2, 35] = 545;
        FC_N_R[3, 35] = 545;
        FC_N_R[4, 35] = 545;
        FC_N_R[5, 35] = 545;
        FC_N_R[6, 35] = 545;;
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
        FC_N_R[0, 34] = 545;;
        FC_N_R[1, 34] = 545;;
        FC_N_R[2, 34] = 545;
        FC_N_R[3, 34] = 545;
        FC_N_R[4, 34] = 545;
        FC_N_R[5, 34] = 545;
        FC_N_R[6, 34] = 545;;
        FC_N_R[7, 34] = 545;;
        FC_N_R[8, 34] = 545;;
        FC_N_R[9, 34] = 545;;
        FC_N_R[10, 34] = 340;
        FC_N_R[11, 34] = 345;
        FC_N_R[12, 34] = 345;
        FC_N_R[13, 34] = 345;
        //*****************************

    }
    public static void setRefsFlippedRight()
    {
        //14 
        //***************************
        FC_B_R[0, 14] = 545; ;
        FC_B_R[1, 14] = 545; ;
        FC_B_R[2, 14] = 545;
        FC_B_R[3, 14] = 545;
        FC_B_R[4, 14] = 545;
        FC_B_R[5, 14] = 545;
        FC_B_R[6, 14] = 545; ;
        FC_B_R[7, 14] = 545; ;
        FC_B_R[8, 14] = 545; ;
        FC_B_R[9, 14] = 545; ;
        FC_B_R[10, 14] = 545; ;
        FC_B_R[11, 14] = 545; ;
        FC_B_R[12, 14] = 545; ;
        FC_B_R[13, 14] = 545; ;
        FC_B_R[14, 14] = 545;
        FC_B_R[15, 14] = 545;
        FC_B_R[16, 14] = 545;
        FC_B_R[17, 14] = 545;
        FC_B_R[18, 14] = 545;
        FC_B_R[19, 14] = 545;
        FC_B_R[20, 14] = 545;



        //13 
        //***************************
        FC_B_R[0, 13] = 545; ;
        FC_B_R[1, 13] = 545; ;
        FC_B_R[2, 13] = 545;
        FC_B_R[3, 13] = 545;
        FC_B_R[4, 13] = 545;
        FC_B_R[5, 13] = 545;
        FC_B_R[6, 13] = 545; ;
        FC_B_R[7, 13] = 545; ;
        FC_B_R[8, 13] = 545; ;
        FC_B_R[9, 13] = 545; ;
        FC_B_R[10, 13] = 545;
        FC_B_R[11, 13] = 545; ;
        FC_B_R[12, 13] = 545; ;
        FC_B_R[13, 13] = 545; ;
        FC_B_R[14, 13] = 545;
        FC_B_R[15, 13] = 545;
        FC_B_R[16, 13] = 545;
        FC_B_R[17, 13] = 545;
        FC_B_R[18, 13] = 545;
        FC_B_R[19, 13] = 545;
        FC_B_R[20, 13] = 545;

        //*****************************

        //12 
        //***************************
        FC_B_R[0, 12] = 545;
        FC_B_R[1, 12] = 545;
        FC_B_R[2, 12] = 545;
        FC_B_R[3, 12] = 545;
        FC_B_R[4, 12] = 545;
        FC_B_R[5, 12] = 545;
        FC_B_R[6, 12] = 545;
        FC_B_R[7, 12] = 545; ;
        FC_B_R[8, 12] = 545; ;
        FC_B_R[9, 12] = 545; ;
        FC_B_R[10, 12] = 545;
        FC_B_R[11, 12] = 545; ;
        FC_B_R[12, 12] = 545; ;
        FC_B_R[13, 12] = 545; ;
        FC_B_R[14, 12] = 545;
        FC_B_R[15, 12] = 545;
        FC_B_R[16, 12] = 545;
        FC_B_R[17, 12] = 545;
        FC_B_R[18, 12] = 545;
        FC_B_R[19, 12] = 545;
        FC_B_R[20, 12] = 545;

        //*****************************
        //11 
        //***************************
        FC_B_R[0, 11] = 545;
        FC_B_R[1, 11] = 545;
        FC_B_R[2, 11] = 545;
        FC_B_R[3, 11] = 545;
        FC_B_R[4, 11] = 545;
        FC_B_R[5, 11] = 545;
        FC_B_R[6, 11] = 545;
        FC_B_R[7, 11] = 545; ;
        FC_B_R[8, 11] = 545; ;
        FC_B_R[9, 11] = 545; ;
        FC_B_R[10, 11] = 545;
        FC_B_R[11, 11] = 545; ;
        FC_B_R[12, 11] = 545; ;
        FC_B_R[13, 11] = 545; ;
        FC_B_R[14, 11] = 545;
        FC_B_R[15, 11] = 545;
        FC_B_R[16, 11] = 545;
        FC_B_R[17, 11] = 545;
        FC_B_R[18, 11] = 545;
        FC_B_R[19, 11] = 545;
        FC_B_R[20, 11] = 545;

        //*****************************


        //10
        //***************************
        FC_B_R[0, 10] = 545;
        FC_B_R[1, 10] = 545;
        FC_B_R[2, 10] = 545;
        FC_B_R[3, 10] = 545;
        FC_B_R[4, 10] = 545;
        FC_B_R[5, 10] = 545;
        FC_B_R[6, 10] = 545;
        FC_B_R[7, 10] = 545; ;
        FC_B_R[8, 10] = 545; ;
        FC_B_R[9, 10] = 545; ;
        FC_B_R[10, 10] = 545;
        FC_B_R[11, 10] = 545; ;
        FC_B_R[12, 10] = 545; ;
        FC_B_R[13, 10] = 545; ;
        FC_B_R[14, 10] = 545;
        FC_B_R[15, 10] = 545;
        FC_B_R[16, 10] = 545;
        FC_B_R[17, 10] = 545;
        FC_B_R[18, 10] = 545;
        FC_B_R[19, 10] = 545;
        FC_B_R[20, 10] = 545;

        //*****************************

        //8
        //***************************
        FC_B_R[0, 8] = 545;
        FC_B_R[1, 8] = 545;
        FC_B_R[2, 8] = 545;
        FC_B_R[3, 8] = 545;
        FC_B_R[4, 8] = 545;
        FC_B_R[5, 8] = 545;
        FC_B_R[6, 8] = 545;
        FC_B_R[7, 8] = 545; ;
        FC_B_R[8, 8] = 545; ;
        FC_B_R[9, 8] = 545; ;
        FC_B_R[10, 8] = 545;
        FC_B_R[11, 8] = 545; ;
        FC_B_R[12, 8] = 545; ;
        FC_B_R[13, 8] = 545; ;
        FC_B_R[14, 8] = 545;
        FC_B_R[15, 8] = 545;
        FC_B_R[16, 8] = 545;
        FC_B_R[17, 8] = 545;
        FC_B_R[18, 8] = 545;
        FC_B_R[19, 8] = 545;
        FC_B_R[20, 8] = 545;

        //*****************************

        //
        //***************************
        FC_B_R[0, 3] = 545;
        FC_B_R[1, 3] = 545;
        FC_B_R[2, 3] = 545;
        FC_B_R[3, 3] = 545;
        FC_B_R[4, 3] = 545;
        FC_B_R[5, 3] = 545;
        FC_B_R[6, 3] = 545;
        //Check
        FC_B_R[7, 3] = 545; ;
        //Check
        FC_B_R[8, 3] = 545; ;
        //Check
        FC_B_R[9, 3] = 545; ;
        FC_B_R[10, 3] = 545;
        FC_B_R[11, 3] = 545;
        FC_B_R[12, 3] = 545;
        FC_B_R[13, 3] = 545;
        FC_B_R[14, 3] = 545;
        FC_B_R[15, 3] = 545;
        FC_B_R[16, 3] = 545;
        FC_B_R[17, 3] = 545;
        FC_B_R[18, 3] = 545;
        FC_B_R[19, 3] = 545;
        FC_B_R[20, 3] = 545;

        //*****************************

        //
        //***************************
        FC_B_R[0, 4] = 545;
        FC_B_R[1, 4] = 545;
        FC_B_R[2, 4] = 545;
        FC_B_R[3, 4] = 545;
        FC_B_R[4, 4] = 545;
        FC_B_R[5, 4] = 545;
        FC_B_R[6, 4] = 545;
        FC_B_R[7, 4] = 545; ;
        FC_B_R[8, 4] = 545; ;
        FC_B_R[9, 4] = 545; ;
        FC_B_R[10, 4] = 545;
        FC_B_R[11, 4] = 545;
        FC_B_R[12, 4] = 545;
        FC_B_R[13, 4] = 545;
        FC_B_R[14, 4] = 545;
        FC_B_R[15, 4] = 545;
        FC_B_R[16, 4] = 545;
        FC_B_R[17, 4] = 545;
        FC_B_R[18, 4] = 545;
        FC_B_R[19, 4] = 545;
        FC_B_R[20, 4] = 545;

        //*****************************

        //5 
        //***************************
        FC_B_R[0, 5] = 545;
        FC_B_R[1, 5] = 545;
        FC_B_R[2, 5] = 545;
        FC_B_R[3, 5] = 545;
        FC_B_R[4, 5] = 545;
        FC_B_R[5, 5] = 545;
        FC_B_R[6, 5] = 545;
        FC_B_R[7, 5] = 545; ;
        FC_B_R[8, 5] = 545; ;
        FC_B_R[9, 5] = 545; ;
        FC_B_R[10, 5] = 545;
        FC_B_R[11, 5] = 545;
        FC_B_R[12, 5] = 545;
        FC_B_R[13, 5] = 545;
        FC_B_R[14, 5] = 545;
        FC_B_R[15, 5] = 545;
        FC_B_R[16, 5] = 545;
        FC_B_R[17, 5] = 545;
        FC_B_R[18, 5] = 545;
        FC_B_R[19, 5] = 545;
        FC_B_R[20, 5] = 545;

        //*****************************


        //6 
        //***************************
        FC_B_R[0, 6] = 545;
        FC_B_R[1, 6] = 545;
        FC_B_R[2, 6] = 545;
        FC_B_R[3, 6] = 545;
        FC_B_R[4, 6] = 545;
        FC_B_R[5, 6] = 545;
        FC_B_R[6, 6] = 545;
        FC_B_R[7, 6] = 545; ;
        FC_B_R[8, 6] = 545; ;
        FC_B_R[9, 6] = 545; ;
        FC_B_R[10, 6] = 545;
        FC_B_R[11, 6] = 545;
        FC_B_R[12, 6] = 545;
        FC_B_R[13, 6] = 545;
        FC_B_R[14, 6] = 545;
        FC_B_R[15, 6] = 545;
        FC_B_R[16, 6] = 545;
        FC_B_R[17, 6] = 545;
        FC_B_R[18, 6] = 545;
        FC_B_R[19, 6] = 545;
        FC_B_R[20, 6] = 545;

        //*****************************

        //7 
        //***************************
        FC_B_R[0, 7] = 545;
        FC_B_R[1, 7] = 545;
        FC_B_R[2, 7] = 545;
        FC_B_R[3, 7] = 545;
        FC_B_R[4, 7] = 545;
        FC_B_R[5, 7] = 545;
        FC_B_R[6, 7] = 545;
        FC_B_R[7, 7] = 545; ;
        FC_B_R[8, 7] = 545; ;
        FC_B_R[9, 7] = 545; ;
        FC_B_R[10, 7] = 545;
        FC_B_R[11, 7] = 545;
        FC_B_R[12, 7] = 545;
        FC_B_R[13, 7] = 545;
        FC_B_R[14, 7] = 545;
        FC_B_R[15, 7] = 545;
        FC_B_R[16, 7] = 545;
        FC_B_R[17, 7] = 545;
        FC_B_R[18, 7] = 545;
        FC_B_R[19, 7] = 545;
        FC_B_R[20, 7] = 545;

        //*****************************
        //9 
        //***************************
        FC_B_R[0, 9] = 545;
        FC_B_R[1, 9] = 545;
        FC_B_R[2, 9] = 545;
        FC_B_R[3, 9] = 545;
        FC_B_R[4, 9] = 545;
        FC_B_R[5, 9] = 545;
        FC_B_R[6, 9] = 545;
        FC_B_R[7, 9] = 545; ;
        FC_B_R[8, 9] = 545; ;
        FC_B_R[9, 9] = 545; ;
        FC_B_R[10, 9] = 545; ;
        FC_B_R[11, 9] = 545;
        FC_B_R[12, 9] = 545;
        FC_B_R[13, 9] = 545;
        FC_B_R[14, 9] = 545;
        FC_B_R[15, 9] = 545;
        FC_B_R[16, 9] = 545;
        FC_B_R[17, 9] = 545;
        FC_B_R[18, 9] = 545;
        FC_B_R[19, 9] = 545;
        FC_B_R[20, 9] = 545;

        //*****************************

        //2 
        //***************************
        FC_B_R[0, 2] = 545;
        FC_B_R[1, 2] = 545;
        FC_B_R[2, 2] = 545;
        FC_B_R[3, 2] = 545;
        FC_B_R[4, 2] = 545;
        FC_B_R[5, 2] = 545;
        FC_B_R[6, 2] = 545;
        FC_B_R[7, 2] = 545; ;
        FC_B_R[8, 2] = 545; ;
        FC_B_R[9, 2] = 545; ;
        FC_B_R[10, 2] = 545;
        FC_B_R[11, 2] = 545;
        FC_B_R[12, 2] = 545;
        FC_B_R[13, 2] = 545;
        FC_B_R[14, 2] = 545;
        FC_B_R[15, 2] = 545;
        FC_B_R[16, 2] = 545;
        FC_B_R[17, 2] = 545;
        FC_B_R[18, 2] = 545;
        FC_B_R[19, 2] = 545;
        FC_B_R[20, 2] = 545;

        //*****************************

        //1 
        //***************************
        FC_B_R[0, 1] = 545;
        FC_B_R[1, 1] = 545;
        FC_B_R[2, 1] = 545;
        FC_B_R[3, 1] = 545;
        FC_B_R[4, 1] = 545;
        FC_B_R[5, 1] = 545;
        FC_B_R[6, 1] = 545;
        FC_B_R[7, 1] = 545; ;
        FC_B_R[8, 1] = 545; ;
        FC_B_R[9, 1] = 545; ;
        FC_B_R[10, 1] = 10;
        FC_B_R[11, 1] = 545;
        FC_B_R[12, 1] = 545;
        FC_B_R[13, 1] = 545;
        FC_B_R[14, 1] = 545;
        FC_B_R[15, 1] = 545;
        FC_B_R[16, 1] = 545;
        FC_B_R[17, 1] = 545;
        FC_B_R[18, 1] = 545;
        FC_B_R[19, 1] = 545;
        FC_B_R[20, 1] = 545;

        //*****************************


        //0 
        //***************************
        FC_B_R[0, 0] = 545;
        FC_B_R[1, 0] = 545;
        FC_B_R[2, 0] = 545;
        FC_B_R[3, 0] = 545;
        FC_B_R[4, 0] = 545;
        FC_B_R[5, 0] = 545;
        FC_B_R[6, 0] = 545;
        FC_B_R[7, 0] = 545; ;
        FC_B_R[8, 0] = 545; ;
        FC_B_R[9, 0] = 545; ;
        FC_B_R[10, 0] = 00;
        FC_B_R[11, 0] = 545;
        FC_B_R[12, 0] = 545;
        FC_B_R[13, 0] = 545;
        FC_B_R[14, 0] = 545;
        FC_B_R[15, 0] = 545;
        FC_B_R[16, 0] = 545;
        FC_B_R[17, 0] = 545;
        FC_B_R[18, 0] = 545;
        FC_B_R[19, 0] = 545;
        FC_B_R[20, 0] = 545;

        //*****************************

        //38 
        //***************************
        FC_B_R[0, 38] = 545;
        FC_B_R[1, 38] = 545;
        FC_B_R[2, 38] = 545;
        FC_B_R[3, 38] = 545;
        FC_B_R[4, 38] = 545;
        FC_B_R[5, 38] = 545;
        FC_B_R[6, 38] = 545;
        FC_B_R[7, 38] = 545; ;
        FC_B_R[8, 38] = 545; ;
        FC_B_R[9, 38] = 545; ;
        FC_B_R[10, 38] = 545; ;
        FC_B_R[11, 38] = 545;
        FC_B_R[12, 38] = 545;
        FC_B_R[13, 38] = 545;
        FC_B_R[14, 38] = 545;
        FC_B_R[15, 38] = 545;
        FC_B_R[16, 38] = 545;
        FC_B_R[17, 38] = 545;
        FC_B_R[18, 38] = 545;
        FC_B_R[19, 38] = 545;
        FC_B_R[20, 38] = 545;

        //*****************************
        //37 
        //***************************
        FC_B_R[0, 37] = 545;
        FC_B_R[1, 37] = 545;
        FC_B_R[2, 37] = 545;
        FC_B_R[3, 37] = 545;
        FC_B_R[4, 37] = 545;
        FC_B_R[5, 37] = 545;
        FC_B_R[6, 37] = 545;
        FC_B_R[7, 37] = 545; ;
        FC_B_R[8, 37] = 545; ;
        FC_B_R[9, 37] = 545; ;
        FC_B_R[10, 37] = 545;
        FC_B_R[11, 37] = 545;
        FC_B_R[12, 37] = 545;
        FC_B_R[13, 37] = 545;
        FC_B_R[14, 37] = 545;
        FC_B_R[15, 37] = 545;
        FC_B_R[16, 37] = 545;
        FC_B_R[17, 37] = 545;
        FC_B_R[18, 37] = 545;
        FC_B_R[19, 37] = 545;
        FC_B_R[20, 37] = 545;

        //*****************************


        //36
        //***************************
        FC_B_R[0, 36] = 545;
        FC_B_R[1, 36] = 545;
        FC_B_R[2, 36] = 545;
        FC_B_R[3, 36] = 545;
        FC_B_R[4, 36] = 545;
        FC_B_R[5, 36] = 545;
        FC_B_R[6, 36] = 545;
        FC_B_R[7, 36] = 545; ;
        FC_B_R[8, 36] = 545; ;
        FC_B_R[9, 36] = 545; ;
        FC_B_R[10, 36] = 545;
        FC_B_R[11, 36] = 545;
        FC_B_R[12, 36] = 545;
        FC_B_R[13, 36] = 545;
        FC_B_R[14, 36] = 545;
        FC_B_R[15, 36] = 545;
        FC_B_R[16, 36] = 545;
        FC_B_R[17, 36] = 545;
        FC_B_R[18, 36] = 545;
        FC_B_R[19, 36] = 545;
        FC_B_R[20, 36] = 545;

        //*****************************


        //35 
        //***************************
        FC_B_R[0, 35] = 545;
        FC_B_R[1, 35] = 545;
        FC_B_R[2, 35] = 545;
        FC_B_R[3, 35] = 545;
        FC_B_R[4, 35] = 545;
        FC_B_R[5, 35] = 545;
        FC_B_R[6, 35] = 545;
        FC_B_R[7, 35] = 545; ;
        FC_B_R[8, 35] = 545; ;
        FC_B_R[9, 35] = 545; ;
        FC_B_R[10, 35] = 545;
        FC_B_R[11, 35] = 545;
        FC_B_R[12, 35] = 545;
        FC_B_R[13, 35] = 545;
        FC_B_R[14, 35] = 545;
        FC_B_R[15, 35] = 545;
        FC_B_R[16, 35] = 545;
        FC_B_R[17, 35] = 545;
        FC_B_R[18, 35] = 545;
        FC_B_R[19, 35] = 545;
        FC_B_R[20, 35] = 545;

        //*****************************


        //34 
        //***************************
        FC_B_R[0, 34] = 545;
        FC_B_R[1, 34] = 545;
        FC_B_R[2, 34] = 545;
        FC_B_R[3, 34] = 545;
        FC_B_R[4, 34] = 545;
        FC_B_R[5, 34] = 545;
        FC_B_R[6, 34] = 545;
        FC_B_R[7, 34] = 545; ;
        FC_B_R[8, 34] = 545; ;
        FC_B_R[9, 34] = 545; ;
        FC_B_R[10, 34] = 545;
        FC_B_R[11, 34] = 545;
        FC_B_R[12, 34] = 545;
        FC_B_R[13, 34] = 545;
        FC_B_R[14, 34] = 545;
        FC_B_R[15, 34] = 545;
        FC_B_R[16, 34] = 545;
        FC_B_R[17, 34] = 545;
        FC_B_R[18, 34] = 545;
        FC_B_R[19, 34] = 545;
        FC_B_R[20, 34] = 545;

        //*****************************



        //33 [Done]
        //***************************
        FC_B_R[0, 33] = 545;
        FC_B_R[1, 33] = 545;
        FC_B_R[2, 33] = 545;
        FC_B_R[3, 33] = 545;
        FC_B_R[4, 33] = 545;
        FC_B_R[5, 33] = 545;
        FC_B_R[6, 33] = 545;
        FC_B_R[7, 33] = 545;
        FC_B_R[8, 33] = 545;
        FC_B_R[9, 33] = 545;
        FC_B_R[10, 33] = 545;
        FC_B_R[11, 33] = 545;
        FC_B_R[12, 33] = 545;
        FC_B_R[13, 33] = 545;
        FC_B_R[14, 33] = 545;
        FC_B_R[15, 33] = 545;
        FC_B_R[16, 33] = 545;
        FC_B_R[17, 33] = 545;
        FC_B_R[18, 33] = 545;
        FC_B_R[19, 33] = 545;
        FC_B_R[20, 33] = 545;

        //*****************************

        //32 
        //***************************
        FC_B_R[0, 32] = 545;
        FC_B_R[1, 32] = 545;
        FC_B_R[2, 32] = 545;
        FC_B_R[3, 32] = 545;
        FC_B_R[4, 32] = 545;
        FC_B_R[5, 32] = 545;
        FC_B_R[6, 32] = 545;
        FC_B_R[7, 32] = 545;
        FC_B_R[8, 32] = 545;
        FC_B_R[9, 32] = 545;
        FC_B_R[10, 32] = 545;
        FC_B_R[11, 32] = 545;
        FC_B_R[12, 32] = 545;
        FC_B_R[13, 32] = 545;
        FC_B_R[14, 32] = 545;
        FC_B_R[15, 32] = 545;
        FC_B_R[16, 32] = 545;
        FC_B_R[17, 32] = 545;
        FC_B_R[18, 32] = 545;
        FC_B_R[19, 32] = 545;
        FC_B_R[20, 32] = 545;

        //*****************************


        //31 
        //***************************
        FC_B_R[0, 31] = 545;
        FC_B_R[1, 31] = 545;
        FC_B_R[2, 31] = 545;
        FC_B_R[3, 31] = 545;
        FC_B_R[4, 31] = 545;
        FC_B_R[5, 31] = 545;
        FC_B_R[6, 31] = 545;
        FC_B_R[7, 31] = 545;
        FC_B_R[8, 31] = 545;
        FC_B_R[9, 31] = 545;
        FC_B_R[10, 31] = 545;
        FC_B_R[11, 31] = 545;
        FC_B_R[12, 31] = 545;
        FC_B_R[13, 31] = 545;
        FC_B_R[14, 31] = 545;
        FC_B_R[15, 31] = 545;
        FC_B_R[16, 31] = 545;
        FC_B_R[17, 31] = 545;
        FC_B_R[18, 31] = 545;
        FC_B_R[19, 31] = 545;
        FC_B_R[20, 31] = 545;

        //*****************************

        //30 
        //***************************
        FC_B_R[0, 30] = 545;
        FC_B_R[1, 30] = 545;
        FC_B_R[2, 30] = 545;
        FC_B_R[3, 30] = 545;
        FC_B_R[4, 30] = 545;
        FC_B_R[5, 30] = 545;
        FC_B_R[6, 30] = 545;
        FC_B_R[7, 30] = 545;
        FC_B_R[8, 30] = 545;
        FC_B_R[9, 30] = 545;
        FC_B_R[10, 30] = 545;
        FC_B_R[11, 30] = 545;
        FC_B_R[12, 30] = 545;
        FC_B_R[13, 30] = 545;
        FC_B_R[14, 30] = 545;
        FC_B_R[15, 30] = 545;
        FC_B_R[16, 30] = 545;
        FC_B_R[17, 30] = 545;
        FC_B_R[18, 30] = 545;
        FC_B_R[19, 30] = 545;
        FC_B_R[20, 30] = 545;

        //*****************************
        //29 
        //***************************
        FC_B_R[0, 29] = 545;
        FC_B_R[1, 29] = 545;
        FC_B_R[2, 29] = 545;
        FC_B_R[3, 29] = 545;
        FC_B_R[4, 29] = 545;
        FC_B_R[5, 29] = 545;
        FC_B_R[6, 29] = 545;
        FC_B_R[7, 29] = 545;
        FC_B_R[8, 29] = 545;
        FC_B_R[9, 29] = 545;
        FC_B_R[10, 29] = 545;
        FC_B_R[11, 29] = 545;
        FC_B_R[12, 29] = 545;
        FC_B_R[13, 29] = 545;
        FC_B_R[14, 29] = 545;
        FC_B_R[15, 29] = 545;
        FC_B_R[16, 29] = 545;
        FC_B_R[17, 29] = 545;
        FC_B_R[18, 29] = 545;
        FC_B_R[19, 29] = 545;
        FC_B_R[20, 29] = 545;

        //*****************************


        //28
        //***************************
        FC_B_R[0, 28] = 545;
        FC_B_R[1, 28] = 545;
        FC_B_R[2, 28] = 545;
        FC_B_R[3, 28] = 545;
        FC_B_R[4, 28] = 545;
        FC_B_R[5, 28] = 545;
        FC_B_R[6, 28] = 545;
        FC_B_R[7, 28] = 545;
        FC_B_R[8, 28] = 545;
        FC_B_R[9, 28] = 545;
        FC_B_R[10, 28] = 545;
        FC_B_R[11, 28] = 545;
        FC_B_R[12, 28] = 545;
        FC_B_R[13, 28] = 545;
        FC_B_R[14, 28] = 545;
        FC_B_R[15, 28] = 545;
        FC_B_R[16, 28] = 545;
        FC_B_R[17, 28] = 545;
        FC_B_R[18, 28] = 545;
        FC_B_R[19, 28] = 545;
        FC_B_R[20, 28] = 545;

        //*****************************

        //27
        //***************************
        FC_B_R[0, 27] = 545;
        FC_B_R[1, 27] = 545;
        FC_B_R[2, 27] = 545;
        FC_B_R[3, 27] = 545;
        FC_B_R[4, 27] = 545;
        FC_B_R[5, 27] = 545;
        FC_B_R[6, 27] = 545;
        FC_B_R[7, 27] = 545;
        FC_B_R[8, 27] = 545;
        FC_B_R[9, 27] = 545;
        FC_B_R[10, 27] = 545;
        FC_B_R[11, 27] = 545;
        FC_B_R[12, 27] = 545;
        FC_B_R[13, 27] = 545;
        FC_B_R[14, 27] = 545;
        FC_B_R[15, 27] = 545;
        FC_B_R[16, 27] = 545;
        FC_B_R[17, 27] = 545;
        FC_B_R[18, 27] = 545;
        FC_B_R[19, 27] = 545;
        FC_B_R[20, 27] = 545;

        //*****************************

        //
        //***************************
        FC_B_R[0, 21] = 545;
        FC_B_R[1, 21] = 545;
        FC_B_R[2, 21] = 545;
        FC_B_R[3, 21] = 545;
        FC_B_R[4, 21] = 545;
        FC_B_R[5, 21] = 545;
        FC_B_R[6, 21] = 545;
        //Check
        FC_B_R[7, 21] = 545;
        //Check
        FC_B_R[8, 21] = 545;
        //Check
        FC_B_R[9, 21] = 545;
        FC_B_R[10, 21] = 545;
        FC_B_R[11, 21] = 545;
        FC_B_R[12, 21] = 545;
        FC_B_R[13, 21] = 545;
        FC_B_R[14, 21] = 545;
        FC_B_R[15, 21] = 545;
        FC_B_R[16, 21] = 545;
        FC_B_R[17, 21] = 545;
        FC_B_R[18, 21] = 545;
        FC_B_R[19, 21] = 545;
        FC_B_R[20, 21] = 545;

        //*****************************

        //
        //***************************
        FC_B_R[0, 22] = 545;
        FC_B_R[1, 22] = 545;
        FC_B_R[2, 22] = 545;
        FC_B_R[3, 22] = 545;
        FC_B_R[4, 22] = 545;
        FC_B_R[5, 22] = 545;
        FC_B_R[6, 22] = 545;
        FC_B_R[7, 22] = 545;
        FC_B_R[8, 22] = 545;
        FC_B_R[9, 22] = 545;
        FC_B_R[10, 22] = 545;
        FC_B_R[11, 22] = 545;
        FC_B_R[12, 22] = 545;
        FC_B_R[13, 22] = 545;
        FC_B_R[14, 22] = 545;
        FC_B_R[15, 22] = 545;
        FC_B_R[16, 22] = 545;
        FC_B_R[17, 22] = 545;
        FC_B_R[18, 22] = 545;
        FC_B_R[19, 22] = 545;
        FC_B_R[20, 22] = 545;

        //*****************************

        //23 
        //***************************
        FC_B_R[0, 23] = 545;
        FC_B_R[1, 23] = 545;
        FC_B_R[2, 23] = 545;
        FC_B_R[3, 23] = 545;
        FC_B_R[4, 23] = 545;
        FC_B_R[5, 23] = 545;
        FC_B_R[6, 23] = 545;
        FC_B_R[7, 23] = 545;
        FC_B_R[8, 23] = 545;
        FC_B_R[9, 23] = 545;
        FC_B_R[10, 23] = 545;
        FC_B_R[11, 23] = 545;
        FC_B_R[12, 23] = 545;
        FC_B_R[13, 23] = 545;
        FC_B_R[14, 23] = 545;
        FC_B_R[15, 23] = 545;
        FC_B_R[16, 23] = 545;
        FC_B_R[17, 23] = 545;
        FC_B_R[18, 23] = 545;
        FC_B_R[19, 23] = 545;
        FC_B_R[20, 23] = 545;

        //*****************************


        //24 
        //***************************
        FC_B_R[0, 24] = 545;
        FC_B_R[1, 24] = 545;
        FC_B_R[2, 24] = 545;
        FC_B_R[3, 24] = 545;
        FC_B_R[4, 24] = 545;
        FC_B_R[5, 24] = 545;
        FC_B_R[6, 24] = 545;
        FC_B_R[7, 24] = 545;
        FC_B_R[8, 24] = 545;
        FC_B_R[9, 24] = 545;
        FC_B_R[10, 24] = 545;
        FC_B_R[11, 24] = 545;
        FC_B_R[12, 24] = 545;
        FC_B_R[13, 24] = 545;
        FC_B_R[14, 24] = 545;
        FC_B_R[15, 24] = 545;
        FC_B_R[16, 24] = 545;
        FC_B_R[17, 24] = 545;
        FC_B_R[18, 24] = 545;
        FC_B_R[19, 24] = 545;
        FC_B_R[20, 24] = 545;

        //*****************************

        //25 
        //***************************
        FC_B_R[0, 25] = 545;
        FC_B_R[1, 25] = 545;
        FC_B_R[2, 25] = 545;
        FC_B_R[3, 25] = 545;
        FC_B_R[4, 25] = 545;
        FC_B_R[5, 25] = 545;
        FC_B_R[6, 25] = 545;
        FC_B_R[7, 25] = 545;
        FC_B_R[8, 25] = 545;
        FC_B_R[9, 25] = 545;
        FC_B_R[10, 25] = 545;
        FC_B_R[11, 25] = 545;
        FC_B_R[12, 25] = 545;
        FC_B_R[13, 25] = 545;
        FC_B_R[14, 25] = 545;
        FC_B_R[15, 25] = 545;
        FC_B_R[16, 25] = 545;
        FC_B_R[17, 25] = 545;
        FC_B_R[18, 25] = 545;
        FC_B_R[19, 25] = 545;
        FC_B_R[20, 25] = 545;

        //*****************************
        //26 
        //***************************
        FC_B_R[0, 26] = 545;
        FC_B_R[1, 26] = 545;
        FC_B_R[2, 26] = 545;
        FC_B_R[3, 26] = 545;
        FC_B_R[4, 26] = 545;
        FC_B_R[5, 26] = 545;
        FC_B_R[6, 26] = 545;
        FC_B_R[7, 26] = 545;
        FC_B_R[8, 26] = 545;
        FC_B_R[9, 26] = 545;
        FC_B_R[10, 26] = 545;
        FC_B_R[11, 26] = 545;
        FC_B_R[12, 26] = 545;
        FC_B_R[13, 26] = 545;
        FC_B_R[14, 26] = 545;
        FC_B_R[15, 26] = 545;
        FC_B_R[16, 26] = 545;
        FC_B_R[17, 26] = 545;
        FC_B_R[18, 26] = 545;
        FC_B_R[19, 26] = 545;
        FC_B_R[20, 26] = 545;

        //*****************************

        //20
        //***************************
        FC_B_R[0, 20] = 545;
        FC_B_R[1, 20] = 545;
        FC_B_R[2, 20] = 545;
        FC_B_R[3, 20] = 545;
        FC_B_R[4, 20] = 545;
        FC_B_R[5, 20] = 545;
        FC_B_R[6, 20] = 545;
        //Check
        FC_B_R[7, 20] = 545;
        //Check
        FC_B_R[8, 20] = 545;
        //Check
        FC_B_R[9, 20] = 545;
        FC_B_R[10, 20] = 545;
        FC_B_R[11, 20] = 545;
        FC_B_R[12, 20] = 545;
        FC_B_R[13, 20] = 545;
        FC_B_R[14, 20] = 545;
        FC_B_R[15, 20] = 545;
        FC_B_R[16, 20] = 545;
        FC_B_R[17, 20] = 545;
        FC_B_R[18, 20] = 545;
        FC_B_R[19, 20] = 545;
        FC_B_R[20, 20] = 545;

        //*****************************

        //19 
        //***************************
        FC_B_R[0, 19] = 545;
        FC_B_R[1, 19] = 545;
        FC_B_R[2, 19] = 545;
        FC_B_R[3, 19] = 545;
        FC_B_R[4, 19] = 545;
        FC_B_R[5, 19] = 545;
        FC_B_R[6, 19] = 545;
        FC_B_R[7, 19] = 545;
        FC_B_R[8, 19] = 545;
        FC_B_R[9, 19] = 545;
        FC_B_R[10, 19] = 545;
        FC_B_R[11, 19] = 545;
        FC_B_R[12, 19] = 545;
        FC_B_R[13, 19] = 545;
        FC_B_R[14, 19] = 545;
        FC_B_R[15, 19] = 545;
        FC_B_R[16, 19] = 545;
        FC_B_R[17, 19] = 545;
        FC_B_R[18, 19] = 545;
        FC_B_R[19, 19] = 545;
        FC_B_R[20, 19] = 545;

        //*****************************

        //18 
        //***************************
        FC_B_R[0, 18] = 545;
        FC_B_R[1, 18] = 545;
        FC_B_R[2, 18] = 545;
        FC_B_R[3, 18] = 545;
        FC_B_R[4, 18] = 545;
        FC_B_R[5, 18] = 545;
        FC_B_R[6, 18] = 545;
        FC_B_R[7, 18] = 545;
        FC_B_R[8, 18] = 545;
        FC_B_R[9, 18] = 545;
        FC_B_R[10, 18] = 545;
        FC_B_R[11, 18] = 545;
        FC_B_R[12, 18] = 545;
        FC_B_R[13, 18] = 545;
        FC_B_R[14, 18] = 545;
        FC_B_R[15, 18] = 545;
        FC_B_R[16, 18] = 545;
        FC_B_R[17, 18] = 545;
        FC_B_R[18, 18] = 545;
        FC_B_R[19, 18] = 545;
        FC_B_R[20, 18] = 545;

        //*****************************


        //17 
        //***************************
        FC_B_R[0, 17] = 545;
        FC_B_R[1, 17] = 545;
        FC_B_R[2, 17] = 545;
        FC_B_R[3, 17] = 545;
        FC_B_R[4, 17] = 545;
        FC_B_R[5, 17] = 545;
        FC_B_R[6, 17] = 545;
        FC_B_R[7, 17] = 545;
        FC_B_R[8, 17] = 545;
        FC_B_R[9, 17] = 545;
        FC_B_R[10, 17] = 545;
        FC_B_R[11, 17] = 545;
        FC_B_R[12, 17] = 545;
        FC_B_R[13, 17] = 545;
        FC_B_R[14, 17] = 545;
        FC_B_R[15, 17] = 545;
        FC_B_R[16, 17] = 545;
        FC_B_R[17, 17] = 545;
        FC_B_R[18, 17] = 545;
        FC_B_R[19, 17] = 545;
        FC_B_R[20, 17] = 545;

        //*****************************

        //16 
        //***************************
        FC_B_R[0, 16] = 545;
        FC_B_R[1, 16] = 545;
        FC_B_R[2, 16] = 545;
        FC_B_R[3, 16] = 545;
        FC_B_R[4, 16] = 545;
        FC_B_R[5, 16] = 545;
        FC_B_R[6, 16] = 545;
        FC_B_R[7, 16] = 545;
        FC_B_R[8, 16] = 545;
        FC_B_R[9, 16] = 545;
        FC_B_R[10, 16] = 545;
        FC_B_R[11, 16] = 545;
        FC_B_R[12, 16] = 545;
        FC_B_R[13, 16] = 545;
        FC_B_R[14, 16] = 545;
        FC_B_R[15, 16] = 545;
        FC_B_R[16, 16] = 545;
        FC_B_R[17, 16] = 545;
        FC_B_R[18, 16] = 545;
        FC_B_R[19, 16] = 545;
        FC_B_R[20, 16] = 545;

        //*****************************
        //15 
        //***************************
        FC_B_R[0, 15] = 545;
        FC_B_R[1, 15] = 545;
        FC_B_R[2, 15] = 545;
        FC_B_R[3, 15] = 545;
        FC_B_R[4, 15] = 545;
        FC_B_R[5, 15] = 545;
        FC_B_R[6, 15] = 545;
        FC_B_R[7, 15] = 545;
        FC_B_R[8, 15] = 545;
        FC_B_R[9, 15] = 545;
        FC_B_R[10, 15] = 545;
        FC_B_R[11, 15] = 545;
        FC_B_R[12, 15] = 545;
        FC_B_R[13, 15] = 545;
        FC_B_R[14, 15] = 545;
        FC_B_R[15, 15] = 545;
        FC_B_R[16, 15] = 545;
        FC_B_R[17, 15] = 545;
        FC_B_R[18, 15] = 545;
        FC_B_R[19, 15] = 545;
        FC_B_R[20, 15] = 545;


        FC_B_R[0, 39] = 545;
        FC_B_R[1, 39] = 545;
        FC_B_R[2, 39] = 545;
        FC_B_R[3, 39] = 545;
        FC_B_R[4, 39] = 545;
        FC_B_R[5, 39] = 545;
        FC_B_R[6, 39] = 545;
        FC_B_R[7, 39] = 545;
        FC_B_R[8, 39] = 545;
        FC_B_R[9, 39] = 545;
        FC_B_R[10, 39] = 545; ; ;
        FC_B_R[12, 39] = 545;
        FC_B_R[13, 39] = 545;
        FC_B_R[14, 39] = 545;
        FC_B_R[15, 39] = 545;
        FC_B_R[16, 39] = 545;
        FC_B_R[17, 39] = 545;
        FC_B_R[18, 39] = 545;
        FC_B_R[19, 39] = 545;
        FC_B_R[20, 39] = 545;




        FC_B_R[0, 41] = 545;
        FC_B_R[1, 41] = 545;
        FC_B_R[2, 41] = 545;
        FC_B_R[3, 41] = 545;
        FC_B_R[4, 41] = 545;
        FC_B_R[5, 41] = 545;
        FC_B_R[6, 41] = 545;
        FC_B_R[7, 41] = 545;
        FC_B_R[8, 41] = 545;
        FC_B_R[9, 41] = 545;
        FC_B_R[10, 41] = 545; ; ;
        FC_B_R[12, 41] = 545;
        FC_B_R[13, 41] = 545;
        FC_B_R[14, 41] = 545;
        FC_B_R[15, 41] = 545;
        FC_B_R[16, 41] = 545;
        FC_B_R[17, 41] = 545;
        FC_B_R[18, 41] = 545;
        FC_B_R[19, 41] = 545;
        FC_B_R[20, 41] = 545;




        FC_B_R[0, 40] = 545;
        FC_B_R[1, 40] = 545;
        FC_B_R[2, 40] = 545;
        FC_B_R[3, 40] = 545;
        FC_B_R[4, 40] = 545;
        FC_B_R[5, 40] = 545;
        FC_B_R[6, 40] = 545;
        FC_B_R[7, 40] = 545;
        FC_B_R[8, 40] = 545;
        FC_B_R[9, 40] = 545;
        FC_B_R[10, 40] = 545; ; ;
        FC_B_R[12, 40] = 545;
        FC_B_R[13, 40] = 545;
        FC_B_R[14, 40] = 545;
        FC_B_R[15, 40] = 545;
        FC_B_R[16, 40] = 545;
        FC_B_R[17, 40] = 545;
        FC_B_R[18, 40] = 545;
        FC_B_R[19, 40] = 545;
        FC_B_R[20, 40] = 545;

        FC_B_R[0, 42] = 545;
        FC_B_R[1, 42] = 545;
        FC_B_R[2, 42] = 545;
        FC_B_R[3, 42] = 545;
        FC_B_R[4, 42] = 545;
        FC_B_R[5, 42] = 545;
        FC_B_R[6, 42] = 545;
        FC_B_R[7, 42] = 545;
        FC_B_R[8, 42] = 545;
        FC_B_R[9, 42] = 545;
        FC_B_R[10, 42] = 545; ; ;
        FC_B_R[12, 42] = 545;
        FC_B_R[13, 42] = 545;
        FC_B_R[14, 42] = 545;
        FC_B_R[15, 42] = 545;
        FC_B_R[16, 42] = 545;
        FC_B_R[17, 42] = 545;
        FC_B_R[18, 42] = 545;
        FC_B_R[19, 42] = 545;
        FC_B_R[20, 42] = 545;

        FC_B_R[0, 43] = 545;
        FC_B_R[1, 43] = 545;
        FC_B_R[2, 43] = 545;
        FC_B_R[3, 43] = 545;
        FC_B_R[4, 43] = 545;
        FC_B_R[5, 43] = 545;
        FC_B_R[6, 43] = 545;
        FC_B_R[7, 43] = 545;
        FC_B_R[8, 43] = 545;
        FC_B_R[9, 43] = 545;
        FC_B_R[10, 43] = 545; ; ;
        FC_B_R[12, 43] = 545;
        FC_B_R[13, 43] = 545;
        FC_B_R[14, 43] = 545;
        FC_B_R[15, 43] = 545;
        FC_B_R[16, 43] = 545;
        FC_B_R[17, 43] = 545;
        FC_B_R[18, 43] = 545;
        FC_B_R[19, 43] = 545;
        FC_B_R[20, 43] = 545;

        FC_B_R[0, 44] = 545;
        FC_B_R[1, 44] = 545;
        FC_B_R[2, 44] = 545;
        FC_B_R[3, 44] = 545;
        FC_B_R[4, 44] = 545;
        FC_B_R[5, 44] = 545;
        FC_B_R[6, 44] = 545;
        FC_B_R[7, 44] = 545;
        FC_B_R[8, 44] = 545;
        FC_B_R[9, 44] = 545;
        FC_B_R[10, 44] = 545; ; ;
        FC_B_R[12, 44] = 545;
        FC_B_R[13, 44] = 545;
        FC_B_R[14, 44] = 545;
        FC_B_R[15, 44] = 545;
        FC_B_R[16, 44] = 545;
        FC_B_R[17, 44] = 545;
        FC_B_R[18, 44] = 545;
        FC_B_R[19, 44] = 545;
        FC_B_R[20, 44] = 545;

        FC_B_R[0, 45] = 545;
        FC_B_R[1, 45] = 545;
        FC_B_R[2, 45] = 545;
        FC_B_R[3, 45] = 545;
        FC_B_R[4, 45] = 545;
        FC_B_R[5, 45] = 545;
        FC_B_R[6, 45] = 545;
        FC_B_R[7, 45] = 545;
        FC_B_R[8, 45] = 545;
        FC_B_R[9, 45] = 545;
        FC_B_R[10, 45] = 545; ; ;
        FC_B_R[12, 45] = 545;
        FC_B_R[13, 45] = 545;
        FC_B_R[14, 45] = 545;
        FC_B_R[15, 45] = 545;
        FC_B_R[16, 45] = 545;
        FC_B_R[17, 45] = 545;
        FC_B_R[18, 45] = 545;
        FC_B_R[19, 45] = 545;
        FC_B_R[20, 45] = 545;

        FC_B_R[0, 46] = 545;
        FC_B_R[1, 46] = 545;
        FC_B_R[2, 46] = 545;
        FC_B_R[3, 46] = 545;
        FC_B_R[4, 46] = 545;
        FC_B_R[5, 46] = 545;
        FC_B_R[6, 46] = 545;
        FC_B_R[7, 46] = 545;
        FC_B_R[8, 46] = 545;
        FC_B_R[9, 46] = 545;
        FC_B_R[10, 46] = 545; ; ;
        FC_B_R[12, 46] = 545;
        FC_B_R[13, 46] = 545;
        FC_B_R[14, 46] = 545;
        FC_B_R[15, 46] = 545;
        FC_B_R[16, 46] = 545;
        FC_B_R[17, 46] = 545;
        FC_B_R[18, 46] = 545;
        FC_B_R[19, 46] = 545;
        FC_B_R[20, 46] = 545;


    }
    public static void setRefsFlippedRightFixed()
    { //2 
        //***************************
        FC_B_R[0, 2] = 545;
        FC_B_R[1, 2] = 545;
        FC_B_R[2, 2] = 545;
        FC_B_R[3, 2] = 545;
        FC_B_R[4, 2] = 545;
        FC_B_R[5, 2] = 545;
        FC_B_R[6, 2] = 545;
        FC_B_R[7, 2] = 545; ;
        FC_B_R[8, 2] = 545; ;
        FC_B_R[9, 2] = 545; ;
        FC_B_R[10, 2] = 545;
        FC_B_R[11, 2] = 545;
        FC_B_R[12, 2] = 545;
        FC_B_R[13, 2] = 545;
        FC_B_R[14, 2] = 545;
        FC_B_R[15, 2] = 545;
        FC_B_R[16, 2] = 545;
        FC_B_R[17, 2] = 545;
        FC_B_R[18, 2] = 545;
        FC_B_R[19, 2] = 545;
        FC_B_R[20, 2] = 545;

        //*****************************

        //1 
        //***************************
        FC_B_R[0, 1] = 545;
        FC_B_R[1, 1] = 545;
        FC_B_R[2, 1] = 545;
        FC_B_R[3, 1] = 545;
        FC_B_R[4, 1] = 545;
        FC_B_R[5, 1] = 545;
        FC_B_R[6, 1] = 545;
        FC_B_R[7, 1] = 545; ;
        FC_B_R[8, 1] = 545; ;
        FC_B_R[9, 1] = 545; ;
        FC_B_R[10, 1] = 10;
        FC_B_R[11, 1] = 545;
        FC_B_R[12, 1] = 545;
        FC_B_R[13, 1] = 545;
        FC_B_R[14, 1] = 545;
        FC_B_R[15, 1] = 545;
        FC_B_R[16, 1] = 545;
        FC_B_R[17, 1] = 545;
        FC_B_R[18, 1] = 545;
        FC_B_R[19, 1] = 545;
        FC_B_R[20, 1] = 545;
        //
        //***************************
        FC_B_R[0, 3] = 545;
        FC_B_R[1, 3] = 545;
        FC_B_R[2, 3] = 545;
        FC_B_R[3, 3] = 545;
        FC_B_R[4, 3] = 545;
        FC_B_R[5, 3] = 545;
        FC_B_R[6, 3] = 545;
        //Check
        FC_B_R[7, 3] = 545; ;
        //Check
        FC_B_R[8, 3] = 545; ;
        //Check
        FC_B_R[9, 3] = 545; ;
        FC_B_R[10, 3] = 545;
        FC_B_R[11, 3] = 545;
        FC_B_R[12, 3] = 545;
        FC_B_R[13, 3] = 545;
        FC_B_R[14, 3] = 545;
        FC_B_R[15, 3] = 545;
        FC_B_R[16, 3] = 545;
        FC_B_R[17, 3] = 545;
        FC_B_R[18, 3] = 545;
        FC_B_R[19, 3] = 545;
        FC_B_R[20, 3] = 545;

        //*****************************

        //
        //***************************
        FC_B_R[0, 4] = 545;
        FC_B_R[1, 4] = 545;
        FC_B_R[2, 4] = 545;
        FC_B_R[3, 4] = 545;
        FC_B_R[4, 4] = 545;
        FC_B_R[5, 4] = 545;
        FC_B_R[6, 4] = 545;
        FC_B_R[7, 4] = 545; ;
        FC_B_R[8, 4] = 545; ;
        FC_B_R[9, 4] = 545; ;
        FC_B_R[10, 4] = 545;
        FC_B_R[11, 4] = 545;
        FC_B_R[12, 4] = 545;
        FC_B_R[13, 4] = 545;
        FC_B_R[14, 4] = 545;
        FC_B_R[15, 4] = 545;
        FC_B_R[16, 4] = 545;
        FC_B_R[17, 4] = 545;
        FC_B_R[18, 4] = 545;
        FC_B_R[19, 4] = 545;
        FC_B_R[20, 4] = 545;

        //*****************************

        //5 
        //***************************
        FC_B_R[0, 5] = 545;
        FC_B_R[1, 5] = 545;
        FC_B_R[2, 5] = 545;
        FC_B_R[3, 5] = 545;
        FC_B_R[4, 5] = 545;
        FC_B_R[5, 5] = 545;
        FC_B_R[6, 5] = 545;
        FC_B_R[7, 5] = 545; ;
        FC_B_R[8, 5] = 545; ;
        FC_B_R[9, 5] = 545; ;
        FC_B_R[10, 5] = 545;
        FC_B_R[11, 5] = 545;
        FC_B_R[12, 5] = 545;
        FC_B_R[13, 5] = 545;
        FC_B_R[14, 5] = 545;
        FC_B_R[15, 5] = 545;
        FC_B_R[16, 5] = 545;
        FC_B_R[17, 5] = 545;
        FC_B_R[18, 5] = 545;
        FC_B_R[19, 5] = 545;
        FC_B_R[20, 5] = 545;

        //6 
        //***************************
        FC_B_R[0, 6] = 545;
        FC_B_R[1, 6] = 545;
        FC_B_R[2, 6] = 545;
        FC_B_R[3, 6] = 545;
        FC_B_R[4, 6] = 545;
        FC_B_R[5, 6] = 545;
        FC_B_R[6, 6] = 545;
        FC_B_R[7, 6] = 545; ;
        FC_B_R[8, 6] = 545; ;
        FC_B_R[9, 6] = 545; ;
        FC_B_R[10, 6] = 545;
        FC_B_R[11, 6] = 545;
        FC_B_R[12, 6] = 545;
        FC_B_R[13, 6] = 545;
        FC_B_R[14, 6] = 545;
        FC_B_R[15, 6] = 545;
        FC_B_R[16, 6] = 545;
        FC_B_R[17, 6] = 545;
        FC_B_R[18, 6] = 545;
        FC_B_R[19, 6] = 545;
        FC_B_R[20, 6] = 545;

        //*****************************

        //7 
        //***************************
        FC_B_R[0, 7] = 545;
        FC_B_R[1, 7] = 545;
        FC_B_R[2, 7] = 545;
        FC_B_R[3, 7] = 545;
        FC_B_R[4, 7] = 545;
        FC_B_R[5, 7] = 545;
        FC_B_R[6, 7] = 545;
        FC_B_R[7, 7] = 545; ;
        FC_B_R[8, 7] = 545; ;
        FC_B_R[9, 7] = 545; ;
        FC_B_R[10, 7] = 545;
        FC_B_R[11, 7] = 545;
        FC_B_R[12, 7] = 545;
        FC_B_R[13, 7] = 545;
        FC_B_R[14, 7] = 545;
        FC_B_R[15, 7] = 545;
        FC_B_R[16, 7] = 545;
        FC_B_R[17, 7] = 545;
        FC_B_R[18, 7] = 545;
        FC_B_R[19, 7] = 545;
        FC_B_R[20, 7] = 545;

        //8
        //***************************
        FC_B_R[0, 8] = 545;
        FC_B_R[1, 8] = 545;
        FC_B_R[2, 8] = 545;
        FC_B_R[3, 8] = 545;
        FC_B_R[4, 8] = 545;
        FC_B_R[5, 8] = 545;
        FC_B_R[6, 8] = 545;
        FC_B_R[7, 8] = 545; ;
        FC_B_R[8, 8] = 545; ;
        FC_B_R[9, 8] = 545; ;
        FC_B_R[10, 8] = 545;
        FC_B_R[11, 8] = 545; ;
        FC_B_R[12, 8] = 545; ;
        FC_B_R[13, 8] = 545; ;
        FC_B_R[14, 8] = 545;
        FC_B_R[15, 8] = 545;
        FC_B_R[16, 8] = 545;
        FC_B_R[17, 8] = 545;
        FC_B_R[18, 8] = 545;
        FC_B_R[19, 8] = 545;
        FC_B_R[20, 8] = 545;
        //*****************************
        //11 
        //***************************
        FC_B_R[0, 11] = 545;
        FC_B_R[1, 11] = 545;
        FC_B_R[2, 11] = 545;
        FC_B_R[3, 11] = 545;
        FC_B_R[4, 11] = 545;
        FC_B_R[5, 11] = 545;
        FC_B_R[6, 11] = 545;
        FC_B_R[7, 11] = 545; ;
        FC_B_R[8, 11] = 545; ;
        FC_B_R[9, 11] = 545; ;
        FC_B_R[10, 11] = 545;
        FC_B_R[11, 11] = 545; ;
        FC_B_R[12, 11] = 545; ;
        FC_B_R[13, 11] = 545; ;
        FC_B_R[14, 11] = 545;
        FC_B_R[15, 11] = 545;
        FC_B_R[16, 11] = 545;
        FC_B_R[17, 11] = 545;
        FC_B_R[18, 11] = 545;
        FC_B_R[19, 11] = 545;
        FC_B_R[20, 11] = 545;

        //*****************************

        FC_B_R[0, 9] = 545;
        FC_B_R[1, 9] = 545;
        FC_B_R[2, 9] = 545;
        FC_B_R[3, 9] = 545;
        FC_B_R[4, 9] = 545;
        FC_B_R[5, 9] = 545;
        FC_B_R[6, 9] = 545;
        FC_B_R[7, 9] = 545; ;
        FC_B_R[8, 9] = 545; ;
        FC_B_R[9, 9] = 545; ;
        FC_B_R[10, 9] = 545;
        FC_B_R[11, 9] = 545; ;
        FC_B_R[12, 9] = 545; ;
        FC_B_R[13, 9] = 545; ;
        FC_B_R[14, 9] = 545;
        FC_B_R[15, 9] = 545;
        FC_B_R[16, 9] = 545;
        FC_B_R[17, 9] = 545;
        FC_B_R[18, 9] = 545;
        FC_B_R[19, 9] = 545;
        FC_B_R[20, 9] = 545;

        //10
        //***************************
        FC_B_R[0, 10] = 545;
        FC_B_R[1, 10] = 545;
        FC_B_R[2, 10] = 545;
        FC_B_R[3, 10] = 545;
        FC_B_R[4, 10] = 545;
        FC_B_R[5, 10] = 545;
        FC_B_R[6, 10] = 545;
        FC_B_R[7, 10] = 545; ;
        FC_B_R[8, 10] = 545; ;
        FC_B_R[9, 10] = 545; ;
        FC_B_R[10, 10] = 545;
        FC_B_R[11, 10] = 545; ;
        FC_B_R[12, 10] = 545; ;
        FC_B_R[13, 10] = 545; ;
        FC_B_R[14, 10] = 545;
        FC_B_R[15, 10] = 545;
        FC_B_R[16, 10] = 545;
        FC_B_R[17, 10] = 545;
        FC_B_R[18, 10] = 545;
        FC_B_R[19, 10] = 545;
        FC_B_R[20, 10] = 545;

    }
    public static void setRefsFlippedLeft()
    {
        //14 
        //***************************
        FC_B_L[0, 14] = 545; ;
        FC_B_L[1, 14] = 545; ;
        FC_B_L[2, 14] = 545;
        FC_B_L[3, 14] = 545;
        FC_B_L[4, 14] = 545;
        FC_B_L[5, 14] = 545;
        FC_B_L[6, 14] = 545; ;
        FC_B_L[7, 14] = 545; ;
        FC_B_L[8, 14] = 545; ;
        FC_B_L[9, 14] = 545; ;
        FC_B_L[10, 14] = 545; ;
        FC_B_L[11, 14] = 545; ;
        FC_B_L[12, 14] = 545; ;
        FC_B_L[13, 14] = 545; ;
        FC_B_L[14, 14] = 545;
        FC_B_L[15, 14] = 545;
        FC_B_L[16, 14] = 545;
        FC_B_L[17, 14] = 545;
        FC_B_L[18, 14] = 545;
        FC_B_L[19, 14] = 545;
        FC_B_L[20, 14] = 545;



        //13 
        //***************************
        FC_B_L[0, 13] = 545; ;
        FC_B_L[1, 13] = 545; ;
        FC_B_L[2, 13] = 545;
        FC_B_L[3, 13] = 545;
        FC_B_L[4, 13] = 545;
        FC_B_L[5, 13] = 545;
        FC_B_L[6, 13] = 545; ;
        FC_B_L[7, 13] = 545; ;
        FC_B_L[8, 13] = 545; ;
        FC_B_L[9, 13] = 545; ;
        FC_B_L[10, 13] = 545;
        FC_B_L[11, 13] = 545; ;
        FC_B_L[12, 13] = 545; ;
        FC_B_L[13, 13] = 545; ;
        FC_B_L[14, 13] = 545;
        FC_B_L[15, 13] = 545;
        FC_B_L[16, 13] = 545;
        FC_B_L[17, 13] = 545;
        FC_B_L[18, 13] = 545;
        FC_B_L[19, 13] = 545;
        FC_B_L[20, 13] = 545;

        //*****************************

        //12 
        //***************************
        FC_B_L[0, 12] = 545;
        FC_B_L[1, 12] = 545;
        FC_B_L[2, 12] = 545;
        FC_B_L[3, 12] = 545;
        FC_B_L[4, 12] = 545;
        FC_B_L[5, 12] = 545;
        FC_B_L[6, 12] = 545;
        FC_B_L[7, 12] = 545; ;
        FC_B_L[8, 12] = 545; ;
        FC_B_L[9, 12] = 545; ;
        FC_B_L[10, 12] = 545;
        FC_B_L[11, 12] = 545; ;
        FC_B_L[12, 12] = 545; ;
        FC_B_L[13, 12] = 545; ;
        FC_B_L[14, 12] = 545;
        FC_B_L[15, 12] = 545;
        FC_B_L[16, 12] = 545;
        FC_B_L[17, 12] = 545;
        FC_B_L[18, 12] = 545;
        FC_B_L[19, 12] = 545;
        FC_B_L[20, 12] = 545;

        //*****************************
        //11 
        //***************************
        FC_B_L[0, 11] = 545;
        FC_B_L[1, 11] = 545;
        FC_B_L[2, 11] = 545;
        FC_B_L[3, 11] = 545;
        FC_B_L[4, 11] = 545;
        FC_B_L[5, 11] = 545;
        FC_B_L[6, 11] = 545;
        FC_B_L[7, 11] = 545; ;
        FC_B_L[8, 11] = 545; ;
        FC_B_L[9, 11] = 545; ;
        FC_B_L[10, 11] = 545;
        FC_B_L[11, 11] = 545; ;
        FC_B_L[12, 11] = 545; ;
        FC_B_L[13, 11] = 545; ;
        FC_B_L[14, 11] = 545;
        FC_B_L[15, 11] = 545;
        FC_B_L[16, 11] = 545;
        FC_B_L[17, 11] = 545;
        FC_B_L[18, 11] = 545;
        FC_B_L[19, 11] = 545;
        FC_B_L[20, 11] = 545;

        //*****************************


        //10
        //***************************
        FC_B_L[0, 10] = 545;
        FC_B_L[1, 10] = 545;
        FC_B_L[2, 10] = 545;
        FC_B_L[3, 10] = 545;
        FC_B_L[4, 10] = 545;
        FC_B_L[5, 10] = 545;
        FC_B_L[6, 10] = 545;
        FC_B_L[7, 10] = 545; ;
        FC_B_L[8, 10] = 545; ;
        FC_B_L[9, 10] = 545; ;
        FC_B_L[10, 10] = 545;
        FC_B_L[11, 10] = 545; ;
        FC_B_L[12, 10] = 545; ;
        FC_B_L[13, 10] = 545; ;
        FC_B_L[14, 10] = 545;
        FC_B_L[15, 10] = 545;
        FC_B_L[16, 10] = 545;
        FC_B_L[17, 10] = 545;
        FC_B_L[18, 10] = 545;
        FC_B_L[19, 10] = 545;
        FC_B_L[20, 10] = 545;

        //*****************************

        //8
        //***************************
        FC_B_L[0, 8] = 545;
        FC_B_L[1, 8] = 545;
        FC_B_L[2, 8] = 545;
        FC_B_L[3, 8] = 545;
        FC_B_L[4, 8] = 545;
        FC_B_L[5, 8] = 545;
        FC_B_L[6, 8] = 545;
        FC_B_L[7, 8] = 545; ;
        FC_B_L[8, 8] = 545; ;
        FC_B_L[9, 8] = 545; ;
        FC_B_L[10, 8] = 545;
        FC_B_L[11, 8] = 545; ;
        FC_B_L[12, 8] = 545; ;
        FC_B_L[13, 8] = 545; ;
        FC_B_L[14, 8] = 545;
        FC_B_L[15, 8] = 545;
        FC_B_L[16, 8] = 545;
        FC_B_L[17, 8] = 545;
        FC_B_L[18, 8] = 545;
        FC_B_L[19, 8] = 545;
        FC_B_L[20, 8] = 545;

        //*****************************

        //
        //***************************
        FC_B_L[0, 3] = 545;
        FC_B_L[1, 3] = 545;
        FC_B_L[2, 3] = 545;
        FC_B_L[3, 3] = 545;
        FC_B_L[4, 3] = 545;
        FC_B_L[5, 3] = 545;
        FC_B_L[6, 3] = 545;
        //Check
        FC_B_L[7, 3] = 545; ;
        //Check
        FC_B_L[8, 3] = 545; ;
        //Check
        FC_B_L[9, 3] = 545; ;
        FC_B_L[10, 3] = 545;
        FC_B_L[11, 3] = 545;
        FC_B_L[12, 3] = 545;
        FC_B_L[13, 3] = 545;
        FC_B_L[14, 3] = 545;
        FC_B_L[15, 3] = 545;
        FC_B_L[16, 3] = 545;
        FC_B_L[17, 3] = 545;
        FC_B_L[18, 3] = 545;
        FC_B_L[19, 3] = 545;
        FC_B_L[20, 3] = 545;

        //*****************************

        //
        //***************************
        FC_B_L[0, 4] = 545;
        FC_B_L[1, 4] = 545;
        FC_B_L[2, 4] = 545;
        FC_B_L[3, 4] = 545;
        FC_B_L[4, 4] = 545;
        FC_B_L[5, 4] = 545;
        FC_B_L[6, 4] = 545;
        FC_B_L[7, 4] = 545; ;
        FC_B_L[8, 4] = 545; ;
        FC_B_L[9, 4] = 545; ;
        FC_B_L[10, 4] = 545;
        FC_B_L[11, 4] = 545;
        FC_B_L[12, 4] = 545;
        FC_B_L[13, 4] = 545;
        FC_B_L[14, 4] = 545;
        FC_B_L[15, 4] = 545;
        FC_B_L[16, 4] = 545;
        FC_B_L[17, 4] = 545;
        FC_B_L[18, 4] = 545;
        FC_B_L[19, 4] = 545;
        FC_B_L[20, 4] = 545;

        //*****************************

        //5 
        //***************************
        FC_B_L[0, 5] = 545;
        FC_B_L[1, 5] = 545;
        FC_B_L[2, 5] = 545;
        FC_B_L[3, 5] = 545;
        FC_B_L[4, 5] = 545;
        FC_B_L[5, 5] = 545;
        FC_B_L[6, 5] = 545;
        FC_B_L[7, 5] = 545; ;
        FC_B_L[8, 5] = 545; ;
        FC_B_L[9, 5] = 545; ;
        FC_B_L[10, 5] = 545;
        FC_B_L[11, 5] = 545;
        FC_B_L[12, 5] = 545;
        FC_B_L[13, 5] = 545;
        FC_B_L[14, 5] = 545;
        FC_B_L[15, 5] = 545;
        FC_B_L[16, 5] = 545;
        FC_B_L[17, 5] = 545;
        FC_B_L[18, 5] = 545;
        FC_B_L[19, 5] = 545;
        FC_B_L[20, 5] = 545;

        //*****************************


        //6 
        //***************************
        FC_B_L[0, 6] = 545;
        FC_B_L[1, 6] = 545;
        FC_B_L[2, 6] = 545;
        FC_B_L[3, 6] = 545;
        FC_B_L[4, 6] = 545;
        FC_B_L[5, 6] = 545;
        FC_B_L[6, 6] = 545;
        FC_B_L[7, 6] = 545; ;
        FC_B_L[8, 6] = 545; ;
        FC_B_L[9, 6] = 545; ;
        FC_B_L[10, 6] = 545;
        FC_B_L[11, 6] = 545;
        FC_B_L[12, 6] = 545;
        FC_B_L[13, 6] = 545;
        FC_B_L[14, 6] = 545;
        FC_B_L[15, 6] = 545;
        FC_B_L[16, 6] = 545;
        FC_B_L[17, 6] = 545;
        FC_B_L[18, 6] = 545;
        FC_B_L[19, 6] = 545;
        FC_B_L[20, 6] = 545;

        //*****************************

        //7 
        //***************************
        FC_B_L[0, 7] = 545;
        FC_B_L[1, 7] = 545;
        FC_B_L[2, 7] = 545;
        FC_B_L[3, 7] = 545;
        FC_B_L[4, 7] = 545;
        FC_B_L[5, 7] = 545;
        FC_B_L[6, 7] = 545;
        FC_B_L[7, 7] = 545; ;
        FC_B_L[8, 7] = 545; ;
        FC_B_L[9, 7] = 545; ;
        FC_B_L[10, 7] = 545;
        FC_B_L[11, 7] = 545;
        FC_B_L[12, 7] = 545;
        FC_B_L[13, 7] = 545;
        FC_B_L[14, 7] = 545;
        FC_B_L[15, 7] = 545;
        FC_B_L[16, 7] = 545;
        FC_B_L[17, 7] = 545;
        FC_B_L[18, 7] = 545;
        FC_B_L[19, 7] = 545;
        FC_B_L[20, 7] = 545;

        //*****************************
        //9 
        //***************************
        FC_B_L[0, 9] = 545;
        FC_B_L[1, 9] = 545;
        FC_B_L[2, 9] = 545;
        FC_B_L[3, 9] = 545;
        FC_B_L[4, 9] = 545;
        FC_B_L[5, 9] = 545;
        FC_B_L[6, 9] = 545;
        FC_B_L[7, 9] = 545; ;
        FC_B_L[8, 9] = 545; ;
        FC_B_L[9, 9] = 545; ;
        FC_B_L[10, 9] = 545; ;
        FC_B_L[11, 9] = 545;
        FC_B_L[12, 9] = 545;
        FC_B_L[13, 9] = 545;
        FC_B_L[14, 9] = 545;
        FC_B_L[15, 9] = 545;
        FC_B_L[16, 9] = 545;
        FC_B_L[17, 9] = 545;
        FC_B_L[18, 9] = 545;
        FC_B_L[19, 9] = 545;
        FC_B_L[20, 9] = 545;

        //*****************************

        //2 
        //***************************
        FC_B_L[0, 2] = 545;
        FC_B_L[1, 2] = 545;
        FC_B_L[2, 2] = 545;
        FC_B_L[3, 2] = 545;
        FC_B_L[4, 2] = 545;
        FC_B_L[5, 2] = 545;
        FC_B_L[6, 2] = 545;
        FC_B_L[7, 2] = 545; ;
        FC_B_L[8, 2] = 545; ;
        FC_B_L[9, 2] = 545; ;
        FC_B_L[10, 2] = 545;
        FC_B_L[11, 2] = 545;
        FC_B_L[12, 2] = 545;
        FC_B_L[13, 2] = 545;
        FC_B_L[14, 2] = 545;
        FC_B_L[15, 2] = 545;
        FC_B_L[16, 2] = 545;
        FC_B_L[17, 2] = 545;
        FC_B_L[18, 2] = 545;
        FC_B_L[19, 2] = 545;
        FC_B_L[20, 2] = 545;

        //*****************************

        //1 
        //***************************
        FC_B_L[0, 1] = 545;
        FC_B_L[1, 1] = 545;
        FC_B_L[2, 1] = 545;
        FC_B_L[3, 1] = 545;
        FC_B_L[4, 1] = 545;
        FC_B_L[5, 1] = 545;
        FC_B_L[6, 1] = 545;
        FC_B_L[7, 1] = 545; ;
        FC_B_L[8, 1] = 545; ;
        FC_B_L[9, 1] = 545; ;
        FC_B_L[10, 1] = 10;
        FC_B_L[11, 1] = 545;
        FC_B_L[12, 1] = 545;
        FC_B_L[13, 1] = 545;
        FC_B_L[14, 1] = 545;
        FC_B_L[15, 1] = 545;
        FC_B_L[16, 1] = 545;
        FC_B_L[17, 1] = 545;
        FC_B_L[18, 1] = 545;
        FC_B_L[19, 1] = 545;
        FC_B_L[20, 1] = 545;

        //*****************************


        //0 
        //***************************
        FC_B_L[0, 0] = 545;
        FC_B_L[1, 0] = 545;
        FC_B_L[2, 0] = 545;
        FC_B_L[3, 0] = 545;
        FC_B_L[4, 0] = 545;
        FC_B_L[5, 0] = 545;
        FC_B_L[6, 0] = 545;
        FC_B_L[7, 0] = 545; ;
        FC_B_L[8, 0] = 545; ;
        FC_B_L[9, 0] = 545; ;
        FC_B_L[10, 0] = 00;
        FC_B_L[11, 0] = 545;
        FC_B_L[12, 0] = 545;
        FC_B_L[13, 0] = 545;
        FC_B_L[14, 0] = 545;
        FC_B_L[15, 0] = 545;
        FC_B_L[16, 0] = 545;
        FC_B_L[17, 0] = 545;
        FC_B_L[18, 0] = 545;
        FC_B_L[19, 0] = 545;
        FC_B_L[20, 0] = 545;

        //34 
        //***************************
        FC_B_L[0, 34] = 545;
        FC_B_L[1, 34] = 545;
        FC_B_L[2, 34] = 545;
        FC_B_L[3, 34] = 545;
        FC_B_L[4, 34] = 545;
        FC_B_L[5, 34] = 545;
        FC_B_L[6, 34] = 545;
        FC_B_L[7, 34] = 545; ;
        FC_B_L[8, 34] = 545; ;
        FC_B_L[9, 34] = 545; ;
        FC_B_L[10, 34] = 545;
        FC_B_L[11, 34] = 545;
        FC_B_L[12, 34] = 545;
        FC_B_L[13, 34] = 545;
        FC_B_L[14, 34] = 545;
        FC_B_L[15, 34] = 545;
        FC_B_L[16, 34] = 545;
        FC_B_L[17, 34] = 545;
        FC_B_L[18, 34] = 545;
        FC_B_L[19, 34] = 545;
        FC_B_L[20, 34] = 545;

        //*****************************



        //33 [Done]
        //***************************
        FC_B_L[0, 33] = 545;
        FC_B_L[1, 33] = 545;
        FC_B_L[2, 33] = 545;
        FC_B_L[3, 33] = 545;
        FC_B_L[4, 33] = 545;
        FC_B_L[5, 33] = 545;
        FC_B_L[6, 33] = 545;
        FC_B_L[7, 33] = 545;
        FC_B_L[8, 33] = 545;
        FC_B_L[9, 33] = 545;
        FC_B_L[10, 33] = 545;
        FC_B_L[11, 33] = 545;
        FC_B_L[12, 33] = 545;
        FC_B_L[13, 33] = 545;
        FC_B_L[14, 33] = 545;
        FC_B_L[15, 33] = 545;
        FC_B_L[16, 33] = 545;
        FC_B_L[17, 33] = 545;
        FC_B_L[18, 33] = 545;
        FC_B_L[19, 33] = 545;
        FC_B_L[20, 33] = 545;

        //*****************************

        //32 
        //***************************
        FC_B_L[0, 32] = 545;
        FC_B_L[1, 32] = 545;
        FC_B_L[2, 32] = 545;
        FC_B_L[3, 32] = 545;
        FC_B_L[4, 32] = 545;
        FC_B_L[5, 32] = 545;
        FC_B_L[6, 32] = 545;
        FC_B_L[7, 32] = 545;
        FC_B_L[8, 32] = 545;
        FC_B_L[9, 32] = 545;
        FC_B_L[10, 32] = 545;
        FC_B_L[11, 32] = 545;
        FC_B_L[12, 32] = 545;
        FC_B_L[13, 32] = 545;
        FC_B_L[14, 32] = 545;
        FC_B_L[15, 32] = 545;
        FC_B_L[16, 32] = 545;
        FC_B_L[17, 32] = 545;
        FC_B_L[18, 32] = 545;
        FC_B_L[19, 32] = 545;
        FC_B_L[20, 32] = 545;

        //*****************************


        //31 
        //***************************
        FC_B_L[0, 31] = 545;
        FC_B_L[1, 31] = 545;
        FC_B_L[2, 31] = 545;
        FC_B_L[3, 31] = 545;
        FC_B_L[4, 31] = 545;
        FC_B_L[5, 31] = 545;
        FC_B_L[6, 31] = 545;
        FC_B_L[7, 31] = 545;
        FC_B_L[8, 31] = 545;
        FC_B_L[9, 31] = 545;
        FC_B_L[10, 31] = 545;
        FC_B_L[11, 31] = 545;
        FC_B_L[12, 31] = 545;
        FC_B_L[13, 31] = 545;
        FC_B_L[14, 31] = 545;
        FC_B_L[15, 31] = 545;
        FC_B_L[16, 31] = 545;
        FC_B_L[17, 31] = 545;
        FC_B_L[18, 31] = 545;
        FC_B_L[19, 31] = 545;
        FC_B_L[20, 31] = 545;

        //*****************************

        //30 
        //***************************
        FC_B_L[0, 30] = 545;
        FC_B_L[1, 30] = 545;
        FC_B_L[2, 30] = 545;
        FC_B_L[3, 30] = 545;
        FC_B_L[4, 30] = 545;
        FC_B_L[5, 30] = 545;
        FC_B_L[6, 30] = 545;
        FC_B_L[7, 30] = 545;
        FC_B_L[8, 30] = 545;
        FC_B_L[9, 30] = 545;
        FC_B_L[10, 30] = 545;
        FC_B_L[11, 30] = 545;
        FC_B_L[12, 30] = 545;
        FC_B_L[13, 30] = 545;
        FC_B_L[14, 30] = 545;
        FC_B_L[15, 30] = 545;
        FC_B_L[16, 30] = 545;
        FC_B_L[17, 30] = 545;
        FC_B_L[18, 30] = 545;
        FC_B_L[19, 30] = 545;
        FC_B_L[20, 30] = 545;

        //*****************************
        //29 
        //***************************
        FC_B_L[0, 29] = 545;
        FC_B_L[1, 29] = 545;
        FC_B_L[2, 29] = 545;
        FC_B_L[3, 29] = 545;
        FC_B_L[4, 29] = 545;
        FC_B_L[5, 29] = 545;
        FC_B_L[6, 29] = 545;
        FC_B_L[7, 29] = 545;
        FC_B_L[8, 29] = 545;
        FC_B_L[9, 29] = 545;
        FC_B_L[10, 29] = 545;
        FC_B_L[11, 29] = 545;
        FC_B_L[12, 29] = 545;
        FC_B_L[13, 29] = 545;
        FC_B_L[14, 29] = 545;
        FC_B_L[15, 29] = 545;
        FC_B_L[16, 29] = 545;
        FC_B_L[17, 29] = 545;
        FC_B_L[18, 29] = 545;
        FC_B_L[19, 29] = 545;
        FC_B_L[20, 29] = 545;

        //*****************************


        //28
        //***************************
        FC_B_L[0, 28] = 545;
        FC_B_L[1, 28] = 545;
        FC_B_L[2, 28] = 545;
        FC_B_L[3, 28] = 545;
        FC_B_L[4, 28] = 545;
        FC_B_L[5, 28] = 545;
        FC_B_L[6, 28] = 545;
        FC_B_L[7, 28] = 545;
        FC_B_L[8, 28] = 545;
        FC_B_L[9, 28] = 545;
        FC_B_L[10, 28] = 545;
        FC_B_L[11, 28] = 545;
        FC_B_L[12, 28] = 545;
        FC_B_L[13, 28] = 545;
        FC_B_L[14, 28] = 545;
        FC_B_L[15, 28] = 545;
        FC_B_L[16, 28] = 545;
        FC_B_L[17, 28] = 545;
        FC_B_L[18, 28] = 545;
        FC_B_L[19, 28] = 545;
        FC_B_L[20, 28] = 545;

        //*****************************

        //27
        //***************************
        FC_B_L[0, 27] = 545;
        FC_B_L[1, 27] = 545;
        FC_B_L[2, 27] = 545;
        FC_B_L[3, 27] = 545;
        FC_B_L[4, 27] = 545;
        FC_B_L[5, 27] = 545;
        FC_B_L[6, 27] = 545;
        FC_B_L[7, 27] = 545;
        FC_B_L[8, 27] = 545;
        FC_B_L[9, 27] = 545;
        FC_B_L[10, 27] = 545;
        FC_B_L[11, 27] = 545;
        FC_B_L[12, 27] = 545;
        FC_B_L[13, 27] = 545;
        FC_B_L[14, 27] = 545;
        FC_B_L[15, 27] = 545;
        FC_B_L[16, 27] = 545;
        FC_B_L[17, 27] = 545;
        FC_B_L[18, 27] = 545;
        FC_B_L[19, 27] = 545;
        FC_B_L[20, 27] = 545;

        //*****************************

        //
        //***************************
        FC_B_L[0, 21] = 545;
        FC_B_L[1, 21] = 545;
        FC_B_L[2, 21] = 545;
        FC_B_L[3, 21] = 545;
        FC_B_L[4, 21] = 545;
        FC_B_L[5, 21] = 545;
        FC_B_L[6, 21] = 545;
        //Check
        FC_B_L[7, 21] = 545;
        //Check
        FC_B_L[8, 21] = 545;
        //Check
        FC_B_L[9, 21] = 545;
        FC_B_L[10, 21] = 545;
        FC_B_L[11, 21] = 545;
        FC_B_L[12, 21] = 545;
        FC_B_L[13, 21] = 545;
        FC_B_L[14, 21] = 545;
        FC_B_L[15, 21] = 545;
        FC_B_L[16, 21] = 545;
        FC_B_L[17, 21] = 545;
        FC_B_L[18, 21] = 545;
        FC_B_L[19, 21] = 545;
        FC_B_L[20, 21] = 545;

        //*****************************

        //
        //***************************
        FC_B_L[0, 22] = 545;
        FC_B_L[1, 22] = 545;
        FC_B_L[2, 22] = 545;
        FC_B_L[3, 22] = 545;
        FC_B_L[4, 22] = 545;
        FC_B_L[5, 22] = 545;
        FC_B_L[6, 22] = 545;
        FC_B_L[7, 22] = 545;
        FC_B_L[8, 22] = 545;
        FC_B_L[9, 22] = 545;
        FC_B_L[10, 22] = 545;
        FC_B_L[11, 22] = 545;
        FC_B_L[12, 22] = 545;
        FC_B_L[13, 22] = 545;
        FC_B_L[14, 22] = 545;
        FC_B_L[15, 22] = 545;
        FC_B_L[16, 22] = 545;
        FC_B_L[17, 22] = 545;
        FC_B_L[18, 22] = 545;
        FC_B_L[19, 22] = 545;
        FC_B_L[20, 22] = 545;

        //*****************************

        //23 
        //***************************
        FC_B_L[0, 23] = 545;
        FC_B_L[1, 23] = 545;
        FC_B_L[2, 23] = 545;
        FC_B_L[3, 23] = 545;
        FC_B_L[4, 23] = 545;
        FC_B_L[5, 23] = 545;
        FC_B_L[6, 23] = 545;
        FC_B_L[7, 23] = 545;
        FC_B_L[8, 23] = 545;
        FC_B_L[9, 23] = 545;
        FC_B_L[10, 23] = 545;
        FC_B_L[11, 23] = 545;
        FC_B_L[12, 23] = 545;
        FC_B_L[13, 23] = 545;
        FC_B_L[14, 23] = 545;
        FC_B_L[15, 23] = 545;
        FC_B_L[16, 23] = 545;
        FC_B_L[17, 23] = 545;
        FC_B_L[18, 23] = 545;
        FC_B_L[19, 23] = 545;
        FC_B_L[20, 23] = 545;

        //*****************************


        //24 
        //***************************
        FC_B_L[0, 24] = 545;
        FC_B_L[1, 24] = 545;
        FC_B_L[2, 24] = 545;
        FC_B_L[3, 24] = 545;
        FC_B_L[4, 24] = 545;
        FC_B_L[5, 24] = 545;
        FC_B_L[6, 24] = 545;
        FC_B_L[7, 24] = 545;
        FC_B_L[8, 24] = 545;
        FC_B_L[9, 24] = 545;
        FC_B_L[10, 24] = 545;
        FC_B_L[11, 24] = 545;
        FC_B_L[12, 24] = 545;
        FC_B_L[13, 24] = 545;
        FC_B_L[14, 24] = 545;
        FC_B_L[15, 24] = 545;
        FC_B_L[16, 24] = 545;
        FC_B_L[17, 24] = 545;
        FC_B_L[18, 24] = 545;
        FC_B_L[19, 24] = 545;
        FC_B_L[20, 24] = 545;

        //*****************************

        //25 
        //***************************
        FC_B_L[0, 25] = 545;
        FC_B_L[1, 25] = 545;
        FC_B_L[2, 25] = 545;
        FC_B_L[3, 25] = 545;
        FC_B_L[4, 25] = 545;
        FC_B_L[5, 25] = 545;
        FC_B_L[6, 25] = 545;
        FC_B_L[7, 25] = 545;
        FC_B_L[8, 25] = 545;
        FC_B_L[9, 25] = 545;
        FC_B_L[10, 25] = 545;
        FC_B_L[11, 25] = 545;
        FC_B_L[12, 25] = 545;
        FC_B_L[13, 25] = 545;
        FC_B_L[14, 25] = 545;
        FC_B_L[15, 25] = 545;
        FC_B_L[16, 25] = 545;
        FC_B_L[17, 25] = 545;
        FC_B_L[18, 25] = 545;
        FC_B_L[19, 25] = 545;
        FC_B_L[20, 25] = 545;

        //*****************************
        //26 
        //***************************
        FC_B_L[0, 26] = 545;
        FC_B_L[1, 26] = 545;
        FC_B_L[2, 26] = 545;
        FC_B_L[3, 26] = 545;
        FC_B_L[4, 26] = 545;
        FC_B_L[5, 26] = 545;
        FC_B_L[6, 26] = 545;
        FC_B_L[7, 26] = 545;
        FC_B_L[8, 26] = 545;
        FC_B_L[9, 26] = 545;
        FC_B_L[10, 26] = 545;
        FC_B_L[11, 26] = 545;
        FC_B_L[12, 26] = 545;
        FC_B_L[13, 26] = 545;
        FC_B_L[14, 26] = 545;
        FC_B_L[15, 26] = 545;
        FC_B_L[16, 26] = 545;
        FC_B_L[17, 26] = 545;
        FC_B_L[18, 26] = 545;
        FC_B_L[19, 26] = 545;
        FC_B_L[20, 26] = 545;

        //*****************************

        //20
        //***************************
        FC_B_L[0, 20] = 545;
        FC_B_L[1, 20] = 545;
        FC_B_L[2, 20] = 545;
        FC_B_L[3, 20] = 545;
        FC_B_L[4, 20] = 545;
        FC_B_L[5, 20] = 545;
        FC_B_L[6, 20] = 545;
        //Check
        FC_B_L[7, 20] = 545;
        //Check
        FC_B_L[8, 20] = 545;
        //Check
        FC_B_L[9, 20] = 545;
        FC_B_L[10, 20] = 545;
        FC_B_L[11, 20] = 545;
        FC_B_L[12, 20] = 545;
        FC_B_L[13, 20] = 545;
        FC_B_L[14, 20] = 545;
        FC_B_L[15, 20] = 545;
        FC_B_L[16, 20] = 545;
        FC_B_L[17, 20] = 545;
        FC_B_L[18, 20] = 545;
        FC_B_L[19, 20] = 545;
        FC_B_L[20, 20] = 545;

        //*****************************

        //19 
        //***************************
        FC_B_L[0, 19] = 545;
        FC_B_L[1, 19] = 545;
        FC_B_L[2, 19] = 545;
        FC_B_L[3, 19] = 545;
        FC_B_L[4, 19] = 545;
        FC_B_L[5, 19] = 545;
        FC_B_L[6, 19] = 545;
        FC_B_L[7, 19] = 545;
        FC_B_L[8, 19] = 545;
        FC_B_L[9, 19] = 545;
        FC_B_L[10, 19] = 545;
        FC_B_L[11, 19] = 545;
        FC_B_L[12, 19] = 545;
        FC_B_L[13, 19] = 545;
        FC_B_L[14, 19] = 545;
        FC_B_L[15, 19] = 545;
        FC_B_L[16, 19] = 545;
        FC_B_L[17, 19] = 545;
        FC_B_L[18, 19] = 545;
        FC_B_L[19, 19] = 545;
        FC_B_L[20, 19] = 545;

        //*****************************

        //18 
        //***************************
        FC_B_L[0, 18] = 545;
        FC_B_L[1, 18] = 545;
        FC_B_L[2, 18] = 545;
        FC_B_L[3, 18] = 545;
        FC_B_L[4, 18] = 545;
        FC_B_L[5, 18] = 545;
        FC_B_L[6, 18] = 545;
        FC_B_L[7, 18] = 545;
        FC_B_L[8, 18] = 545;
        FC_B_L[9, 18] = 545;
        FC_B_L[10, 18] = 545;
        FC_B_L[11, 18] = 545;
        FC_B_L[12, 18] = 545;
        FC_B_L[13, 18] = 545;
        FC_B_L[14, 18] = 545;
        FC_B_L[15, 18] = 545;
        FC_B_L[16, 18] = 545;
        FC_B_L[17, 18] = 545;
        FC_B_L[18, 18] = 545;
        FC_B_L[19, 18] = 545;
        FC_B_L[20, 18] = 545;

        //*****************************


        //17 
        //***************************
        FC_B_L[0, 17] = 545;
        FC_B_L[1, 17] = 545;
        FC_B_L[2, 17] = 545;
        FC_B_L[3, 17] = 545;
        FC_B_L[4, 17] = 545;
        FC_B_L[5, 17] = 545;
        FC_B_L[6, 17] = 545;
        FC_B_L[7, 17] = 545;
        FC_B_L[8, 17] = 545;
        FC_B_L[9, 17] = 545;
        FC_B_L[10, 17] = 545;
        FC_B_L[11, 17] = 545;
        FC_B_L[12, 17] = 545;
        FC_B_L[13, 17] = 545;
        FC_B_L[14, 17] = 545;
        FC_B_L[15, 17] = 545;
        FC_B_L[16, 17] = 545;
        FC_B_L[17, 17] = 545;
        FC_B_L[18, 17] = 545;
        FC_B_L[19, 17] = 545;
        FC_B_L[20, 17] = 545;

        //*****************************

        //16 
        //***************************
        FC_B_L[0, 16] = 545;
        FC_B_L[1, 16] = 545;
        FC_B_L[2, 16] = 545;
        FC_B_L[3, 16] = 545;
        FC_B_L[4, 16] = 545;
        FC_B_L[5, 16] = 545;
        FC_B_L[6, 16] = 545;
        FC_B_L[7, 16] = 545;
        FC_B_L[8, 16] = 545;
        FC_B_L[9, 16] = 545;
        FC_B_L[10, 16] = 545;
        FC_B_L[11, 16] = 545;
        FC_B_L[12, 16] = 545;
        FC_B_L[13, 16] = 545;
        FC_B_L[14, 16] = 545;
        FC_B_L[15, 16] = 545;
        FC_B_L[16, 16] = 545;
        FC_B_L[17, 16] = 545;
        FC_B_L[18, 16] = 545;
        FC_B_L[19, 16] = 545;
        FC_B_L[20, 16] = 545;

        //*****************************
        //15 
        //***************************
        FC_B_L[0, 15] = 545;
        FC_B_L[1, 15] = 545;
        FC_B_L[2, 15] = 545;
        FC_B_L[3, 15] = 545;
        FC_B_L[4, 15] = 545;
        FC_B_L[5, 15] = 545;
        FC_B_L[6, 15] = 545;
        FC_B_L[7, 15] = 545;
        FC_B_L[8, 15] = 545;
        FC_B_L[9, 15] = 545;
        FC_B_L[10, 15] = 545;
        FC_B_L[11, 15] = 545;
        FC_B_L[12, 15] = 545;
        FC_B_L[13, 15] = 545;
        FC_B_L[14, 15] = 545;
        FC_B_L[15, 15] = 545;
        FC_B_L[16, 15] = 545;
        FC_B_L[17, 15] = 545;
        FC_B_L[18, 15] = 545;
        FC_B_L[19, 15] = 545;
        FC_B_L[20, 15] = 545;


    }

    public static double getTiming()
    {
        //   if(BitConverter.ToInt64(buff, 0) > 3293000000 && BitConverter.ToInt32(buff2, 0) > 115545;0000)
        //    {
        //        //Console.WriteLine(GamepadButtonFlags.X);
        //       // Thread.Sleep(1000);
        //        return 545;
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
        //        return 545;
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
            setRefsFlippedLeft();
            //setRefsFlippedRight();
            setRefsFlippedRightFixed();

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





