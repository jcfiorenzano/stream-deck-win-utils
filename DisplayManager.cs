/**
 * Most of the code in this file was taken from the repository https://github.com/timmui/ScreenResolutionChanger
 */

using System;
using System.Runtime.InteropServices;
using WindowsResolutionChanger.Models;

namespace WindowsResolutionChanger
{
    public class DisplayManager
    {
        [Flags()]
        enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop. This is the default display device.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [Flags()]
        enum ChangeDisplaySettingsFlags : uint
        {
            CDS_NONE = 0,
            CDS_UPDATEREGISTRY = 0x00000001,
            CDS_TEST = 0x00000002,
            CDS_FULLSCREEN = 0x00000004,
            CDS_GLOBAL = 0x00000008,
            CDS_SET_PRIMARY = 0x00000010,
            CDS_VIDEOPARAMETERS = 0x00000020,
            CDS_ENABLE_UNSAFE_MODES = 0x00000100,
            CDS_DISABLE_UNSAFE_MODES = 0x00000200,
            CDS_RESET = 0x40000000,
            CDS_RESET_EX = 0x20000000,
            CDS_NORESET = 0x10000000
        }

        [StructLayout(LayoutKind.Sequential)]
        //The DEVMODE data structure contains information about the initialization and environment of a printer or a display device.
        struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;

            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;

            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmPosition;

            public int dmDisplayFlags;
            public int dmDisplayFrequency;

            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;

            public int dmPanningWidth;
            public int dmPanningHeight;
        };

        [Flags()]
        enum DISP_CHANGE : int
        {
            SUCCESSFUL = 0,
            RESTART = 1,
            FAILED = -1,
            BADMODE = -2,
            NOTUPDATED = -3,
            BADFLAGS = -4,
            BADPARAM = -5,
            BADDUALVIEW = -6
        }

        class User_32
        {
            [DllImport("user32.dll")]
            public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
            [DllImport("user32.dll")]
            public static extern int EnumDisplaySettingsEx(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode, uint dwFlags);
            [DllImport("user32.dll")]
            public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

            public const int ENUM_CURRENT_SETTINGS = -1;
        }

        /// <summary>
        /// Change the resolution of the primary display
        /// </summary>
        /// <param name="width">resolution width in pixeles</param>
        /// <param name="height">resolution height in pixeles</param>
        /// <exception cref="Exception">Throw an exception if it is not possible to change the resolution or it cannot find the device settings.</exception>
        static public void ChangeResolution(int width, int height)
        {
            const uint EDS_RAWMODE = 0x00000002;

            DISPLAY_DEVICE display = new DISPLAY_DEVICE();

            bool foundPrimary = false;
            // The operating system indentify each device in the system with an index value. The indexes
            // are consecutives numbers starting at 0, if you want to get the device information of all monitors
            // in the computer you have to increase this value until the function return false.
            for(uint deviceIndex = 0; deviceIndex < 10; deviceIndex++)
            {
                DISPLAY_DEVICE testDisplayDevice = new DISPLAY_DEVICE();
                testDisplayDevice.cb = Marshal.SizeOf(testDisplayDevice);

                if (User_32.EnumDisplayDevices(null, deviceIndex, ref testDisplayDevice, 1))
                {
                    if ((testDisplayDevice.StateFlags & DisplayDeviceStateFlags.PrimaryDevice) == DisplayDeviceStateFlags.PrimaryDevice) 
                    {
                        foundPrimary = true;
                        display = testDisplayDevice;
                        break;
                    }
                }
            }

            if (!foundPrimary) 
            {
                throw new Exception("Unable to find the primary display device");
            }

            DEVMODE devMode = GetDevMode();

            if (0 == User_32.EnumDisplaySettingsEx(display.DeviceName, User_32.ENUM_CURRENT_SETTINGS, ref devMode, EDS_RAWMODE))
            {
                throw new Exception($"Unable to get current settings for {display.DeviceName}");
            }


            devMode.dmPelsWidth = width;
            devMode.dmPelsHeight = height;

            int displayChangeReturnVal = User_32.ChangeDisplaySettingsEx(display.DeviceName, ref devMode, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_TEST, IntPtr.Zero);

            if (displayChangeReturnVal == (int)DISP_CHANGE.FAILED)
            {
               throw new Exception("Unable to change the display resolution.");
            }

            displayChangeReturnVal = User_32.ChangeDisplaySettingsEx(display.DeviceName, ref devMode, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);

            if (displayChangeReturnVal == (int)DISP_CHANGE.SUCCESSFUL)
            {
                return;
            }

            throw new Exception("Unable to change the display resolution.");
        }


        public static ScreenResolutionModel GetCurrentResolution() 
        {
            const uint EDS_RAWMODE = 0x00000002;

            DISPLAY_DEVICE display = new DISPLAY_DEVICE();

            bool foundPrimary = false;
            for(uint deviceIndex = 0; deviceIndex < 10; deviceIndex++)
            {
                DISPLAY_DEVICE testDisplayDevice = new DISPLAY_DEVICE();
                testDisplayDevice.cb = Marshal.SizeOf(testDisplayDevice);

                if (User_32.EnumDisplayDevices(null, deviceIndex, ref testDisplayDevice, 1))
                {
                    if ((testDisplayDevice.StateFlags & DisplayDeviceStateFlags.PrimaryDevice) == DisplayDeviceStateFlags.PrimaryDevice) 
                    {
                        foundPrimary = true;
                        display = testDisplayDevice;
                        break;
                    }
                }
            }

            if (!foundPrimary) 
            {
                throw new Exception("Unable to find the primary display device");
            }

            DEVMODE devMode = GetDevMode();

            if (0 == User_32.EnumDisplaySettingsEx(display.DeviceName, User_32.ENUM_CURRENT_SETTINGS, ref devMode, EDS_RAWMODE))
            {
                throw new Exception($"Unable to get current settings for {display.DeviceName}");
            }

            return new ScreenResolutionModel
            {

                Width = devMode.dmPelsWidth,
                Heigth = devMode.dmPelsHeight
            };
        }

        private static DEVMODE GetDevMode()
        {
            DEVMODE dm = new DEVMODE();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);
            return dm;
        }
    }
}
