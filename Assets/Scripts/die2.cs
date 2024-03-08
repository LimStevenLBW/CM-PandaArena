using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class die2 : MonoBehaviour
{
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
        if (other.gameObject.tag == "Player")
        {
           Vector3 spawnPoint = new Vector3(0, 1, 0);
            other.gameObject.transform.position = spawnPoint;
        }
    }
}
