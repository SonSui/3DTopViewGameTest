using UnityEngine;
using UnityEngine.UI; 

public class CameraFollow : MonoBehaviour
{
    public Transform target;    
    public Vector3 offset;      

    //入力
    public InputField posXInputField;
    public InputField posYInputField;
    public InputField posZInputField;

    
    public InputField rotXInputField;
    public InputField rotYInputField;
    public InputField rotZInputField;

    //モノクロ
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
                //キャラとカメラの偏差
                target = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player not found in the scene.");
            }
        }

        if (monoTone==null)
        {
            Debug.LogError("There is no Material of shader");
        }

        offset = transform.position - target.position;

        UpdateInputFields();


        //入力
        posXInputField.onEndEdit.AddListener(delegate { OnPositionInputChanged(); });
        posYInputField.onEndEdit.AddListener(delegate { OnPositionInputChanged(); });
        posZInputField.onEndEdit.AddListener(delegate { OnPositionInputChanged(); });

        rotXInputField.onEndEdit.AddListener(delegate { OnRotationInputChanged(); });
        rotYInputField.onEndEdit.AddListener(delegate { OnRotationInputChanged(); });
        rotZInputField.onEndEdit.AddListener(delegate { OnRotationInputChanged(); });
    }
    void Update()
    {
        currentAmount = Mathf.Lerp(currentAmount, targetAmount, Time.deltaTime * transitionSpeed);
    }

    void LateUpdate()
    {
        //キャラに追跡
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void UpdateInputFields()
    {
        //カメラの値を表示
        posXInputField.text = transform.position.x.ToString("F2");
        posYInputField.text = transform.position.y.ToString("F2");
        posZInputField.text = transform.position.z.ToString("F2");

        
        rotXInputField.text = transform.eulerAngles.x.ToString("F2");
        rotYInputField.text = transform.eulerAngles.y.ToString("F2");
        rotZInputField.text = transform.eulerAngles.z.ToString("F2");
    }

    void OnPositionInputChanged()
    {
        float posX, posY, posZ;

        //位置が変わった
        if (float.TryParse(posXInputField.text, out posX) &&
            float.TryParse(posYInputField.text, out posY) &&
            float.TryParse(posZInputField.text, out posZ))
        {
            
            Vector3 newPosition = new Vector3(posX, posY, posZ);
            offset = newPosition;
        }
        else
        {
            Debug.LogWarning("無効な位置数字");
        }
    }

    void OnRotationInputChanged()
    {
        float rotX, rotY, rotZ;

        //角度が変わった
        if (float.TryParse(rotXInputField.text, out rotX) &&
            float.TryParse(rotYInputField.text, out rotY) &&
            float.TryParse(rotZInputField.text, out rotZ))
        {
            
            transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
        }
        else
        {
            Debug.LogWarning("無効な角度数字");
        }
    }

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
}