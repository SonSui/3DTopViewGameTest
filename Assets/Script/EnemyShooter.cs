using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class EnemyShooter : MonoBehaviour, IOnHit
{
    EnemyStatus enemyStatus;

    public Color hitColor = Color.red;

    public float hitDuration = 0.1f;

    public GameObject hitboxPrefab;
    private GameObject hitbox = null;

    public GameObject bulletHitbox;
    GameManager gameManager = GameManager.Instance;

    private Material oriMaterial;
    private Material temMaterial;

    int hp = 4;

    float speed = 2.0f;
    int dmg = 1;

    float atkInterval = 4f;
    float atkTime = 0f;
    float shootTime = 0f;

    //死亡判定
    bool isDying = false;

    //プレイヤーの座標
    public Transform playerT;

    EnemyGenerator enemyGenerator;

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

        if (hitbox == null)
        {
            atkTime += Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, playerT.position) < 3.0f && hitbox == null && atkTime > atkInterval)
        {
            Attack();
           

        }


        if (Vector3.Distance(transform.position, playerT.position) < 10.0f)
        {
            shootTime += Time.deltaTime;
            if (shootTime > 1)
            {
                Shoot();
                shootTime = 0;
            }
        }


        //プレイヤーとの距離が近くなったら移動を止める
        if (Vector3.Distance(transform.position, playerT.position) < 10.0f) { 
            //face to player

            transform.LookAt(new Vector3(playerT.position.x, transform.position.y, playerT.position.z));
            shootTime += Time.deltaTime;
            if (shootTime > 1)
            {
                Shoot();
                shootTime = 0;
            }



            return; 
        }






            //プレイヤーに向けて進む
            transform.position =
            Vector3.MoveTowards(transform.position, new Vector3(playerT.position.x, playerT.position.y, playerT.position.z), speed * Time.deltaTime);
    }


    //撃たれると0.1秒間赤くなる
    public void OnHit(int dmg, bool crit = false)
    {
        if (isDying) return;

        hp -= dmg;
        StartCoroutine(ChangeColorTemporarily());
        //被弾アニメーションとエフェクト

        if (hp < 0)
        {
            enemyGenerator.deadEnemyNum++;
            OnDead();

        }
        DeleteHitbox();
        atkTime = 0;
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
    }

    private void Shoot()
    {
        Vector3 gunRot = transform.eulerAngles;
        gunRot.x = 90f;
        Vector3 bulletPos = transform.position;
        bulletPos.y += 2;

        GameObject bull = Instantiate(bulletHitbox, bulletPos, Quaternion.Euler(gunRot));
        bull.GetComponent<HitBox_EnemyBullet>().Initialize(5);
    }
    private void DeleteHitbox()
    {
        if (hitbox != null) Destroy(hitbox.gameObject);
    }
}
