using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Teki01 : MonoBehaviour, IOnHit
{
    EnemyStatus enemyStatus; //敵のステータス

    //Inspector上設定できるステータス
    public string name_ = "Enemy_Teki01";
    public int hp_ = 4;
    public int attack_ = 1;
    public int defense_ = 1;
    public string type_ = "SuicideBomb";
    public bool hasShiled_ = false;
    public int shieldDurability_ = 0;
    public float moveSpeed_ = 1.0f;
    public float attackSpeed_ = 1.0f;


    //被弾の色変化
    private Renderer[] renderers;        // 敵のRendererリスト
    [SerializeField] private Material overlayMaterial;   // 半透明赤色のマテリアル
    private bool isFlashing = false;    // フラッシュ中かどうか
    private float flashDuration = 0.1f; // フラッシュ持続時間

    //攻撃のPrefab
    public GameObject hitboxPrefab;
    private GameObject hitbox = null;　//生成したHitboxを保存

    //ステータスマシン
    public enum EnemyState
    {
        Idle,         // 待機状態：敵が動かずに待機している
        Patrol,       // 巡回状態：指定されたルートやランダムに移動している
        Chase,        // 追跡状態：プレイヤーを発見し追いかけている
        Attack,       // 攻撃状態：プレイヤーや目標を攻撃している
        Hit,          // 被撃状態：攻撃を受けてダメージを受けている

        Dead,         // 死亡状態：体力がゼロになり行動不能
        Stunned,      // 気絶状態：スキルや攻撃で行動不能な状態
        Flee,         // 逃走状態：プレイヤーに負けると判断し逃げる
        Alert,        // 警戒状態：プレイヤーの存在に気づいたがまだ追跡していない
        Guard,        // 防御状態：盾を持つ状態
    }

    private EnemyState enemyState;

    //プレイヤーの座標
    public Transform playerT;
    EnemyGenerator enemyGenerator;


    private void OnEnable()
    {
        name_ += System.Guid.NewGuid().ToString(); //唯一の名前付ける
        //EnemyBufferを使う場合、Enableたびにステータスをリセットする
        enemyStatus = new EnemyStatus(
            name_,
            hp_,
            attack_,
            defense_,
            type_,
            hasShiled_,
            shieldDurability_,
            moveSpeed_,
            attackSpeed_);
        enemyState = EnemyState.Idle;
    }

    void Start()
    {
        if (enemyGenerator == null) enemyGenerator = FindObjectOfType<EnemyGenerator>();
        playerT = GameObject.FindGameObjectWithTag("Player").transform;

        // 全てのRendererを取得
        renderers = GetComponentsInChildren<Renderer>();
        // overlayMaterialが未設定の場合エラー
        if (overlayMaterial == null)
        {
            Debug.Log("Overlay Material が設定されていません！");
        }
    }


    private void Update()
    {
        if (enemyStatus.IsDead())
        {

        }




        enemyStatus.UpdateStatus(Time.deltaTime);
    }
    public void OnHit(int dmg, bool crit = false)
    {

        enemyStatus.TakeDamage(dmg);
    
        //被弾アニメーションとエフェクト
        if (enemyStatus.IsDead())
        {
            OnDead();
            return;
        }
        if (!isFlashing)
        {
            StartCoroutine(HitFlash());
        }
    }

  



    public void SetGenerator(EnemyGenerator generator)
    {
        enemyGenerator = generator;
    }




    private System.Collections.IEnumerator HitFlash()
    {
        isFlashing = true;
        
        // 全てのRendererにマテリアルを追加
        foreach (Renderer renderer in renderers)
        {
            var materials = renderer.materials;
            var newMaterials = new Material[materials.Length + 1];
            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }
            newMaterials[materials.Length] = overlayMaterial; // 最後に追加
            renderer.materials = newMaterials;
        }
        // 指定時間待機
        yield return new WaitForSeconds(flashDuration);
        // マテリアルを削除
        foreach (Renderer renderer in renderers)
        {
            var materials = renderer.materials;
            if (materials.Length > 1)
            {
                var newMaterials = new Material[materials.Length - 1];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = materials[i];
                }
                renderer.materials = newMaterials; // 最後のマテリアルを除去
            }
        }

        isFlashing = false;
    }


    private void OnDead()
    {

        //死亡アニメーションとエフェクト

        //EnemyGeneratorに通知
        if(enemyGenerator!=null)enemyGenerator.deadEnemyNum++;
        //アニメーション完了したら削除
        Destroy(gameObject);
    }

    private void Attack()
    {
        int atk = enemyStatus.GetAttackNow();
        //攻撃のprefabを生成
    }

}

