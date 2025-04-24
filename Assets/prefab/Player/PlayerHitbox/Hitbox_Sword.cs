using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Hitbox_Sword : MonoBehaviour
{
    public Material critMaterial;
    public Material genMaterial;

    public GameObject hitParticleEffect;
    public GameObject hitParticleShiled;
    public GameObject defaultTrail;
    public GameObject fireTrail;
    public GameObject exolpPrefab;

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // 攻撃した敵を記録するハッシュセット

    private int damage;
    private float critical=0f;
    private bool isDefensePenetration = false;
    private bool isBleed = false;
    
    bool isDefDown=false;  //防御力減
    bool isAtkDown = false; //攻撃力減
    bool isRecover = false;
    bool isExolo = false;
    private CameraFollow camera1;

    private PlayerControl player;
    private Collider hitboxCollider;
    private Vector3 originalColliderSize; // 元のColliderサイズを保存

    public float resetColliderTime = 0f;

    private void OnEnable()
    {
        // 有効化されるたびに記録をクリアする
        hitTargets.Clear();

        // Colliderサイズを一時的に0にする
        if (hitboxCollider != null)
        {
            StartCoroutine(ResetCollider());
        }
    }
    private void OnDisable()
    {
        hitTargets.Clear();
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        hitboxCollider = GetComponent<Collider>();

        // Colliderの元のサイズを保存
        if (hitboxCollider != null)
        {
            originalColliderSize = hitboxCollider.bounds.size;
            hitboxCollider.enabled = true; // Colliderを有効化
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"SwordHitbox Enter {other}");
        CheckTrigger(other);
    }
    private void OnTriggerStay(Collider other)
    {
       // Debug.Log($"SwordHitbox Stay {other}");
        CheckTrigger(other);
    }

    private void CheckTrigger(Collider other)
    {

        if (other.gameObject.CompareTag("Enemy") && !hitTargets.Contains(other))
        {
            // 攻撃対象を記録する
            hitTargets.Add(other);
            IOnHit io = other.gameObject.GetComponent<IOnHit>();
            if (io != null)
            {
                if (io.IsDying()) return;
                // クリティカルヒット判定
                bool crit = Random.Range(0f, 1f) < critical;
                // ダメージを与える
                int dmg = io.OnHit(damage, crit, isDefensePenetration, isBleed, isDefDown, isAtkDown, isRecover);

                if (dmg >= 0)
                {
                    camera1.ZoomAndShakeCamera();

                    player.VibrateForDuration();
                    // ヒットした位置を取得
                    Vector3 contactPoint = other.ClosestPoint(transform.position);

                    // エフェクトを生成
                    GameObject effect = Instantiate(hitParticleEffect, contactPoint, Quaternion.identity);

                    // エフェクトを自動削除
                    Destroy(effect, 2f);
                }
                else
                {
                    camera1.ZoomAndShakeCamera();

                    player.VibrateForDuration(0.2f,0.3f);
                    // ヒットした位置を取得
                    Vector3 contactPoint = other.ClosestPoint(transform.position);

                    // エフェクトを生成
                    GameObject effect = Instantiate(hitParticleShiled, contactPoint, Quaternion.identity);

                    // エフェクトを自動削除
                    Destroy(effect, 2f);
                }
                if(isExolo)
                {
                    Vector3 contactPoint = other.ClosestPoint(transform.position);

                    // エフェクトを生成
                    GameObject effect = Instantiate(exolpPrefab, contactPoint, Quaternion.identity);
                    effect.GetComponent<Hitbox_PlayerExplosion>().Initialized(player, camera1, damage);
                }
            }
        }
    }

    public void Initialize(CameraFollow camera_, int dmg, 
        int type, 
        float criRate, 
        bool isDefPen,
        bool isBleed_,   //流血、燃焼
        bool isDefDown_,  //防御力減
        bool isAtkDown_, //攻撃力減
        bool isRecover_ ,
        bool isExplo_
        )
    {
        camera1 = camera_;
        damage = dmg;
        critical = criRate;
        isDefensePenetration = isDefPen;
        isBleed = isBleed_;
        isDefDown = isDefDown_;
        isAtkDown = isAtkDown_;
        isRecover = isRecover_;
        isExolo = isExplo_;
        switch (type)
        {
            case 0: SetDefaultTrail(); break;
            case 1: SetFireTrail(); break;
        }
    }

    private void SetDefaultTrail()
    {
        isBleed = false;
        defaultTrail.SetActive(true);
        fireTrail.SetActive(false);
    }

    private void SetFireTrail()
    {
        isBleed = true;
        defaultTrail.SetActive(false);
        fireTrail.SetActive(true);
    }

    /// <summary>
    /// Colliderサイズを一時的に0にし、resetColliderTime秒後に元のサイズに戻す
    /// </summary>
    private IEnumerator ResetColliderSize()
    {
        // サイズを0に設定
        Vector3 zeroSize = Vector3.zero;

        // BoxColliderの場合、サイズを直接設定
        if (hitboxCollider is BoxCollider boxCollider)
        {
            boxCollider.size = zeroSize;
        }
        yield return new WaitForSeconds(resetColliderTime / GameManager.Instance.playerStatus.GetAttackSpeed());

        // 元のサイズに戻す
        if (hitboxCollider is BoxCollider boxColliderRestore)
        {
            boxColliderRestore.size = originalColliderSize;
        }

    }
    private IEnumerator ResetCollider()
    {
        hitboxCollider.enabled = false;
        yield return new WaitForSeconds(resetColliderTime / GameManager.Instance.playerStatus.GetAttackSpeed());
        hitboxCollider.enabled = true;
    }
}
