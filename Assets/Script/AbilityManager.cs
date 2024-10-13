using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class AbilityManager : MonoBehaviour
{
    //���ׂẴ^�O
    public List<AbilityTagDefinition> abilityTagDefinitions;

    // �^�O�̐����ƌ���
    private Dictionary<string, int> tagCounts = new Dictionary<string, int>();
    private Dictionary<string, AbilityEffect> currentEffects = new Dictionary<string, AbilityEffect>();

    // �W�߂�����
    private List<Item> collectedItems = new List<Item>();

    // �W�߂����̂�\��
    public IReadOnlyList<Item> CollectedItems => collectedItems.AsReadOnly();

    public PlayerController player;
    public Text itemTag;


    public static AbilityManager Instance { get; private set; }

    private void Awake()
    {
        
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

    // �V�������̂��W�߂�
    public void CollectItem(Item item)
    {
        collectedItems.Add(item);

        foreach (string tag in item.tags)
        {
            // �^�O�̐���
            if (tagCounts.ContainsKey(tag))
                tagCounts[tag]++;
            else
                tagCounts[tag] = 1;

            // ���ʍX�V
            UpdateAbilityEffect(tag);
        }

        // UI
        Update_UI();
    }

    // ���ʍX�V
    private void UpdateAbilityEffect(string tag)
    {
        AbilityTagDefinition tagDefinition = abilityTagDefinitions.Find(t => t.tagName == tag);
        if (tagDefinition == null)
            return;

        int count = tagCounts[tag];
        AbilityEffect totalEffect = new AbilityEffect();

       
        for (int i = 0; i < tagDefinition.thresholds.Count; i++)
        {
            if (count >= tagDefinition.thresholds[i])
            {
                AbilityEffect effect = tagDefinition.effects[i];
                totalEffect.critBonus += effect.critBonus;
                if (effect.unlockExplosion)
                    totalEffect.unlockExplosion = true;
                
            }
        }

        // �X�V
        if (currentEffects.ContainsKey(tag))
            currentEffects[tag] = totalEffect;
        else
            currentEffects.Add(tag, totalEffect);

        // �v���C���[�ɓn��
        ApplyEffects();
    }

    // �v���C���[�ɓn��
    private void ApplyEffects()
    {
        
        foreach (var effect in currentEffects.Values)
        {
            PlayerStats.Instance.crit += effect.critBonus;
            if (effect.unlockExplosion)
                PlayerStats.Instance.EnableExplosionEffect();
            
        }
    }

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
        itemTag.text = text;
    }
}