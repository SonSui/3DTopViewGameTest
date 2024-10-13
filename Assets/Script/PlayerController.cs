using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    Animator animator;

    Vector3 moveDirection = Vector3.zero;
    Vector3 charaRotationOri = Vector3.zero;
     
    //物理
    public float gravity = 20f;
    public float speedJump = 8f;

    public float defSpeed = 5f;
    public int defLife = 3;
    public int defDmg = 1;
    public float defCrit = 0f;

    /*private LineRenderer lineRenderer;
    private int pointCount = 0;

    public float pointDistance = 0.1f;
    private Vector3 lastPosition;*/

    Transform cameraTransform;

    private struct playerState
    {
        public int life;
        public float speed;
        public int damage;
        public float crit;

        public bool isExplo;
        public playerState (int life, float speed, int damage,float crit)
        {
            this.life= life;
            this.speed= speed;
            this.damage= damage;
            this.crit= crit;
            this.isExplo= false;
        }
        public void UpdateState(int life, float speed, int damage, float crit)
        {
            this.life = life;
            this.speed = speed;
            this.damage = damage;
            this.crit = crit;
        }
        public void UpdateAblitiy(bool explo)
        {
            this.isExplo = explo;
        }
    };
    playerState state;

    //巻き戻す変数
    private int bufferSize = 180;
    private int recordInterval = 2;

    private CircularBuffer<TimeSnapShot> snapshots;
    private int frameCounter = 0;
    private bool isRewinding = false;

    public Text currentBuffer;
    public CameraFollow camera1;

    void Start()
    {
        Application.targetFrameRate = 60;

        snapshots = new CircularBuffer<TimeSnapShot>(bufferSize);

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        charaRotationOri = transform.eulerAngles;

        cameraTransform = Camera.main.transform;

        state = new playerState(defLife, defSpeed, defDmg, defCrit);



        if (camera1 == null)
        {
            Debug.LogError("There is no Camera");
        }

        //ライン
        /*lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lastPosition = transform.position;
        Debug.Log("RecordInterval: " + recordInterval.ToString());*/

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

            //カメラの角度を確認
            Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

            // 入力角度
            float inputVertical = Input.GetAxis("Vertical");
            float inputHorizontal = Input.GetAxis("Horizontal");

            // 高さ
            float yDirection = moveDirection.y;

            // 移動方向を計算
            Vector3 horizontalMove = (forward * inputVertical + right * inputHorizontal).normalized;

            if (controller.isGrounded)
            {
                if (Input.GetButton("Jump"))
                {
                    yDirection = speedJump;
                    animator.SetTrigger("jump");
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    animator.SetTrigger("attack");
                }
            }

            // 重力計算
            yDirection -= gravity * Time.deltaTime;

            // 移動速度と方向
            moveDirection = horizontalMove * this.state.speed;
            moveDirection.y = yDirection;

            // 移動
            string animeName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (animeName!= "WAIT04"&& animeName != "DAMAGED00")//移動できる状態を確認
            controller.Move(moveDirection * Time.deltaTime);

            // キャラクターの方向を変更
            if (horizontalMove.magnitude > 0.1f)
            {
                transform.forward = horizontalMove;  // 
            }

            //ランニングアニメ
            animator.SetBool("run", inputVertical != 0 || inputHorizontal != 0);

            //記録フレーム表示
            currentBuffer.text = snapshots.GetSize().ToString() + "/" + bufferSize.ToString();

            //ラインの端
/*
            if (Vector3.Distance(transform.position, lastPosition) >= pointDistance)
            {
                AddPoint(transform.position);
                lastPosition = transform.position;
            }*/
        }
        currentBuffer.text = snapshots.GetSize().ToString() + "/" + bufferSize.ToString();


        
    }
    /*void AddPoint(Vector3 newPosition)
    {
        lineRenderer.positionCount = pointCount + 1;
        lineRenderer.SetPosition(pointCount, newPosition);
        pointCount++;
    }*/















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
        camera1.MonoTone_SetSpeed(5.0f);
        camera1.MonoTone_Enable();
    }

    void StopRewind()
    {
        isRewinding = false;
        animator.speed = 1; //アニメーションを続き
        camera1.MonoTone_SetSpeed(3.0f);
        camera1.MonoTone_Disable();
    }
    public bool IsRewinding()
    {
        return isRewinding;
    }
}
