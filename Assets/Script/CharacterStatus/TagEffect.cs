using UnityEngine;

[System.Serializable] 
public class TagEffect
{
    [Header("基本属性*8+1")]
    [SerializeField] public int hpMax;                  // 最大HP
    [SerializeField] public int attackPower;            // 基本攻撃力
    [SerializeField] public float criticalRate;         // クリティカル率
    [SerializeField] public float criticalDamage;       // クリティカルダメージ倍率
    [SerializeField] public float moveSpeed;            // 基本移動速度（1.0が基準）
    [SerializeField] public float attackSpeed;          // 攻撃速度（1.0が基準）
    [SerializeField] public float attackRange;          // 攻撃範囲（1.0が基準）
    [SerializeField] public float evasionRate;          // 回避率


    [Header("デバフ能力*5")]
    [SerializeField] public bool isDefenseReduction;    // 防御力ダウン
    [SerializeField] public bool isAttackReduction;     // 攻撃力ダウン
    [SerializeField] public bool isSlowEffect;          // 減速
    [SerializeField] public bool isBleedingEffect;      // 流血
    [SerializeField] public bool isStun;                // スタン

    [Header("射撃能力*5")]
    [SerializeField] public int ammoCapacity;           // 弾量
    [SerializeField] public int ammoRecovery;           // 敵を倒したときに回復する弾数
    [SerializeField] public float ammoEcho;             // 弾節約確率
    [SerializeField] public int ammoPenetration;        // 弾貫通
    [SerializeField] public int resurrectionTime;       // 復活可能な回数

    [Header("特殊能力*11")]
    [SerializeField] public bool hpRecovery;            // HP回復する
    [SerializeField] public bool explosion;             // 爆発
    [SerializeField] public bool timeStop;              // 時間停止
    [SerializeField] public bool teleport;              // 瞬間移動
    [SerializeField] public bool timedPowerUpMode;      // 時限強化モード
    [SerializeField] public bool swordBeam;             // ソードビーム
    [SerializeField] public bool resurrection;          // 復活    
    [SerializeField] public bool barrier;               // バリア
    [SerializeField] public bool oneHitKill;            // 一撃必殺
    [SerializeField] public bool multiAttack;           // 多重攻撃
    [SerializeField] public bool defensePenetration;    // 防御貫通
}