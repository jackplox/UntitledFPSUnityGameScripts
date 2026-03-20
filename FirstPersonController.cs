using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FirstPersonController : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float sprintSpeed = 2f;
    public float gravity = -9.18f;
    public float jumpHeight = 0.25f;
    public float sensitivity = 0.25f;
    public Camera mainCamera;
    public GameObject ui;
    public GameObject impact;

    private GameObject currentItem;
    private CharacterController controller;
    private InputAction sprintAction;
    private InputAction interactAction;
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookDirection = Vector2.zero;
    private Vector3 velocity;
    private float moveSpeed;
    private float currentCamX = 0f;
    private TextMeshProUGUI pickUpText;
    private List<GameObject> inventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new List<GameObject>();
        pickUpText = ui.GetComponentInChildren<TextMeshProUGUI>();

        sprintAction = this.GetComponent<PlayerInput>().actions["Sprint"];
        sprintAction.Enable();
        
        interactAction = this.GetComponent<PlayerInput>().actions["Interact"];
        interactAction.Enable();

        mainCamera.transform.localPosition = Vector3.zero;
        
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        controller = this.GetComponent<CharacterController>();
    }  

    // Update is called once per frame
    void Update()
    {
        HandleLook();
        HandleSprint();
        HandleMove(); 
        HandleInteract();
    }

    public void HandleInteract()
    {
        //Interact
        
        RaycastHit? item = SeeObject();

        if (item.HasValue)
        {
            pickUpText.text = "Pick Up " + item.Value.collider.name+ " - " + interactAction.bindings[0];
            pickUpText.gameObject.SetActive(true);

            if(interactAction.WasPressedThisFrame()){
                Interact(item.Value.collider.gameObject);
            }
        }
        else
        {
            pickUpText.gameObject.SetActive(false);
        }
    }

    public void HandleMove()
    {
        //Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        //Moving
        Vector3 move = transform.rotation * new Vector3(moveDirection.x, 0, moveDirection.y);
        controller.Move(move * moveSpeed * Time.deltaTime); 
    }

    public void HandleSprint()
    {
        //Check sprint
        moveSpeed = baseSpeed;
        if (sprintAction.IsPressed())
        {
            moveSpeed = baseSpeed * sprintSpeed;
        } 
    }

    public void HandleLook()
    {
        //Get directions
        float mouseX = lookDirection.x * sensitivity;
        float mouseY = lookDirection.y * sensitivity;

        // Clamp vertical camera rotation
        currentCamX = Mathf.Clamp(currentCamX - mouseY, -90, 90);
        mainCamera.transform.localEulerAngles = new Vector3(currentCamX, 0, 0);
        transform.Rotate(0, mouseX, 0); 
    }

    public void Interact(GameObject item)
    {
        switch (item.tag)
        {
            case "Weapon":
                inventory.Add(item);
                Debug.Log("INVENTORY: " + inventory.ToString());
                if (currentItem is null)
                {
                    currentItem = item;
                    Debug.Log("Current item: " + currentItem.name);
                }
                item.SetActive(false);
                break;
            default:
                break;
        }
    }

    public RaycastHit? SeeObject()
    {
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 3f, Color.red);

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        LayerMask mask = LayerMask.GetMask("Weapon");
        if(Physics.Raycast(ray, out RaycastHit hit, 3f, mask))
        {
            return hit;
        }

        return null;
    } 

    public void OnLook(InputAction.CallbackContext context)
    {
        lookDirection = context.ReadValue<Vector2>();
        //Debug.Log($"LOOKING AROUND : {lookDirection}");
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded && context.performed)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();

    }

    public void OnFire(InputAction.CallbackContext context)
    {
        //TODO: ADD WEAPON CHECK
        
        float weaponMaxDistance = 100f;
        float weaponForce = 5f;
        
        //TODO: ADD CHECK IF AUTOMATIC && HELD vv This is for semi-automatic
        if (context.performed && currentItem != null)
        {
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

            LayerMask mask = LayerMask.GetMask("World", "NPC"); //Add more, if need be.
            if (Physics.Raycast(ray, out RaycastHit hit, weaponMaxDistance, mask))
            {
                switch (hit.collider.gameObject.tag)
                {
                    case "World":
                        Debug.Log("WORLD: " + hit.collider.gameObject.name);
                        Instantiate(impact, 
                            hit.point + (hit.normal * 0.01f),   //0.01f to avoid clipping with wall. Maybe fix this in the future.
                            Quaternion.FromToRotation(Vector3.up, hit.normal));
                        break;
                    case "NPC":
                        Debug.Log("NPC: " + hit.collider.gameObject.name);
                        hit.collider.GetComponent<Rigidbody>().AddForce(ray.direction * weaponForce, ForceMode.Impulse);
                        hit.collider.GetComponent<NPCScript>().takeDamage(weaponForce);
                        break;
                    default:
                        break;
                }
            }
        }
        else if(context.performed)
        {
            Debug.Log("Did not fire -- CURRENT ITEM: " + currentItem);
        }
    }
}
