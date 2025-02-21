using GamedayTracker.Interfaces;
using System.Timers;
using Timer = System.Timers.Timer;

namespace GamedayTracker.Services
{
    public class TimerService : ITimerService

    {
        public Timer? Timer { get; set; }
        public DateTime StartTime { get; set; }

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
            Timer!.Enabled = true;
            Timer.Start();
        }

        public void Stop()
        {
              Timer!.Stop();
              Timer.Enabled = false;
        }
    }
}
