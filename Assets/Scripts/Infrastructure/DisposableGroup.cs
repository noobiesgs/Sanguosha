using System.Collections.Generic;
using System;

namespace Noobie.Sanguosha.Infrastructure
{
    public class DisposableGroup : IDisposable
    {
        private readonly List<IDisposable> m_Disposables = new();

        public void Dispose()
        {
            foreach (var disposable in m_Disposables)
            {
                disposable.Dispose();
            }

            m_Disposables.Clear();
        }

        public void Add(IDisposable disposable)
        {
            m_Disposables.Add(disposable);
        }
    }
}
