using System.Collections;
using System.Collections.Generic;
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
    GameObject dropPrefab;
    GameObject drop = null;
    bool isClearUI = false;
    

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
        if (drop==null&&isClear&&!isClearUI)
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
            if (drop == null && dropPrefab != null)
            {
                drop = Instantiate(dropPrefab);
            }
        }
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
        yield return new WaitForSeconds(t);
        uiManager.ContinueUI1();
    }


}
