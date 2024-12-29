using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Enemy_Teki4_3 : MonoBehaviour
{
    EnemyStatus enemyStatus; //敵のステータス
    EnemyGenerator enemyGenerator;

    //Inspector上設定できる基本のステータス
    public string name_ = "Enemy_RobotSoldier";
    public int hp_ = 10;
    public int attack_ = 2;
    public int defense_ = 1;
    public string type_ = "SuicideBomb";
    public bool hasShiled_ = false;
    public int shieldDurability_ = 0;
    public float moveSpeed_ = 1.8f;
    public float attackSpeed_ = 1.0f;


    //被弾の色変化
    private Renderer[] renderers;        // 敵のRendererリスト
    [SerializeField] private Material overlayMaterial;   // 被弾のマテリアル
    private bool isFlashing = false;    // フラッシュ中かどうか
    private float flashDuration = 0.1f; // フラッシュ持続時間

    //攻撃のPrefab
    public GameObject hitboxPrefab;    //攻撃のHitboxと攻撃のエフェクトをprefabにする
    private GameObject hitbox = null;　//生成したHitboxを保存

    //ステータスマシン
    public enum EnemyState
    {
        //全部使う必要がない
        //Idle,         // 待機状態：敵が動かずに待機している
        //Patrol,       // 巡回状態：指定されたルートやランダムに移動している
        Chase,        // 追跡状態：プレイヤーを発見し追いかけている
        Attack,       // 攻撃状態：プレイヤーや目標を攻撃している
        Hit,          // 被撃状態：攻撃を受けてダメージを受けている
        Dead,         // 死亡状態：体力がゼロになり行動不能

        //Stunned,      // 気絶状態：スキルや攻撃で行動不能な状態
        //Flee,         // 逃走状態：プレイヤーに負けると判断し逃げる
        //Alert,        // 警戒状態：プレイヤーの存在に気づいたがまだ追跡していない
        //Guard,        // 防御状態：盾を持つ状態
    }
    private EnemyState _state = EnemyState.Chase;
    private EnemyState _nextstate = EnemyState.Chase;

    float count = 0.0f;//Idleで使用
    //プレイヤーの座標
    public Transform playerT;

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
        //ChangeState(EnemyState.Idle); //待機状態設定
    }

    public void SetGenerator(EnemyGenerator generator)
    {
        enemyGenerator = generator;
    }

    void Start()
    {
        //敵生成するとOnEnable(prefabはEnableの状態の場合)->Start->Update->Update->Update(毎フレイム循環)

        if (enemyGenerator == null) enemyGenerator = FindObjectOfType<EnemyGenerator>();
        playerT = GameObject.FindGameObjectWithTag("Player").transform;

        // 全てのRendererを取得
        renderers = GetComponentsInChildren<Renderer>();
        // overlayMaterialが未設定の場合エラー
        if (overlayMaterial == null)
        {
            Debug.Log("Overlay Material が設定されていません！");
        }

        //最初はChase
        ChaseUpdate();
    }

    private void Update()
    {
        switch(_state)
        { 
            //現在のUpsate
                case EnemyState.Chase:
                ChaseUpdate();
                break;
                case EnemyState.Attack:
                AttackUpdate();
                break;
                case EnemyState.Hit:
                HitUpdate();
                break;
                case EnemyState.Dead:
                DeadUpdate();
                break;
        }

        if (_state != _nextstate)
        {
            //終了処理
            switch (_state)
            {
                case EnemyState.Chase:
                    ChaseEnd();
                    break;
                case EnemyState.Attack:
                    AttackEnd();
                    break;
                case EnemyState.Hit:
                    HitEnd();
                    break;
                case EnemyState.Dead:
                    DeadEnd();
                    break;
            }

            //次のステート遷移
            _state = _nextstate;
            switch (_state)
            {
                case EnemyState.Chase:
                    ChaseStart();
                    break;
                case EnemyState.Attack:
                    AttackStart();
                    break;
                case EnemyState.Hit:
                    HitStart();
                    break;
                case EnemyState.Dead:
                    DeadStart();
                    break;
            }
        }

        count += 0.1f;
     enemyStatus.UpdateStatus(Time.deltaTime);//流血、スタン、デバフなど毎フレイム自動的に処理
        if(hp_ < 0)
        {
            ChangeState(EnemyState.Dead);
        }
    }

    public void ChangeState(EnemyState nextState)
    {
        _nextstate = nextState;
    }

    //ステート処理-----------------------------------------------------------------------------
    private void ChaseStart() { }
    private void ChaseUpdate()
    {
        //プレイヤーとの距離が近くなったら移動を止める
        if (Vector3.Distance(transform.position, playerT.position) < 2.1f)
        {
            ChangeState(EnemyState.Attack);
            return; 
        }

        //プレイヤーに向けて進む
        transform.position =
            Vector3.MoveTowards(transform.position, new Vector3(playerT.position.x, playerT.position.y, playerT.position.z), moveSpeed_ * Time.deltaTime);
    }
    private void ChaseEnd() { }

    bool attackFin = false;
    private IEnumerator WaitCoroutine()
    {
        //攻撃前隙
        yield return new WaitForSeconds(0.3f);

        attackFin = true;
    }
    private IEnumerator ChaseCoroutine()
    {
        yield return new WaitForSeconds(3.0f);

        ChangeState(EnemyState.Chase);
    }
    private void AttackStart() 
    { 
        StartCoroutine(WaitCoroutine());
        StartCoroutine(ChaseCoroutine());
    }
    private void AttackUpdate()
    {
        if (attackFin == true)
        {
            Attack();

            attackFin &= false;
        }
    }
    private void AttackEnd() 
    {
        ChangeState(EnemyState.Chase);
    }
    private void HitStart() { }
    private void HitUpdate() { }
    private void HitEnd() { }

    private void DeadStart() 
    {
        OnDead();
    }
    private void DeadUpdate() { }
    private void DeadEnd()
    { 
    }

    private bool enemyDying;//Enemyは死んでいるか？OnHitに使用

    public void OnHit(int dmg, bool crit = false)
    {

        if (enemyDying)//今の状態を判断、死んでいるのはダメージ受けない
        {

            hp_ -= dmg;
            enemyStatus.TakeDamage(dmg);//防御力などの影響を含めてダメージ計算できる
            //被弾アニメーションとエフェクト

            if (enemyStatus.IsDead())
            {
                OnDead();
                return;
            }
            if (!isFlashing)
            {
                if (overlayMaterial != null) StartCoroutine(HitFlash());
            }
        }
    }

    private System.Collections.IEnumerator HitFlash()
    {
        ChangeState(EnemyState.Hit);// 被撃状態

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
        ChangeState(EnemyState.Dead);//死亡状態
        //死亡アニメーションとエフェクト
        StartCoroutine(DyingAnimation());

    }
    private IEnumerator DyingAnimation()
    {
        float dyingTime = 0f;
        float dyingTimeMax = 0.5f;//0.5秒後削除
        //死亡エフェクトを生成

        while (dyingTime < dyingTimeMax)
        {
            //削除前のアニメーション（例：小さくなるなど）

            dyingTime += Time.deltaTime;
            yield return null;
        }
        //EnemyGeneratorに通知
        if (enemyGenerator != null) enemyGenerator.deadEnemyNum++;
        //アニメーション完了したら削除
        Destroy(gameObject);
    }

    private void Attack()
    {
        int atk = enemyStatus.GetAttackNow();  //攻撃力ダウンなどを計算した攻撃力を取得
        hitbox = Instantiate(hitboxPrefab);
        hitbox.transform.position = transform.position;
        hitbox.transform.SetParent(transform);
        //攻撃のprefabを生成
        
    }

}