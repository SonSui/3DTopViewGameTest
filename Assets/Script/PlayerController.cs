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

    public GameObject hitbox;
    private float attackDelayTime = 0.3f;
    private float attackInterval = 1.2f;
    private float preAttackTime = 10f;

    CameraFollow camera1;
    Transform cameraTransform;

    GameManager gameManager = GameManager.Instance;
    AbilityManager abilityManager = AbilityManager.Instance;


    //物理
    public float gravity = 20f;
    public float speedJump = 8f;

    private float defSpeed = 5f;
    public int defLife = 3;
    public int defDmg = 1;
    public float defCrit = 0f;

    /*private LineRenderer lineRenderer;
    private int pointCount = 0;

    public float pointDistance = 0.1f;
    private Vector3 lastPosition;*/

    

    public struct playerState
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
            this.life += life;
            this.speed += speed;
            this.damage += damage;
            this.crit += crit;
        }
        public void SetState(int life, float speed, int damage, float crit,bool explo)
        {
            this.life += life;
            this.speed += speed;
            this.damage += damage;
            this.crit += crit;
            this.isExplo = explo;
        }
        public void UpdateAblitiy(bool explo)
        {
            this.isExplo = explo;
        }
        public void ResetState()
        {
            this.life = 0;
            this.speed = 0;
            this.damage = 0;
            this.crit = 0;
        }
        public string ShowState()
        {
            string st =
                "life:" + this.life.ToString() + "\n" +
                "speed:" + this.speed.ToString() + "\n" +
                "damage:" + this.damage.ToString() + "\n" +
                "critical:" + this.crit.ToString() + "\n" +
                "exploAbility:" + this.isExplo.ToString() + "\n";
            Debug.Log(st);
            return st;
        }
    };
    public playerState state,finalState;

    //巻き戻す変数
    private int bufferSize = 180;
    private int recordInterval = 2;

    private CircularBuffer<TimeSnapShot> snapshots;
    private int frameCounter = 0;
    private bool isRewinding = false;

    public Text currentBuffer;
    

    void Start()
    {
        cameraTransform = gameManager.GetCamera().transform;
        if (cameraTransform == null)
        {
            Debug.Log("Camera not found in the scene.");
        }
        else
        {
            camera1=cameraTransform.GetComponent<CameraFollow>();
            if (camera1 == null)
            {
                Debug.Log("Camera don't have CameraFollow Script");
            }
        }
        

        snapshots = new CircularBuffer<TimeSnapShot>(bufferSize);

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        charaRotationOri = transform.eulerAngles;

        state = new playerState(0,0f,0,0f);
        CaculateFinalState();

        

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
            if (cameraTransform == null) return;
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
                if (Input.GetKeyDown(KeyCode.F) && preAttackTime > attackInterval) 
                {
                    animator.SetTrigger("attack");
                    SpawnHitbox();
                    preAttackTime = 0;
                }
                if(Input.GetKeyDown(KeyCode.C))
                {
                    animator.SetTrigger("dash");
                }
            }
            preAttackTime += Time.deltaTime;
            // 重力計算
            yDirection -= gravity * Time.deltaTime;

            // 移動速度と方向
            moveDirection = horizontalMove * (finalState.speed);
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

    public string GetStateString()
    {
        string st;
        CaculateFinalState();

        st = "Player State:\n" +
            "life:" + finalState.life.ToString() + "\n" +
            "speed:" + finalState.speed.ToString() + "\n" +
            "damage:" + finalState.damage.ToString() + "\n" +
            "critical:" + finalState.crit.ToString() + "\n" +
            "exploAbility:" + finalState.isExplo.ToString() + "\n";

        return st;
    }
    void SpawnHitbox()
    {
        
        Vector3 forwardDirection = transform.forward;

        Vector3 spawnPosition = transform.position + forwardDirection * 0.5f;

        GameObject hit = Instantiate(hitbox, spawnPosition, Quaternion.LookRotation(forwardDirection));
        attack attackScript = hit.GetComponent<attack>();
        if (attackScript != null)
        {
            int attackDamage = finalState.damage;
            float cirtRate = finalState.crit / 100f;
            bool isCriticalHit = (Random.Range(0f,1f) < cirtRate);
            if (isCriticalHit) { attackDamage *= 2; }
            attackScript.Initialize(attackDamage, isCriticalHit,finalState.isExplo);
            hit.SetActive(false);
            hit.transform.SetParent(transform);
            StartCoroutine(EnableAttackAfterDelay(hit));
        }

    }
    private IEnumerator EnableAttackAfterDelay(GameObject at)
    {
        
        yield return new WaitForSeconds(attackDelayTime);

        at.SetActive(true);
    }

    public void CaculateFinalState()
    {
        finalState = state;
        finalState.damage += defDmg;
        finalState.crit += defCrit;
        finalState.speed += defSpeed;
        finalState.life += defLife;
    }
    public void SetFinalState(int life, float speed, int damage, float crit, bool explo)
    {
        finalState.SetState(life, speed, damage, crit, explo);
    }
    










    //-------------------巻き戻す機能-------------------

    void RecordSnapshot()
    {
        //記録
        TimeSnapShot snapshot = new TimeSnapShot(transform, animator,frameCounter);
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
    /*void AddPoint(Vector3 newPosition)
    {
        lineRenderer.positionCount = pointCount + 1;
        lineRenderer.SetPosition(pointCount, newPosition);
        pointCount++;
    }*/
}
