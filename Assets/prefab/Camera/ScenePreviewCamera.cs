using UnityEngine;

public class ScenePreviewCamera : MonoBehaviour
{
    public float moveSpeed = 10f;       // カメラの移動速度
    public float mouseSensitivity = 100f; // マウスの感度
    public float scrollSensitivity = 2f;  // ホイールズームの感度
    public float minFov = 15f;            // 最小視野角
    public float maxFov = 90f;            // 最大視野角

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // マウスカーソルを非表示にしてロック
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement(); // 移動処理
        HandleMouseRotation(); // マウス回転処理
        HandleZoom(); // ズーム処理
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/Dキーによる水平移動
        float vertical = Input.GetAxis("Vertical");     // W/Sキーによる前後移動

        Vector3 forwardMovement = transform.forward * vertical;
        Vector3 rightMovement = transform.right * horizontal;

        transform.position += (forwardMovement + rightMovement) * moveSpeed * Time.deltaTime;
    }

    void HandleMouseRotation()
    {
        if (Input.GetMouseButton(1)) // 右クリックを押して視点を回転
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            rotationX -= mouseY;
            rotationY += mouseX;

            rotationX = Mathf.Clamp(rotationX, -80f, 80f); // 上下視点の制限

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // マウスホイールの入力
        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.fieldOfView -= scroll * scrollSensitivity;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minFov, maxFov); // 視野角の制限
        }
    }
}
