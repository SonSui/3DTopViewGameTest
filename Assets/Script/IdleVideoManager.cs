
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Linq;

public class IdleVideoManager : MonoBehaviour
{
    [Header("����ݒ�")]
    public VideoPlayer videoPlayer; // VideoPlayer �R���|�[�l���g
    public Canvas normalUI; // �ʏ��UI�i����Đ����ɉB���j
    public GameObject videoPanel; // ����Đ��p�̃p�l��

    [Header("���Ԑݒ�")]
    public float idleTimeToPlay = 10f; // ���b�ԓ��͂��Ȃ���Γ�����Đ����邩
    public float waitTimeAfterVideo = 5f; // ����I����A���b�ԓ��͂��Ȃ���΍Ăѓ�����Đ����邩

    private float idleTimer = 0f; // ���͂��Ȃ����Ԃ��v��
    private bool isVideoPlaying = false; // ����Đ������ǂ���
    private bool videoFinished = false; // ���悪�I��������

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd; // ����I�����̏�����o�^
        videoPanel.SetActive(false); // ������Ԃł͓���p�l�����\��
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
            if (isVideoPlaying) return;
            idleTimer += Time.unscaledDeltaTime; // ���͂��Ȃ��Ԃ͎��Ԃ����Z
        }

        // ��莞�ԓ��͂��Ȃ���Γ�����Đ�
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
       // normalUI.gameObject.SetActive(false); // UI���\��
        videoPanel.SetActive(true); // ����p�l����\��
        videoPlayer.Play(); // ������Đ�
    }

    private void StopVideo()
    {
        videoPlayer.Stop(); // ������~
        isVideoPlaying = false;
        idleTimer = 0f; // ���͑ҋ@���Ԃ����Z�b�g
        //normalUI.gameObject.SetActive(true); // UI��\��
        videoPanel.SetActive(false); // ����p�l�����\��
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoFinished = true;
        StartCoroutine(WaitAndRestartVideo());
    }

    private IEnumerator WaitAndRestartVideo()
    {
        yield return new WaitForSeconds(waitTimeAfterVideo); // ��莞�ԑҋ@

        if (!AnyInputDetected())
        {
            PlayVideo(); // �Ăѓ�����Đ�
        }
    }
}
