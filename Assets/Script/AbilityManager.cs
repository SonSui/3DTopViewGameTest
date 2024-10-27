using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public class AbilityManager : MonoBehaviour
{
    // ���ׂẴ^�O
    public List<AbilityTagDefinition> abilityTagDefinitions;

    // �^�O�̐����ƌ���
    private Dictionary<string, int> tagCounts = new Dictionary<string, int>();
    private Dictionary<string, AbilityEffect> currentEffects = new Dictionary<string, AbilityEffect>();

    // �W�߂�����
    private List<Item> collectedItems = new List<Item>();

    // �W�߂����̂�\��
    public IReadOnlyList<Item> CollectedItems => collectedItems.AsReadOnly();

    // UI�֘A
    public GameObject uiCanvasPrefab; // �v���C���[���p�ӂ���Canvas��Prefab
    private GameObject parameterUI;
    private Text playerStatusText;    // �v���C���[��ԕ\���p��Text
    private Text itemTag;             // �A�C�e���\���p��Text
    private InputField[] inputFields; // �v���C���[��ԕҏW�p��InputField

    public PlayerController player;

    public static AbilityManager Instance { get; private set; }

    private void Awake()
    {
        // �V���O���g���p�^�[���̎���
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
        // �v���C���[��T��
        StartCoroutine(WaitForPlayer());

        // UI�̏����ݒ�
        SetupUI();
        Update_UI(); // UI�̏�����Ԃ��X�V
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchParameterUI();
        }
    }

    // �v���C���[�̐�����҂R���[�`��
    private IEnumerator WaitForPlayer()
    {
        // GameManager���v���C���[�𐶐�����܂őҋ@
        while (player == null)
        {
            player = GameManager.Instance?.GetPlayer()?.GetComponent<PlayerController>();
            if (player == null)
            {
                Debug.Log("Player��������܂���A�ҋ@��...");
                yield return null; // ���̃t���[���܂ő҂�
            }
        }
        Debug.Log("Player��������܂����I");
        Update_UI(); // �v���C���[������������UI���X�V
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


    // UI�̏����ݒ�
    private void SetupUI()
    {
        // Canvas��Prefab���C���X�^���X��
        if (uiCanvasPrefab != null)
        {
            parameterUI = Instantiate(uiCanvasPrefab);
            DontDestroyOnLoad(parameterUI); // �V�[���J�ڎ��ɍ폜����Ȃ��悤�ɐݒ�

            // �v���C���[��ԕ\���p��Text���擾
            Transform statusTextTransform = parameterUI.transform.Find("PlayerStatusText");
            if (statusTextTransform != null)
                playerStatusText = statusTextTransform.GetComponent<Text>();

            // �A�C�e���\���p��Text���擾
            Transform itemTagTransform = parameterUI.transform.Find("ItemTagText");
            if (itemTagTransform != null)
                itemTag = itemTagTransform.GetComponent<Text>();

            // InputField�̎擾
            inputFields = statusTextTransform.GetComponentsInChildren<InputField>();
            for (int i = 0; i < inputFields.Length; i++)
            {
                int index = i; // ���[�J���ϐ��Ƃ��ăC���f�b�N�X��ۑ�
                inputFields[i].onEndEdit.AddListener((input) => UpdatePlayerStatusFromInput(index, input));
            }
        }
        else
        {
            Debug.LogError("UI Canvas Prefab is not assigned.");
        }
        parameterUI.SetActive(false);
    }

    // InputField����v���C���[�̏�Ԃ��X�V
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
                // �K�v�ɉ����đ��̃X�e�[�^�X��ǉ�
                default: break;
            }

            player.finalState.ShowState(); // �v���C���[��Ԃ̕\�����X�V
            Update_UI(); // UI�S�̂̍X�V
        }
        else
        {
            Debug.LogWarning("���͂������ł�: " + input);
        }
    }

    // �V�������̂��W�߂郁�\�b�h
    public void CollectItem(Item item)
    {
        collectedItems.Add(item);

        foreach (string tag in item.tags)
        {
            // �����Ă���^�O�̐��ʂ𑝉�
            if (tagCounts.ContainsKey(tag))
                tagCounts[tag]++;
            else
                tagCounts[tag] = 1;

            // ���ʂ��X�V
            UpdateAbilityEffect(tag);
        }
    }

    // ���ʂ��X�V���郁�\�b�h
    private void UpdateAbilityEffect(string tag)
    {
        if (abilityTagDefinitions == null || abilityTagDefinitions.Count == 0)
        {
            Debug.LogError("AbilityTagDefinitions is null");
            return;
        }

        // �^�O��T��
        AbilityTagDefinition tagDefinition = abilityTagDefinitions.Find(t => t.tagName == tag);
        if (tagDefinition == null)
        {
            Debug.Log("Can't Find tag:" + tag);
            return;
        }

        int count = tagCounts[tag];
        AbilityEffect totalEffect = new AbilityEffect();

        // �^�O���ʂ����Z
        for (int i = 0; i < tagDefinition.thresholds.Count; i++)
        {
            if (count >= tagDefinition.thresholds[i])
            {
                AbilityEffect effect = tagDefinition.effects[i];
                totalEffect += effect;
            }
        }

        // �^�O���ʂ��X�V
        if (currentEffects.ContainsKey(tag))
            currentEffects[tag] = totalEffect;
        else
            currentEffects.Add(tag, totalEffect);

        // �v���C���[�Ɍ��ʂ�K�p
        ApplyEffects();
    }

    // �v���C���[�Ɍ��ʂ�K�p���郁�\�b�h
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
        Update_UI(); // UI�̍X�V
    }

    // ��ʂɃ^�O�Ə�Ԃ�\�����郁�\�b�h
    private void Update_UI()
    {
        // �v���C���[��Ԃ̕\��
        if (playerStatusText != null && player != null)
        {
            playerStatusText.text = player.GetStateString();
        }

        // �A�C�e���̕\��
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
