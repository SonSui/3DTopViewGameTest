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
    public PlayerInput playerInput; // プレイヤーの入力を管理するPlayerInputの参照

    private Vector2 lastMousePosition; // 前回のマウス位置
    private const float mouseMoveThreshold = 20f; // 移動距離の閾値
    private GameObject lastSelectedGameObject; // 直前に選択されていたゲームオブジェクトを保存
    private float idleTime = 0f; // マウスが静止している時間
    public float restoreDelay = 0.1f; // マウスが停止してから選択を復元するまでの遅延時間



    public GameObject hpBarContainer; // HPバーコンテナ
    public GameObject hpSegmentPrefab; // HPセグメントプレハブ
    public GameObject ammoBarContainer; // 弾薬バーコンテナ
    public GameObject ammoSegmentPrefab; // 弾薬セグメントプレハブ
    public Canvas canvas; // UIキャンバス
    public GameObject damageTextPrefab; // ダメージテキストのプレハブ
    public int poolSize = 30; // オブジェクトプールのサイズ

    public GameObject tagIconPrefab;
    public GameObject tooltip;
    public GameObject tagIconTextPrefab;
    public GameObject endingTagParent;
    public TextMeshProUGUI timeText;

    private Queue<GameObject> damageTextPool; // ダメージテキストのオブジェクトプール
    private List<GameObject> hpSegments; // 現在のHPセグメントのリスト
    private List<GameObject> ammoSegments; // 現在の弾薬セグメントのリスト
    private List<GameObject> tagIcons; // タグアイコンのリスト

    private Dictionary<AbilityTagDefinition, int> currTags;

    private Camera mainCamera;

    int maxHP = 5;
    int currHP = 5;

    int maxAmmo = 10;
    int currAmmo = 10;

    private const float hpSegmentSpacing = 80f; // HPセグメント間の距離
    private const float hpSegmentHeight = 50f;
    private const float hpBarHeight = 30f; // HPバーの高さ
    private const float hpBarWidth = 80f;
    private const float hpBarOffsetX = 10f;
    private const float hpBarOffsetY = -40f;


    private const float ammoSegmentSpacing = 20f; // 弾薬セグメント間の距離
    private const float ammoSegmentHeight = 50f;
    private const float ammoBarHeight = 30f; // 弾薬バーの高さ
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
            InitializeDamageTextPool(); // オブジェクトプールを初期化
            hpSegments = new List<GameObject>();
            ammoSegments = new List<GameObject>();
            tagIcons = new List<GameObject>();
            UpdateHPBar(); // 初期HPバーの設定
            UpdateAmmoBar(); // 初期弾薬バーの設定
            UnableButtons();
            tooltip.SetActive(false);
        }
        else
        {
            Destroy(gameObject); // すでに存在する場合は削除する
        }
    }

    private void Start()
    {
        // openSettingsAction によるイベントを登録
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
        // 現在のマウス位置を取得
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();

        // マウスの移動距離を計算
        float distance = Vector2.Distance(currentMousePosition, lastMousePosition);

        if (distance > mouseMoveThreshold)
        {
            // マウス移動距離が閾値を超えた場合、現在の選択を解除
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                // 現在選択されているオブジェクトを保存
                lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
                EventSystem.current.SetSelectedGameObject(null); // 選択を解除
            }

            // マウスが動いているので、静止時間をリセット
            idleTime = 0f;
        }
        else
        {
            // マウスが動いていない場合、静止時間を加算
            idleTime += Time.deltaTime;

            // 静止時間が指定した遅延時間を超え、前回の選択が保存されている場合
            if (idleTime >= restoreDelay && lastSelectedGameObject != null)
            {
                // 前回選択されていたオブジェクトを再選択
                EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
                lastSelectedGameObject = null; // 選択復元後、保存データをリセット
            }
        }

        // マウス位置を更新
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
            // タグアイコンが存在しない場合は何もしない
            return;
        }
        if (!playerStatusPanel.activeSelf) return;
        // 現在の選択オブジェクトを取得
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        // すでにタグアイコンが選択されている場合、選択を解除
        if (currentSelected != null && currentSelected.transform.IsChildOf(tagIconsParent.transform) && !continuePanel.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(null); // 選択を解除
            return;
        }

        // 最初のタグアイコンを選択
        GameObject firstIcon = tagIconsParent.transform.GetChild(0).gameObject;
        EventSystem.current.SetSelectedGameObject(firstIcon);
    }

    // パネルをトグルする関数を追加
    private void ToggleSettingsPanel()
    {
        Debug.Log("StartButton down");
        if (settingsPanel.activeSelf)
        {
            UnableButtons(); // 設定パネルを非表示にする
        }
        else
        {
            AbleButtons(); // 設定パネルを表示する
        }
    }


    // =====ダメージ表示=====
    private void InitializeDamageTextPool()
    {
        damageTextPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(damageTextPrefab);
            obj.transform.SetParent(canvas.transform, false); // キャンバスに追加
            obj.SetActive(false); // 非アクティブ化
            damageTextPool.Enqueue(obj); // キューに追加
        }
    }

    public GameObject GetDamageTextObject()
    {
        if (damageTextPool.Count > 0)
        {
            GameObject obj = damageTextPool.Dequeue();
            obj.SetActive(true); // アクティブ化
            return obj;
        }
        else
        {
            // プールに余裕がない場合、新しいオブジェクトを作成
            GameObject obj = Instantiate(damageTextPrefab);
            obj.transform.SetParent(canvas.transform, false);
            return obj;
        }
    }

    public void ReturnDamageTextObject(GameObject obj)
    {
        obj.SetActive(false); // 非アクティブ化
        damageTextPool.Enqueue(obj); // キューに戻す
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

            // アイコンを生成
            GameObject tagIcon = Instantiate(tagIconTextPrefab, canvas.transform);
            tagIcon.SetActive(true);
            tagIcon.transform.SetParent(endingTagParent.transform, false);

            // アイコンの位置を設定
            RectTransform rectTransform = tagIcon.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float xPos = 75 + ((float)index - (float)count / 2) * 150; // X座標
                rectTransform.sizeDelta = new Vector2(tagWidth, tagHeight); // サイズ
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

            // アイコンを生成
            GameObject tagIcon = Instantiate(tagIconPrefab, canvas.transform);
            tagIcon.SetActive(true);
            tagIcon.transform.SetParent(tagIconsParent.transform, false);
            tagIcons.Add(tagIcon);

            // アイコンの位置を設定
            RectTransform rectTransform = tagIcon.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float xPos = index * tagWidth + tagPosXDefault; // X座標
                rectTransform.sizeDelta = new Vector2(tagWidth, tagHeight); // サイズ
                rectTransform.anchoredPosition = new Vector2(xPos, tagPosYDefault); // 左上に集中
            }

            // アイコンに内容を設定
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
        // 既存のタグアイコンを削除
        foreach (var tagIcon in tagIcons)
        {
            if (tagIcon != null)
            {
                Destroy(tagIcon);
            }
        }
        tagIcons.Clear();
    }

    // ===== プレイヤーステータス =====
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

    private void UpdateHPBar() // HPバーを更新
    {
        if (hpBarContainer == null || hpSegmentPrefab == null)
        {
            Debug.LogError("hpBarContainer または hpSegmentPrefab が設定されていません！");
            return;
        }

        // HPバーコンテナのサイズと位置を更新
        RectTransform barRect = hpBarContainer.GetComponent<RectTransform>();
        if (barRect != null)
        {
            // 幅を計算
            float newWidth = hpBarWidth + hpSegmentSpacing * (maxHP - 1);
            barRect.sizeDelta = new Vector2(newWidth, hpBarHeight);

            // 位置を計算
            float newXPos = hpBarWidth/2 + (hpSegmentSpacing / 2f) * (maxHP - 1)+ hpBarOffsetX;
            barRect.anchoredPosition = new Vector2(newXPos, -hpBarHeight / 2 + hpBarOffsetY);
        }

        // 既存のセグメントをクリア
        foreach (GameObject segment in hpSegments)
        {
            if (segment != null)
            {
                Destroy(segment);
            }
        }
        hpSegments.Clear();

        // 現在のHPセグメントを作成
        for (int i = 0; i < maxHP; i++)
        {
            GameObject segment = Instantiate(hpSegmentPrefab, hpBarContainer.transform);
            segment.SetActive(true);

            // セグメントの位置を設定
            RectTransform rectTransform = segment.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float xPos = -hpSegmentSpacing * (maxHP - 1) / 2f + i * hpSegmentSpacing;
                rectTransform.anchoredPosition = new Vector2(xPos, 0); 
                rectTransform.sizeDelta = new Vector2(hpSegmentSpacing, hpSegmentHeight); // セグメントのサイズを設定
            }

            // セグメントの状態を設定
            Image segmentImage = segment.GetComponent<Image>();
            if (segmentImage != null)
            {
                segmentImage.color = (i < currHP) ? Color.white : Color.red; // 現在のHPと空のHP
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

    private void UpdateAmmoBar() // 弾薬バーを更新
    {
        if (ammoBarContainer == null || ammoSegmentPrefab == null)
        {
            Debug.LogError("ammoBarContainer または ammoSegmentPrefab が設定されていません！");
            return;
        }

        // 弾薬バーコンテナのサイズと位置を更新
        RectTransform barRect = ammoBarContainer.GetComponent<RectTransform>();
        if (barRect != null)
        {
            // 幅を計算
            float newWidth = ammoBarWidth + ammoSegmentSpacing * (maxAmmo - 1);
            barRect.sizeDelta = new Vector2(newWidth, ammoBarHeight);

            // 位置を計算
            float newXPos = ammoBarWidth + (ammoSegmentSpacing / 2f) * (maxAmmo - 1) + ammoBarOffsetX ;
            barRect.anchoredPosition = new Vector2(newXPos, -ammoBarHeight / 2 + ammoBarOffsetY);
        }

        // 既存のセグメントをクリア
        foreach (GameObject segment in ammoSegments)
        {
            if (segment != null)
            {
                Destroy(segment);
            }
        }
        ammoSegments.Clear();

        // 現在の弾薬セグメントを作成
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject segment = Instantiate(ammoSegmentPrefab, ammoBarContainer.transform);
            segment.SetActive(true);

            // セグメントの位置を設定
            RectTransform rectTransform = segment.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float xPos = -ammoSegmentSpacing * (maxAmmo - 1) / 2f + i * ammoSegmentSpacing;
                rectTransform.anchoredPosition = new Vector2(xPos, 0); // 
                rectTransform.sizeDelta = new Vector2(ammoSegmentSpacing, ammoSegmentHeight); // セグメントのサイズを設定
            }

            // セグメントの状態を設定
            Image segmentImage = segment.GetComponent<Image>();
            if (segmentImage != null)
            {
                segmentImage.color = (i < currAmmo) ? Color.white : Color.red; // 現在の弾薬と空の弾薬
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

    // ===== UI管理 =====
    public void AbleButtons()
    {
        if (!CheckMenuAcceptable()) return;
        CloseAllPanel();
        settingsPanel.SetActive(true);
        playerInput.actions.FindActionMap("PlayerCharacter").Disable();

        // EventSystemのFirstSelectedを設定UIの最初のButtonに設定
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
            Application.Quit();//ゲームプレイ終了
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