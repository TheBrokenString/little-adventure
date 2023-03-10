using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    public GameObject gameObject;
    public GameObject fCam; //第一视角摄像机
    public GameObject tCam; //第三人称摄像机
                             //public Button[] buttons;
    bool flag = false; //fCam启用为true
    void Start()
    {
        Invoke("cam", 2f);
    }
    private void Awake()
    {
        //获取相机
        fCam = GameObject.Find("VirtualCamera1"); //获取第一视角相机
        tCam = GameObject.Find("CM vcam1"); //获取第三视角相机
        tCam.SetActive(false); //禁用第三视角相机，第一视角作为主相机
    }


    private void Update()
    {
        //空格键F1作为切换触发
        //每按下一次发生切换

        if (Input.GetKeyDown(KeyCode.F1))
        {
            cam();
        }


    }
    public void cam()
    {
        flag = !flag; //状态反转
        tCam.SetActive(flag);
        fCam.SetActive(!flag); //两个相机状态互斥
    }

}