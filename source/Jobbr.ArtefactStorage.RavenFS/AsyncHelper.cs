using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jobbr.ArtefactStorage.RavenFS
{
    /// <summary>
    /// Helper class to call async methods from non async methods
    /// </summary>
    internal static class AsyncHelper
    {
        private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncHelper.TaskFactory
                .StartNew<Task<TResult>>(func)
                .Unwrap<TResult>()
                .GetAwaiter()
                .GetResult();
        }
    }
}