
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Linq;

public class IdleVideoManager : MonoBehaviour
{
    [Header("動画設定")]
    public VideoPlayer videoPlayer; // VideoPlayer コンポーネント
    public Canvas normalUI; // 通常のUI（動画再生時に隠す）
    public GameObject videoPanel; // 動画再生用のパネル

    [Header("時間設定")]
    public float idleTimeToPlay = 10f; // 何秒間入力がなければ動画を再生するか
    public float waitTimeAfterVideo = 5f; // 動画終了後、何秒間入力がなければ再び動画を再生するか

    private float idleTimer = 0f; // 入力がない時間を計測
    private bool isVideoPlaying = false; // 動画再生中かどうか
    private bool videoFinished = false; // 動画が終了したか

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd; // 動画終了時の処理を登録
        videoPanel.SetActive(false); // 初期状態では動画パネルを非表示
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
            if (isVideoPlaying) return;
            idleTimer += Time.unscaledDeltaTime; // 入力がない間は時間を加算
        }

        // 一定時間入力がなければ動画を再生
        if (idleTimer >= idleTimeToPlay)
        {
            PlayVideo();
        }
        Debug.Log("Video time "+ idleTimer);
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
       // normalUI.gameObject.SetActive(false); // UIを非表示
        videoPanel.SetActive(true); // 動画パネルを表示
        videoPlayer.Play(); // 動画を再生
    }

    private void StopVideo()
    {
        videoPlayer.Stop(); // 動画を停止
        isVideoPlaying = false;
        idleTimer = 0f; // 入力待機時間をリセット
        //normalUI.gameObject.SetActive(true); // UIを表示
        videoPanel.SetActive(false); // 動画パネルを非表示
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoFinished = true;
        StartCoroutine(WaitAndRestartVideo());
    }

    private IEnumerator WaitAndRestartVideo()
    {
        yield return new WaitForSeconds(waitTimeAfterVideo); // 一定時間待機

        if (!AnyInputDetected())
        {
            PlayVideo(); // 再び動画を再生
        }
    }
}
