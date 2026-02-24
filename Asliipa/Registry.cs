using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Whizz
{
    public class Registry<T>
    {
        protected List<T> Container = [];

        public ushort Register(T value)
        {
            Container.Add(value);
            return (ushort)(Container.Count - 1);
        }

        public T Get(ushort index)
        {
            return Container[index];
        }

        public T this[ushort index]
        {
            get => Get(index);
        }
    }
}
