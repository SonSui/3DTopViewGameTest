using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //OnButtonTest();
    }

    public void OnButtonTest()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //SceneManager.LoadScene("TestScene2");
        //}
        //else if (Input.GetMouseButtonDown(1))
        //{
        SceneManager.LoadScene("SampleScene");
        //}
    }
    public void OnButtonTest2()
    {
        SceneManager.LoadScene("TestScene2");
    }

}
