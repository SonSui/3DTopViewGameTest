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
    private PlayerController playerController;
    private CameraFollow cameraFollow;

    public Vector3 defPlayerPos = new Vector3(0, 0, 0);
    public Vector3 defCameraPos = new Vector3(5, 5, -5);
    public Vector3 defCameraRot = new Vector3(45, -45, 0);
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

    public bool SpawnPlayer()
    {
        if (player == null)
        {
            player = Instantiate(playerPrefab, defPlayerPos, Quaternion.identity);
            playerController = player.GetComponent<PlayerController>();
            /*if (abilityManager != null)
            {
                abilityManager.player = playerController;
            }
            else
            {
                abilityManager = FindObjectOfType<AbilityManager>();
                if (abilityManager == null)
                {
                    Debug.Log("AbilityManger が見つけません");
                    return false;
                }
                abilityManager.player = playerController;
            }*/
        }
        return true;
    }
    public bool SpawnCamera()
    {
        if(camera1==null)
        {
            Vector3 cameraPosition = defPlayerPos + defCameraPos; // プレイヤーの相対位置に配置
            Quaternion cameraRotation = Quaternion.Euler(defCameraRot); // カメラの角度を設定

            // カメラのインスタンス化
            camera1 = Instantiate(cameraPrefab);
            cameraFollow = camera1.GetComponent<CameraFollow>(); // CameraFollowスクリプトを取得

            camera1.transform.position = cameraPosition;
            camera1.transform.rotation = cameraRotation;

            // カメラがプレイヤーを追従するように設定
            cameraFollow.SetTarget(player.transform);
        }
        return false;
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
    private void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        SpawnPlayer();
        SpawnCamera();
    }


}