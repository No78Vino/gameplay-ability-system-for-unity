using UnityEngine;

namespace RootMotion
{

    /// <summary>
    /// Auto-instantiated singleton base class.
    /// </summary>
    public abstract class LazySingleton<T> : MonoBehaviour where T : LazySingleton<T>
    {

        private static T sInstance = null;

        public static bool hasInstance
        {
            get
            {
                return sInstance != null;
            }
        }

        public static T instance
        {
            get
            {
                if (sInstance == null)
                {
                    string name = typeof(T).ToString();
                    sInstance = new GameObject(name).AddComponent<T>();
                }
                return sInstance;
            }
        }

        protected virtual void Awake()
        {
            sInstance = (T)this;
        }
    }
}
