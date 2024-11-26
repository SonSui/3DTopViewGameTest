using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject EnemyA1;
    float span = 5.0f;
    float delta = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
