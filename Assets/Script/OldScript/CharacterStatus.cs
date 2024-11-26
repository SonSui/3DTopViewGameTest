using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
/*    [SerializeField]
    // 基本属性
    int hpNow;                 // 当前血量
    int hpMax;
    int attackNow;
    int attackPower;        // 攻撃力
    int defenseNow;
    int defense;            // 防御力
    float criticalRate;     // クリティカル率
    float criticalDamage;   // クリティカルダメージ
    float moveSpeedNow;
    float moveSpeed;        // 移動速度1.0はベース
    float attackSpeed;      // 攻撃速度1.0はベース
    
    float attackRange;      // 攻撃範囲1.0はベース
    float evasionRate;      // 回避率

    // 状态效果
    
    float defenseReduction;                 // 防御力ダウンの時間
    const float defenseReductionRate = 0.5f;// 防御力ダウン率
    float attackReduction;                  // 攻撃力ダウン
    const float attackReductionRate = 0.5f; // 攻撃力ダウン率
    float slowEffect;                       // 減速
    const float slowEffectRate = 0.5f;      // 減速率
    float bleedingEffect;                   // 流血
    const float bleedingEffectRate = 0.05f; // 流血率
    float stun;                             // スタン



    // 特殊效果プレイヤーのみ
    *//*
         int ammoCapacity;       // 弾量
        int ammoRecovery;       // 弾回復
        float hpAutoRecovery;   // HP自動的に回復
        bool explosion;         // 爆発
        bool timeStop;          // 時間停止
        bool teleport;          // 瞬間移動
        bool timedPowerUpMode;  // 時限強化モード
        bool swordBeam;         // ソードビーム
        bool resurrection;      // 復活
        bool barrier;           // バリア
        bool oneHitKill;        // 一撃必殺
        bool multiAttack;       // 多重攻撃
        bool defensePenetration;// 防御貫通
    *//*


    public CharacterStatus(
        int hp_ = 0,
        int attack_ = 0,
        int defence_ = 0,
        float moveSpeed_ = 0.0f,
        float attackSpeed_ = 0.0f,
        float attackRange_ = 0.0f,
        int ammoCapacity_ = 0,
        float evasionRate_ = 0.0f,
        float criticalRate_ = 0.0f,
        float criticalDmg_ = 0.0f
        )
    {
        this.hp = hp_;
        this.attackPower = attack_;
        this.defense = defence_;
        this.movementSpeed = moveSpeed_;
        this.attackSpeed = attackSpeed_;
        this.attackRange = attackRange_;
        this.ammoCapacity = ammoCapacity_;
        this.evasionRate = evasionRate_;
        this.criticalRate = criticalRate_;
        this.criticalDamage = criticalDmg_;

        this.defenseReduction = 0f;
        this.attackReduction = 0f;
        this.slowEffect = 0f;
        this.bleedingEffect = 0f;
        this.stun = 0f;

        this.preDmg = 0;
        this.preDot = 0;
        this.dotTime = 0;
        this.isPreCrit = false;
        

    }
    public void SetBaseStatus(
        int hp_ = 0,
        int attack_ = 0,
        int defence_ = 0,
        float moveSpeed_ = 1.0f,
        float attackSpeed_ = 1.0f,
        float attackRange_ = 1.0f,
        int ammoCapacity_ = 0,
        float evasionRate_ = 0.0f,
        float criticalRate_ = 0.0f,
        float criticalDmg_ = 1.5f
        )
    {
        this.hp = hp_;
        this.attackPower = attack_;
        this.defense = defence_;
        this.movementSpeed = moveSpeed_;
        this.attackSpeed = attackSpeed_;
        this.attackRange = attackRange_;
        this.ammoCapacity = ammoCapacity_;
        this.evasionRate = evasionRate_;
        this.criticalRate = criticalRate_;
        this.criticalDamage = criticalDmg_;
    }
    public void SetBuffStatus(
            float defenseReduction_ = 0f,
            float attackReduction_ = 0f,
            float slowEffect_ = 0f,
            float bleedingEffect_ = 0f,
            float stun_ = 0f
        )
    {
        this.defenseReduction = defenseReduction_;
        this.attackReduction = attackReduction_;
        this.slowEffect = slowEffect_;
        this.bleedingEffect = bleedingEffect_;
        this.stun = stun_;


    }
    public void SetFullStatus(
            int hp_ = 0,
            int attack_ = 0,
            int defence_ = 0,
            float moveSpeed_ = 0.0f,
            float attackSpeed_ = 0.0f,
            float attackRange_ = 0.0f,
            int ammoCapacity_ = 0,
            float evasionRate_ = 0.0f,
            float criticalRate_ = 0.0f,
            float criticalDmg_ = 0.0f,

            float defenseReduction_ = 0f,
            float attackReduction_ = 0f,
            float slowEffect_ = 0f,
            float bleedingEffect_ = 0f,
            float stun_ = 0f
        )
    {
        this.SetBaseStatus(hp_, attack_, defence_, moveSpeed_, attackSpeed_, attackRange_, ammoCapacity_, evasionRate_, criticalRate_, criticalDmg_);
        this.SetBuffStatus(defenseReduction_, attackReduction_, slowEffect_, bleedingEffect_,stun_) ;
    }

    public void ResetState()
    {
        this.SetBaseStatus();
        this.SetBuffStatus();
    }

    public string ShowStatus()
    {
        string status =
            $"HP: {this.hp}\n" +
            $"Attack Power: {this.attackPower}\n" +
            $"Defense: {this.defense}\n" +
            $"Critical Rate: {this.criticalRate * 100:F2}%\n" +
            $"Critical Damage: {this.criticalDamage * 100:F2}%\n" +
            $"Movement Speed: {this.movementSpeed:F2}\n" +
            $"Attack Speed: {this.attackSpeed:F2}\n" +
            $"Ammo Capacity: {this.ammoCapacity}\n" +
            $"Attack Range: {this.attackRange:F2}\n" +
            $"Evasion Rate: {this.evasionRate * 100:F2}%\n" +
            $"Defense Reduction: {this.defenseReduction:F2} (Rate: {defenseReductionRate * 100:F2}%)\n" +
            $"Attack Reduction: {this.attackReduction:F2} (Rate: {attackReductionRate * 100:F2}%)\n" +
            $"Slow Effect: {this.slowEffect:F2} (Rate: {slowEffectRate * 100:F2}%)\n" +
            $"Bleeding Effect: {this.bleedingEffect:F2} (Rate: {bleedingEffectRate * 100:F2}%)\n" +
            $"Stun: {this.stun:F2}";

        Debug.Log(status);
        return status;
    }

    public bool IsDead()
    {
        return this.hp <= 0;
    }

    public bool TakeDamage(int damage,bool isdefensePenetration = false)
    {
        if(isdefensePenetration)
        {
            this.preDmg = damage;
            this.isPreCrit = true;
            this.hp -= this.preDmg;
            
        }
        else
        {
            int def;
            if(this.defenseReduction>0)
            {
                def = (int)(this.defense * (1 - defenseReductionRate));
            }
            else
            {
                def = this.defense;
            }
            this.preDmg = ((def - damage) > 0 ? def - damage : 1);
            this.isPreCrit = false;
            this.hp -= this.preDmg;
        }
        return this.IsDead();
    }
    public static CharacterStatus operator +(CharacterStatus a, CharacterStatus b)
    {
        return new CharacterStatus(
            hp_: a.hp + b.hp,                              // HP（体力）の加算
            attack_: a.attackPower + b.attackPower,        // 攻撃力の加算
            defence_: a.defense + b.defense,               // 防御力の加算
            moveSpeed_: a.movementSpeed + b.movementSpeed, // 移動速度の加算
            attackSpeed_: a.attackSpeed + b.attackSpeed,   // 攻撃速度の加算
            attackRange_: a.attackRange + b.attackRange,   // 攻撃範囲の加算
            ammoCapacity_: a.ammoCapacity + b.ammoCapacity,// 弾量の加算
            evasionRate_: a.evasionRate + b.evasionRate,   // 回避率の加算
            criticalRate_: a.criticalRate + b.criticalRate,// クリティカル率の加算
            criticalDmg_: a.criticalDamage + b.criticalDamage // クリティカルダメージの加算
        )
        {
            // 状态效果の加算
            defenseReduction = a.defenseReduction + b.defenseReduction, // 防御力ダウンの加算
            attackReduction = a.attackReduction + b.attackReduction,    // 攻撃力ダウンの加算
            slowEffect = a.slowEffect + b.slowEffect,                   // 減速の加算
            bleedingEffect = a.bleedingEffect + b.bleedingEffect,       // 流血の加算
            stun = a.stun + b.stun,                                     // スタンの加算

            // 表示用変数は左側（`a`）の値を保持
            preDmg = a.preDmg,                      // 直前のダメージは左側の値
            isPreCrit = a.isPreCrit,                // 直前のクリティカル判定は左側の値
            preDot = a.preDot                       // 直前の継続ダメージ（DoT）は左側の値
        };
    }


    public void CopyTo(CharacterStatus target)
    {
        if (target == null) return;// 対象が null の場合、処理を終了する

        // 基本属性をコピー
        target.hp = this.hp;                            // 体力をコピー
        target.attackPower = this.attackPower;          // 攻撃力をコピー
        target.defense = this.defense;                  // 防御力をコピー
        target.criticalRate = this.criticalRate;        // クリティカル率をコピー
        target.criticalDamage = this.criticalDamage;    // クリティカルダメージをコピー
        target.movementSpeed = this.movementSpeed;      // 移動速度をコピー
        target.attackSpeed = this.attackSpeed;          // 攻撃速度をコピー
        target.ammoCapacity = this.ammoCapacity;        // 弾量をコピー
        target.attackRange = this.attackRange;          // 攻撃範囲をコピー
        target.evasionRate = this.evasionRate;          // 回避率をコピー

        // 状態効果をコピー
        target.defenseReduction = this.defenseReduction;// 防御力ダウンをコピー
        target.attackReduction = this.attackReduction;  // 攻撃力ダウンをコピー
        target.slowEffect = this.slowEffect;            // 減速をコピー
        target.bleedingEffect = this.bleedingEffect;    // 流血をコピー
        target.stun = this.stun;                        // スタンをコピー

        // 表示用変数をコピー
        target.preDmg = this.preDmg;                    // 直前のダメージをコピー
        target.isPreCrit = this.isPreCrit;              // 直前のクリティカルかどうかをコピー
        target.preDot = this.preDot;                    // 直前の継続ダメージ（DoT）をコピー
    }
    public bool CaculateBuffTime(float timeRate = 1.0f) //timeRateは計算速度,dot受けるとtrueを返信

    {
        this.defenseReduction -= Time.deltaTime * timeRate;
        this.attackReduction -= Time.deltaTime * timeRate;
        this.slowEffect -= Time.deltaTime * timeRate;
        this.stun -= Time.deltaTime * timeRate;
        if(this.defenseReduction < 0) this.defenseReduction = 0;
        if(this.attackReduction < 0) this.attackReduction = 0;
        if(this.slowEffect < 0) this.slowEffect = 0;
        if(this.stun < 0) this.stun = 0;

        if (this.bleedingEffect > 0)
        {
            this.bleedingEffect -= Time.deltaTime * timeRate;
            if(this.bleedingEffect < 0)this.bleedingEffect = 0;
            this.dotTime -= Time.deltaTime * timeRate;

            if (this.dotTime <= 0)
            {
                this.dotTime = 1.0f;
                return true;
            }
        }
        return false;
    }
    // HP（体力）
    public int GetHp() => this.hp; // 体力を取得
    public void SetHp(int value) => this.hp = value; // 体力を設定

    // 攻撃力
    public int GetAttackPower() => this.attackPower; // 攻撃力を取得
    public int GetCaculatedAttackPower()
    {
        if(this.attackReduction>0)
        {
            return (int)(this.attackPower * (1 - attackReductionRate));
        }
        else return this.attackPower;
    }
    public void SetAttackPower(int value) => this.attackPower = value; // 攻撃力を設定

    // 防御力
    public int GetDefense() => this.defense; // 防御力を取得
    public int GetCaculatedDefense()
    {
        if (this.defenseReduction > 0)
        {
            return (int)(this.defense * (1 - defenseReductionRate));
        }
        else return this.defense;
    }

    public void SetDefense(int value) => this.defense = value; // 防御力を設定

    // クリティカル率
    public float GetCriticalRate() => this.criticalRate; // クリティカル率を取得
    public void SetCriticalRate(float value) => this.criticalRate = value; // クリティカル率を設定

    // クリティカルダメージ
    public float GetCriticalDamage() => this.criticalDamage; // クリティカルダメージを取得
    public void SetCriticalDamage(float value) => this.criticalDamage = value; // クリティカルダメージを設定

    // 移動速度
    public float GetMovementSpeed() => this.movementSpeed; // 移動速度を取得
    public float GetCaculatedMovementSpeed()
    {
        if (this.slowEffect > 0)
        {
            return (this.movementSpeed * (1 - slowEffectRate));
        }
        else return this.movementSpeed;
    }
    public void SetMovementSpeed(float value) => this.movementSpeed = value; // 移動速度を設定

    // 攻撃速度
    public float GetAttackSpeed() => this.attackSpeed; // 攻撃速度を取得
    public float GetCaculatedAttackSpeed()
    {
        if (this.attackReduction > 0)
        {
            return (this.attackSpeed * (1 - attackReductionRate));
        }
        else return this.attackSpeed;
    }
    public void SetAttackSpeed(float value) => this.attackSpeed = value; // 攻撃速度を設定

    // 弾量
    public int GetAmmoCapacity() => this.ammoCapacity; // 弾量を取得
    public void SetAmmoCapacity(int value) => this.ammoCapacity = value; // 弾量を設定

    // 攻撃範囲
    public float GetAttackRange() => this.attackRange; // 攻撃範囲を取得
    public void SetAttackRange(float value) => this.attackRange = value; // 攻撃範囲を設定

    // 回避率
    public float GetEvasionRate() => this.evasionRate; // 回避率を取得
    public void SetEvasionRate(float value) => this.evasionRate = value; // 回避率を設定


    // 防御力ダウンを設定
    public void SetDefenseReduction(float value) => this.defenseReduction = value;

    // 攻撃力ダウンを設定
    public void SetAttackReduction(float value) => this.attackReduction = value;

    // 減速を設定
    public void SetSlowEffect(float value) => this.slowEffect = value;

    // 流血を設定
    public void SetBleedingEffect(float value) => this.bleedingEffect = value;

    // スタンを設定
    public void SetStun(float value) => this.stun = value;

    // 防御力ダウン中か確認
    public bool IsDefenseReduced() => this.defenseReduction > 0;

    // 攻撃力ダウン中か確認
    public bool IsAttackReduced() => this.attackReduction > 0;

    // 減速中か確認
    public bool IsSlowed() => this.slowEffect > 0;

    // 流血中か確認
    public bool IsBleeding() => this.bleedingEffect > 0;

    // スタン中か確認
    public bool IsStunned() => this.stun > 0;*/
}
