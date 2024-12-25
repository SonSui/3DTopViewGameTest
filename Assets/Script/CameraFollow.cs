using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 defOffset;
    public Vector3 defRotation;
    public float defFieldView;

    private Vector3 currTargetOffset;
    private Vector3 currTargetRot;
    private float currTargetView;


    public float smoothTime = 0.1f;

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
    private Slider fieldViewSlider;
    private FlexibleColorPicker colorPicker;

    // モノクロ
    public Material monoTone;
    private float targetAmount = 0f;
    private float currentAmount = 0f;
    private float transitionSpeed = 1f;

    // カメラ変換
    private float occlusionCheckTimer = 0f;
    private float occlusionCheckDelay = 0.2f; // Adjust as needed
    private bool previousOcclusionState = false;
    private bool isOccluded = false;
    private Vector3 desiredPosition;
    private Vector3 rotUpDown;
    private float posUpDownY;
    private float rotUpDownX = 75f; // 角度調整

    private Coroutine zoomShakeCoroutine; // ズームと揺れのコルーチン


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

        defOffset = transform.position - target.position;
        posUpDownY = defOffset.magnitude;
        defRotation = transform.eulerAngles;
        defFieldView = this.GetComponent<Camera>().fieldOfView;
        rotUpDown = new Vector3 (rotUpDownX, defRotation.y, defRotation.z);
        currTargetOffset = defOffset;
        currTargetRot = defRotation;
        currTargetView = defFieldView;

        // UIの初期設定
        SetupUIComponents();
        UpdateSliders(); // 初期状態のスライダーを更新
    }

    void Update()
    {

        //モノクロ更新
        currentAmount = Mathf.Lerp(currentAmount, targetAmount, Time.deltaTime * transitionSpeed);

        if (colorPicker != null&&cameraUI.activeSelf)
        {
            RenderSettings.ambientLight = colorPicker.color;
            Debug.Log("ColorPicker:"+colorPicker.color);
        }

    }

    void LateUpdate()
    {

        // キャラに追跡
        if (target != null)
        {


            desiredPosition = target.position + currTargetOffset;
            Vector3 pos2 = new Vector3(target.position.x, target.position.y + posUpDownY, target.position.z); ;
            

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
                if (zoomShakeCoroutine != null) StopCoroutine(zoomShakeCoroutine);
                //建物の後ろで攻撃処理未実装
                //位置を円滑に更新
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotUpDown), Time.deltaTime / smoothTime);
                transform.position = Vector3.Lerp(transform.position, pos2, Time.deltaTime / smoothTime );
            }
            else
            {
                //位置を円滑に更新
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(defRotation), Time.deltaTime / smoothTime);
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
        Vector3 rotatedOffset = rotation * defOffset;

        // オブジェクトの位置を更新
        desiredPosition += rotatedOffset;

        
    }

    public void ZoomAndShakeCamera(float shakeIntensity = 0.1f, float returnSpeed = 0.5f, float minZoomRate = 0.3f)
    {
        float zoomRate = 0.9f; // 距離を0.1倍縮小
        // コルーチンが実行中の場合、停止する
        if (zoomShakeCoroutine != null)
        {
            StopCoroutine(zoomShakeCoroutine);
        }

        // 現在のオフセットが最小距離を超えない場合、新しいコルーチンを開始
        if (currTargetOffset.magnitude > defOffset.magnitude * minZoomRate)
        {
            Vector3 newTargetOffset = currTargetOffset * zoomRate; 
            zoomShakeCoroutine = StartCoroutine(ZoomAndShakeCoroutine(newTargetOffset, shakeIntensity, returnSpeed));
        }
    }

    private IEnumerator ZoomAndShakeCoroutine(Vector3 newTargetOffset, float shakeIntensity, float returnSpeed)
    {
        // ランダム揺れを加えつつズームインする
        float zoomTime = 0.3f; // ランダム揺れとズームインの持続時間
        float elapsedTime = 0f;

        Vector3 initialOffset = currTargetOffset;

        while (elapsedTime < zoomTime)
        {
            // ランダム揺れオフセット
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity)
            );

            // ズームと揺れを同時に適用
            currTargetOffset = Vector3.Lerp(initialOffset, newTargetOffset, elapsedTime / zoomTime);
            transform.position = target.position + currTargetOffset + shakeOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ズーム完了後、ターゲットオフセットを更新
        currTargetOffset = newTargetOffset;

        // 揺れの終了後、徐々に元のオフセットに戻す
        while (Vector3.Distance(currTargetOffset, defOffset) > 0.01f)
        {
            currTargetOffset = Vector3.Lerp(currTargetOffset, defOffset, Time.deltaTime * returnSpeed);
            transform.position = target.position + currTargetOffset;
            yield return null;
        }

        // オフセットを元の位置に戻す
        currTargetOffset = defOffset;
        zoomShakeCoroutine = null; // コルーチンをリセット
    }



    public void SetOffset(Vector3 pos)
    {
        defOffset = pos;
    }

    public void SetRotate(Vector3 rot)
    {
        transform.Rotate(rot);
    }



    // ===== カメラUI =====
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
                fieldViewSlider = cameraStatusText.transform.Find("SliderFildedView").GetComponent<Slider>();

                // スライダーの設定
                ConfigureSlider(posXSlider, -30f, 30f, OnPositionSliderChanged);
                ConfigureSlider(posYSlider, -30f, 30f, OnPositionSliderChanged);
                ConfigureSlider(posZSlider, -30f, 30f, OnPositionSliderChanged);

                // 回転スライダーの範囲を0から360に設定
                ConfigureSlider(rotXSlider, 0f, 360f, OnRotationSliderChanged);
                ConfigureSlider(rotYSlider, 0f, 360f, OnRotationSliderChanged);
                ConfigureSlider(rotZSlider, 0f, 360f, OnRotationSliderChanged);
                //視野の設定
                ConfigureSlider(fieldViewSlider, 20f, 160f, OnRotationSliderChanged);

                colorPicker = cameraStatusText.transform.Find("FlexibleColorPicker").GetComponent<FlexibleColorPicker>();
                colorPicker.color = RenderSettings.ambientLight;
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
        // 他のパラメータに影響を与えないため一旦停止
        if (posXSlider != null) posXSlider.onValueChanged.RemoveListener(OnPositionSliderChanged);
        if (posYSlider != null) posYSlider.onValueChanged.RemoveListener(OnPositionSliderChanged);
        if (posZSlider != null) posZSlider.onValueChanged.RemoveListener(OnPositionSliderChanged);
        if (rotXSlider != null) rotXSlider.onValueChanged.RemoveListener(OnRotationSliderChanged);
        if (rotYSlider != null) rotYSlider.onValueChanged.RemoveListener(OnRotationSliderChanged);
        if (rotZSlider != null) rotZSlider.onValueChanged.RemoveListener(OnRotationSliderChanged);
        if (fieldViewSlider != null) fieldViewSlider.onValueChanged.RemoveListener(OnFieldViewSliderChanged);

        // スライダーの位置を更新
        if (posXSlider != null) posXSlider.value = defOffset.x;
        if (posYSlider != null) posYSlider.value = defOffset.y;
        if (posZSlider != null) posZSlider.value = defOffset.z;
        if (rotXSlider != null) rotXSlider.value = transform.eulerAngles.x;
        if (rotYSlider != null) rotYSlider.value = transform.eulerAngles.y;
        if (rotZSlider != null) rotZSlider.value = transform.eulerAngles.z;
        if (fieldViewSlider != null) fieldViewSlider.value = defFieldView;
        
        // 更新イベントを戻す 
        if (posXSlider != null) posXSlider.onValueChanged.AddListener(OnPositionSliderChanged);
        if (posYSlider != null) posYSlider.onValueChanged.AddListener(OnPositionSliderChanged);
        if (posZSlider != null) posZSlider.onValueChanged.AddListener(OnPositionSliderChanged);
        if (rotXSlider != null) rotXSlider.onValueChanged.AddListener(OnRotationSliderChanged);
        if (rotYSlider != null) rotYSlider.onValueChanged.AddListener(OnRotationSliderChanged);
        if (rotZSlider != null) rotZSlider.onValueChanged.AddListener(OnRotationSliderChanged);
        if (fieldViewSlider != null) fieldViewSlider.onValueChanged.AddListener(OnFieldViewSliderChanged);
        UpdateCameraStatusText();
    }

    // 位置スライダーが変更されたときの処理
    private void OnPositionSliderChanged(float value)
    {
        if (posXSlider != null && posYSlider != null && posZSlider != null)
        {
            Vector3 newPosition = new Vector3(posXSlider.value, posYSlider.value, posZSlider.value);
            defOffset = newPosition;
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
            defRotation = new Vector3(xRotation, yRotation, zRotation);
            transform.rotation = Quaternion.Euler(defRotation);
            UpdateCameraStatusText();
        }
    }
    private void OnFieldViewSliderChanged(float value)
    {
        if(fieldViewSlider != null)
        {
            // 視野角を更新
            float fieldView = fieldViewSlider.value;

            defFieldView = fieldView;
            this.GetComponent<Camera>().fieldOfView = defFieldView;
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
                "PosZ: " + transform.position.z.ToString("F2") + "\n" +
                "View: " + defFieldView.ToString("F2");
        }
    }


    // ===== モノクロ =====
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
