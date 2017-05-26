using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    // Use this for initialization
    public int radius = 10;
    public int spawnOnStart = 10;
    public GameObject toSpawn;
    GameObject _player;
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        for (int i = 0; i < spawnOnStart; i++)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        var j = Random.Range(0, 360);
        Instantiate<GameObject>(toSpawn, new Vector3(_player.transform.position.x + (Mathf.Cos(j * Mathf.Deg2Rad) * radius), _player.transform.position.y, transform.position.z + radius * Mathf.Sin(j * Mathf.Deg2Rad)), Quaternion.identity);
    }
}
