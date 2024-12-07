using UnityEngine;

[System.Serializable] 
public class TagEffect
{
    [SerializeField] private int hpMax;                  // 最大HP
    [SerializeField] private int attackPower;            // 基本攻撃力
    [SerializeField] private float criticalRate;         // クリティカル率
    [SerializeField] private float criticalDamage;       // クリティカルダメージ倍率
    [SerializeField] private float moveSpeed;            // 基本移動速度（1.0が基準）
    [SerializeField] private float attackSpeed;          // 攻撃速度（1.0が基準）
    [SerializeField] private float attackRange;          // 攻撃範囲（1.0が基準）
    [SerializeField] private float evasionRate;          // 回避率

    [SerializeField] private bool isDefenseReduction;    // 防御力ダウン
    [SerializeField] private bool isAttackReduction;     // 攻撃力ダウン
    [SerializeField] private bool isSlowEffect;          // 減速
    [SerializeField] private bool isBleedingEffect;      // 流血
    [SerializeField] private bool isStun;                // スタン

    [SerializeField] private int ammoCapacity;           // 弾量
    [SerializeField] private int ammoRecovery;           // 敵を倒したときに回復する弾数
    [SerializeField] private float ammoEcho;             // 弾節約確率
    [SerializeField] private int ammoPenetration;        // 弾貫通
    [SerializeField] private int resurrectionTime;       // 復活可能な回数

    [SerializeField] private bool hpRecovery;            // HP回復する
    [SerializeField] private bool explosion;             // 爆発
    [SerializeField] private bool timeStop;              // 時間停止
    [SerializeField] private bool teleport;              // 瞬間移動
    [SerializeField] private bool timedPowerUpMode;      // 時限強化モード
    [SerializeField] private bool swordBeam;             // ソードビーム
    [SerializeField] private bool resurrection;          // 復活    
    [SerializeField] private bool barrier;               // バリア
    [SerializeField] private bool oneHitKill;            // 一撃必殺
    [SerializeField] private bool multiAttack;           // 多重攻撃
    [SerializeField] private bool defensePenetration;    // 防御貫通
}