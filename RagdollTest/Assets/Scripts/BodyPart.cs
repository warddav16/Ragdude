using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour, IDamagable {

    //Root object must be damagable
    IDamagable root;
    public int MaxHealth = 20;
    int _health;
    void Start()
    {
        root = transform.root.GetComponent<IDamagable>();
        _health = MaxHealth;
    }
    public void Damage(int Damage)
    {
        Debug.Log(tag + " took " + Damage);
        _health -= Damage;
        root.Damage(Damage);
        /* If there was renderers on each piece
        if (_health < .5 * MaxHealth && _health > .8 * MaxHealth)
        {
            gameObject.GetComponentInParent<Renderer>().material.color = Color.yellow;
        }
        else if(_health <.2 * MaxHealth)
        {
            gameObject.GetComponentInParent<Renderer>().material.color = Color.red;
        }
        */
    }
}
