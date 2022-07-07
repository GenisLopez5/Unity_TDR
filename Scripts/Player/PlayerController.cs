using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Move")]
    CharacterController controller;

    public float speed = 12f;
    public float gravity = -10f;
    public float jumpHeight = 2f;

    Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    [Header("Look")]

    public float mouseSensitivity = 100f;
    float mousexRotation = 0f;
    GameObject cameraObj;

    [Header("Raycast")]
    public LayerMask layerMask;
    public Image CursorImage;
    public Color normal, pickable;
    public Camera cam;

    [Header("Stats")]
    public Animator[] HealthAnim, StaminaAnim, HungerAnim;
    public float MaxHealth, Health, MaxStamina, Stamina, MaxHunger, Hunger;
    float prevHunger;

    bool running;
    bool previousStateRunning;
    bool ableToRecover = true;

    bool ableToMove = true;


    [HideInInspector]
    public PhotonView view;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        groundCheck = gameObject.transform.GetChild(1);
        cameraObj = GetComponentInChildren<Camera>().gameObject;

        view = GetComponent<PhotonView>();

        if (view.IsMine)
        {
            cameraObj.SetActive(true);
        }
        else
        {
            cameraObj.SetActive(false);
        }

        Health = MaxHealth;
        Stamina = MaxStamina;
        Hunger = MaxHunger;
    }

    private void Awake()
    {
        InGameEvents.current.OnInventoryOpened += CancelMove;
        InGameEvents.current.OnGamePaused += CancelMove;

        InGameEvents.current.OnInventoryClosed += InvClosed;
        InGameEvents.current.OnGameResumed += AbleMove;

        ConfigEvents.current.OnSensitivityChanged += OnSensitivityChanged;
    }

    private void OnDestroy()
    {
        InGameEvents.current.OnInventoryOpened -= CancelMove;
        InGameEvents.current.OnGamePaused -= CancelMove;

        InGameEvents.current.OnInventoryClosed -= InvClosed;
        InGameEvents.current.OnGameResumed -= AbleMove;

        ConfigEvents.current.OnSensitivityChanged -= OnSensitivityChanged;
    }

    private void Update()
    {
        if (!view.IsMine) return;

        Look();
        RayCast();
        if (ableToRecover && !running) StaminaR();
        HealthR();
        HungerR();
    }

    private void FixedUpdate()
    {
        if (!view.IsMine) return;

        Move();
    }

    private void Move()
    {
        float x;
        float z;
        float vel = 1;

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -5f;
        }

        previousStateRunning = running;

        if (Input.GetKey(KeyCode.LeftShift) && z > 0 && x == 0 && Stamina > 0 && ableToMove)
        {
            vel = 2.5f;
            Stamina -= 5 * Time.deltaTime;
            running = true;

            StaminaAnim[0].SetBool("Changing", false);
            StaminaAnim[1].SetBool("Changing", false);
        }
        else
        {
            vel = 1;
            running = false;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        if (ableToMove) controller.Move(move * speed * Time.deltaTime * vel);

        if (Input.GetButtonDown("Jump") && isGrounded && Stamina > 10 && ableToMove)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Stamina -= 10;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Look()
    {
        if (!ableToMove) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        mousexRotation -= mouseY;
        mousexRotation = Mathf.Clamp(mousexRotation, -90f, 90f);

        cameraObj.transform.localRotation = Quaternion.Euler(mousexRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void RayCast()
    {
        if (!ableToMove) return;

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out hit, 3f, layerMask);

        CursorImage.color = normal;

        if (hit.transform != null)
        {   
            if (hit.transform.tag == "PickableObject")
            {
                CursorImage.color = pickable;
                if (Input.GetButtonDown("Interact"))
                {
                    InGameEvents.current.PickableObjectInteracted(hit.transform.gameObject.GetComponent<PickableObject>());
                }
            }
        }
    }

    async void StaminaR()
    {
        if (previousStateRunning == true && running == false)
        {
            ableToRecover = false;

            StaminaAnim[0].SetBool("Changing", false);
            StaminaAnim[1].SetBool("Changing", false);

            if (Stamina <= 0)
            {
                await Task.Delay(3000);
            }
            else
            {
                await Task.Delay(1500);
            }
            

            ableToRecover = true;
        }

        if (Stamina < 100)
        {
            if (Hunger > 0)
            {
                Stamina += 10 * Time.deltaTime;
                Hunger -= Random.Range(0.33f, 0.66f) * Time.deltaTime;
            }
            else
            {
                Stamina += 0.5f * Time.deltaTime;
            }

            StaminaAnim[0].SetBool("Changing", true);
            StaminaAnim[1].SetBool("Changing", true);
        }
        else
        {
            StaminaAnim[0].SetBool("Changing", false);
            StaminaAnim[1].SetBool("Changing", false);
        }

        if (Stamina > MaxStamina) Stamina = MaxStamina;
        if (Stamina < 0) Stamina = 0;
    }

    void HealthR()
    {
        if (Health < MaxHealth)
        {
            if (Hunger > 0)
            {
                if (Hunger > MaxHunger - 1)
                {
                    Health += 10 * Time.deltaTime;
                    Hunger -= 1.5f * Time.deltaTime;
                }
                else if (Hunger > MaxHunger - 5)
                {
                    Health += 2 * Time.deltaTime;
                    Hunger -= 0.75f;
                }
                else
                {
                    Health += 1 * Time.deltaTime;
                    Hunger -= 0.5f * Time.deltaTime;
                }

                HealthAnim[0].SetBool("Changing", true);
                HealthAnim[1].SetBool("Changing", true);
            }
            else
            {
                HealthAnim[0].SetBool("Changing", false);
                HealthAnim[1].SetBool("Changing", false);
            }
            
        }
        else
        {
            HealthAnim[0].SetBool("Changing", false);
            HealthAnim[1].SetBool("Changing", false);
        }

        if (Health > MaxHealth) Health = MaxHealth;
        if (Health < 0) Health = 0;
    }

    void HungerR()
    {
        if (prevHunger < Hunger)
        {
            HungerAnim[0].SetBool("Changing", true);
            HungerAnim[1].SetBool("Changing", true);
        }
        else
        {
            HungerAnim[0].SetBool("Changing", false);
            HungerAnim[1].SetBool("Changing", false);
        }

        if (Hunger == 0)
        {
            Health -= 0.2f * Time.deltaTime;
        }

        prevHunger = Hunger;

        Hunger -= 0.05f * Time.deltaTime;

        if (Hunger > MaxHunger) Hunger = MaxHunger;
        if (Hunger < 0) Hunger = 0;
    }

    void CancelMove()
    {
        ableToMove = false;
    }

    void AbleMove()
    {
        ableToMove = true;
    }

    void InvClosed(bool withEsc)
    {
        if (!withEsc) AbleMove();
    }

    void OnSensitivityChanged(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}
