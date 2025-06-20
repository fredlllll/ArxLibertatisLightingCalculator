using System;
using System.Threading;

namespace ArxLibertatisLightingCalculatorLib.Util
{
    public class ProgressPrinter
    {
        int current = 0;
        readonly int total;
        readonly string name;
        DateTime lastMessagePrintedAt = DateTime.MinValue;
        readonly TimeSpan interval = new TimeSpan(0, 0, 2); //print every 2 seconds
        readonly object lock_ = new object();

        public ProgressPrinter(int total, string name = "Progress")
        {
            this.total = total;
            this.name = name;
        }

        public void Progress()
        {
            Interlocked.Increment(ref current);

            if (current >= total || DateTime.Now - lastMessagePrintedAt > interval)
            {
                lock (lock_)
                {
                    lastMessagePrintedAt = DateTime.Now;
                    PrintProgress();
                }
            }
        }

        void PrintProgress()
        {
            Console.WriteLine($"{name}: {current * 100f / total:0.00}% ({current}/{total})");
        }
    }
}
