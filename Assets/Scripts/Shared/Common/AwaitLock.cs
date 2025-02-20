﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Common
{
/* ------------使用方式-----------------

private AwaitLock _awaitLock = new AwaitLock();

private async Task SomeMethod()
{
   using (await _awaitLock.Lock())
   {
        ...
        await XXX1();
        ...
        await XXX2();
        ...
        await XXX3();
   }
}

------------------------------------ */

    /// <summary>
    /// <para>用於鎖定具有「多個 await的程式碼片段」</para>
    /// <para>以確保同一時間只有一個非同步執行流程可以進入該程式碼片段。</para>
    /// </summary>
    public class AwaitLock : IDisposable
    {
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<IDisposable> Lock()
        {
            await _semaphore.WaitAsync();
            return this;
        }

        void IDisposable.Dispose()
        {
            _semaphore.Release();
        }
    }
}