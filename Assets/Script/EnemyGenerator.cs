using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject EnemyA1;
    float span = 5.0f;
    float delta = 0;
    int enemyNumMax = 3;
    int currEnemyNum = 0;
    public int deadEnemyNum = 0;
    public GameObject clearUI;
    // Start is called before the first frame update
    void Start()
    {
        clearUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        this.delta += Time.deltaTime;
        if (currEnemyNum < enemyNumMax)
        {
            if (this.delta > this.span)
            {
                this.delta = 0;
                GameObject go = Instantiate(EnemyA1);
                int px = Random.Range(-20, 20);
                go.transform.position = new Vector3(px, 0, 0);
                currEnemyNum++;
            }
        }
        if(deadEnemyNum>= enemyNumMax)
        {
            clearUI.SetActive(true);
        }
        
    }
}
