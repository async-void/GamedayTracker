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

        public void Log(LogTarget target, LogType type, DateTime timestamp, string message)
        {
            switch (target)
            {
                case LogTarget.Console:
                { 
                    switch (type)
                    {
                        case LogType.Information:
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write($"[{timestamp}");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write($" INF");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("] ");
                                Console.ForegroundColor= ConsoleColor.Yellow;
                                Console.Write($"[Gameday Tracker] ");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(message);
                                Console.WriteLine();
                                Console.ResetColor();
                                break;
                        case LogType.Warning:
                                Console.ForegroundColor = ConsoleColor.DarkGray; ;
                                Console.Write($"[{timestamp}");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write($" WARN");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("] ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"[Gameday Tracker] ");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(message);
                                Console.WriteLine();
                                Console.ResetColor();
                                break;
                            case LogType.Error:
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write($"[{timestamp}");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write($" ERR");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("] ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"[Gameday Tracker] ");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(message);
                                Console.WriteLine();
                                Console.ResetColor();
                                break;
                            case LogType.Debug:
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write($"[{timestamp}");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write($" DBG");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("] ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"[Gameday Tracker] ");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(message);
                                Console.WriteLine();
                                Console.ResetColor();
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"[{timestamp}");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write($" LOG");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("] ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"[Gameday Tracker] ");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(message);
                                Console.WriteLine();
                                Console.ResetColor();
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
