using System;
using System.Collections.Generic;
using System.Threading;
#if !NET20
using System.Linq;

#endif

namespace Cave.Progress
{
    /// <summary>Provides progress management using callback events on progress change and completion.</summary>
    public abstract class ProgressManagerBase : IProgressManager
    {
#if NET20
        readonly Dictionary<IProgress, IProgress> items = new Dictionary<IProgress, IProgress>();
#else
        readonly HashSet<IProgress> items = new HashSet<IProgress>();
#endif
        int nextIdentifier;

        class ProgressItem : IProgress
        {
            readonly object syncRoot = new object();
            readonly ProgressManagerBase manager;

            #region Constructors

            public ProgressItem(ProgressManagerBase manager, object source)
            {
                this.manager = manager;
                Source = source;
                Identifier = Interlocked.Increment(ref this.manager.nextIdentifier);
            }

            #endregion

            #region IProgress Members

            public void Complete()
            {
                lock (syncRoot)
                {
                    Value = 1;
                    Completed = true;
                    manager.OnUpdated(this);
                }
            }

            public object Source { get; private set; }
            public bool Completed { get; private set; }
            public int Identifier { get; }
            public string Text { get; private set; }

            public void Update(float value, string text = null)
            {
                lock (syncRoot)
                {
                    if (Completed)
                    {
                        throw new InvalidOperationException();
                    }

                    if (value < Value)
                    {
                        return;
                    }

                    if (value > 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value));
                    }

                    if (text != null)
                    {
                        Text = text;
                    }

                    Value = value;
                    manager.OnUpdated(this);
                }
            }

            public float Value { get; private set; }

            #endregion

            #region Overrides

            public override string ToString() => $"{Value:P} : {Text}";

            #endregion
        }

        void OnUpdated(IProgress progress)
        {
            Updated?.Invoke(progress, new ProgressEventArgs(progress));
            if (progress.Completed)
            {
                lock (items)
                {
                    items.Remove(progress);
                }
            }
        }

        /// <summary>Provides an event for each progress update / completion</summary>
        public event EventHandler<ProgressEventArgs> Updated;

        /// <summary>Creates a new progress object implementing the <see cref="IProgress" /> interface.</summary>
        /// <remarks>
        /// This function does not call the <see cref="Updated" /> event for the newly created <see cref="IProgress" /> instance. The
        /// <see cref="Updated" /> event will be fired upon the first <see cref="IProgress.Update(float, string)" /> call.
        /// </remarks>
        /// <returns>Retruns a new instance implementing the <see cref="IProgress" /> interface.</returns>
        public IProgress CreateProgress(object source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var result = new ProgressItem(this, source);
            lock (items)
            {
#if NET20
                items.Add(result, result);
#else
                items.Add(result);
#endif
            }

            return result;
        }

        /// <summary>Gets the current progress items.</summary>
        public IEnumerable<IProgress> Items
        {
            get
            {
                lock (items)
                {
#if NET20
                    return new List<IProgress>(items.Keys);
#else
                    return items.ToList();
#endif
                }
            }
        }
    }
}
