using GamedayTracker.Interfaces;
using System.Timers;
using Timer = System.Timers.Timer;

namespace GamedayTracker.Services
{
    public class TimerService : ITimerService

    {
        public Timer? Timer { get; set; }
        private static DateTime StartTime { get; set; }

        public void CreateNew()
        {
            StartTime = DateTime.UtcNow;
            Timer = new Timer(1000);
            Timer!.Elapsed += OnTimerTick;
            Timer.AutoReset = true;
        }

        private void OnTimerTick(object? sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            if (Timer != null) return;
            CreateNew();
            Timer!.Enabled = true;
            Timer.Start();
        }

        public void Stop()
        {
            if (Timer == null) return;
            Timer!.Stop();
            Timer.Enabled = false;

        }

        public TimeSpan CalculateRunningTime()
        {
            return DateTime.UtcNow - StartTime;
        }
    }
}
