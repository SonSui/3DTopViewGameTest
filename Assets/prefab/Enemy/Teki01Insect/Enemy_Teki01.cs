using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Teki01 : MonoBehaviour, IOnHit
{
    EnemyStatus enemyStatus; //敵のステータス
    [Header("敵のモデリング")]
    public Animator animator;
    public Transform targetTransform;//エネミーのモデリング、エフェクトなどが不要

    //Inspector上設定できる基本のステータス
    [Header("敵のステータス")]
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
    [Header("被弾フラッシュのマテリアル")]
    [SerializeField] private Material overlayMaterial;   // 被弾のマテリアル
    private bool isFlashing = false;    // フラッシュ中かどうか
    private float flashDuration = 0.1f; // フラッシュ持続時間

    //光調節シェーダー、敵を明るくする。他の敵が暗くないなら、なしでもいい
    [Header("明るさ調整のマテリアル")]
    [SerializeField] private Material brightMaterial;

    //攻撃のPrefab
    [Header("攻撃のPrefab")]
    public GameObject biteHitbox;    //攻撃のHitboxと攻撃のエフェクトをprefabにする
    public GameObject bombPrefab;

    [Header("攻撃範囲と間隔の設定")]
    public float attackRange = 1.5f;　//プレイヤーがどのぐらい近くなると近接攻撃する
    private bool isAttacking = false;　//攻撃しているかどうか
    public float atkInterval = 1f;　//攻撃間隔
    float atkTime = 0f;　//次の攻撃可能までの時間


    //ステータスマシン
    public enum EnemyState
    {
        //全部使う必要がない
        Idle,           // 待機状態：敵が動かずに待機している
        //Patrol,         // 巡回状態：指定されたルートやランダムに移動している
        Chase,          // 追跡状態：プレイヤーを発見し追いかけている
        Attack,         // 攻撃状態：プレイヤーや目標を攻撃している
        Hit,            // 被撃状態：攻撃を受けてダメージを受けている
        Dead,           // 死亡状態：体力がゼロになり行動不能

        Bomb ,           // 爆発状態：虫敵の特殊能力

        Stunned      // 気絶状態：スキルや攻撃で行動不能な状態
        //Flee,         // 逃走状態：プレイヤーに負けると判断し逃げる
        //Alert,        // 警戒状態：プレイヤーの存在に気づいたがまだ追跡していない
        //Guard,        // 防御状態：盾を持つ状態
    }
    public EnemyState _state = EnemyState.Idle;


    //プレイヤーの座標
    [Header("Player(ドラッグしない)")]
    public Transform playerT;
    EnemyGenerator enemyGenerator;

    private float stunTime=0f;
    private float stunTimeMax = 1f;

    [Header("Debuffのエフェクト")]
    public GameObject fireParticle;
    public GameObject shiled;
    public GameObject debuff_Atk;
    public GameObject debuff_Def;

    private void Awake()
    {

        //UnityのMonoBehaviourによる：
        //GameObject生成するとAwake->OnEnable(prefabはEnableの状態の場合)->Start->Update->Update->Update(毎フレイム循環)


        renderers = targetTransform.GetComponentsInChildren<Renderer>();

        // overlayMaterialが設定されているか確認
        if (overlayMaterial == null)
        {
            Debug.LogError("Overlay Material が設定されていません！");
            return;
        }

        if(brightMaterial==null)
        {
            Debug.LogError("Bright Material が設定されていません！");
            return;
        }


        //明るさ調整、敵が暗くないなら、削除でもいい
        foreach (Renderer renderer in renderers)
        {
            // 現在のオブジェクトの全てのマテリアルを取得
            var materials = renderer.materials;

            // 新しいマテリアル配列を作成（既存のマテリアルに加えて明るさ用のマテリアルを追加）
            var newMaterials = new Material[materials.Length + 1];
            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }
            newMaterials[materials.Length] = brightMaterial; // 増亮用マテリアルを追加
            renderer.materials = newMaterials;
        }
    }
    private void OnEnable()
    {
        name_ += System.Guid.NewGuid().ToString(); //唯一の名前付ける

        //EnemyPoolを使うなら、Enableたびにステータスをリセットする
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
        ChangeState(EnemyState.Idle); //待機状態設定
        if(fireParticle != null)fireParticle.SetActive(false);
        if(shiled!=null)shiled.SetActive(false);
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Start()
    {
        if (enemyGenerator == null) enemyGenerator = FindObjectOfType<EnemyGenerator>();
    }

    private void Update()
    {
        int bleedDmg = enemyStatus.UpdateStatus(Time.deltaTime);//流血、スタン、デバフなど毎フレイム自動的に処理

        if (bleedDmg > 0 && _state != EnemyState.Dead) //流血（燃焼）ダメージが出たら数字で表示
        {
            UIManager.Instance.ShowDamage(bleedDmg, transform.position, new Color(0.5f, 0f, 0.5f, 1f));
            if (enemyStatus.IsDead())
            {
                OnDead();
            }
        }

        //Debuffのエフェクト表示
        if (enemyStatus.IsBleeding())fireParticle.SetActive(true);
        else fireParticle.SetActive(false);
        if(enemyStatus.HasShield())shiled.SetActive(true);
        else shiled.SetActive(false);
        if(enemyStatus.IsAttackReduced())debuff_Atk.SetActive(true);
        else debuff_Atk.SetActive(false);
        if (enemyStatus.IsDefenseReduced())debuff_Def.SetActive(true);
        else debuff_Def.SetActive(false);
        

        //状態更新
        StateUpdate();
    }
    void OnDestroy()　//UnityがGameObjectを削除するたびに自動的に呼ぶ関数
    {
        Debug.Log($"{this.name_}削除処理");
        //EnemyGeneratorに通知
        if (enemyGenerator != null) enemyGenerator.EnemyDead(gameObject);
    }


    // ===== ステート処理 =====
    private void StateUpdate()
    {
        switch (_state)
        {
            case EnemyState.Idle:
                OnIdle();// 待機中の処理
                break;
            case EnemyState.Chase:
                OnChase();// 追跡の処理
                break;

            //以下の行動はAnimationEventや他のオブジェクトが呼んでくれるので、何もしない
            
            case EnemyState.Attack:
                break;
            case EnemyState.Bomb:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;

            //気絶状態
            case EnemyState.Stunned:
                OnStuned();
                break;
        }
    }
    private void ChangeState(EnemyState nextState)
    {
        _state = nextState;
        //状態変更したら、アニメーションも変更
        switch (nextState)
        {
            case EnemyState.Idle:
                animator.SetBool("Chase", false);
                break;

            case EnemyState.Chase:
                animator.SetBool("Chase", true);
                break;
            case EnemyState.Attack:
                animator.SetTrigger("Bite");
                atkTime = atkInterval;
                break;
            case EnemyState.Hit:
                biteHitbox.SetActive(false);
                isAttacking = false;　//被弾するとすぐ被弾アニメーションを流す、攻撃判断のリセットが必要
                animator.SetTrigger("Hit");
                break;
            case EnemyState.Bomb:
                animator.SetTrigger("Bomb");
                break;
            case EnemyState.Dead:
                animator.SetTrigger("Dead");
                break;
            case EnemyState.Stunned:
                stunTime = 0;
                animator.enabled = false;　//アニメーションを止める
                break;
        }
    }
    private void OnIdle()
    {
        float distance = Vector3.Distance(transform.position, playerT.position);

        //プレイヤーの距離計算、遠いなら追跡、近いなら攻撃
        if (distance > attackRange)
        {
            ChangeState(EnemyState.Chase);
        }
        else
        {
            //攻撃間隔の判断
            if (atkTime > 0)
            {
                atkTime -= Time.deltaTime;
            }
            else
            {
                atkTime = 0;
                ChangeState(EnemyState.Attack);
            }
        }
    }
    private void OnChase()
    {
        if (playerT != null)
        {
            //移動
            transform.position = Vector3.MoveTowards(transform.position, playerT.position, enemyStatus.GetMoveSpeed() * Time.deltaTime);
            float distance = Vector3.Distance(transform.position, playerT.position);

            //回転
            Vector3 direction = (playerT.position - transform.position).normalized;
            direction.y = 0;
            if (direction.magnitude > 0.01)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            //状態変更
            if (distance <= attackRange)
            {
                //攻撃間隔の判断
                if (atkTime > 0)
                {
                    atkTime -= Time.deltaTime;
                    ChangeState(EnemyState.Idle);
                }
                else
                {
                    atkTime = 0;
                    ChangeState(EnemyState.Attack);
                }
            }
        }
    }

    private void OnStuned()
    {
        stunTime += Time.deltaTime;
        if (stunTime < stunTimeMax) return;　//気絶時間が終わるまで状態変更しない
        else
        {
            animator.enabled = true;　//アニメーション再開
            ChangeState(EnemyState.Idle);
        }
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

        //被弾のアニメーションが終了したかを確認し
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95) yield return null;

        isFlashing = false;　//被弾のフラッシュ終了
    }


    private void OnDead()
    {
        if(IsDying())return;
        ChangeState(EnemyState.Dead);//死亡状態
    }


    // ===== AnimationEvent =====
    public void OnBiteAnime()
    {
        //攻撃AnimationEvent
        if (isAttacking) return;
        isAttacking = true;
        biteHitbox.SetActive(true);　//Hitbox有効化
        biteHitbox.GetComponent<Hitbox_Teki01_Bite>().Initialized(enemyStatus.GetAttackNow()); //攻撃力設定

    }
    public void OnBiteAnimeEnd()
    {
        isAttacking = false;
        biteHitbox.SetActive(false);
        ChangeState(EnemyState.Idle);
    }
    public void OnBombAnime()
    {
        isAttacking = true;
        GameObject bomb = Instantiate(bombPrefab, transform);　//爆発Hitbox生成
        bomb.GetComponent<Hitbox_Teki01_Bomb>().Initialized(enemyStatus.GetAttackNow());
        //爆発したら死ぬ
        OnDead();
    }
    public void OnIdleAnime()
    {
        ChangeState(EnemyState.Idle); //想定外の状態変更を対応するために、待機アニメになると、スクリプト内も待機にする
    }
    public void OnHitAnimeEnd()
    {
        ChangeState(EnemyState.Bomb);　//虫は被弾したら、爆発、他の敵は待機に戻る
    }
    public void OnDeadAnimeEnd()
    {

        //アニメーション完了したら削除
        Destroy(gameObject);
    }



    // =====　外部インタラクション　=====
    
    public void SetGenerator(EnemyGenerator generator)
    {
        enemyGenerator = generator;
    }



    public void Initialize(
        string name = "Enemy_Teki01",
        int hpMax = 4,
        int attackPower = 1,
        int defense = 1,
        string enemyType = "SuicideBomb",
        bool hasShield = false,
        int shieldDurability = 0,

        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f
        )
    {
        enemyStatus = new EnemyStatus(name, hpMax, attackPower, defense, enemyType, hasShield, shieldDurability, moveSpeed, attackSpeed);
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

        if (_state != EnemyState.Dead && !enemyStatus.IsDead())//今の状態を判断、死んでいるのはダメージ受けない
        {
            bool isBomb = false;
            if (crit)
            {
                //クリティカルエフェクト（あれば）
            }
            if (isBleed) { enemyStatus.ApplyBleedingEffect(5f); }
            if (isDefDown) { enemyStatus.ApplyDefenseReduction(5f); }
            if (isAtkDown) { enemyStatus.ApplyAttackReduction(5f); }

            int hitDmg = enemyStatus.TakeDamage(dmg, isPenetrate);//防御力などの影響を含めてダメージ計算できる
            int trueDmg = hitDmg;
            if (hitDmg != 0)
            {
                Color displayColor = Color.red;
                if (hitDmg < 0)
                {
                    displayColor = Color.white;
                    hitDmg = -hitDmg;
                }
                else
                {
                    //被弾アニメーションとエフェクト
                    isBomb = true;
                    if (enemyStatus.IsDead())
                    {
                        OnDead();
                        if (isRecover)
                        {
                            //回復エフェクト
                            GameManager.Instance.RecoverHP();
                        }

                    }
                    else ChangeState(EnemyState.Hit); // シールドないなら被撃状態
                }
                //Vector3 worldPosition = transform.position + Vector3.up * 1; // テキスト表示位置

                UIManager.Instance.ShowDamage(hitDmg, transform.position, displayColor);
                Debug.Log($"Enemyは{hitDmg}ダメージ受けた");
            }
            
            if (isFlashing) StopCoroutine("HitFlash");
            if (overlayMaterial != null) StartCoroutine(HitFlash());
            return trueDmg;
        }
        return 0;
    }
    public void OnHooked(int dmg)
    {
        Color displayColor = Color.red;
        //フックショットに当たる行動（シールド破壊）
        if (enemyStatus.HasShield())
        {
            enemyStatus.SetShield(false ,0);
            displayColor = Color.blue;
        }
        ChangeState(EnemyState.Stunned);
        int hitDmg=enemyStatus.TakeDamage(dmg);
        if (hitDmg != 0)
        {
            
            if (hitDmg < 0)
            {
                displayColor = Color.white;
                hitDmg = -hitDmg;
            }
            else
            {
                //被弾アニメーションとエフェクト

                if (enemyStatus.IsDead())
                {
                    OnDead();
                }
            }
            //Vector3 worldPosition = transform.position + Vector3.up * 1; // テキスト表示位置

            UIManager.Instance.ShowDamage(hitDmg, transform.position, displayColor);
            Debug.Log($"Enemyは{hitDmg}ダメージ受けた");
        }
    }
    
    public bool IsDying()
    {
        return _state == EnemyState.Dead;
    }
    
}

