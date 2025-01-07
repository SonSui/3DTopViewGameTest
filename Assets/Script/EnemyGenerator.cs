using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject EnemyA1;
    public GameObject Enemy_Teki01;
    public float span = 3.0f;
    float delta = 0;
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    

    //クリア判定変数
    public  int enemyNumMax = 15;

    int currEnemyNum = 0;
    public int deadEnemyNum = 0;//テストのため、EnemyA1のUpdate関数が変更してくる


    //public GameObject clearUI;

    void Start()
    {
       //clearUI.SetActive(false);//クリアテキストを見えないようにする
    }

    void Update()
    {
        this.delta += Time.deltaTime;
        if (this.delta > this.span && currEnemyNum < enemyNumMax)
        {
            foreach (SpawnPoint p in spawnPoints)
            {
                if (p.GetSpawnedNum() < 1)
                {
                    this.delta = 0;
                    Vector3 pos = new Vector3(0f, -100f, 0f);
                    int r = UnityEngine.Random.Range(1, 2);
                    GameObject enemy_ = EnemyA1;
                    if (r == 1)
                    {
                        enemy_ = Enemy_Teki01;
                    }

                    GameObject go = Instantiate(enemy_, pos, Quaternion.identity);
                    p.SpawnEnemy(go);

                    float r2 = UnityEngine.Random.Range(0f, 1f);
                    if(r2 <0.3f)
                    {
                        go.GetComponent<IOnHit>().Initialize(
                            "Enemy_Teki01",4,1,1,"SuicideBomb",true,5,1.0f,1.0f
                            );
                    }
                    currEnemyNum++;
                    break;
                }
            }
        }

        /*if (this.delta > this.span && currEnemyNum < enemyNumMax)
        {
            this.delta = 0;
            float px;
            float pz;
            Vector3 pos = new Vector3(0f, 10f, 0f);
            int r = UnityEngine.Random.Range(1, 2);
            GameObject enemy_ = EnemyA1;
            if(r == 1 )
            {
                enemy_ = Enemy_Teki01;
            }

            GameObject go = Instantiate(enemy_, pos, Quaternion.identity);
            do
            {
                px = UnityEngine.Random.Range(-10.0f, 10.0f);
            } while (px >= -3 && px <= 3);

            do
            {
                pz = UnityEngine.Random.Range(-5.0f, 5.0f);
            } while (pz >= -3 && pz <= 3);

            go.transform.position = new Vector3(px, 3, pz);
            Debug.Log(new Vector3(px, 3, pz));
            currEnemyNum++;
            
        }*/

    }
    public void DropItem(Vector3 pos)
    {

    }
    public void SetItem()
    {

    }
}
