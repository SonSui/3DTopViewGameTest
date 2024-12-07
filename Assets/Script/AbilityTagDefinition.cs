using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAbilityTag", menuName = "AbilityTag")]
public class AbilityTagDefinition : ScriptableObject
{
    public string tagName;              // タグの名前
    public List<int> thresholds;        // 新しい能力をアンロックが必要なタグの数量
    public List<TagEffect> effects;     // タグ数について能力変化
}