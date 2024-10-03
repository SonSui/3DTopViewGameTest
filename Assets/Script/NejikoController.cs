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


    //巻き戻す変数
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


        //ライン
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


            //ラインの端
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




    //-------------------巻き戻す機能-------------------

    void RecordSnapshot()
    {
        //記録
        TimeSnapShot snapshot = new TimeSnapShot(transform, animator);
        snapshots.Add(snapshot);
    }
    void RewindTime()
    {
        //巻き戻し開始
        if (snapshots.Size > 0)
        {
            //一番新しい状態を獲得
            TimeSnapShot snapshot = snapshots.Get(snapshots.Size - 1);
            ApplySnapshot(snapshot);
            snapshots.RemoveLast(); 
        }
        else
        {
            //記録したデータもうない
            StopRewind();
        }
    }

    void ApplySnapshot(TimeSnapShot snapshot)
    {
        //状態を戻す

        // 位置と角度を戻す
        transform.position = snapshot.position;
        transform.rotation = snapshot.rotation;

        // アニメーションを戻す
        animator.Play(snapshot.animationStateName, 0, snapshot.animationNormalizedTime);
        animator.Update(0); // アニメーションを更新
    }

    void StartRewind()
    {
        isRewinding = true;
        animator.speed = 0; // アニメーションを停止
    }

    void StopRewind()
    {
        isRewinding = false;
        animator.speed = 1; //アニメーションを続き
    }
}
