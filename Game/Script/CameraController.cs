using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	//跟随目标点
	public Transform targetPos;
	//相机看向目标点
	public Transform lookAtPos;
	//临时变量，当前速度
	Vector3 currentV;
	//跟随速度
	public float speed = 20;
	void Start()
	{

	}
	void Update()
	{
		//相机跟随[当前位置目标位置，跟随速度，当前速度，跟随速度]
		Vector3 tempPos = Vector3.SmoothDamp(transform.position, targetPos.position, ref currentV, speed);

		//将得到的值赋值给相机的位置
		transform.position = tempPos;

		//LookAk 看向目标点
		transform.LookAt(lookAtPos);
	}
}
