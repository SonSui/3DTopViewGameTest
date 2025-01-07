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
    public GameObject hpBarContainer; // HPバーコンテナ
    public GameObject hpSegmentPrefab; // HPセグメントプレハブ
    public GameObject ammoBarContainer; // 弾薬バーコンテナ
    public GameObject ammoSegmentPrefab; // 弾薬セグメントプレハブ
=======
    public Image hp; // HPバー
>>>>>>> origin/main
    public Canvas canvas; // UIキャンバス
    public GameObject settingUI; // 設定UI
    public GameObject damageTextPrefab; // ダメージテキストのプレハブ
    public int poolSize = 30; // オブジェクトプールのサイズ

    private Queue<GameObject> damageTextPool; // ダメージテキストのオブジェクトプール
<<<<<<< HEAD
    private List<GameObject> hpSegments; // 現在のHPセグメントのリスト
    private List<GameObject> ammoSegments; // 現在の弾薬セグメントのリスト
=======
>>>>>>> origin/main

    private Camera mainCamera;

    int maxHP = 5;
    int currHP = 5;

<<<<<<< HEAD
    int maxAmmo = 10;
    int currAmmo = 10;

    private const float hpSegmentSpacing = 65f; // HPセグメント間の距離
    private const float hpBarHeight = 200f; // HPバーの高さ

    private const float ammoSegmentSpacing = 30f; // 弾薬セグメント間の距離
    private const float ammoBarHeight = 200f; // 弾薬バーの高さ

=======
>>>>>>> origin/main
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDamageTextPool(); // オブジェクトプールを初期化
<<<<<<< HEAD
            hpSegments = new List<GameObject>();
            ammoSegments = new List<GameObject>();
            UpdateHPBar(); // 初期HPバーの設定
            UpdateAmmoBar(); // 初期弾薬バーの設定
            UnableButtons();
=======
>>>>>>> origin/main
        }
        else
        {
            Destroy(gameObject); // すでに存在する場合は削除する
        }
    }

<<<<<<< HEAD
=======
    void Start()
    {
        UnableButtons();
    }

>>>>>>> origin/main

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
    // ===== プレイヤーステータス =====
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

    private void UpdateHPBar() // HPバーを更新
    {
<<<<<<< HEAD
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
            float newWidth = 45 + ammoSegmentSpacing * (maxAmmo - 1);
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
=======
        UpdateHPBar();
    }



>>>>>>> origin/main

    // ===== UI管理 =====
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
