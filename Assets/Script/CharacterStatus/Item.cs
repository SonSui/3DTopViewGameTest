using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("�����f�[�^")]
    public ItemData itemData; // �����f�[�^�i�A�C�e���̑������Ǘ��j

    [Header("�r�W���A���G�t�F�N�g")]
    [SerializeField] private List<Material> rareMaterials; // ���A���e�B�ɉ������}�e���A�����X�g
    private MeshRenderer rareRenderer;                     // ���b�V�������_���[
<<<<<<< HEAD
    public Material brightCircle;
=======
>>>>>>> origin/main

    // �A�j���[�V�����֘A
    private float rotSpd = 45f;            // ��]���x
    private Vector3 oriScale;              // ���̃X�P�[��
    private float ableTime = 0f;           // �X�P�[���ύX�̌o�ߎ���
    private float sizeChangeTime = 2f;     // �X�P�[���ύX�̎���
    private bool isEquipped = false;       // �����ς݃t���O

    private void Update()
    {
        // �A�C�e������]
        transform.Rotate(Vector3.up, rotSpd * Time.deltaTime, Space.World);

        // �X�P�[���ύX�A�j���[�V����
        if (transform.localScale != oriScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, oriScale, ableTime / sizeChangeTime);
            ableTime += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        oriScale = transform.localScale; // ���̃X�P�[����ۑ�
        transform.localScale = Vector3.zero; // �X�P�[�����[���ɐݒ�
        ableTime = 0f; // ���Ԃ����Z�b�g


        //Test ��ō폜
        InitializeItem(itemData);
    }


    public void InitializeItem(ItemData itemData_)
    {
        // �����f�[�^��ݒ�
        this.itemData = itemData_;

        // ���A���e�B��ݒ�i�͈͊O�̏ꍇ�̓f�t�H���g�ɐݒ�j
        itemData.rare = (itemData.rare >= 0 && itemData.rare < rareMaterials.Count) ? itemData.rare : 0;

        // ���b�V�������_���[���擾�܂��͍쐬
        rareRenderer = GetComponent<MeshRenderer>();
        if (rareRenderer == null)
        {
            Debug.LogWarning("MeshRenderer��������Ȃ����߁A�f�t�H���g�̍ގ����g�p���܂��B");
            rareRenderer = gameObject.AddComponent<MeshRenderer>();
            rareRenderer.material = rareMaterials[0];
        }

        // ���A���e�B�ɉ������}�e���A����ݒ�
        rareRenderer.material = rareMaterials[itemData.rare];
        Color materialColor = rareMaterials[itemData.rare].color;

        // �q�I�u�W�F�N�g�̃p�[�e�B�N���V�X�e���̐F��ݒ�
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            var mainModule = ps.main;
            mainModule.startColor = materialColor;
        }

<<<<<<< HEAD
        if (brightCircle != null)
        {
            brightCircle.SetColor("_Color", materialColor);
        }
        else
        {
            Debug.LogWarning("brightCircle�}�e���A�����ݒ肳��Ă��܂���B");
        }

=======
>>>>>>> origin/main
        // �A�C�e����L����
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�ƐڐG���A�܂���������Ă��Ȃ��ꍇ
        if (other.gameObject.tag == "Player" && isEquipped == false)
        {
            Debug.Log($"{itemData.itemName} in items");
            GameManager.Instance.EquipItem(itemData); // ������GameManager�ɒǉ�
            isEquipped = true; // �����ς݃t���O��ݒ�
            StartCoroutine(OnEquipped()); // ������̏������J�n
        }
    }

    
    private IEnumerator OnEquipped()
    {
        float time = 0f;
        float resizeDuration = 0.5f; // �k���A�j���[�V�����̎���

        // �A�j���[�V���������s
        while (time < resizeDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, time / resizeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // �A�C�e�����폜
        Destroy(gameObject);
    }
}