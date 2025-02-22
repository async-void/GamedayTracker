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
                    Console.WriteLine($"{Chalk.Yellow($"[{timestamp.ToString(CultureInfo.CurrentCulture)}]")} {Chalk.Yellow("[Gameday Tracker]")} {Chalk.DarkBlue($"[{type}]")} {Chalk.DarkGray(message.ToString(CultureInfo.CurrentCulture))}");
                    break;
                }
                case LogTarget.Debug:
                case LogTarget.File:
                default:
                    break;
            }
        }
    }
}
