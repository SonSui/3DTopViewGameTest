using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Hitbox_Sword : MonoBehaviour
{
    public Material critMaterial;
    public Material genMaterial;

    public GameObject hitParticleEffect;
    public GameObject defaultTrail;
    public GameObject fireTrail;


    private HashSet<Collider> hitTargets = new HashSet<Collider>(); //攻撃した敵を記録


    private int damage;
    private float critical;
    private bool isDefensePenetration;


    private void OnEnable()
    {
        // 有効化されるたびに記録をクリアする
        hitTargets.Clear();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Box:crit:" + critical.ToString() + " DefPen:" + isDefensePenetration.ToString());
        if (other.gameObject.CompareTag("Enemy") && !hitTargets.Contains(other))
        {
            // 記録
            hitTargets.Add(other);
            if (other.gameObject.GetComponent<IOnHit>() != null)
            {
                // ダメージ与える
                other.gameObject.GetComponent<IOnHit>().OnHit(damage);

                // 接する位置
                Vector3 contactPoint = other.ClosestPoint(transform.position);

                // エフェクト生成
                GameObject effect = Instantiate(hitParticleEffect, contactPoint, Quaternion.identity);

                // 自動的に削除
                Destroy(effect, 2f);
            }

        }
    }
    

    public void Initialize(int dmg, int type = 0,float criRate = 0.01f, bool isDefPen = false)
    {
        damage = dmg;
        critical = criRate;
        isDefensePenetration = isDefPen;
        switch(type)
        {
            case 0:SetDefaultTrail();break;
            case 1:SetFireTrail();break;
        }
        
    }
    private void SetDefaultTrail()
    {
        defaultTrail.SetActive(true);
        fireTrail.SetActive(false);
    }
    private void SetFireTrail()
    {
        defaultTrail.SetActive(false );
        fireTrail.SetActive(true);
    }

}
