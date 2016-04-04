using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FiniteThreadPool
{
    class Program
    {
        const int MAX_PARALLEL_UPLOADS = 2;
        private static int _count = 0;
        private static int _jobCount;
        private static SemaphoreSlim _semaphore;
        private static DateTime _start = DateTime.Now;
        static void Main(string[] args)
        {
            _semaphore = new SemaphoreSlim(MAX_PARALLEL_UPLOADS);
            for (int _jobCount = 0; _jobCount < 1; _jobCount++)
            {
                Console.WriteLine("Datetime_Main: {0}, _jobCount: {1}, semaphore: {2}", (DateTime.Now - _start).TotalMilliseconds, _jobCount, _semaphore.CurrentCount);
                _semaphore.WaitAsync().Wait();
                DoJob(_jobCount);
            }

            Console.ReadKey();
        }

        static async Task DoJob(int jobCount)
        {
            Console.WriteLine("Datetime_DoJob: {0}, jobCount: {1}, _semaphore: {2}", (DateTime.Now - _start).TotalMilliseconds, jobCount, _semaphore.CurrentCount);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(30000);
            await Task.Factory.StartNew(() => Dummy(jobCount, cts));
            _semaphore.Release();
            Console.WriteLine("Datetime_Release: {0}, jobCount: {1}, semaphore: {2} after Release", (DateTime.Now - _start).TotalMilliseconds, jobCount, _semaphore.CurrentCount);
        }

        static async Task Dummy(int jobCount, CancellationTokenSource cts)
        {
            await Task.Delay(5000);
            if (cts.IsCancellationRequested)
            {
                Console.WriteLine("dummy");
            }
        }
    }
}
