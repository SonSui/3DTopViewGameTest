using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NejikoController : MonoBehaviour
{
    CharacterController controller;
    Animator animator;

    Vector3 moveDirection = Vector3.zero;
    Vector3 charaRotationOri = Vector3.zero;

    public float gravity = 20f;
    public float speedZ = 5f;
    public float speedJump = 8f;
    public float speedX = 5f;

    private LineRenderer lineRenderer;
    private int pointCount = 0;

    public float pointDistance = 0.1f;
    private Vector3 lastPosition;


    //�����߂��ϐ�
    private int bufferSize = 10000;      
    private int recordInterval = 1;     

    private CircularBuffer<TimeSnapShot> snapshots;
    private int frameCounter = 0;
    private bool isRewinding = false;

    void Start()
    {
        snapshots = new CircularBuffer<TimeSnapShot>(bufferSize);
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        charaRotationOri = transform.eulerAngles;


        //���C��
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lastPosition = transform.position;
        Debug.Log("RecordInterval: "+recordInterval.ToString());
    }

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

            if (controller.isGrounded)
            {
                if (Input.GetAxis("Vertical") > 0.0f)
                {
                    moveDirection.z = Input.GetAxis("Vertical") * speedZ;
                    transform.eulerAngles = new Vector3(charaRotationOri.x, charaRotationOri.y, charaRotationOri.z);

                }
                else if (Input.GetAxis("Vertical") < 0.0f)
                {
                    moveDirection.z = Input.GetAxis("Vertical") * -speedZ;
                    float newAngle = charaRotationOri.y + 180f;
                    transform.eulerAngles = new Vector3(charaRotationOri.x, newAngle, charaRotationOri.z);
                }
                if (Input.GetAxis("Horizontal") < 0.0f)
                {
                    moveDirection.z = Input.GetAxis("Horizontal") * -speedZ;
                    float newAngle = charaRotationOri.y - 90f;
                    transform.eulerAngles = new Vector3(charaRotationOri.x, newAngle, charaRotationOri.z);
                }
                else if (Input.GetAxis("Horizontal") > 0.0f)
                {
                    moveDirection.z = Input.GetAxis("Horizontal") * speedZ;
                    float newAngle = charaRotationOri.y + 90f;
                    transform.eulerAngles = new Vector3(charaRotationOri.x, newAngle, charaRotationOri.z);
                }
                if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
                {
                    moveDirection.z = 0.0f;
                }
                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = speedJump;
                    animator.SetTrigger("jump");
                }
            }
            moveDirection.y -= gravity * Time.deltaTime;

            Vector3 globalDirection = transform.TransformDirection(moveDirection);
            controller.Move(globalDirection * Time.deltaTime);

            if (controller.isGrounded) { moveDirection.y = 0; }

            animator.SetBool("run", moveDirection.z > 0.0f || moveDirection.x > 0.0f);


            //���C���̒[
            if (Vector3.Distance(transform.position, lastPosition) >= pointDistance)
            {
                AddPoint(transform.position);
                lastPosition = transform.position;
            }
        }

    }
    void AddPoint(Vector3 newPosition)
    {
        lineRenderer.positionCount = pointCount + 1;
        lineRenderer.SetPosition(pointCount, newPosition);
        pointCount++;
    }
    void kuro()
    {
        Debug.Log("https://nn-hokuson.hatenablog.com/entry/2016/11/17/204831");
    }




    //-------------------�����߂��@�\-------------------

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
