using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManagerのインスタンスを保持する静的変数
    public static GameManager Instance { get; private set; }

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
    public Vector3 defCameraPos = new Vector3(0, 8, -5);
    public Vector3 defCameraRot = new Vector3(45, 0, 0);
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
        // プレイヤーの生成
        player = Instantiate(playerPrefab, defPlayerPos, Quaternion.identity);
        playerController = player.GetComponent<PlayerController>(); // PlayerControllerスクリプトを取得
        if (abilityManager != null)
        {
            abilityManager.player = playerController;
        }
        else
        {
            abilityManager = FindObjectOfType<AbilityManager>();
            if(abilityManager == null) 
            {
                Debug.Log("AbilityManger が見つけません");
                return;
            }
            abilityManager.player = playerController;
        }

        // カメラの生成
        Vector3 cameraPosition = player.transform.position + defCameraPos; // プレイヤーの相対位置に配置
        //Quaternion cameraRotation = Quaternion.Euler(defCameraRot); // カメラの角度を設定

        // カメラのインスタンス化
        camera1 = Instantiate(cameraPrefab, cameraPosition, Quaternion.identity);
        cameraFollow = camera1.GetComponent<CameraFollow>(); // CameraFollowスクリプトを取得

        // カメラの初期回転を再設定
        camera1.transform.Rotate(defCameraRot.x,0,0);
        camera1.transform.Rotate(0, defCameraRot.y, 0);

        // カメラがプレイヤーを追従するように設定
        cameraFollow.SetTarget(player.transform);
    }

    public GameObject GetPlayer()
    {
        return player;
    }
    public GameObject GetCamera()
    { 
        return camera1; 
    }
}