using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    public GameObject gameObject;
    public GameObject fCam; //��һ�ӽ������
    public GameObject tCam; //�����˳������
                             //public Button[] buttons;
    bool flag = false; //fCam����Ϊtrue
    void Start()
    {
        Invoke("cam", 2f);
    }
    private void Awake()
    {
        //��ȡ���
        fCam = GameObject.Find("VirtualCamera1"); //��ȡ��һ�ӽ����
        tCam = GameObject.Find("CM vcam1"); //��ȡ�����ӽ����
        tCam.SetActive(false); //���õ����ӽ��������һ�ӽ���Ϊ�����
    }


    private void Update()
    {
        //�ո��F1��Ϊ�л�����
        //ÿ����һ�η����л�

        if (Input.GetKeyDown(KeyCode.F1))
        {
            cam();
        }


    }
    public void cam()
    {
        flag = !flag; //״̬��ת
        tCam.SetActive(flag);
        fCam.SetActive(!flag); //�������״̬����
    }

}