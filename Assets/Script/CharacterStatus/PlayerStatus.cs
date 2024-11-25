using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �v���C���[�N���X�F�v���C���[�̃X�e�[�^�X�Ǘ�
public class PlayerStatus : BaseCharacterStatus
{
    // ===== �v���C���[��p�̑��� =====

    // �e�ʊ֘A
    private int ammoCapacity;    // �e��
    private int ammoRecovery;    // �G��|�����Ƃ��ɉ񕜂���e���i�f�t�H���g0�j

    // HP������
    private float hpAutoRecovery;   // ��莞�Ԃ��Ƃɉ񕜂���HP�ʁi�f�t�H���g0�j
    private float hpRecoveryInterval; // HP�񕜂̊Ԋu
    private float hpRecoveryTimer;    // HP�񕜂̃^�C�}�[

    // ����\�̗͂L�����t���O
    private bool explosion;           // ����
    private bool timeStop;            // ���Ԓ�~
    private bool teleport;            // �u�Ԉړ�
    private bool timedPowerUpMode;    // �����������[�h
    private bool swordBeam;           // �\�[�h�r�[��
    private bool resurrection;        // ����
    private int resurrectionTime;     // �����\�ȉ�

    private bool barrier;             // �o���A
    private bool oneHitKill;          // �ꌂ�K�E
    private bool multiAttack;         // ���d�U��
    private bool defensePenetration;  // �h��ђ�

    // ===== �R���X�g���N�^ =====

    // �������i���N���X�̃R���X�g���N�^���Ăяo���j
    public PlayerStatus(
        int hpMax = 5,
        int attackPower = 3,
        float criticalRate = 0.1f,
        float criticalDamage = 2.0f,
        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f,
        float attackRange = 1.0f,
        float evasionRate = 0.05f,
        int ammoCapacity = 10,
        int ammoRecovery = 0,
        float hpAutoRecovery = 0f,
        float hpRecoveryInterval = 5f,
        int resurrectionTime = 1,
        string name = "Player"
    ) : base(
        name: name,
        hpMax: hpMax,
        attackPower: attackPower,
        defense: 0, // �v���C���[�̖h��͂͏��0
        criticalRate: criticalRate,
        criticalDamage: criticalDamage,
        moveSpeed: moveSpeed,
        attackSpeed: attackSpeed,
        attackRange: attackRange,
        evasionRate: evasionRate
    )
    {
        // �v���C���[��p�̑�����������
        this.ammoCapacity = ammoCapacity;
        this.ammoRecovery = ammoRecovery;

        this.hpAutoRecovery = hpAutoRecovery;
        this.hpRecoveryInterval = hpRecoveryInterval;
        this.hpRecoveryTimer = this.hpRecoveryInterval;

        this.explosion = false;
        this.timeStop = false;
        this.teleport = false;
        this.timedPowerUpMode = false;
        this.swordBeam = false;
        this.resurrection = false;
        this.resurrectionTime = resurrectionTime;

        this.barrier = false;
        this.oneHitKill = false;
        this.multiAttack = false;
        this.defensePenetration = false;
    }

    // ===== ���\�b�h =====

    // �_���[�W���󂯂鏈��
    public override void TakeDamage(int damage, bool isDefensePenetration = false)
    {
        // �o���A���L���ȏꍇ�A�_���[�W���󂯂Ȃ�
        if (barrier)
        {
            Debug.Log("�o���A�ɂ���ă_���[�W�𖳌���");
            return;
        }

        // ��𔻒�
        if (Random.value < evasionRate)
        {
            Debug.Log("�U�����������");
            return;
        }

        // �h��ђʂ��L���ȏꍇ�A�h��͂𖳎��i�h��͂͏��0�Ȃ̂ŉe���Ȃ��j
        int actualDamage = damage;

        // �_���[�W�K�p
        hpNow -= actualDamage;
        Debug.Log($"�v���C���[��{actualDamage}�̃_���[�W���󂯂��i�c��HP: {hpNow}/{hpMax}�j");

        if (IsDead())
        {
            if (resurrection && resurrectionTime > 0)
            {
                Resurrection();
            }
            else
            {
                OnDeath();
            }
        }
    }

    // �X�e�[�^�X���X�V�i���t���[���Ăяo���j
    public override void UpdateStatus(float deltaTime, float timeRate = 1.0f)
    {
        base.UpdateStatus(deltaTime, timeRate);

        float adjustedDeltaTime = deltaTime * timeRate;

        // HP������
        if (hpAutoRecovery > 0)
        {
            hpRecoveryTimer -= adjustedDeltaTime;
            if (hpRecoveryTimer <= 0)
            {
                hpNow = Mathf.Min(hpNow + (int)hpAutoRecovery, hpMax);
                hpRecoveryTimer = hpRecoveryInterval;
                Debug.Log($"HP�������񕜁F���݂�HP��{hpNow}/{hpMax}");
            }
        }

        // �����������[�h�̏����i�K�v�ɉ����Ď����j
        if (timedPowerUpMode)
        {
            // �������[�h�̏���
        }

        // ���̑��̓���\�͂̏����i�K�v�ɉ����Ď����j
    }

    // ��������
    private void Resurrection()
    {
        resurrectionTime--;
        hpNow = hpMax; // HP��S��
        Debug.Log($"�v���C���[�͕��������i�c�蕜����: {resurrectionTime}�j");
    }

    // ���S���̏���
    protected override void OnDeath()
    {
        // �v���C���[�̎��S����
        Debug.Log("�v���C���[�͓|�ꂽ");
        // �Q�[���I�[�o�[�����Ȃǂ�����
    }

    // ===== Getter/Setter =====

    // �e��
    public int GetAmmoCapacity() => ammoCapacity;
    public void SetAmmoCapacity(int value) => ammoCapacity = Mathf.Max(0, value);

    // �e�񕜗�
    public int GetAmmoRecovery() => ammoRecovery;
    public void SetAmmoRecovery(int value) => ammoRecovery = Mathf.Max(0, value);

    // HP�����񕜗�
    public float GetHpAutoRecovery() => hpAutoRecovery;
    public void SetHpAutoRecovery(float value) => hpAutoRecovery = Mathf.Max(0f, value);

    // HP�񕜊Ԋu
    public float GetHpRecoveryInterval() => hpRecoveryInterval;
    public void SetHpRecoveryInterval(float value) => hpRecoveryInterval = Mathf.Max(0.1f, value); // 0.1�b�ȏ�

    // ����\�̗͂L�����t���O��Getter/Setter

    // ����
    public bool IsExplosionEnabled() => explosion;
    public void SetExplosion(bool value) => explosion = value;

    // ���Ԓ�~
    public bool IsTimeStopEnabled() => timeStop;
    public void SetTimeStop(bool value) => timeStop = value;

    // �u�Ԉړ�
    public bool IsTeleportEnabled() => teleport;
    public void SetTeleport(bool value) => teleport = value;

    // �����������[�h
    public bool IsTimedPowerUpModeEnabled() => timedPowerUpMode;
    public void SetTimedPowerUpMode(bool value) => timedPowerUpMode = value;

    // �\�[�h�r�[��
    public bool IsSwordBeamEnabled() => swordBeam;
    public void SetSwordBeam(bool value) => swordBeam = value;

    // ����
    public bool IsResurrectionEnabled() => resurrection;
    public void SetResurrection(bool value) => resurrection = value;

    // �o���A
    public bool IsBarrierEnabled() => barrier;
    public void SetBarrier(bool value) => barrier = value;

    // �ꌂ�K�E
    public bool IsOneHitKillEnabled() => oneHitKill;
    public void SetOneHitKill(bool value) => oneHitKill = value;

    // ���d�U��
    public bool IsMultiAttackEnabled() => multiAttack;
    public void SetMultiAttack(bool value) => multiAttack = value;

    // �h��ђ�
    public bool IsDefensePenetrationEnabled() => defensePenetration;
    public void SetDefensePenetration(bool value) => defensePenetration = value;

    // ������
    public int GetResurrectionTime() => resurrectionTime;
    public void SetResurrectionTime(int value) => resurrectionTime = Mathf.Max(0, value);

    // ===== ���̑��̃��\�b�h =====

    // �e�����
    public void RecoverAmmo()
    {
        ammoCapacity += ammoRecovery;
        Debug.Log($"�e����񕜁F���݂̒e���{ammoCapacity}");
    }

    // �G��|�����Ƃ��̏���
    public void OnEnemyDefeated()
    {
        RecoverAmmo();
        // ���̑��̏���
    }


    // =====�X�e�[�^�X��\��=====
    public  string ShowPlayerStatus()
    {
        string baseStatus = base.GetBaseStatus(); // ��{�X�e�[�^�X���擾

        // �v���C���[��p�̃X�e�[�^�X��ǉ�
        string playerStatus =
            $"Ammo Capacity: {ammoCapacity}\n" +
            "";

        string fullStatus = baseStatus + "\n" + playerStatus;

        Debug.Log($"PlayerStatus:{fullStatus}");
        return fullStatus;
    }
}
