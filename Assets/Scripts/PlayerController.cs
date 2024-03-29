using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using JetBrains.Annotations;

public class PlayerController : NetworkBehaviour
{
    private Vector3 direction;
    [Header("Movement")]
    public float maxStamina;
    private float currentStamina;
    public float speed;
    public float jumpSpeed;
    public float rotationSpeed;

    [Header("References")]
    public Transform orientation;
    public LayerMask groundLayerMask;
    public Hitbox hitbox;
    private Vector3 spawnPoint;

    private NetworkVariable<bool> attackAvailable = new NetworkVariable<bool>(
      true,
      NetworkVariableReadPermission.Everyone,
      NetworkVariableWritePermission.Server
    );

    private float playerHeight;
    private Animator playerAnimator;
    private ClientNetworkAnimator clientAnimator;

    [Header("Sounds")]
    public AudioSource source;
    public AudioClip jumpClip;
    public AudioClip hitClip;

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;

    public Transform cameraTransform;
    private bool isGrounded;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            cameraTransform.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        clientAnimator = GetComponent<ClientNetworkAnimator>();
        cameraTransform = Camera.main.transform;
        playerHeight = 3; //Estimated
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;
        StartCoroutine(SprintingCheck());
    }

    private void Update()
    {
        if (!IsOwner) return;

        //Ground Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);
        //if (isGrounded) rb.drag = 5;
       // else rb.drag = 0;

        //Orientation
        Vector3 viewDir = transform.position - new Vector3(transform.position.x, transform.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;


       // horizontalInput = Input.GetAxis("Horizontal");
        //verticalInput = Input.GetAxis("Vertical");
        //direction = orientation.forward * verticalInput + orientation.right * horizontalInput;

        Jump();
        Move();
        Attack();

        if (direction != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, direction.normalized, Time.deltaTime * rotationSpeed);
    }

    void FixedUpdate()
    {
        //Gravity Boost
        rb.AddForce(Vector3.down * 12);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Debug.Log("Jump");
            source.PlayOneShot(jumpClip);
            //direction = orientation.forward * verticalInput + orientation.right * horizontalInpuoh set;
             rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            //rb.velocity += new Vector3(0, jumpSpeed, 0);
        }

    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && attackAvailable.Value)
        {
            PlayerAttackServerRpc();
        }
    }

    [ServerRpc]
    private void PlayerAttackServerRpc()
    {
        StartCoroutine(HitboxActivity());
        clientAnimator.SetTrigger("attackTrigger");
        //playerAnimator.SetTrigger("attackTrigger");

    }

    IEnumerator HitboxActivity()
    {
        attackAvailable.Value = false;
        yield return new WaitForSeconds(0.3f);
        hitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitbox.gameObject.SetActive(false);

        attackAvailable.Value = true;
    }

    void Move() {
        direction = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey("w"))
        {
            direction += cameraTransform.forward;
            direction.y = 0;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey("s"))
        {
            direction += cameraTransform.forward * -1;
            direction.y = 0;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey("a"))
        {
            direction += cameraTransform.right * -1;
            direction.y = 0;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey("d"))
        {
            direction += cameraTransform.right;
            direction.y = 0;
        }

        if (direction != Vector3.zero) playerAnimator.SetBool("isMoving", true);
        else playerAnimator.SetBool("isMoving", false);
        //   direction = Vector3.Normalize(new Vector3(direction.x, 0, direction.z));

        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    IEnumerator SprintingCheck()
    {
        float defaultSpeed = speed;

        while (true)
        {
            if (Input.GetMouseButton(1) && currentStamina >= 0)
            {
                if(speed <= defaultSpeed * 2) speed += 1;
                currentStamina -= 1;
            }
            else
            {
                currentStamina += 2;
                if(currentStamina >= maxStamina) currentStamina = maxStamina;
                speed = defaultSpeed;
            }
            yield return new WaitForSeconds(0.1f);

            Debug.Log("Stamina: " + currentStamina);
        }
    }

    public bool GetIsMoving()
    {
        if (direction != Vector3.zero) return true;
        return false;
    }

    public bool GetGrounded()
    {
        return isGrounded;
    }

    public void SetSpawnPoint(Vector3 spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<EnemyController>())
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();

            Vector3 directionOfStrike = transform.position - enemy.transform.position;
            rb.AddForce(directionOfStrike * 1000);
            source.PlayOneShot(hitClip);
        }
    }
}
