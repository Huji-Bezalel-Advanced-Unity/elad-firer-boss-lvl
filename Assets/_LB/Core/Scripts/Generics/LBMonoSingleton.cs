using _LB.Core.Scripts.AbstractsMono;
using UnityEngine;

namespace _LB.Core.Scripts.Generics
{
    /// <summary>
    /// A generic Singleton class for MonoBehaviours.
    /// Example usage: public class GameManager : MonoSingleton<GameManager>
    /// </summary>
    public class LBMonoSingleton<T> : LBBaseMono where T : LBBaseMono
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    var singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);            }

                return _instance;
            }
        }

        // Ensure no other instances can be created by having the constructor as protected
        protected LBMonoSingleton() { }
    }


}