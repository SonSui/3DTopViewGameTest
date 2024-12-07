using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAbilityTag", menuName = "AbilityTag")]
public class AbilityTagDefinition : ScriptableObject
{
    public string tagName;              // �^�O�̖��O
    public List<int> thresholds;        // �V�����\�͂��A�����b�N���K�v�ȃ^�O�̐���
    public List<TagEffect> effects;     // �^�O���ɂ��Ĕ\�͕ω�
}