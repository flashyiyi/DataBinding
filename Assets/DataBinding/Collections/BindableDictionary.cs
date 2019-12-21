using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class BindableDictionary<TKey, TValue> : BindAble, IEnumerable
    {
        public Dictionary<TKey, TValue> dictionary;

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => dictionary.Keys;

        public ICollection<TValue> Values => dictionary.Values;

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set 
            {
                TValue oldValue;
                dictionary.TryGetValue(key,out oldValue);
                if (oldValue is BindAble)
                   (oldValue as BindAble).onPropertyChanged -= OnChildPropertyChange;
                dictionary[key] = value;
                if (value is BindAble)
                    (value as BindAble).onPropertyChanged += OnChildPropertyChange;
                TriggerPropertyChange(this);
            }
        }

        public BindableDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
        }
        public BindableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);
            foreach (var item in dictionary)
            {
                if (item.Value is BindAble)
                    (item.Value as BindAble).onPropertyChanged += OnChildPropertyChange;
            }
        }
        public BindableDictionary(Dictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
            foreach (var item in dictionary)
            {
                if (item.Value is BindAble)
                    (item.Value as BindAble).onPropertyChanged += OnChildPropertyChange;
            }
        }
        public BindableDictionary(int capacity)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public void Clear()
        {
            dictionary.Clear();
            TriggerPropertyChange(this);
        }

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
            if (value is BindAble)
                (value as BindAble).onPropertyChanged += OnChildPropertyChange;
            TriggerPropertyChange(this);
        }

        public bool Remove(TKey key)
        {
            TValue oldValue;
            dictionary.TryGetValue(key, out oldValue);
            var result = dictionary.Remove(key);
            if (oldValue is BindAble)
                (oldValue as BindAble).onPropertyChanged -= OnChildPropertyChange;
            TriggerPropertyChange(this);
            return result;
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        void OnChildPropertyChange(PropertyChangedEvent e)
        {
            TriggerPropertyChange(e.target, e.propertyName);
        }
    }
}
