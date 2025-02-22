using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public AudioSource audioSource; // 音声を再生するAudioSourceコンポーネント
    [Range(0.1f, 3f)] public float playbackSpeed = 1f; // 再生速度（0.1倍速～3倍速）
    public float startTime = 0f; // 再生開始時間（秒単位）

    void Start()
    {
        // AudioSourceが設定されていなければ、自動的に取得
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // AudioSourceが存在する場合に設定を適用
        if (audioSource != null)
        {
            // 再生速度を設定
            audioSource.pitch = playbackSpeed;

            // 再生開始位置を設定（範囲外のチェックも含む）
            if (startTime >= 0f && startTime < audioSource.clip.length)
            {
                audioSource.time = startTime;
            }
            else
            {
                Debug.LogWarning("開始時間が音声の長さを超えています！");
            }

            // 再生を開始
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSourceが見つかりません！");
        }
    }

    void Update()
    {
        // 再生中の速度をリアルタイムで調整可能
        if (audioSource != null)
        {
            audioSource.pitch = playbackSpeed;
        }
    }
}
