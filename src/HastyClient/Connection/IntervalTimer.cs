using System;
using System.Timers;

namespace Hasty.Client
{
	public class IntervalTimer
	{
		Timer timer;
		public delegate void Action();

		public Action OnElapsed;

		public IntervalTimer(uint waitTime)
		{
			timer = new Timer()
			{
				Interval = waitTime
			};
			timer.Elapsed += Timer_Elapsed;
		}

		public void Start()
		{
			timer.Start();
		}

		public void Stop()
		{
			timer.Elapsed -= Timer_Elapsed;
			timer.Stop();
			timer.Dispose();
			timer = null;
		}

		void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			OnElapsed();
		}
	}
}
