using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Hitbox_Sword : MonoBehaviour
{
    public Material critMaterial;
    public Material genMaterial;

    public GameObject hitParticleEffect;
    public GameObject hitParticleShiled;
    public GameObject defaultTrail;
    public GameObject fireTrail;
    public GameObject exolpPrefab;

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // �U�������G���L�^����n�b�V���Z�b�g

    private int damage;
    private float critical=0f;
    private bool isDefensePenetration = false;
    private bool isBleed = false;
    
    bool isDefDown=false;  //�h��͌�
    bool isAtkDown = false; //�U���͌�
    bool isRecover = false;
    bool isExolo = false;
    private CameraFollow camera1;

    private PlayerControl player;
    private Collider hitboxCollider;
    private Vector3 originalColliderSize; // ����Collider�T�C�Y��ۑ�

    public float resetColliderTime = 0f;

    private void OnEnable()
    {
        // �L��������邽�тɋL�^���N���A����
        hitTargets.Clear();

        // Collider�T�C�Y���ꎞ�I��0�ɂ���
        if (hitboxCollider != null)
        {
            StartCoroutine(ResetCollider());
        }
    }
    private void OnDisable()
    {
        hitTargets.Clear();
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        hitboxCollider = GetComponent<Collider>();

        // Collider�̌��̃T�C�Y��ۑ�
        if (hitboxCollider != null)
        {
            originalColliderSize = hitboxCollider.bounds.size;
            hitboxCollider.enabled = true; // Collider��L����
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"SwordHitbox Enter {other}");
        CheckTrigger(other);
    }
    private void OnTriggerStay(Collider other)
    {
       // Debug.Log($"SwordHitbox Stay {other}");
        CheckTrigger(other);
    }

    private void CheckTrigger(Collider other)
    {

        if (other.gameObject.CompareTag("Enemy") && !hitTargets.Contains(other))
        {
            // �U���Ώۂ��L�^����
            hitTargets.Add(other);
            IOnHit io = other.gameObject.GetComponent<IOnHit>();
            if (io != null)
            {
                if (io.IsDying()) return;
                // �N���e�B�J���q�b�g����
                bool crit = Random.Range(0f, 1f) < critical;
                // �_���[�W��^����
                int dmg = io.OnHit(damage, crit, isDefensePenetration, isBleed, isDefDown, isAtkDown, isRecover);

                if (dmg >= 0)
                {
                    camera1.ZoomAndShakeCamera();

                    player.VibrateForDuration();
                    // �q�b�g�����ʒu���擾
                    Vector3 contactPoint = other.ClosestPoint(transform.position);

                    // �G�t�F�N�g�𐶐�
                    GameObject effect = Instantiate(hitParticleEffect, contactPoint, Quaternion.identity);

                    // �G�t�F�N�g�������폜
                    Destroy(effect, 2f);
                }
                else
                {
                    camera1.ZoomAndShakeCamera();

                    player.VibrateForDuration(0.2f,0.3f);
                    // �q�b�g�����ʒu���擾
                    Vector3 contactPoint = other.ClosestPoint(transform.position);

                    // �G�t�F�N�g�𐶐�
                    GameObject effect = Instantiate(hitParticleShiled, contactPoint, Quaternion.identity);

                    // �G�t�F�N�g�������폜
                    Destroy(effect, 2f);
                }
                if(isExolo)
                {
                    Vector3 contactPoint = other.ClosestPoint(transform.position);

                    // �G�t�F�N�g�𐶐�
                    GameObject effect = Instantiate(exolpPrefab, contactPoint, Quaternion.identity);
                    effect.GetComponent<Hitbox_PlayerExplosion>().Initialized(player, camera1, damage);
                }
            }
        }
    }

    public void Initialize(CameraFollow camera_, int dmg, 
        int type, 
        float criRate, 
        bool isDefPen,
        bool isBleed_,   //�����A�R��
        bool isDefDown_,  //�h��͌�
        bool isAtkDown_, //�U���͌�
        bool isRecover_ ,
        bool isExplo_
        )
    {
        camera1 = camera_;
        damage = dmg;
        critical = criRate;
        isDefensePenetration = isDefPen;
        isBleed = isBleed_;
        isDefDown = isDefDown_;
        isAtkDown = isAtkDown_;
        isRecover = isRecover_;
        isExolo = isExplo_;
        switch (type)
        {
            case 0: SetDefaultTrail(); break;
            case 1: SetFireTrail(); break;
        }
    }

    private void SetDefaultTrail()
    {
        isBleed = false;
        defaultTrail.SetActive(true);
        fireTrail.SetActive(false);
    }

    private void SetFireTrail()
    {
        isBleed = true;
        defaultTrail.SetActive(false);
        fireTrail.SetActive(true);
    }

    /// <summary>
    /// Collider�T�C�Y���ꎞ�I��0�ɂ��AresetColliderTime�b��Ɍ��̃T�C�Y�ɖ߂�
    /// </summary>
    private IEnumerator ResetColliderSize()
    {
        // �T�C�Y��0�ɐݒ�
        Vector3 zeroSize = Vector3.zero;

        // BoxCollider�̏ꍇ�A�T�C�Y�𒼐ڐݒ�
        if (hitboxCollider is BoxCollider boxCollider)
        {
            boxCollider.size = zeroSize;
        }
        yield return new WaitForSeconds(resetColliderTime / GameManager.Instance.playerStatus.GetAttackSpeed());

        // ���̃T�C�Y�ɖ߂�
        if (hitboxCollider is BoxCollider boxColliderRestore)
        {
            boxColliderRestore.size = originalColliderSize;
        }

    }
    private IEnumerator ResetCollider()
    {
        hitboxCollider.enabled = false;
        yield return new WaitForSeconds(resetColliderTime / GameManager.Instance.playerStatus.GetAttackSpeed());
        hitboxCollider.enabled = true;
    }
}
