using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject); // 保持单例在场景切换时不被销毁
        }
        else
            Destroy(gameObject); // 如果已经存在实例，则销毁新创建的实例
    }

    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this) { instance = null; }// 清除实例引用 
    }
}