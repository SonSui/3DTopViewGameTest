using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �v���C���[�N���X�F�v���C���[�̃X�e�[�^�X�Ǘ�
public class PlayerStatus : BaseCharacterStatus
{

    private AbilityManager abilityManager;
    // ===== �v���C���[��p�̑��� =====

    // �U���͈�
    protected float attackRange;  // �U���͈́i1.0����j

    // ���
    protected float evasionRate;  // ���

    // �N���e�B�J���֘A
    protected float criticalRate;    // �N���e�B�J����
    protected float criticalDamage;  // �N���e�B�J���_���[�W�{��

    // �e�ʊ֘A
    private int ammoCapacity;    // �e��
    private int ammoRecovery;    // �G��|�����Ƃ��ɉ񕜂���e���i�f�t�H���g0�j
    private float ammoEcho;      // �e�ߖ�m��
    private int ammoPenetration; // �e�ђ�



    // ����\�̗͂L�����t���O
    private bool hpRecovery;          // HP�񕜂���
    private bool explosion;           // ����
    private bool timeStop;            // ���Ԓ�~
    private bool teleport;            // �u�Ԉړ�
    private bool timedPowerUpMode;    // �����������[�h
    private bool swordBeam;           // �\�[�h�r�[��
    private bool resurrection;        // ����
    private int resurrectionTime;     // �����\�ȉ�

    private bool barrier;             // �o���A
    private int barrierHP;          �@// �o���A��HP
    private bool oneHitKill;          // �ꌂ�K�E
    private bool multiAttack;         // ���d�U��
    private bool defensePenetration;  // �h��ђ�


    private bool isDefenseReduction;    // �h��̓_�E��
    private bool isAttackReduction;  // �U���̓_�E��
    private bool isSlowEffect;       // ����
    private bool isBleedingEffect;   // ����
    private bool isStun;             // �X�^��

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
        int resurrectionTime = 1,
        string name = "Player"
    ) : base(
        name: name,
        hpMax: hpMax,
        attackPower: attackPower,
        defense: 0, // �v���C���[�̖h��͂͏��0
        moveSpeed: moveSpeed,
        attackSpeed: attackSpeed
    )
    {
        // �v���C���[��p�̑�����������

        this.criticalRate = criticalRate;
        this.criticalDamage = criticalDamage;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;
        this.evasionRate = evasionRate;


        this.ammoCapacity = ammoCapacity;
        this.ammoRecovery = ammoRecovery;
        this.ammoEcho = 0f;
        this.ammoPenetration = 0;

        this.explosion = false;
        this.timeStop = false;
        this.teleport = false;
        this.timedPowerUpMode = false;
        this.swordBeam = false;
        this.resurrection = false;
        this.resurrectionTime = resurrectionTime;

        this.barrier = false;
        this.barrierHP = 0;
        this.oneHitKill = false;
        this.multiAttack = false;
        this.defensePenetration = false;
        this.isAttackReduction = false;
        this.isBleedingEffect = false;
        this.isDefenseReduction = false;
        this.isSlowEffect = false;
        this.isStun = false;
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
    public bool GetHpAutoRecovery() => hpRecovery;
    public void SetHpAutoRecovery(bool value) => hpRecovery = value;


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

    // �U���͈͂��擾
    public float GetAttackRange() => attackRange;
    // �U���͈͂�ݒ�
    public void SetAttackRange(float value) => attackRange = Mathf.Max(0f, value);

    // ��𗦂��擾
    public float GetEvasionRate() => evasionRate;
    public void SetEvasionRate(float value) => evasionRate = Mathf.Clamp01(value);

    // �N���e�B�J����
    public float GetCriticalRate() => criticalRate;
    public void SetCriticalRate(float value) => criticalRate = Mathf.Clamp01(value);

    /// �N���e�B�J���_���[�W�{��
    public float GetCriticalDamage() => criticalDamage;
    public void SetCriticalDamage(float value) => criticalDamage = Mathf.Max(1f, value);

    // �e�ߖ�m��
    public float GetAmmoEcho() => ammoEcho;
    public void SetAmmoEcho(float value) => ammoEcho = Mathf.Clamp01(value);

    // �e�ђʉ�
    public int GetAmmoPenetration() => ammoPenetration;
    public void SetAmmoPenetration(int value) => ammoPenetration = Mathf.Max(0, value);

    // �o���A��HP
    public int GetBarrierHP() => barrierHP;
    public void SetBarrierHP(int value) => barrierHP = Mathf.Max(0, value);

    //�h��̓_�E�����
    public bool GetIsDefenseReductionFlag() => isDefenseReduction;
    public void SetIsDefenseReductionFlag(bool value) => isDefenseReduction = value;

    //�U���̓_�E��
    public bool GetIsAttackReductionFlag() => isAttackReduction;
    public void SetIsAttackReductionFlag(bool value) => isAttackReduction = value;

    //����
    public bool GetIsSlowEffectFlag() => isSlowEffect;
    public void SetIsSlowEffectFlag(bool value) => isSlowEffect = value;

    //�����
    public bool GetIsBleedingEffectFlag() => isBleedingEffect;
    public void SetIsBleedingEffectFlag(bool value) => isBleedingEffect = value;

    // �X�^��
    public bool GetIsStunFlag() => isStun;
    public void SetIsStunFlag(bool value) => isStun = value;


    // ���L�͗�FBase�p��Getter�i���ۂ͏��������̒l���L�����ĕԂ��Ȃǂ̍H�v���K�v�j
    public int GetBaseHpMax() { /* �����l��Ԃ������������� */ return 5; }
    public int GetBaseAttackPower() { /*�����l*/ return 3; }
    public float GetBaseCriticalRate() { return 0.1f; }
    public float GetBaseCriticalDamage() { return 2.0f; }
    public float GetBaseMoveSpeed() { return 1.0f; }
    public float GetBaseAttackSpeed() { return 1.0f; }
    public float GetBaseAttackRange() { return 1.0f; }
    public float GetBaseEvasionRate() { return 0.05f; }

    public int GetBaseAmmoCapacity() { return 0; }
    public int GetBaseAmmoRecovery() { return 0; }
    public float GetBaseAmmoEcho() { return 0f; }
    public int GetBaseAmmoPenetration() { return 0; }
    public int GetBaseResurrectionTime() { return 1; }

    // bool�nBase�l�͂��ׂ�false(�܂��̓f�t�H���g)�Ƃ���z��
    public bool GetBaseHpAutoRecovery() { return false; }
    public bool GetBaseExplosion() { return false; }
    public bool GetBaseTimeStop() { return false; }
    public bool GetBaseTeleport() { return false; }
    public bool GetBaseTimedPowerUpMode() { return false; }
    public bool GetBaseSwordBeam() { return false; }
    public bool GetBaseResurrection() { return false; }
    public bool GetBaseBarrier() { return false; }
    public bool GetBaseOneHitKill() { return false; }
    public bool GetBaseMultiAttack() { return false; }
    public bool GetBaseDefensePenetration() { return false; }

    public bool GetBaseIsDefenseReduction() { return false; }
    public bool GetBaseIsAttackReduction() { return false; }
    public bool GetBaseIsSlowEffect() { return false; }
    public bool GetBaseIsBleedingEffect() { return false; }
    public bool GetBaseIsStun() { return false; }




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
    public int ReturnTakeDamage(int damage)
    {
        // �o���A���L���ȏꍇ�A�_���[�W���󂯂Ȃ�
        if (barrier)
        {
            Debug.Log("�o���A�ɂ���ă_���[�W�𖳌���");
            return 0;
        }

        // ��𔻒�
        if (Random.value < evasionRate)
        {
            Debug.Log("�U�����������");
            return 0;
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
        return actualDamage;
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
    public Dictionary<string, bool> GetAllAbilitiesStatus()
    {
        // �S�Ă�bool�t���O���܂Ƃ߂ĕԂ���
        Dictionary<string, bool> abilities = new Dictionary<string, bool>()
        {
            { "HpRecovery", hpRecovery },
            { "Explosion", explosion },
            { "TimeStop", timeStop },
            { "Teleport", teleport },
            { "TimedPowerUpMode", timedPowerUpMode },
            { "SwordBeam", swordBeam },
            { "Resurrection", resurrection },
            { "Barrier", barrier },
            { "OneHitKill", oneHitKill },
            { "MultiAttack", multiAttack },
            { "DefensePenetration", defensePenetration },
            { "IsDefenseReduction", isDefenseReduction },
            { "IsAttackReduction", isAttackReduction },
            { "IsSlowEffect", isSlowEffect },
            { "IsBleedingEffect", isBleedingEffect },
            { "IsStun", isStun }
        };

        return abilities;
    }
    // �A�C�e���擾���ɌĂԃ��\�b�h�̗�
    public void OnItemCollected(ItemData item)
    {
        abilityManager.AddItem(item);
    }

    public void OnItemRemoved(ItemData item)
    {
        abilityManager.RemoveItem(item);
    }
}
