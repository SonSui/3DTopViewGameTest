using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float defCameraFieldView = 70f;

    public GameObject uiPrefab;
    private UIManager uiManager;

    private bool isPlayerDead = false;

    public List<GameObject> commonItems = new List<GameObject>();

    // �H�L�ȃh���b�v�p�̃A�C�e�����X�g
    public List<GameObject> rareItems = new List<GameObject>();

    // ���݂̃X�e�[�W�ԍ��i1����n�܂�j
    private int currentStage = 1;

    // �h���b�v���ꂽ�A�C�e����ǐՂ���C���f�b�N�X
    private int commonDropIndex = 0;
    private int rareDropIndex = 0;

    // �X�e�[�W��i�߂�
    

    private void Awake()
    {
        // �V���O���g���p�^�[���̎����FGameManager��1�������݂��Ȃ��悤�ɂ���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[���J�ڎ��ɍ폜����Ȃ��悤�ɂ���
        }
        else
        {
            Destroy(gameObject); // ���łɑ��݂���ꍇ�͍폜����
        }
    }

    private void Start()
    {

        Application.targetFrameRate = 60;

    }

    private bool SpawnPlayer()
    {
        if (player == null)
        {
            player = Instantiate(playerPrefab, defPlayerPos, Quaternion.identity);

            playerControl = player.GetComponent<PlayerControl>();
            //playerControl.SetActSpeed(1.5f);

        }
        return true;
    }
    private bool SpawnCamera()
    {
        if (camera1 == null)
        {
            Vector3 cameraPosition = defPlayerPos + defCameraPos; // �v���C���[�̑��Έʒu�ɔz�u
            Quaternion cameraRotation = Quaternion.Euler(defCameraRot); // �J�����̊p�x��ݒ�

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
        uiManager = FindAnyObjectByType<UIManager>();
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
            SpawnPlayer();
            SpawnCamera();
            StageManager.Instance?.SetStageNum(currentStage);
        }
        else if(scene.name =="Title")
        {
            ResetPlayerStatus();
            ResetDropInfo();
            FindUIManager();
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

    // ===== ItemPool =====


    // ���݂̃X�e�[�W�Ɋ�Â��Ēʏ�h���b�v���擾����
    public GameObject GetCommonDrop()
    {
        return GetItemFromPool(commonItems, ref commonDropIndex);
    }

    // ���݂̃X�e�[�W�Ɋ�Â��ă��A�h���b�v���擾����
    public GameObject GetRareDrop()
    {
        return GetItemFromPool(rareItems, ref rareDropIndex);
    }

    // �w�肵���v�[������A�C�e�����擾����������\�b�h
    private GameObject GetItemFromPool(List<GameObject> pool, ref int dropIndex)
    {
        if (dropIndex < pool.Count)
        {
            GameObject item = pool[dropIndex];
            dropIndex++; // ���̃A�C�e���ɐi��
            return item;
        }
        else
        {
            // ���ׂẴA�C�e�����h���b�v�ς݂̏ꍇ
            Debug.LogWarning("���ׂẴA�C�e�����h���b�v�ς݂ł��B");
            return null;
        }
    }

    // �h���b�v�������Z�b�g����
    public void ResetDropInfo()
    {
        currentStage = 1; // �X�e�[�W�����Z�b�g
        commonDropIndex = 0; // �ʏ�h���b�v�̃C���f�b�N�X�����Z�b�g
        rareDropIndex = 0; // ���A�h���b�v�̃C���f�b�N�X�����Z�b�g
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
    }
    public void RecoverHP()
    {
        playerStatus.OnHpRecover();
        uiManager.SetHP(playerStatus.GetHpNow(), playerStatus.GetHpMax());
    }
    public void AdvanceStage()
    {
        currentStage++;
    }
}