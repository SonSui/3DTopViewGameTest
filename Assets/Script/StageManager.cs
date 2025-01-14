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
    }

    public void StageClear()
    {
        if (!isClear)
        {
            isClear = true;
            uiManager.ContinueUI1();
        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
