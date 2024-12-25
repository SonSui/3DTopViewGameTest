using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    public TextMeshProUGUI damageText; // TextMeshPro コンポーネント
    public float displayDuration = 1.0f; // 表示時間
    public Vector3 floatUpOffset = new Vector3(0, 200, 0); // 浮き上がるオフセット
    private float timer;
    private RectTransform rectTransform;
    private Vector2 floatSpeed;
    public AnimationCurve sizeCurve;

    public void Initialize(int damage, Vector3 worldPosition, Color color)
    {
        damageText=GetComponent<TextMeshProUGUI>();
        damageText.text = damage.ToString(); // テキストを設定
        damageText.color = color; // 色を設定
        timer = displayDuration; // タイマーをリセット

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

        // 浮き上がり速度を設定
        floatSpeed = (Vector2)floatUpOffset / displayDuration;
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
            float newSize = Mathf.Lerp(50, 100, sizeCurve.Evaluate(normalizedTime)); // サイズをアニメーションカーブに基づいて計算
            damageText.fontSize = newSize;

            if (timer <= 0)
            {
                // 表示が終わったらオブジェクトを非アクティブ化
                gameObject.SetActive(false);
                UIManager.Instance.ReturnDamageTextObject(gameObject);
            }
        }
    }
}
