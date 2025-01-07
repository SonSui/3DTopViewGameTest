using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    private GameObject parentObject;
    void Start()
    {
        parentObject = transform.parent.gameObject;
    }

    public void OnIdleAnime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnIdleAnime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnRunningAnime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnRunningAnime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack01Enter()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack01Enter", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack01Update1()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack01Update1", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack01Update2()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack01Update2", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack01Exit()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack01Exit", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack02Enter()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack02Enter", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack02Update1()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack02Update1", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack02Update2()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack02Update2", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordArrack02Exit()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordArrack02Exit", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack03Enter()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack03Enter", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack03Update()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack03Updat", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnSwordAttack03Exit()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnSwordAttack03Exit", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnShootEnter()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnShootEnter", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnShoot()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnShoot", SendMessageOptions.DontRequireReceiver);
        }

    }
    public void OnHookEnter()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnHookEnter", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnHookShoot()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnHookShoot", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnImpact()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnImpact", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void StartDashing()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("StartDashing", SendMessageOptions.DontRequireReceiver);
        }
    }
<<<<<<< HEAD
    public void OnDeadAnimation()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnDeadAnimation", SendMessageOptions.DontRequireReceiver);
        }
    }
=======
>>>>>>> origin/main
}
