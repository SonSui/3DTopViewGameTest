using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HookMove : MonoBehaviour
{
    public GameObject linePos;�@//��̒[�P
    public GameObject hookShooter; //��̒[�Q

    public LineRenderer lineRenderer;
    public GameObject trailRenderer;
    public List<string> targetTags;
    public GameObject hitParticleEffect;

    public float flyTime = 1.0f;
    public float flySpeed = 60f;
    public float forcePower = 3000f;

    private Collider colli;
    private PlayerControl player;
    private Rigidbody rb;
    private float adjustTime = 1f;
    private float time = 0f;
    private float distoryCountdown = 999f;
    private int atk = 0;
    
    private bool isPulling = false;

    enum Status //�t�b�N�̏��
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
        if (status != Status.Flying) return; //��ԈȊO�̏�Ԕ��肵�Ȃ�

        if(targetTags.Contains(collision.gameObject.tag))
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            status = Status.Pull;
            transform.position = collision.contacts[0].point;
            transform.SetParent(collision.transform);

            if(collision.gameObject.GetComponent<IOnHit>()!=null)
            {
                collision.gameObject.GetComponent<IOnHit>().OnHit(atk);
                // �ڂ���ʒu
                Vector3 contactPoint = collision.contacts[0].point;

                // �G�t�F�N�g����
                GameObject effect = Instantiate(hitParticleEffect, contactPoint, Quaternion.identity);

                // �����I�ɍ폜
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
        }
        
    }
    private void PullPlayer()
    {
        player.PullPlayer(GetLinePos());
    }

}
