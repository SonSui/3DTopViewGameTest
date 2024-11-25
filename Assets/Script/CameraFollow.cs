using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public float smoothTime = 0.1f;

    // UI�v�f
    public GameObject uiCanvasPrefab; // Canvas��Prefab
    private GameObject cameraUI;
    private Text cameraStatusText;    // �J������Ԃ�\������Text
    private Slider posXSlider;
    private Slider posYSlider;
    private Slider posZSlider;
    private Slider rotXSlider;
    private Slider rotYSlider;
    private Slider rotZSlider;

    // ���m�N��
    public Material monoTone;
    private float targetAmount = 0f;
    private float currentAmount = 0f;
    private float transitionSpeed = 1f;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform; // �L�����ƃJ�����̕΍���ݒ�
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

        // UI�̏����ݒ�
        SetupUIComponents();
        UpdateSliders(); // ������Ԃ̃X���C�_�[���X�V
    }

    void Update()
    {

        //���m�N���X�V
        currentAmount = Mathf.Lerp(currentAmount, targetAmount, Time.deltaTime * transitionSpeed);
    }

    void LateUpdate()
    {
        // �L�����ɒǐ�
        if (target != null)
        {
            
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime / smoothTime);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchCameraUI();
        }
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

    // �X���C�_�[�͈̔͂�0����360�Ɋg��
    private void SetupUIComponents()
    {
        if (uiCanvasPrefab != null)
        {
            // Canvas��Prefab���C���X�^���X��
            cameraUI = Instantiate(uiCanvasPrefab);
            

            // �J������Ԃ�\������Text���擾
            cameraStatusText = cameraUI.GetComponentInChildren<Text>();
            if (cameraStatusText != null)
            {
                // Text�I�u�W�F�N�g�̎q�I�u�W�F�N�g����X���C�_�[���擾
                posXSlider = cameraStatusText.transform.Find("SliderPosX").GetComponent<Slider>();
                posYSlider = cameraStatusText.transform.Find("SliderPosY").GetComponent<Slider>();
                posZSlider = cameraStatusText.transform.Find("SliderPosZ").GetComponent<Slider>();

                rotXSlider = cameraStatusText.transform.Find("SliderRotX").GetComponent<Slider>();
                rotYSlider = cameraStatusText.transform.Find("SliderRotY").GetComponent<Slider>();
                rotZSlider = cameraStatusText.transform.Find("SliderRotZ").GetComponent<Slider>();

                // �X���C�_�[�̐ݒ�
                ConfigureSlider(posXSlider, -30f, 30f, OnPositionSliderChanged);
                ConfigureSlider(posYSlider, -30f, 30f, OnPositionSliderChanged);
                ConfigureSlider(posZSlider, -30f, 30f, OnPositionSliderChanged);

                // ��]�X���C�_�[�͈̔͂�0����360�ɐݒ�
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
    // �X���C�_�[�͈̔͂ƃ��X�i�[��ݒ肷��w���p�[���\�b�h
    private void ConfigureSlider(Slider slider, float minValue, float maxValue, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider != null)
        {
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.onValueChanged.AddListener(callback);
        }
    }

    // �J�����̏�ԂɊ�Â��ăX���C�_�[���X�V
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

    // �ʒu�X���C�_�[���ύX���ꂽ�Ƃ��̏���
    private void OnPositionSliderChanged(float value)
    {
        if (posXSlider != null && posYSlider != null && posZSlider != null)
        {
            Vector3 newPosition = new Vector3(posXSlider.value, posYSlider.value, posZSlider.value);
            offset = newPosition;
            UpdateCameraStatusText();
        }
    }

    // �J�����̉�]�X���C�_�[���ύX���ꂽ�Ƃ��̏���
    private void OnRotationSliderChanged(float value)
    {
        if (rotXSlider != null && rotYSlider != null && rotZSlider != null)
        {
            float xRotation = rotXSlider.value;
            float yRotation = rotYSlider.value;
            float zRotation = rotZSlider.value;

            // Quaternion�ŉ�]��ݒ�
            transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);

            UpdateCameraStatusText();
        }
    }

    // �J�����̏�Ԃ�Text�ɍX�V
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


    //���m�N��
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
