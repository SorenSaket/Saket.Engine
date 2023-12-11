using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saket.Navigation
{
    public interface IDataProvider<T>
    {
        /// <summary>
        /// Invoked when the state changes. Change is not guaranteed.
        /// </summary>
        public Action<T> OnDataChanged { get; set; }
        public T GetData();
        public void RegenerateData();
        //public bool HasData();

    }
    public interface IDataListProvider<T>
    { /// <summary>
      /// Called item is modified
      /// </summary>
        public Action<T> OnItemChanged { get; set; }

        /// <summary>
        /// Called when the list is modified
        /// </summary>
        public Action<List<T>> OnDataChanged { get; set; }
        public List<T> GetData();
        public void RegenerateData();
        //public bool HasData();
    }

}