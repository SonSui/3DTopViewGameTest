using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Teki04 : MonoBehaviour
{
    EnemyStatus enemyStatus; //敵のステータス
    public Animator animator;

    //Inspector上設定できる基本のステータス
    public string name_ = "Enemy_Boss01";
    public int hp_ = 4;
    public int attack_ = 1;
    public int defense_ = 1;
    public string type_ = "SpiderBoss";
    public bool hasShiled_ = false;
    public int shieldDurability_ = 0;
    public float moveSpeed_ = 1.0f;
    public float attackSpeed_ = 1.0f;


    //被弾の色変化
    private Renderer[] renderers;        // 敵のRendererリスト
    [SerializeField] private Material overlayMaterial;   // 被弾のマテリアル
    private bool isFlashing = false;    // フラッシュ中かどうか
    private float flashDuration = 0.1f; // フラッシュ持続時間

    //光調節シェーダー
    //[SerializeField] private Material brightMaterial;

    //攻撃のPrefab
    public GameObject waveHitbox;    //攻撃のHitboxと攻撃のエフェクトをprefabにする
    public GameObject bulletPrefab;
    private GameObject hitbox = null; //生成したHitboxを保存
    public GameObject eye;
    public float attackRange = 1.5f;
    private bool isAttacking = false;
    float atkInterval = 2f;
    float atkTime = 0f;




    //ステータスマシン
    public enum EnemyState
    {
        //全部使う必要がない
        Idle,           // 待機状態：敵が動かずに待機している
        //Patrol,         // 巡回状態：指定されたルートやランダムに移動している
        //Chase,          // 追跡状態：プレイヤーを発見し追いかけている
        Attack,         // 攻撃状態：プレイヤーや目標を攻撃している
        Hit,            // 被撃状態：攻撃を受けてダメージを受けている
        Dead,           // 死亡状態：体力がゼロになり行動不能

        Stunned      // 気絶状態：スキルや攻撃で行動不能な状態
        //Flee,         // 逃走状態：プレイヤーに負けると判断し逃げる
        //Alert,        // 警戒状態：プレイヤーの存在に気づいたがまだ追跡していない
        //Guard,        // 防御状態：盾を持つ状態
    }
    public EnemyState _state = EnemyState.Idle;


    //プレイヤーの座標
    public Transform playerT = null;
    EnemyGenerator enemyGenerator;

    private float stunTime = 0f;
    private float stunTimeMax = 1f;

    public GameObject fireParticle;
    public GameObject shiled;


    public float fallSpeed = 10f; // 落下速度
    public float groundYPosition = -3.3f; // 地面のY座標（停止する高さ）

    private float liveTime = 0f;
    public float shiledTime = 20f;
    public int shiledDuration = 10;

    private void Awake()
    {

        // overlayMaterialが設定されているか確認


        // "low_poly_robot_3d_model_by_niko"という名前の子オブジェクトを探す
        Transform targetTransform = transform.Find("low_poly_robot_3d_model_by_niko");
        if (targetTransform == null)
        {
            Debug.LogError("指定されたオブジェクト 'low_poly_robot_3d_model_by_niko' が見つかりません！");
            return;
        }

        renderers = targetTransform.GetComponentsInChildren<Renderer>();

        /*
        //明るさ調整
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
        }*/
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
        if (fireParticle != null) fireParticle.SetActive(false);
        if (shiled != null) shiled.SetActive(false);
        liveTime = 0f;
        //playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Start()
    {
        //敵生成するとAwake->OnEnable(prefabはEnableの状態の場合)->Start->Update->Update->Update(毎フレイム循環)

        if (enemyGenerator == null) enemyGenerator = FindObjectOfType<EnemyGenerator>();

    }

    private void Update()
    {
        // 現在のY軸位置を確認
        float currentY = transform.position.y;

        // Y軸位置が地面（groundYPosition）より上なら落下を続ける
        if (currentY > groundYPosition)
        {
            // Y軸方向に移動（落下）
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
        if (playerT == null)
        {
            playerT = GameObject.FindGameObjectWithTag("Player").transform;
            if (playerT == null) { return; }
        }
        int bleedDmg = enemyStatus.UpdateStatus(Time.deltaTime);//流血、スタン、デバフなど毎フレイム自動的に処理

        if (bleedDmg > 0 && _state != EnemyState.Dead) //流血（燃焼）ダメージが出たら数字で表示
        {
            UIManager.Instance.ShowDamage(bleedDmg, transform.position, new Color(0.5f, 0f, 0.5f, 1f));
            if (enemyStatus.IsDead())
            {
                OnDead();
            }
        }
        if (enemyStatus.IsBleeding())
        {
            fireParticle.SetActive(true);
        }
        else fireParticle.SetActive(false);
        if (enemyStatus.HasShield()) shiled.SetActive(true);
        else shiled.SetActive(false);

        //状態更新
        StateUpdate();
    }




    // ===== ステート処理 =====
    private void StateUpdate()
    {
        switch (_state)
        {
            case EnemyState.Idle:
                OnIdle();// 待機中の処理
                break;


            //以下の行動はAnimationEventや他のオブジェクトが呼んでくれる
            case EnemyState.Attack:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;
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

                break;
            case EnemyState.Attack:
                OnAttaceState();
                break;
            case EnemyState.Hit:
                waveHitbox.SetActive(false);
                isAttacking = false;
                animator.SetTrigger("Hit");
                break;
            case EnemyState.Dead:
                animator.SetBool("Dead", true);
                animator.SetTrigger("Dead 0");
                break;
            case EnemyState.Stunned:
                stunTime = 0;
                animator.enabled = false;
                break;
        }
    }
    private void OnAttaceState()
    {
        atkTime = atkInterval;
        float distance = Vector3.Distance(transform.position, playerT.position);

        //プレイヤーの距離計算、遠いなら追跡、近いなら攻撃
        if (distance > attackRange)
        {
            animator.SetTrigger("Attack2");
        }
        else
        {
            animator.SetTrigger("Attack1");
        }
    }
    private void OnIdle()
    {
        liveTime += Time.deltaTime;
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
        if (liveTime > shiledTime)
        {
            if (!enemyStatus.HasShield())
            {
                enemyStatus.SetShield(true);
                enemyStatus.SetShieldDurability(shiledDuration);
                liveTime = 0;
            }
        }

    }

    private void OnStuned()
    {
        stunTime += Time.deltaTime;
        if (stunTime < stunTimeMax) return;
        else
        {
            animator.enabled = true;
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
        isFlashing = false;　//被弾の行動終了

    }


    private void OnDead()
    {
        if (IsDying()) return;
        ChangeState(EnemyState.Dead);//死亡状態
        //死亡アニメーションとエフェクト
        StartCoroutine(DyingAnimation());

    }
    private IEnumerator DyingAnimation()
    {
        float dyingTime = 0f;
        float dyingTimeMax = 5.5f;//5秒後削除
        //死亡エフェクトを生成

        while (dyingTime < dyingTimeMax)
        {
            //削除前のアニメーション（例：小さくなるなど）

            dyingTime += Time.deltaTime;
            yield return null;
        }
        //EnemyGeneratorに通知
        if (enemyGenerator == null) enemyGenerator = FindObjectOfType<EnemyGenerator>();
        enemyGenerator.EnemyDead(gameObject);
        Debug.Log("BossDead");
        //アニメーション完了したら削除
        //Destroy(gameObject);
    }






    // =====　外部インタラクション　=====
    public void Initialize(
        string name = "Enemy_Boss01",
        int hpMax = 4,
        int attackPower = 1,
        int defense = 1,
        string enemyType = "SpiderBoss",
        bool hasShield = false,
        int shieldDurability = 0,

        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f
        )
    {
        enemyStatus = new EnemyStatus(name, hpMax, attackPower, defense, enemyType, hasShield, shieldDurability, moveSpeed, attackSpeed);
    }
    public void SetGenerator(EnemyGenerator generator)
    {
        enemyGenerator = generator;
    }
    public void OnAttack1Anime()
    {
        //攻撃AnimationEvent
        if (isAttacking) return;
        isAttacking = true;
        waveHitbox.SetActive(true);　//Hitbox有効化
        waveHitbox.GetComponent<Hitbox_Boss01_wave>().Initialized(enemyStatus.GetAttackNow() * 2); //攻撃力設定

    }
    public void OnAttack1Over()
    {
        isAttacking = false;
        ChangeState(EnemyState.Idle);
    }
    public void OnAttack2Anime()
    {
        isAttacking = true;
        GameObject bullet = Instantiate(bulletPrefab, eye.transform.position, Quaternion.identity);
        Vector3 eyeEulerAngles = eye.transform.eulerAngles;
        bullet.transform.rotation = Quaternion.Euler(0, eyeEulerAngles.y, 0);
        bullet.GetComponent<Hitbox_Boss01_Bullet>().Initialize(playerT, enemyStatus.GetAttackNow());
    }
    public void OnAttack2Over()
    {
        isAttacking = false;
        ChangeState(EnemyState.Idle);
    }
    public void OnIdleAnime()
    {
        ChangeState(EnemyState.Idle);
    }
    public void OnHitAnimeOver()
    {
        ChangeState(EnemyState.Idle);
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

            if (crit)
            {
                //クリティカルエフェクト（あれば）
            }
            if (isBleed) { enemyStatus.ApplyBleedingEffect(5f); }
            if (isDefDown) { enemyStatus.ApplyDefenseReduction(5f); }
            if (isAtkDown) { enemyStatus.ApplyAttackReduction(5f); }

            int hitDmg = enemyStatus.TakeDamage(dmg, isPenetrate);//防御力などの影響を含めてダメージ計算できる
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
            return hitDmg;
        }
        return 0;
    }
    public void OnHooked(int dmg)
    {
        Color displayColor = Color.red;
        //フックショットに当たる行動（シールド破壊）
        if (enemyStatus.HasShield())
        {
            enemyStatus.SetShield(false, 0);
            displayColor = Color.blue;
        }
        ChangeState(EnemyState.Stunned);
        int hitDmg = enemyStatus.TakeDamage(dmg);
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
