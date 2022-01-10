/*
// MIT License
//
// Copyright (c) 2020 Erik Eriksson
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ExperimentStructures
{
    /// <summary>
    /// Generic Serializable Dictionary for Unity 2020.1.
    /// Simply declare your key/value types and you're good to go - zero boilerplate.
    /// </summary>
    [Serializable]
    public class GenericDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        // Internal
        [SerializeField] private List<KeyValuePair> list = new List<KeyValuePair>();
        [SerializeField] private Dictionary<TKey, int> indexByKey = new Dictionary<TKey, int>();
        [SerializeField] [HideInInspector] private Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool keyCollision;
#pragma warning restore 0414

        // Serializable KeyValuePair struct
        [Serializable]
        private struct KeyValuePair
        {
            public TKey Key;
            public TValue Value;

            public KeyValuePair(TKey Key, TValue Value)
            {
                this.Key = Key;
                this.Value = Value;
            }
        }

        // Since lists can be serialized natively by unity no custom implementation is needed
        public void OnBeforeSerialize()
        {
        }

        // Fill dictionary with list pairs and flag key-collisions.
        public void OnAfterDeserialize()
        {
            dict.Clear();
            indexByKey.Clear();
            keyCollision = false;

            for (var i = 0; i < list.Count; i++)
            {
                var key = list[i].Key;
                if (key != null && !ContainsKey(key))
                {
                    dict.Add(key, list[i].Value);
                    indexByKey.Add(key, i);
                }
                else
                {
                    keyCollision = true;
                }
            }
        }

        // IDictionary
        public TValue this[TKey key]
        {
            get => dict[key];
            set
            {
                dict[key] = value;

                if (indexByKey.ContainsKey(key))
                {
                    var index = indexByKey[key];
                    list[index] = new KeyValuePair(key, value);
                }
                else
                {
                    list.Add(new KeyValuePair(key, value));
                    indexByKey.Add(key, list.Count - 1);
                }
            }
        }

        public ICollection<TKey> Keys => dict.Keys;
        public ICollection<TValue> Values => dict.Values;

        public void Add(TKey key, TValue value)
        {
            dict.Add(key, value);
            list.Add(new KeyValuePair(key, value));
            indexByKey.Add(key, list.Count - 1);
        }

        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (dict.Remove(key))
            {
                var index = indexByKey[key];
                list.RemoveAt(index);
                UpdateIndexes(index);
                indexByKey.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateIndexes(int removedIndex)
        {
            for (var i = removedIndex; i < list.Count; i++)
            {
                var key = list[i].Key;
                indexByKey[key]--;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dict.TryGetValue(key, out value);
        }

        // ICollection
        public int Count => dict.Count;
        public bool IsReadOnly { get; set; }

        public void Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public void Clear()
        {
            dict.Clear();
            list.Clear();
            indexByKey.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            TValue value;
            if (dict.TryGetValue(pair.Key, out value))
                return EqualityComparer<TValue>.Default.Equals(value, pair.Value);
            else
                return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (array.Length - arrayIndex < dict.Count)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            foreach (var pair in dict)
            {
                array[arrayIndex] = pair;
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> pair)
        {
            TValue value;
            if (dict.TryGetValue(pair.Key, out value))
            {
                var valueMatch = EqualityComparer<TValue>.Default.Equals(value, pair.Value);
                if (valueMatch) return Remove(pair.Key);
            }

            return false;
        }

        // IEnumerable
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }
}