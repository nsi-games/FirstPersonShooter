using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

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

    [Header("UI")]
    public GameObject interactUIPrefab;
    public Transform interactUIParent;

    [Header("References")]
    public Camera attachedCamera;
    public Transform hand;

    // Components
    private Animator anim;

    // Movement
    private CharacterController controller;
    private Vector3 movement;

    // UI
    private GameObject interactUI;
    private TextMeshProUGUI interactText;

    // Weapons
    public Weapon currentWeapon;
    private List<Weapon> weapons = new List<Weapon>();
    private int currentWeaponIndex = 0;

    void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        CreateUI();
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
    void CreateUI()
    {
        interactUI = Instantiate(interactUIPrefab, interactUIParent);
        interactText = interactUI.GetComponentInChildren<TextMeshProUGUI>();
    }
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
    void Pickup(Weapon weaponToPickup)
    {
        // Pick up the weapon
        weaponToPickup.Pickup();
        // Get transform
        Transform weaponTransform = weaponToPickup.transform;
        weaponTransform.SetParent(hand);
        weaponTransform.localRotation = Quaternion.identity;
        weaponTransform.localPosition = Vector3.zero;
        // Add to list
        weapons.Add(weaponToPickup);
        // Select new weapon
        SelectWeapon(weapons.Count - 1);
    }
    void Drop(Weapon weaponToDrop)
    {
        // Remove from list
        weapons.Remove(weaponToDrop);
        // Get transform
        Transform weaponTransform = weaponToDrop.transform;
        weaponTransform.SetParent(null);
        // Drop the weapon
        weaponToDrop.Drop();
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
    void Interact()
    {
        interactUI.SetActive(false);
        // Create a ray from centre of camera
        Ray interactRay = attachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, .5f));
        RaycastHit hit;
        // Shoot ray in a range
        if (Physics.Raycast(interactRay, out hit, interactRange))
        {
            // Try getting Interactable object
            Interactable interact = hit.collider.GetComponent<Interactable>();
            // Is there an interactable object available
            if (interact)
            {
                // Enable Interact UI
                interactUI.SetActive(true);
                // Change text to item's title
                interactText.text = interact.title;

                // Get input from user
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Switch the different types of interactions
                    switch (interact.type)
                    {
                        case Interactable.Type.Weapon:
                            // Get Weapon Script from interactable
                            Weapon weapon = interact.GetComponent<Weapon>();
                            if (weapon)
                            {
                                // Pickup the weapon
                                Pickup(weapon);
                            }
                            break;
                        case Interactable.Type.Item:
                            break;
                        case Interactable.Type.Consumable:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    void Shooting()
    {
        // Is a current weapon selected
        if (currentWeapon)
        {
            // Is the fire button pressed?
            if (Input.GetButton("Fire1"))
            {
                // Shoot the current weapon
                currentWeapon.Shoot();
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
        //if (photonView.isMine)
        //{
            Movement();
            Interact();
            Shooting();
            Switching();
        //}
    }
}