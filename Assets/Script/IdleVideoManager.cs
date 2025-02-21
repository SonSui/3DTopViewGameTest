using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using System.Linq;
using UnityEngine.SceneManagement;

public class IdleVideoManager : MonoBehaviour
{
    [Header("����ݒ�")]
    public VideoPlayer videoPlayer; // VideoPlayer �R���|�[�l���g
    public GameObject videoPanel; // ����Đ��p�̃p�l��

    [Header("���Ԑݒ�")]
    public float idleTimeToPlay = 10f; // ���b�ԓ��͂��Ȃ���Γ�����Đ����邩
    public float waitTimeAfterVideo = 5f; // ����I����A���b�ԓ��͂��Ȃ���΍Ăѓ�����Đ����邩

    private float idleTimer = 0f; // ���͂��Ȃ����Ԃ��v��
    private bool isVideoPlaying = false; // ����Đ������ǂ���

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
        if (scene.name == "Title") // �^�C�g���V�[���ɖ߂�����
        {
            ResetVideoState();
        }
        /*videoPlayer = GameObject.Find("Video")?.GetComponent<VideoPlayer>();
        videoPanel = GameObject.Find("VideoPanel");

        if (videoPlayer == null || videoPanel == null)
        {
            Debug.LogError("����v���C���[�܂��̓p�l����������܂���I");
            return;
        }

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPanel.SetActive(false);
        videoPlayer.isLooping = false;*/
    }

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd; // ����I�����̏�����o�^
        videoPanel.SetActive(false); // ������Ԃł͓���p�l�����\��
        videoPlayer.isLooping = false; // ���[�v���Ȃ��悤�ɐݒ�
    }

    private void Update()
    {
        if (AnyInputDetected())
        {
            idleTimer = 0f; // ���͂��������̂Ń^�C�}�[�����Z�b�g
            StopVideo();
        }
        else
        {
            if (!isVideoPlaying) idleTimer += Time.unscaledDeltaTime; // ���͂��Ȃ��Ԃ͎��Ԃ����Z
        }

        // ��莞�ԓ��͂��Ȃ���Γ�����Đ�
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
        videoPanel.SetActive(true); // ����p�l����\��
        videoPlayer.Play(); // ������Đ�
    }

    private void StopVideo()
    {
        if (!isVideoPlaying) return;

        videoPlayer.Stop(); // ������~
        isVideoPlaying = false;
        idleTimer = 0f; // ���͑ҋ@���Ԃ����Z�b�g
        videoPanel.SetActive(false); // ����p�l�����\��
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (isVideoPlaying)
        {
            StopVideo(); // ����𖾎��I�ɒ�~
            StartCoroutine(WaitAndRestartVideo()); // ��莞�Ԍ�ɍĐ������݂�
        }
    }

    private IEnumerator WaitAndRestartVideo()
    {
        yield return new WaitForSeconds(waitTimeAfterVideo); // ��莞�ԑҋ@

        if (!AnyInputDetected() && !isVideoPlaying)
        {
            PlayVideo(); // �Ăѓ�����Đ�
        }
    }

    private void ResetVideoState()
    {
        idleTimer = 0f;
        isVideoPlaying = false;
        videoPanel.SetActive(false); // ����p�l�����\��
    }
}
