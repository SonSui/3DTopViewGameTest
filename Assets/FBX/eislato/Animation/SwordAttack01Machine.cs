using UnityEngine;

public class SwordAttack01Machine : StateMachineBehaviour
{
    public float fixEnterAngle = 135f; // �C���p�xEnter
    public float enterDuration = 0.05f;
    public float fixExitAngle = 2f; // �C���p�xExit
    public float exitDuration = 0.2f; // �C���ɂ����鎞��
    

    private Transform characterTransform; // �L�����N�^�[��Transform
    private bool isRotating = false; // �R���[�`���̏d����h���t���O


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �L�����N�^�[��Transform���擾
        characterTransform = animator.transform;

        if (!isRotating)
        {
            // �R���[�`�����J�n���ĕ����ȉ�]������
            animator.GetComponent<MonoBehaviour>().StartCoroutine(RotateSmoothly(characterTransform, fixEnterAngle, enterDuration));
            isRotating = true; // �R���[�`���̏d����h��
        }
    }


    // ��Ԃ̍X�V���ɌĂяo�����
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ���݂̃g�����W�V������ "SwordAttack01_to_Idle" �ł��邩���m�F
        AnimatorTransitionInfo transitionInfo = animator.GetAnimatorTransitionInfo(layerIndex);
        if (transitionInfo.IsUserName("SwordAttack01_to_Idle") && !isRotating)
        {
            // �L�����N�^�[��Transform���擾
            characterTransform = animator.transform;

            if(!isRotating)
            {
                // �R���[�`�����J�n���ĕ����ȉ�]������
                animator.GetComponent<MonoBehaviour>().StartCoroutine(RotateSmoothly(characterTransform, fixExitAngle, exitDuration));
                isRotating = true; // �R���[�`���̏d����h��
            }
            
        }
    }

    // ��Ԃ��I�������Ƃ��ɌĂяo�����
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �t���O�����Z�b�g���Ď���̃g�����W�V�����ōēx�����\�ɂ���
        isRotating = false;
    }

    // �R���[�`���ŕ����ȉ�]����������
    private System.Collections.IEnumerator RotateSmoothly(Transform target, float angle, float duration)
    {
        Quaternion initialRotation = target.rotation; // ���݂̉�]
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, angle, 0f); // �ڕW��]
        float elapsedTime = 0f; // �o�ߎ���

        // �w�肳�ꂽ���ԓ��ŉ�]��␳
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // ���݂̉�]�������l����ڕW�l�֕��
            target.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);

            yield return null; // ���̃t���[���܂ő҂�
        }

        // �ŏI�I�ȉ�]��ڕW�l�ɌŒ�
        target.rotation = targetRotation;
        isRotating = false;
    }
}