using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UITooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("タグの設定")]
    AbilityTagDefinition abilityTag;
    public Image iconImage;          // ツールチップ内のアイコン
    public TextMeshProUGUI tagLevelText;
    public Sprite icon;              // 表示するアイコン
    public int tagLevel = 0;


    [Header("ツールチップの設定")]
    public GameObject tooltip;       // ツールチップオブジェクト
    string description;       // 表示する説明文
    private TextMeshProUGUI tooltipText;
    private Image tooltipBackground; 

    

    void Start()
    {
        // 初期状態でツールチップを非表示にする
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
    

    // マウスがボタンの上に乗ったとき
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip(); // ツールチップを表示
    }

    // マウスがボタンから外れたとき
    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip(); // ツールチップを非表示
    }

    // 手動でボタンが選択されたとき（コントローラーやキーボード）
    public void OnSelect(BaseEventData eventData)
    {
        ShowTooltip(); // ツールチップを表示
    }

    // ボタンの選択が解除されたとき
    public void OnDeselect(BaseEventData eventData)
    {
        HideTooltip(); // ツールチップを非表示
    }

    // ツールチップを表示する関数
    private void ShowTooltip()
    {
        // ツールチップを表示
        tooltip.SetActive(true);

        // テキストを設定
        string[] lines = description.Split('\n'); // 説明文を行ごとに分割
        int maxLineLength = 0;

        // 各行の長さを計算し、最大値を取得
        foreach (string line in lines)
        {
            int lineLength = line.Length;
            if (lineLength > maxLineLength)
            {
                maxLineLength = lineLength;
            }
        }

        // 説明文の色を設定
        tooltipText.text = ""; // 初期化
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0)
            {
                // 最初の行は緑色
                tooltipText.text += $"<color=#00FF00>{lines[i]}</color>\n";
            }
            else if (i <= tagLevel)
            {
                // tagLevel 以下の行は白色
                tooltipText.text += $"<color=#FFFFFF>{lines[i]}</color>\n";
            }
            else
            {
                // その他の行は灰色
                tooltipText.text += $"<color=#808080>{lines[i]}</color>\n";
            }
        }

        // テキストサイズと背景サイズを計算
        float textWidth = maxLineLength * 24 + 1; // 1文字24ピクセルの幅
        float textHeight = lines.Length * 36 + 1; // 1行36ピクセルの高さ
        tooltipText.rectTransform.sizeDelta = new Vector2(textWidth, textHeight);

        float backgroundWidth = textWidth + 20; // 背景はテキストより20ピクセル大きい
        float backgroundHeight = textHeight + 20;

        // ツールチップ全体の位置を設定
        RectTransform rectTransform = tooltip.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0 + backgroundWidth / 2, -400 - backgroundHeight / 2);

        // 背景のサイズを変更
        rectTransform.sizeDelta = new Vector2(backgroundWidth, backgroundHeight);
    }


    // ツールチップを非表示にする関数
    private void HideTooltip()
    {
        tooltip.SetActive(false);       // ツールチップを非表示
    }
}
