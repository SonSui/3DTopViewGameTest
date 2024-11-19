using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    CharacterController controller;

    //�A�j���[�V����
    Animator animator;
    private HashSet<int> commonAttack;
    private HashSet<int> shootAttack;
    private HashSet<int> idleAct;
    private HashSet<int> runAct;
    private HashSet<int> dashAct;
    private HashSet<int> inpactAct;


    Vector3 moveDirection = Vector3.zero;
    Vector3 charaRotationOri = Vector3.zero;
    Vector2 inputMove = Vector2.zero;

    public CharacterStatus finalState;

    //�U��
    public GameObject hitbox;
    private float attackDelayTime = 0.3f;

    private int comboStep = 0;
    private float comboTimer = 0f;
    private float comboResetTime = 0.6f;
    private int maxComboStep = 2;

    //Hitbox����
    public GameObject rightHand;
    public GameObject rightHandHitbox;
    public GameObject bulletHitbox;
    public GameObject gun;
    public GameObject leftLeg;
    public GameObject leftLegHitbox;


    //���̓o�b�t�@
    public float bufferTime = 0.4f;
    enum Action
    {
        Attack1, Attack2,
        Dash,
    }
    private Queue<(Action, float)> inputQueue = new Queue<(Action, float)>();
    private bool isAnimeOver;

    //�J����
    CameraFollow camera1;
    Transform cameraTransform;

    //�}�l�[�W���[
    GameManager gameManager = GameManager.Instance;


    //����
    public float gravity = 20f;
    private float currSpeed = 1.2f;



    //�_�b�V��
    public bool isDashing;
    private float dashTimeMax = 0.46f;
    private float dashSpeed = 15f;

    private Vector3 dashDirection;
    private float dashTime;
    private float dashEndTime;

    private bool isDashingEventTriggered = false;

    void Start()
    {

        //Animation�̖��O�t��
        commonAttack = new HashSet<int>
        {
            Animator.StringToHash("GreatSwordRound"),
            Animator.StringToHash("GreatSwordKick")
        };
        shootAttack = new HashSet<int> 
        { 
            Animator.StringToHash("Gun") 
        };
        idleAct = new HashSet<int>
        {
            Animator.StringToHash("Idle"),
            Animator.StringToHash("T-Pose")
        };
        runAct = new HashSet<int> 
        { 
            Animator.StringToHash("Running") 
        };
        dashAct = new HashSet<int> 
        { 
            Animator.StringToHash("Dash") 
        };
        inpactAct = new HashSet<int> 
        { 
            Animator.StringToHash("Inpact") 
        };


        controller = GetComponent<CharacterController>();

        //�J�����ǐ�
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
        animator = GetComponent<Animator>();
        charaRotationOri = transform.eulerAngles;
        isAnimeOver = true;
    }

    void Update()
    {
        //�A�j�����
        string animeName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        //�U���R���{�X�V
        if (comboStep > 0)
        {
            UpdateCombo();
        }

        //�J�����̊p�x���m�F
        if (cameraTransform == null) return;
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        // �ړ��������v�Z
        Vector3 horizontalMove = (forward * inputMove.y + right * inputMove.x).normalized;
        // �d�͌v�Z
        float yDirection = moveDirection.y - gravity * Time.deltaTime;

        // �ړ����x�ƕ���
        moveDirection = horizontalMove * currSpeed;
        moveDirection.y = yDirection;
        float speed_ = inputMove.magnitude * currSpeed;



        //controller.Move(moveDirection * Time.deltaTime);
        if (horizontalMove.magnitude > 0.1f && IsRotatable())
        {
            transform.forward = horizontalMove;
            moveDirection = horizontalMove;
        }
        animator.SetFloat("MoveSpeed", speed_);
        //Debug.Log("CurrMoveSpeed:" + speed_ + "Time:"+Time.time);

        
        
        CheckInputQueue();

    }

    void UpdateCombo()
    {
        comboTimer += Time.deltaTime;
        if (comboTimer > comboResetTime)
        {
            comboStep = 0;
            comboTimer = 0f;
            animator.SetInteger("ComboStep", comboStep);
        }
    }

    public void ResetAttack1Combo()
    {
        comboStep = 0;
        animator.SetInteger("ComboStep", comboStep);
    }

    public void SetMoveSpeed(float speed)
    {
        currSpeed = speed;
    }

    void OnMove(InputValue value)//�ړ�����
    {
        Debug.Log("onMove:" + value.ToString());
        inputMove = value.Get<Vector2>();
    }
    void OnDash()//����E�_�b�V��
    {
        Debug.Log("DashKeyDown:" + Time.time);
        AddToInputQueue(Action.Dash);
    }
    void OnAttack1()//�U���P
    {
        Debug.Log("Attack1KeyDown:" + Time.time);
        AddToInputQueue(Action.Attack1);
    }
    void OnAttack2()//�U���Q
    {
        Debug.Log("Attack2KeyDown:" + Time.time);
        AddToInputQueue(Action.Attack2);
    }
    void OnAim(InputValue value)//�Ə�
    {
        Debug.Log("Aim");
        Vector2 forw = value.Get<Vector2>();
        if (cameraTransform == null) return;
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;
        Vector3 horizontalMove = (forward * forw.y + right * forw.x).normalized;
        if (horizontalMove.magnitude > 0.1f && IsRotatable())
        {
            transform.forward = horizontalMove;
            moveDirection = horizontalMove;
        }
    }


    private void AddToInputQueue(Action key)//���̓o�b�t�@�ɓ��͏������
    {
        inputQueue.Enqueue((key, Time.time + bufferTime));
    }
    private void CheckInputQueue()//���̓o�b�t�@�̊Ǘ�
    {
        if (inputQueue.Count <= 0) return;
        while (inputQueue.Peek().Item2 < Time.time)//�����؂ꂽ���͂��̂Ă�
        {
            inputQueue.Dequeue();
            if (inputQueue.Count == 0) return;
        }

        if (inputQueue.Count <= 0) return;
        if (CanPerformNextAction(inputQueue.Peek().Item1))//�����؂�Ȃ����͂��m�F
        {
            Action nextInput = inputQueue.Dequeue().Item1;
            PerformAction(nextInput);
        }
    }

    private bool CanPerformNextAction(Action key)//�A�j���[�V�������s�\���m�F
    {
        if (key == Action.Dash)
        {
            if (!IsInInpactState()) 
                return true;
        }
        return isAnimeOver;
    }

    private void PerformAction(Action input)//�A�N�V�����Ǘ�
    {

        switch (input)
        {

            case Action.Attack1:
                Debug.Log("attack1");
                comboTimer = 0f;
                comboStep++;
                comboStep = comboStep > maxComboStep ? maxComboStep : comboStep;
                animator.SetInteger("ComboStep", comboStep);

                break;
            case Action.Attack2:
                Debug.Log("Shoot");
                //if(moveDirection.magnitude>0.05) transform.forward = moveDirection;
                animator.SetTrigger("Shoot");
                break;
            case Action.Dash:

                if (!isDashing)
                {
                    Debug.Log("dash");
                    animator.SetTrigger("Dash");
                    //StartDashing();
                }
                break;
        }
    }
    public void StartDashing()//�A�j���[�V�����ŌĂ�
    {
        if (isDashingEventTriggered)
        {
            return;
        }

        isDashingEventTriggered = true;
        StartCoroutine(Dashing());
        Invoke(nameof(ResetDashingEvent), dashTimeMax);
    }
    private void ResetDashingEvent()
    {
        isDashingEventTriggered = false;
    }
    private IEnumerator Dashing()//�R���[�`���Ń_�b�V������
    {
        isDashing = true;
        dashDirection = transform.forward;
        dashEndTime = Time.time + dashTimeMax;

        Debug.Log("Start Dashing");
        while (Time.time < dashEndTime)
        {

            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            Debug.Log("dash dir" + dashDirection);
            yield return null;
        }

        isDashing = false;
    }


    //�A�j���[�V������Ԋm�F�֐�
    private bool IsInAttack1State()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;


        return commonAttack.Contains(currentAnimationHash);
    }

    private bool IsInAttack2State()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;


        return shootAttack.Contains(currentAnimationHash);
    }

    private bool IsInInpactState()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;


        return inpactAct.Contains(currentAnimationHash);
    }
    private bool IsInDashState()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;


        return dashAct.Contains(currentAnimationHash);
    }

    private bool IsRotatable()
    {
        return !(IsInDashState() || IsInAttack1State()||IsInInpactState()||IsInAttack2State());
    }

    public void SpawnSwordHitbox()
    {
        int dmg = 2;
        if (rightHandHitbox != null)
        {

            Vector3 spawnPosition = rightHand.transform.position;
            Quaternion spawnRotation = rightHand.transform.rotation;

            GameObject hit = Instantiate(rightHandHitbox, spawnPosition, spawnRotation);
            Hitbox_Sword attackScript = hit.GetComponent<Hitbox_Sword>();
            if (attackScript != null)
            {
                
                attackScript.Initialize(dmg);
                hit.transform.SetParent(rightHand.transform);

            }
        }
    }
}
