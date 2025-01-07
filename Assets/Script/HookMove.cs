using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HookMove : MonoBehaviour
{
    public GameObject linePos;　//縄の端１
    public GameObject hookShooter; //縄の端２

    public LineRenderer lineRenderer;
    public GameObject trailRenderer;
    public List<string> targetTags;
    public GameObject hitParticleEffect;

    public float flyTime = 1.0f;
    public float flySpeed = 60f;
    public float forcePower = 20f;

    public float detectionRange = 30f; // 敵を探す範囲
    public float detectionAngle = 30f;

    private Collider colli;
    private PlayerControl player;
    private Rigidbody rb;
    private float adjustTime = 1f;
    private float time = 0f;
    private float distoryCountdown = 999f;
    private int atk = 0;
    
    private bool isPulling = false;

    enum Status //フックの状態
    {
        Flying,Ground,Pull,Return
    };
    
    Status status;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        colli = GetComponent<Collider>();
        
        rb.AddForce(transform.forward*forcePower);
        status = Status.Flying;
    }



    private void Update()
    {
        if(linePos!=null&&hookShooter!=null)
        {
            lineRenderer.SetPosition(0, hookShooter.transform.position);
            lineRenderer.SetPosition(1, linePos.transform.position);
        }
        if(status== Status.Ground||status == Status.Pull)
        {
            distoryCountdown -= Time.deltaTime * adjustTime;
            if(distoryCountdown<=0) 
            {
                Destroy(gameObject);
            }
        }

        if (status == Status.Flying)
        {
            AdjustDirectionTowardsEnemy();
        }
    }
    
    public void InitHook(PlayerControl player_,GameObject hookShooter_,int atk_)
    {
        
        player = player_;
        hookShooter = hookShooter_;
        atk = atk_;
        
        lineRenderer.SetPosition(0, hookShooter.transform.position);
        lineRenderer.SetPosition(1, linePos.transform.position);
    }
    public GameObject GetLinePos()=>linePos;

    private void OnCollisionEnter(Collision collision)
    {
        if (status != Status.Flying) return; //飛ぶ以外の状態判定しない

        if(targetTags.Contains(collision.gameObject.tag))
        {
            colli.enabled = false;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            status = Status.Pull;
            transform.position = collision.contacts[0].point;
            transform.SetParent(collision.transform);

            if(collision.gameObject.GetComponent<IOnHit>()!=null)
            {
                collision.gameObject.GetComponent<IOnHit>().OnHooked(atk);
                // 接する位置
                Vector3 contactPoint = collision.contacts[0].point;

                // エフェクト生成
                GameObject effect = Instantiate(hitParticleEffect, contactPoint, Quaternion.identity);

                // 自動的に削除
                Destroy(effect, 2f);

                
            }

            PullPlayer();
            distoryCountdown = 2f;
            trailRenderer.SetActive(false);

        }
        if(collision.gameObject.tag=="Ground")
        {
            distoryCountdown = 2f;
            status = Status.Ground;
            player.HookDown();
        }
        
    }
    private void PullPlayer()
    {
        player.PullPlayer(GetLinePos());
    }

    private void AdjustDirectionTowardsEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange); // 周囲の敵を探す
        Transform closestTarget = null;
        float smallestAngle = float.MaxValue; // 初期化最大角度

        foreach (var hit in hits)
        {
            if (targetTags.Contains(hit.gameObject.tag))
            {
                if (hit.gameObject.tag == "Enemy")
                {
                    if (hit.GetComponent<IOnHit>().IsDying()) continue;
                }

                Vector3 directionToTarget = (hit.transform.position - transform.position).normalized; // ターゲットへの方向
                directionToTarget.y = 0; // 水平方向に限定
                directionToTarget.Normalize(); // 正規化

                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget); // 前方との角度

                if (angleToTarget <= detectionAngle / 2 && angleToTarget < smallestAngle)
                {
                    // 更新最小角度とターゲット
                    smallestAngle = angleToTarget;
                    closestTarget = hit.transform;
                }
            }
        }

        // 最も角度の小さいターゲットを追跡
        if (closestTarget != null)
        {
            Vector3 directionToClosest = (closestTarget.position - transform.position).normalized;
            directionToClosest.y = 0; // 水平方向に限定
            directionToClosest.Normalize();

            Vector3 newDirection = Vector3.Lerp(transform.forward, directionToClosest, Time.deltaTime * 5f);
            transform.rotation = Quaternion.LookRotation(newDirection); // 回転を適用
            rb.velocity = newDirection * rb.velocity.magnitude; // 現在の速度を保持
        }
    }

}
