using Timer = System.Timers.Timer;
namespace GamedayTracker.Interfaces
{
    public interface ITimerService
    {
        public Timer? Timer { get; set; }
        public TimeSpan CalculateRunningTime();
        void CreateNew();
        void Start();
        void Stop();
    }
}
