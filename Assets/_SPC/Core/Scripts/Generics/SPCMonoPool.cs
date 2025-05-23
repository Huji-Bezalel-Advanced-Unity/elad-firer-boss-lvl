using System.Collections.Generic;
using _SPC.Core.Scripts.Interfaces;
using UnityEngine;

namespace _SPC.Core.Scripts.Generics
{
    public class SPCMonoPool<T> : LBBaseMono.SPCBaseMono where T : LBBaseMono.SPCBaseMono, IPoolable
    {
        [SerializeField] private int initialSize = 10;
        [SerializeField] private T prefab;
        [SerializeField] private Transform parent;
        private Stack<T> _pool;

        private void Awake()
        {
            _pool = new Stack<T>();
            CreateObjects();
        }
        public T Get()
        {
            if (_pool.Count == 0)
            {
                CreateObjects();
            }
            T obj = _pool.Pop();
            obj.Reset();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Push(obj);
        }

        private void CreateObjects()
        {
            for (int i = 0; i < initialSize; i++)
            {
                var obj = Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);
                _pool.Push(obj);
            }
        }
    
    }
}
