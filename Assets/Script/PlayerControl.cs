using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    CharacterController controller;
    Animator animator;
    public GameObject model;

    Vector3 moveDirection = Vector3.zero;
    Vector3 charaRotationOri = Vector3.zero;
    Vector2 inputMove = Vector2.zero;

    public CharacterStatus finalState;

    //攻撃
    public GameObject hitbox;
    private float attackDelayTime = 0.3f;

    private int comboStep = 0;
    private float comboTimer = 0f;
    private float comboResetTime = 1.0f;
    private int maxComboStep = 2;

    //Hitbox生成
    public GameObject rightHand;
    public GameObject rightHandHitbox;
    public GameObject magicHitbox;
    public GameObject leftLeg;
    public GameObject leftLegHitbox;

    //入力バッファ
    public float bufferTime = 0.2f;
    enum Action
    {
        Attack1, Attack2, 
        Dash,
    }
    private Queue<(Action, float)> inputQueue = new Queue<(Action, float)>();
    private bool isAnimeOver;

    //カメラ
    CameraFollow camera1;
    Transform cameraTransform;

    //マネージャー
    GameManager gameManager = GameManager.Instance;


    //物理
    public float gravity = 20f; 
    private float currSpeed = 3.0f;



    //ローリング
    public bool isRolling;
    public float rollingTimeMax = 0.5f;
    private float rollSpeed = 10f;

    private Vector3 rollDirection;
    private float rollTime;
    private float rollEndTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = gameManager.GetCamera().transform;
        if (cameraTransform == null)
        {
            Debug.Log("Camera not found in the scene.");
        }
        else
        {
            camera1 = cameraTransform.GetComponent<CameraFollow>();
            if (camera1 == null)
            {
                Debug.Log("Camera don't have CameraFollow Script");
            }
        }
        controller = GetComponent<CharacterController>();
        animator =  model.GetComponent<Animator>();
        charaRotationOri = transform.eulerAngles;
        isAnimeOver = true;
    }

    void Update()
    {
        //アニメ状態
        string animeName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        //カメラの角度を確認
        if (cameraTransform == null) return;
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        // 移動方向を計算
        Vector3 horizontalMove = (forward * inputMove.y + right * inputMove.x).normalized;
        // 重力計算
        float yDirection = moveDirection.y - gravity * Time.deltaTime;

        // 移動速度と方向
        moveDirection = horizontalMove * currSpeed;
        moveDirection.y = yDirection;

        controller.Move(moveDirection * Time.deltaTime);
        transform.forward = horizontalMove;
        animator.SetFloat("MoveSpeed", horizontalMove.magnitude);
    }



    public void SetMoveSpeed(float speed)
    {
        currSpeed = speed;
    }

    void OnMove(InputValue value)
    {
        Debug.Log("onMove:"+value.ToString());
        inputMove = value.Get<Vector2>();
    }
    void OnDash()
    {
        Debug.Log("Dash");
    }
    void OnAttack1()
    {
        Debug.Log("Attack1");
    }
    void OnAttack2()
    {
        Debug.Log("Attack2");
    }
}
