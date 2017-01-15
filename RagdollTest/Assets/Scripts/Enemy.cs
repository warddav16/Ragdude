using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    enum State
    {
        ComingForYa, 
        Ragdoll,
        StandinUp
    }

    Animator _animator;
    Rigidbody[] _bodies;
    State _currState = State.ComingForYa;
    Player _player;
    NavMeshAgent _navAgent;
    Collider _collider;
    float _downTimeTimer = 0.0f;


    public float DownTime = 3.0f;

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

    public void TakeHit()
    {
        if ( _currState == State.ComingForYa )
        {
            _currState = State.Ragdoll;
            ToggleRagdoll( true );
            _downTimeTimer = 0.0f;
            _navAgent.enabled = false;
        }
    }

    public void OnIsStanding()
    {
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
                }
                break;
            case State.StandinUp:
                break;
        }
    }
}
