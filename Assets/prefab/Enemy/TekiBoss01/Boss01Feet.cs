using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01Feet : MonoBehaviour, IOnHit
{
    public Animator animator;
    public Enemy_Boss01 boss;

    private Collider collider1; 
    private float triggerDuration = 1.55f; // Trigger状態の持続時間
    private float recoverDuration = 0.5f; // Colliderを元のサイズに戻す持続時間
    private Vector3 shrinkSize = new Vector3(0.001f, 0.001f, 0.001f); // 縮小時のサイズ

    private Vector3 originalSize; // 元のサイズを保存



    void Start()
    {
        // 対象のColliderを取得（BoxColliderを前提にしています）
        collider1 = GetComponent<Collider>();
        if (collider1 is BoxCollider boxCollider)
        {
            originalSize = boxCollider.size; // 元のサイズを記録
        }
    }


    public int OnHit(
    int dmg,                //ダメージ
    bool crit = false,      //クリティカル
    bool isPenetrate = false, //防御貫通
    bool isBleed = false,   //流血、燃焼
    bool isDefDown = false,  //防御力減
    bool isAtkDown = false, //攻撃力減
    bool isRecover = false  //HP回復
    )
    {
        int hitDmg = boss.OnHit(dmg, crit, isPenetrate, isBleed, isDefDown, isAtkDown, isRecover);
        if (hitDmg != 0)
        {
            animator.SetTrigger("Hit");
        }
        return hitDmg;
    }
    public void OnHooked(int dmg)
    {
        boss.OnHooked(dmg);
    }
    public bool IsDying()
        { return boss.IsDying(); }
    public void Initialize(
        string name = "Enemy",
        int hpMax = 3,
        int attackPower = 1,
        int defense = 1,
        string enemyType = "Normal",
        bool hasShield = false,
        int shieldDurability = 0,

        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f)
    { }


    public void ActivateColliderSequence()
    {
        StartCoroutine(ColliderSequence());
    }

    // 一連の処理（Trigger設定、縮小、サイズ復元）を実行するコルーチン
    private IEnumerator ColliderSequence()
    {
        // Trigger状態にする
        collider1.isTrigger = true;

        // 攻撃終わるまで待機
        yield return new WaitForSeconds(triggerDuration);

        if (collider1 is BoxCollider boxCollider)
        {
            boxCollider.size = shrinkSize;
        }
        collider1.isTrigger = false;

        if (collider1 is BoxCollider boxColliderRecover)
        {
            float elapsedTime = 0f;
            Vector3 startSize = boxColliderRecover.size; // 現在のサイズを開始サイズとして記録

            while (elapsedTime < recoverDuration)
            {
                // サイズを線形補間で徐々に元に戻す
                boxColliderRecover.size = Vector3.Lerp(startSize, originalSize, elapsedTime / recoverDuration);
                elapsedTime += Time.deltaTime;
                yield return null; 
            }

            // 元のサイズに設定
            boxColliderRecover.size = originalSize;
        }
    }
}
