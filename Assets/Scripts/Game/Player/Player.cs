using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Mechanics")]
    public int health = 100;
    public float runSpeed = 7.5f;
    public float walkSpeed = 6f;
    public float gravity = 10f;
    public float crouchSpeed = 4f;
    public float jumpHeight = 20f;
    public float interactRange = 10f;
    public float groundRayDistance = 1.1f;

    // Temporarily Public
    public bool isRunning = false;
    public bool isCrouching = false;
    public bool isJumping = false;

    [Header("Weapons")]
    public float switchDelay = 15f; // In Milliseconds

    [Header("References")]
    public CameraLook cameraLook;
    public Camera attachedCamera;
    public Transform hand;

    // Components
    private Animator anim;

    // Movement
    private CharacterController controller;
    private Vector3 movement;

    // Weapons
    public Weapon currentWeapon;
    private List<Weapon> weapons = new List<Weapon>();
    private int currentWeaponIndex = 0;

    private Collider collider;

    void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        collider = GetComponent<Collider>();
        RegisterWeapons();
    }
    void Start()
    {
        SelectWeapon(0);
    }

    void OnDrawGizmosSelected()
    {
        Ray interactRay = attachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(interactRay.origin, interactRay.origin + interactRay.direction * interactRange);

        Ray groundRay = new Ray(transform.position, -transform.up);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundRay.origin, groundRay.origin + groundRay.direction * groundRayDistance);
    }

    private void OnCollisionEnter(Collision other)
    {
        //other.collider.SendMessage("TakeDamage", 10);
        Enemy enemy = other.collider.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.TakeDamage(10);
        }
    }

    #region Initialization
    void RegisterWeapons()
    {
        weapons = new List<Weapon>(GetComponentsInChildren<Weapon>());
    }
    #endregion

    #region Controls
    void Move(float inputH, float inputV)
    {
        Vector3 input = new Vector3(inputH, 0, inputV);
        input = transform.TransformDirection(input);

        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

        movement.x = input.x * currentSpeed;
        movement.z = input.z * currentSpeed;
    }
    #endregion

    #region Combat
    void SwitchWeapon(int direction)
    {
        // Increment index
        currentWeaponIndex += direction;
        // Check if index is below zero
        if (currentWeaponIndex < 0)
        {
            // Loop back to end
            currentWeaponIndex = weapons.Count - 1;
        }
        // Check if index is exceeding length
        if (currentWeaponIndex >= weapons.Count)
        {
            // Reset back to zero
            currentWeaponIndex = 0;
        }
        // Select weapon
        SelectWeapon(currentWeaponIndex);
    }
    void DisableAllWeapons()
    {
        // Loop through all weapons
        foreach (var item in weapons)
        {
            item.gameObject.SetActive(false);
        }
    }
    void SelectWeapon(int index)
    {
        // Is index in range?
        if (index >= 0 && index < weapons.Count)
        {
            // Disable all weapons
            DisableAllWeapons();
            // Select weapon
            currentWeapon = weapons[index];
            // Enable weapon
            currentWeapon.gameObject.SetActive(true);
            // Update index
            currentWeaponIndex = index;
        }
    }
    #endregion

    #region DelayedActions
    IEnumerator SwitchWeapon(float millisecondDelay, int direction)
    {
        float secondDelay = (millisecondDelay / 60f);
        // Wait a few seconds
        yield return new WaitForSeconds(secondDelay);
        // Switch
        SwitchWeapon(direction);
    }
    #endregion

    #region Actions
    void Movement()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        Move(inputH, inputV);

        Ray groundRay = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(groundRay, out hit, groundRayDistance))
        {
            if (Input.GetButton("Jump"))
            {
                movement.y = jumpHeight;
            }
        }

        movement.y -= gravity * Time.deltaTime;

        if (movement.y < -gravity)
            movement.y = -gravity;

        controller.Move(movement * Time.deltaTime);
    }
    void Shooting()
    {
        // Is a current weapon selected
        if (currentWeapon)
        {
            // Is the fire button pressed?
            if (Input.GetButton("Fire1"))
            {
                if (currentWeapon.canShoot)
                {
                    // Shoot the current weapon
                    currentWeapon.Shoot(collider);

                    Vector3 direction = Vector3.up * 2f;
                    direction.x = Random.Range(-1f, 1f);

                    anim.SetTrigger("Fire");

                    cameraLook.ApplyOffset(direction * currentWeapon.recoil);
                }
            }
        }
    }
    void Switching()
    {
        // If there is more than one weapon
        if (weapons.Count > 1)
        {
            // Check animation state
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            // If animation state is in Idle
            if (info.IsName("Idle"))
            {
                // Get scroll input
                float inputScroll = Input.GetAxis("Mouse ScrollWheel");
                // If scroll input has been made
                if (inputScroll != 0)
                {
                    // Note (Manny): Find a nicer way to do this line:
                    int direction = inputScroll > 0 ? Mathf.CeilToInt(inputScroll) : Mathf.FloorToInt(inputScroll);
                    // Start animation for switching
                    anim.SetTrigger("Switch");
                    // Start delay for switching weapons
                    StartCoroutine(SwitchWeapon(switchDelay, direction));
                }
            }
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        Movement();
        Switching();
    }

    void FixedUpdate()
    {
        Shooting();
    }
}