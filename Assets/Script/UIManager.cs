using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{ 
    public Image hp;

    int maxHP = 5;
    int currHP = 5;


    float currTime = 0.0f;
    float timeMax = 2.0f;


    void Start()
    {
         
    }
    void Update()
    {

        //�e�X�g�p���ω�
        currTime += Time.deltaTime;
        if(currTime > timeMax)
        {
            TakeDamage(1);
            currTime = 0.0f;
            if(currHP<0)
            {
                currHP = maxHP;
            }
        }
    }
    public void TakeDamage(int dmg)
    {
        currHP -= dmg;
        UpdateHPBar();

    }
    private void UpdateHPBar()//HP�\���̍X�V
    {
        hp.fillAmount = (float)currHP / (float)maxHP;
    }

    public void SetHP(int curr,int max_ )
    {
        //GameManager�ĂԊ֐��A����HP��HP������X�V����
        currHP = curr;
        maxHP = max_;
        UpdateHPBar() ;
    }
}