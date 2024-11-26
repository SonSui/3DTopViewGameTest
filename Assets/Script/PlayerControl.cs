using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    // �A�j���[�V����
    Animator animator;
    private HashSet<int> commonAttack;
    private HashSet<int> shootAttack;
    private HashSet<int> idleAct;
    private HashSet<int> runAct;
    private HashSet<int> dashAct;
    private HashSet<int> impactAct;
    private HashSet<int> dyingAct;

    // �ړ�
    CharacterController controller;
    Vector3 moveDirection = Vector3.zero;
    Vector3 charaRotationOri = Vector3.zero;
    Vector2 inputMove = Vector2.zero;
    private float rotationSpeed = 20f;
    private float currMoveSpeed = 1.2f;


    // �U��
    private float currActSpeed = 1.2f;

    private int comboStep = 0;
    private float comboTimer = 0f;
    private float comboResetTime = 0.8f;
    private int maxComboStep = 3;

    private bool isAttack1Acceptable = true;
    private bool isAttack2Acceptable = true;

    // Hitbox�����i�G�t�F�N�g�܂݁j
    public GameObject onAttackSword;
    public GameObject unAttackSword;
    public GameObject swordAttack01Hitbox;
    public GameObject swordAttack02Hitbox1;
    public GameObject swordAttack02Hitbox2;
    public GameObject swordAttack03Hitbox;

    public GameObject bulletHitbox;
    public GameObject gun;
    public GameObject leftLeg;
    public GameObject leftLegHitbox;

    //�@�G�t�F�N�g
    public GameObject dashEffect;


    // ���̓o�b�t�@
    enum Action
    {
        Attack1, Attack2,
        Dash,
    }
    private Queue<(Action, float)> inputQueue = new Queue<(Action, float)>();

    private bool isAnimeOver;
    private bool isShortRotatable = false;

    public float bufferTime = 0.25f;

    // �J����
    CameraFollow camera1;
    Transform cameraTransform;

    // �}�l�[�W���[
    GameManager gameManager = GameManager.Instance;


    // ����
    public float gravity = 20f;





    //�_�b�V��
    private bool isDashing = false;
    private float dashTimeMax = 0.46f;
    private float dashSpeed = 15f;

    private Vector3 dashDirection;
    private float dashTime;
    private float dashEndTime;

    private bool isDashingEventTriggered = false;

    // ��e
    private bool isOnImpact = false;

    void Start()
    {

        //Animation�̖��O�t��
        commonAttack = new HashSet<int>
        {
            Animator.StringToHash("SwordAttack01"),
            Animator.StringToHash("SwordAttack02"),
            Animator.StringToHash("SwordAttack03"),
            Animator.StringToHash("GreatSwordRound"),
            Animator.StringToHash("GreatSwordKick"),
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
        impactAct = new HashSet<int>
        {
            Animator.StringToHash("Impact")
        };
        dyingAct = new HashSet<int>
        {
            Animator.StringToHash("Dying")
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
        SetActSpeed(currActSpeed);
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
        Vector3 gra = new Vector3(0f, yDirection, 0f);
        controller.Move(gra * Time.deltaTime);

        // �ړ������Ƒ��x�i�΂�1.414�{�j
        moveDirection = horizontalMove;
        float speed_ = inputMove.magnitude * currMoveSpeed;

        


        if (horizontalMove.magnitude > 0.1f && IsRotatable())
        {
            //��]
            Quaternion targetRotation = Quaternion.LookRotation(horizontalMove, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        animator.SetFloat("MoveSpeed", speed_);
        //Debug.Log("CurrMoveSpeed:" + speed_ + "Time:"+Time.time);



        CheckInputQueue();

    }

    // ===== Combo =====
    void UpdateCombo()
    {
        // �U�����͒�~����Ǝ����I�ɃR���{�L�����Z�b�g
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
        //�R���{�L�����Z�b�g
        comboStep = 0;
        comboTimer = 0;
        animator.SetInteger("ComboStep", comboStep);
    }




    // ===== ���͊Ǘ� =====

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
        if (key == Action.Dash) //�_�b�V�����荞��
        {
            if (!IsInImpactState())
                return true;
        }
        return isAnimeOver;
    }

    private void PerformAction(Action input)//�A�N�V�����Ǘ�
    {

        switch (input)
        {

            case Action.Attack1:

                if (isAttack1Acceptable)
                {
                    isAttack1Acceptable = false; //���̍U���A�j�������O�ɃR���{���͕s��
                    Debug.Log("attack1");
                    comboStep++;
                    comboStep %= (maxComboStep+1); //�A���R���{�̂��߁A�z�ɂ���
                    comboTimer = 0;
                    animator.SetInteger("ComboStep", comboStep);
                }

                break;
            case Action.Attack2:
                if (isAttack2Acceptable)
                {
                    isAttack2Acceptable = false;
                    Debug.Log("Shoot");
                    //if(moveDirection.magnitude>0.05) transform.forward = moveDirection;
                    animator.SetTrigger("Shoot");
                }
                break;
            case Action.Dash:

                if (!isDashing)
                {
                    Debug.Log("dash");
                    animator.SetTrigger("Dash");
                }
                break;
        }
    }



    // ===== �A�j���[�V�����C�x���g =====

    public void OnRunningAnime()
    {
        // ����̕\���ʒu�Ɠ��͂𐮂�
        isAttack1Acceptable = true;
        isAttack2Acceptable = true;
        unAttackSword.SetActive(true);
        onAttackSword.SetActive(false);
        UnableAllHitBox();
    }
    public void OnIdleAnime()
    {
        // ����̕\���ʒu�Ɠ��͂𐮂�
        isAttack1Acceptable = true;
        isAttack2Acceptable = true;
        unAttackSword.SetActive(true);
        onAttackSword.SetActive(false);
        UnableAllHitBox();
    }

    public void OnSwordAttack01Enter()
    {
        // ����̍U���̕\���ʒu�ƃR���{����
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(true);
        comboResetTime = 0.81f/currActSpeed;
        comboTimer = 0f;
    }
    public void OnSwordAttack01Update1()
    {
        // Hitbox
        swordAttack01Hitbox.SetActive(true);
        swordAttack01Hitbox.GetComponent<Hitbox_Sword>().Initialize(gameManager.GetPlayerAttackNow());
    }
    public void OnSwordAttack01Update2()
    {
        // ���̃R���{���͉\
        isAttack1Acceptable = true;
        animator.ResetTrigger("Shoot");//�������U�����Z�b�g
    }
    public void OnSwordAttack01Exit()
    {
        // Hitbox��\��
        swordAttack01Hitbox.SetActive(false);
        SetShortRotateBool(0.28f); //�Z���Ԃɉ�]�\
    }
    public void OnSwordAttack02Enter()
    {
        // ����̍U���̕\���ʒu��Hitbox�ƃR���{����
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(true);
        swordAttack02Hitbox1.SetActive(true);
        swordAttack02Hitbox1.GetComponent<Hitbox_Sword>().Initialize(gameManager.GetPlayerAttackNow());
        comboResetTime = 0.81f / currActSpeed;
        comboTimer = 0f;
    }
    public void OnSwordAttack02Update1()
    {
        // ���̃R���{���͉\
        isAttack1Acceptable = true;
        animator.ResetTrigger("Shoot");//�������U�����Z�b�g
    }
    public void OnSwordAttack02Update2()
    {
        // ��i�R���{�̂ŁA��ڂ�Hitbox�\��
        swordAttack02Hitbox1.SetActive(false);
        swordAttack02Hitbox2.SetActive(true);
        swordAttack02Hitbox2.GetComponent<Hitbox_Sword>().Initialize(gameManager.GetPlayerAttackNow());
    }
    public void OnSwordArrack02Exit()
    {
        // Hitbox��\��
        swordAttack02Hitbox2.SetActive(false);
        SetShortRotateBool(); //�Z���Ԃɉ�]�\
    }


    public void OnSwordAttack03Enter()
    {
        // ����̍U���̕\���ʒu��Hitbox�ƃR���{����
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(true);
        swordAttack03Hitbox.SetActive(true);
        swordAttack03Hitbox.GetComponent<Hitbox_Sword>().Initialize(gameManager.GetPlayerAttackNow());
        comboResetTime = 0.83f / currActSpeed;
        comboTimer = 0f;
        comboStep = 0; //�A���R���{�̂��߂Ƀ��Z�b�g
        animator.SetInteger("ComboStep", comboStep);
    }
    public void OnSwordAttack03Update()
    {
        // ���̃R���{���͉\
        isAttack1Acceptable = true;
        animator.ResetTrigger("Shoot"); //�������U�����Z�b�g
    }
    public void OnSwordAttack03Exit()
    {
        // Hitbox��\��
        swordAttack03Hitbox.SetActive(false);  
    }

    public void OnShootEnter()
    {
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(false);
    }
    public void OnShoot()
    {
        

        Vector3 gunRot = gun.transform.eulerAngles;
        gunRot.x = 90f;
        
        GameObject bull = Instantiate(bulletHitbox, gun.transform.position, Quaternion.Euler(gunRot));
        bull.GetComponent<Hitbox_PlayerBullet>().Initialize(gameManager.GetPlayerAttackNow());
        
    }

    public void OnImpact()
    {
        //���荞�݉\�̂ŁA���ׂẴR���g���[������ƕ\�������Z�b�g
        unAttackSword.SetActive(true);
        onAttackSword.SetActive(false);
        UnableAllHitBox();
        ResetAttack1Combo();
        animator.ResetTrigger("Shoot");
    }


    public void StartDashing()
    {
        if (isDashingEventTriggered)
        {
            return;
        }

        isDashingEventTriggered = true;

        //�_�b�V���͊��荞�݉\�̂ŁA���ׂẴR���g���[������ƕ\�������Z�b�g
        unAttackSword.SetActive(true);
        onAttackSword.SetActive(false);
        UnableAllHitBox();
        ResetAttack1Combo();
        animator.ResetTrigger("Shoot");

        dashEffect.SetActive(true);�@//�_�b�V���̃G�t�F�N�g
        SetShortRotateBool(0.08f);�@//��]�\
        StartCoroutine(Dashing());�@//�R���[�`���ňړ�
        Invoke(nameof(ResetDashingEvent), dashTimeMax);�@//
    }
    private void ResetDashingEvent()
    {
        dashEffect.SetActive(false);
        isDashingEventTriggered = false;
        isAttack1Acceptable = true;
        isAttack2Acceptable = true;
    }
    private IEnumerator Dashing()//�R���[�`���Ń_�b�V������
    {
        isDashing = true;
        dashEndTime = Time.time + dashTimeMax;

        Debug.Log("Start Dashing");
        while (Time.time < dashEndTime)
        {
            dashDirection = transform.forward;
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            //Debug.Log("dash dir" + dashDirection);
            UnableAllHitBox();
            yield return null;
        }

        isDashing = false;
    }
    private void SetShortRotateBool(float sTime = 0.3f)
    {
        // �Z�����ԓ��ŉ�]�ł���
        isShortRotatable = true;
        Debug.Log("Can rot");
        StartCoroutine(ResetBoolAfterDelay(sTime)); 
    }

    private System.Collections.IEnumerator ResetBoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isShortRotatable = false;
        Debug.Log("Cant rot");
    }
    private void UnableAllHitBox()
    {
        //���ׂĂ�Hitbox��\��
        swordAttack01Hitbox.SetActive(false);
        swordAttack02Hitbox1.SetActive(false);
        swordAttack02Hitbox2.SetActive(false);
        swordAttack03Hitbox.SetActive(false);
    }



    // ===== �A�j���[�V������� =====
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

    private bool IsInImpactState()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return impactAct.Contains(currentAnimationHash);
    }
    private bool IsInDashState()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return dashAct.Contains(currentAnimationHash);
    }

    private bool IsInDyingState() 
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return dyingAct.Contains(currentAnimationHash);
    }

    private bool IsRotatable()
    {
        //�_�b�V���A���ʍU���A��e�̎���]�s��
        bool rotAnime = !(IsInDashState() || IsInAttack1State() || IsInImpactState() || IsInAttack2State()|| IsInDyingState());
        return rotAnime || isShortRotatable;
    }


    // ===== ImputSystem =====
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
        
        if (cameraTransform == null) return;

        Vector2 forw = value.Get<Vector2>();
        //�@�J�������Ə��������v�Z
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;
        Vector3 horizontalMove = (forward * forw.y + right * forw.x).normalized;
        
        if (horizontalMove.magnitude > 0.1f && IsRotatable())
        {
            moveDirection = horizontalMove;
            Quaternion targetRotation = Quaternion.LookRotation(horizontalMove, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    //  ===== Getter/Setter =====
    public void SetMoveSpeed(float speed)
    {
        currMoveSpeed = speed;
    }
    public bool IsDashing()
    {
        return isDashing;
    }
    public void SetActSpeed(float speed)
    {
        currActSpeed = speed;
        animator.SetFloat("ActSpeed",currActSpeed);
    }

    public void OnHit(int dmg)
    {
        if (IsInDyingState()||IsInImpactState()||IsInDashState())
        {
            return;
        }
        // ��e
        int applyDmg = gameManager.PlayerTakeDamage(dmg);
        if (applyDmg > 0) animator.SetTrigger("Impact");
    }
    public void OnDying()
    {
        animator.SetTrigger("Dead");
    }
}
