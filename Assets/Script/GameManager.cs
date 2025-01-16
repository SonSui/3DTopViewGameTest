using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // GameManagerのインスタンスを保持する静的変数
    public static GameManager Instance { get; private set; }

    public PlayerStatus playerStatus;

    // プレイヤーとカメラのPrefab
    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    public AbilityManager abilityManager;

    private GameObject camera1;
    private GameObject player;

    // プレイヤーのPlayerControllerスクリプトとカメラのCameraFollowスクリプト

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

    // 稀有なドロップ用のアイテムリスト
    public List<GameObject> rareItems = new List<GameObject>();

    // 現在のステージ番号（1から始まる）
    private int currentStage = 1;

    // ドロップされたアイテムを追跡するインデックス
    private int commonDropIndex = 0;
    private int rareDropIndex = 0;

    // ステージを進める
    

    private void Awake()
    {
        // シングルトンパターンの実装：GameManagerが1つしか存在しないようにする
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移時に削除されないようにする
        }
        else
        {
            Destroy(gameObject); // すでに存在する場合は削除する
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
            Vector3 cameraPosition = defPlayerPos + defCameraPos; // プレイヤーの相対位置に配置
            Quaternion cameraRotation = Quaternion.Euler(defCameraRot); // カメラの角度を設定

            // カメラのインスタンス化
            camera1 = Instantiate(cameraPrefab);
            cameraFollow = camera1.GetComponent<CameraFollow>(); // CameraFollowスクリプトを取得

            camera1.transform.position = cameraPosition;
            camera1.transform.rotation = cameraRotation;
            camera1.GetComponentInParent<Camera>().fieldOfView = defCameraFieldView;

            // カメラがプレイヤーを追従するように設定
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


    // 現在のステージに基づいて通常ドロップを取得する
    public GameObject GetCommonDrop()
    {
        return GetItemFromPool(commonItems, ref commonDropIndex);
    }

    // 現在のステージに基づいてレアドロップを取得する
    public GameObject GetRareDrop()
    {
        return GetItemFromPool(rareItems, ref rareDropIndex);
    }

    // 指定したプールからアイテムを取得する内部メソッド
    private GameObject GetItemFromPool(List<GameObject> pool, ref int dropIndex)
    {
        if (dropIndex < pool.Count)
        {
            GameObject item = pool[dropIndex];
            dropIndex++; // 次のアイテムに進む
            return item;
        }
        else
        {
            // すべてのアイテムがドロップ済みの場合
            Debug.LogWarning("すべてのアイテムがドロップ済みです。");
            return null;
        }
    }

    // ドロップ情報をリセットする
    public void ResetDropInfo()
    {
        currentStage = 1; // ステージをリセット
        commonDropIndex = 0; // 通常ドロップのインデックスをリセット
        rareDropIndex = 0; // レアドロップのインデックスをリセット
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