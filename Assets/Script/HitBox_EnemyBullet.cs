using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox_EnemyBullet : MonoBehaviour
{
    public GameObject hitParticleEffect;

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); //攻撃した敵を記録

    private GameObject hitbox = null;
    private int damage;
    private float critical;
    private bool isDefensePenetration;

    public float speed = 10f;
    public float lifeTime = 5f;

    PlayerStatus playerStatus;

    float atkTime = 0f;
    private void OnEnable()
    {
        // 有効化されるたびに記録をクリアする
       
        hitTargets.Clear();
        StartCoroutine(AutoDestroyAfterTime(lifeTime));

    }
    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("bullet hit on player!!!!");
            // 記録
            








            // ダメージ与える

        }

        
    }


    public void Initialize(int dmg, float criRate = 0.01f, bool isDefPen = false)
    {
        damage = dmg;
        critical = criRate;
        isDefensePenetration = isDefPen;

    }

    private IEnumerator AutoDestroyAfterTime(float time)
    {

        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
