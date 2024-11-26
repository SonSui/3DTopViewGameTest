using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject EnemyA1;
    float span = 2.0f;
    float delta = 0;

    //クリア判定変数

    public  int enemyNumMax = 3;


    int currEnemyNum = 0;
    public  int deadEnemyNum = 0;//テストため、EnemyA1のUpdate関数が変更してくる


    //public GameObject clearUI;
    
    void Start()
    {
       // clearUI.SetActive(false);//クリアテキストを見えないようにする
    }

    void Update()
    {
        this.delta += Time.deltaTime;

        if (this.delta > this.span)
        {
            this.delta = 0;
            float px;
            float pz;

            GameObject go = Instantiate(EnemyA1);
            do
            {
                px = UnityEngine.Random.Range(-12.0f, 12.0f);
            } while (px >= -3 && px <= 3);

            do
            {
                pz = UnityEngine.Random.Range(-12.0f, 12.0f);
            } while (pz >= -3 && pz <= 3);

            go.transform.position = new Vector3(px, 3, pz);
            Debug.Log(new Vector3(px, 3, pz));
        }

    }
}
