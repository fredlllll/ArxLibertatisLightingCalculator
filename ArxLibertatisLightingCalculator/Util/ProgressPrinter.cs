using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArxLibertatisLightingCalculator.Util
{
    public class ProgressPrinter
    {
        int current = 0;
        int total;
        string name;
        DateTime lastMessagePrintedAt = DateTime.MinValue;
        TimeSpan interval = new TimeSpan(0, 0, 2); //print every 2 seconds
        object lock_ = new object();

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
            Console.WriteLine($"{name}: {((this.current * 100f) / this.total).ToString("0.00")}% ({this.current}/{this.total})");
        }
    }
}
