﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BidirectionalDict
{
	/// <summary>
	/// Represents a bidirectional collection of value pairs.
	/// </summary>
	/// <typeparam name="TFirst">The type of the first values in the dictionary.</typeparam>
	/// <typeparam name="TSecond">The type of the second values in the dictionary.</typeparam>
	public class BiDictionary<TFirst, TSecond> : IBiDictionary<TFirst, TSecond>
												 where TFirst : notnull
												 where TSecond : notnull
	{
		/// <inheritdoc/>
		public bool IsSynced => Count == _secondToFirst.Count;

		/// <inheritdoc/>
		public int Count => _firstToSecond.Count;

		private readonly IDictionary<TFirst, TSecond> _firstToSecond;
		private readonly IDictionary<TSecond, TFirst> _secondToFirst;

		/// <summary>Initializes a new instance of the <see cref="BiDictionary{TFirst, TSecond}" /> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.</summary>
		public BiDictionary()
		{
			_firstToSecond = new Dictionary<TFirst, TSecond>();
			_secondToFirst = new Dictionary<TSecond, TFirst>();
		}

		/// <summary>Initializes a new instance of the <see cref="BiDictionary{TFirst, TSecond}" /> class that contains elements copied from the specified <see cref="IEnumerable{T}" /> and uses the default equality comparer for the key type.</summary>
		/// <param name="collection">The <see cref="IEnumerable{T}" /> whose elements are copied to the new <see cref="BiDictionary{TFirst, TSecond}" />.</param>
		public BiDictionary(IEnumerable<KeyValuePair<TFirst, TSecond>> collection)
		{

			_firstToSecond = new Dictionary<TFirst, TSecond>(collection.ToDictionary(x => x.Key, x => x.Value));
            _secondToFirst = new Dictionary<TSecond, TFirst>(collection.ToDictionary(k => k.Value, v => v.Key));
		}

		/// <summary>Initializes a new instance of the <see cref="BiDictionary{TFirst, TSecond}" /> class that is empty, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{T}" />.</summary>
		/// <param name="firstComparer">The <see cref="IEqualityComparer{TFirst}" /> implementation to use when comparing the first values, or <see langword="null" /> to use the default <see cref="IEqualityComparer{TFirst}" /> for the type of the key.</param>
		/// <param name="secondComparer">The <see cref="IEqualityComparer{TSecond}" /> implementation to use when comparing the first values, or <see langword="null" /> to use the default <see cref="IEqualityComparer{TSecond}" /> for the type of the key.</param>
		public BiDictionary(IEqualityComparer<TFirst> firstComparer, IEqualityComparer<TSecond> secondComparer)
		{
			_firstToSecond = new Dictionary<TFirst, TSecond>(firstComparer);
			_secondToFirst = new Dictionary<TSecond, TFirst>(secondComparer);
		}


        /// <inheritdoc/>
        public void Add(TFirst first, TSecond second)
        {

            if (!_firstToSecond.ContainsKey(first))
            {
                _firstToSecond[first] = second;
            }

            if (!_secondToFirst.ContainsKey(second))
            {
                _secondToFirst[second] = first;
            }
        }


        /// <inheritdoc/>
        public void Add((TFirst, TSecond) value)
        {

			if (!_firstToSecond.ContainsKey(value.Item1))
			{
				_firstToSecond[value.Item1] = value.Item2;
			}

			if (!_secondToFirst.ContainsKey(value.Item2))
			{
				_secondToFirst[value.Item2] = value.Item1;
			}
        }


        /// <inheritdoc/>
        public bool TryAdd(TFirst first, TSecond second)
		{

			if (_firstToSecond.ContainsKey(first) ||
				_secondToFirst.ContainsKey(second))
			{
				return false;
			}

			_firstToSecond.Add(first, second);
			_secondToFirst.Add(second, first);
            return true;
		}

		/// <inheritdoc/>
		public void AddOrUpdate(TFirst first, TSecond second)
		{

			if (!_firstToSecond.ContainsKey(first))
			{
				_firstToSecond[first] = second;
			}

			if (!_secondToFirst.ContainsKey(second))
			{
				_secondToFirst[second] = first;
			}
        }

        /// <inheritdoc/>
        public bool TryRemove(TFirst first)
		{

			var second = _firstToSecond[first];

			if (!_firstToSecond.Remove(first))
			{
				return false;
			}
            _secondToFirst.Remove(second);

			return true;
		}

		/// <inheritdoc/>
		public bool TryRemove(TSecond second)
		{
			var first = _secondToFirst[second];

			if (!_secondToFirst.Remove(second))
			{
				return false;
			}

			_firstToSecond.Remove(first);

			return true;
		}

		/// <inheritdoc/>
		public bool Contains(TFirst first)
			=> _firstToSecond.ContainsKey(first);

		/// <inheritdoc/>
		public bool Contains(TSecond second)
			=> _secondToFirst.ContainsKey(second);

		/// <inheritdoc/>
		public void Clear()
		{
			_secondToFirst.Clear();
			_firstToSecond.Clear();
		}

		/// <inheritdoc/>
		public TSecond this[TFirst first]
			=> _firstToSecond[first];

		/// <inheritdoc/>
		public TFirst this[TSecond second]
			=> _secondToFirst[second];

		/// <inheritdoc/>
		public bool TryGet(TFirst first, out TSecond second)
			=> _firstToSecond.TryGetValue(first, out second);

		/// <inheritdoc/>
		public bool TryGet(TSecond second, out TFirst first)
			=> _secondToFirst.TryGetValue(second, out first);

		/// <summary>
		/// Gets the <see cref="IEnumerator{T}"/> of the <see cref="BiDictionary{TFirst, TSecond}"/>.
		/// </summary>
		/// <returns>The <see cref="IEnumerator{T}"/> of the <see cref="BiDictionary{TFirst, TSecond}"/></returns>
		public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator()
			=> _firstToSecond.GetEnumerator();

		/// <summary>
		/// Gets the <see cref="IEnumerator"/> of the <see cref="BiDictionary{TFirst, TSecond}"/>.
		/// </summary>
		/// <returns>The <see cref="IEnumerator"/> of the <see cref="BiDictionary{TFirst, TSecond}"/></returns>
		IEnumerator IEnumerable.GetEnumerator()
			=> _firstToSecond.GetEnumerator();
	}
}
