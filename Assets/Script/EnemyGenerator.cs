using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject EnemyA1;
    float span = 2.0f;
    float delta = 0;

    //クリア判定変数
    int enemyNumMax = 6;
    int currEnemyNum = 0;
    public int deadEnemyNum = 0;//テストため、EnemyA1のUpdate関数が変更してくる


    public GameObject clearUI;
    
    void Start()
    {
        clearUI.SetActive(false);//クリアテキストを見えないようにする
    }

    void Update()
    {
        this.delta += Time.deltaTime;
        if (currEnemyNum < enemyNumMax)//シーンの敵を管理
        {
            if (this.delta > this.span)
            {
                this.delta = 0;
                GameObject go = Instantiate(EnemyA1);
                float px = Random.Range(-15f, 15f);
                float pz = Random.Range(-5f, 5f);
                go.transform.position = new Vector3(px, 0, pz);

                currEnemyNum++;//敵の記数
            }
        }
        if(deadEnemyNum>= enemyNumMax)
        {
            clearUI.SetActive(true);//クリアテキストを表示する
        }
        
    }
}
