using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Image hp; // HP�o�[
    public Canvas canvas; // UI�L�����o�X
    public GameObject settingUI; // �ݒ�UI
    public GameObject damageTextPrefab; // �_���[�W�e�L�X�g�̃v���n�u
    public int poolSize = 30; // �I�u�W�F�N�g�v�[���̃T�C�Y

    private Queue<GameObject> damageTextPool; // �_���[�W�e�L�X�g�̃I�u�W�F�N�g�v�[��

    private Camera mainCamera;

    int maxHP = 5;
    int currHP = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDamageTextPool(); // �I�u�W�F�N�g�v�[����������
        }
        else
        {
            Destroy(gameObject); // ���łɑ��݂���ꍇ�͍폜����
        }
    }

    void Start()
    {
        UnableButtons();
    }


    // =====�_���[�W�\��=====
    private void InitializeDamageTextPool()
    {
        damageTextPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(damageTextPrefab);
            obj.transform.SetParent(canvas.transform, false); // �L�����o�X�ɒǉ�
            obj.SetActive(false); // ��A�N�e�B�u��
            damageTextPool.Enqueue(obj); // �L���[�ɒǉ�
        }
    }

    public GameObject GetDamageTextObject()
    {
        if (damageTextPool.Count > 0)
        {
            GameObject obj = damageTextPool.Dequeue();
            obj.SetActive(true); // �A�N�e�B�u��
            return obj;
        }
        else
        {
            // �v�[���ɗ]�T���Ȃ��ꍇ�A�V�����I�u�W�F�N�g���쐬
            GameObject obj = Instantiate(damageTextPrefab);
            obj.transform.SetParent(canvas.transform, false);
            return obj;
        }
    }

    public void ReturnDamageTextObject(GameObject obj)
    {
        obj.SetActive(false); // ��A�N�e�B�u��
        damageTextPool.Enqueue(obj); // �L���[�ɖ߂�
    }

    public void ShowDamage(int damage, Vector3 worldPosition, Color showColor)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main camera is not set!");
            return;
        }

        GameObject damageTextObj = GetDamageTextObject();

        
        DamageDisplay damageDisplay = damageTextObj.GetComponent<DamageDisplay>();
        if (damageDisplay != null)
        {
            damageDisplay.Initialize(damage, worldPosition, showColor);
        }
        else
        {
            Debug.LogError("Missing DamageDisplay component on damage text prefab!");
        }
    }







    // ===== �v���C���[�X�e�[�^�X =====
    public void TakeDamage(int dmg)
    {
        currHP -= dmg;
        UpdateHPBar();
    }

    private void UpdateHPBar() // HP�o�[���X�V
    {
        hp.fillAmount = (float)currHP / (float)maxHP;
        Debug.Log($"Hp{currHP},max{maxHP}");
    }

    public void SetHP(int curr, int max_)
    {
        currHP = curr;
        maxHP = max_;
        UpdateHPBar();
    }




    // ===== UI�Ǘ� =====
    public void AbleButtons()
    {
        settingUI.SetActive(true);
    }

    public void UnableButtons()
    {
        settingUI.SetActive(false);
    }

    public void OnExitGameButtonDown()
    {
        Application.Quit();
    }

    public void OnRetryButtonDown()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void SetMainCamera(Camera cam)
    {
        mainCamera = cam;
    }
}
