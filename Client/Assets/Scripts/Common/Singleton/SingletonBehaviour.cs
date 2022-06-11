using UnityEngine;

namespace Common
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        Debug.LogError($"<color=red>{typeof(T)}</color> is nothing.");
                        throw new System.Exception($"{typeof(T)} is not found gameObject.");
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (Instance != null && Instance.gameObject != this.gameObject)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this as T;
            }
        }
    }
}