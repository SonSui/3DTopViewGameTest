using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 基底クラス：キャラクターステータス
public abstract class BaseCharacterStatus : MonoBehaviour
{
    // ===== 基本属性 =====

    // HP（体力）
    protected int hpNow;     // 現在のHP
    protected int hpMax;     // 最大HP

    // 攻撃力
    protected int attackNow;    // 現在の攻撃力（バフ・デバフ適用後）
    protected int attackPower;  // 基本攻撃力

    // 防御力
    protected int defenseNow;   // 現在の防御力（バフ・デバフ適用後）
    protected int defense;      // 基本防御力

    // クリティカル関連
    protected float criticalRate;    // クリティカル率
    protected float criticalDamage;  // クリティカルダメージ倍率

    // 移動速度
    protected float moveSpeedNow; // 現在の移動速度
    protected float moveSpeed;    // 基本移動速度（1.0が基準）

    // 攻撃速度
    protected float attackSpeed;  // 攻撃速度（1.0が基準）

    // 攻撃範囲
    protected float attackRange;  // 攻撃範囲（1.0が基準）

    // 回避率
    protected float evasionRate;  // 回避率

    // ===== 状態効果 =====

    // 防御力ダウン
    protected float defenseReduction;                   // 防御力ダウンの残り時間
    protected const float defenseReductionRate = 0.5f;  // 防御力ダウン率

    // 攻撃力ダウン
    protected float attackReduction;                    // 攻撃力ダウンの残り時間
    protected const float attackReductionRate = 0.5f;   // 攻撃力ダウン率

    // 減速
    protected float slowEffect;                         // 減速の残り時間
    protected const float slowEffectRate = 0.5f;        // 減速率

    // 流血
    protected float bleedingEffect;                     // 流血の残り時間
    protected const float bleedingEffectRate = 0.05f;   // 流血ダメージ率
    protected float bleedingInterval;                   // 流血ダメージの間隔
    protected float bleedingTimer;                      // 流血ダメージのタイマー

    // スタン
    protected float stun;                               // スタンの残り時間

    // ===== コンストラクタ =====

    // 初期化（パラメータ付き、デフォルト値あり）
    public BaseCharacterStatus(
        int hpMax = 5,
        int attackPower = 2,
        int defense = 1,
        float criticalRate = 0.0f,
        float criticalDamage = 2.0f,
        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f,
        float attackRange = 1.0f,
        float evasionRate = 0.05f
    )
    {
        // 基本属性の初期化
        this.hpMax = hpMax;
        this.hpNow = this.hpMax;

        this.attackPower = attackPower;
        this.attackNow = this.attackPower;

        this.defense = defense;
        this.defenseNow = this.defense;

        this.criticalRate = criticalRate;
        this.criticalDamage = criticalDamage;

        this.moveSpeed = moveSpeed;
        this.moveSpeedNow = this.moveSpeed;

        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;

        this.evasionRate = evasionRate;

        // 状態効果の初期化
        this.defenseReduction = 0f;
        this.attackReduction = 0f;
        this.slowEffect = 0f;
        this.bleedingEffect = 0f;
        this.bleedingInterval = 1.0f; // 流血ダメージの間隔を初期化
        this.bleedingTimer = this.bleedingInterval;
        this.stun = 0f;
    }

    // ===== メソッド =====

    // ダメージを受ける処理（抽象メソッド）
    public abstract void TakeDamage(int damage, bool isDefensePenetration = false);

    // ステータスを更新（毎フレーム呼び出す）
    public virtual void UpdateStatus(float deltaTime, float timeRate = 1.0f)
    {
        // バフ計算を一時停止する場合は、timeRateを0に設定
        float adjustedDeltaTime = deltaTime * timeRate;

        // 状態効果の時間を更新
        UpdateStateEffects(adjustedDeltaTime);

        // 現在のステータスを再計算
        RecalculateStats();
    }

    // 状態効果の時間を更新
    protected void UpdateStateEffects(float deltaTime)
    {
        // 防御力ダウン時間の更新
        if (defenseReduction > 0)
        {
            defenseReduction = Mathf.Max(0, defenseReduction - deltaTime);
        }

        // 攻撃力ダウン時間の更新
        if (attackReduction > 0)
        {
            attackReduction = Mathf.Max(0, attackReduction - deltaTime);
        }

        // 減速時間の更新
        if (slowEffect > 0)
        {
            slowEffect = Mathf.Max(0, slowEffect - deltaTime);
        }

        // 流血時間の更新
        if (bleedingEffect > 0)
        {
            bleedingEffect = Mathf.Max(0, bleedingEffect - deltaTime);

            // 流血ダメージのタイマーを更新
            bleedingTimer -= deltaTime;
            if (bleedingTimer <= 0)
            {
                // 流血ダメージを適用
                ApplyBleedingDamage();
                bleedingTimer = bleedingInterval;
            }
        }
        else
        {
            bleedingTimer = bleedingInterval; // 流血効果が終了したらタイマーをリセット
        }

        // スタン時間の更新
        if (stun > 0)
        {
            stun = Mathf.Max(0, stun - deltaTime);
        }
    }

    // ステータスの再計算
    protected void RecalculateStats()
    {
        // 防御力の再計算
        defenseNow = defense;
        if (defenseReduction > 0)
        {
            defenseNow = (int)(defense * (1 - defenseReductionRate));
        }

        // 攻撃力の再計算
        attackNow = attackPower;
        if (attackReduction > 0)
        {
            attackNow = (int)(attackPower * (1 - attackReductionRate));
        }

        // 移動速度の再計算
        moveSpeedNow = moveSpeed;
        if (slowEffect > 0)
        {
            moveSpeedNow = moveSpeed * (1 - slowEffectRate);
        }
    }

    // 流血ダメージを適用
    protected void ApplyBleedingDamage()
    {
        int bleedingDamage = (int)(hpMax * bleedingEffectRate);
        hpNow -= bleedingDamage;

        // ダメージログ
        Debug.Log($"{gameObject.name}は流血で{bleedingDamage}のダメージを受けた");

        if (hpNow <= 0)
        {
            hpNow = 0;
            OnDeath();
        }
    }

    // 死亡時の処理
    protected virtual void OnDeath()
    {
        // デフォルトの死亡処理
        Debug.Log($"{gameObject.name}は倒れた");
    }

    // HPがゼロかどうかを確認
    public bool IsDead()
    {
        return hpNow <= 0;
    }

    // ===== Getter/Setter =====

    // HP
    public int GetHpNow() => hpNow;
    public int GetHpMax() => hpMax;
    public void SetHpNow(int value) => hpNow = Mathf.Clamp(value, 0, hpMax);
    public void SetHpMax(int value)
    {
        hpMax = Mathf.Max(1, value);
        hpNow = Mathf.Min(hpNow, hpMax); // 最大HPが減少した場合、現在のHPを調整
    }

    // 攻撃力
    public int GetAttackNow() => attackNow;
    public int GetAttackPower() => attackPower;
    public void SetAttackPower(int value) => attackPower = Mathf.Max(0, value);

    // 防御力
    public int GetDefenseNow() => defenseNow;
    public int GetDefense() => defense;
    public void SetDefense(int value) => defense = Mathf.Max(0, value);

    // クリティカル率
    public float GetCriticalRate() => criticalRate;
    public void SetCriticalRate(float value) => criticalRate = Mathf.Clamp01(value);

    // クリティカルダメージ
    public float GetCriticalDamage() => criticalDamage;
    public void SetCriticalDamage(float value) => criticalDamage = Mathf.Max(1.0f, value);

    // 移動速度
    public float GetMoveSpeedNow() => moveSpeedNow;
    public float GetMoveSpeed() => moveSpeed;
    public void SetMoveSpeed(float value) => moveSpeed = Mathf.Max(0f, value);

    // 攻撃速度
    public float GetAttackSpeed() => attackSpeed;
    public void SetAttackSpeed(float value) => attackSpeed = Mathf.Max(0f, value);

    // 攻撃範囲
    public float GetAttackRange() => attackRange;
    public void SetAttackRange(float value) => attackRange = Mathf.Max(0f, value);

    // 回避率
    public float GetEvasionRate() => evasionRate;
    public void SetEvasionRate(float value) => evasionRate = Mathf.Clamp01(value);

    // ===== 状態効果の適用 =====

    // 防御力ダウンを適用
    public void ApplyDefenseReduction(float duration)
    {
        defenseReduction = Mathf.Max(defenseReduction, duration);
    }

    // 攻撃力ダウンを適用
    public void ApplyAttackReduction(float duration)
    {
        attackReduction = Mathf.Max(attackReduction, duration);
    }

    // 減速を適用
    public void ApplySlowEffect(float duration)
    {
        slowEffect = Mathf.Max(slowEffect, duration);
    }

    // 流血を適用
    public void ApplyBleedingEffect(float duration)
    {
        bleedingEffect = Mathf.Max(bleedingEffect, duration);
    }

    // スタンを適用
    public void ApplyStun(float duration)
    {
        stun = Mathf.Max(stun, duration);
    }

    // ===== 状態効果の確認 =====

    // 防御力ダウン中か確認
    public bool IsDefenseReduced() => defenseReduction > 0;

    // 攻撃力ダウン中か確認
    public bool IsAttackReduced() => attackReduction > 0;

    // 減速中か確認
    public bool IsSlowed() => slowEffect > 0;

    // 流血中か確認
    public bool IsBleeding() => bleedingEffect > 0;

    // スタン中か確認
    public bool IsStunned() => stun > 0;
}
