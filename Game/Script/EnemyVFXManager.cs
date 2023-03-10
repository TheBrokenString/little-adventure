using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect FoodStep;
    public VisualEffect AttackVFX;
    public ParticleSystem BeingHitVFX;
    public VisualEffect BeigingHitSplashVFX;
    public void PlayAttackVFX()
    {
        AttackVFX.SendEvent("OnPlay");
    }
    public void BurstFootStep()
    {
        FoodStep.SendEvent("OnPlay");//�����Զ��������¼�
    }

    public void PlayBeingHitVFX(Vector3 attackerPos)
    {
        Vector3 forceForward = transform.position - attackerPos;
        forceForward.Normalize();
        forceForward.y = 0;
        BeingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        BeingHitVFX.Play();

        Vector3 splashPos = transform.position;
        splashPos.y += 2f;
        VisualEffect newSplashVFX = Instantiate(BeigingHitSplashVFX, splashPos, Quaternion.identity);
        newSplashVFX.SendEvent("OnPlay");
        Destroy(newSplashVFX.gameObject, 10f);//����splash��Ч����Ȼ���ڵ���һ��ʯ��

    }
}
