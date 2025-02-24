using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static StageManager Instance { get; private set; }
    
    UIManager uiManager;
    private bool isDead= false;
    private bool isClear = false;

    public bool isStageDifficult = false;
    private GameObject dropPrefab;
    private GameObject drop = null;
    private bool isClearUI = false;

    private static int stageNum = 0;
    public int maxStage = 3;
    public bool isTutorial = false;
    public bool isBoss = false;
    public int RecoverHP = 1;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ç∑Ç≈Ç…ë∂ç›Ç∑ÇÈèÍçáÇÕçÌèúÇ∑ÇÈ
        }
    }

    void Start()
    {
        
        uiManager = UIManager.Instance;
        
        if(isStageDifficult)
        {
            dropPrefab = GameManager.Instance.GetRareDrop();
        }
        else if(isTutorial)
        {

        }
        else if(isBoss)
        {

        }
        else
        {
            dropPrefab = GameManager.Instance.GetCommonDrop();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPlayerDead())
        {
            if (!isDead)
            {
                isDead = true;
                uiManager.FailUI();
            }
        }
        if (drop==null&&isClear&&!isClearUI&&!isBoss)
        {
            isClearUI = true;
            StartCoroutine(EnableClearUIAfterDelay());
        }
    }

    public void StageClear()
    {
        if (!isClear)
        {
            isClear = true;
            if(isBoss)BossClear();
            else if (drop == null && dropPrefab != null)
            {
                drop = Instantiate(dropPrefab);
                GameManager.Instance.RecoverHP(RecoverHP);
            }
        }
    }
    public void BossClear()
    {
        StartCoroutine(EnableClearUIAfterDelay());
        
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    private IEnumerator EnableClearUIAfterDelay(float t = 3f)
    {
        Debug.Log("EnableClearUI");
        yield return new WaitForSeconds(t);
        if (isBoss)
        {
            //uiManager.EndingUI();
            SceneManager.LoadScene("Ending");
            Debug.Log("Ending");
        }
        else
        {
            if (stageNum < maxStage) uiManager.ContinueUI1();
            else uiManager.ContinueToBoss();
        }
    }
    public void SetStageNum(int n)
    {
        stageNum = n;
    }
    public void SpawnSuperRareDrop()
    {
        Instantiate(GameManager.Instance?.GetSuperRareItem());
    }
}
