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
    static Int64 baseA = 0x7FF69C427620;
    static double[,] FC_N_R = new double[25,60]; //Freecourt normal right side
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
        //oThread2.Start();
        h.Update();
        //uint ucode = (uint)Keys.A;
        //uint scancode = MapVirtualKey(ucode, MAPVK_VK_TO_VSC);
        //its doing backsapce
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
            Console.WriteLine(getTimingA());
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
        int a = (int)((rightLeft / divisor) - 3234);
        int b = ((int) ((upDown / divisor)) - 1126);
       
        
     
        Console.WriteLine(b + " dsad");
        Thread.Sleep(5000);
        return FC_N_R[a, b];
    }

    public void setRefs()
    {
        
        //0,60
        FC_N_R[0,60] = 515;
        FC_N_R[0, 59] = 515;
        FC_N_R[0, 58] = 515;
        FC_N_R[0, 57] = 515;
        FC_N_R[0, 56] = 515;
        FC_N_R[0, 55] = 515;
        FC_N_R[0, 54] = 515;
        FC_N_R[0, 53] = 515;
        FC_N_R[0, 52] = 515;
    }

    public static double getTiming()
    {
        //   if(BitConverter.ToInt64(buff, 0) > 3293000000 && BitConverter.ToInt32(buff2, 0) > 1154000000)
        //    {
        //        //Console.WriteLine(GamepadButtonFlags.X);
        //       // Thread.Sleep(1000);
        //        return 515;
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

            bytesRead = 0;
            byte[] buffer = new byte[24]; //'Hello World!' takes 12*2 bytes because of Unicode 
            buff = new byte[8];

            bytesRead2 = 0;
            byte[] buffer2 = new byte[24]; //'Hello World!' takes 12*2 bytes because of Unicode 
            buff2 = new byte[8];
            // x.Update();
            Console.WriteLine(buffer2[0] + " (" + bytesRead2.ToString() + "bytes)");
            //while (1 == 1)
            //{
                Console.Clear();

                // 0x0046A3B8 is the address where I found the string, replace it with what you found
                try
                {
                    ReadProcessMemory((int)processHandle, baseA - 0x10, buffer, buffer.Length, ref bytesRead);
                    ReadProcessMemory((int)processHandle, baseA - 0x02, buffer2, buffer2.Length, ref bytesRead2);
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
                rightLeft = BitConverter.ToInt64(buff, 0);
                upDown = BitConverter.ToInt32(buff2, 0);
                // x.Update();

                //if works
                //if (Keyboard.IsKeyDown(Key.NumPad5))
                //{
                //    for (int i = 0; i < 10000; i++)
                //    {
                //        SendKey(0x65);
                //    }

                //    //keybd_event(0x65, 0, 0, (System.UIntPtr) 0); // KEY_DOWN
                //    //Console.WriteLine("Up Dowfaddfadan ");
                //    //System.Threading.Thread.Sleep(1000);

                //    //keybd_event(0x65, 0, 0x02, (System.UIntPtr)0); // KEY_UP

                //    // KeyboardA.HoldKey((byte)Key.NumPad5, 10500);


                //}
                Thread.Sleep(500);
            }

        }
    }

//}





