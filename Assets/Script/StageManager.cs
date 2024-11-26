using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject clearUI;
    EnemyGenerator enemyGenerator;




    void Start()
    {
        enemyGenerator = FindObjectOfType<EnemyGenerator>();
        clearUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyGenerator.deadEnemyNum >= enemyGenerator.enemyNumMax)
        {
            clearUI.SetActive(true);//クリアテキストを表示する
        }
    }
}
