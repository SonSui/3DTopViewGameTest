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
    private PlayerController playerController;
    private CameraFollow cameraFollow;

    public Vector3 defPlayerPos = new Vector3(0, 0, 0);
    public Vector3 defCameraPos = new Vector3(5, 5, -5);
    public Vector3 defCameraRot = new Vector3(45, -45, 0);
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
                    Debug.Log("AbilityManger �������܂���");
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
            Vector3 cameraPosition = defPlayerPos + defCameraPos; // �v���C���[�̑��Έʒu�ɔz�u
            Quaternion cameraRotation = Quaternion.Euler(defCameraRot); // �J�����̊p�x��ݒ�

            // �J�����̃C���X�^���X��
            camera1 = Instantiate(cameraPrefab);
            cameraFollow = camera1.GetComponent<CameraFollow>(); // CameraFollow�X�N���v�g���擾

            camera1.transform.position = cameraPosition;
            camera1.transform.rotation = cameraRotation;

            // �J�������v���C���[��Ǐ]����悤�ɐݒ�
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