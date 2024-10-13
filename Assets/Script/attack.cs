using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour
{
    public PlayerController player;
    private Animator animator;

    private int bufferSize = 180;
    private int recordInterval = 2;

    private CircularBuffer<TimeSnapShot> snapshots;
    private int frameCounter = 0;
    private bool isRewinding = false;

    private float moveSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        player = FindFirstObjectByType(typeof(PlayerController)) as PlayerController;
        animator = GetComponent<Animator>();
        snapshots = new CircularBuffer<TimeSnapShot>(bufferSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRewind();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            StopRewind();
        }
        if (isRewinding)
        {
            RewindTime();
        }
        else
        {
            if (frameCounter % recordInterval == 0)
            {
                RecordSnapshot();
            }
            frameCounter++;
        }
    }
    void RecordSnapshot()
    {
        //�L�^
        TimeSnapShot snapshot = new TimeSnapShot(transform, animator);
        snapshots.Add(snapshot);
    }
    void RewindTime()
    {
        //�����߂��J�n
        if (snapshots.Size > 0)
        {
            //��ԐV������Ԃ��l��

            TimeSnapShot snapshot = snapshots.Get(snapshots.Size - 1);
            ApplySnapshot(snapshot);
            snapshots.RemoveLast();

        }
        else
        {
            //�L�^�����f�[�^�����Ȃ�
            StopRewind();
        }
    }

    void ApplySnapshot(TimeSnapShot snapshot)
    {
        //��Ԃ�߂�

        // �ʒu�Ɗp�x��߂�
        transform.position = snapshot.position;
        transform.rotation = snapshot.rotation;
        // �A�j���[�V������߂�
        animator.Play(snapshot.animationStateName, 0, snapshot.animationNormalizedTime);
        animator.Update(0); // �A�j���[�V�������X�V

    }

    void StartRewind()
    {
        isRewinding = true;
        animator.speed = 0; // �A�j���[�V�������~
        
    }

    void StopRewind()
    {
        isRewinding = false;
        animator.speed = 1; //�A�j���[�V�����𑱂�
        
    }
}
