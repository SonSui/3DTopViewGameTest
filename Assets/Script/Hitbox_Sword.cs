using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Sword : MonoBehaviour
{
    public Material critMaterial;
    public Material genMaterial;

    public GameObject hitParticleEffect;
    public GameObject defaultTrail;
    public GameObject fireTrail;

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // 攻撃した敵を記録するハッシュセット

    private int damage;
    private float critical;
    private bool isDefensePenetration;
    private bool isBleed;
    private CameraFollow camera1;

    private PlayerControl player;
    private Collider hitboxCollider;
    private Vector3 originalColliderSize; // 元のColliderサイズを保存

    private void OnEnable()
    {
        // 有効化されるたびに記録をクリアする
        hitTargets.Clear();

        // Colliderサイズを一時的に0にする
        if (hitboxCollider != null)
        {
            StartCoroutine(ResetColliderSize());
        }
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
        Debug.Log($"Hit {other}");
        if (other.gameObject.CompareTag("Enemy") && !hitTargets.Contains(other))
        {
            // 攻撃対象を記録する
            hitTargets.Add(other);
            if (other.gameObject.GetComponent<IOnHit>() != null)
            {
                // クリティカルヒット判定
                bool crit = Random.Range(0f, 1f) < critical;
                // ダメージを与える
                int dmg = other.gameObject.GetComponent<IOnHit>().OnHit(damage, crit, isDefensePenetration, isBleed);

                if (dmg != 0)
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
            }
        }
    }

    public void Initialize(CameraFollow camera_, int dmg, int type = 0, float criRate = 0.01f, bool isDefPen = false)
    {
        camera1 = camera_;
        damage = dmg;
        critical = criRate;
        isDefensePenetration = isDefPen;
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
    /// Colliderサイズを一時的に0にし、0.1秒後に元のサイズに戻す
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
        

        // 0.1秒待つ
        yield return new WaitForSeconds(0.05f);

        // 元のサイズに戻す
        if (hitboxCollider is BoxCollider boxColliderRestore)
        {
            boxColliderRestore.size = originalColliderSize;
        }
        
    }
}
