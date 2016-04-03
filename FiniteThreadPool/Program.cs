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
            for (int _jobCount = 0; _jobCount < 10; _jobCount++)
            {
                Console.WriteLine("Datetime: {0}, _jobCount: {1}", (DateTime.Now - _start).TotalMilliseconds, _jobCount);
                DoJob(_jobCount);
            }

            Console.ReadKey();
        }

        static async Task DoJob(int jobCount)
        {
            Console.WriteLine("Datetime: {0}, jobCount: {1}, _semaphore: {2}", (DateTime.Now - _start).TotalMilliseconds, jobCount, _semaphore.CurrentCount);
            await Task.Factory.StartNew(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    Dummy(jobCount);
                    Console.WriteLine("Datetime: {0}, jobCount: {1}, _semaphore: {2} after Dummy", (DateTime.Now - _start).TotalMilliseconds, jobCount, _semaphore.CurrentCount);
                }
                finally
                {
                    //_semaphore.Release();
                    Console.WriteLine("Datetime: {0}, jobCount: {1}, _semaphore: {2} after Release", (DateTime.Now - _start).TotalMilliseconds, jobCount, _semaphore.CurrentCount);
                }
            });
        }

        static async Task Dummy(int jobCount)
        {
            await Task.Delay(3000);
            _semaphore.Release();
            Console.WriteLine("Datetime: {0}, jobCount: {1}", (DateTime.Now - _start).TotalMilliseconds, jobCount);
        }
    }
}
