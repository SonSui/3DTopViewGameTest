using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public class AbilityManager : MonoBehaviour
{
    // すべてのタグ
    public List<AbilityTagDefinition> abilityTagDefinitions;

    // タグの数字と効果
    private Dictionary<string, int> tagCounts = new Dictionary<string, int>();
    private Dictionary<string, AbilityEffect> currentEffects = new Dictionary<string, AbilityEffect>();

    // 集めたもの
    private List<Item> collectedItems = new List<Item>();

    // 集めたものを表示
    public IReadOnlyList<Item> CollectedItems => collectedItems.AsReadOnly();

    // UI関連
    public GameObject uiCanvasPrefab; // プレイヤーが用意したCanvasのPrefab
    private GameObject parameterUI;
    private Text playerStatusText;    // プレイヤー状態表示用のText
    private Text itemTag;             // アイテム表示用のText
    private InputField[] inputFields; // プレイヤー状態編集用のInputField

    public PlayerController player;

    public static AbilityManager Instance { get; private set; }

    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        // プレイヤーを探す
        StartCoroutine(WaitForPlayer());

        // UIの初期設定
        SetupUI();
        Update_UI(); // UIの初期状態を更新
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchParameterUI();
        }
    }

    // プレイヤーの生成を待つコルーチン
    private IEnumerator WaitForPlayer()
    {
        // GameManagerがプレイヤーを生成するまで待機
        while (player == null)
        {
            player = GameManager.Instance?.GetPlayer()?.GetComponent<PlayerController>();
            if (player == null)
            {
                Debug.Log("Playerが見つかりません、待機中...");
                yield return null; // 次のフレームまで待つ
            }
        }
        Debug.Log("Playerが見つかりました！");
        Update_UI(); // プレイヤーが見つかったらUIを更新
    }


    private void SwitchParameterUI()
    {
        if (parameterUI.activeInHierarchy)
        {
            parameterUI.SetActive(false);
        }
        else
        {
            parameterUI.SetActive(true);
            Update_UI();
        }
    }


    // UIの初期設定
    private void SetupUI()
    {
        // CanvasのPrefabをインスタンス化
        if (uiCanvasPrefab != null)
        {
            parameterUI = Instantiate(uiCanvasPrefab);
            DontDestroyOnLoad(parameterUI); // シーン遷移時に削除されないように設定

            // プレイヤー状態表示用のTextを取得
            Transform statusTextTransform = parameterUI.transform.Find("PlayerStatusText");
            if (statusTextTransform != null)
                playerStatusText = statusTextTransform.GetComponent<Text>();

            // アイテム表示用のTextを取得
            Transform itemTagTransform = parameterUI.transform.Find("ItemTagText");
            if (itemTagTransform != null)
                itemTag = itemTagTransform.GetComponent<Text>();

            // InputFieldの取得
            inputFields = statusTextTransform.GetComponentsInChildren<InputField>();
            for (int i = 0; i < inputFields.Length; i++)
            {
                int index = i; // ローカル変数としてインデックスを保存
                inputFields[i].onEndEdit.AddListener((input) => UpdatePlayerStatusFromInput(index, input));
            }
        }
        else
        {
            Debug.LogError("UI Canvas Prefab is not assigned.");
        }
        parameterUI.SetActive(false);
    }

    // InputFieldからプレイヤーの状態を更新
    private void UpdatePlayerStatusFromInput(int index, string input)
    {
        if (player == null) return;

        if (int.TryParse(input, out int value))
        {
            switch (index)
            {
                case 0: player.state.life = value; break;
                case 1: player.state.speed = value; break;
                case 2: player.state.damage = value; break;
                case 3: player.state.crit = value; break;
                // 必要に応じて他のステータスを追加
                default: break;
            }

            player.finalState.ShowState(); // プレイヤー状態の表示を更新
            Update_UI(); // UI全体の更新
        }
        else
        {
            Debug.LogWarning("入力が無効です: " + input);
        }
    }

    // 新しいものを集めるメソッド
    public void CollectItem(Item item)
    {
        collectedItems.Add(item);

        foreach (string tag in item.tags)
        {
            // 持っているタグの数量を増加
            if (tagCounts.ContainsKey(tag))
                tagCounts[tag]++;
            else
                tagCounts[tag] = 1;

            // 効果を更新
            UpdateAbilityEffect(tag);
        }
    }

    // 効果を更新するメソッド
    private void UpdateAbilityEffect(string tag)
    {
        if (abilityTagDefinitions == null || abilityTagDefinitions.Count == 0)
        {
            Debug.LogError("AbilityTagDefinitions is null");
            return;
        }

        // タグを探す
        AbilityTagDefinition tagDefinition = abilityTagDefinitions.Find(t => t.tagName == tag);
        if (tagDefinition == null)
        {
            Debug.Log("Can't Find tag:" + tag);
            return;
        }

        int count = tagCounts[tag];
        AbilityEffect totalEffect = new AbilityEffect();

        // タグ効果を加算
        for (int i = 0; i < tagDefinition.thresholds.Count; i++)
        {
            if (count >= tagDefinition.thresholds[i])
            {
                AbilityEffect effect = tagDefinition.effects[i];
                totalEffect += effect;
            }
        }

        // タグ効果を更新
        if (currentEffects.ContainsKey(tag))
            currentEffects[tag] = totalEffect;
        else
            currentEffects.Add(tag, totalEffect);

        // プレイヤーに効果を適用
        ApplyEffects();
    }

    // プレイヤーに効果を適用するメソッド
    private void ApplyEffects()
    {
        if (player == null) return;

        player.state.ResetState();

        foreach (var effect in currentEffects.Values)
        {
            player.state.UpdateState(effect.life, effect.speed, effect.damage, effect.critBonus);
            if (effect.unlockExplosion)
            {
                player.state.UpdateAblitiy(effect.unlockExplosion);
            }
        }

        player.state.ShowState();
        Update_UI(); // UIの更新
    }

    // 画面にタグと状態を表示するメソッド
    private void Update_UI()
    {
        // プレイヤー状態の表示
        if (playerStatusText != null && player != null)
        {
            playerStatusText.text = player.GetStateString();
        }

        // アイテムの表示
        if (itemTag != null)
        {
            string text = "Item:\n";
            var sortedTagCounts = tagCounts.OrderBy(kv => kv.Key);
            foreach (var kvp in sortedTagCounts)
            {
                string tag = kvp.Key;
                string count = kvp.Value.ToString();
                text += tag + " " + count + "\n";
            }
            itemTag.text = text;
        }
    }
}
