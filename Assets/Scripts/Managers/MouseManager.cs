using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;//public使其在Inspector中可见
    RaycastHit hitInfo;//射线信息
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        // DontDestroyOnLoad(gameObject); // 保持单例在场景切换时不被销毁
    }

    void Update()//每次刷新
    {
        SetCursorTexture();//鼠标移动到时：换鼠标贴图
        MouseControl();//鼠标点击时：触发事件
    }

    void SetCursorTexture()
    {
        // Debug.Log("enter into SetCursorTexture");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        // Debug.Log("enter into MouseControl");
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null) //左键点击
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point); //触发事件
            if (hitInfo.collider.gameObject.CompareTag("Portal")) //与Ground逻辑相同，要移动过去
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Attackable")) //与攻击敌人的逻辑相同
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);

        }
    }
}
