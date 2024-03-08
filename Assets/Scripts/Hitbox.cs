using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Transform origin;
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
        if(other.gameObject.tag == "Enemy")
        {
            //Getting a reference to the enemy that we collided with
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            Vector3 destination = enemy.transform.position;

            Vector3 direction = destination - origin.position;
            enemy.GetKnockedBack(direction, 25);
            enemy.TakeDamage(10);
        }
    }



}
