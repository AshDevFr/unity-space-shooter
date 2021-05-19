using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get { return _instance; }
    }

    public static bool IsInitialized
    {
        get { return _instance != null; }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
            _instance = (T) this;
        else
        {
            Debug.LogError("[Singleton] Trying to instanciate a second instance of the singleton class");
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}