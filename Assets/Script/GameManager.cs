using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class AbilityTagEntry
{
    public AbilityTagDefinition tag;
    public int limit;
}

public class GameManager : MonoBehaviour
{
    // GameManager�̃C���X�^���X��ێ�����ÓI�ϐ�
    public static GameManager Instance { get; private set; }

    public PlayerStatus playerStatus;

    // �v���C���[�ƃJ������Prefab
    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    public AbilityManager abilityManager;

    private GameObject camera1;
    private GameObject player;

    // �v���C���[��PlayerController�X�N���v�g�ƃJ������CameraFollow�X�N���v�g

    private PlayerControl playerControl;
    private CameraFollow cameraFollow;

    public Vector3 defPlayerPos = new Vector3(0, 0, 0);
    public Vector3 defCameraPos = new Vector3(5, 5, -5);
    public Vector3 defCameraRot = new Vector3(45, -45, 0);
    public Vector3 bossCameraPos = new Vector3(23, 7, -20);
    public Vector3 bossCameraRot = new Vector3(15,0,0);
    public float defCameraFieldView = 70f;

    public GameObject uiPrefab;
    private UIManager uiManager;

    private bool isPlayerDead = false;

    public GameObject ItemsPrefab;
    // ���݂̃X�e�[�W�ԍ��i1����n�܂�j
    private int currentStage = 1;
    public const int MAX_STAGE = 3;

    public float gameTime = 0f;


    //�h���b�v
    
    [SerializeField]
    private List<AbilityTagEntry> tagsPoolList = new List<AbilityTagEntry>();
    [SerializeField]
    private GameObject superRarePrefab;

    private Dictionary<AbilityTagDefinition,int> tagsPool = new Dictionary<AbilityTagDefinition, int>();


    private void Start()
    {
        Application.targetFrameRate = 60;
        FadeManager.Instance.FadeIn(2f);
    }


    private void Awake()
    {

        // �V���O���g���p�^�[���̎����FGameManager��1�������݂��Ȃ��悤�ɂ���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[���J�ڎ��ɍ폜����Ȃ��悤�ɂ���
            foreach (var entry in tagsPoolList)
            {
                if (entry.tag != null)
                {
                    tagsPool[entry.tag] = entry.limit;
                }
            }
        }
        else
        {
            Destroy(gameObject); // ���łɑ��݂���ꍇ�͍폜����
        }
    }

    /*private void Start()
    {

        Application.targetFrameRate = 60;
        
    }*/
    private void Update()
    {
        gameTime += Time.deltaTime;
    }

    private bool SpawnPlayer()
    {
        if (player == null)
        {
            player = Instantiate(playerPrefab, defPlayerPos, Quaternion.identity);

            playerControl = player.GetComponent<PlayerControl>();
            playerControl.SetActSpeed(playerStatus.GetAttackSpeed());
            playerControl.RecordWeaponOriginalScales();
            playerControl.SetSwordCube(playerStatus.GetAttackRange());

        }
        return true;
    }
    private bool SpawnCamera()
    {
        if (camera1 == null)
        {
            Vector3 cameraPosition = defPlayerPos + defCameraPos; // �v���C���[�̑��Έʒu�ɔz�u
            Quaternion cameraRotation = Quaternion.Euler(defCameraRot); // �J�����̊p�x��ݒ�
            if (SceneManager.GetActiveScene().name=="Boss")
            {
                cameraPosition = defPlayerPos + bossCameraPos;
                cameraRotation = Quaternion.Euler(bossCameraRot);
            }
            // �J�����̃C���X�^���X��
            camera1 = Instantiate(cameraPrefab);
            cameraFollow = camera1.GetComponent<CameraFollow>(); // CameraFollow�X�N���v�g���擾

            camera1.transform.position = cameraPosition;
            camera1.transform.rotation = cameraRotation;
            camera1.GetComponentInParent<Camera>().fieldOfView = defCameraFieldView;

            // �J�������v���C���[��Ǐ]����悤�ɐݒ�
            cameraFollow.SetTarget(player.transform);

            if (uiManager != null) uiManager.SetMainCamera(camera1.GetComponent<Camera>());
        }
        return false;
    }

    private void FindUIManager()
    {
        if (uiManager == null) uiManager = FindAnyObjectByType<UIManager>();
        if (uiManager == null)
        {
            GameObject obj = Instantiate(uiPrefab);
            uiManager = obj.GetComponent<UIManager>();
        }
        if (playerStatus != null)
        {
            uiManager.SetHP(playerStatus.GetHpNow(), playerStatus.GetHpMax());
            Dictionary<AbilityTagDefinition, int> currTags = playerStatus.GetCollectedTagDefinitions();
            uiManager.SetCurrentTags(currTags);
            uiManager.SetAmmo(playerStatus.GetAmmoCapacity(), playerStatus.GetAmmoMax());
        }
    }

    public GameObject GetPlayer()
    {
        return player;
    }
    public GameObject GetCamera()
    {
        return camera1;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        UnpauseGame();
        if (scene.name != "Title" && scene.name != "Test_BossScene")
        { 
            FindUIManager();
            uiManager.DisableAllDamageText();
            SpawnPlayer();
            SpawnCamera();
            StageManager.Instance?.SetStageNum(currentStage);
        }
        else if(scene.name =="Title")
        {
            ResetPlayerStatus();
            ResetStageInfo();
            FindUIManager();
            uiManager.DisableAllDamageText();

        }
        StartCoroutine(ResumeAfterFade());
        if(scene.name == "Tutorial")
        {
            gameTime = 0f;
        }
    }
    public void GameStart_Initialize()
    {
        ResetPlayerStatus();
    }

    public void PauseGame()
    {
        Time.timeScale = 0.02f;
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1f;
    }
    private IEnumerator ResumeAfterFade()
    {
        FadeManager.Instance.FadeOut(0.01f);
        yield return new WaitForSecondsRealtime(0.5f); 
        FadeManager.Instance.FadeIn(2f); 
    }


    // ===== ItemPool =====

    // �ʏ�h���b�v���擾����
    public GameObject GetCommonDrop()
    {
        List<AbilityTagDefinition> availableTags = new List<AbilityTagDefinition>();
        Dictionary<AbilityTagDefinition, int> currTags = playerStatus.GetCollectedTagDefinitions();

        // tagsPool������W�ς݂̏���ɒB���Ă��Ȃ��^�O���擾
        foreach (var tag in tagsPool)
        {
            if (!currTags.ContainsKey(tag.Key) || currTags[tag.Key] < tag.Value)
            {
                availableTags.Add(tag.Key);
            }
        }

        // ���p�\�ȃ^�O���Ȃ��ꍇ�͌x�����o���ďI��
        if (availableTags.Count <= 0)
        {
            ItemData nullData = ItemData.CreateInstance(rare_: 0);
            ItemsPrefab.GetComponent<Item>().InitializeItem(nullData);
            Debug.LogWarning("No available tags for common drop!");
            return ItemsPrefab;
        }

        // 1�̃^�O�������_���I��
        AbilityTagDefinition selectedTag = availableTags[UnityEngine.Random.Range(0, availableTags.Count)];

        // ItemData�̍쐬
        ItemData itemData = ItemData.CreateInstance(
            rare_: 0,
            tags_: new List<AbilityTagDefinition> { selectedTag }
        );

        // ItemsPrefab�ɐݒ�
        ItemsPrefab.GetComponent<Item>().InitializeItem(itemData);
        return ItemsPrefab;
    }

    // ���A�h���b�v���擾����
    public GameObject GetRareDrop()
    {
        List<AbilityTagDefinition> availableTags = new List<AbilityTagDefinition>();
        Dictionary<AbilityTagDefinition, int> currTags = playerStatus.GetCollectedTagDefinitions();

        // tagsPool������W�ς݂̏���ɒB���Ă��Ȃ��^�O���擾
        foreach (var tag in tagsPool)
        {
            if (!currTags.ContainsKey(tag.Key) || currTags[tag.Key] < tag.Value)
            {
                availableTags.Add(tag.Key);
            }
        }

        // ���p�\�ȃ^�O��2�����̏ꍇ�͌x�����o���ďI��
        if (availableTags.Count < 2)
        {
            ItemData nullData = ItemData.CreateInstance(rare_: 0);
            ItemsPrefab.GetComponent<Item>().InitializeItem(nullData);
            Debug.LogWarning("Not enough available tags for rare drop!");
            return ItemsPrefab;
        }

        // 2�`3�̃^�O�������_���I��
        float tagRate = Mathf.Max((float)currentStage-0.8f,0f) / (float)MAX_STAGE;
        float randNum = UnityEngine.Random.Range(0f, 1f);
        int numTags = 2;
        if (randNum < tagRate)numTags = 3;
        List<AbilityTagDefinition> selectedTags = new List<AbilityTagDefinition>();

        for (int i = 0; i < numTags && availableTags.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, availableTags.Count);
            selectedTags.Add(availableTags[index]);
            availableTags.RemoveAt(index); // �I�������^�O�����X�g����폜
        }

        // ItemData�̍쐬
        ItemData itemData = ItemData.CreateInstance(
            rare_: UnityEngine.Random.Range(1, 4),
            tags_: selectedTags
        );

        // ItemsPrefab�ɐݒ�
        ItemsPrefab.GetComponent<Item>().InitializeItem(itemData);
        return ItemsPrefab;
    }
    public GameObject GetSuperRareItem()
    {
        return superRarePrefab;
    }
    public void AdvanceStage()
    {
        currentStage++;
    }
    private void ResetStageInfo()
    {
        currentStage = 1;
    }

    // ===== Player =====
    private void ResetPlayerStatus()
    {
        playerStatus = new PlayerStatus(5, 6);
        isPlayerDead = false;
    }

    public bool IsHaveAmmo()=>playerStatus.IsHaveAmmo();
    public void UseAmmo()
    {
        playerStatus.UseAmmo();
        uiManager.SetAmmo(playerStatus.GetAmmoCapacity(), playerStatus.GetAmmoMax());
    }
    public int GetPlayerAttackNow()
    {
        return playerStatus.GetAttackNow();
    }
    public int PlayerTakeDamage(int dmg)
    {
        int realDmg = playerStatus.ReturnTakeDamage(dmg);
        Debug.Log($"Player Take {realDmg}Damage,Remaining{playerStatus.GetHpNow()}");
        if(realDmg > 0)
        {
            uiManager.TakeDamage(realDmg);
        }
        if(playerStatus.IsDead())
        {
            playerControl.OnDying();
        }

        return realDmg;
    }
    public void PlayerDeadAnimeOver()=>isPlayerDead = true;

    public bool IsPlayerDead()
    {
        return isPlayerDead;
    }
    public void EquipItem(ItemData item)
    {
        Debug.Log($"{item.itemName} in gameManager");
        playerStatus.OnItemCollected(item);
        Dictionary<AbilityTagDefinition, int> currTags = playerStatus.GetCollectedTagDefinitions();
        uiManager.SetCurrentTags(currTags);
        uiManager.SetHP(playerStatus.GetHpNow(), playerStatus.GetHpMax());
        uiManager.SetAmmo(playerStatus.GetAmmoCapacity(), playerStatus.GetAmmoMax());
        playerControl.SetActSpeed(playerStatus.GetAttackSpeed());
        playerControl.SetSwordCube(playerStatus.GetAttackRange());
        playerControl.EquipEffect();
    }
    public void RecoverHP(int n = 1)
    {
        playerStatus.OnHpRecover(n);
        uiManager.SetHP(playerStatus.GetHpNow(), playerStatus.GetHpMax());
        playerControl.HealEffect();
    }


    
}