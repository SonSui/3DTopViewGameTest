using UnityEngine;

[System.Serializable]
public struct TimeSnapShot
{
    public Vector3 position;
    public Quaternion rotation;
    public string animationStateName;
    public float animationNormalizedTime;


    public TimeSnapShot(Transform transform, Animator animator)
    {
        // �ʒu�Ɗp�x���L�^
        position = transform.position;
        rotation = transform.rotation;

        // �A�j���[�V�������L�^
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animationStateName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        animationNormalizedTime = stateInfo.normalizedTime;





    }
}