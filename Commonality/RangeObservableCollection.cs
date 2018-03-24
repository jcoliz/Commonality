using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Commonality
{
    /// <summary>
    /// Extends ObservableCollection to add an AddRange
    /// </summary>
    /// <remarks>
    /// This is needed to avoid the notification for every single element
    /// </remarks>
    /// <typeparam name="T">Type of object to be collected</typeparam>
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Add a range of items to the collectiom
        /// </summary>
        /// <param name="list">Which items</param>
        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            foreach (T item in list)
            {
                Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Insert a range of items into the collection
        /// </summary>
        /// <param name="index">Position where the list will be found, after the operation, e.g. '0' to insert at the front</param>
        /// <param name="list">Which items</param>

        public void InsertRange(int index, IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            int i = index;
            foreach (T item in list)
            {
                Insert(i++, item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Called by the base class when the collection changes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Whether we should supress notifications of the collection changing
        /// </summary>
        protected bool _suppressNotification = false;
    }
}
