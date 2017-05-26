using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamagable
{
    enum State
    {
        ComingForYa, 
        Ragdoll,
        StandinUp,
        Dead
    }

    Animator _animator;
    Rigidbody[] _bodies;
    State _currState = State.ComingForYa;
    Player _player;
    NavMeshAgent _navAgent;
    Collider _collider;
    float _downTimeTimer = 0.0f;
    public Transform Pelvis;
    public int maxHealth = 100;
    int _currentHealth = 100;
    public GameObject _mapPoint;

    public float DownTime = 3.0f;
    public BodyPartDamages bodyDamage;
    [System.Serializable]
    public class BodyPartDamages
    {
        public int Arm = 10, LowerChest = 15, UpperChest = 15, LeftUpperLeg = 10, LeftLowerLeg = 10, RightUpperLeg = 10, RightLowerLeg = 10, Crotch = 50, Head = 100;
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _bodies = GetComponentsInChildren<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _navAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
        ToggleRagdoll(false);
    }

    void ToggleRagdoll( bool toggle )
    {
        _animator.enabled = !toggle;

        foreach ( Rigidbody rb in _bodies )
        {
            rb.isKinematic = !toggle;
        }
    }

    public void TakeHit(Collider col = null)
    {
        if (col != null)
        {
            var damagable = col.gameObject.GetComponent<IDamagable>();
            if (damagable != null)
            {
                switch(col.gameObject.tag)
                {
                    case "Arm":
                        damagable.Damage(bodyDamage.Arm);
                        break;
                    case "Crotch":
                        damagable.Damage(bodyDamage.Crotch);
                        break;
                    case "Head":
                        damagable.Damage(bodyDamage.Head);
                        break;
                    case "LeftUpperLeg":
                        damagable.Damage(bodyDamage.LeftUpperLeg);
                        break;
                    case "LeftLowerLeg":
                        damagable.Damage(bodyDamage.LeftLowerLeg);
                        break;
                    case "RightLowerLeg":
                        damagable.Damage(bodyDamage.RightLowerLeg);
                        break;
                    case "RightUpperLeg":
                        damagable.Damage(bodyDamage.RightUpperLeg);
                        break;
                    case "UpperChest":
                        damagable.Damage(bodyDamage.UpperChest);
                        break;
                    case "LowerChest":
                        damagable.Damage(bodyDamage.LowerChest);
                        break;
                    default:
                        Debug.LogError(col.gameObject.name + " doesn't have a damage value assigned");
                        break;
                }
            }
        }

        if ( _currState == State.ComingForYa )
        {
            _currState = State.Ragdoll;
            _animator.SetTrigger("Ragdoll");
            ToggleRagdoll( true );
            _downTimeTimer = 0.0f;
            _navAgent.enabled = false;
        }
    }
    public void Damage(int Damage)
    {
        _currentHealth -= Damage;
        Debug.Log("took " + Damage);
    }

    public void OnIsStanding()
    {
        Debug.Log("Is Standing");
        _currState = State.ComingForYa;
        _navAgent.enabled = true;
    }

    void Update()
    {
        switch( _currState )
        {
            case State.ComingForYa:
                _navAgent.SetDestination( _player.transform.position );
                break;
            case State.Ragdoll:
                _downTimeTimer += Time.deltaTime;
                if( _downTimeTimer >= DownTime )
                {
                    ToggleRagdoll( false );
                    _currState = State.StandinUp;
                    _animator.SetTrigger("StandUp");
                }
                break;
            case State.StandinUp:
                break;
            case State.Dead:
                break;
        }
        if(_currState != State.Dead && _currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        transform.root.GetChild(1).GetComponent<Renderer>().material.color = Color.black;
        Destroy(_mapPoint);
        _currState = State.Dead;
        ScoreManager.Instance.AddScore(1);
        //transform.position += Vector3.forward * 5;
    }
}
