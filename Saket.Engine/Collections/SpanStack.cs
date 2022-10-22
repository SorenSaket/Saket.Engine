using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Collections
{
    public ref struct SpanStack<T> where T : unmanaged
    {
        Span<T> values;
        int header = 0;

        public SpanStack(Span<T> span)
        {
            values = span;
        }

        public void Push(T value)
        {
            if(header < values.Length)
            {
                values[header] = value;
                header++;
            }
            else
            {
                throw new IndexOutOfRangeException("no more room");
            }
        }

        public T Pop()
        {
            if(header > 0)
            {
                int h = header;
                header--;
                return values[h];
            }
            else
            {
                throw new InvalidOperationException("No more objects in stack");
            }
        }

        public bool Contains(T value)
        {
            for (int i = 0; i < header; i++)
            {
                if (values[i].Equals(value))
                    return true;
            }
            return false;
        }
    }
}
