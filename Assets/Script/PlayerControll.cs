using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour {

    float jump = 7f;
    float speed = 3f;
    //위는 스탯이될수있음
    Rigidbody rb;
    private Vector3 moveDiection = Vector3.zero;
    public bool isJump = false;

    public Transform gun;
    // Use this for initialization
    public void aiming(Vector2 lookPoint)
    {
        float angle = Mathf.Atan2(transform.position.x - lookPoint.x, transform.position.y - lookPoint.y) * Mathf.Rad2Deg;
        gun.localRotation = Quaternion.Euler(0, 0, -(angle + 90f));
    }


    void Start () {
        rb = GetComponent<Rigidbody>();
        
	}
    // Update is called once per frame
    void FixedUpdate () {
        if ((Input.GetKeyDown(KeyCode.Space)) && isJump == false)
        {
            rb.AddForce(new Vector3(0,jump,0),ForceMode.Impulse);//컨트롤러의 이동으로 이동해야 best
            isJump = true;
        }
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(new Vector3(x, 0));
        GameManager.inst.GetComponent<FieldManager>().distance = transform.position.x;
//        rb.AddForce(new Vector3(x, 0));

        
    }
}
