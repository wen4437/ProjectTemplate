using System;
using log4net.Appender;
using log4net.Core;
using System.IO;
using log4net.Util;
using System.Runtime.InteropServices;
using System.Reflection;

namespace System.My.CommonUtil
{
    public class ErrorMessageColoredConsoleAppender : ColoredConsoleAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            StreamWriter m_consoleOutputWriter = (StreamWriter)this.GetType().BaseType.GetField("m_consoleOutputWriter", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
            bool m_writeToErrorStream = (bool)this.GetType().BaseType.GetField("m_writeToErrorStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
            LevelMapping m_levelMapping = (LevelMapping)this.GetType().BaseType.GetField("m_levelMapping", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);

            if (m_consoleOutputWriter != null)
            {
                IntPtr consoleHandle = IntPtr.Zero;
                if (m_writeToErrorStream)
                {
                    consoleHandle = GetStdHandle(STD_ERROR_HANDLE);
                }
                else
                {
                    consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
                }

                // Default to white on black
                ushort colorInfo = (ushort)Colors.White;

                LevelColors levelColors = m_levelMapping.Lookup(loggingEvent.Level) as LevelColors;
                if (levelColors != null)
                {
                    colorInfo = (ushort)levelColors.GetType().GetProperty("CombinedColor", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(levelColors, null);
                }

                string strLoggingMessage = string.Empty;
                if(loggingEvent.Level == Level.Error && loggingEvent.ExceptionObject != null)
                {
                    strLoggingMessage = string.Format(loggingEvent.RenderedMessage + "\r\n", loggingEvent.ExceptionObject.Message);
                }
                else
                {
                    strLoggingMessage = this.RenderLoggingEvent(loggingEvent);
                }

                // get the current console color - to restore later
                CONSOLE_SCREEN_BUFFER_INFO bufferInfo;
                GetConsoleScreenBufferInfo(consoleHandle, out bufferInfo);

                // set the console colors
                SetConsoleTextAttribute(consoleHandle, colorInfo);

                char[] messageCharArray = strLoggingMessage.ToCharArray();
                int arrayLength = messageCharArray.Length;
                bool appendNewline = false;

                // Trim off last newline, if it exists
                if (arrayLength > 1 && messageCharArray[arrayLength - 2] == '\r' && messageCharArray[arrayLength - 1] == '\n')
                {
                    arrayLength -= 2;
                    appendNewline = true;
                }

                // Write to the output stream
                m_consoleOutputWriter.Write(messageCharArray, 0, arrayLength);

                // Restore the console back to its previous color scheme
                SetConsoleTextAttribute(consoleHandle, bufferInfo.wAttributes);

                if (appendNewline)
                {
                    // Write the newline, after changing the color scheme
                    m_consoleOutputWriter.Write(new char[] { '\r', '\n' }, 0, 2);
                }
            }
        }

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetConsoleTextAttribute(
            IntPtr consoleHandle,
            ushort attributes);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetConsoleScreenBufferInfo(
            IntPtr consoleHandle,
            out CONSOLE_SCREEN_BUFFER_INFO bufferInfo);

        private const UInt32 STD_OUTPUT_HANDLE = unchecked((UInt32)(-11));
        private const UInt32 STD_ERROR_HANDLE = unchecked((UInt32)(-12));

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetStdHandle(
            UInt32 type);

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public UInt16 x;
            public UInt16 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            public UInt16 Left;
            public UInt16 Top;
            public UInt16 Right;
            public UInt16 Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public ushort wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }
    }
}
