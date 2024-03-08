using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    public EnemyController enemyPrefab;
    public Transform spawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && IsServer)
        {
            EnemyController enemy = Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn();
        }
    }
}
