using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
<<<<<<< HEAD
    public GameObject hpBarContainer; // HP�o�[�R���e�i
    public GameObject hpSegmentPrefab; // HP�Z�O�����g�v���n�u
    public GameObject ammoBarContainer; // �e��o�[�R���e�i
    public GameObject ammoSegmentPrefab; // �e��Z�O�����g�v���n�u
=======
    public Image hp; // HP�o�[
>>>>>>> origin/main
    public Canvas canvas; // UI�L�����o�X
    public GameObject settingUI; // �ݒ�UI
    public GameObject damageTextPrefab; // �_���[�W�e�L�X�g�̃v���n�u
    public int poolSize = 30; // �I�u�W�F�N�g�v�[���̃T�C�Y

    private Queue<GameObject> damageTextPool; // �_���[�W�e�L�X�g�̃I�u�W�F�N�g�v�[��
<<<<<<< HEAD
    private List<GameObject> hpSegments; // ���݂�HP�Z�O�����g�̃��X�g
    private List<GameObject> ammoSegments; // ���݂̒e��Z�O�����g�̃��X�g
=======
>>>>>>> origin/main

    private Camera mainCamera;

    int maxHP = 5;
    int currHP = 5;

<<<<<<< HEAD
    int maxAmmo = 10;
    int currAmmo = 10;

    private const float hpSegmentSpacing = 65f; // HP�Z�O�����g�Ԃ̋���
    private const float hpBarHeight = 200f; // HP�o�[�̍���

    private const float ammoSegmentSpacing = 30f; // �e��Z�O�����g�Ԃ̋���
    private const float ammoBarHeight = 200f; // �e��o�[�̍���

=======
>>>>>>> origin/main
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDamageTextPool(); // �I�u�W�F�N�g�v�[����������
<<<<<<< HEAD
            hpSegments = new List<GameObject>();
            ammoSegments = new List<GameObject>();
            UpdateHPBar(); // ����HP�o�[�̐ݒ�
            UpdateAmmoBar(); // �����e��o�[�̐ݒ�
            UnableButtons();
=======
>>>>>>> origin/main
        }
        else
        {
            Destroy(gameObject); // ���łɑ��݂���ꍇ�͍폜����
        }
    }

<<<<<<< HEAD
=======
    void Start()
    {
        UnableButtons();
    }

>>>>>>> origin/main

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

<<<<<<< HEAD
=======
        
>>>>>>> origin/main
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

<<<<<<< HEAD
=======






>>>>>>> origin/main
    // ===== �v���C���[�X�e�[�^�X =====
    public void TakeDamage(int dmg)
    {
        currHP -= dmg;
<<<<<<< HEAD
        if (currHP < 0) currHP = 0;
        UpdateHPBar();
    }

    public void Heal(int healAmount)
    {
        currHP += healAmount;
        if (currHP > maxHP) currHP = maxHP;
=======
>>>>>>> origin/main
        UpdateHPBar();
    }

    private void UpdateHPBar() // HP�o�[���X�V
    {
<<<<<<< HEAD
        if (hpBarContainer == null || hpSegmentPrefab == null)
        {
            Debug.LogError("hpBarContainer �܂��� hpSegmentPrefab ���ݒ肳��Ă��܂���I");
            return;
        }

        // HP�o�[�R���e�i�̃T�C�Y�ƈʒu���X�V
        RectTransform barRect = hpBarContainer.GetComponent<RectTransform>();
        if (barRect != null)
        {
            // �����v�Z
            float newWidth = 90 + hpSegmentSpacing * (maxHP - 1);
            barRect.sizeDelta = new Vector2(newWidth, hpBarHeight);

            // �ʒu���v�Z
            float newXPos = 60 + (hpSegmentSpacing / 2f) * (maxHP - 1);
            barRect.anchoredPosition = new Vector2(newXPos, -100);
        }

        // �����̃Z�O�����g���N���A
        foreach (GameObject segment in hpSegments)
        {
            if (segment != null)
            {
                Destroy(segment);
            }
        }
        hpSegments.Clear();

        // ���݂�HP�Z�O�����g���쐬
        for (int i = 0; i < maxHP; i++)
        {
            GameObject segment = Instantiate(hpSegmentPrefab, hpBarContainer.transform);
            segment.SetActive(true);

            // �Z�O�����g�̈ʒu��ݒ�
            RectTransform rectTransform = segment.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float xPos = -hpSegmentSpacing * (maxHP - 1) / 2f + i * hpSegmentSpacing;
                rectTransform.anchoredPosition = new Vector2(xPos, -3); // Y����-3�ɐݒ�
                rectTransform.sizeDelta = new Vector2(110, 110); // �Z�O�����g�̃T�C�Y��ݒ�
            }

            // �Z�O�����g�̏�Ԃ�ݒ�
            Image segmentImage = segment.GetComponent<Image>();
            if (segmentImage != null)
            {
                segmentImage.color = (i < currHP) ? Color.white : Color.red; // ���݂�HP�Ƌ��HP
            }

            hpSegments.Add(segment);
        }
=======
        hp.fillAmount = (float)currHP / (float)maxHP;
        Debug.Log($"Hp{currHP},max{maxHP}");
>>>>>>> origin/main
    }

    public void SetHP(int curr, int max_)
    {
        currHP = curr;
        maxHP = max_;
<<<<<<< HEAD
        if (currHP > maxHP) currHP = maxHP;
        UpdateHPBar();
    }

    public void SetAmmo(int curr, int max_)
    {
        currAmmo = curr;
        maxAmmo = max_;
        if (currAmmo > maxAmmo) currAmmo = maxAmmo;
        UpdateAmmoBar();
    }

    private void UpdateAmmoBar() // �e��o�[���X�V
    {
        if (ammoBarContainer == null || ammoSegmentPrefab == null)
        {
            Debug.LogError("ammoBarContainer �܂��� ammoSegmentPrefab ���ݒ肳��Ă��܂���I");
            return;
        }

        // �e��o�[�R���e�i�̃T�C�Y�ƈʒu���X�V
        RectTransform barRect = ammoBarContainer.GetComponent<RectTransform>();
        if (barRect != null)
        {
            // �����v�Z
            float newWidth = 45 + ammoSegmentSpacing * (maxAmmo - 1);
            barRect.sizeDelta = new Vector2(newWidth, ammoBarHeight);

            // �ʒu���v�Z
            float newXPos = 37 + (ammoSegmentSpacing / 2f) * (maxAmmo - 1);
            barRect.anchoredPosition = new Vector2(newXPos, -220);
        }

        // �����̃Z�O�����g���N���A
        foreach (GameObject segment in ammoSegments)
        {
            if (segment != null)
            {
                Destroy(segment);
            }
        }
        ammoSegments.Clear();

        // ���݂̒e��Z�O�����g���쐬
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject segment = Instantiate(ammoSegmentPrefab, ammoBarContainer.transform);
            segment.SetActive(true);

            // �Z�O�����g�̈ʒu��ݒ�
            RectTransform rectTransform = segment.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float xPos = -ammoSegmentSpacing * (maxAmmo - 1) / 2f + i * ammoSegmentSpacing;
                rectTransform.anchoredPosition = new Vector2(xPos, -3); // Y����-3�ɐݒ�
                rectTransform.sizeDelta = new Vector2(80, 110); // �Z�O�����g�̃T�C�Y��ݒ�
            }

            // �Z�O�����g�̏�Ԃ�ݒ�
            Image segmentImage = segment.GetComponent<Image>();
            if (segmentImage != null)
            {
                segmentImage.color = (i < currAmmo) ? Color.white : Color.red; // ���݂̒e��Ƌ�̒e��
            }

            ammoSegments.Add(segment);
        }
    }
=======
        UpdateHPBar();
    }



>>>>>>> origin/main

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
<<<<<<< HEAD
    public void OnGameStart()
    {
        SceneManager.LoadScene("Tutorial");
    }

=======
>>>>>>> origin/main
    public void SetMainCamera(Camera cam)
    {
        mainCamera = cam;
    }
}
