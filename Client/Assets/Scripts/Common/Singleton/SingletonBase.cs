using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class SingletonBase<T> where T : class, new()
    {
        private static T instance = null;
        public static T Instance 
        {
            get
            {
                if (instance == null)
                {
                    Create();
                }
                return instance;
            }
        }

        protected SingletonBase()
        {
            Create();
        }

        ~SingletonBase()
        {
            Remove();
        }

        public static void Create()
        {
            if (instance == null)
            {
                instance = new T();
            }
        }

        public static void Remove()
        {
            if (instance != null)
            {
                instance = null;
            }
        }
    }
}