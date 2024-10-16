using UnityEngine;

[System.Serializable]
public struct TimeSnapShot
{
    public Vector3 position;
    public Quaternion rotation;
    public string animationStateName;
    public float animationNormalizedTime;
    public int frame;


    public TimeSnapShot(Transform transform, Animator animator,int frame_)
    {
        // 位置と角度を記録
        position = transform.position;
        rotation = transform.rotation;

        // アニメーションを記録
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animationStateName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        animationNormalizedTime = stateInfo.normalizedTime;

        //プッシュの時のフレイム記数
        frame = frame_;

    }
}