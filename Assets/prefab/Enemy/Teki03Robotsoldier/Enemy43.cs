using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Teki03 : MonoBehaviour
{
    EnemyStatus enemyStatus; //�G�̃X�e�[�^�X
    public Animator animator;

    //Inspector��ݒ�ł����{�̃X�e�[�^�X
    public string name_ = "Enemy_Teki03";
    public int hp_ = 10;
    public int attack_ = 1;
    public int defense_ = 1;
    public string type_ = "Solder";
    public bool hasShiled_ = false;
    public int shieldDurability_ = 0;
    public float moveSpeed_ = 1.0f;
    public float attackSpeed_ = 1.0f;


    //��e�̐F�ω�
    private Renderer[] renderers;        // �G��Renderer���X�g
    [SerializeField] private Material overlayMaterial;   // ��e�̃}�e���A��
    private bool isFlashing = false;    // �t���b�V�������ǂ���
    private float flashDuration = 0.1f; // �t���b�V����������

    //�����߃V�F�[�_�[
    [SerializeField] private Material brightMaterial;

    //�U����Prefab
    public GameObject biteHitbox;    //�U����Hitbox�ƍU���̃G�t�F�N�g��prefab�ɂ���
    public GameObject bombPrefab;
    private GameObject hitbox = null; //��������Hitbox��ۑ�
    public float attackRange = 1.5f;
    private bool isAttacking = false;
    float atkInterval = 1f;
    float atkTime = 0f;


    private bool enemyDying;//Enemy�͎���ł��邩�HOnHit�Ɏg�p

    //�X�e�[�^�X�}�V��
    public enum EnemyState
    {
        //�S���g���K�v���Ȃ�
        Idle,           // �ҋ@��ԁF�G���������ɑҋ@���Ă���
        //Patrol,         // �����ԁF�w�肳�ꂽ���[�g�⃉���_���Ɉړ����Ă���
        Chase,          // �ǐՏ�ԁF�v���C���[�𔭌����ǂ������Ă���
        Attack,         // �U����ԁF�v���C���[��ڕW���U�����Ă���
        Hit,            // �팂��ԁF�U�����󂯂ă_���[�W���󂯂Ă���
        Dead,           // ���S��ԁF�̗͂��[���ɂȂ�s���s�\

        //Bomb,           // ������ԁF���G�̓���\��

        Stunned      // �C���ԁF�X�L����U���ōs���s�\�ȏ��
        //Flee,         // ������ԁF�v���C���[�ɕ�����Ɣ��f��������
        //Alert,        // �x����ԁF�v���C���[�̑��݂ɋC�Â������܂��ǐՂ��Ă��Ȃ�
        //Guard,        // �h���ԁF���������
    }
    public EnemyState _state = EnemyState.Idle;


    //�v���C���[�̍��W
    public Transform playerT;
    EnemyGenerator enemyGenerator;

    private float stunTime = 0f;
    private float stunTimeMax = 1f;

    public GameObject fireParticle;
    public GameObject shiled;

    private void Awake()
    {

        // overlayMaterial���ݒ肳��Ă��邩�m�F
        if (overlayMaterial == null)
        {
            Debug.LogError("Overlay Material ���ݒ肳��Ă��܂���I");
            return;
        }

        // "teki01_test"�Ƃ������O�̎q�I�u�W�F�N�g��T��
        Transform targetTransform = transform.Find("teki01_test");
        if (targetTransform == null)
        {
            Debug.LogError("�w�肳�ꂽ�I�u�W�F�N�g 'teki01_test' ��������܂���I");
            return;
        }

        renderers = targetTransform.GetComponentsInChildren<Renderer>();

        //���邳����
        foreach (Renderer renderer in renderers)
        {
            // ���݂̃I�u�W�F�N�g�̑S�Ẵ}�e���A�����擾
            var materials = renderer.materials;

            // �V�����}�e���A���z����쐬�i�����̃}�e���A���ɉ����Ė��邳�p�̃}�e���A����ǉ��j
            var newMaterials = new Material[materials.Length + 1];
            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }
            newMaterials[materials.Length] = brightMaterial; // �����p�}�e���A����ǉ�
            renderer.materials = newMaterials;
        }
    }
    private void OnEnable()
    {
        name_ += System.Guid.NewGuid().ToString(); //�B��̖��O�t����

        //EnemyPool���g���Ȃ�AEnable���тɃX�e�[�^�X�����Z�b�g����
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
        if (fireParticle != null) fireParticle.SetActive(false);
        if (shiled != null) shiled.SetActive(false);
    }
    void Start()
    {
        //�G���������Awake->OnEnable(prefab��Enable�̏�Ԃ̏ꍇ)->Start->Update->Update->Update(���t���C���z��)

        if (enemyGenerator == null) enemyGenerator = FindObjectOfType<EnemyGenerator>();
        playerT = GameObject.FindGameObjectWithTag("Player").transform;

    }

    private void Update()
    {
        int bleedDmg = enemyStatus.UpdateStatus(Time.deltaTime);//�����A�X�^���A�f�o�t�Ȃǖ��t���C�������I�ɏ���

        if (bleedDmg > 0 && _state != EnemyState.Dead) //�����i�R�āj�_���[�W���o���琔���ŕ\��
        {
            UIManager.Instance.ShowDamage(bleedDmg, transform.position, new Color(0.5f, 0f, 0.5f, 1f));
            if (enemyStatus.IsDead())
            {
                OnDead();
            }
        }
        if (enemyStatus.IsBleeding()) fireParticle.SetActive(true);
        else fireParticle.SetActive(false);
        if (enemyStatus.HasShield()) shiled.SetActive(true);
        else shiled.SetActive(false);

        //��ԍX�V
        StateUpdate();
    }



    // ===== �X�e�[�g���� =====
    private void StateUpdate()
    {
        switch (_state)
        {
            case EnemyState.Idle:
                OnIdle();// �ҋ@���̏���
                break;
            case EnemyState.Chase:
                OnChase();// �ǐՂ̏���
                break;

            //�ȉ��̍s����AnimationEvent�⑼�̃I�u�W�F�N�g���Ă�ł����
            case EnemyState.Attack:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;
            case EnemyState.Stunned:
                OnStuned();
                break;
        }
    }
    private void ChangeState(EnemyState nextState)
    {
        _state = nextState;
        //��ԕύX������A�A�j���[�V�������ύX
        switch (nextState)
        {
            case EnemyState.Idle:
                animator.SetBool("Chase", false);
                break;
            case EnemyState.Chase:
                animator.SetBool("Chase", true);
                break;
            case EnemyState.Attack:
                animator.SetTrigger("Bite");
                atkTime = atkInterval;
                break;
            case EnemyState.Hit:
                biteHitbox.SetActive(false);
                isAttacking = false;
                animator.SetTrigger("Hit");
                break;
            case EnemyState.Dead:
                animator.SetTrigger("Dead");
                break;
            case EnemyState.Stunned:
                stunTime = 0;
                animator.SetBool("Chase", false);
                break;
        }
    }
    private void OnIdle()
    {
        float distance = Vector3.Distance(transform.position, playerT.position);

        //�v���C���[�̋����v�Z�A�����Ȃ�ǐՁA�߂��Ȃ�U��
        if (distance > attackRange)
        {
            ChangeState(EnemyState.Chase);
        }
        else
        {
            //�U���Ԋu�̔��f
            if (atkTime > 0)
            {
                atkTime -= Time.deltaTime;
            }
            else
            {
                atkTime = 0;
                ChangeState(EnemyState.Attack);
            }
        }
    }
    private void OnChase()
    {
        if (playerT != null)
        {


            //�ړ�
            transform.position = Vector3.MoveTowards(transform.position, playerT.position, enemyStatus.GetMoveSpeed() * Time.deltaTime);
            float distance = Vector3.Distance(transform.position, playerT.position);

            //��]
            Vector3 direction = (playerT.position - transform.position).normalized;
            direction.y = 0;
            if (direction.magnitude > 0.01)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            //��ԕύX
            if (distance <= attackRange)
            {
                //�U���Ԋu�̔��f
                if (atkTime > 0)
                {
                    atkTime -= Time.deltaTime;
                    ChangeState(EnemyState.Idle);
                }
                else
                {
                    atkTime = 0;
                    ChangeState(EnemyState.Attack);
                }
            }
        }
    }

    private void OnStuned()
    {
        if (stunTime < stunTimeMax) return;
        else ChangeState(EnemyState.Idle);
    }
    private System.Collections.IEnumerator HitFlash(bool isBomb)
    {
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

        //��e�̃A�j���[�V�������I�����������m�F��
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95) yield return null;

        isFlashing = false;�@//��e�̍s���I��

        //if (isBomb) ChangeState(EnemyState.Bomb);// ��ʂ̓G�͑ҋ@��Ԃɖ߂��B���͔���

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
        float dyingTimeMax = 2f;//2�b��폜
        //���S�G�t�F�N�g�𐶐�

        while (dyingTime < dyingTimeMax)
        {
            //�폜�O�̃A�j���[�V�����i��F�������Ȃ�Ȃǁj

            dyingTime += Time.deltaTime;
            yield return null;
        }
        //EnemyGenerator�ɒʒm
        if (enemyGenerator != null) enemyGenerator.EnemyDead(gameObject);
        //�A�j���[�V��������������폜
        Destroy(gameObject);
    }


    private System.Collections.IEnumerator Attack()
    {
        //��e�̃A�j���[�V�������I�����������m�F��
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95) yield return null;
        biteHitbox.SetActive(false);
        isAttacking = false;

        //�ҋ@��Ԃɖ߂�
        ChangeState(EnemyState.Idle);
    }



    // =====�@�O���C���^���N�V�����@=====
    public void Initialize(
        string name = "Enemy_Teki01",
        int hpMax = 4,
        int attackPower = 1,
        int defense = 1,
        string enemyType = "SuicideBomb",
        bool hasShield = false,
        int shieldDurability = 0,

        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f
        )
    {
        enemyStatus = new EnemyStatus(name, hpMax, attackPower, defense, enemyType, hasShield, shieldDurability, moveSpeed, attackSpeed);
    }
    public void SetGenerator(EnemyGenerator generator)
    {
        enemyGenerator = generator;
    }
    public void OnBiteAnime()
    {
        //�U��AnimationEvent
        if (isAttacking) return;
        isAttacking = true;
        biteHitbox.SetActive(true);�@//Hitbox�L����
        biteHitbox.GetComponent<Hitbox_Teki01_Bite>().Initialized(enemyStatus.GetAttackNow());�@//�U���͐ݒ�
        StartCoroutine(Attack());�@//��ԏ���
    }
    public void OnBombAnime()
    {
        isAttacking = true;
        GameObject bomb = Instantiate(bombPrefab, transform);�@//����Hitbox����
        bomb.GetComponent<Hitbox_Teki01_Bomb>().Initialized(enemyStatus.GetAttackNow());
        //���������玀��
        ChangeState(EnemyState.Dead);
        OnDead();
    }
    public void OnIdleAnime()
    {
        ChangeState(EnemyState.Idle);
    }




    public int OnHit(
    int dmg,                //�_���[�W
    bool crit = false,      //�N���e�B�J��
    bool isPenetrate = false, //�h��ђ�
    bool isBleed = false,   //�����A�R��
    bool isDefDown = false,  //�h��͌�
    bool isAtkDown = false, //�U���͌�
    bool isRecover = false  //HP��
    )
    {

        if (_state != EnemyState.Dead && !enemyStatus.IsDead())//���̏�Ԃ𔻒f�A����ł���̂̓_���[�W�󂯂Ȃ�
        {
            bool isBomb = false;
            if (crit)
            {
                //�N���e�B�J���G�t�F�N�g�i����΁j
            }
            if (isBleed) { enemyStatus.ApplyBleedingEffect(5f); }
            if (isDefDown) { enemyStatus.ApplyDefenseReduction(5f); }
            if (isAtkDown) { enemyStatus.ApplyAttackReduction(5f); }

            int hitDmg = enemyStatus.TakeDamage(dmg, isPenetrate);//�h��͂Ȃǂ̉e�����܂߂ă_���[�W�v�Z�ł���
            if (hitDmg != 0)
            {
                Color displayColor = Color.red;
                if (hitDmg < 0)
                {
                    displayColor = Color.white;
                    hitDmg = -hitDmg;
                }
                else
                {
                    //��e�A�j���[�V�����ƃG�t�F�N�g
                    isBomb = true;
                    if (enemyStatus.IsDead())
                    {
                        OnDead();
                        if (isRecover)
                        {
                            //�񕜃G�t�F�N�g
                            GameManager.Instance.RecoverHP();
                        }

                    }
                    else ChangeState(EnemyState.Hit); // �V�[���h�Ȃ��Ȃ�팂���
                }
                //Vector3 worldPosition = transform.position + Vector3.up * 1; // �e�L�X�g�\���ʒu

                UIManager.Instance.ShowDamage(hitDmg, transform.position, displayColor);
                Debug.Log($"Enemy��{hitDmg}�_���[�W�󂯂�");
            }

            if (isFlashing) StopCoroutine("HitFlash");
            if (overlayMaterial != null) StartCoroutine(HitFlash(isBomb));
            return hitDmg;
        }
        return 0;
    }
    public void OnHooked(int dmg)
    {
        //�t�b�N�V���b�g�ɓ�����s���i�V�[���h�j��j
        if (enemyStatus.HasShield())
        {
            enemyStatus.SetShield(false, 0);
        }
        ChangeState(EnemyState.Stunned);
        int hitDmg = enemyStatus.TakeDamage(dmg);
        if (hitDmg != 0)
        {
            Color displayColor = Color.red;
            if (hitDmg < 0)
            {
                displayColor = Color.white;
                hitDmg = -hitDmg;
            }
            else
            {
                //��e�A�j���[�V�����ƃG�t�F�N�g

                if (enemyStatus.IsDead())
                {
                    OnDead();
                }
            }
            //Vector3 worldPosition = transform.position + Vector3.up * 1; // �e�L�X�g�\���ʒu

            UIManager.Instance.ShowDamage(hitDmg, transform.position, displayColor);
            Debug.Log($"Enemy��{hitDmg}�_���[�W�󂯂�");
        }
    }

    public bool IsDying()
    {
        return _state == EnemyState.Dead;
    }
}
