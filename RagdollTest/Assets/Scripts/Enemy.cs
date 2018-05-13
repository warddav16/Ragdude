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
		Taunting,
        ComingForYa, 
        Ragdoll,
        StandinUp,
        Waiting,
        Dead
    }

    public class Bone
    {
        public Transform transform;
        public Vector3 originalPos;
        public Quaternion originalRot;
    }

    Animator _animator;
    Rigidbody[] _bodies;
    State _currState = State.ComingForYa;
    Player _player;
    NavMeshAgent _navAgent;
    Collider _collider;
    float _downTimeTimer = 0.0f;
    public Transform Pelvis;
    public Transform TrueRoot;
    public int maxHealth = 100;
    int _currentHealth = 100;
    public GameObject _mapPoint;
    List<Bone> _boneList = new List<Bone>();
    float _standUpTimer = 0.0f;
    public float RagdollBlendTime = 0.5f;
    public Transform HipJoint;

    public float DownTime = 3.0f;
    public BodyPartDamages bodyDamage;
    public int deathTimeBeforeCleanup = 5;
    float _deathTimer = 0;
	private float _tauntTimer;
	private float animSpeed;
	private float _tauntAnimLength;

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
        _deathTimer = 0;
        ToggleRagdoll(false);

		_navAgent.speed = Random.Range(.5f,3);
		animSpeed = Mathf.Lerp (0, 1, Mathf.InverseLerp (.5f, 3, _navAgent.speed));//map the speed the agent can can move to the mecanim blend tree 0-1
		_animator.SetFloat("Speed",animSpeed);
		if (animSpeed < .2f) {
			_animator.SetFloat("Speed",0);
			_navAgent.speed = 0;
		}


        var boneTransforms = TrueRoot.gameObject.GetComponentsInChildren<Transform>();
        for( int i = 0; i < boneTransforms.Length; ++i )
        {
            Bone b = new Bone();
            b.transform = boneTransforms[i];
            _boneList.Add(b);
        }
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
            _animator.SetTrigger("Ragdoll");
            ToggleRagdoll(true);
            _navAgent.enabled = false; 
            _downTimeTimer = 0.0f;
            _currState = State.Ragdoll;

        }
    }
    public void Damage(int Damage)
    {
        _currentHealth -= Damage;
//        Debug.Log("took " + Damage);
    }

	//fired off via animation event
    public void OnIsStanding()
    {
        Debug.Log("Is Standing");
        _currState = State.ComingForYa;
        _navAgent.enabled = true;
    }

    void Update()
    {

        switch (_currState)
        {
		case State.Taunting:

			_animator.SetTrigger ("Taunt");
			_navAgent.speed = 0;
			_tauntAnimLength += Time.deltaTime;

			if (_tauntAnimLength > _animator.GetCurrentAnimatorStateInfo (0).length) {
				Debug.Log ("ok now");
				_tauntAnimLength = 0;
				_currState = State.ComingForYa;
			}
		
			break;
		case State.ComingForYa:
			_navAgent.speed = animSpeed;
			_navAgent.SetDestination (_player.transform.position);				
				//set random taunt anim
			_tauntTimer += Time.deltaTime;
			if (_tauntTimer >= 4) {
				_tauntTimer = 0;

				_currState = State.Taunting;
			}
                break;
            case State.Ragdoll:
                _downTimeTimer += Time.deltaTime;
                if (_downTimeTimer >= DownTime)
                {
                    for (int boneIdx = 0; boneIdx < _boneList.Count; ++boneIdx)
                    {
                        _boneList[boneIdx].originalPos = _boneList[boneIdx].transform.position;
                        _boneList[boneIdx].originalRot = _boneList[boneIdx].transform.rotation;
                    }
                    Vector3 curr = transform.position;
                    transform.MoveOnlyParent(TrueRoot.position);
                    transform.position = new Vector3(transform.position.x, curr.y, transform.position.z);
                    _currState = State.StandinUp;
                    ToggleRagdoll(false);
                    _standUpTimer = 0.0f;
                    _animator.SetTrigger("StandUp");
                }
                break;
            case State.StandinUp:
				//see late update
                break;
            case State.Dead:
                _deathTimer += Time.deltaTime;
                if (_deathTimer > deathTimeBeforeCleanup)
                {
                    Destroy(gameObject);
                }
                break;
        }
        if (_currState != State.Dead && _currentHealth <= 0)
        {
			Debug.Log ("dead");
            _currState = State.Dead;
            _navAgent.enabled = false;
             Die();
        }
    }

    void LateUpdate()
    {
        if( _currState == State.StandinUp )
        {
            _standUpTimer += Time.deltaTime;
            float T = Mathf.Min(_standUpTimer / RagdollBlendTime, 1.0f);
            if (T <= 1.0f)
            {
                foreach (var bone in _boneList)
                {
                    if (bone.transform == HipJoint)
                        bone.transform.position = Vector3.Lerp(bone.originalPos, bone.transform.position, T);
                    bone.transform.rotation = Quaternion.Slerp(bone.originalRot, bone.transform.rotation, T);
                }
            }
        }
    }

    void Die()
    {
       // transform.root.GetChild(1).GetComponent<Renderer>().material.color = Color.black;
        Destroy(_mapPoint);
        _currState = State.Dead;
        ScoreManager.Instance.AddScore(1);
        //transform.position += Vector3.forward * 5;
    }
}
