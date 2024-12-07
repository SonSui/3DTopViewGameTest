using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���N���X�F�L�����N�^�[�X�e�[�^�X
public abstract class BaseCharacterStatus
{
    // ===== ��{���� =====
    // ���O
    protected string name;

    // HP�i�̗́j
    protected int hpNow;     // ���݂�HP
    protected int hpMax;     // �ő�HP

    // �U����
    protected int attackNow;    // ���݂̍U���́i�o�t�E�f�o�t�K�p��j
    protected int attackPower;  // ��{�U����

    // �h���
    protected int defenseNow;   // ���݂̖h��́i�o�t�E�f�o�t�K�p��j
    protected int defense;      // ��{�h���

    // �ړ����x
    protected float moveSpeedNow; // ���݂̈ړ����x
    protected float moveSpeed;    // ��{�ړ����x�i1.0����j

    // �U�����x
    protected float attackSpeed;  // �U�����x�i1.0����j



    // ===== ��Ԍ��� =====

    // �h��̓_�E��
    protected float defenseReduction;                   // �h��̓_�E���̎c�莞��
    protected const float defenseReductionRate = 0.5f;  // �h��̓_�E����

    // �U���̓_�E��
    protected float attackReduction;                    // �U���̓_�E���̎c�莞��
    protected const float attackReductionRate = 0.5f;   // �U���̓_�E����

    // ����
    protected float slowEffect;                         // �����̎c�莞��
    protected const float slowEffectRate = 0.5f;        // ������

    // ����
    protected float bleedingEffect;                     // �����̎c�莞��
    protected const float bleedingEffectRate = 0.05f;   // �����_���[�W��
    protected float bleedingInterval;                   // �����_���[�W�̊Ԋu
    protected float bleedingTimer;                      // �����_���[�W�̃^�C�}�[

    // �X�^��
    protected float stun;                               // �X�^���̎c�莞��

    // ===== �R���X�g���N�^ =====

    // �������i�p�����[�^�t���A�f�t�H���g�l����j
    public BaseCharacterStatus(
        string name = "",
        int hpMax = 5,
        int attackPower = 2,
        int defense = 1,
        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f
    )
    {
        this.name = name;
        // ��{�����̏�����
        this.hpMax = hpMax;
        this.hpNow = this.hpMax;

        this.attackPower = attackPower;
        this.attackNow = this.attackPower;

        this.defense = defense;
        this.defenseNow = this.defense;


        this.moveSpeed = moveSpeed;
        this.moveSpeedNow = this.moveSpeed;

        this.attackSpeed = attackSpeed;


        // ��Ԍ��ʂ̏�����
        this.defenseReduction = 0f;
        this.attackReduction = 0f;
        this.slowEffect = 0f;
        this.bleedingEffect = 0f;
        this.bleedingInterval = 1.0f; // �����_���[�W�̊Ԋu��������
        this.bleedingTimer = this.bleedingInterval;
        this.stun = 0f;
    }

    // ===== ���\�b�h =====

    // �_���[�W���󂯂鏈���i���ۃ��\�b�h�j
    public abstract void TakeDamage(int damage, bool isDefensePenetration = false);

    // �X�e�[�^�X���X�V�i���t���[���Ăяo���j
    public virtual void UpdateStatus(float deltaTime, float timeRate = 1.0f)
    {
        // �o�t�v�Z���ꎞ��~����ꍇ�́AtimeRate��0�ɐݒ�
        float adjustedDeltaTime = deltaTime * timeRate;

        // ��Ԍ��ʂ̎��Ԃ��X�V
        UpdateStateEffects(adjustedDeltaTime);

        // ���݂̃X�e�[�^�X���Čv�Z
        RecalculateStats();
    }

    // ��Ԍ��ʂ̎��Ԃ��X�V
    protected void UpdateStateEffects(float deltaTime)
    {
        // �h��̓_�E�����Ԃ̍X�V
        if (defenseReduction > 0)
        {
            defenseReduction = Mathf.Max(0, defenseReduction - deltaTime);
        }

        // �U���̓_�E�����Ԃ̍X�V
        if (attackReduction > 0)
        {
            attackReduction = Mathf.Max(0, attackReduction - deltaTime);
        }

        // �������Ԃ̍X�V
        if (slowEffect > 0)
        {
            slowEffect = Mathf.Max(0, slowEffect - deltaTime);
        }

        // �������Ԃ̍X�V
        if (bleedingEffect > 0)
        {
            bleedingEffect = Mathf.Max(0, bleedingEffect - deltaTime);

            // �����_���[�W�̃^�C�}�[���X�V
            bleedingTimer -= deltaTime;
            if (bleedingTimer <= 0)
            {
                // �����_���[�W��K�p
                ApplyBleedingDamage();
                bleedingTimer = bleedingInterval;
            }
        }
        else
        {
            bleedingTimer = bleedingInterval; // �������ʂ��I��������^�C�}�[�����Z�b�g
        }

        // �X�^�����Ԃ̍X�V
        if (stun > 0)
        {
            stun = Mathf.Max(0, stun - deltaTime);
        }
    }

    // �X�e�[�^�X�̍Čv�Z
    protected void RecalculateStats()
    {
        // �h��͂̍Čv�Z
        defenseNow = defense;
        if (defenseReduction > 0)
        {
            defenseNow = (int)(defense * (1 - defenseReductionRate));
        }

        // �U���͂̍Čv�Z
        attackNow = attackPower;
        if (attackReduction > 0)
        {
            attackNow = (int)(attackPower * (1 - attackReductionRate));
        }

        // �ړ����x�̍Čv�Z
        moveSpeedNow = moveSpeed;
        if (slowEffect > 0)
        {
            moveSpeedNow = moveSpeed * (1 - slowEffectRate);
        }
    }

    // �����_���[�W��K�p
    protected void ApplyBleedingDamage()
    {
        int bleedingDamage = (int)(hpMax * bleedingEffectRate);
        hpNow -= bleedingDamage;

        // �_���[�W���O
        Debug.Log($"{this.name}�͗�����{bleedingDamage}�̃_���[�W���󂯂�");

        if (hpNow <= 0)
        {
            hpNow = 0;
            OnDeath();
        }
    }

    // ���S���̏���
    protected virtual void OnDeath()
    {
        // �f�t�H���g�̎��S����
        Debug.Log($"{this.name}�͓|�ꂽ");
    }

    // HP���[�����ǂ������m�F
    public bool IsDead()
    {
        return hpNow <= 0;
    }

    // ===== Getter/Setter =====

    // HP
    public int GetHpNow() => hpNow;
    public int GetHpMax() => hpMax;
    public void SetHpNow(int value) => hpNow = Mathf.Clamp(value, 0, hpMax);
    public void SetHpMax(int value)
    {
        hpMax = Mathf.Max(1, value);
        hpNow = Mathf.Min(hpNow, hpMax); // �ő�HP�����������ꍇ�A���݂�HP�𒲐�
    }

    // �U����
    public int GetAttackNow() => attackNow;
    public int GetAttackPower() => attackPower;
    public void SetAttackPower(int value) => attackPower = Mathf.Max(0, value);

    // �h���
    public int GetDefenseNow() => defenseNow;
    public int GetDefense() => defense;
    public void SetDefense(int value) => defense = Mathf.Max(0, value);

    // �ړ����x
    public float GetMoveSpeedNow() => moveSpeedNow;
    public float GetMoveSpeed() => moveSpeed;
    public void SetMoveSpeed(float value) => moveSpeed = Mathf.Max(0f, value);

    // �U�����x
    public float GetAttackSpeed() => attackSpeed;
    public void SetAttackSpeed(float value) => attackSpeed = Mathf.Max(0f, value);



    // ===== ��Ԍ��ʂ̓K�p =====

    // �h��̓_�E����K�p
    public void ApplyDefenseReduction(float duration)
    {
        defenseReduction = Mathf.Max(defenseReduction, duration);
    }

    // �U���̓_�E����K�p
    public void ApplyAttackReduction(float duration)
    {
        attackReduction = Mathf.Max(attackReduction, duration);
    }

    // ������K�p
    public void ApplySlowEffect(float duration)
    {
        slowEffect = Mathf.Max(slowEffect, duration);
    }

    // ������K�p
    public void ApplyBleedingEffect(float duration)
    {
        bleedingEffect = Mathf.Max(bleedingEffect, duration);
    }

    // �X�^����K�p
    public void ApplyStun(float duration)
    {
        stun = Mathf.Max(stun, duration);
    }
    //�܂Ƃ߂ēK�p
    public void ApplyDebuff(float stun,float bleed,float slow,float defReduce,float atkReduce)
    {
        this.stun = stun;
        this.bleedingEffect = bleed;
        this.slowEffect = slow;
        this.defenseReduction = defReduce;
        this.attackReduction = atkReduce;
    }


    // ===== ��Ԍ��ʂ̊m�F =====

    // �h��̓_�E�������m�F
    public bool IsDefenseReduced() => defenseReduction > 0;

    // �U���̓_�E�������m�F
    public bool IsAttackReduced() => attackReduction > 0;

    // ���������m�F
    public bool IsSlowed() => slowEffect > 0;

    // ���������m�F
    public bool IsBleeding() => bleedingEffect > 0;

    // �X�^�������m�F
    public bool IsStunned() => stun > 0;

    public (
        bool isStunned, 
        bool isBleeding, 
        bool isSlowed, 
        bool isDefReduced, 
        bool isAtkReduced
        ) GetAllDebuffStatus()
    {
        // �܂Ƃ߂Ċm�F
        return (
            stun > 0,
            bleedingEffect > 0,
            slowEffect > 0,
            defenseReduction > 0,
            attackReduction > 0
            );
    }
    

    // =====��{�X�e�[�^�X��\��=====
    public string GetBaseStatus()
    {
        string status =
            $"Name:{name}\n"+
            $"HP: {hpNow}/{hpMax}\n" +
            $"Attack Power: {attackPower}\n" +
            $"Defense: {defense}\n" +
            $"Movement Speed: {moveSpeed:F2}\n" +
            $"Attack Speed: {attackSpeed:F2}\n" +
            $"Defense Reduction Time: {defenseReduction:F2}s (Rate: {defenseReductionRate * 100:F2}%)\n" +
            $"Attack Reduction Time: {attackReduction:F2}s (Rate: {attackReductionRate * 100:F2}%)\n" +
            $"Slow Effect Time: {slowEffect:F2}s (Rate: {slowEffectRate * 100:F2}%)\n" +
            $"Bleeding Effect Time: {bleedingEffect:F2}s (Rate: {bleedingEffectRate * 100:F2}%)\n" +
            $"Stun Time: {stun:F2}s";

        
        return status;
    }
}
