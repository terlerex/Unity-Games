using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Portfolio
{
public class CollectibleItems : MonoBehaviour
{

    //Stamina
    public float StaminaPlus = 30.0f;
    private float maxStamina = 100.0f;

    //Health
    public float HealthPlus = 20.0f;
    public float maxHealth = 100.0f;


    void OnCollisionEnter(Collision col) 
    {
        if(col.gameObject.tag == "Player")
        {       
            if(gameObject.name == "Energy Box")
            {
                Destroy(gameObject);
                if(PlayerController.currentStamina <= 70)
                {
                    PlayerController.currentStamina = PlayerController.currentStamina + StaminaPlus; 

                }
                else
                {
                    PlayerController.currentStamina += StaminaPlus;
                    if(PlayerController.currentStamina >= maxStamina)
                    {
                        PlayerController.currentStamina = maxStamina;
                    }
  
                }  
            }

            if(gameObject.name == "MedicKit")
            {
                Destroy(gameObject);
                if(PlayerController.currentHealth <=80)
                {
                    PlayerController.currentHealth = PlayerController.currentHealth + HealthPlus;

                }
                else
                {
                    PlayerController.currentHealth += HealthPlus;
                    if(PlayerController.currentHealth >= maxHealth)
                    {
                        PlayerController.currentHealth = maxHealth;
                    }

                }
            }
                  
        }
    }

        void OnTriggerStay(Collider col) 
        {

        //FireDamage
        if(col.gameObject.tag == "Player")
        {
            if(gameObject.name == "Fire")
            {
                PlayerController.currentHealth = PlayerController.currentHealth - 0.1f * PlayerController.HealthDecreaseTime;
                Debug.Log(PlayerController.currentHealth);
            }
        }
    }  

}
    

}
