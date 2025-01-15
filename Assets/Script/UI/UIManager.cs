using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    [Header("Input Settings")]
    public InputActionReference openSettingsAction;
    public InputActionReference navigateTagsAction;
    public PlayerInput playerInput; // プレイヤーの入力を管理するPlayerInputの参照

    private Vector2 lastMousePosition; // 前回のマウス位置
    private const float mouseMoveThreshold = 10f; // 移動距離の閾値



    public GameObject hpBarContainer; // HPバーコンテナ
    public GameObject hpSegmentPrefab; // HPセグメントプレハブ
    public GameObject ammoBarContainer; // 弾薬バーコンテナ
    public GameObject ammoSegmentPrefab; // 弾薬セグメントプレハブ
    public Canvas canvas; // UIキャンバス
    public GameObject damageTextPrefab; // ダメージテキストのプレハブ
    public int poolSize = 30; // オブジェクトプールのサイズ

    public GameObject tagIconPrefab;
    public GameObject tooltip;

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

    private const float hpSegmentSpacing = 60f; // HPセグメント間の距離
    private const float hpBarHeight = 200f; // HPバーの高さ

    private const float ammoSegmentSpacing = 28f; // 弾薬セグメント間の距離
    private const float ammoBarHeight = 200f; // 弾薬バーの高さ

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
        
    }
    private void OnEnable()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Title")
        {
            CloseAllPanel();
        }
        else if(scene.name =="Tutorial")
        {
            TutorialUI();
        }
        else
        {
            CloseAllPanel();
            playerStatusPanel.SetActive(true);
        }

    }
    private void Update()
    {
        // 現在のマウス位置を取得
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();

        // マウスの移動距離を計算
        float distance = Vector2.Distance(currentMousePosition, lastMousePosition);

        // 移動距離が閾値を超えた場合、選択を解除
        if (distance > mouseMoveThreshold)
        {
            EventSystem.current.SetSelectedGameObject(null); // 現在の選択を解除
        }

        // マウス位置を更新
        lastMousePosition = currentMousePosition;
    }
    private void NavigateTagIcons()
    {
        if (tagIconsParent.transform.childCount == 0)
        {
            // タグアイコンが存在しない場合は何もしない
            return;
        }

        // 現在の選択オブジェクトを取得
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        // すでにタグアイコンが選択されている場合、選択を解除
        if (currentSelected != null && currentSelected.transform.IsChildOf(tagIconsParent.transform))
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
                float xPos = index * 100 + 50; // X座標
                rectTransform.sizeDelta = new Vector2(100, 100); // サイズ
                rectTransform.anchoredPosition = new Vector2(xPos, -350); // 左上に集中
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
            float newWidth = 90 + hpSegmentSpacing * (maxHP - 1);
            barRect.sizeDelta = new Vector2(newWidth, hpBarHeight);

            // 位置を計算
            float newXPos = 60 + (hpSegmentSpacing / 2f) * (maxHP - 1);
            barRect.anchoredPosition = new Vector2(newXPos, -100);
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
                rectTransform.anchoredPosition = new Vector2(xPos, -3); // Y軸を-3に設定
                rectTransform.sizeDelta = new Vector2(110, 110); // セグメントのサイズを設定
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
            float newWidth = 48 + ammoSegmentSpacing * (maxAmmo - 1);
            barRect.sizeDelta = new Vector2(newWidth, ammoBarHeight);

            // 位置を計算
            float newXPos = 37 + (ammoSegmentSpacing / 2f) * (maxAmmo - 1);
            barRect.anchoredPosition = new Vector2(newXPos, -220);
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
                rectTransform.anchoredPosition = new Vector2(xPos, -3); // Y軸を-3に設定
                rectTransform.sizeDelta = new Vector2(80, 110); // セグメントのサイズを設定
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
        else return null;
        
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
    }

    // ===== UI管理 =====
    public void AbleButtons()
    {
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

    public void OnExitGameButtonDown()
    {
        SceneManager.LoadScene("Title");
    }

    public void OnRetryButtonDown()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnGameStart()
    {
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
        SceneManager.LoadScene("Difficult");
    }
    public void LoadEasyScene()
    {
        playerInput.actions.FindActionMap("PlayerCharacter").Enable();
        CloseAllPanel();
        SceneManager.LoadScene("Easy");
    }
    public void LoadBossScene()
    {
        playerInput.actions.FindActionMap("PlayerCharacter").Enable();
        CloseAllPanel();
        SceneManager.LoadScene("Boss");
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