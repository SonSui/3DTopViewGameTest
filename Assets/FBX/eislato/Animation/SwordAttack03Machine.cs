using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack03Machine : StateMachineBehaviour
{
    public float fixEnterAngle = 60f; // �C���p�xEnter
    public float enterDuration = 0.25f;
    public float fixExitAngle = 10f; // �C���p�xExit
    public float exitDuration = 0.25f; // �C���ɂ����鎞��


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
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ���݂̃g�����W�V������ "SwordAttack01_to_Idle" �ł��邩���m�F
        AnimatorTransitionInfo transitionInfo = animator.GetAnimatorTransitionInfo(layerIndex);
        if (transitionInfo.IsUserName("SwordAttack03_to_Idle") && !isRotating)
        {
            // �L�����N�^�[��Transform���擾
            characterTransform = animator.transform;

            if (!isRotating)
            {
                // �R���[�`�����J�n���ĕ����ȉ�]������
                animator.GetComponent<MonoBehaviour>().StartCoroutine(RotateSmoothly(characterTransform, fixExitAngle, exitDuration));
                isRotating = true; // �R���[�`���̏d����h��
            }

        }
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
