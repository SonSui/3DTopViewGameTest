using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �G�L�����N�^�[�N���X�F�G�̃X�e�[�^�X�Ǘ�
public class EnemyStatus : BaseCharacterStatus
{
    // ===== �G��p�̑��� =====

    // �G�̎�ށi�ʏ�A�G���[�g�A�{�X�Ȃǁj
    private string enemyType;

    // �h���b�v�A�C�e���̃��X�g
    private List<Item> dropItems;

    // ����\�̗͂L�����t���O
    private bool hasShield;        // �V�[���h�������Ă��邩
    private int shieldDurability; // �V�[���h�̑ϋv�l

    // ===== �R���X�g���N�^ =====

    // �������i���N���X�̃R���X�g���N�^���Ăяo���j
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
        // �G��p�̑�����������
        this.enemyType = enemyType;
        this.dropItems = new List<Item>();

        this.hasShield = hasShield;
        this.shieldDurability = shieldDurability;

    }

    // ===== ���\�b�h =====

    // �_���[�W���󂯂鏈��
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

        if (!isDefensePenetration)// �h��ђʂ��L���ȏꍇ�A�h��͂𖳎�
        {
            if(actualDamage>0)actualDamage = Mathf.Max(1, actualDamage - defenseNow);
        }
        


        // �_���[�W�K�p
        if (actualDamage > 0)
        {
            hpNow -= actualDamage;
            Debug.Log($"{name} ��{actualDamage} �̃_���[�W���󂯂��i�c��HP: {hpNow}/{hpMax}�j");
        }



        if (IsDead())
        {
            OnDeath();
        }
        return actualDamage;
    }


    // �X�e�[�^�X���X�V�i���t���[���Ăяo���j
    public override int UpdateStatus(float deltaTime, float timeRate = 1.0f)
    {
        return base.UpdateStatus(deltaTime, timeRate);
    }

    // ���S���̏���
    protected override void OnDeath()
    {
        // �G�̎��S����
        Debug.Log($"{this.name} �͓|���ꂽ");
        DropItems();

    }


    // �h���b�v�A�C�e���̐���
    private void DropItems()
    {
        // �h���b�v�A�C�e���𐶐����鏈��
        foreach (var item in dropItems)
        {
            // �A�C�e���𐶐�?
            
            //Debug.Log($"{item.itemName} ���h���b�v����");
        }
    }

    // �A�C�e�����h���b�v���X�g�ɒǉ�
    public void AddDropItem(Item item)
    {
        dropItems.Add(item);
    }

    // ===== Getter/Setter =====

    // �G�̎��
    public string GetEnemyType() => enemyType;
    public void SetEnemyType(string value) => enemyType = value;


    // �V�[���h�֘A
    public bool HasShield() => hasShield;
    public void SetShield(bool value, int durability = 0)
    {
        hasShield = value;
        shieldDurability = durability;
    }

    public int GetShieldDurability() => shieldDurability;
    public void SetShieldDurability(int value) => shieldDurability = Mathf.Max(0, value);



    // ===== ���̑��̃��\�b�h =====

    // ����\�͂�K�p
    public void ApplySpecialAbility(string abilityName)
    {
        switch (abilityName)
        {
            case "Shield":
                SetShield(true, 50); // �V�[���h�ϋv�l50�ŗL����
                break;
            
            default:
                Debug.Log($"���m�̓���\��: {abilityName}");
                break;
        }
    }
}
