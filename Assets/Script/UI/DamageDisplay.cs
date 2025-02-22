using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    public TextMeshProUGUI damageText; // TextMeshPro コンポーネント
    public float displayDuration = 1.0f; // 表示時間
    public float floatUpOffset = 150f; // 浮き上がるオフセット
    private float timer;
    private RectTransform rectTransform;
    private Vector2 floatSpeed;
    public AnimationCurve sizeCurve;

    public float darkColorRate = 0.6f; // 色を暗くする割合
    public float transpotColor = 0.8f; // 透明度

    private Material textMaterial; // テキスト用マテリアル
    public Color initialGlowColor; // 初期発光色

    public void Initialize(int damage, Vector3 worldPosition, Color color)
    {
        damageText = GetComponent<TextMeshProUGUI>();
        damageText.text = damage.ToString(); // テキストを設定
        timer = displayDuration; // タイマーをリセット

        // 色を自動調整（少し暗くし、透明度を設定）
        color = new Color(color.r * darkColorRate, color.g * darkColorRate, color.b * darkColorRate, transpotColor);
        damageText.color = color; // 色を設定

        /*// 発光色を初期化
        initialGlowColor = color;*/

        // 必要なコンポーネントを取得
        Camera camera = GameManager.Instance?.GetCamera()?.GetComponent<Camera>() ?? Camera.main;
        Canvas canvas = UIManager.Instance?.canvas ?? FindObjectOfType<Canvas>();

        // 3Dワールド座標をUIスクリーン座標に変換
        Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPosition,
            canvas.worldCamera,
            out Vector2 uiPosition
        );

        // UI位置を設定
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = uiPosition;

        // ランダム方向を計算
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // 移動速度を設定
        floatSpeed = randomDirection * floatUpOffset / displayDuration;
        //floatSpeed = Vector2.up * floatUpOffset / displayDuration;

        // マテリアルを取得
        textMaterial = damageText.fontMaterial;
        textMaterial.SetColor(ShaderUtilities.ID_GlowColor, initialGlowColor); // 発光色を設定
        textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 2f); // 初期発光強度
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            // テキストを上に移動
            rectTransform.anchoredPosition += floatSpeed * Time.deltaTime;

            // アニメーションカーブに基づくサイズ変更
            float normalizedTime = 1 - (timer / displayDuration); // 進行度（0～1）
            float newSize = Mathf.Lerp(10, 100, sizeCurve.Evaluate(normalizedTime)); // サイズをアニメーションカーブに基づいて計算
            damageText.fontSize = newSize;

            // 発光強度を時間経過に伴って減少
            float glowIntensity = Mathf.Lerp(2f, 0f, normalizedTime); // 発光強度を徐々に減少
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
        // 表示が終わったらオブジェクトを非アクティブ化
        gameObject.SetActive(false);
        UIManager.Instance.ReturnDamageTextObject(gameObject);
    }
}
