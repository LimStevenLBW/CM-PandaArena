using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerController playerPrefab;
    private int numberOfPlayers;


    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += NewPlayerConnected;
    }

    void NewPlayerConnected(ulong id)
    {
        numberOfPlayers += 1;

        if(numberOfPlayers == 1)
        {
            Vector3 spawnPoint1 = new Vector3(1, 1, 0);
            PlayerController player1 = Instantiate(playerPrefab, spawnPoint1, Quaternion.identity);
            player1.SetSpawnPoint(spawnPoint1);
            player1.GetComponent<NetworkObject>().SpawnAsPlayerObject(id);
        }
        else if(numberOfPlayers == 2)
        {
            Vector3 spawnPoint2 = new Vector3(-1, 1, 0);
            PlayerController player2 = Instantiate(playerPrefab, spawnPoint2, Quaternion.identity);
            player2.SetSpawnPoint(spawnPoint2);
            player2.GetComponent<NetworkObject>().SpawnAsPlayerObject(id);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
