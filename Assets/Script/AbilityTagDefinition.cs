using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAbilityTag", menuName = "AbilityTag")]
public class AbilityTagDefinition : ScriptableObject
{
    public string tagName;              // タグの名前
    public List<int> thresholds;        // タグ上限
    public List<AbilityEffect> effects; // タグ数について能力変化
}