using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public AudioSource audioSource; // �������Đ�����AudioSource�R���|�[�l���g
    [Range(0.1f, 3f)] public float playbackSpeed = 1f; // �Đ����x�i0.1�{���`3�{���j
    public float startTime = 0f; // �Đ��J�n���ԁi�b�P�ʁj

    void Start()
    {
        // AudioSource���ݒ肳��Ă��Ȃ���΁A�����I�Ɏ擾
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // AudioSource�����݂���ꍇ�ɐݒ��K�p
        if (audioSource != null)
        {
            // �Đ����x��ݒ�
            audioSource.pitch = playbackSpeed;

            // �Đ��J�n�ʒu��ݒ�i�͈͊O�̃`�F�b�N���܂ށj
            if (startTime >= 0f && startTime < audioSource.clip.length)
            {
                audioSource.time = startTime;
            }
            else
            {
                Debug.LogWarning("�J�n���Ԃ������̒����𒴂��Ă��܂��I");
            }

            // �Đ����J�n
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource��������܂���I");
        }
    }

    void Update()
    {
        // �Đ����̑��x�����A���^�C���Œ����\
        if (audioSource != null)
        {
            audioSource.pitch = playbackSpeed;
        }
    }
}
