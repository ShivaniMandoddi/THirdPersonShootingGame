using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public float playerSpeed;
    CharacterController character;
    Animator animator;
    public float rotateSpeed;
    void Start()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       float inputX= Input.GetAxis("Horizontal")*playerSpeed;
       float inputZ= Input.GetAxis("Vertical")*playerSpeed;
        Vector3 movement = new Vector3(inputX, 0f, inputZ);
        //character.Move(movement*Time.deltaTime);
        animator.SetFloat("Speed", movement.magnitude);
        /*   if (movement.magnitude > 0f)
           {

               Quaternion tempDirection = Quaternion.LookRotation(movement);
               transform.rotation = Quaternion.Slerp(transform.rotation, tempDirection, Time.deltaTime * rotateSpeed);
           }*/
        transform.Rotate(Vector3.up, inputX * rotateSpeed * Time.deltaTime);
        if(inputZ!=0)
        {
           character.Move(transform.forward * Time.deltaTime*inputZ);
        }
       

    }
}
