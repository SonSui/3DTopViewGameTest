using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_PlayerBullet : MonoBehaviour
{
    
    public GameObject hitParticleEffect;
    public GameObject trailEffect;

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); //攻撃した敵を記録


    private int damage;
    private float critical;
    private bool isDefensePenetration;

    public float speed = 10f; 
    public float lifeTime = 5f;

    private int ammoPenetration = 0;


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
        Debug.Log("Box:crit:" + critical.ToString() + " DefPen:" + isDefensePenetration.ToString());
        if (other.gameObject.CompareTag("Enemy") && !hitTargets.Contains(other))
        {
            // 記録
            hitTargets.Add(other);
            // ダメージ与える
            other.gameObject.GetComponent<IOnHit>().OnHit(damage);

            // 接する位置
            Vector3 contactPoint = other.ClosestPoint(transform.position);

            // エフェクト生成
            GameObject effect = Instantiate(hitParticleEffect, contactPoint, Quaternion.identity);

            // 自動的に削除
            Destroy(effect, 2f);

            ammoPenetration--;
            if (ammoPenetration < 0)
            {
                if (trailEffect != null)
                {

                    trailEffect.transform.parent = null;


                    var effectDestroyer = trailEffect.GetComponent<TrailEffectDestroyer>();
                    if (effectDestroyer != null)
                    {
                        effectDestroyer.StartDestroySequence();
                    }
                }
                Destroy(gameObject);
            }
                
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
    private void OnDestroy()
    {
        
    }
}
