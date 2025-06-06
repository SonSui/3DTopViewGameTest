using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack03Machine : StateMachineBehaviour
{
    public float fixEnterAngle = 60f; // 修正角度Enter
    public float enterDuration = 0.25f;
    public float fixExitAngle = 10f; // 修正角度Exit
    public float exitDuration = 0.25f; // 修正にかける時間


    private Transform characterTransform; // キャラクターのTransform
    private bool isRotating = false; // コルーチンの重複を防ぐフラグ


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // キャラクターのTransformを取得
        characterTransform = animator.transform;

        if (!isRotating)
        {
            // コルーチンを開始して平滑な回転を実現
            animator.GetComponent<MonoBehaviour>().StartCoroutine(RotateSmoothly(characterTransform, fixEnterAngle, enterDuration));
            isRotating = true; // コルーチンの重複を防ぐ
        }
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 現在のトランジションが "SwordAttack01_to_Idle" であるかを確認
        AnimatorTransitionInfo transitionInfo = animator.GetAnimatorTransitionInfo(layerIndex);
        if (transitionInfo.IsUserName("SwordAttack03_to_Idle") && !isRotating)
        {
            // キャラクターのTransformを取得
            characterTransform = animator.transform;

            if (!isRotating)
            {
                // コルーチンを開始して平滑な回転を実現
                animator.GetComponent<MonoBehaviour>().StartCoroutine(RotateSmoothly(characterTransform, fixExitAngle, exitDuration));
                isRotating = true; // コルーチンの重複を防ぐ
            }

        }
    }


    // コルーチンで平滑な回転を実現する
    private System.Collections.IEnumerator RotateSmoothly(Transform target, float angle, float duration)
    {
        Quaternion initialRotation = target.rotation; // 現在の回転
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, angle, 0f); // 目標回転
        float elapsedTime = 0f; // 経過時間

        // 指定された時間内で回転を補正
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // 現在の回転を初期値から目標値へ補間
            target.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);

            yield return null; // 次のフレームまで待つ
        }

        // 最終的な回転を目標値に固定
        target.rotation = targetRotation;
        isRotating = false;
    }
}
