using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Camera _cam;
    public float Knockback = 1.0f;
    public int movementSpeed = 2, rotationSpeed = 2;
    public float phoneSpeed = .25f;
    

    void Awake()
    {
        _cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        //transform.Translate(0, 0, Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime);
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0) ;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, Input.GetAxis("Horizontal"), 0), Time.deltaTime * rotationSpeed));
        if (Input.touchCount > 0)
        {
            transform.Rotate(0, Input.touches[0].deltaPosition.x * phoneSpeed * Time.deltaTime, 0);
            
        }
        if ( Input.GetMouseButtonDown(0) )
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit outInfo;
            if( Physics.Raycast(ray, out outInfo) )
            {
                Enemy enemy = outInfo.collider.transform.root.GetComponent<Enemy>();
                if( enemy )
                {
                    enemy.TakeHit(outInfo.collider);
                    Vector3 knockbackDir = ( outInfo.point - _cam.transform.position ).normalized;
                    outInfo.rigidbody.AddForce(knockbackDir * Knockback, ForceMode.Impulse);
                }
            }
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if(col.transform.root.tag == "enemy")
        {
            Die();
        }
    }
     void Die()
    {
        Time.timeScale = 0;
        ScoreManager.Instance.SaveScore();
        GameObject.FindGameObjectWithTag("GameOver").transform.GetChild(0).gameObject.SetActive(true);
    }
}
