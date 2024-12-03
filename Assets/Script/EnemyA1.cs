using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA1 : MonoBehaviour
{
    public Color hitColor = Color.red;

    public float hitDuration = 0.1f;

    private Material oriMaterial;
    private Material temMaterial;

    int hp = 4;
    float speed = 2.0f;

    //プレイヤーの座標
    private Transform playerT;

    EnemyGenerator enemyGenerator;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        oriMaterial = renderer.material;
        temMaterial = new Material(oriMaterial);
        //enemyGenerator = FindObjectOfType<EnemyGenerator>();

        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        if (hp < 0) {
            //enemyGenerator.deadEnemyNum++;
            Destroy(gameObject);
        }

        //プレイヤーとの距離が近くなったら移動を止める
        if (Vector3.Distance(transform.position, playerT.position) < 2.1f)
        return;

        //プレイヤーに向けて進む
        transform.position = 
            Vector3.MoveTowards(transform.position, new Vector3(playerT.position.x, playerT.position.y, playerT.position.z), speed * Time.deltaTime);
    }


    //撃たれると0.1秒間赤くなる
    public void OnHit(int dmg)
    {
        hp -= dmg;
        StartCoroutine(ChangeColorTemporarily());
    }
    private IEnumerator ChangeColorTemporarily()
    {
        
        temMaterial.color = hitColor;
        GetComponent<Renderer>().material = temMaterial;

        
        yield return new WaitForSeconds(hitDuration);

        
        temMaterial.color = oriMaterial.color;
        GetComponent<Renderer>().material = temMaterial;
    }
}
