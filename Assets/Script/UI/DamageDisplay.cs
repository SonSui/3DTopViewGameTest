using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    public TextMeshProUGUI damageText; // TextMeshPro �R���|�[�l���g
    public float displayDuration = 1.0f; // �\������
    public Vector3 floatUpOffset = new Vector3(0, 200, 0); // �����オ��I�t�Z�b�g
    private float timer;
    private RectTransform rectTransform;
    private Vector2 floatSpeed;
    public AnimationCurve sizeCurve;

    public void Initialize(int damage, Vector3 worldPosition, Color color)
    {
        damageText=GetComponent<TextMeshProUGUI>();
        damageText.text = damage.ToString(); // �e�L�X�g��ݒ�
        damageText.color = color; // �F��ݒ�
        timer = displayDuration; // �^�C�}�[�����Z�b�g

        // �K�v�ȃR���|�[�l���g���擾
        Camera camera = GameManager.Instance?.GetCamera()?.GetComponent<Camera>() ?? Camera.main;
        Canvas canvas = UIManager.Instance?.canvas ?? FindObjectOfType<Canvas>();

        // 3D���[���h���W��UI�X�N���[�����W�ɕϊ�
        Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPosition,
            canvas.worldCamera,
            out Vector2 uiPosition
        );

        // UI�ʒu��ݒ�
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = uiPosition;

        // �����オ�葬�x��ݒ�
        floatSpeed = (Vector2)floatUpOffset / displayDuration;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            // �e�L�X�g����Ɉړ�
            rectTransform.anchoredPosition += floatSpeed * Time.deltaTime;

            // �A�j���[�V�����J�[�u�Ɋ�Â��T�C�Y�ύX
            float normalizedTime = 1 - (timer / displayDuration); // �i�s�x�i0�`1�j
            float newSize = Mathf.Lerp(50, 100, sizeCurve.Evaluate(normalizedTime)); // �T�C�Y���A�j���[�V�����J�[�u�Ɋ�Â��Čv�Z
            damageText.fontSize = newSize;

            if (timer <= 0)
            {
                // �\�����I�������I�u�W�F�N�g���A�N�e�B�u��
                gameObject.SetActive(false);
                UIManager.Instance.ReturnDamageTextObject(gameObject);
            }
        }
    }
}
