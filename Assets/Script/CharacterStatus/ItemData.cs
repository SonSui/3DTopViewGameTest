using UnityEngine;
using System.Collections.Generic;


// 装備データクラス
// 各装備の基本属性とタグを管理するScriptableObject

[CreateAssetMenu(fileName = "NewItemData", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("装備基本情報")]
    [Tooltip("装備の名前")]
    public string itemName; // 装備名
    [Tooltip("装備のレアリティ(0~3)")]
    public int rare; // 装備のレアリティ(0~3)
    [Header("基本属性")]
    [Tooltip("最大HPの増加量")]
    public int hpMax; // 最大HPの増加量
    [Tooltip("攻撃力の増加量")]
    public int attackPower; // 攻撃力の増加量
    [Tooltip("クリティカル率の増加量")]
    public float criticalRate; // クリティカル率の増加量
    [Tooltip("クリティカルダメージ倍率の増加量")]
    public float criticalDamage; // クリティカルダメージ倍率
    [Tooltip("移動速度の増加量")]
    public float moveSpeed; // 移動速度の増加量
    [Tooltip("攻撃速度の増加量")]
    public float attackSpeed; // 攻撃速度の増加量
    [Tooltip("攻撃範囲の増加量")]
    public float attackRange; // 攻撃範囲の増加量
    [Tooltip("回避率の増加量")]
    public float evasionRate; // 回避率の増加量
    [Tooltip("弾薬容量の増加量")]
    public int ammoCapacity; // 弾薬容量の増加量

    [Header("タグ")]
    [Tooltip("この装備に関連付けられている能力タグ")]
    public List<AbilityTagDefinition> tags = new List<AbilityTagDefinition>(); // 関連する能力タグのリスト

    public static ItemData CreateInstance(
        string name_ = "Chip",
        int rare_ = 0,
        int hpMax_ = 0,
        int attackPower_ = 0,
        float criticalRate_ = 0f,
        float criticalDamage_ = 0f,
        float moveSpeed_ = 0f,
        float attackSpeed_ = 0f,
        float attackRange_ = 0f,
        float evasionRate_ = 0f,
        int ammoCapacity_ = 0,
        List<AbilityTagDefinition> tags_ = null
    )
    {
        // ScriptableObjectのインスタンスを作成
        ItemData instance = ScriptableObject.CreateInstance<ItemData>();
        instance.itemName = name_;
        instance.rare = rare_;
        instance.hpMax = hpMax_;
        instance.attackPower = attackPower_;
        instance.criticalRate = criticalRate_;
        instance.criticalDamage = criticalDamage_;
        instance.moveSpeed = moveSpeed_;
        instance.attackSpeed = attackSpeed_;
        instance.attackRange = attackRange_;
        instance.evasionRate = evasionRate_;
        instance.ammoCapacity = ammoCapacity_;
        instance.tags = tags_ ?? new List<AbilityTagDefinition>();
        return instance;
    }
}