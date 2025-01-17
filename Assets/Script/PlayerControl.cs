using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public Transform charaTrans;
    // アニメーション
    public Animator animator;
    private HashSet<int> commonAttack;
    private HashSet<int> shootAttack;
    private HashSet<int> hookAttack;
    private HashSet<int> idleAct;
    private HashSet<int> runAct;
    private HashSet<int> dashAct;
    private HashSet<int> impactAct;
    private HashSet<int> dyingAct;

    // 移動
    CharacterController controller;
    Vector3 moveDirection = Vector3.zero;
    Vector3 charaRotationOri = Vector3.zero;
    Vector2 inputMove = Vector2.zero;
    private float rotationSpeed = 20f;
    private float currMoveSpeed = 1.2f;


    // 攻撃
    private float currActSpeed = 1.3f;

    private int comboStep = 0;
    private float comboTimer = 0f;
    private float comboResetTime = 0.8f;
    private int maxComboStep = 3;

    private bool isAttack1Acceptable = true;
    private bool isAttack2Acceptable = true;

    // Hitbox生成（エフェクト含み）
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

    //　エフェクト
    public GameObject dashEffect;
    public GameObject evasionEffect;
    public GameObject hitEffect;


    // 入力バッファ
    enum Action
    {
        Attack1, Attack2, Hook,
        Dash,
    }
    private Queue<(Action, float)> inputQueue = new Queue<(Action, float)>();

    private bool isAnimeOver;
    private bool isShortRotatable = false;

    public float bufferTime = 0.25f;

    // カメラ
    CameraFollow camera1;
    Transform cameraTransform;

    // マネージャー
    GameManager gameManager = GameManager.Instance;


    // 物理
    public float gravity = 20f;


    //ダッシュ
    private bool isDashing = false;
    private float dashTimeMax = 0.46f;
    private float dashSpeed = 15f;

    private Vector3 dashDirection;
    private float dashTime;
    private float dashEndTime;

    private int defaultLayer;
    private int dashingLayer; //敵との衝突防止

    private bool isDashingEventTriggered = false;


    //フック
    public GameObject hookPrefab;
    public GameObject hookShooter;
    private HookMove hookMove;
    private bool isHookAcceptable = true;
    private bool isPulling = false;
    private float pullDuration = 1.0f;
    private GameObject pullTarget;




    // 被弾
    private bool isOnImpact = false;
    private bool isInvic = false;
    private float InvincibilityTime = 1.1f;
    private float currInvinTime = 0f;

    //自動照準
    private float detectionAngle = 60f; 
    private float detectionDistance = 20f;
    private float deltaDistance = 1f; // 最近距離と比較する許容範囲
    private Coroutine rotationCoroutine;

    void Start()
    {

        //Animationの名前付け
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

        //カメラ追跡
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
        

        //アニメ状態
        string animeName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        if (isInvic) //被弾の無敵時間
        {
            currInvinTime += Time.deltaTime;
            if (currInvinTime > InvincibilityTime)
            {
                isInvic = false;
            }
        }


        //攻撃コンボ更新
        if (comboStep > 0)
        {
            UpdateCombo();
        }

        //カメラの角度を確認
        if (cameraTransform == null) return;
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        // 移動方向を計算
        Vector3 horizontalMove = (forward * inputMove.y + right * inputMove.x).normalized;
        // 重力計算
        float yDirection = moveDirection.y - gravity * Time.deltaTime;
        Vector3 gra = new Vector3(0f, yDirection, 0f);
        controller.Move(gra * Time.deltaTime);

        // 移動方向と速度
        moveDirection = horizontalMove;
        float speed_ = inputMove.magnitude * currMoveSpeed;




        if (horizontalMove.magnitude > 0.1f && IsRotatable())
        {
            //回転
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
        // 攻撃入力停止すると自動的にコンボ記数リセット
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
        //コンボ記数リセット
        comboStep = 0;
        comboTimer = 0;
        animator.SetInteger("ComboStep", comboStep);
    }
    // ===== 入力管理 =====

    private void AddToInputQueue(Action key)//入力バッファに入力情報入れる
    {
        inputQueue.Enqueue((key, Time.time + bufferTime));
    }
    private void CheckInputQueue()//入力バッファの管理
    {
        if (inputQueue.Count <= 0) return;
        while (inputQueue.Peek().Item2 < Time.time)//期限切れた入力を捨てる
        {
            inputQueue.Dequeue();
            if (inputQueue.Count == 0) return;
        }

        if (inputQueue.Count <= 0) return;
        if (CanPerformNextAction(inputQueue.Peek().Item1))//期限切れない入力を確認
        {
            Action nextInput = inputQueue.Dequeue().Item1;
            PerformAction(nextInput);
        }
    }
    private bool CanPerformNextAction(Action key)//アニメーション実行可能性確認
    {
        if (key == Action.Dash) //ダッシュ割り込み
        {
            if (!(IsInImpactState()||IsInHookState()))
                return true;
        }
        return isAnimeOver;
    }
    private void PerformAction(Action input)//アクション管理
    {

        switch (input)
        {

            case Action.Attack1:

                if (isAttack1Acceptable)
                {
                    isAttack1Acceptable = false; //次の攻撃アニメ流す前にコンボ入力不可
                    Debug.Log("attack1");
                    comboStep++;
                    comboStep %= (maxComboStep + 1); //連続コンボのため、循環にする
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

    // ===== アニメーションイベント =====

    public void OnRunningAnime()
    {
        // 武器の表示位置と入力を整う
        isAttack1Acceptable = true;
        isAttack2Acceptable = true;
        isHookAcceptable = true;
        UnAttackWeaponDisplay();
        UnableAllHitBox();
        
    }
    public void OnIdleAnime()
    {
        // 武器の表示位置と入力を整う
        isAttack1Acceptable = true;
        isAttack2Acceptable = true;
        isHookAcceptable = true;
        UnAttackWeaponDisplay() ;

        UnableAllHitBox();
        
    }
    public void OnSwordAttack01Enter()
    {
        // 武器の攻撃の表示位置とコンボ入力
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
        // 次のコンボ入力可能
        isAttack1Acceptable = true;
        animator.ResetTrigger("Shoot");//遠距離攻撃リセット
    }
    public void OnSwordAttack01Exit()
    {
        // Hitbox非表示
        swordAttack01Hitbox.SetActive(false);
        SetShortRotateBool(0.28f); //短い間に回転可能
        
    }
    public void OnSwordAttack02Enter()
    {
        // 武器の攻撃の表示位置とHitboxとコンボ入力
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
        // 次のコンボ入力可能
        isAttack1Acceptable = true;
        animator.ResetTrigger("Shoot");//遠距離攻撃リセット
    }
    public void OnSwordAttack02Update2()
    {
        // 二段コンボので、二つ目のHitbox表示
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
        // Hitbox非表示
        swordAttack02Hitbox2.SetActive(false);
        SetShortRotateBool(); //短い間に回転可能
        
    }
    public void OnSwordAttack03Enter()
    {
        // 武器の攻撃の表示位置とHitboxとコンボ入力
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
        comboStep = 0; //連続コンボのためにリセット
        animator.SetInteger("ComboStep", comboStep);
        //AdjustYRotationRelativeToParent(charaTrans, -75.012f);
        AdjustRotationToNearestEnemy(120f);
    }
    public void OnSwordAttack03Update()
    {
        // 次のコンボ入力可能
        isAttack1Acceptable = true;
        animator.ResetTrigger("Shoot"); //遠距離攻撃リセット
    }
    public void OnSwordAttack03Exit()
    {
        // Hitbox非表示
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
        //割り込み可能ので、すべてのコントロール制御と表示をリセット
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

        //ダッシュは割り込み可能ので、すべてのコントロール制御と表示をリセット
        UnAttackWeaponDisplay();
        UnableAllHitBox();
        ResetAttack1Combo();
        animator.ResetTrigger("Shoot");
        gameObject.layer = dashingLayer;//敵との衝撃禁止

        dashEffect.SetActive(true);　//ダッシュのエフェクト
        SetShortRotateBool(0.08f);　//回転可能
        StartCoroutine(Dashing());　//コルーチンで移動
        Invoke(nameof(ResetDashingEvent), dashTimeMax);　//
    }
    private void ResetDashingEvent()
    {
        dashEffect.SetActive(false);
        isDashingEventTriggered = false;
        isAttack1Acceptable = true;
        isAttack2Acceptable = true;
        isHookAcceptable = true;
        gameObject.layer = defaultLayer;//敵との衝撃戻る
    }
    private IEnumerator Dashing()//コルーチンでダッシュ処理
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
        // 短い時間内で回転できる
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
        //すべてのHitbox非表示
        swordAttack01Hitbox.SetActive(false);
        swordAttack02Hitbox1.SetActive(false);
        swordAttack02Hitbox2.SetActive(false);
        swordAttack03Hitbox.SetActive(false);
    }
    private void UnAttackWeaponDisplay()
    {
        unAttackSword.SetActive(true);
        onAttackSword.SetActive(false);
        //モデリングなしで、未実装:
        //Gun
        //HookShoot
    }
    private void OnAttackWeaponDisplay()
    {
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(true);
        //モデリングなしで、未実装:
        //Gun
        //HookShoot
    }
    private void OnShootWeaponDisplay()
    {
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(false);
        //モデリングなしで、未実装:
        //Gun
        //HookShoot
    }
    private void OnHookWeaponDisplay()
    {
        unAttackSword.SetActive(false);
        onAttackSword.SetActive(false);
        //モデリングなしで、未実装:
        //Gun
        //HookShoot
    }
    private void AdjustRotationToNearestEnemy(float plusAngle = 0f)
    {
        Vector3 forward = transform.forward; // 現在の前方向ベクトル
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionDistance); // 検出範囲内のコライダーを取得
        Transform targetEnemy = null; // ターゲットとなる敵
        float effectiveAngle = detectionAngle + plusAngle; // 有効な検出角度
        
        float nearestValue = float.MaxValue; // 優先順位値（角度または距離）

        if (plusAngle == 0f)
        {
            // plusAngleが0の場合：回転角度が最小の敵を探す
            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Enemy") && !hit.GetComponent<IOnHit>().IsDying()) // "Enemy"タグかつ非死亡状態を確認
                {
                    Vector3 directionToEnemy = (hit.transform.position - transform.position).normalized; // 敵への方向ベクトルを計算
                    float angle = Vector3.Angle(forward, directionToEnemy); // 敵と前方向との角度を計算

                    if (angle <= detectionAngle && angle < nearestValue)
                    {
                        targetEnemy = hit.transform;
                        nearestValue = angle; // 最小角度を更新
                    }
                }
            }
        }
        else
        {
            // plusAngleが0以外の場合：最も近い敵を探す
            Transform nearestEnemy = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Enemy") && !hit.GetComponent<IOnHit>().IsDying()) // "Enemy"タグかつ非死亡状態を確認
                {
                    Vector3 directionToEnemy = (hit.transform.position - transform.position).normalized; // 敵への方向ベクトルを計算
                    float angle = Vector3.Angle(forward, directionToEnemy); // 敵と前方向との角度を計算

                    if (angle <= effectiveAngle) // 検出可能な角度内
                    {
                        float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position); // 敵との距離を計算
                        if (distanceToEnemy < nearestDistance)
                        {
                            nearestEnemy = hit.transform;
                            nearestDistance = distanceToEnemy; // 最短距離を更新
                        }
                    }
                }
            }

            // 距離差が1未満の敵の中で角度が最小の敵を探す
            if (nearestEnemy != null)
            {
                targetEnemy = nearestEnemy; // 初期値として最も近い敵を設定
                float smallestAngle = Vector3.Angle(forward, (nearestEnemy.position - transform.position).normalized); // 初期角度

                foreach (Collider hit in hitColliders)
                {
                    if (hit.CompareTag("Enemy") && !hit.GetComponent<IOnHit>().IsDying()) // "Enemy"タグかつ非死亡状態を確認
                    {
                        Vector3 directionToEnemy = (hit.transform.position - transform.position).normalized; // 敵への方向ベクトルを計算
                        float angle = Vector3.Angle(forward, directionToEnemy); // 敵と前方向との角度を計算

                        if (angle <= effectiveAngle) // 検出可能な角度内
                        {
                            float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position); // 敵との距離を計算
                            if (Mathf.Abs(distanceToEnemy - nearestDistance) < deltaDistance) // 距離差がdeltaDistance未満
                            {
                                if (angle < smallestAngle)
                                {
                                    targetEnemy = hit.transform; // 角度が最小の敵を更新
                                    smallestAngle = angle;
                                }
                            }
                        }
                    }
                }
            }
        }

        // 最終的なターゲットに回転
        if (targetEnemy != null)
        {
            Debug.Log("ターゲットに回転中: " + targetEnemy.name);

            // ターゲット方向を計算
            Vector3 targetDirection = (targetEnemy.position - transform.position).normalized;
            targetDirection.y = 0; // Y軸の高さの影響を無視

            // 回転を設定
            transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            // キャラクターのローカル回転をリセット
            if (plusAngle != 0f)
            {
                charaTrans.localRotation = Quaternion.identity;
            }
        }
        else
        {
            Debug.Log("回転対象の敵が見つかりませんでした。");
        }
    }


    void AdjustYRotationRelativeToParent(Transform target, float targetYRotation)
    {
        // 親オブジェクトが存在するか確認
        if (target.parent == null)
        {
            Debug.LogError("指定されたオブジェクトに親オブジェクトがありません！");
            return;
        }

        // 現在のローカル回転を取得
        Quaternion currentLocalRotation = target.localRotation;

        // 新しいローカル回転を計算（X軸とZ軸は変更せず、Y軸のみ設定）
        Quaternion newLocalRotation = Quaternion.Euler(
            currentLocalRotation.eulerAngles.x,
            targetYRotation,
            currentLocalRotation.eulerAngles.z
        );

        // 計算したローカル回転を適用
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

    // ===== アニメーション状態 =====
    private bool IsInAttack1State()
    {   //近接攻撃
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return commonAttack.Contains(currentAnimationHash);
    }
    private bool IsInAttack2State()
    {　 // 遠距離攻撃
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return shootAttack.Contains(currentAnimationHash);
    }
    private bool IsInHookState() 
    {
        // ホックショット
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return hookAttack.Contains(currentAnimationHash);
    }
    private bool IsInImpactState()
    {   // 被弾
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return impactAct.Contains(currentAnimationHash);
    }
    private bool IsInDashState()
    {   // ダッシュ
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return dashAct.Contains(currentAnimationHash);
    }
    private bool IsInDyingState() 
    {   // 死亡
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = currentState.shortNameHash;

        return dyingAct.Contains(currentAnimationHash);
    }
    private bool IsRotatable()
    {
        //ダッシュ、普通攻撃、被弾の時回転不可
        bool rotAnime = !(IsInDashState() || IsInAttack1State() || IsInImpactState() || IsInAttack2State()|| IsInDyingState()|| IsInHookState());
        return rotAnime || isShortRotatable;
    }

    // ===== ImputSystem =====
    void OnMove(InputValue value)//移動入力
    {
        Debug.Log("onMove:" + value.ToString());
        inputMove = value.Get<Vector2>();
    }
    void OnDash()//回避・ダッシュ
    {
        Debug.Log("DashKeyDown:" + Time.time);
        AddToInputQueue(Action.Dash);
    }
    void OnAttack1()//攻撃１
    {
        Debug.Log("Attack1KeyDown:" + Time.time);
        AddToInputQueue(Action.Attack1);
    }
    void OnAttack2()//攻撃２
    {
        Debug.Log("Attack2KeyDown:" + Time.time);
        AddToInputQueue(Action.Attack2);
    }
    void OnHook()
    {
        Debug.Log("HookShootKeyDown:" + Time.time);
        AddToInputQueue(Action.Hook);
    }
    void OnAim(InputValue value)//照準
    {
        Debug.Log("Aim");
        
        if (cameraTransform == null) return;

        Vector2 forw = value.Get<Vector2>();
        //　カメラよる照準方向を計算
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

    //  ===== インタラクション =====
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
            if (distance < 2) elapsedTime += 0.6f; //距離が近いなら早く終わらせる

            while (elapsedTime < pullDuration)
            {
                if (pullTarget == null) break;

                elapsedTime += Time.deltaTime;

                //プレイヤーの方向計算
                Vector3 targetPosition = pullTarget.transform.position;
                targetPosition.y = transform.position.y;

                //移動距離計算
                Vector3 newPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / pullDuration);
                Vector3 displacement = newPosition - transform.position;

                //回転
                Vector3 dir = targetPosition - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                
                //移動
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
            //回避エフェクト
            SpawnEvasionEffect();
            return;
        }
        if (IsInDyingState()||IsInImpactState()||isInvic)
        {
            return;
        }
        // 被弾
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
        //死亡
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
