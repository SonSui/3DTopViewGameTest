using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManager�̃C���X�^���X��ێ�����ÓI�ϐ�
    public static GameManager Instance { get; private set; }

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
    public Vector3 defCameraPos = new Vector3(0, 8, -5);
    public Vector3 defCameraRot = new Vector3(45, 0, 0);
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
        // �v���C���[�̐���
        player = Instantiate(playerPrefab, defPlayerPos, Quaternion.identity);
        playerController = player.GetComponent<PlayerController>(); // PlayerController�X�N���v�g���擾
        if (abilityManager != null)
        {
            abilityManager.player = playerController;
        }
        else
        {
            abilityManager = FindObjectOfType<AbilityManager>();
            if(abilityManager == null) 
            {
                Debug.Log("AbilityManger �������܂���");
                return;
            }
            abilityManager.player = playerController;
        }

        // �J�����̐���
        Vector3 cameraPosition = player.transform.position + defCameraPos; // �v���C���[�̑��Έʒu�ɔz�u
        //Quaternion cameraRotation = Quaternion.Euler(defCameraRot); // �J�����̊p�x��ݒ�

        // �J�����̃C���X�^���X��
        camera1 = Instantiate(cameraPrefab, cameraPosition, Quaternion.identity);
        cameraFollow = camera1.GetComponent<CameraFollow>(); // CameraFollow�X�N���v�g���擾

        // �J�����̏�����]���Đݒ�
        camera1.transform.Rotate(defCameraRot.x,0,0);
        camera1.transform.Rotate(0, defCameraRot.y, 0);

        // �J�������v���C���[��Ǐ]����悤�ɐݒ�
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