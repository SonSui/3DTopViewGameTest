using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using System.Linq;
using UnityEngine.SceneManagement;

public class IdleVideoManager : MonoBehaviour
{
    [Header("動画設定")]
    public VideoPlayer videoPlayer; // VideoPlayer コンポーネント
    public GameObject videoPanel; // 動画再生用のパネル

    [Header("時間設定")]
    public float idleTimeToPlay = 10f; // 何秒間入力がなければ動画を再生するか
    public float waitTimeAfterVideo = 5f; // 動画終了後、何秒間入力がなければ再び動画を再生するか

    private float idleTimer = 0f; // 入力がない時間を計測
    private bool isVideoPlaying = false; // 動画再生中かどうか

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Title") // タイトルシーンに戻った時
        {
            ResetVideoState();
        }
        /*videoPlayer = GameObject.Find("Video")?.GetComponent<VideoPlayer>();
        videoPanel = GameObject.Find("VideoPanel");

        if (videoPlayer == null || videoPanel == null)
        {
            Debug.LogError("動画プレイヤーまたはパネルが見つかりません！");
            return;
        }

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPanel.SetActive(false);
        videoPlayer.isLooping = false;*/
    }

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd; // 動画終了時の処理を登録
        videoPanel.SetActive(false); // 初期状態では動画パネルを非表示
        videoPlayer.isLooping = false; // ループしないように設定
    }

    private void Update()
    {
        if (AnyInputDetected())
        {
            idleTimer = 0f; // 入力があったのでタイマーをリセット
            StopVideo();
        }
        else
        {
            if (!isVideoPlaying) idleTimer += Time.unscaledDeltaTime; // 入力がない間は時間を加算
        }

        // 一定時間入力がなければ動画を再生
        if (idleTimer >= idleTimeToPlay && !isVideoPlaying)
        {
            PlayVideo();
        }
    }

    private bool AnyInputDetected()
    {
        return Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed ||
           (Gamepad.current != null && Gamepad.current.allControls.Any(c => c.IsPressed())) ||
           (Touchscreen.current != null && Touchscreen.current.press.isPressed);
    }

    private void PlayVideo()
    {
        isVideoPlaying = true;
        videoPanel.SetActive(true); // 動画パネルを表示
        videoPlayer.Play(); // 動画を再生
    }

    private void StopVideo()
    {
        if (!isVideoPlaying) return;

        videoPlayer.Stop(); // 動画を停止
        isVideoPlaying = false;
        idleTimer = 0f; // 入力待機時間をリセット
        videoPanel.SetActive(false); // 動画パネルを非表示
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (isVideoPlaying)
        {
            StopVideo(); // 動画を明示的に停止
            StartCoroutine(WaitAndRestartVideo()); // 一定時間後に再生を試みる
        }
    }

    private IEnumerator WaitAndRestartVideo()
    {
        yield return new WaitForSeconds(waitTimeAfterVideo); // 一定時間待機

        if (!AnyInputDetected() && !isVideoPlaying)
        {
            PlayVideo(); // 再び動画を再生
        }
    }

    private void ResetVideoState()
    {
        idleTimer = 0f;
        isVideoPlaying = false;
        videoPanel.SetActive(false); // 動画パネルを非表示
    }
}
