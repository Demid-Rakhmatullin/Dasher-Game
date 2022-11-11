using Mirror;
using UnityEngine;

namespace Utils
{
    public abstract class NetworkSceneSingleton<T> : NetworkBehaviour where T : NetworkSceneSingleton<T>
    {
        private static readonly object lockObj = new();

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = FindObjectOfType(typeof(T), true) as T;
                        }

                        if (instance == null)
                            Debug.LogWarning($"Can't find object of type {typeof(T)} on scene. All scripts derived from NetworkSceneSingleton must be attached to single object on the scene.");
                    }
                }
                return instance;
            }
        }
    }
}
