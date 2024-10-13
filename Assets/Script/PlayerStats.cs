using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int crit;      
    public int health;    
    public float speed;   
    

    private void Awake()
    {
        Instance = this;
        ResetStats();
    }

    public void ResetStats()
    {
        
        crit = 0;
        health = 100;
        speed = 5f;
        
    }

    public void EnableExplosionEffect()
    {
        
    }
}