using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public Transform charaTrans;
    // �A�j���[�V����
    public Animator animator;
    private HashSet<int> commonAttack;
    private HashSet<int> shootAttack;
    private HashSet<int> hookAttack;
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
    private float currActSpeed = 1.3f;

    private int comboStep = 0;
    private float comboTimer = 0f;
    private float comboResetTime = 0.8f;
    private int maxComboStep = 3;

    private bool isAttack1Acceptable = true;
    private bool isAttack2Acceptable = true;

    // Hitbox�����i�G�t�F�N�g�܂݁j
    public GameObject swordCube;
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
    public GameObject evasionEffect;
    public GameObject hitEffect;


    // ���̓o�b�t�@
    enum Action
    {
        Attack1, Attack2, Hook,
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

    private int defaultLayer;
    private int dashingLayer; //�G�Ƃ̏Փ˖h�~

    private bool isDashingEventTriggered = false;


    //�t�b�N
    public GameObject hookPrefab;
    public GameObject hookShooter;
    private HookMove hookMove;
    private bool isHookAcceptable = true;
    private bool isPulling = false;
    private float pullDuration = 1.0f;
    private GameObject pullTarget;




    // ��e
    private bool isOnImpact = false;
    private bool isInvic = false;
    private float InvincibilityTime = 1.1f;
    private float currInvinTime = 0f;

    //�����Ə�
    private float detectionAngle = 60f; 
    private float detectionDistance = 20f;
    private float deltaDistance = 1f; // �ŋߋ����Ɣ�r���鋖�e�͈�
    private Coroutine rotationCoroutine;

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
        hookAttack = new HashSet<int>
        {
            Animator.StringToHash("ShootHook"),
            Animator.StringToHash("PullHook")
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
        defaultLayer = gameObject.layer;
        dashingLayer = LayerMask.NameToLayer("DashingPlayer");

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
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        SetActSpeed(currActSpeed);
        charaRotationOri = transform.eulerAngles;
        isAnimeOver = true;
    }

    void Update()
    {
        if (charaTrans != null&& charaTrans.localPosition.magnitude>0.001f)
        {

            Vector3 worldMovement = charaTrans.TransformDirection(charaTrans.localPosition); 
            worldMovement.y = 0; 
            controller.Move(worldMovement);
            charaTrans.localPosition = Vector3.zero;
            //Debug.Log("World movement: " + worldMovement);
        }
        

        //�A�j�����
        string animeName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        if (isInvic) //��e�̖��G����
        {
            currInvinTime += Time.deltaTime;
            if (currInvinTime > InvincibilityTime)
            {
                isInvic = false;
            }
        }


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

        // �ړ������Ƒ��x
        moveDirection = horizontalMove;
        float speed_ = inputMove.magnitude * currMoveSpeed;




        if (horizontalMove.magnitude > 0.1f && IsRotatable())
        {
            //��]
            Quaternion targetRotation = Quaternion.LookRotation(horizontalMove, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            charaTrans.localRotation = Quaternion.identity;
        }
        animator.SetFloat("MoveSpeed", speed_);
        //Debug.Log("CurrMoveSpeed:" + speed_ + "Time:"+Time.time);

        CheckInputQueue();

    }


    // ===== Attack1Combo =====
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
            if (!(IsInImpactState()||IsInHookState()))
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
                    comboStep %= (maxComboStep + 1); //�A���R���{�̂��߁A�z�ɂ���
                    comboTimer = 0;
                    animator.SetInteger("ComboStep", comboStep);
                }

                break;
            case Action.Attack2:
                if (isAttack2Acceptable&&gameManager.IsHaveAmmo())
                {
                    isAttack2Acceptable = false;
                    Debug.Log("Shoot");
                    //if(moveDirection.magnitude>0.05) transform.forward = moveDirection;
                    animator.SetTrigger("Shoot");
                }
                break;

            case Action.Hook:
                if (isHookAcceptable)
                {
                    isHookAcceptable = false;
                    Debug.Log("HookShoot");
                    //if(moveDirection.magnitude>0.05) transform.forward = moveDirection;
                    animator.SetTrigger("HookShoot");
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
        isHookAcceptable = true;
        UnAttackWeaponDisplay();
        UnableAllHitBox();
        
    }
    public void OnIdleAnime()
    {
        // ����̕\���ʒu�Ɠ��͂𐮂�
        isAttack1Acceptable = true;
        isAttack2Acceptable = true;
        isHookAcceptable = true;
        UnAttackWeaponDisplay() ;

        UnableAllHitBox();
        
    }
    public void OnSwordAttack01Enter()
    {
        // ����̍U���̕\���ʒu�ƃR���{����
        OnAttackWeaponDisplay();
        comboResetTime = 0.81f / currActSpeed;
        comboTimer = 0f;
        //AdjustYRotationRelativeToParent(charaTrans, 128.154f);
        
        AdjustRotationToNearestEnemy(120f);



    }
    public void OnSwordAttack01Update1()
    {
        // Hitbox
        swordAttack01Hitbox.SetActive(true);
        int type = 0;
        if (gameManager.playerStatus.GetIsBleedingEffectFlag())
        {
            type = 1;
        }
        swordAttack01Hitbox.GetComponent<Hitbox_Sword>().Initialize(
            camera1,
            gameManager.GetPlayerAttackNow(),
            type,
            gameManager.playerStatus.GetCriticalRate(),
            gameManager.playerStatus.IsDefensePenetrationEnabled(),
            gameManager.playerStatus.GetIsBleedingEffectFlag(),
            gameManager.playerStatus.GetIsDefenseReductionFlag(),
            gameManager.playerStatus.GetIsAttackReductionFlag(),
            gameManager.playerStatus.GetHpAutoRecovery(),
            gameManager.playerStatus.IsExplosionEnabled()

            );
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
        OnAttackWeaponDisplay();
        swordAttack02Hitbox1.SetActive(true);
        int type = 0;
        if (gameManager.playerStatus.GetIsBleedingEffectFlag())
        {
            type = 1;
        }
        swordAttack02Hitbox1.GetComponent<Hitbox_Sword>().Initialize(
            camera1,
            gameManager.GetPlayerAttackNow(),
            type,
            gameManager.playerStatus.GetCriticalRate(),
            gameManager.playerStatus.IsDefensePenetrationEnabled(),
            gameManager.playerStatus.GetIsBleedingEffectFlag(),
            gameManager.playerStatus.GetIsDefenseReductionFlag(),
            gameManager.playerStatus.GetIsAttackReductionFlag(),
            gameManager.playerStatus.GetHpAutoRecovery(),
            gameManager.playerStatus.IsExplosionEnabled());
        comboResetTime = 0.81f / currActSpeed;
        comboTimer = 0f;
        //AdjustYRotationRelativeToParent(charaTrans, 4.967f);
        AdjustRotationToNearestEnemy(120f);
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
        int type = 0;
        if (gameManager.playerStatus.GetIsBleedingEffectFlag())
        {
            type = 1;
        }
        swordAttack02Hitbox2.GetComponent<Hitbox_Sword>().Initialize(
            camera1,
            gameManager.GetPlayerAttackNow(),
            type,
            gameManager.playerStatus.GetCriticalRate(),
            gameManager.playerStatus.IsDefensePenetrationEnabled(),
            gameManager.playerStatus.GetIsBleedingEffectFlag(),
            gameManager.playerStatus.GetIsDefenseReductionFlag(),
            gameManager.playerStatus.GetIsAttackReductionFlag(),
            gameManager.playerStatus.GetHpAutoRecovery(),
            gameManager.playerStatus.IsExplosionEnabled());
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
        OnAttackWeaponDisplay();
        swordAttack03Hitbox.SetActive(true);
        int type = 0;
        if (gameManager.playerStatus.GetIsBleedingEffectFlag())
        {
            type = 1;
        }
        swordAttack03Hitbox.GetComponent<Hitbox_Sword>().Initialize(
            camera1,
            gameManager.GetPlayerAttackNow(),
            type,
            gameManager.playerStatus.GetCriticalRate(),
            gameManager.playerStatus.IsDefensePenetrationEnabled(),
            gameManager.playerStatus.GetIsBleedingEffectFlag(),
            gameManager.playerStatus.GetIsDefenseReductionFlag(),
            gameManager.playerStatus.GetIsAttackReductionFlag(),
            gameManager.playerStatus.GetHpAutoRecovery(),
            gameManager.playerStatus.IsExplosionEnabled());
        comboResetTime = 0.83f / currActSpeed;
        comboTimer = 0f;
        comboStep = 0; //�A���R���{�̂��߂Ƀ��Z�b�g
        animator.SetInteger("ComboStep", comboStep);
        //AdjustYRotationRelativeToParent(charaTrans, -75.012f);
        AdjustRotationToNearestEnemy(120f);
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
        OnShootWeaponDisplay();
        
    }
    public void OnShoot()
    {
        //AdjustYRotationRelativeToParent(charaTrans, -39.203f);
        AdjustRotationToNearestEnemy();
        Vector3 gunRot = gun.transform.eulerAngles;
        gunRot.x = 90f;
        
        GameObject bull = Instantiate(bulletHitbox, gun.transform.position, Quaternion.Euler(gunRot));
        bull.GetComponent<Hitbox_PlayerBullet>().Initialize(gameManager.GetPlayerAttackNow());
        gameManager.UseAmmo();
    }
    public void OnHookEnter()
    {
        OnHookWeaponDisplay();
        SetShortRotateBool(0.2f);
        isAttack1Acceptable = false;
        
    }
    public void OnHookShoot()
    {
        //AdjustYRotationRelativeToParent(charaTrans, -41.015f);
        AdjustRotationToNearestEnemy();
        GameObject hook = Instantiate(hookPrefab, hookShooter.transform.position, hookShooter.transform.rotation);
        hookMove = hook.GetComponent<HookMove>();
        hookMove.InitHook(this, hookShooter, gameManager.GetPlayerAttackNow());
        animator.SetFloat("HookSpeed", 0.1f);

    }
    public void OnImpact()
    {
        //���荞�݉\�̂ŁA���ׂẴR���g���[������ƕ\�������Z�b�g
        UnAttackWeaponDisplay();
        UnableAllHitBox();
        ResetAttack1Combo();
        animator.ResetTrigger("Shoot");
        animator.ResetTrigger("HookShoot");
        isAttack1Acceptable = true;
        isAttack2Acceptable = true;
    }
    public void StartDashing()
    {
        if (isDashingEventTriggered)
        {
            return;
        }

        isDashingEventTriggered = true;

        //�_�b�V���͊��荞�݉\�̂ŁA���ׂẴR���g���[������ƕ\�������Z�b�g
        UnAttackWeaponDisplay();
        UnableAllHitBox();
        ResetAttack1Combo();
        animator.ResetTrigger("Shoot");
        gameObject.layer = dashingLayer;//�G�Ƃ̏Ռ��֎~

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
        isHookAcceptable = true;
        gameObject.layer = defaultLayer;//�G�Ƃ̏Ռ��߂�
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
    private void SpawnEvasionEffect()
    {
        GameObject eff = Instantiate(evasionEffect);
        eff.transform.position = transform.position;
    }
    private void SetShortRotateBool(float sTime = 0.3f)
    {
        // �Z�����ԓ��ŉ�]�ł���
        /*isShortRotatable = true;
        Debug.Log("Can rot");
        StartCoroutine(ResetBoolAfterDelay(sTime)); */
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
    private void UnAttackWeaponDisplay()
    {
        unAttackSword.SetActive(true);
        onAttackSword.SetActive(false);
        //���f�����O�Ȃ��ŁA������:
        //Gun
        //HookShoot
    }
    private void OnAttackWeaponDisplay()
    {
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(true);
        //���f�����O�Ȃ��ŁA������:
        //Gun
        //HookShoot
    }
    private void OnShootWeaponDisplay()
    {
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(false);
        //���f�����O�Ȃ��ŁA������:
        //Gun
        //HookShoot
    }
    private void OnHookWeaponDisplay()
    {
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(false);
        //���f�����O�Ȃ��ŁA������:
        //Gun
        //HookShoot
    }
    private void AdjustRotationToNearestEnemy(float plusAngle = 0f)
    {
        Vector3 forward = transform.forward; // ���݂̑O�����x�N�g��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionDistance); // ���o�͈͓��̃R���C�_�[���擾
        Transform targetEnemy = null; // �^�[�Q�b�g�ƂȂ�G
        float effectiveAngle = detectionAngle + plusAngle; // �L���Ȍ��o�p�x
        
        float nearestValue = float.MaxValue; // �D�揇�ʒl�i�p�x�܂��͋����j

        if (plusAngle == 0f)
        {
            // plusAngle��0�̏ꍇ�F��]�p�x���ŏ��̓G��T��
            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Enemy") && !hit.GetComponent<IOnHit>().IsDying()) // "Enemy"�^�O���񎀖S��Ԃ��m�F
                {
                    Vector3 directionToEnemy = (hit.transform.position - transform.position).normalized; // �G�ւ̕����x�N�g�����v�Z
                    float angle = Vector3.Angle(forward, directionToEnemy); // �G�ƑO�����Ƃ̊p�x���v�Z

                    if (angle <= detectionAngle && angle < nearestValue)
                    {
                        targetEnemy = hit.transform;
                        nearestValue = angle; // �ŏ��p�x���X�V
                    }
                }
            }
        }
        else
        {
            // plusAngle��0�ȊO�̏ꍇ�F�ł��߂��G��T��
            Transform nearestEnemy = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Enemy") && !hit.GetComponent<IOnHit>().IsDying()) // "Enemy"�^�O���񎀖S��Ԃ��m�F
                {
                    Vector3 directionToEnemy = (hit.transform.position - transform.position).normalized; // �G�ւ̕����x�N�g�����v�Z
                    float angle = Vector3.Angle(forward, directionToEnemy); // �G�ƑO�����Ƃ̊p�x���v�Z

                    if (angle <= effectiveAngle) // ���o�\�Ȋp�x��
                    {
                        float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position); // �G�Ƃ̋������v�Z
                        if (distanceToEnemy < nearestDistance)
                        {
                            nearestEnemy = hit.transform;
                            nearestDistance = distanceToEnemy; // �ŒZ�������X�V
                        }
                    }
                }
            }

            // ��������1�����̓G�̒��Ŋp�x���ŏ��̓G��T��
            if (nearestEnemy != null)
            {
                targetEnemy = nearestEnemy; // �����l�Ƃ��čł��߂��G��ݒ�
                float smallestAngle = Vector3.Angle(forward, (nearestEnemy.position - transform.position).normalized); // �����p�x

                foreach (Collider hit in hitColliders)
                {
                    if (hit.CompareTag("Enemy") && !hit.GetComponent<IOnHit>().IsDying()) // "Enemy"�^�O���񎀖S��Ԃ��m�F
                    {
                        Vector3 directionToEnemy = (hit.transform.position - transform.position).normalized; // �G�ւ̕����x�N�g�����v�Z
                        float angle = Vector3.Angle(forward, directionToEnemy); // �G�ƑO�����Ƃ̊p�x���v�Z

                        if (angle <= effectiveAngle) // ���o�\�Ȋp�x��
                        {
                            float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position); // �G�Ƃ̋������v�Z
                            if (Mathf.Abs(distanceToEnemy - nearestDistance) < deltaDistance) // ��������deltaDistance����
                            {
                                if (angle < smallestAngle)
                                {
                                    targetEnemy = hit.transform; // �p�x���ŏ��̓G���X�V
                                    smallestAngle = angle;
                                }
                            }
                        }
                    }
                }
            }
        }

        // �ŏI�I�ȃ^�[�Q�b�g�ɉ�]
        if (targetEnemy != null)
        {
            Debug.Log("�^�[�Q�b�g�ɉ�]��: " + targetEnemy.name);

            // �^�[�Q�b�g�������v�Z
            Vector3 targetDirection = (targetEnemy.position - transform.position).normalized;
            targetDirection.y = 0; // Y���̍����̉e���𖳎�

            // ��]��ݒ�
            transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            // �L�����N�^�[�̃��[�J����]�����Z�b�g
            if (plusAngle != 0f)
            {
                charaTrans.localRotation = Quaternion.identity;
            }
        }
        else
        {
            Debug.Log("��]�Ώۂ̓G��������܂���ł����B");
        }
    }


    void AdjustYRotationRelativeToParent(Transform target, float targetYRotation)
    {
        // �e�I�u�W�F�N�g�����݂��邩�m�F
        if (target.parent == null)
        {
            Debug.LogError("�w�肳�ꂽ�I�u�W�F�N�g�ɐe�I�u�W�F�N�g������܂���I");
            return;
        }

        // ���݂̃��[�J����]���擾
        Quaternion currentLocalRotation = target.localRotation;

        // �V�������[�J����]���v�Z�iX����Z���͕ύX�����AY���̂ݐݒ�j
        Quaternion newLocalRotation = Quaternion.Euler(
            currentLocalRotation.eulerAngles.x,
            targetYRotation,
            currentLocalRotation.eulerAngles.z
        );

        // �v�Z�������[�J����]��K�p
        target.localRotation = newLocalRotation;

    }

    public void OnDeadAnimation()
    {
        controller.center = new Vector3(0f, 2f, 3f);
        StartCoroutine(DeadCountDown(2f));
    }
    private System.Collections.IEnumerator DeadCountDown(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameManager.PlayerDeadAnimeOver();
    }

    // ===== �A�j���[�V������� =====
    private bool IsInAttack1State()
    {   //�ߐڍU��
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return commonAttack.Contains(currentAnimationHash);
    }
    private bool IsInAttack2State()
    {�@ // �������U��
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return shootAttack.Contains(currentAnimationHash);
    }
    private bool IsInHookState() 
    {
        // �z�b�N�V���b�g
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return hookAttack.Contains(currentAnimationHash);
    }
    private bool IsInImpactState()
    {   // ��e
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return impactAct.Contains(currentAnimationHash);
    }
    private bool IsInDashState()
    {   // �_�b�V��
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return dashAct.Contains(currentAnimationHash);
    }
    private bool IsInDyingState() 
    {   // ���S
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return dyingAct.Contains(currentAnimationHash);
    }
    private bool IsRotatable()
    {
        //�_�b�V���A���ʍU���A��e�̎���]�s��
        bool rotAnime = !(IsInDashState() || IsInAttack1State() || IsInImpactState() || IsInAttack2State()|| IsInDyingState()|| IsInHookState());
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
    void OnHook()
    {
        Debug.Log("HookShootKeyDown:" + Time.time);
        AddToInputQueue(Action.Hook);
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

    //  ===== �C���^���N�V���� =====
    public void PullPlayer(GameObject target)
    {
        pullTarget = target;
        animator.SetBool("isHooked", true);
        animator.SetFloat("HookSpeed", 1f);


        StartCoroutine(PullToHook());
    }
    private IEnumerator PullToHook()
    {
        if (pullTarget != null)
        {
            isPulling = true;
            
            float elapsedTime = 0f;

            Vector3 initialPosition = transform.position;

            CharacterController characterController = GetComponent<CharacterController>();

            float distance = (pullTarget.transform.position - transform.position).magnitude;
            if (distance < 2) elapsedTime += 0.6f; //�������߂��Ȃ瑁���I��点��

            while (elapsedTime < pullDuration)
            {
                if (pullTarget == null) break;

                elapsedTime += Time.deltaTime;

                //�v���C���[�̕����v�Z
                Vector3 targetPosition = pullTarget.transform.position;
                targetPosition.y = transform.position.y;

                //�ړ������v�Z
                Vector3 newPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / pullDuration);
                Vector3 displacement = newPosition - transform.position;

                //��]
                Vector3 dir = targetPosition - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                
                //�ړ�
                characterController.Move(displacement);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                yield return null;
            }
        }
        isPulling = false;
        isAttack1Acceptable = true;
        animator.SetBool("isHooked", false);
    }
    public void HookDown()
    {
        animator.SetBool("isHooked", false);
        animator.SetFloat("HookSpeed", 1f);
    }
    public void OnHit(int dmg)
    {
        if (IsDashing())
        {
            //����G�t�F�N�g
            SpawnEvasionEffect();
            return;
        }
        if (IsInDyingState()||IsInImpactState()||isInvic)
        {
            return;
        }
        // ��e
        int applyDmg = gameManager.PlayerTakeDamage(dmg);
        if (applyDmg > 0)
        {
            animator.SetTrigger("Impact");
            GameObject impact = Instantiate(hitEffect, dashEffect.transform.position, Quaternion.identity);
            Destroy(impact,1f);
            
            isInvic = true;
            currInvinTime = 0;
        }
    }
    public void OnDying()
    {
        //���S
        UnableAllHitBox();
        animator.SetTrigger("Dead");
    }
    public void VibrateForDuration(float duration=0.2f,float speed_ = 0.5f)
    {
        var gamepad = Gamepad.current; 
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(speed_, speed_); 
            Invoke(nameof(StopVibration), duration); 
        }
        else
        {
            Debug.LogWarning("No gamepad connected!");
        }
    }

    private void StopVibration()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f); 
        }
    }
    public void SetSwordCube(float rage=1f)
    {
        swordCube.transform.localScale = new Vector3(rage, rage, rage);
    }
}
