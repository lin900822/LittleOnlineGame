#nullable enable
using System;
using System.Threading.Tasks;

namespace Common
{
    public static class TaskExtensions
    {
        /// <summary>
        /// 一發即忘的安全同步呼叫非同步方法
        /// </summary>
        public static async void Await(this Task task, Action? onCompleted = null, Action<Exception>? onError = null)
        {
            try
            {
                await task;
                onCompleted?.Invoke();
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
        }

        /// <summary>
        /// 一發即忘的安全同步呼叫非同步方法
        /// </summary>
        public static async void Await<T>(this Task<T> task, Action<T>? onCompleted = null,
            Action<Exception>?                         onError = null)
        {
            try
            {
                T response = await task;
                onCompleted?.Invoke(response);
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
        }
    }
}