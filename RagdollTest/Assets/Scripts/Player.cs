using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Camera _cam;
    public float Knockback = 1.0f;

    void Awake()
    {
        _cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if( Input.GetMouseButtonDown(0) )
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
}
