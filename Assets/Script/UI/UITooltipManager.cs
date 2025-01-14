using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UITooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("�^�O�̐ݒ�")]
    AbilityTagDefinition abilityTag;
    public Image iconImage;          // �c�[���`�b�v���̃A�C�R��
    public TextMeshProUGUI tagLevelText;
    public Sprite icon;              // �\������A�C�R��
    public int tagLevel = 0;


    [Header("�c�[���`�b�v�̐ݒ�")]
    public GameObject tooltip;       // �c�[���`�b�v�I�u�W�F�N�g
    string description;       // �\�����������
    private TextMeshProUGUI tooltipText;
    private Image tooltipBackground; 

    

    void Start()
    {
        // ������ԂŃc�[���`�b�v���\���ɂ���
        tooltip.SetActive(false);
    }

    public void SetTag(AbilityTagDefinition tag,int level, GameObject tooltip_)
    {
        tooltip = tooltip_;
        tooltipBackground = tooltip_.GetComponent<Image>();
        tooltipText = tooltip_.GetComponentInChildren<TextMeshProUGUI>();
        abilityTag = tag;
        icon = tag.icon;
        description = tag.description;
        Image img = GetComponent<Image>();
        img.sprite = icon;
        tagLevel = level;
        tagLevelText.text = level.ToString();
    }
    

    // �}�E�X���{�^���̏�ɏ�����Ƃ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip(); // �c�[���`�b�v��\��
    }

    // �}�E�X���{�^������O�ꂽ�Ƃ�
    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip(); // �c�[���`�b�v���\��
    }

    // �蓮�Ń{�^�����I�����ꂽ�Ƃ��i�R���g���[���[��L�[�{�[�h�j
    public void OnSelect(BaseEventData eventData)
    {
        ShowTooltip(); // �c�[���`�b�v��\��
    }

    // �{�^���̑I�����������ꂽ�Ƃ�
    public void OnDeselect(BaseEventData eventData)
    {
        HideTooltip(); // �c�[���`�b�v���\��
    }

    // �c�[���`�b�v��\������֐�
    private void ShowTooltip()
    {
        // �c�[���`�b�v��\��
        tooltip.SetActive(true);

        // �e�L�X�g��ݒ�
        string[] lines = description.Split('\n'); // ���������s���Ƃɕ���
        int maxLineLength = 0;

        // �e�s�̒������v�Z���A�ő�l���擾
        foreach (string line in lines)
        {
            int lineLength = line.Length;
            if (lineLength > maxLineLength)
            {
                maxLineLength = lineLength;
            }
        }

        // �������̐F��ݒ�
        tooltipText.text = ""; // ������
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0)
            {
                // �ŏ��̍s�͗ΐF
                tooltipText.text += $"<color=#00FF00>{lines[i]}</color>\n";
            }
            else if (i <= tagLevel)
            {
                // tagLevel �ȉ��̍s�͔��F
                tooltipText.text += $"<color=#FFFFFF>{lines[i]}</color>\n";
            }
            else
            {
                // ���̑��̍s�͊D�F
                tooltipText.text += $"<color=#808080>{lines[i]}</color>\n";
            }
        }

        // �e�L�X�g�T�C�Y�Ɣw�i�T�C�Y���v�Z
        float textWidth = maxLineLength * 24 + 1; // 1����24�s�N�Z���̕�
        float textHeight = lines.Length * 36 + 1; // 1�s36�s�N�Z���̍���
        tooltipText.rectTransform.sizeDelta = new Vector2(textWidth, textHeight);

        float backgroundWidth = textWidth + 20; // �w�i�̓e�L�X�g���20�s�N�Z���傫��
        float backgroundHeight = textHeight + 20;

        // �c�[���`�b�v�S�̂̈ʒu��ݒ�
        RectTransform rectTransform = tooltip.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0 + backgroundWidth / 2, -400 - backgroundHeight / 2);

        // �w�i�̃T�C�Y��ύX
        rectTransform.sizeDelta = new Vector2(backgroundWidth, backgroundHeight);
    }


    // �c�[���`�b�v���\���ɂ���֐�
    private void HideTooltip()
    {
        tooltip.SetActive(false);       // �c�[���`�b�v���\��
    }
}
