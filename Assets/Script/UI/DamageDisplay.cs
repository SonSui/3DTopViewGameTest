using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    public TextMeshProUGUI damageText; // TextMeshPro �R���|�[�l���g
    public float displayDuration = 1.0f; // �\������
    public float floatUpOffset = 150f; // �����オ��I�t�Z�b�g
    private float timer;
    private RectTransform rectTransform;
    private Vector2 floatSpeed;
    public AnimationCurve sizeCurve;

    public float darkColorRate = 0.6f; // �F���Â����銄��
    public float transpotColor = 0.8f; // �����x

    private Material textMaterial; // �e�L�X�g�p�}�e���A��
    public Color initialGlowColor; // ���������F

    public void Initialize(int damage, Vector3 worldPosition, Color color)
    {
        damageText = GetComponent<TextMeshProUGUI>();
        damageText.text = damage.ToString(); // �e�L�X�g��ݒ�
        timer = displayDuration; // �^�C�}�[�����Z�b�g

        // �F�����������i�����Â����A�����x��ݒ�j
        color = new Color(color.r * darkColorRate, color.g * darkColorRate, color.b * darkColorRate, transpotColor);
        damageText.color = color; // �F��ݒ�

        /*// �����F��������
        initialGlowColor = color;*/

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

        // �����_���������v�Z
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // �ړ����x��ݒ�
        floatSpeed = randomDirection * floatUpOffset / displayDuration;
        //floatSpeed = Vector2.up * floatUpOffset / displayDuration;

        // �}�e���A�����擾
        textMaterial = damageText.fontMaterial;
        textMaterial.SetColor(ShaderUtilities.ID_GlowColor, initialGlowColor); // �����F��ݒ�
        textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 2f); // �����������x
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
            float newSize = Mathf.Lerp(10, 100, sizeCurve.Evaluate(normalizedTime)); // �T�C�Y���A�j���[�V�����J�[�u�Ɋ�Â��Čv�Z
            damageText.fontSize = newSize;

            // �������x�����Ԍo�߂ɔ����Č���
            float glowIntensity = Mathf.Lerp(2f, 0f, normalizedTime); // �������x�����X�Ɍ���
            textMaterial.SetColor(ShaderUtilities.ID_GlowColor, initialGlowColor * glowIntensity);
            textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, glowIntensity);

            if (timer <= 0)
            {
                ResetDmgText();
            }
        }
    }
    public void ResetDmgText()
    {
        // �\�����I�������I�u�W�F�N�g���A�N�e�B�u��
        gameObject.SetActive(false);
        UIManager.Instance.ReturnDamageTextObject(gameObject);
    }
}
