using System;
using System.Collections;
using System.Collections.Generic;

namespace Saket.Engine.Collections
{
    public class RingBuffer<T> : IEnumerable<T>
    {
        private readonly T[] _elements;
        private int _start;
        private int _end;
        private readonly int _capacity;

		public int Capacity => _capacity;


		public T this[int i]
		{
			get => _elements[(_start + i) % _capacity];
			set => _elements[(_start + i) % _capacity] = value;
		}

        public RingBuffer(int capacity)
        {
            _elements = new T[capacity];
            _capacity = capacity;
        }

        public T First => _elements[_start];

        public void RemoveFromStart(int count)
        {
			// null all values
			for (int i = 0; i < count; i++)
			{
				_elements[(_start + i) % _capacity] = default(T)!;
			}
			_start = (_start + count) % _capacity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int counter = _start;
            while (counter != _end)
            {
                yield return _elements[counter];
                counter = (counter + 1) % _capacity;
            }           
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}