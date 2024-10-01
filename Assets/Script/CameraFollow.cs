using UnityEngine;
using UnityEngine.UI; 

public class CameraFollow : MonoBehaviour
{
    public Transform target;    
    public Vector3 offset;      

    
    public InputField posXInputField;
    public InputField posYInputField;
    public InputField posZInputField;

    
    public InputField rotXInputField;
    public InputField rotYInputField;
    public InputField rotZInputField;

    void Start()
    {
        offset = transform.position - target.position;

        
        UpdateInputFields();

        
        posXInputField.onEndEdit.AddListener(delegate { OnPositionInputChanged(); });
        posYInputField.onEndEdit.AddListener(delegate { OnPositionInputChanged(); });
        posZInputField.onEndEdit.AddListener(delegate { OnPositionInputChanged(); });

        rotXInputField.onEndEdit.AddListener(delegate { OnRotationInputChanged(); });
        rotYInputField.onEndEdit.AddListener(delegate { OnRotationInputChanged(); });
        rotZInputField.onEndEdit.AddListener(delegate { OnRotationInputChanged(); });
    }

    void LateUpdate()
    {
        
        transform.position = target.position + offset;
    }

    void UpdateInputFields()
    {
        
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

        
        if (float.TryParse(posXInputField.text, out posX) &&
            float.TryParse(posYInputField.text, out posY) &&
            float.TryParse(posZInputField.text, out posZ))
        {
            
            Vector3 newPosition = new Vector3(posX, posY, posZ);
            offset = newPosition;
        }
        else
        {
            Debug.LogWarning("ñ≥å¯Ç»êîéö");
        }
    }

    void OnRotationInputChanged()
    {
        float rotX, rotY, rotZ;

        
        if (float.TryParse(rotXInputField.text, out rotX) &&
            float.TryParse(rotYInputField.text, out rotY) &&
            float.TryParse(rotZInputField.text, out rotZ))
        {
            
            transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
        }
        else
        {
            Debug.LogWarning("ñ≥å¯Ç»êîéö");
        }
    }
}