using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class BindableList<T> : BindAble, ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        public List<T> list;

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => list[index];
            set 
            {
                if (index < list.Count)
                {
                    var oldValue = list[index];
                    if (oldValue is BindAble)
                        (oldValue as BindAble).onPropertyChanged -= OnChildPropertyChange;
                }
                list[index] = value;
                if (value is BindAble)
                    (value as BindAble).onPropertyChanged += OnChildPropertyChange;
                TriggerPropertyChange(this);
            }
        }

        public BindableList()
        {
            list = new List<T>();
        }
        public BindableList(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
            foreach (var item in collection)
            {
                if (item is BindAble)
                    (item as BindAble).onPropertyChanged += OnChildPropertyChange;
            }
        }
        public BindableList(List<T> list)
        {
            this.list = list;
            foreach (var item in list)
            {
                if (item is BindAble)
                    (item as BindAble).onPropertyChanged += OnChildPropertyChange;
            }
        }
        public BindableList(int capacity)
        {
            list = new List<T>(capacity);
        }

        public void Add(T item)
        {
            list.Add(item);
            if (item is BindAble)
                (item as BindAble).onPropertyChanged += OnChildPropertyChange;
            TriggerPropertyChange(this);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            if (item is BindAble)
                (item as BindAble).onPropertyChanged += OnChildPropertyChange;
            TriggerPropertyChange(this);
        }

        public bool Remove(T item)
        {
            var result = list.Remove(item);
            if (item is BindAble)
                (item as BindAble).onPropertyChanged -= OnChildPropertyChange;
            TriggerPropertyChange(this);
            return result;
        }

        public void RemoveAt(int index)
        {
            var item = list[index];
            list.RemoveAt(index);
            if (item is BindAble)
                (item as BindAble).onPropertyChanged -= OnChildPropertyChange;
            TriggerPropertyChange(this);
        }

        public void Clear()
        {
            list.Clear();
            TriggerPropertyChange(this);
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        void OnChildPropertyChange(PropertyChangedEvent e)
        {
            TriggerPropertyChange(e.target, e.propertyName);
        }
    }
}
