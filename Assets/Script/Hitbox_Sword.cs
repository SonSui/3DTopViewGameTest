using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Sword : MonoBehaviour
{
    public Material critMaterial;
    public Material genMaterial;

    public GameObject hitParticleEffect;
    public GameObject defaultTrail;
    public GameObject fireTrail;

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // �U�������G���L�^����n�b�V���Z�b�g

    private int damage;
    private float critical;
    private bool isDefensePenetration;
    private bool isBleed;
    private CameraFollow camera1;

    private PlayerControl player;
    private Collider hitboxCollider;
    private Vector3 originalColliderSize; // ����Collider�T�C�Y��ۑ�

    private void OnEnable()
    {
        // �L��������邽�тɋL�^���N���A����
        hitTargets.Clear();

        // Collider�T�C�Y���ꎞ�I��0�ɂ���
        if (hitboxCollider != null)
        {
            StartCoroutine(ResetColliderSize());
        }
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
        Debug.Log($"Hit {other}");
        if (other.gameObject.CompareTag("Enemy") && !hitTargets.Contains(other))
        {
            // �U���Ώۂ��L�^����
            hitTargets.Add(other);
            if (other.gameObject.GetComponent<IOnHit>() != null)
            {
                // �N���e�B�J���q�b�g����
                bool crit = Random.Range(0f, 1f) < critical;
                // �_���[�W��^����
                int dmg = other.gameObject.GetComponent<IOnHit>().OnHit(damage, crit, isDefensePenetration, isBleed);

                if (dmg != 0)
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
            }
        }
    }

    public void Initialize(CameraFollow camera_, int dmg, int type = 0, float criRate = 0.01f, bool isDefPen = false)
    {
        camera1 = camera_;
        damage = dmg;
        critical = criRate;
        isDefensePenetration = isDefPen;
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
    /// Collider�T�C�Y���ꎞ�I��0�ɂ��A0.1�b��Ɍ��̃T�C�Y�ɖ߂�
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
        

        // 0.1�b�҂�
        yield return new WaitForSeconds(0.05f);

        // ���̃T�C�Y�ɖ߂�
        if (hitboxCollider is BoxCollider boxColliderRestore)
        {
            boxColliderRestore.size = originalColliderSize;
        }
        
    }
}
