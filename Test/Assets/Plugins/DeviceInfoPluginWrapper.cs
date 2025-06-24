using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DeviceInfoPluginTools
{
    public static class DeviceInfoPlugin
    {
        [DllImport("DeviceInfoPlugin", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetPlatform([Out] byte[] buffer, int bufferSize);

        [DllImport("DeviceInfoPlugin", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCurrentDateTime([Out] byte[] buffer, int bufferSize);

        [DllImport("DeviceInfoPlugin", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCurrentLocale([Out] byte[] buffer, int bufferSize);

        public static string GetPlatformString()
        {
            var buffer = new byte[128];
            GetPlatform(buffer, buffer.Length);
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        public static string GetCurrentDateTimeString()
        {
            var buffer = new byte[128];
            GetCurrentDateTime(buffer, buffer.Length);
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        public static string GetCurrentLocaleString()
        {
            var buffer = new byte[128];
            GetCurrentLocale(buffer, buffer.Length);
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }
    }
}