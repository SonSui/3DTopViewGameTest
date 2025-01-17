using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public float litHight = 3f;
    public float litTime = 4f;
    public ParticleSystem spawnedParticle;
    public Vector3 spawn0ffset = Vector3.zero;
    public AudioSource spawnSound;

    void OnEnable()
    {
        spawnedEnemies.Clear();
        spawnSound.Stop();
    }
    public void SpawnEnemy(GameObject enemy)
    {
        spawnedEnemies.Add(enemy);
        Vector3 spawnPos = transform.position;
        spawnPos.y -= litHight;
        enemy.transform.position = spawnPos+spawn0ffset;
        StartCoroutine(LitUpEnemy(enemy));
        spawnedParticle.Play();
        spawnSound.Play();
    }
    private System.Collections.IEnumerator LitUpEnemy(GameObject enemy)
    {
        Animator[] animators = enemy.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            animator.enabled = false;
        }
        float moveTime = 0f;
        Vector3 startPos = enemy.transform.position; 
        Vector3 targetPos = transform.position+spawn0ffset;

        while (moveTime < litTime && enemy != null)
        {
            moveTime += Time.deltaTime;
            float t = Mathf.Clamp01(moveTime / litTime); 
            enemy.transform.position = Vector3.Lerp(startPos, targetPos, t); 
            yield return null;
        }
        //enemy.transform.position = targetPos;
        if (enemy != null)
        {
            foreach (Animator animator in animators)
            {
                animator.enabled = true;
            }
        }
        spawnSound.Stop();
    }

    private void CleanupNullEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }
    public int GetSpawnedNum()
    {
        CleanupNullEnemies();
        return spawnedEnemies.Count;
    }
}
