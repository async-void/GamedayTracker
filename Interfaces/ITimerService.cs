using Timer = System.Timers.Timer;
namespace GamedayTracker.Interfaces
{
    public interface ITimerService
    {
        public Timer? Timer { get; set; } 
        DateTime StartTime { get; set; }

        void CreateNew();
        void Start();
        void Stop();
    }
}
