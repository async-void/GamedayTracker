using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using ChalkDotNET;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace GamedayTracker.Services
{
    public class LoggerService : ILogger, IDisposable
    {
        private bool _disposedValue;
        private SafeHandle? _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _safeHandle?.Dispose();
                _safeHandle = null;
            }

            _disposedValue = true;
        }

        public void Log(LogTarget target, LogType type, DateTimeOffset timestamp, string message)
        {
            switch (target)
            {
                case LogTarget.Console:
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"[{timestamp}] [Gameday Tracker] ");
                    switch (type)
                    {
                        case LogType.Information:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write($"[INFO] ");
                            Console.ResetColor();
                            Console.Write(message);
                            Console.WriteLine();
                            break;
                        case LogType.Warning:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write($"[WARNING] ");
                            Console.ResetColor();
                            Console.Write(message);
                            Console.WriteLine();
                            break;
                        case LogType.Error:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write($"[ERROR] ");
                            Console.ResetColor();
                            Console.Write(message);
                            Console.WriteLine();
                            break;
                        case LogType.Debug: 
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.Write($"[DEBUG] ");
                            Console.ResetColor();
                            Console.Write(message);
                            Console.WriteLine();
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"[LOG] ");
                            Console.ResetColor();
                            Console.Write(message);
                            Console.WriteLine();
                            break;
                        }
                        break;
                }
                case LogTarget.Debug:
                    Debug.WriteLine($"[{timestamp}] [Gameday Tracker] [{type}] {message}");
                    break;
                case LogTarget.File:
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TextFiles", "Logs", $"{type}", $"{DateTime.UtcNow.ToLongDateString()}.txt");
                    if (!File.Exists(path))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                        File.Create(path).Dispose();
                        File.AppendAllText(path, $"[{timestamp}] [Gameday Tracker] [{type}] {message}{Environment.NewLine}");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
