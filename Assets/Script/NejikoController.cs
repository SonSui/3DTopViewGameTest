using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    Transform cameraTransform;


    //�����߂��ϐ�
    private int bufferSize = 100000;      
    private int recordInterval = 1;     

    private CircularBuffer<TimeSnapShot> snapshots;
    private int frameCounter = 0;
    private bool isRewinding = false;

    public Text currentBuffer;

    void Start()
    {
        Application.targetFrameRate = 60;

        snapshots = new CircularBuffer<TimeSnapShot>(bufferSize);
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        charaRotationOri = transform.eulerAngles;

        cameraTransform = Camera.main.transform;

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

            //�J�����̊p�x���m�F
            Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

            // ���͊p�x
            float inputVertical = Input.GetAxis("Vertical");
            float inputHorizontal = Input.GetAxis("Horizontal");

            // ����
            float yDirection = moveDirection.y;

            // �ړ��������v�Z
            Vector3 horizontalMove = (forward * inputVertical + right * inputHorizontal).normalized;

            if (controller.isGrounded)
            {
                if (Input.GetButton("Jump"))
                {
                    yDirection = speedJump;  // 
                    animator.SetTrigger("jump");
                }
            }

            // �d�͌v�Z
            yDirection -= gravity * Time.deltaTime;

            // �ړ����x�ƕ���
            moveDirection = horizontalMove * speedZ;
            moveDirection.y = yDirection;

            // �ړ�
            controller.Move(moveDirection * Time.deltaTime);

            // �L�����N�^�[�̕�����ύX
            if (horizontalMove.magnitude > 0.1f)
            {
                transform.forward = horizontalMove;  // 
            }

            //�����j���O�A�j��
            animator.SetBool("run", inputVertical != 0 || inputHorizontal != 0);

            //�L�^�t���[���\��
            currentBuffer.text = snapshots.GetSize().ToString() + "/" + bufferSize.ToString();

            //���C���̒[
            if (Vector3.Distance(transform.position, lastPosition) >= pointDistance)
            {
                AddPoint(transform.position);
                lastPosition = transform.position;
            }
        }
        currentBuffer.text = snapshots.GetSize().ToString() + "/" + bufferSize.ToString();
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
            //StopRewind();
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
