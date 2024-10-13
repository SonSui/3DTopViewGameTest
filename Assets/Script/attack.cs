using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour
{
    private Animator animator;


    public Material critMaterial;
    public Material genMaterial;
    public float currTime;
    public const float TIME_MAX = 0.2f;
    private bool isCritical;
    private bool isExpl;
    private int damage;
    public GameObject expl;
    


    //-----------巻き戻す--------------
    private int bufferSize = 180;
    private int recordInterval = 2;

    private CircularBuffer<TimeSnapShot> snapshots;
    private int frameCounter = 0;
    private bool isRewinding = false;


    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        snapshots = new CircularBuffer<TimeSnapShot>(bufferSize);
        
        currTime = 0f;

        
    }

    // Update is called once per frame
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
            currTime += Time.deltaTime;
            if(currTime > TIME_MAX) 
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Box:crit:" + isCritical.ToString() + " expl:" + isExpl.ToString());
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyA1>().OnHit();
            if(isCritical&&isExpl)
            {
                Debug.Log("Explo");
                Instantiate(expl, transform.position, Quaternion.identity);
            }
        }
    }

    public void Initialize(int dmg, bool isCri = false,bool isExp=false)
    {
        damage = dmg;
        isCritical = isCri;
        isExpl = isExp;
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            if (isCritical && critMaterial != null)
            {

                renderer.material = critMaterial;
                
            }
            else if (genMaterial != null)
            {
                renderer.material = genMaterial;
            }
        }
    }

    



    void RecordSnapshot()
    {
        //記録
        TimeSnapShot snapshot = new TimeSnapShot(transform, animator, frameCounter);
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
            if(snapshot.frame<=0)
            {
                StopRewind();
                Destroy(gameObject);
            }

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
