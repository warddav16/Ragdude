using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class RandomRange
    {
        public int Min = 1;
        public int Max = 5;
    }
    public int radius = 10;
    public int spawnOnStart = 10;
    public int waitBetweenEnemySpawn = 3, waitBetweenDropSpawn = 30;

    public RandomRange RandomSpawnAmount;
    public GameObject[] weapons;

    public GameObject toSpawn;
    GameObject _player;
    private Coroutine _spawnCoroutine;
    bool isSpawning = true;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
       // StartCoroutine(RepeatingSpawn());
       // StartCoroutine(Drops());
		StartCoroutine(SpawnInView());

    }
    IEnumerator RepeatingSpawn()
    {
        while (isSpawning)
        {
            Spawn(Random.Range(RandomSpawnAmount.Min, RandomSpawnAmount.Max), toSpawn);
            yield return new WaitForSeconds(waitBetweenEnemySpawn);
        }
    }
    IEnumerator Drops()
    {
        var toSpawn = weapons[Random.Range(0, weapons.Length)];
        Spawn(1, toSpawn);
        yield return new WaitForSeconds(waitBetweenDropSpawn);
    }
    void Spawn(int spawnNum, GameObject toSpawn)
    {
        for (int i = 0; i < spawnNum; i++)
        {
            var j = Random.Range(0, 360);
            Instantiate<GameObject>(toSpawn, new Vector3(_player.transform.position.x + (Mathf.Cos(j * Mathf.Deg2Rad) * radius), _player.transform.position.y, transform.position.z + radius * Mathf.Sin(j * Mathf.Deg2Rad)), Quaternion.identity);
        }
        
    }
    void StopSpawn()
    {
        StopCoroutine(_spawnCoroutine);
    }

	IEnumerator SpawnInView(){

		while (isSpawning) {


			GameObject enemy = Instantiate (toSpawn, FindNewSpawnPoint (), _player.transform.rotation);

			if (Physics.CheckSphere(enemy.transform.position, 2))
			{
				enemy.transform.position = FindNewSpawnPoint ();
			}

			yield return new WaitForSeconds (waitBetweenEnemySpawn);
		}
	}
	Vector3 FindNewSpawnPoint(){
		Vector3 randomVector = _player.transform.position + new Vector3 (Random.Range (-10, 10), 0, 0);
		Vector3 position = _player.transform.position + randomVector + _player.transform.forward * radius;
		return position;
	}
}
