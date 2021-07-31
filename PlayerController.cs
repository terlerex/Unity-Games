using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace Portfolio 
{
public class PlayerController : MonoBehaviour
{

    //Health
    public static float currentHealth;
    public static float startHealth = 100f;
    [HideInInspector] public static float HealthDecreaseTime = 2.0f;
    public Slider healthSlider;


    //Stamina
    public static float currentStamina;
    public static int startStamina = 100;
    public float staminaIncreaseTime = 2.0f;
    public float staminaDecreaseTime = 2.0f;
    public Slider staminaSlider;
               

    //Movement
    [SerializeField] float speed = 6.0f;
    private CapsuleCollider capsuleCollider;
    public float sprintSpeed = 6.0f;
    private bool isSprinting;
    private bool isMoving;

    //Grouching
    private bool IsGrouching = false;
    private float startColliderHeight = 0f;
    public float CrouchingSpeed = 3f;
    
    //Jump
    private Rigidbody rb;
    public float jumpforce = 7.0f;
    private bool onGround;
    private bool IsJumping;

    //Climb
    public float climbSpeed = 3f;
    private bool ClimbLadder = false;



    void Start()

    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        onGround = true;
        startColliderHeight = capsuleCollider.height;
        currentStamina = startStamina;
        currentHealth = startHealth;
    }


    void Update()
    {
        //Health
        healthSlider.value = currentHealth;
        if(currentHealth < 0)
        {
            SceneManager.LoadScene(0);
        }

        //Stamina 
        staminaSlider.value = currentStamina;

        if(IsJumping && currentStamina >=20)
        {
            currentStamina = currentStamina - 20f;
        }

        if(isSprinting && isMoving && currentStamina >= 0 && onGround)
        {
            currentStamina = currentStamina - 0.02f * staminaDecreaseTime;           
        }
        else if(!isSprinting && currentStamina <= 100 && onGround)
        {
            currentStamina = currentStamina + 0.01f  * staminaIncreaseTime;
        }

        if(currentStamina <= 0)
        {
            sprintSpeed = speed;
        }
        else
        {
            sprintSpeed = 6.0f;
        }

        //Sprint
        if(Input.GetKey(KeyCode.LeftShift) && !IsGrouching && onGround)
        {           
            speed = sprintSpeed; 
            isSprinting = true;           
        }
        else
        {
            isSprinting = false;
        }

        //Climb
        if(ClimbLadder && Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
            GetComponent<Rigidbody>().useGravity = false;
        }

        //Mouvement
        if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }


        if(Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
            isMoving = true;
        }
     

        if(Input.GetKey(KeyCode.Z))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            isMoving = true;
        }

        
        if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);
            isMoving = true;
        }
      
        
        if(Input.GetKey(KeyCode.Space) && onGround == true && currentStamina >= 20)
        {
            IsJumping = true;
            rb.velocity = new Vector3(0f, jumpforce , 0f);  
            onGround = false;            
        } 
        else
        {
            IsJumping = false;
        }

        //S'acroupir
        if(Input.GetKey(KeyCode.C))
        {   

            IsGrouching = true;
            capsuleCollider.height = startColliderHeight * 0.5f;
            float centery = 0f;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, centery, capsuleCollider.center.z);
            Vector3 local = transform.localScale;
            transform.localScale = new Vector3(1,0.5f,1);
            speed = CrouchingSpeed;
        }
        else
        {
            IsGrouching = false;
            capsuleCollider.height = startColliderHeight;
            float centery = 0f;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, centery, capsuleCollider.center.z);
            transform.localScale = new Vector3(1,1,1);
            speed = 3f;
        }
     

    }

    void OnCollisionEnter(Collision col) 
    {
        //Jump
        if(col.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
        //ClimbLadder
        if(col.gameObject.CompareTag("Ladder"))
        {          
            ClimbLadder = true;
        }
    }
    //ClimbLadder
    void OnCollisionExit(Collision other) 
    {
        if(other.gameObject.CompareTag("Ladder"))
        {
            GetComponent<Rigidbody>().useGravity = true;         
            ClimbLadder = false;
        }     
    }

                                                         

}

}
