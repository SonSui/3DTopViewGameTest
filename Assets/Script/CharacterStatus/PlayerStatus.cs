using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤークラス：プレイヤーのステータス管理
public class PlayerStatus : BaseCharacterStatus
{

    private AbilityManager abilityManager;
    // ===== プレイヤー専用の属性 =====

    // 攻撃範囲
    protected float attackRange;  // 攻撃範囲（1.0が基準）

    // 回避率
    protected float evasionRate;  // 回避率

    // クリティカル関連
    protected float criticalRate;    // クリティカル率
    protected float criticalDamage;  // クリティカルダメージ倍率

    // 弾量関連
    private int ammoCapacity;    // 弾量
    private int ammoRecovery;    // 敵を倒したときに回復する弾数（デフォルト0）
    private float ammoEcho;      // 弾節約確率
    private int ammoPenetration; // 弾貫通



    // 特殊能力の有効化フラグ
    private bool hpRecovery;          // HP回復する
    private bool explosion;           // 爆発
    private bool timeStop;            // 時間停止
    private bool teleport;            // 瞬間移動
    private bool timedPowerUpMode;    // 時限強化モード
    private bool swordBeam;           // ソードビーム
    private bool resurrection;        // 復活
    private int resurrectionTime;     // 復活可能な回数

    private bool barrier;             // バリア
    private int barrierHP;          　// バリアのHP
    private bool oneHitKill;          // 一撃必殺
    private bool multiAttack;         // 多重攻撃
    private bool defensePenetration;  // 防御貫通


    private bool isDefenseReduction;    // 防御力ダウン
    private bool isAttackReduction;  // 攻撃力ダウン
    private bool isSlowEffect;       // 減速
    private bool isBleedingEffect;   // 流血
    private bool isStun;             // スタン

    // ===== コンストラクタ =====

    // 初期化（基底クラスのコンストラクタを呼び出す）
    public PlayerStatus(
        int hpMax = 5,
        int attackPower = 3,
        float criticalRate = 0.1f,
        float criticalDamage = 2.0f,
        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f,
        float attackRange = 1.0f,
        float evasionRate = 0.05f,
        int ammoCapacity = 10,
        int ammoRecovery = 0,
        int resurrectionTime = 1,
        string name = "Player"
    ) : base(
        name: name,
        hpMax: hpMax,
        attackPower: attackPower,
        defense: 0, // プレイヤーの防御力は常に0
        moveSpeed: moveSpeed,
        attackSpeed: attackSpeed
    )
    {
        // プレイヤー専用の属性を初期化

        this.criticalRate = criticalRate;
        this.criticalDamage = criticalDamage;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;
        this.evasionRate = evasionRate;


        this.ammoCapacity = ammoCapacity;
        this.ammoRecovery = ammoRecovery;
        this.ammoEcho = 0f;
        this.ammoPenetration = 0;

        this.explosion = false;
        this.timeStop = false;
        this.teleport = false;
        this.timedPowerUpMode = false;
        this.swordBeam = false;
        this.resurrection = false;
        this.resurrectionTime = resurrectionTime;

        this.barrier = false;
        this.barrierHP = 0;
        this.oneHitKill = false;
        this.multiAttack = false;
        this.defensePenetration = false;
        this.isAttackReduction = false;
        this.isBleedingEffect = false;
        this.isDefenseReduction = false;
        this.isSlowEffect = false;
        this.isStun = false;
    }

    // ===== メソッド =====

    // ダメージを受ける処理
    public override void TakeDamage(int damage, bool isDefensePenetration = false)
    {
        // バリアが有効な場合、ダメージを受けない
        if (barrier)
        {
            Debug.Log("バリアによってダメージを無効化");
            return;
        }

        // 回避判定
        if (Random.value < evasionRate)
        {
            Debug.Log("攻撃を回避した");
            return;
        }

        // 防御貫通が有効な場合、防御力を無視（防御力は常に0なので影響なし）
        int actualDamage = damage;

        // ダメージ適用
        hpNow -= actualDamage;
        Debug.Log($"プレイヤーは{actualDamage}のダメージを受けた（残りHP: {hpNow}/{hpMax}）");

        if (IsDead())
        {
            if (resurrection && resurrectionTime > 0)
            {
                Resurrection();
            }
            else
            {
                OnDeath();
            }
        }
    }

    // ステータスを更新（毎フレーム呼び出す）
    public override void UpdateStatus(float deltaTime, float timeRate = 1.0f)
    {
        base.UpdateStatus(deltaTime, timeRate);

        float adjustedDeltaTime = deltaTime * timeRate;


        // その他の特殊能力の処理（必要に応じて実装）
    }

    // 復活処理
    private void Resurrection()
    {
        resurrectionTime--;
        hpNow = hpMax; // HPを全回復
        Debug.Log($"プレイヤーは復活した（残り復活回数: {resurrectionTime}）");
    }

    // 死亡時の処理
    protected override void OnDeath()
    {
        // プレイヤーの死亡処理
        Debug.Log("プレイヤーは倒れた");
        // ゲームオーバー処理などを実装
    }

    // ===== Getter/Setter =====

    // 弾量
    public int GetAmmoCapacity() => ammoCapacity;
    public void SetAmmoCapacity(int value) => ammoCapacity = Mathf.Max(0, value);

    // 弾回復量
    public int GetAmmoRecovery() => ammoRecovery;
    public void SetAmmoRecovery(int value) => ammoRecovery = Mathf.Max(0, value);

    // HP自動回復量
    public bool GetHpAutoRecovery() => hpRecovery;
    public void SetHpAutoRecovery(bool value) => hpRecovery = value;


    // 特殊能力の有効化フラグのGetter/Setter

    // 爆発
    public bool IsExplosionEnabled() => explosion;
    public void SetExplosion(bool value) => explosion = value;

    // 時間停止
    public bool IsTimeStopEnabled() => timeStop;
    public void SetTimeStop(bool value) => timeStop = value;

    // 瞬間移動
    public bool IsTeleportEnabled() => teleport;
    public void SetTeleport(bool value) => teleport = value;

    // 時限強化モード
    public bool IsTimedPowerUpModeEnabled() => timedPowerUpMode;
    public void SetTimedPowerUpMode(bool value) => timedPowerUpMode = value;

    // ソードビーム
    public bool IsSwordBeamEnabled() => swordBeam;
    public void SetSwordBeam(bool value) => swordBeam = value;

    // 復活
    public bool IsResurrectionEnabled() => resurrection;
    public void SetResurrection(bool value) => resurrection = value;

    // バリア
    public bool IsBarrierEnabled() => barrier;
    public void SetBarrier(bool value) => barrier = value;

    // 一撃必殺
    public bool IsOneHitKillEnabled() => oneHitKill;
    public void SetOneHitKill(bool value) => oneHitKill = value;

    // 多重攻撃
    public bool IsMultiAttackEnabled() => multiAttack;
    public void SetMultiAttack(bool value) => multiAttack = value;

    // 防御貫通
    public bool IsDefensePenetrationEnabled() => defensePenetration;
    public void SetDefensePenetration(bool value) => defensePenetration = value;

    // 復活回数
    public int GetResurrectionTime() => resurrectionTime;
    public void SetResurrectionTime(int value) => resurrectionTime = Mathf.Max(0, value);

    // 攻撃範囲を取得
    public float GetAttackRange() => attackRange;
    // 攻撃範囲を設定
    public void SetAttackRange(float value) => attackRange = Mathf.Max(0f, value);

    // 回避率を取得
    public float GetEvasionRate() => evasionRate;
    public void SetEvasionRate(float value) => evasionRate = Mathf.Clamp01(value);

    // クリティカル率
    public float GetCriticalRate() => criticalRate;
    public void SetCriticalRate(float value) => criticalRate = Mathf.Clamp01(value);

    /// クリティカルダメージ倍率
    public float GetCriticalDamage() => criticalDamage;
    public void SetCriticalDamage(float value) => criticalDamage = Mathf.Max(1f, value);

    // 弾節約確率
    public float GetAmmoEcho() => ammoEcho;
    public void SetAmmoEcho(float value) => ammoEcho = Mathf.Clamp01(value);

    // 弾貫通回数
    public int GetAmmoPenetration() => ammoPenetration;
    public void SetAmmoPenetration(int value) => ammoPenetration = Mathf.Max(0, value);

    // バリアのHP
    public int GetBarrierHP() => barrierHP;
    public void SetBarrierHP(int value) => barrierHP = Mathf.Max(0, value);

    //防御力ダウン状態
    public bool GetIsDefenseReductionFlag() => isDefenseReduction;
    public void SetIsDefenseReductionFlag(bool value) => isDefenseReduction = value;

    //攻撃力ダウン
    public bool GetIsAttackReductionFlag() => isAttackReduction;
    public void SetIsAttackReductionFlag(bool value) => isAttackReduction = value;

    //減速
    public bool GetIsSlowEffectFlag() => isSlowEffect;
    public void SetIsSlowEffectFlag(bool value) => isSlowEffect = value;

    //血状態
    public bool GetIsBleedingEffectFlag() => isBleedingEffect;
    public void SetIsBleedingEffectFlag(bool value) => isBleedingEffect = value;

    // スタン
    public bool GetIsStunFlag() => isStun;
    public void SetIsStunFlag(bool value) => isStun = value;


    // 下記は例：Base用のGetter（実際は初期化時の値を記憶して返すなどの工夫が必要）
    public int GetBaseHpMax() { /* 初期値を返す処理をここに */ return 5; }
    public int GetBaseAttackPower() { /*初期値*/ return 3; }
    public float GetBaseCriticalRate() { return 0.1f; }
    public float GetBaseCriticalDamage() { return 2.0f; }
    public float GetBaseMoveSpeed() { return 1.0f; }
    public float GetBaseAttackSpeed() { return 1.0f; }
    public float GetBaseAttackRange() { return 1.0f; }
    public float GetBaseEvasionRate() { return 0.05f; }

    public int GetBaseAmmoCapacity() { return 0; }
    public int GetBaseAmmoRecovery() { return 0; }
    public float GetBaseAmmoEcho() { return 0f; }
    public int GetBaseAmmoPenetration() { return 0; }
    public int GetBaseResurrectionTime() { return 1; }

    // bool系Base値はすべてfalse(またはデフォルト)とする想定
    public bool GetBaseHpAutoRecovery() { return false; }
    public bool GetBaseExplosion() { return false; }
    public bool GetBaseTimeStop() { return false; }
    public bool GetBaseTeleport() { return false; }
    public bool GetBaseTimedPowerUpMode() { return false; }
    public bool GetBaseSwordBeam() { return false; }
    public bool GetBaseResurrection() { return false; }
    public bool GetBaseBarrier() { return false; }
    public bool GetBaseOneHitKill() { return false; }
    public bool GetBaseMultiAttack() { return false; }
    public bool GetBaseDefensePenetration() { return false; }

    public bool GetBaseIsDefenseReduction() { return false; }
    public bool GetBaseIsAttackReduction() { return false; }
    public bool GetBaseIsSlowEffect() { return false; }
    public bool GetBaseIsBleedingEffect() { return false; }
    public bool GetBaseIsStun() { return false; }




    // ===== その他のメソッド =====

    // 弾薬を回復
    public void RecoverAmmo()
    {
        ammoCapacity += ammoRecovery;
        Debug.Log($"弾薬を回復：現在の弾薬は{ammoCapacity}");
    }

    // 敵を倒したときの処理
    public void OnEnemyDefeated()
    {
        RecoverAmmo();
        // その他の処理
    }
    public int ReturnTakeDamage(int damage)
    {
        // バリアが有効な場合、ダメージを受けない
        if (barrier)
        {
            Debug.Log("バリアによってダメージを無効化");
            return 0;
        }

        // 回避判定
        if (Random.value < evasionRate)
        {
            Debug.Log("攻撃を回避した");
            return 0;
        }

        // 防御貫通が有効な場合、防御力を無視（防御力は常に0なので影響なし）
        int actualDamage = damage;

        // ダメージ適用
        hpNow -= actualDamage;
        Debug.Log($"プレイヤーは{actualDamage}のダメージを受けた（残りHP: {hpNow}/{hpMax}）");

        if (IsDead())
        {
            if (resurrection && resurrectionTime > 0)
            {
                Resurrection();
            }
            else
            {
                OnDeath();
            }
        }
        return actualDamage;
    }


    // =====ステータスを表示=====
    public  string ShowPlayerStatus()
    {
        string baseStatus = base.GetBaseStatus(); // 基本ステータスを取得

        // プレイヤー専用のステータスを追加
        string playerStatus =
            $"Ammo Capacity: {ammoCapacity}\n" +
            "";

        string fullStatus = baseStatus + "\n" + playerStatus;

        Debug.Log($"PlayerStatus:{fullStatus}");
        return fullStatus;
    }
    public Dictionary<string, bool> GetAllAbilitiesStatus()
    {
        // 全てのboolフラグをまとめて返す例
        Dictionary<string, bool> abilities = new Dictionary<string, bool>()
        {
            { "HpRecovery", hpRecovery },
            { "Explosion", explosion },
            { "TimeStop", timeStop },
            { "Teleport", teleport },
            { "TimedPowerUpMode", timedPowerUpMode },
            { "SwordBeam", swordBeam },
            { "Resurrection", resurrection },
            { "Barrier", barrier },
            { "OneHitKill", oneHitKill },
            { "MultiAttack", multiAttack },
            { "DefensePenetration", defensePenetration },
            { "IsDefenseReduction", isDefenseReduction },
            { "IsAttackReduction", isAttackReduction },
            { "IsSlowEffect", isSlowEffect },
            { "IsBleedingEffect", isBleedingEffect },
            { "IsStun", isStun }
        };

        return abilities;
    }
    // アイテム取得時に呼ぶメソッドの例
    public void OnItemCollected(ItemData item)
    {
        abilityManager.AddItem(item);
    }

    public void OnItemRemoved(ItemData item)
    {
        abilityManager.RemoveItem(item);
    }
}
