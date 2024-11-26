﻿using UnityEngine;
using UnityEngine.UI;


public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    
    

    public float smoothTime = 0.2f;

    // UI要素
    public GameObject uiCanvasPrefab; // CanvasのPrefab
    private GameObject cameraUI;
    private Text cameraStatusText;    // カメラ状態を表示するText
    private Slider posXSlider;
    private Slider posYSlider;
    private Slider posZSlider;
    private Slider rotXSlider;
    private Slider rotYSlider;
    private Slider rotZSlider;

    // モノクロ
    public Material monoTone;
    private float targetAmount = 0f;
    private float currentAmount = 0f;
    private float transitionSpeed = 1f;

    // 角度調整
    
    private bool isOccluded = false;
    private Vector3 desiredPosition;
    private Vector3 oriRot;
    private float pos2Y;
    private Vector3 rot2;
    private float occlusionCheckTimer = 0f;
    private float occlusionCheckDelay = 0.2f; // Adjust as needed
    private bool previousOcclusionState = false;






    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform; // キャラとカメラの偏差を設定
            }
            else
            {
                Debug.LogError("Player not found in the scene.");
            }
        }

        if (monoTone == null)
        {
            Debug.LogError("There is no Material of shader");
        }

        offset = transform.position - target.position;
        pos2Y = offset.magnitude;
        oriRot = transform.eulerAngles;
        rot2 = new Vector3 (80f, oriRot.y, oriRot.z);
        

        // UIの初期設定
        SetupUIComponents();
        UpdateSliders(); // 初期状態のスライダーを更新
    }

    void Update()
    {

        //モノクロ更新
        currentAmount = Mathf.Lerp(currentAmount, targetAmount, Time.deltaTime * transitionSpeed);
    }

    void LateUpdate()
    {

        // キャラに追跡
        if (target != null)
        {


            desiredPosition = target.position + offset;
            Vector3 pos2 = new Vector3(target.position.x, target.position.y + pos2Y, target.position.z); ;
            

            Vector3 footOffset = new Vector3(0f, 0.8f, 0f);
            Vector3 directionToTarget = target.position - footOffset - desiredPosition;

            // 射線を飛ばして遮蔽物を検出
            RaycastHit[] hits = Physics.RaycastAll(desiredPosition, directionToTarget);
            bool buildingOcclusion = false;
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Building"))
                {
                    buildingOcclusion = true;
                    break;
                }
            }
            isOccluded = buildingOcclusion;

            if (isOccluded != previousOcclusionState)
            {
                occlusionCheckTimer += Time.deltaTime;
                if (occlusionCheckTimer >= occlusionCheckDelay)
                {
                    previousOcclusionState = isOccluded;
                    occlusionCheckTimer = 0f;
                }
            }
            else
            {
                
                occlusionCheckTimer = 0f;
            }


            if (previousOcclusionState)
            {
                //位置を円滑に更新
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot2), Time.deltaTime / smoothTime);
                transform.position = Vector3.Lerp(transform.position, pos2, Time.deltaTime / smoothTime);
            }
            else
            {
                //位置を円滑に更新
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(oriRot), Time.deltaTime / smoothTime);
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime / smoothTime);
            }
            
            




            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SwitchCameraUI();
            }

        }

    }


    public void RotateAroundTarget(float angle)
    {
        // ワールド空間での回転軸を計算（オブジェクトのローカルX軸）
        Vector3 rotationAxis = transform.right;

        // オフセットを回転軸周りに回転
        Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);
        Vector3 rotatedOffset = rotation * offset;

        // オブジェクトの位置を更新
        desiredPosition += rotatedOffset;

        
    }


    public void SetOffset(Vector3 pos)
    {
        offset = pos;
    }

    public void SetRotate(Vector3 rot)
    {
        transform.Rotate(rot);
    }

    private void SwitchCameraUI()
    {
        if (cameraUI.activeInHierarchy)
        {
            cameraUI.SetActive(false);
        }
        else
        {
            cameraUI.SetActive(true);
            UpdateSliders ();
        }
    }

    // スライダーの範囲を0から360に拡張
    private void SetupUIComponents()
    {
        if (uiCanvasPrefab != null)
        {
            // CanvasのPrefabをインスタンス化
            cameraUI = Instantiate(uiCanvasPrefab);
            

            // カメラ状態を表示するTextを取得
            cameraStatusText = cameraUI.GetComponentInChildren<Text>();
            if (cameraStatusText != null)
            {
                // Textオブジェクトの子オブジェクトからスライダーを取得
                posXSlider = cameraStatusText.transform.Find("SliderPosX").GetComponent<Slider>();
                posYSlider = cameraStatusText.transform.Find("SliderPosY").GetComponent<Slider>();
                posZSlider = cameraStatusText.transform.Find("SliderPosZ").GetComponent<Slider>();

                rotXSlider = cameraStatusText.transform.Find("SliderRotX").GetComponent<Slider>();
                rotYSlider = cameraStatusText.transform.Find("SliderRotY").GetComponent<Slider>();
                rotZSlider = cameraStatusText.transform.Find("SliderRotZ").GetComponent<Slider>();

                // スライダーの設定
                ConfigureSlider(posXSlider, -30f, 30f, OnPositionSliderChanged);
                ConfigureSlider(posYSlider, -30f, 30f, OnPositionSliderChanged);
                ConfigureSlider(posZSlider, -30f, 30f, OnPositionSliderChanged);

                // 回転スライダーの範囲を0から360に設定
                ConfigureSlider(rotXSlider, 0f, 360f, OnRotationSliderChanged);
                ConfigureSlider(rotYSlider, 0f, 360f, OnRotationSliderChanged);
                ConfigureSlider(rotZSlider, 0f, 360f, OnRotationSliderChanged);
            }
            else
            {
                Debug.LogError("CameraStatusText not found in the Canvas.");
            }
            cameraUI.SetActive(false);
        }
        else
        {
            Debug.LogError("UI Canvas Prefab is not assigned.");
        }
    }
    // スライダーの範囲とリスナーを設定するヘルパーメソッド
    private void ConfigureSlider(Slider slider, float minValue, float maxValue, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider != null)
        {
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.onValueChanged.AddListener(callback);
        }
    }

    // カメラの状態に基づいてスライダーを更新
    private void UpdateSliders()
    {
        //
        if (posXSlider != null) posXSlider.onValueChanged.RemoveListener(OnPositionSliderChanged);
        if (posYSlider != null) posYSlider.onValueChanged.RemoveListener(OnPositionSliderChanged);
        if (posZSlider != null) posZSlider.onValueChanged.RemoveListener(OnPositionSliderChanged);
        if (rotXSlider != null) rotXSlider.onValueChanged.RemoveListener(OnRotationSliderChanged);
        if (rotYSlider != null) rotYSlider.onValueChanged.RemoveListener(OnRotationSliderChanged);
        if (rotZSlider != null) rotZSlider.onValueChanged.RemoveListener(OnRotationSliderChanged);

        // 
        if (posXSlider != null) posXSlider.value = offset.x;
        if (posYSlider != null) posYSlider.value = offset.y;
        if (posZSlider != null) posZSlider.value = offset.z;
        if (rotXSlider != null) rotXSlider.value = transform.eulerAngles.x;
        if (rotYSlider != null) rotYSlider.value = transform.eulerAngles.y;
        if (rotZSlider != null) rotZSlider.value = transform.eulerAngles.z;

        // 
        if (posXSlider != null) posXSlider.onValueChanged.AddListener(OnPositionSliderChanged);
        if (posYSlider != null) posYSlider.onValueChanged.AddListener(OnPositionSliderChanged);
        if (posZSlider != null) posZSlider.onValueChanged.AddListener(OnPositionSliderChanged);
        if (rotXSlider != null) rotXSlider.onValueChanged.AddListener(OnRotationSliderChanged);
        if (rotYSlider != null) rotYSlider.onValueChanged.AddListener(OnRotationSliderChanged);
        if (rotZSlider != null) rotZSlider.onValueChanged.AddListener(OnRotationSliderChanged);

        UpdateCameraStatusText();
    }

    // 位置スライダーが変更されたときの処理
    private void OnPositionSliderChanged(float value)
    {
        if (posXSlider != null && posYSlider != null && posZSlider != null)
        {
            Vector3 newPosition = new Vector3(posXSlider.value, posYSlider.value, posZSlider.value);
            offset = newPosition;
            UpdateCameraStatusText();
        }
    }

    // カメラの回転スライダーが変更されたときの処理
    private void OnRotationSliderChanged(float value)
    {
        if (rotXSlider != null && rotYSlider != null && rotZSlider != null)
        {
            float xRotation = rotXSlider.value;
            float yRotation = rotYSlider.value;
            float zRotation = rotZSlider.value;

            // Quaternionで回転を設定
            transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);

            UpdateCameraStatusText();
        }
    }

    // カメラの状態をTextに更新
    private void UpdateCameraStatusText()
    {
        if (cameraStatusText != null)
        {
            cameraStatusText.text = "Camera\n" +
                "RotX: " + transform.eulerAngles.x.ToString("F2") + "\n" +
                "RotY: " + transform.eulerAngles.y.ToString("F2") + "\n" +
                "RotZ: " + transform.eulerAngles.z.ToString("F2") + "\n" +
                "PosX: " + transform.position.x.ToString("F2") + "\n" +
                "PosY: " + transform.position.y.ToString("F2") + "\n" +
                "PosZ: " + transform.position.z.ToString("F2");
        }
    }


    //モノクロ
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        monoTone.SetFloat("_GrayScaleAmount", currentAmount);
        Graphics.Blit(src, dest, monoTone);
    }

    public void MonoTone_SetSpeed(float speed)
    {
        transitionSpeed = speed;
    }

    public void MonoTone_Enable()
    {
        targetAmount = 1.0f;
    }

    public void MonoTone_Disable()
    {
        targetAmount = 0.0f;
    }

    public void SetTarget(Transform target_)
    {
        target = target_;
    }
}
