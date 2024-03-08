using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public PlayerController player;
    public AudioClip footstepClip;
    
    // Start is called before the first frame update
    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        StartCoroutine(FootstepsCoroutine());

    }

    IEnumerator FootstepsCoroutine()
    {
        while (true)
        {
            
            if (player.GetGrounded())
            {
                if (player.GetIsMoving()) audioSource.PlayOneShot(footstepClip);

             //   if (player.isWalking) yield return new WaitForSeconds(0.3f);

            }
            yield return new WaitForSeconds(0.3f);
        }
    }

}
