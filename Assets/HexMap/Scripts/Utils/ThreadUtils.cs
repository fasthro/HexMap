using System;
using System.Threading;

namespace HexMap
{
    public class ThreadUtils
    {
        public static void Run(Action taskFunc, Action completeFunc = null, int sleep = 0)
        {
            WaitCallback wc = (state) =>
            {
                if (sleep > 0)
                {
                    Sleep(sleep);
                }
                taskFunc();
                completeFunc?.Invoke();
            };
            ThreadPool.QueueUserWorkItem(wc);
        }

        public static void Run<T>(Action<T> taskFunc, T arg, Action completeFunc = null, int sleep = 0)
        {
            WaitCallback wc = (state) =>
            {
                if (sleep > 0)
                {
                    Sleep(sleep);
                }
                taskFunc(arg);
                completeFunc?.Invoke();
            };
            ThreadPool.QueueUserWorkItem(wc);
        }

        public static void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}