using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NejikoController : MonoBehaviour
{
    CharacterController controller;
    Animator animator;

    Vector3 moveDirection = Vector3.zero;
    Vector3 charaRotationOri = Vector3.zero;

    public float gravity=20f;
    public float speedZ=5f;
    public float speedJump=8f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        charaRotationOri = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            if (Input.GetAxis("Vertical") > 0.0f)
            {
                moveDirection.z = Input.GetAxis("Vertical") * speedZ;
                transform.eulerAngles = new Vector3(charaRotationOri.x, charaRotationOri.y, charaRotationOri.z);

            }
            else if(Input.GetAxis("Vertical") < 0.0f)
            {
                moveDirection.z = Input.GetAxis("Vertical") * -speedZ;
                float newAngle = charaRotationOri.y + 180f;
                transform.eulerAngles = new Vector3(charaRotationOri.x, newAngle, charaRotationOri.z);
            }
            else
            {
                moveDirection.z = 0.0f;
            }
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = speedJump;
                animator.SetTrigger("jump");
            }
        }
            moveDirection.y-=gravity*Time.deltaTime;

            Vector3 globalDirection = transform.TransformDirection(moveDirection);
            controller.Move(globalDirection*Time.deltaTime);

            if(controller.isGrounded) { moveDirection.y = 0; }

            animator.SetBool("run", moveDirection.z > 0.0f);
        
    }
}
