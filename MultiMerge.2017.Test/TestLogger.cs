using System;
using MultiMerge;

namespace FindChangeByComment.Test
{
    class TestLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(@"{0} [{1}] - {2}", DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, message);
        }

        public void Debug(string message, params object[] args)
        {
            var log = string.Format(message, args);
            Console.WriteLine(@"{0} [{1}] - {2}", DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, log);
        }

        public void Error(string message)
        {
            Console.WriteLine(@"{0} [{1}] - {2}", DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, message);
        }

        public void Error(string message, Exception exception)
        {
            var now = DateTime.Now;
            Console.WriteLine(@"{0} [{1}] - {2}", now, System.Threading.Thread.CurrentThread.ManagedThreadId, message);
            Console.WriteLine(@"{0} [{1}] - {2}", now, System.Threading.Thread.CurrentThread.ManagedThreadId, exception);
        }
    }
}
