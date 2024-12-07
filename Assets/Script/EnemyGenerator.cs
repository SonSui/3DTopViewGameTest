using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject EnemyA1;
    public GameObject EnemyShooter;
    public float span = 3.0f;
    float delta = 0;

    //クリア判定変数

    public int enemyNumMax = 15;


    int currEnemyNum = 0;
    public int deadEnemyNum = 0;//テストため、EnemyA1のUpdate関数が変更してくる


    //public GameObject clearUI;

    void Start()
    {
        // clearUI.SetActive(false);//クリアテキストを見えないようにする
    }

    void Update()
    {
        this.delta += Time.deltaTime;


        if (this.delta > this.span && currEnemyNum < enemyNumMax)
        {
            this.delta = 0;
            float px;
            float pz;
            Vector3 pos = new Vector3(0f, 10f, 0f);
            ///////4Enemy1/////

            GameObject go = Instantiate(EnemyA1, pos, Quaternion.identity);
            do
            {
                px = UnityEngine.Random.Range(-12.0f, 12.0f);
            } while (px >= -3 && px <= 3);

            do
            {
                pz = UnityEngine.Random.Range(-5.0f, 5.0f);
            } while (pz >= -3 && pz <= 3);

            go.transform.position = new Vector3(px, 3, pz);
            Debug.Log(new Vector3(px, 3, pz));
            currEnemyNum++;
            ///////4EnemyShooter/////
            /*go = Instantiate(EnemyShooter, pos, Quaternion.identity);
            do
            {
                px = UnityEngine.Random.Range(-12.0f, 12.0f);
            } while (px >= -3 && px <= 3);

            do
            {
                pz = UnityEngine.Random.Range(-5.0f, 5.0f);
            } while (pz >= -3 && pz <= 3);

            go.transform.position = new Vector3(px, 3, pz);
            Debug.Log(new Vector3(px, 3, pz));
            currEnemyNum++;*/
        }

    }
}
