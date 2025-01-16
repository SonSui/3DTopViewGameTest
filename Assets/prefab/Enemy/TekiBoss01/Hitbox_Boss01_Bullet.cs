using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Boss01_Bullet : MonoBehaviour
{
    public float speed = 10f; // 子弾の移動速度
    public float heightAdjustmentSpeed = 8f; // Y軸（高さ）を目標に近づける速度
    private Transform playerT; // プレイヤーのTransformを保持する変数
    public float lifetime = 10f;
    private int dmg = 1;
    public float preAtkTime = 0.1f;
    public float lifeTime = 1f;
    public float currTime = 0f;
    public ParticleSystem particleSystem;

    private bool isHitted = false;

    private void OnEnable()
    {
        Destroy(gameObject, lifetime);


        isHitted = false;
        currTime = 0f;

        
        particleSystem.Clear(); //エフェクトをリセット
        particleSystem.Play();

    }



    // 子弾を初期化するメソッド
    public void Initialize(Transform playerTarget,int dmg_)
    {
        playerT = playerTarget; // プレイヤーのTransformを設定
        this.dmg = dmg_;
    }

    private void Update()
    {
        if (playerT == null)
        {
            playerT = GameObject.FindGameObjectWithTag("Player").transform;
            // 目標が設定されていない場合、直線的に前進
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        // プレイヤーのY軸位置を取得
        float targetY = playerT.position.y+1;

        // 現在の子弾の位置を取得
        Vector3 currentPosition = transform.position;

        // Y軸の高さを目標に向けて平滑に調整
        float newY = Mathf.MoveTowards(currentPosition.y, targetY, heightAdjustmentSpeed * Time.deltaTime);

        // 新しい位置を設定（Y軸を調整した位置）
        transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);

        // 子弾をZ軸方向に移動
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isHitted)
        {
            other.GetComponent<PlayerControl>().OnHit(dmg);
            isHitted = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isHitted)
        {
            other.GetComponent<PlayerControl>().OnHit(dmg);
            isHitted = true;
        }
    }

    public void Initialized(int dmg_)
    {
        dmg = dmg_;
    }
}
