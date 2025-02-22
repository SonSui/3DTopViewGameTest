using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01Feet : MonoBehaviour, IOnHit
{
    public Animator animator;
    public Enemy_Boss01 boss;

    private Collider collider1; 
    private float triggerDuration = 1.55f; // Trigger��Ԃ̎�������
    private float recoverDuration = 0.5f; // Collider�����̃T�C�Y�ɖ߂���������
    private Vector3 shrinkSize = new Vector3(0.001f, 0.001f, 0.001f); // �k�����̃T�C�Y

    private Vector3 originalSize; // ���̃T�C�Y��ۑ�



    void Start()
    {
        // �Ώۂ�Collider���擾�iBoxCollider��O��ɂ��Ă��܂��j
        collider1 = GetComponent<Collider>();
        if (collider1 is BoxCollider boxCollider)
        {
            originalSize = boxCollider.size; // ���̃T�C�Y���L�^
        }
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
        int hitDmg = boss.OnHit(dmg, crit, isPenetrate, isBleed, isDefDown, isAtkDown, isRecover);
        if (hitDmg != 0)
        {
            animator.SetTrigger("Hit");
        }
        return hitDmg;
    }
    public void OnHooked(int dmg)
    {
        boss.OnHooked(dmg);
    }
    public bool IsDying()
        { return boss.IsDying(); }
    public void Initialize(
        string name = "Enemy",
        int hpMax = 3,
        int attackPower = 1,
        int defense = 1,
        string enemyType = "Normal",
        bool hasShield = false,
        int shieldDurability = 0,

        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f)
    { }


    public void ActivateColliderSequence()
    {
        StartCoroutine(ColliderSequence());
    }

    // ��A�̏����iTrigger�ݒ�A�k���A�T�C�Y�����j�����s����R���[�`��
    private IEnumerator ColliderSequence()
    {
        // Trigger��Ԃɂ���
        collider1.isTrigger = true;

        // �U���I���܂őҋ@
        yield return new WaitForSeconds(triggerDuration);

        if (collider1 is BoxCollider boxCollider)
        {
            boxCollider.size = shrinkSize;
        }
        collider1.isTrigger = false;

        if (collider1 is BoxCollider boxColliderRecover)
        {
            float elapsedTime = 0f;
            Vector3 startSize = boxColliderRecover.size; // ���݂̃T�C�Y���J�n�T�C�Y�Ƃ��ċL�^

            while (elapsedTime < recoverDuration)
            {
                // �T�C�Y����`��Ԃŏ��X�Ɍ��ɖ߂�
                boxColliderRecover.size = Vector3.Lerp(startSize, originalSize, elapsedTime / recoverDuration);
                elapsedTime += Time.deltaTime;
                yield return null; 
            }

            // ���̃T�C�Y�ɐݒ�
            boxColliderRecover.size = originalSize;
        }
    }
}
