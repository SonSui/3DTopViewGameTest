using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Teki04 : MonoBehaviour
{
    EnemyStatus enemyStatus; //�G�̃X�e�[�^�X
    public Animator animator;

    //Inspector��ݒ�ł����{�̃X�e�[�^�X
    public string name_ = "Enemy_Boss01";
    public int hp_ = 4;
    public int attack_ = 1;
    public int defense_ = 1;
    public string type_ = "SpiderBoss";
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
    //[SerializeField] private Material brightMaterial;

    //�U����Prefab
    public GameObject waveHitbox;    //�U����Hitbox�ƍU���̃G�t�F�N�g��prefab�ɂ���
    public GameObject bulletPrefab;
    private GameObject hitbox = null; //��������Hitbox��ۑ�
    public GameObject eye;
    public float attackRange = 1.5f;
    private bool isAttacking = false;
    float atkInterval = 2f;
    float atkTime = 0f;




    //�X�e�[�^�X�}�V��
    public enum EnemyState
    {
        //�S���g���K�v���Ȃ�
        Idle,           // �ҋ@��ԁF�G���������ɑҋ@���Ă���
        //Patrol,         // �����ԁF�w�肳�ꂽ���[�g�⃉���_���Ɉړ����Ă���
        //Chase,          // �ǐՏ�ԁF�v���C���[�𔭌����ǂ������Ă���
        Attack,         // �U����ԁF�v���C���[��ڕW���U�����Ă���
        Hit,            // �팂��ԁF�U�����󂯂ă_���[�W���󂯂Ă���
        Dead,           // ���S��ԁF�̗͂��[���ɂȂ�s���s�\

        Stunned      // �C���ԁF�X�L����U���ōs���s�\�ȏ��
        //Flee,         // ������ԁF�v���C���[�ɕ�����Ɣ��f��������
        //Alert,        // �x����ԁF�v���C���[�̑��݂ɋC�Â������܂��ǐՂ��Ă��Ȃ�
        //Guard,        // �h���ԁF���������
    }
    public EnemyState _state = EnemyState.Idle;


    //�v���C���[�̍��W
    public Transform playerT = null;
    EnemyGenerator enemyGenerator;

    private float stunTime = 0f;
    private float stunTimeMax = 1f;

    public GameObject fireParticle;
    public GameObject shiled;


    public float fallSpeed = 10f; // �������x
    public float groundYPosition = -3.3f; // �n�ʂ�Y���W�i��~���鍂���j

    private float liveTime = 0f;
    public float shiledTime = 20f;
    public int shiledDuration = 10;

    private void Awake()
    {

        // overlayMaterial���ݒ肳��Ă��邩�m�F


        // "low_poly_robot_3d_model_by_niko"�Ƃ������O�̎q�I�u�W�F�N�g��T��
        Transform targetTransform = transform.Find("low_poly_robot_3d_model_by_niko");
        if (targetTransform == null)
        {
            Debug.LogError("�w�肳�ꂽ�I�u�W�F�N�g 'low_poly_robot_3d_model_by_niko' ��������܂���I");
            return;
        }

        renderers = targetTransform.GetComponentsInChildren<Renderer>();

        /*
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
        }*/
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
        liveTime = 0f;
        //playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Start()
    {
        //�G���������Awake->OnEnable(prefab��Enable�̏�Ԃ̏ꍇ)->Start->Update->Update->Update(���t���C���z��)

        if (enemyGenerator == null) enemyGenerator = FindObjectOfType<EnemyGenerator>();

    }

    private void Update()
    {
        // ���݂�Y���ʒu���m�F
        float currentY = transform.position.y;

        // Y���ʒu���n�ʁigroundYPosition�j����Ȃ痎���𑱂���
        if (currentY > groundYPosition)
        {
            // Y�������Ɉړ��i�����j
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
        if (playerT == null)
        {
            playerT = GameObject.FindGameObjectWithTag("Player").transform;
            if (playerT == null) { return; }
        }
        int bleedDmg = enemyStatus.UpdateStatus(Time.deltaTime);//�����A�X�^���A�f�o�t�Ȃǖ��t���C�������I�ɏ���

        if (bleedDmg > 0 && _state != EnemyState.Dead) //�����i�R�āj�_���[�W���o���琔���ŕ\��
        {
            UIManager.Instance.ShowDamage(bleedDmg, transform.position, new Color(0.5f, 0f, 0.5f, 1f));
            if (enemyStatus.IsDead())
            {
                OnDead();
            }
        }
        if (enemyStatus.IsBleeding())
        {
            fireParticle.SetActive(true);
        }
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

                break;
            case EnemyState.Attack:
                OnAttaceState();
                break;
            case EnemyState.Hit:
                waveHitbox.SetActive(false);
                isAttacking = false;
                animator.SetTrigger("Hit");
                break;
            case EnemyState.Dead:
                animator.SetBool("Dead", true);
                animator.SetTrigger("Dead 0");
                break;
            case EnemyState.Stunned:
                stunTime = 0;
                animator.enabled = false;
                break;
        }
    }
    private void OnAttaceState()
    {
        atkTime = atkInterval;
        float distance = Vector3.Distance(transform.position, playerT.position);

        //�v���C���[�̋����v�Z�A�����Ȃ�ǐՁA�߂��Ȃ�U��
        if (distance > attackRange)
        {
            animator.SetTrigger("Attack2");
        }
        else
        {
            animator.SetTrigger("Attack1");
        }
    }
    private void OnIdle()
    {
        liveTime += Time.deltaTime;
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
        if (liveTime > shiledTime)
        {
            if (!enemyStatus.HasShield())
            {
                enemyStatus.SetShield(true);
                enemyStatus.SetShieldDurability(shiledDuration);
                liveTime = 0;
            }
        }

    }

    private void OnStuned()
    {
        stunTime += Time.deltaTime;
        if (stunTime < stunTimeMax) return;
        else
        {
            animator.enabled = true;
            ChangeState(EnemyState.Idle);
        }
    }
    private System.Collections.IEnumerator HitFlash()
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
        isFlashing = false;�@//��e�̍s���I��

    }


    private void OnDead()
    {
        if (IsDying()) return;
        ChangeState(EnemyState.Dead);//���S���
        //���S�A�j���[�V�����ƃG�t�F�N�g
        StartCoroutine(DyingAnimation());

    }
    private IEnumerator DyingAnimation()
    {
        float dyingTime = 0f;
        float dyingTimeMax = 5.5f;//5�b��폜
        //���S�G�t�F�N�g�𐶐�

        while (dyingTime < dyingTimeMax)
        {
            //�폜�O�̃A�j���[�V�����i��F�������Ȃ�Ȃǁj

            dyingTime += Time.deltaTime;
            yield return null;
        }
        //EnemyGenerator�ɒʒm
        if (enemyGenerator == null) enemyGenerator = FindObjectOfType<EnemyGenerator>();
        enemyGenerator.EnemyDead(gameObject);
        Debug.Log("BossDead");
        //�A�j���[�V��������������폜
        //Destroy(gameObject);
    }






    // =====�@�O���C���^���N�V�����@=====
    public void Initialize(
        string name = "Enemy_Boss01",
        int hpMax = 4,
        int attackPower = 1,
        int defense = 1,
        string enemyType = "SpiderBoss",
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
    public void OnAttack1Anime()
    {
        //�U��AnimationEvent
        if (isAttacking) return;
        isAttacking = true;
        waveHitbox.SetActive(true);�@//Hitbox�L����
        waveHitbox.GetComponent<Hitbox_Boss01_wave>().Initialized(enemyStatus.GetAttackNow() * 2); //�U���͐ݒ�

    }
    public void OnAttack1Over()
    {
        isAttacking = false;
        ChangeState(EnemyState.Idle);
    }
    public void OnAttack2Anime()
    {
        isAttacking = true;
        GameObject bullet = Instantiate(bulletPrefab, eye.transform.position, Quaternion.identity);
        Vector3 eyeEulerAngles = eye.transform.eulerAngles;
        bullet.transform.rotation = Quaternion.Euler(0, eyeEulerAngles.y, 0);
        bullet.GetComponent<Hitbox_Boss01_Bullet>().Initialize(playerT, enemyStatus.GetAttackNow());
    }
    public void OnAttack2Over()
    {
        isAttacking = false;
        ChangeState(EnemyState.Idle);
    }
    public void OnIdleAnime()
    {
        ChangeState(EnemyState.Idle);
    }
    public void OnHitAnimeOver()
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
            if (overlayMaterial != null) StartCoroutine(HitFlash());
            return hitDmg;
        }
        return 0;
    }
    public void OnHooked(int dmg)
    {
        Color displayColor = Color.red;
        //�t�b�N�V���b�g�ɓ�����s���i�V�[���h�j��j
        if (enemyStatus.HasShield())
        {
            enemyStatus.SetShield(false, 0);
            displayColor = Color.blue;
        }
        ChangeState(EnemyState.Stunned);
        int hitDmg = enemyStatus.TakeDamage(dmg);
        if (hitDmg != 0)
        {

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
