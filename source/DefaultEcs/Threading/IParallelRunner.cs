using System;

namespace DefaultEcs.Threading
{
    /// <summary>
    /// Exposes a method to run in parallel a <see cref="IParallelRunnable"/>.
    /// </summary>
    public interface IParallelRunner : IDisposable
    {
        /// <summary>
        /// Gets the degree of parallelism used to run an <see cref="IParallelRunnable"/>.
        /// </summary>
        int DegreeOfParallelism { get; }

        /// <summary>
        /// Runs the provided <see cref="IParallelRunnable"/>.
        /// </summary>
        /// <param name="runnable">The <see cref="IParallelRunnable"/> to run.</param>
        /// <param name="maxThreadCount">Maximum count of threads to use, main thread included. Zero or less is no restriction</param>
        void Run(IParallelRunnable runnable, int maxThreadCount);
    }
}
