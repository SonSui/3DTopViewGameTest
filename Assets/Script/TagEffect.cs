using UnityEngine;

[System.Serializable] 
public class TagEffect
{
    [SerializeField] private int hpMax;                  // �ő�HP
    [SerializeField] private int attackPower;            // ��{�U����
    [SerializeField] private float criticalRate;         // �N���e�B�J����
    [SerializeField] private float criticalDamage;       // �N���e�B�J���_���[�W�{��
    [SerializeField] private float moveSpeed;            // ��{�ړ����x�i1.0����j
    [SerializeField] private float attackSpeed;          // �U�����x�i1.0����j
    [SerializeField] private float attackRange;          // �U���͈́i1.0����j
    [SerializeField] private float evasionRate;          // ���

    [SerializeField] private bool isDefenseReduction;    // �h��̓_�E��
    [SerializeField] private bool isAttackReduction;     // �U���̓_�E��
    [SerializeField] private bool isSlowEffect;          // ����
    [SerializeField] private bool isBleedingEffect;      // ����
    [SerializeField] private bool isStun;                // �X�^��

    [SerializeField] private int ammoCapacity;           // �e��
    [SerializeField] private int ammoRecovery;           // �G��|�����Ƃ��ɉ񕜂���e��
    [SerializeField] private float ammoEcho;             // �e�ߖ�m��
    [SerializeField] private int ammoPenetration;        // �e�ђ�
    [SerializeField] private int resurrectionTime;       // �����\�ȉ�

    [SerializeField] private bool hpRecovery;            // HP�񕜂���
    [SerializeField] private bool explosion;             // ����
    [SerializeField] private bool timeStop;              // ���Ԓ�~
    [SerializeField] private bool teleport;              // �u�Ԉړ�
    [SerializeField] private bool timedPowerUpMode;      // �����������[�h
    [SerializeField] private bool swordBeam;             // �\�[�h�r�[��
    [SerializeField] private bool resurrection;          // ����    
    [SerializeField] private bool barrier;               // �o���A
    [SerializeField] private bool oneHitKill;            // �ꌂ�K�E
    [SerializeField] private bool multiAttack;           // ���d�U��
    [SerializeField] private bool defensePenetration;    // �h��ђ�
}