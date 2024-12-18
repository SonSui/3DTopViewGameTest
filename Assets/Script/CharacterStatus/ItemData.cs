using UnityEngine;
using System.Collections.Generic;


// �����f�[�^�N���X
// �e�����̊�{�����ƃ^�O���Ǘ�����ScriptableObject

[CreateAssetMenu(fileName = "NewItemData", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("������{���")]
    [Tooltip("�����̖��O")]
    public string itemName; // ������
    [Tooltip("�����̃��A���e�B(0~3)")]
    public int rare; // �����̃��A���e�B(0~3)
    [Header("��{����")]
    [Tooltip("�ő�HP�̑�����")]
    public int hpMax; // �ő�HP�̑�����
    [Tooltip("�U���͂̑�����")]
    public int attackPower; // �U���͂̑�����
    [Tooltip("�N���e�B�J�����̑�����")]
    public float criticalRate; // �N���e�B�J�����̑�����
    [Tooltip("�N���e�B�J���_���[�W�{���̑�����")]
    public float criticalDamage; // �N���e�B�J���_���[�W�{��
    [Tooltip("�ړ����x�̑�����")]
    public float moveSpeed; // �ړ����x�̑�����
    [Tooltip("�U�����x�̑�����")]
    public float attackSpeed; // �U�����x�̑�����
    [Tooltip("�U���͈͂̑�����")]
    public float attackRange; // �U���͈͂̑�����
    [Tooltip("��𗦂̑�����")]
    public float evasionRate; // ��𗦂̑�����
    [Tooltip("�e��e�ʂ̑�����")]
    public int ammoCapacity; // �e��e�ʂ̑�����

    [Header("�^�O")]
    [Tooltip("���̑����Ɋ֘A�t�����Ă���\�̓^�O")]
    public List<AbilityTagDefinition> tags = new List<AbilityTagDefinition>(); // �֘A����\�̓^�O�̃��X�g

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
        // ScriptableObject�̃C���X�^���X���쐬
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