using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤークラス：プレイヤーのステータス管理
public class PlayerStatus : BaseCharacterStatus
{
    // ===== プレイヤー専用の属性 =====

    // 弾量関連
    private int ammoCapacity;    // 弾量
    private int ammoRecovery;    // 敵を倒したときに回復する弾数（デフォルト0）

    // HP自動回復
    private float hpAutoRecovery;   // 一定時間ごとに回復するHP量（デフォルト0）
    private float hpRecoveryInterval; // HP回復の間隔
    private float hpRecoveryTimer;    // HP回復のタイマー

    // 特殊能力の有効化フラグ
    private bool explosion;           // 爆発
    private bool timeStop;            // 時間停止
    private bool teleport;            // 瞬間移動
    private bool timedPowerUpMode;    // 時限強化モード
    private bool swordBeam;           // ソードビーム
    private bool resurrection;        // 復活
    private int resurrectionTime;     // 復活可能な回数

    private bool barrier;             // バリア
    private bool oneHitKill;          // 一撃必殺
    private bool multiAttack;         // 多重攻撃
    private bool defensePenetration;  // 防御貫通

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
        float hpAutoRecovery = 0f,
        float hpRecoveryInterval = 5f,
        int resurrectionTime = 1,
        string name = "Player"
    ) : base(
        name: name,
        hpMax: hpMax,
        attackPower: attackPower,
        defense: 0, // プレイヤーの防御力は常に0
        criticalRate: criticalRate,
        criticalDamage: criticalDamage,
        moveSpeed: moveSpeed,
        attackSpeed: attackSpeed,
        attackRange: attackRange,
        evasionRate: evasionRate
    )
    {
        // プレイヤー専用の属性を初期化
        this.ammoCapacity = ammoCapacity;
        this.ammoRecovery = ammoRecovery;

        this.hpAutoRecovery = hpAutoRecovery;
        this.hpRecoveryInterval = hpRecoveryInterval;
        this.hpRecoveryTimer = this.hpRecoveryInterval;

        this.explosion = false;
        this.timeStop = false;
        this.teleport = false;
        this.timedPowerUpMode = false;
        this.swordBeam = false;
        this.resurrection = false;
        this.resurrectionTime = resurrectionTime;

        this.barrier = false;
        this.oneHitKill = false;
        this.multiAttack = false;
        this.defensePenetration = false;
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

        // HP自動回復
        if (hpAutoRecovery > 0)
        {
            hpRecoveryTimer -= adjustedDeltaTime;
            if (hpRecoveryTimer <= 0)
            {
                hpNow = Mathf.Min(hpNow + (int)hpAutoRecovery, hpMax);
                hpRecoveryTimer = hpRecoveryInterval;
                Debug.Log($"HPが自動回復：現在のHPは{hpNow}/{hpMax}");
            }
        }

        // 時限強化モードの処理（必要に応じて実装）
        if (timedPowerUpMode)
        {
            // 強化モードの処理
        }

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
    public float GetHpAutoRecovery() => hpAutoRecovery;
    public void SetHpAutoRecovery(float value) => hpAutoRecovery = Mathf.Max(0f, value);

    // HP回復間隔
    public float GetHpRecoveryInterval() => hpRecoveryInterval;
    public void SetHpRecoveryInterval(float value) => hpRecoveryInterval = Mathf.Max(0.1f, value); // 0.1秒以上

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
}
