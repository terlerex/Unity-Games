using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Portfolio {

public class SaveData : MonoBehaviour
{

    public static SaveData instance;
    public GameObject player;

    private void Awake() {

        if(instance != null)
        {
            Debug.LogWarning("Il y à plus d'une instance SaveData dans la scène");
            return;
        }
        instance = this;
    }

    void Start()
    {
        if(PlayerPrefs.GetInt("Saved") == 1 && PlayerPrefs.GetInt("TimeToLoad") == 1)
        {
            float pX = player.transform.position.x;
            float pY = player.transform.position.y;
            float pZ = player.transform.position.z;

            pX = PlayerPrefs.GetFloat("p_x");
            pY = PlayerPrefs.GetFloat("p_y");
            pZ = PlayerPrefs.GetFloat("p_z");
            player.transform.position = new Vector3(pX,pY,pZ);
            PlayerPrefs.SetInt("TimeToLoad", 0);
            PlayerPrefs.Save();
        }

        PlayerController.instance.currentStamina = PlayerPrefs.GetFloat("currentStamina", PlayerController.startStamina);
        PlayerController.instance.currentHealth = PlayerPrefs.GetFloat("currentHealth", PlayerController.startHealth);
        Weapon.currentMagAmmo = PlayerPrefs.GetInt("currentMagAmmo", Weapon.instance.maxMagAmmo);
        Weapon.currentAmmo = PlayerPrefs.GetInt("currentAmmo", Weapon.instance.maxAmmo);
        PlayerPrefs.SetString("CurrentFireMode", Weapon.instance._fireMode.ToString());
        Weapon.instance.CurrentFireMode();
        Weapon.instance.CurrentMagAmmoText();
        Weapon.instance.CurrentAmmoText();
    }

    public void SaveSystem()
    {
        PlayerPrefs.SetString("CurrentFireMode", Weapon.instance._fireMode.ToString());
        PlayerPrefs.SetFloat("currentStamina", PlayerController.instance.currentStamina);
        PlayerPrefs.SetFloat("currentHealth", PlayerController.instance.currentHealth);
        PlayerPrefs.SetInt("currentMagAmmo", Weapon.currentMagAmmo);
        PlayerPrefs.SetInt("currentAmmo", Weapon.currentAmmo);
        PlayerPrefs.SetInt("SaveScene", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetFloat("p_x", player.transform.position.x);
        PlayerPrefs.SetFloat("p_y", player.transform.position.y);
        PlayerPrefs.SetFloat("p_z", player.transform.position.z);
        PlayerPrefs.SetInt("Saved", 1);
        PlayerPrefs.Save();
    }

    public void PlayerPosLoad()
    {
        PlayerPrefs.SetInt("TimeToLoad", 1);
        PlayerPrefs.Save();

    }

}

}



