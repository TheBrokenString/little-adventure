using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerinput : MonoBehaviour
{
    public float HorizontalInput;
    public float verticalInput;
    public bool MouseButtonDown;//攻击输入
    public bool SpaceKeyDown;

    public float timer = 0f;


    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5)//延迟两秒获取玩家输入
        {
            if (!MouseButtonDown && Time.timeScale != 0)
            {
                MouseButtonDown = Input.GetMouseButtonDown(0);//只要鼠标左键，输入数据
            }
            HorizontalInput = Input.GetAxisRaw("Horizontal");// 获取垂直输入
            verticalInput = Input.GetAxisRaw("Vertical");//获取水平输入

            if (!SpaceKeyDown && Time.timeScale != 0)
            {
                SpaceKeyDown = Input.GetKeyDown(KeyCode.Space);
            }
        }

           
    }

    private void OnDisable()
    {
        ClearCache();
    }
    public void ClearCache()
    {
        MouseButtonDown = false;
        SpaceKeyDown = false;
        HorizontalInput = 0;
        verticalInput = 0;
        //每次调用，确保数据置零，当角色死亡时不会接收任何数据。
    }
}