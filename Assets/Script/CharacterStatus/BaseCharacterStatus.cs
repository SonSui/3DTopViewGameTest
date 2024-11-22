using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���N���X�F�L�����N�^�[�X�e�[�^�X
public abstract class BaseCharacterStatus : MonoBehaviour
{
    // ===== ��{���� =====

    // HP�i�̗́j
    protected int hpNow;     // ���݂�HP
    protected int hpMax;     // �ő�HP

    // �U����
    protected int attackNow;    // ���݂̍U���́i�o�t�E�f�o�t�K�p��j
    protected int attackPower;  // ��{�U����

    // �h���
    protected int defenseNow;   // ���݂̖h��́i�o�t�E�f�o�t�K�p��j
    protected int defense;      // ��{�h���

    // �N���e�B�J���֘A
    protected float criticalRate;    // �N���e�B�J����
    protected float criticalDamage;  // �N���e�B�J���_���[�W�{��

    // �ړ����x
    protected float moveSpeedNow; // ���݂̈ړ����x
    protected float moveSpeed;    // ��{�ړ����x�i1.0����j

    // �U�����x
    protected float attackSpeed;  // �U�����x�i1.0����j

    // �U���͈�
    protected float attackRange;  // �U���͈́i1.0����j

    // ���
    protected float evasionRate;  // ���

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
        int hpMax = 5,
        int attackPower = 2,
        int defense = 1,
        float criticalRate = 0.0f,
        float criticalDamage = 2.0f,
        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f,
        float attackRange = 1.0f,
        float evasionRate = 0.05f
    )
    {
        // ��{�����̏�����
        this.hpMax = hpMax;
        this.hpNow = this.hpMax;

        this.attackPower = attackPower;
        this.attackNow = this.attackPower;

        this.defense = defense;
        this.defenseNow = this.defense;

        this.criticalRate = criticalRate;
        this.criticalDamage = criticalDamage;

        this.moveSpeed = moveSpeed;
        this.moveSpeedNow = this.moveSpeed;

        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;

        this.evasionRate = evasionRate;

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
        Debug.Log($"{gameObject.name}�͗�����{bleedingDamage}�̃_���[�W���󂯂�");

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
        Debug.Log($"{gameObject.name}�͓|�ꂽ");
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

    // �N���e�B�J����
    public float GetCriticalRate() => criticalRate;
    public void SetCriticalRate(float value) => criticalRate = Mathf.Clamp01(value);

    // �N���e�B�J���_���[�W
    public float GetCriticalDamage() => criticalDamage;
    public void SetCriticalDamage(float value) => criticalDamage = Mathf.Max(1.0f, value);

    // �ړ����x
    public float GetMoveSpeedNow() => moveSpeedNow;
    public float GetMoveSpeed() => moveSpeed;
    public void SetMoveSpeed(float value) => moveSpeed = Mathf.Max(0f, value);

    // �U�����x
    public float GetAttackSpeed() => attackSpeed;
    public void SetAttackSpeed(float value) => attackSpeed = Mathf.Max(0f, value);

    // �U���͈�
    public float GetAttackRange() => attackRange;
    public void SetAttackRange(float value) => attackRange = Mathf.Max(0f, value);

    // ���
    public float GetEvasionRate() => evasionRate;
    public void SetEvasionRate(float value) => evasionRate = Mathf.Clamp01(value);

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
}
