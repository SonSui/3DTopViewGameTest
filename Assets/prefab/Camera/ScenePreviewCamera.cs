using UnityEngine;

public class ScenePreviewCamera : MonoBehaviour
{
    public float moveSpeed = 10f;       // �J�����̈ړ����x
    public float mouseSensitivity = 100f; // �}�E�X�̊��x
    public float scrollSensitivity = 2f;  // �z�C�[���Y�[���̊��x
    public float minFov = 15f;            // �ŏ�����p
    public float maxFov = 90f;            // �ő压��p

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // �}�E�X�J�[�\�����\���ɂ��ă��b�N
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement(); // �ړ�����
        HandleMouseRotation(); // �}�E�X��]����
        HandleZoom(); // �Y�[������
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D�L�[�ɂ�鐅���ړ�
        float vertical = Input.GetAxis("Vertical");     // W/S�L�[�ɂ��O��ړ�

        Vector3 forwardMovement = transform.forward * vertical;
        Vector3 rightMovement = transform.right * horizontal;

        transform.position += (forwardMovement + rightMovement) * moveSpeed * Time.deltaTime;
    }

    void HandleMouseRotation()
    {
        if (Input.GetMouseButton(1)) // �E�N���b�N�������Ď��_����]
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            rotationX -= mouseY;
            rotationY += mouseX;

            rotationX = Mathf.Clamp(rotationX, -80f, 80f); // �㉺���_�̐���

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // �}�E�X�z�C�[���̓���
        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.fieldOfView -= scroll * scrollSensitivity;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minFov, maxFov); // ����p�̐���
        }
    }
}
