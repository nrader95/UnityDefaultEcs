using System;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable IDE0011 // Add braces to "if" statement

namespace DefaultEcs.Threading
{
    /// <summary>
    /// Represents an object used to run an <see cref="IParallelRunnable"/> by using multiple <see cref="Thread"/>.
    /// </summary>
    public sealed class DefaultParallelRunner : IParallelRunner
    {
        #region Fields

        internal static readonly DefaultParallelRunner Default = new(1);

        private readonly object _syncObject = new();
        private readonly ThreadWrapper[] _threads;
        private readonly bool[] _threadsWorkState;

        private volatile bool _isAlive;
        private volatile int _pendingTasks;
        private volatile IParallelRunnable _currentRunnable;
        private int _maxThreadIndex;

        #endregion

        private sealed class ThreadWrapper : IDisposable
        {
            public readonly Thread Thread;
            public readonly ManualResetEvent WaitEvent;
            public void Dispose() => WaitEvent.Dispose();
            public ThreadWrapper(Thread thread)
            {
                WaitEvent = new ManualResetEvent(initialState: false);
                Thread = thread;
            }
        }

        #region Initialisation

        /// <summary>
        /// Initialises a new instance of the <see cref="DefaultParallelRunner"/> class.
        /// </summary>
        /// <param name="degreeOfParallelism">The number of concurrent <see cref="Thread"/> used to update an <see cref="IParallelRunnable"/> in parallel.</param>
        /// <param name="threadNamePrefix"> Name prefix for the threads in case you have more than one runner and need to mark them. </param>
        /// <exception cref="ArgumentException"><paramref name="degreeOfParallelism"/> cannot be inferior to one.</exception>
        public DefaultParallelRunner(int degreeOfParallelism, string threadNamePrefix = null)
        {
            if (0 >= degreeOfParallelism)
                throw new ArgumentException($"Argument {nameof(degreeOfParallelism)} cannot be inferior to one!");

            _isAlive = true;
            threadNamePrefix ??= "";
            _threads = new ThreadWrapper[degreeOfParallelism - 1];
            _threadsWorkState = new bool[_threads.Length];
            for (int threadIndex = 0; _threads.Length > threadIndex; threadIndex++)
            {
                Thread newThread = new(new ParameterizedThreadStart(ThreadExecutionLoop))
                {
                    Name = threadNamePrefix + $"{nameof(DefaultParallelRunner)} worker {threadIndex + 1}",
                    IsBackground = true
                };
                _threads[threadIndex] = new ThreadWrapper(newThread);
                newThread.Start(threadIndex);
            }
        }

        #endregion

        #region Methods

        private void ThreadExecutionLoop(object initObject)
        {
            int workerIndex = (int)initObject;
            ManualResetEvent waitEvent = _threads[workerIndex].WaitEvent;
            while (_isAlive)
            {
                waitEvent.WaitOne();
                if (!_isAlive) return;
                if (!_threadsWorkState[workerIndex]) continue;

                try
                {
                    _currentRunnable?.Run(workerIndex, _maxThreadIndex);
                }
                finally
                {
                    lock (_syncObject)
                    {
                        _pendingTasks--;
                        _threadsWorkState[workerIndex] = false;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetThreadEvents(int maxThreadIndex)
        {
            for (int i = 0; maxThreadIndex > i; i++)
            {
                _threads[i].WaitEvent.Set();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetThreadEvents(int maxThreadIndex)
        {
            for (int i = 0; maxThreadIndex > i; i++)
            {
                _threads[i].WaitEvent.Reset();
            }
        }

        private bool IsAllThreadsStopped()
        {
            bool result = true;
            foreach (ThreadWrapper threadWrapper in _threads)
            {
                if (threadWrapper.Thread.ThreadState == ThreadState.Running)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        #endregion

        #region IRunner

        /// <summary>
        /// Gets the degree of parallelism used to run an <see cref="IParallelRunnable"/>.
        /// </summary>
        public int DegreeOfParallelism => _threads.Length + 1;

        /// <summary>
        /// Runs the provided <see cref="IParallelRunnable"/>.
        /// </summary>
        /// <param name="runnable">The <see cref="IParallelRunnable"/> to run.</param>
        /// <param name="maxThreadCount">Maximum count of threads to use, main thread included. Zero or less is no restriction</param>
        /// <exception cref="InvalidOperationException"> Runner was already disposed </exception>
        public void Run(IParallelRunnable runnable, int maxThreadCount)
        {
            if (!_isAlive)
                throw new InvalidOperationException("Runner was already disposed!");

            runnable.ThrowIfNull();
            _currentRunnable = runnable;

            int threadsToUse = (0 >= maxThreadCount || maxThreadCount > DegreeOfParallelism) ? DegreeOfParallelism : maxThreadCount;
            if (threadsToUse > 1)
            {
                _maxThreadIndex = threadsToUse - 1;
                for (int workerIndex = 0; _maxThreadIndex > workerIndex; workerIndex++)
                {
                    _threadsWorkState[workerIndex] = true;
                }
                _pendingTasks = _maxThreadIndex;

                SetThreadEvents(_maxThreadIndex);
                try
                {
                    _currentRunnable.Run(index: _maxThreadIndex, maxIndex: _maxThreadIndex);
                }
                finally
                {
                    SpinWait.SpinUntil(() => _pendingTasks == 0);
                    ResetThreadEvents(_maxThreadIndex);
                    SpinWait.SpinUntil(IsAllThreadsStopped);
                }
            }
            else
            {
                _currentRunnable.Run(index: 0, maxIndex: 0);
            }
            _currentRunnable = null;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases all the resources used by the current <see cref="DefaultParallelRunner"/> instance.
        /// </summary>
        public void Dispose()
        {
            if (!_isAlive)
                return;

            _isAlive = false;
            _currentRunnable = null;
            SetThreadEvents(_threads.Length);
            Thread.Sleep(millisecondsTimeout: 1);

            for (int i = 0; _threads.Length > i; i++)
            {
                _threads[i].Dispose();
            }
        }

        #endregion
    }
}
