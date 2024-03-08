using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    public int maxHp;
    private NetworkVariable<int> currentHp = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public float speed;
    public TextMeshPro hpText;
    public Projectile projectilePrefab;
    public int shootingForce;

    private Animator animator;
    private Rigidbody body;
    private GameObject[] targets;
    private PlayerController target; //accessing player info
    private Vector3 direction;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        currentHp.Value = maxHp;

        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        StartCoroutine(UpdateTargets());
        StartCoroutine(FireProjectileCoroutine());

    }

    // Start is called before the first frame update
    void Start()
    {

    }


    IEnumerator UpdateTargets() {
        while (true) {
            FindTargets();
            yield return new WaitForSeconds(2);
        }
    }
    void FindTargets()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
        SetClosestTarget();
    }
    void SetClosestTarget() {
        float smallestDistance = 30;
        for (int i = 0; i < targets.Count(); i++) {
            float distance = Vector3.Distance(transform.position, targets[i].transform.position);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                target = targets[i].GetComponent<PlayerController>();
            }
        }
    }

    public void GetKnockedBack(Vector3 direction, int force)
    {
        body.AddForce(direction.normalized * force, ForceMode.Impulse);
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            currentHp.Value -= damage;

            if (currentHp.Value <= 0) StartCoroutine(EnemyDying());
        }
  

    }

    IEnumerator EnemyDying()
    {
        animator.SetTrigger("isDead");
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }

    void FireProjectile()
    {
        if (projectilePrefab == null) return;
        Projectile projectile = Instantiate(projectilePrefab, transform.forward + transform.position, Quaternion.identity);
        projectile.GetComponent<NetworkObject>().Spawn();
        projectile.Shoot(direction, shootingForce);
    }

    IEnumerator FireProjectileCoroutine()
    {
        while(true)
        {
            if (target != null) FireProjectile();
            yield return new WaitForSeconds(2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        hpText.SetText(currentHp.Value + "/" + maxHp);

        if (!IsServer) return;

        if (target!=null) {
            animator.SetBool("isWalking", true);
            direction = target.transform.position - transform.position;
            direction.y = 0;
            transform.position += direction.normalized*speed*Time.deltaTime;


            if (direction != Vector3.zero)
                transform.forward = Vector3.Slerp(transform.forward, direction.normalized, Time.deltaTime * 10);

        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
