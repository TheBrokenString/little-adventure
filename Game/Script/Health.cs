using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth=100;//最大血量

    public int CurrentHealth;//当前血量

    public float CurrentHealthPercentage
    {
        get
        {
            return (float)CurrentHealth / (float)MaxHealth; 
        }
    }
    private Character _cc;       

    private void Awake()
    {
        CurrentHealth = MaxHealth; 
        _cc = GetComponent<Character>();

    }
    public void ApplayDamage(int damage)
    {
        CurrentHealth -= damage;
        Debug.Log(gameObject.name + "受到了" + damage + "点伤害,"+ "当前生命值为" + CurrentHealth);
        CheckHealth();
    }

    private void CheckHealth()//确认玩家血量是否低于0
    {
        if (CurrentHealth <= 0)
        {
            _cc.SwitchStateTo(Character.CharacterState.Dead);
        }
    }
    public void AddHealth(int health)
    {
        //if (CurrentHealth >= 100)
        //    CurrentHealth = 100;

        //else
        //    CurrentHealth += health;
        CurrentHealth = Mathf.Clamp(CurrentHealth + health, 0, MaxHealth); // 这个函数将血量钳制在最小值和最大值之间，0——maxhealth之间。
    }
}
