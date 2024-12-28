using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Image hp; // HPバー
    public Canvas canvas; // UIキャンバス
    public GameObject settingUI; // 設定UI
    public GameObject damageTextPrefab; // ダメージテキストのプレハブ
    public int poolSize = 30; // オブジェクトプールのサイズ

    private Queue<GameObject> damageTextPool; // ダメージテキストのオブジェクトプール

    private Camera mainCamera;

    int maxHP = 5;
    int currHP = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDamageTextPool(); // オブジェクトプールを初期化
        }
        else
        {
            Destroy(gameObject); // すでに存在する場合は削除する
        }
    }

    void Start()
    {
        UnableButtons();
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







    // ===== プレイヤーステータス =====
    public void TakeDamage(int dmg)
    {
        currHP -= dmg;
        UpdateHPBar();
    }

    private void UpdateHPBar() // HPバーを更新
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
    public void SetMainCamera(Camera cam)
    {
        mainCamera = cam;
    }
}
