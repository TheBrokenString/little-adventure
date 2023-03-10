using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameStartTitle : MonoBehaviour
{
    GameObject campFire;
    public Image title;
    [Range(0f,10f)]public float timer =0f;//计时器
    public float duration ; //预设时间
    [Range(0f, 10f)] public float timer2 = 0;
    [Range(0f, 10f)] public float timer3 = 0;
    private void Awake()
    {
        campFire = GameObject.Find("Title");
        title = campFire.GetComponent<Image>();
    }
    private void Update()
    {
        Invoke("TitleAnimation", 2f);

        //Debug.Log(Time.deltaTime);
    }


    private void TitleAnimation()
    {

        if (timer <=duration )
        {
            timer += Time.deltaTime;
            title.fillAmount = Mathf.Lerp(0, 1, timer / duration);
            Debug.Log(timer / duration);
        }
        timer3 += Time.deltaTime;
        if (timer3 > duration+2)
        {

            timer2 += Time.deltaTime;
            title.fillAmount = Mathf.Lerp(0, 1, -(timer2 / duration)+1);
                Debug.Log(-(timer2 / duration) + 1);
            if(title.fillAmount==0)
            {
                campFire.SetActive(false);
            }

        }
        

    }
}
