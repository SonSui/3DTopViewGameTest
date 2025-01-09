using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject clearUI;
    public GameObject defeatUI;
    EnemyGenerator enemyGenerator;
    UIManager uiManager;




    void Start()
    {
        enemyGenerator = FindObjectOfType<EnemyGenerator>();
        uiManager = FindObjectOfType<UIManager>();
        /*clearUI.SetActive(false);
        defeatUI.SetActive(false);*/
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPlayerDead())
        {
            //defeatUI.SetActive(true);
            uiManager.AbleButtons();
        }
        else if (enemyGenerator.deadEnemyNum >= enemyGenerator.enemyNumMax)
        {
            //clearUI.SetActive(true);//クリアテキストを表示する
            uiManager.AbleButtons();
        }
        
    }
}
