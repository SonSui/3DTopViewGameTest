using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Teki01 : MonoBehaviour, IOnHit
{
    EnemyStatus enemyStatus; //�G�̃X�e�[�^�X

    //Inspector��ݒ�ł���X�e�[�^�X
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
    [SerializeField] private Material overlayMaterial;   // �������ԐF�̃}�e���A��
    private bool isFlashing = false;    // �t���b�V�������ǂ���
    private float flashDuration = 0.1f; // �t���b�V����������

    //�U����Prefab
    public GameObject hitboxPrefab;
    private GameObject hitbox = null;�@//��������Hitbox��ۑ�

    //�X�e�[�^�X�}�V��
    public enum EnemyState
    {
        Idle,         // �ҋ@��ԁF�G���������ɑҋ@���Ă���
        Patrol,       // �����ԁF�w�肳�ꂽ���[�g�⃉���_���Ɉړ����Ă���
        Chase,        // �ǐՏ�ԁF�v���C���[�𔭌����ǂ������Ă���
        Attack,       // �U����ԁF�v���C���[��ڕW���U�����Ă���
        Hit,          // �팂��ԁF�U�����󂯂ă_���[�W���󂯂Ă���

        Dead,         // ���S��ԁF�̗͂��[���ɂȂ�s���s�\
        Stunned,      // �C���ԁF�X�L����U���ōs���s�\�ȏ��
        Flee,         // ������ԁF�v���C���[�ɕ�����Ɣ��f��������
        Alert,        // �x����ԁF�v���C���[�̑��݂ɋC�Â������܂��ǐՂ��Ă��Ȃ�
        Guard,        // �h���ԁF���������
    }

    private EnemyState enemyState;

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
        enemyState = EnemyState.Idle;
    }

    void Start()
    {
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
        if (enemyStatus.IsDead())
        {

        }




        enemyStatus.UpdateStatus(Time.deltaTime);
    }
    public void OnHit(int dmg, bool crit = false)
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
            StartCoroutine(HitFlash());
        }
    }

  



    public void SetGenerator(EnemyGenerator generator)
    {
        enemyGenerator = generator;
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

        isFlashing = false;
    }


    private void OnDead()
    {

        //���S�A�j���[�V�����ƃG�t�F�N�g

        //EnemyGenerator�ɒʒm
        if(enemyGenerator!=null)enemyGenerator.deadEnemyNum++;
        //�A�j���[�V��������������폜
        Destroy(gameObject);
    }

    private void Attack()
    {
        int atk = enemyStatus.GetAttackNow();
        //�U����prefab�𐶐�
    }

}

