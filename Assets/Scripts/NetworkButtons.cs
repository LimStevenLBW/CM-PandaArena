using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetworkButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void JoinGame(){
        NetworkManager.Singleton.StartClient();
        Destroy(gameObject);
    }
    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
        Destroy(gameObject);
    }
}
