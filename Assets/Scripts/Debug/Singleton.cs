using UnityEngine;

/// <summary>
/// Ensures only one instance of type T is in the scene at all times.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            // If instance does exist, return it otherwise try to find it.
            if (m_Instance != null)
                return m_Instance;
            m_Instance = FindObjectOfType<T>();

            // If instance does exist, return it, otherwise create an instance and return its.
            if (m_Instance != null)
                return m_Instance;
            var obj = new GameObject
            {
                name = typeof(T).Name
            };
            m_Instance = obj.AddComponent<T>();
            return m_Instance;
        }
    }

    /// <summary>
    /// If Instance is null then set Instance to this, otherwise destroy this.
    /// </summary>
    public virtual void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}