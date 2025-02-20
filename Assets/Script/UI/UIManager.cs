using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject failPanel;
    public GameObject continuePanel;
    public GameObject continuePanel2;
    public GameObject tutorialPanel;
    public GameObject tagIconsParent;
    public GameObject playerStatusPanel;
    public GameObject bossPanel;
    public GameObject EndingPanel;
    public GameObject TitlePanel;

    [Header("Input Settings")]
    public InputActionReference openSettingsAction;
    public InputActionReference navigateTagsAction;
    public InputActionReference endGameAction;
    public InputActionReference cheatAction;
    public PlayerInput playerInput; // �v���C���[�̓��͂��Ǘ�����PlayerInput�̎Q��

    private Vector2 lastMousePosition; // �O��̃}�E�X�ʒu
    private const float mouseMoveThreshold = 20f; // �ړ�������臒l
    private GameObject lastSelectedGameObject; // ���O�ɑI������Ă����Q�[���I�u�W�F�N�g��ۑ�
    private float idleTime = 0f; // �}�E�X���Î~���Ă��鎞��
    public float restoreDelay = 0.1f; // �}�E�X����~���Ă���I���𕜌�����܂ł̒x������



    public GameObject hpBarContainer; // HP�o�[�R���e�i
    public GameObject hpSegmentPrefab; // HP�Z�O�����g�v���n�u
    public GameObject ammoBarContainer; // �e��o�[�R���e�i
    public GameObject ammoSegmentPrefab; // �e��Z�O�����g�v���n�u
    public Canvas canvas; // UI�L�����o�X
    public GameObject damageTextPrefab; // �_���[�W�e�L�X�g�̃v���n�u
    public int poolSize = 30; // �I�u�W�F�N�g�v�[���̃T�C�Y

    public GameObject tagIconPrefab;
    public GameObject tooltip;
    public GameObject tagIconTextPrefab;
    public GameObject endingTagParent;
    public TextMeshProUGUI timeText;

    private Queue<GameObject> damageTextPool; // �_���[�W�e�L�X�g�̃I�u�W�F�N�g�v�[��
    private List<GameObject> hpSegments; // ���݂�HP�Z�O�����g�̃��X�g
    private List<GameObject> ammoSegments; // ���݂̒e��Z�O�����g�̃��X�g
    private List<GameObject> tagIcons; // �^�O�A�C�R���̃��X�g

    private Dictionary<AbilityTagDefinition, int> currTags;

    private Camera mainCamera;

    int maxHP = 5;
    int currHP = 5;

    int maxAmmo = 10;
    int currAmmo = 10;

    private const float hpSegmentSpacing = 80f; // HP�Z�O�����g�Ԃ̋���
    private const float hpSegmentHeight = 50f;
    private const float hpBarHeight = 30f; // HP�o�[�̍���
    private const float hpBarWidth = 80f;
    private const float hpBarOffsetX = 10f;
    private const float hpBarOffsetY = -40f;


    private const float ammoSegmentSpacing = 20f; // �e��Z�O�����g�Ԃ̋���
    private const float ammoSegmentHeight = 50f;
    private const float ammoBarHeight = 30f; // �e��o�[�̍���
    private const float ammoBarWidth = 24f;
    private const float ammoBarOffsetX = 0f;
    private const float ammoBarOffsetY = -135f;

    private const float tagPosXDefault = 50f;
    private const float tagPosYDefault = -300f;
    private const float tagWidth = 100f;
    private const float tagHeight = 100f;

    private float startTime = 0f;
    private float titleTime = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDamageTextPool(); // �I�u�W�F�N�g�v�[����������
            hpSegments = new List<GameObject>();
            ammoSegments = new List<GameObject>();
            tagIcons = new List<GameObject>();
            UpdateHPBar(); // ����HP�o�[�̐ݒ�
            UpdateAmmoBar(); // �����e��o�[�̐ݒ�
            UnableButtons();
            tooltip.SetActive(false);
        }
        else
        {
            Destroy(gameObject); // ���łɑ��݂���ꍇ�͍폜����
        }
    }

    private void Start()
    {
        // openSettingsAction �ɂ��C�x���g��o�^
        if (openSettingsAction != null)
        {
            openSettingsAction.action.Enable();
            openSettingsAction.action.performed += ctx => ToggleSettingsPanel();
        }

        if (navigateTagsAction != null)
        {
            navigateTagsAction.action.Enable();
            navigateTagsAction.action.performed += ctx => NavigateTagIcons();
        }
        if(endGameAction!=null)
        {
            endGameAction.action.Enable();
            endGameAction.action.performed += ctx => OnTitleExitDown();
        }
        if(cheatAction!=null)
        {
            cheatAction.action.Enable();
            cheatAction.action.performed += ctx => OnCheatButtonDown();
        }
        CloseAllPanel();
    }
    private void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            startTime += Time.deltaTime;
            if (startTime>titleTime&&!TitlePanel.activeSelf)
            {
                CloseAllPanel();
                TitleUI();
            }
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Update()
    {
        // ���݂̃}�E�X�ʒu���擾
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();

        // �}�E�X�̈ړ��������v�Z
        float distance = Vector2.Distance(currentMousePosition, lastMousePosition);

        if (distance > mouseMoveThreshold)
        {
            // �}�E�X�ړ�������臒l�𒴂����ꍇ�A���݂̑I��������
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                // ���ݑI������Ă���I�u�W�F�N�g��ۑ�
                lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
                EventSystem.current.SetSelectedGameObject(null); // �I��������
            }

            // �}�E�X�������Ă���̂ŁA�Î~���Ԃ����Z�b�g
            idleTime = 0f;
        }
        else
        {
            // �}�E�X�������Ă��Ȃ��ꍇ�A�Î~���Ԃ����Z
            idleTime += Time.deltaTime;

            // �Î~���Ԃ��w�肵���x�����Ԃ𒴂��A�O��̑I�����ۑ�����Ă���ꍇ
            if (idleTime >= restoreDelay && lastSelectedGameObject != null)
            {
                // �O��I������Ă����I�u�W�F�N�g���đI��
                EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
                lastSelectedGameObject = null; // �I�𕜌���A�ۑ��f�[�^�����Z�b�g
            }
        }

        // �}�E�X�ʒu���X�V
        lastMousePosition = currentMousePosition;
    
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("UI SceneLoaded");
        if (scene.name == "Title")
        {
            CloseAllPanel();
            TitleUI();
        }
        else if (scene.name == "Tutorial")
        {
            TutorialUI();
        }
        else
        {
            CloseAllPanel();
            playerStatusPanel.SetActive(true);

        }
    }





    private void NavigateTagIcons()
    {
        if (tagIconsParent.transform.childCount == 0)
        {
            // �^�O�A�C�R�������݂��Ȃ��ꍇ�͉������Ȃ�
            return;
        }
        if (!playerStatusPanel.activeSelf) return;
        // ���݂̑I���I�u�W�F�N�g���擾
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        // ���łɃ^�O�A�C�R�����I������Ă���ꍇ�A�I��������
        if (currentSelected != null && currentSelected.transform.IsChildOf(tagIconsParent.transform) && !continuePanel.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(null); // �I��������
            return;
        }

        // �ŏ��̃^�O�A�C�R����I��
        GameObject firstIcon = tagIconsParent.transform.GetChild(0).gameObject;
        EventSystem.current.SetSelectedGameObject(firstIcon);
    }

    // �p�l�����g�O������֐���ǉ�
    private void ToggleSettingsPanel()
    {
        Debug.Log("StartButton down");
        if (settingsPanel.activeSelf)
        {
            UnableButtons(); // �ݒ�p�l�����\���ɂ���
        }
        else
        {
            AbleButtons(); // �ݒ�p�l����\������
        }
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
    private void ShowAllTagsInCenter()
    {
        foreach (Transform child in endingTagParent.transform)
        {
            Destroy(child.gameObject);
        }

        Dictionary<AbilityTagDefinition, int> tags = GameManager.Instance.playerStatus.GetCollectedTagDefinitions();
        int count = tags.Count;
        int index = 0;
        foreach (var tag in tags)
        {
            AbilityTagDefinition tagDefinition = tag.Key;
            int tagLevel = tag.Value;

            // �A�C�R���𐶐�
            GameObject tagIcon = Instantiate(tagIconTextPrefab, canvas.transform);
            tagIcon.SetActive(true);
            tagIcon.transform.SetParent(endingTagParent.transform, false);

            // �A�C�R���̈ʒu��ݒ�
            RectTransform rectTransform = tagIcon.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float xPos = 75 + ((float)index - (float)count / 2) * 150; // X���W
                rectTransform.sizeDelta = new Vector2(tagWidth, tagHeight); // �T�C�Y
                rectTransform.anchoredPosition = new Vector2(xPos, 0);
                tagIcon.GetComponent<Image>().sprite = tagDefinition.icon;
                tagIcon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tag.Value.ToString();
            }
            index++;
        }

    }
    public void SetCurrentTags(Dictionary<AbilityTagDefinition, int> tags)
    {
        ClearAllTagIcons();
        currTags = tags;

        int index = 0;
        foreach (var tag in currTags)
        {
            AbilityTagDefinition tagDefinition = tag.Key;
            int tagLevel = tag.Value;

            // �A�C�R���𐶐�
            GameObject tagIcon = Instantiate(tagIconPrefab, canvas.transform);
            tagIcon.SetActive(true);
            tagIcon.transform.SetParent(tagIconsParent.transform, false);
            tagIcons.Add(tagIcon);

            // �A�C�R���̈ʒu��ݒ�
            RectTransform rectTransform = tagIcon.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float xPos = index * tagWidth + tagPosXDefault; // X���W
                rectTransform.sizeDelta = new Vector2(tagWidth, tagHeight); // �T�C�Y
                rectTransform.anchoredPosition = new Vector2(xPos, tagPosYDefault); // ����ɏW��
            }

            // �A�C�R���ɓ��e��ݒ�
            UITooltipManager tooltipManager = tagIcon.GetComponent<UITooltipManager>();
            if (tooltipManager != null)
            {
                tooltipManager.SetTag(tagDefinition, tagLevel, tooltip);

            }

            index++;
        }
    }

    private void ClearAllTagIcons()
    {
        // �����̃^�O�A�C�R�����폜
        foreach (var tagIcon in tagIcons)
        {
            if (tagIcon != null)
            {
                Destroy(tagIcon);
            }
        }
        tagIcons.Clear();
    }

    // ===== �v���C���[�X�e�[�^�X =====
    public void TakeDamage(int dmg)
    {
        currHP -= dmg;
        if (currHP < 0) currHP = 0;
        UpdateHPBar();
    }

    public void Heal(int healAmount)
    {
        currHP += healAmount;
        if (currHP > maxHP) currHP = maxHP;
        UpdateHPBar();
    }

    private void UpdateHPBar() // HP�o�[���X�V
    {
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
            float newWidth = hpBarWidth + hpSegmentSpacing * (maxHP - 1);
            barRect.sizeDelta = new Vector2(newWidth, hpBarHeight);

            // �ʒu���v�Z
            float newXPos = hpBarWidth/2 + (hpSegmentSpacing / 2f) * (maxHP - 1)+ hpBarOffsetX;
            barRect.anchoredPosition = new Vector2(newXPos, -hpBarHeight / 2 + hpBarOffsetY);
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
                rectTransform.anchoredPosition = new Vector2(xPos, 0); 
                rectTransform.sizeDelta = new Vector2(hpSegmentSpacing, hpSegmentHeight); // �Z�O�����g�̃T�C�Y��ݒ�
            }

            // �Z�O�����g�̏�Ԃ�ݒ�
            Image segmentImage = segment.GetComponent<Image>();
            if (segmentImage != null)
            {
                segmentImage.color = (i < currHP) ? Color.white : Color.red; // ���݂�HP�Ƌ��HP
            }

            hpSegments.Add(segment);
        }
    }

    public void SetHP(int curr, int max_)
    {
        currHP = curr;
        maxHP = max_;
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
            float newWidth = ammoBarWidth + ammoSegmentSpacing * (maxAmmo - 1);
            barRect.sizeDelta = new Vector2(newWidth, ammoBarHeight);

            // �ʒu���v�Z
            float newXPos = ammoBarWidth + (ammoSegmentSpacing / 2f) * (maxAmmo - 1) + ammoBarOffsetX ;
            barRect.anchoredPosition = new Vector2(newXPos, -ammoBarHeight / 2 + ammoBarOffsetY);
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
                rectTransform.anchoredPosition = new Vector2(xPos, 0); // 
                rectTransform.sizeDelta = new Vector2(ammoSegmentSpacing, ammoSegmentHeight); // �Z�O�����g�̃T�C�Y��ݒ�
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
    private GameObject SetFirstButton(GameObject panel)
    {
        Button firstButton = panel.GetComponentInChildren<Button>();
        if (firstButton != null)
        {
            EventSystem.current.firstSelectedGameObject = firstButton.gameObject;
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            return firstButton.gameObject;
        }
        else
        {
            Debug.Log("Null button");
            return null;
        }
        
    }
    private void CloseAllPanel()
    {
        settingsPanel.SetActive(false);
        continuePanel.SetActive(false);
        continuePanel2.SetActive(false);
        failPanel.SetActive(false);
        playerStatusPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        bossPanel.SetActive(false);
        EndingPanel.SetActive(false);
        TitlePanel.SetActive(false);
    }

    private void OnCheatButtonDown()
    {
        Debug.Log("Cheat!");
        StageManager.Instance?.SpawnSuperRareDrop();
    }

    // ===== UI�Ǘ� =====
    public void AbleButtons()
    {
        if (!CheckMenuAcceptable()) return;
        CloseAllPanel();
        settingsPanel.SetActive(true);
        playerInput.actions.FindActionMap("PlayerCharacter").Disable();

        // EventSystem��FirstSelected��ݒ�UI�̍ŏ���Button�ɐݒ�
        SetFirstButton(settingsPanel);

        GameManager.Instance?.PauseGame();
    }

    public void UnableButtons()
    {
        settingsPanel.SetActive(false);
        playerInput.actions.FindActionMap("PlayerCharacter").Enable();
        GameManager.Instance?.UnpauseGame();
        playerStatusPanel.SetActive(true);

    }

    public void ContinueUI1()
    {
        GameManager.Instance?.PauseGame();
        playerInput.actions.FindActionMap("PlayerCharacter").Disable();
        continuePanel.SetActive(true);
        SetFirstButton(continuePanel);

    }
    public void ContinueUI2()
    {
        CloseAllPanel();
        GameManager.Instance?.PauseGame();
        playerInput.actions.FindActionMap("PlayerCharacter").Disable();
        continuePanel2.SetActive(true);
        SetFirstButton(continuePanel2);

    }
    public void ContinueToBoss()
    {
        CloseAllPanel();
        GameManager.Instance?.PauseGame();
        playerInput.actions.FindActionMap("PlayerCharacter").Disable();
        bossPanel.SetActive(true);
        SetFirstButton(bossPanel);
    }
    public void TitleUI()
    {
        CloseAllPanel();
        GameManager.Instance?.PauseGame();
        playerInput.actions.FindActionMap("PlayerCharacter").Disable();
        TitlePanel.SetActive(true);
        SetFirstButton(TitlePanel);
    }
    public void FailUI()
    {
        CloseAllPanel();
        playerStatusPanel.SetActive(true);
        GameManager.Instance?.PauseGame();
        playerInput.actions.FindActionMap("PlayerCharacter").Disable();
        failPanel.SetActive(true);
        SetFirstButton(failPanel);
    }
    public void TutorialUI()
    {
        CloseAllPanel();
        GameManager.Instance?.PauseGame();
        playerInput.actions.FindActionMap("PlayerCharacter").Disable();
        tutorialPanel.SetActive(true);
        SetFirstButton(tutorialPanel);
    }
    public void EndingUI()
    {
        CloseAllPanel();
        GameManager.Instance?.PauseGame();
        playerInput.actions.FindActionMap("PlayerCharacter").Disable();
        EndingPanel.SetActive(true);
        ShowAllTagsInCenter();
        TimeSpan timeSpan = TimeSpan.FromSeconds(GameManager.Instance.gameTime);
        timeText.text = "Time " + $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        SetFirstButton(EndingPanel);
    }
    public void OnExitGameButtonDown()
    {
        SceneManager.LoadScene("Title");
    }

    public void OnRetryButtonDown()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnTitleExitDown()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();//�Q�[���v���C�I��
        #endif
    }
    public void OnGameStart()
    {
        CloseAllPanel();
        SceneManager.LoadScene("Tutorial");
    }

    public void SetMainCamera(Camera cam)
    {
        mainCamera = cam;
    }
    public void CloseUIReturnToGame()
    {
        CloseAllPanel();
        GameManager.Instance?.UnpauseGame();
        playerStatusPanel.SetActive(true);
        playerInput.actions.FindActionMap("PlayerCharacter").Enable();
    }
    public void LoadDifficultScene()
    {
        playerInput.actions.FindActionMap("PlayerCharacter").Enable();
        CloseAllPanel();
        GameManager.Instance?.AdvanceStage();
        SceneManager.LoadScene("Difficult");
    }
    public void LoadEasyScene()
    {
        playerInput.actions.FindActionMap("PlayerCharacter").Enable();
        CloseAllPanel();
        GameManager.Instance?.AdvanceStage();
        SceneManager.LoadScene("Easy");
    }
    public void LoadBossScene()
    {
        playerInput.actions.FindActionMap("PlayerCharacter").Enable();
        CloseAllPanel();
        GameManager.Instance?.AdvanceStage();
        SceneManager.LoadScene("Boss");
    }
    public bool CheckMenuAcceptable()
    {
        if (
            failPanel.activeSelf ||
            continuePanel.activeSelf ||
            continuePanel2.activeSelf ||
            bossPanel.activeSelf ||
            EndingPanel.activeSelf ||
            TitlePanel.activeSelf
            )
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void OnDestroy()
    {
        if (openSettingsAction != null)
        {
            openSettingsAction.action.performed -= ctx => ToggleSettingsPanel();
        }

        if (navigateTagsAction != null)
        {
            navigateTagsAction.action.performed -= ctx => NavigateTagIcons();
        }
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
}