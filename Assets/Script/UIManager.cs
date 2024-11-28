using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{ 
    public Image hp;

    public GameObject settingUI;

    int maxHP = 5;
    int currHP = 5;


    float currTime = 0.0f;
    float timeMax = 2.0f;


    void Start()
    {
         UnableButtons();
    }
    void Update()
    {

        //テスト用仮変化
        /*currTime += Time.deltaTime;
        if(currTime > timeMax)
        {
            TakeDamage(1);
            currTime = 0.0f;
            if(currHP<0)
            {
                currHP = maxHP;
            }
        }*/
    }
    public void TakeDamage(int dmg)
    {
        currHP -= dmg;
        UpdateHPBar();

    }
    private void UpdateHPBar()//HP表示の更新
    {
        hp.fillAmount = (float)currHP / (float)maxHP;
    }

    public void SetHP(int curr,int max_ )
    {
        //GameManager呼ぶ関数、今のHPとHP上限を更新する
        currHP = curr;
        maxHP = max_;
        UpdateHPBar() ;
    }

    public void AbleButtons()
    {
        settingUI.SetActive(true);
    }
    public void UnableButtons()
    {
        settingUI.SetActive(false);
    }

    public void OnExitGameButtonDown()
    {
        Application.Quit();

    }
    public void OnRetryButtonDown()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
