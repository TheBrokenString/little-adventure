using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Spwaner : MonoBehaviour
{
    private List<SpwanPoint> spwanPointList;
    private List<Character> spwanedCharacters;//��¼�Ѿ����ɵĵ���

    private bool hasSpwaned;
    public Collider _collider;
    public UnityEvent OnAllSpwanedCharacterEliminated;//�¼�
    private void Awake()
    {
        var spwanPointArray = transform.parent.GetComponentsInChildren<SpwanPoint>();
        spwanPointList = new List<SpwanPoint>(spwanPointArray);
        spwanedCharacters = new List<Character>();

    }
    private void Update()
    {
        if (!hasSpwaned || spwanedCharacters.Count==0)
        {
            return;
        }

        bool allSpwanedAreDead = true;
        foreach (Character c in spwanedCharacters)//�жϱ��м�¼�ĵ����Ƿ�ȫ����������
        {
            if(c.CurrentState!=Character.CharacterState.Dead)
            {
                allSpwanedAreDead = false;
                break;
            }
        }

        if(allSpwanedAreDead)
        {
            if(OnAllSpwanedCharacterEliminated!=null)
            {
                
                OnAllSpwanedCharacterEliminated.Invoke(); //������е��˺����
            }
            spwanedCharacters.Clear();
        }
    }

    public void spwanCharacters()
    {
        if (hasSpwaned)
            return;
        hasSpwaned = true;

        foreach(SpwanPoint  point in spwanPointList)
        {
            if(point.EnemyToSpwan != null)
            {
                GameObject spwanedGameObject = Instantiate(point.EnemyToSpwan, point.transform.position, point.transform.rotation);
                spwanedCharacters.Add(spwanedGameObject.GetComponent<Character>());//�����ɵĵ��˼�¼�����棬�������ŵ�����
                
            }
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        {
            spwanCharacters();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _collider.bounds.size);
    }




}
