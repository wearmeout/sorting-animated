using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FractionalSorting.Visualization.Test.Common
{
    public static class AsyncPump
    {
        public static void Run(Func<Task> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var prevCtx = SynchronizationContext.Current;

            try
            {
                runTask(func);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        private static void runTask(Func<Task> func)
        {
            setNewContext();
            Task task = func() ?? throw new InvalidOperationException();
            var frame = new DispatcherFrame();
            task.ContinueWith((t) => frame.Continue = false, TaskScheduler.Default);
            Dispatcher.PushFrame(frame);
            task.GetAwaiter().GetResult();
        }

        private static void setNewContext()
        {
            var syncCtx = new DispatcherSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncCtx);
        }
    }
}
