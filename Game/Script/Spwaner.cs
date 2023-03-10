using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Spwaner : MonoBehaviour
{
    private List<SpwanPoint> spwanPointList;
    private List<Character> spwanedCharacters;//记录已经生成的敌人

    private bool hasSpwaned;
    public Collider _collider;
    public UnityEvent OnAllSpwanedCharacterEliminated;//事件
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
        foreach (Character c in spwanedCharacters)//判断表中记录的敌人是否全部都死亡。
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
                
                OnAllSpwanedCharacterEliminated.Invoke(); //清除所有敌人后调用
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
                spwanedCharacters.Add(spwanedGameObject.GetComponent<Character>());//将生成的敌人记录到里面，用作开门的条件
                
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
