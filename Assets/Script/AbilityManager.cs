using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class AbilityManager : MonoBehaviour
{
    //すべてのタグ
    public List<AbilityTagDefinition> abilityTagDefinitions;

    // タグの数字と効果
    private Dictionary<string, int> tagCounts = new Dictionary<string, int>();
    private Dictionary<string, AbilityEffect> currentEffects = new Dictionary<string, AbilityEffect>();

    // 集めたもの
    private List<Item> collectedItems = new List<Item>();

    // 集めたものを表示
    public IReadOnlyList<Item> CollectedItems => collectedItems.AsReadOnly();

    
    public Text itemTag;

    public GameObject playerPrefab; 
    public GameObject cameraPrefab;

    public PlayerController player;

    public static AbilityManager Instance { get; private set; }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            //SpawnPlayerAndCamera(); 
        }
        else
        {
            Destroy(gameObject); 
        }

    }
    public void Start()
    {
        Instance = this;
        Update_UI();
    }

    // 新しいものを集める
    public void CollectItem(Item item)
    {
        collectedItems.Add(item);

        foreach (string tag in item.tags)
        {
            // 持っているタグの数量増加
            if (tagCounts.ContainsKey(tag))
                tagCounts[tag]++;
            else
                tagCounts[tag] = 1;

            // 効果更新
            UpdateAbilityEffect(tag);
        }

        
    }

    // 効果更新
    private void UpdateAbilityEffect(string tag)
    {
        if (abilityTagDefinitions == null || abilityTagDefinitions.Count == 0)
        {
            Debug.LogError("AbilityTagDefinitions is null");
            return;
        }
        //タグを探す
        AbilityTagDefinition tagDefinition = abilityTagDefinitions.Find(t => t.tagName == tag);
        if (tagDefinition == null)
        {
            Debug.Log("Can't Find tag:" + tag);
            return;
        }

        int count = tagCounts[tag];
        AbilityEffect totalEffect = new AbilityEffect();

        //変化するタグ効果を加算
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

        // プレイヤーに渡す
        ApplyEffects();
    }

    // プレイヤーに渡す
    private void ApplyEffects()
    {
        player.state.ResetState();
        foreach (var effect in currentEffects.Values)
        {
            player.state.UpdateState(effect.life,effect.speed,effect.damage,effect.critBonus);
            if(effect.unlockExplosion)
            {
                player.state.UpdateAblitiy(effect.unlockExplosion);
            }
        }
        player.state.ShowState();
        // UI
        Update_UI();
    }

    //画面にタグと状態を表示
    private void Update_UI()
    {
        string text="";
        var sortedTagCounts = tagCounts.OrderBy(kv => kv.Key);
        foreach (var kvp in sortedTagCounts)
        {
            string tag = kvp.Key;
            string count = kvp.Value.ToString();

            text += tag + " " + count + "\n";
        }
        text += "\n"+player.GetStateString();
        itemTag.text = text;
    }
    /*void SpawnPlayerAndCamera()
    {
        
        if (GameObject.FindWithTag("Player") == null)
        {
            Debug.Log("player");
            player=Instantiate(playerPrefab).GetComponent<PlayerController>();
        }

        if (Camera.main == null)
        {
            Instantiate(cameraPrefab);
        }
    }*/
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //SpawnPlayerAndCamera();
        FindPlayer();
        
    }
    void FindPlayer()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }
}