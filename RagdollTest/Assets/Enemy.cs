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
        Standin
    }

    Animator _animator;
    Rigidbody[] _bodies;
    State _currState = State.ComingForYa;
    Player _player;
    NavMeshAgent _navAgent;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _bodies = GetComponentsInChildren<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _navAgent = GetComponent<NavMeshAgent>();

        ToggleRagdoll(false);
    }

    void ToggleRagdoll( bool toggle )
    {
        _animator.enabled = !toggle;

        foreach ( Rigidbody rb in _bodies )
        {
            rb.isKinematic = toggle;
        }
    }

    void Update()
    {
    }
}
