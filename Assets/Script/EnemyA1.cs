using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class EnemyA1 : MonoBehaviour
{
    EnemyStatus enemyStatus;
    EnemyGenerator enemyGenerator;

    public Color hitColor = Color.red;

    public float hitDuration = 0.1f;

    public GameObject hitboxPrefab;
    private GameObject hitbox=null;

    private Material oriMaterial;
    private Material temMaterial;

    //ステータス
    int hp = 4;
    float speed = 2.0f;
    int dmg = 1;
    float atkInterval = 4f;
    float atkTime = 0f;

    //死亡判定
    bool isDying = false;

    //プレイヤーの座標
    public Transform playerT;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        oriMaterial = renderer.material;
        temMaterial = new Material(oriMaterial);
        enemyGenerator = FindObjectOfType<EnemyGenerator>();

        playerT = GameObject.FindGameObjectWithTag("Player").transform;

    }
    private void Update()
    {

        if (hp < 0) 
        {
            enemyGenerator.deadEnemyNum++;
            Destroy(gameObject);
        }

        if(hitbox==null)
        {
            atkTime += Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, playerT.position) < 3.0f && hitbox == null && atkTime>atkInterval)
        {
            Attack();

        }

        //プレイヤーとの距離が近くなったら移動を止める
        if (Vector3.Distance(transform.position, playerT.position) < 2.1f){ return; }

        //プレイヤーに向けて進む
        transform.position = 
            Vector3.MoveTowards(transform.position, new Vector3(playerT.position.x, playerT.position.y, playerT.position.z), speed * Time.deltaTime);
    }


    //撃たれると0.1秒間赤くなる
    public int OnHit(int dmg,
        bool crit = false,
        bool isPenetrate = false,
        bool isBleed = false,
        bool isDefDown = false,
        bool isAtkDown = false,
        bool isRecover = false)
    {
        if (isDying) return 0;

        hp -= dmg;
        StartCoroutine(ChangeColorTemporarily());
        //被弾アニメーションとエフェクト

        if (hp < 0)
        {
            enemyGenerator.deadEnemyNum += 1;
            OnDead();
        }
        DeleteHitbox();
        atkTime = 0;
        return dmg;
    }
    public void OnHooked(int dmg)
    {
        //未実装
    }
    private IEnumerator ChangeColorTemporarily()
    {
        
        temMaterial.color = hitColor;
        GetComponent<Renderer>().material = temMaterial;

        
        yield return new WaitForSeconds(hitDuration);

        
        temMaterial.color = oriMaterial.color;
        GetComponent<Renderer>().material = temMaterial;
    }
    private void OnDead()
    {
        isDying = true;
        //死亡アニメーションとエフェクト

        //アニメーション完了したら削除
        Destroy(gameObject);
    }
    private void Attack()
    {
        hitbox = Instantiate(hitboxPrefab);
        hitbox.GetComponent<Hitbox_EnemyA1>().Initialized(dmg);
        hitbox.transform.position = transform.position;
        hitbox.transform.SetParent(transform);
        atkTime = 0f;
    }
    private void DeleteHitbox()
    {
        if(hitbox!=null)Destroy(hitbox.gameObject);
    }
}
