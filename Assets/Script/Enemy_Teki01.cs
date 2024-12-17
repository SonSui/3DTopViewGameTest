using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Enemy_Teki01 : MonoBehaviour, IOnHit
{
    EnemyStatus enemyStatus; //�G�̃X�e�[�^�X

    //Inspector��ݒ�ł����{�̃X�e�[�^�X
    public string name_ = "Enemy_Teki01";
    public int hp_ = 4;
    public int attack_ = 1;
    public int defense_ = 1;
    public string type_ = "SuicideBomb";
    public bool hasShiled_ = false;
    public int shieldDurability_ = 0;
    public float moveSpeed_ = 1.0f;
    public float attackSpeed_ = 1.0f;


    //��e�̐F�ω�
    private Renderer[] renderers;        // �G��Renderer���X�g
    [SerializeField] private Material overlayMaterial;   // ��e�̃}�e���A��
    private bool isFlashing = false;    // �t���b�V�������ǂ���
    private float flashDuration = 0.1f; // �t���b�V����������

    //�U����Prefab
    public GameObject hitboxPrefab;    //�U����Hitbox�ƍU���̃G�t�F�N�g��prefab�ɂ���
    private GameObject hitbox = null;�@//��������Hitbox��ۑ�

    //�X�e�[�^�X�}�V��
    public enum EnemyState
    {
        //�S���g���K�v���Ȃ�
        Idle,         // �ҋ@��ԁF�G���������ɑҋ@���Ă���
        Patrol,       // �����ԁF�w�肳�ꂽ���[�g�⃉���_���Ɉړ����Ă���
        Chase,        // �ǐՏ�ԁF�v���C���[�𔭌����ǂ������Ă���
        Attack,       // �U����ԁF�v���C���[��ڕW���U�����Ă���
        Hit,          // �팂��ԁF�U�����󂯂ă_���[�W���󂯂Ă���
        Dead,         // ���S��ԁF�̗͂��[���ɂȂ�s���s�\

        //Stunned,      // �C���ԁF�X�L����U���ōs���s�\�ȏ��
        //Flee,         // ������ԁF�v���C���[�ɕ�����Ɣ��f��������
        //Alert,        // �x����ԁF�v���C���[�̑��݂ɋC�Â������܂��ǐՂ��Ă��Ȃ�
        //Guard,        // �h���ԁF���������
    }
    private EnemyState _state = EnemyState.Idle;
    private EnemyState _nextstate = EnemyState.Idle;

    //�v���C���[�̍��W
    public Transform playerT;
    EnemyGenerator enemyGenerator;


    private void OnEnable()
    {
        name_ += System.Guid.NewGuid().ToString(); //�B��̖��O�t����

        //EnemyBuffer���g���ꍇ�AEnable���тɃX�e�[�^�X�����Z�b�g����
        enemyStatus = new EnemyStatus(
            name_,
            hp_,
            attack_,
            defense_,
            type_,
            hasShiled_,
            shieldDurability_,
            moveSpeed_,
            attackSpeed_);
        ChangeState(EnemyState.Idle); //�ҋ@��Ԑݒ�
    }

    public void SetGenerator(EnemyGenerator generator)
    {
        enemyGenerator = generator;
    }

    void Start()
    {
        //�G���������OnEnable(prefab��Enable�̏�Ԃ̏ꍇ)->Start->Update->Update->Update(���t���C���z��)

        if (enemyGenerator == null) enemyGenerator = FindObjectOfType<EnemyGenerator>();
        playerT = GameObject.FindGameObjectWithTag("Player").transform;

        // �S�Ă�Renderer���擾
        renderers = GetComponentsInChildren<Renderer>();
        // overlayMaterial�����ݒ�̏ꍇ�G���[
        if (overlayMaterial == null)
        {
            Debug.Log("Overlay Material ���ݒ肳��Ă��܂���I");
        }
    }

    private void Update()
    {
        switch(_state)
        { 
            //���݂�Upsate
                case EnemyState.Idle:
                IdleUpdate();
                break;
                case EnemyState.Patrol:
                PatrolUpdate();
                break;
                case EnemyState.Chase:
                ChaseUpdate();
                break;
                case EnemyState.Attack:
                AttackUpdate();
                break;
                case EnemyState.Hit:
                HitUpdate();
                break;
                case EnemyState.Dead:
                DeadUpdate();
                break;
        }

        if (_state != _nextstate)
        {
            //�I������
            switch (_state)
            {
                case EnemyState.Idle:
                    IdleEnd();
                    break;
                case EnemyState.Patrol:
                    PatrolEnd();
                    break;
                case EnemyState.Chase:
                    ChaseEnd();
                    break;
                case EnemyState.Attack:
                    AttackEnd();
                    break;
                case EnemyState.Hit:
                    HitEnd();
                    break;
                case EnemyState.Dead:
                    DeadEnd();
                    break;
            }

            //���̃X�e�[�g�J��
            _state = _nextstate;
            switch (_state)
            {
                case EnemyState.Idle:
                    IdleStart();
                    break;
                case EnemyState.Patrol:
                    PatrolStart();
                    break;
                case EnemyState.Chase:
                    ChaseStart();
                    break;
                case EnemyState.Attack:
                    AttackStart();
                    break;
                case EnemyState.Hit:
                    HitStart();
                    break;
                case EnemyState.Dead:
                    DeadStart();
                    break;
            }
        }

     enemyStatus.UpdateStatus(Time.deltaTime);//�����A�X�^���A�f�o�t�Ȃǖ��t���C�������I�ɏ���
    }

    public void ChangeState(EnemyState nextState)
    {
        _nextstate = nextState;
    }

    //�X�e�[�g����-----------------------------------------------------------------------------

    //Idle
    private void IdleStart() { }
    private void IdleUpdate() { }
    private void IdleEnd() { }

    private void PatrolStart() { }
    private void PatrolUpdate()
    {
        //�v���C���[�Ɍ����Đi��
        transform.position =
            Vector3.MoveTowards
            (transform.position, new Vector3(playerT.position.x, playerT.position.y, playerT.position.z), moveSpeed_ * Time.deltaTime);

        //�v���C���[�Ƃ̋������߂��Ȃ�����Attack
        if (Vector3.Distance(transform.position, playerT.position) < 2.1f) { ChangeState(EnemyState.Attack); }
    }
    private void PatrolEnd() { }

    private void ChaseStart() { }
    private void ChaseUpdate() { }
    private void ChaseEnd() { }

    private void AttackStart() { }
    private void AttackUpdate()
    {
        Attack();
    }
    private void AttackEnd() { }

    private void HitStart() { }
    private void HitUpdate() { }
    private void HitEnd() { }

    private void DeadStart() 
    {
        DyingAnimation();
    }
    private void DeadUpdate() { }
    private void DeadEnd()
    { 
        OnDead();
    }

    private bool enemyDying;//Enemy�͎���ł��邩�HOnHit�Ɏg�p

    public void OnHit(int dmg, bool crit = false)
    {

        if (enemyDying)//���̏�Ԃ𔻒f�A����ł���̂̓_���[�W�󂯂Ȃ�
        {

            enemyStatus.TakeDamage(dmg);

            //��e�A�j���[�V�����ƃG�t�F�N�g
            if (enemyStatus.IsDead())
            {
                OnDead();
                return;
            }
            if (!isFlashing)
            {
                if (overlayMaterial != null) StartCoroutine(HitFlash());
            }
        }
    }

    private System.Collections.IEnumerator HitFlash()
    {
        ChangeState(EnemyState.Hit);
        isFlashing = true;

        // �S�Ă�Renderer�Ƀ}�e���A����ǉ�
        foreach (Renderer renderer in renderers)
        {
            var materials = renderer.materials;
            var newMaterials = new Material[materials.Length + 1];
            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }
            newMaterials[materials.Length] = overlayMaterial; // �Ō�ɒǉ�
            renderer.materials = newMaterials;
        }
        // �w�莞�ԑҋ@
        yield return new WaitForSeconds(flashDuration);
        // �}�e���A�����폜
        foreach (Renderer renderer in renderers)
        {
            var materials = renderer.materials;
            if (materials.Length > 1)
            {
                var newMaterials = new Material[materials.Length - 1];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = materials[i];
                }
                renderer.materials = newMaterials; // �Ō�̃}�e���A��������
            }
        }

        isFlashing = false;
        ChangeState(EnemyState.Idle);
    }


    private void OnDead()
    {
        ChangeState(EnemyState.Dead);//���S���
        //���S�A�j���[�V�����ƃG�t�F�N�g
        StartCoroutine(DyingAnimation());

    }
    private IEnumerator DyingAnimation()
    {
        float dyingTime = 0f;
        float dyingTimeMax = 0.5f;//0.5�b��폜
        //���S�G�t�F�N�g�𐶐�

        while (dyingTime < dyingTimeMax)
        {
            //�폜�O�̃A�j���[�V�����i��F�i�X�������Ȃ�Ȃǁj

            dyingTime += Time.deltaTime;
            yield return null;
        }
        //EnemyGenerator�ɒʒm
        if (enemyGenerator != null) enemyGenerator.deadEnemyNum++;
        //�A�j���[�V��������������폜
        Destroy(gameObject);
    }

    private void Attack()
    {
        int atk = enemyStatus.GetAttackNow();  //�U���̓_�E���Ȃǂ��v�Z�����U���͂��擾
        //�U����prefab�𐶐�
    }
}

