using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵キャラクタークラス：敵のステータス管理
public class EnemyStatus : BaseCharacterStatus
{
    // ===== 敵専用の属性 =====

    // 敵の種類（通常、エリート、ボスなど）
    private string enemyType;

    // ドロップアイテムのリスト
    private List<Item> dropItems;

    // 特殊能力の有効化フラグ
    private bool hasShield;        // シールドを持っているか
    private int shieldDurability; // シールドの耐久値

    // ===== コンストラクタ =====

    // 初期化（基底クラスのコンストラクタを呼び出す）
    public EnemyStatus(
        string name ="Enemy",
        int hpMax = 10,
        int attackPower = 1,
        int defense = 1,
        string enemyType = "Normal",
        bool hasShield = false,
        int shieldDurability = 0,

        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f
        

    ) : base(
        name: name,
        hpMax: hpMax,
        attackPower: attackPower,
        defense: defense,
        moveSpeed: moveSpeed,
        attackSpeed: attackSpeed
    )
    {
        // 敵専用の属性を初期化
        this.enemyType = enemyType;
        this.dropItems = new List<Item>();

        this.hasShield = hasShield;
        this.shieldDurability = shieldDurability;

    }

    // ===== メソッド =====

    // ダメージを受ける処理
    public override int TakeDamage(int damage, bool isDefensePenetration = false)
    {
        if(damage<=0) return 0;

        int actualDamage = damage;

        if (hasShield)
        {
            shieldDurability -= damage;
            if (shieldDurability <= 0)
            {
                actualDamage = -shieldDurability;
                shieldDurability = 0;
                hasShield = false;
            }
            else
            {
                actualDamage = -damage;
            }
        }

        if (!isDefensePenetration)// 防御貫通が有効な場合、防御力を無視
        {
            if(actualDamage>0)actualDamage = Mathf.Max(1, actualDamage - defenseNow);
        }
        


        // ダメージ適用
        if (actualDamage > 0)
        {
            hpNow -= actualDamage;
            Debug.Log($"{name} は{actualDamage} のダメージを受けた（残りHP: {hpNow}/{hpMax}）");
        }



        if (IsDead())
        {
            OnDeath();
        }
        return actualDamage;
    }


    // ステータスを更新（毎フレーム呼び出す）
    public override int UpdateStatus(float deltaTime, float timeRate = 1.0f)
    {
        return base.UpdateStatus(deltaTime, timeRate);
    }

    // 死亡時の処理
    protected override void OnDeath()
    {
        // 敵の死亡処理
        Debug.Log($"{this.name} は倒された");
        DropItems();

    }


    // ドロップアイテムの生成
    private void DropItems()
    {
        // ドロップアイテムを生成する処理
        foreach (var item in dropItems)
        {
            // アイテムを生成?
            
            //Debug.Log($"{item.itemName} をドロップした");
        }
    }

    // アイテムをドロップリストに追加
    public void AddDropItem(Item item)
    {
        dropItems.Add(item);
    }

    // ===== Getter/Setter =====

    // 敵の種類
    public string GetEnemyType() => enemyType;
    public void SetEnemyType(string value) => enemyType = value;


    // シールド関連
    public bool HasShield() => hasShield;
    public void SetShield(bool value, int durability = 0)
    {
        hasShield = value;
        shieldDurability = durability;
    }

    public int GetShieldDurability() => shieldDurability;
    public void SetShieldDurability(int value) => shieldDurability = Mathf.Max(0, value);



    // ===== その他のメソッド =====

    // 特殊能力を適用
    public void ApplySpecialAbility(string abilityName)
    {
        switch (abilityName)
        {
            case "Shield":
                SetShield(true, 50); // シールド耐久値50で有効化
                break;
            
            default:
                Debug.Log($"未知の特殊能力: {abilityName}");
                break;
        }
    }
}
