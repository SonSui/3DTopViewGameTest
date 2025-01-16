using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Boss01_Bullet : MonoBehaviour
{
    public float speed = 10f; // �q�e�̈ړ����x
    public float heightAdjustmentSpeed = 8f; // Y���i�����j��ڕW�ɋ߂Â��鑬�x
    private Transform playerT; // �v���C���[��Transform��ێ�����ϐ�
    public float lifetime = 10f;
    private int dmg = 1;
    public float preAtkTime = 0.1f;
    public float lifeTime = 1f;
    public float currTime = 0f;
    public ParticleSystem particleSystem;

    private bool isHitted = false;

    private void OnEnable()
    {
        Destroy(gameObject, lifetime);


        isHitted = false;
        currTime = 0f;

        
        particleSystem.Clear(); //�G�t�F�N�g�����Z�b�g
        particleSystem.Play();

    }



    // �q�e�����������郁�\�b�h
    public void Initialize(Transform playerTarget,int dmg_)
    {
        playerT = playerTarget; // �v���C���[��Transform��ݒ�
        this.dmg = dmg_;
    }

    private void Update()
    {
        if (playerT == null)
        {
            playerT = GameObject.FindGameObjectWithTag("Player").transform;
            // �ڕW���ݒ肳��Ă��Ȃ��ꍇ�A�����I�ɑO�i
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        // �v���C���[��Y���ʒu���擾
        float targetY = playerT.position.y+1;

        // ���݂̎q�e�̈ʒu���擾
        Vector3 currentPosition = transform.position;

        // Y���̍�����ڕW�Ɍ����ĕ����ɒ���
        float newY = Mathf.MoveTowards(currentPosition.y, targetY, heightAdjustmentSpeed * Time.deltaTime);

        // �V�����ʒu��ݒ�iY���𒲐������ʒu�j
        transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);

        // �q�e��Z�������Ɉړ�
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isHitted)
        {
            other.GetComponent<PlayerControl>().OnHit(dmg);
            isHitted = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isHitted)
        {
            other.GetComponent<PlayerControl>().OnHit(dmg);
            isHitted = true;
        }
    }

    public void Initialized(int dmg_)
    {
        dmg = dmg_;
    }
}
