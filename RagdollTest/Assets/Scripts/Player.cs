using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Weapon))]
public class Player : MonoBehaviour
{
    Camera _cam;
    public int movementSpeed = 2, rotationSpeed = 2;
    public float phoneSpeed = .25f;
    public Weapon _weapon;
	GameObject weapon3D;

    void Awake()
    {
        _cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        //transform.Translate(0, 0, Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime);
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, Input.GetAxis("Horizontal"), 0), Time.deltaTime * rotationSpeed));
        if (Input.touchCount > 0)
        {
            transform.Rotate(0, Input.touches[0].deltaPosition.x * phoneSpeed * Time.deltaTime, 0);

        }
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.transform.root.tag == "enemy")
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
    void Shoot()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit outInfo;
        if (Physics.Raycast(ray, out outInfo))
        {
            //if outInfo.distance
            Drop drop;
            if (drop = outInfo.collider.transform.root.GetComponent<Drop>())
            {
                //Set Weapon
				Destroy(weapon3D);
                _weapon = drop.weapon;
				weapon3D = Instantiate( _weapon.weapon3D);
				weapon3D.transform.parent = this.transform;
				weapon3D.transform.localPosition = new Vector3 (0, -.4f, .7f);
				weapon3D.transform.rotation = this.transform.rotation;
				//best to have these all preloaded under player and just toggle them on?

                Destroy(outInfo.collider.gameObject);
            }
            else
            {
                Enemy enemy = outInfo.collider.transform.root.GetComponent<Enemy>();
                if (enemy && outInfo.distance <= _weapon.Range)
                {
                    _weapon.Fire();
                    enemy.TakeHit(outInfo.collider);
                    Vector3 knockbackDir = (outInfo.point - _cam.transform.position).normalized;
                    outInfo.rigidbody.AddForce(knockbackDir * _weapon.Force, ForceMode.Impulse);
                }
            }
        }
    }
}
