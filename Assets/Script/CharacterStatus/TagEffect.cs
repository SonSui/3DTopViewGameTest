using UnityEngine;

[System.Serializable] 
public class TagEffect
{
    [Header("��{����*8+1")]
    [SerializeField] public int hpMax;                  // �ő�HP
    [SerializeField] public int attackPower;            // ��{�U����
    [SerializeField] public float criticalRate;         // �N���e�B�J����
    [SerializeField] public float criticalDamage;       // �N���e�B�J���_���[�W�{��
    [SerializeField] public float moveSpeed;            // ��{�ړ����x�i1.0����j
    [SerializeField] public float attackSpeed;          // �U�����x�i1.0����j
    [SerializeField] public float attackRange;          // �U���͈́i1.0����j
    [SerializeField] public float evasionRate;          // ���


    [Header("�f�o�t�\��*5")]
    [SerializeField] public bool isDefenseReduction;    // �h��̓_�E��
    [SerializeField] public bool isAttackReduction;     // �U���̓_�E��
    [SerializeField] public bool isSlowEffect;          // ����
    [SerializeField] public bool isBleedingEffect;      // ����
    [SerializeField] public bool isStun;                // �X�^��

    [Header("�ˌ��\��*5")]
    [SerializeField] public int ammoCapacity;           // �e��
    [SerializeField] public int ammoRecovery;           // �G��|�����Ƃ��ɉ񕜂���e��
    [SerializeField] public float ammoEcho;             // �e�ߖ�m��
    [SerializeField] public int ammoPenetration;        // �e�ђ�
    [SerializeField] public int resurrectionTime;       // �����\�ȉ�

    [Header("����\��*11")]
    [SerializeField] public bool hpRecovery;            // HP�񕜂���
    [SerializeField] public bool explosion;             // ����
    [SerializeField] public bool timeStop;              // ���Ԓ�~
    [SerializeField] public bool teleport;              // �u�Ԉړ�
    [SerializeField] public bool timedPowerUpMode;      // �����������[�h
    [SerializeField] public bool swordBeam;             // �\�[�h�r�[��
    [SerializeField] public bool resurrection;          // ����    
    [SerializeField] public bool barrier;               // �o���A
    [SerializeField] public bool oneHitKill;            // �ꌂ�K�E
    [SerializeField] public bool multiAttack;           // ���d�U��
    [SerializeField] public bool defensePenetration;    // �h��ђ�
}