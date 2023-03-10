using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	//����Ŀ���
	public Transform targetPos;
	//�������Ŀ���
	public Transform lookAtPos;
	//��ʱ��������ǰ�ٶ�
	Vector3 currentV;
	//�����ٶ�
	public float speed = 20;
	void Start()
	{

	}
	void Update()
	{
		//�������[��ǰλ��Ŀ��λ�ã������ٶȣ���ǰ�ٶȣ������ٶ�]
		Vector3 tempPos = Vector3.SmoothDamp(transform.position, targetPos.position, ref currentV, speed);

		//���õ���ֵ��ֵ�������λ��
		transform.position = tempPos;

		//LookAk ����Ŀ���
		transform.LookAt(lookAtPos);
	}
}
