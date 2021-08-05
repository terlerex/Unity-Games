using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portfolio 
{
    public enum FireMode {Semi, Burst, Auto}

public class Weapon : MonoBehaviour
{

    [Header("Weapon Inputs")]
    [SerializeField] private KeyCode _shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode _cycleFireModeKey = KeyCode.F;
    

    [Header("Ammo and Reloading")]
    public int maxMagAmmo = 30;
    public static int currentMagAmmo;
    public int maxAmmo = 360;
    public static int currentAmmo;
    public int currentOffestAmmo;
    public float reloadTime = 3f;

    

    [Header("Weapons Logics")]

    [SerializeField ]public FireMode _fireMode = FireMode.Semi;
    [SerializeField] private Transform firePoint;
    [SerializeField] [Range(0.1f, 10.5f)] private float fireRate = 1.5f;
    private bool _shootInput = false;
    private bool _bursting = false;
    private bool IsReloading = false;
    private float timer;
    private int damage = 5; 
    private bool IsAuto = true;

    [Header("Particles / UI")]
    public ParticleSystem MuzzleFlash;
    public GameObject MuzzleLight;
    public GameObject ImpactEffect;
    public GameObject BloodImpactEffect;
    public Text fireModeText;
    public Text AmmoMag;
    public Text AmmoMax;


    public static Weapon instance;

    private void Awake() {

        if(instance != null)
        {
            Debug.LogWarning("Il y à plus d'une instance Weapon dans la scène");
            return;
        }

        instance = this;
    }


    private void Start() 
    {   
        currentAmmo = PlayerPrefs.GetInt("currentAmmo", maxAmmo);
        currentMagAmmo = PlayerPrefs.GetInt("currentMagAmmo", maxMagAmmo);
        MuzzleFlash.Stop();
        MuzzleLight.SetActive(false);
    }

    //Can Shoot
    private bool canShoot()
    {
        if(timer < fireRate)return false;
        if(_bursting) return false;
        return true;
    }

    //Can't Shoot
    private void CantShoot()
    {
        Debug.Log("Plus de munitions");
        _shootInput = false;
    }
    

    //Void Ammo text
    public void CurrentMagAmmoText()
    {
        AmmoMag.text = currentMagAmmo.ToString();
    }
    public void CurrentAmmoText()
    {
        AmmoMax.text = currentAmmo.ToString();
    }

    public void CurrentFireMode()
    {
         if(IsAuto)
        {
            fireModeText.text = "Full-Auto";
        }
        else if(_fireMode == FireMode.Burst)
        {
            fireModeText.text = "Burst";
        }
        else
        {
           fireModeText.text = "Semi-Auto"; 
        }
    }


    void Update()
    { 
        //Ammo Text
        CurrentMagAmmoText();
        CurrentAmmoText();
        //FireMode Texte
        CurrentFireMode();

        //Check ammo not used
        currentOffestAmmo = maxMagAmmo - currentMagAmmo;
        
        
        // Fire mode switch
        switch(_fireMode)
        {
            case FireMode.Auto:
            IsAuto = true;
            _shootInput = Input.GetKey(_shootKey);
                break;

            default:
            IsAuto = false;
            _shootInput = Input.GetKeyDown(_shootKey);
                break;
        }

        // Reload
        if(IsReloading)
            return;
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        //Check if ammo or mag is empty
        if(currentMagAmmo <= 0)
        {
            CantShoot();
        }
        if(currentAmmo < 0)
        {
            CantShoot();       
        }
        //Check player is 
        if(PlayerController.IsCrouching)
        {
            CantShoot();
        }
        



        //Shoot
       timer += Time.deltaTime;
       if(_shootInput)
       {
           if(canShoot())
           {
            timer = 0f; 
            FireGun();
           }

           if(_fireMode == FireMode.Burst)
           {
               _bursting = true;
               StartCoroutine(BurstFire());
           }
       }
       //Change Fire mode
       if(Input.GetKeyDown(_cycleFireModeKey))
       {
           CycleFireMode();
       }
    }

    //Shoot Fonction
    private void FireGun()
    {
        if(PlayerController.isTurn)  
        {
            Debug.DrawRay(firePoint.position, -firePoint.right * 100, Color.red, 2f);
            Ray ray = new Ray(firePoint.position, -firePoint.right);
            RaycastHit hitInfo;
            MuzzleFlash.Play();
            CinemachineShake.Instance.ShakeCamera(2f, .1f);
            currentMagAmmo--;

       if(Physics.Raycast(ray, out hitInfo, 100))
       {
           if(hitInfo.transform.CompareTag("Enemy"))
           {
               Debug.Log("Hit : " + damage);
               hitInfo.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
               Instantiate(BloodImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
           }
           else
           {
               Instantiate(ImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
               Debug.Log("Miss");
               
           }   
       }
        } 
        else
        {
            Debug.DrawRay(firePoint.position, firePoint.right * 100, Color.red, 2f);
            Ray ray = new Ray(firePoint.position, firePoint.right);
            RaycastHit hitInfo;
            CinemachineShake.Instance.ShakeCamera(2f, .1f);
            MuzzleFlash.Play();
            currentMagAmmo--;

       if(Physics.Raycast(ray, out hitInfo, 100))
       {
           if(hitInfo.transform.CompareTag("Enemy"))
           {
               Debug.Log("Hit : " + damage);
               hitInfo.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
               Instantiate(BloodImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
           }
           else
           {
               Instantiate(ImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
               Debug.Log("Miss");
               
           }   
       }
        }   


    }

    //Reload Fonction
    IEnumerator Reload()
    {
        IsReloading = true;
        Debug.Log("reloading");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = currentAmmo - currentOffestAmmo;
        currentMagAmmo = maxMagAmmo;
        IsReloading = false;
    }
    
    //Burst mode
    private IEnumerator BurstFire()
    {
        yield return new WaitForSeconds(fireRate);
        FireGun();
        yield return new WaitForSeconds(fireRate);
        FireGun();
        yield return new WaitForSeconds(fireRate * 3);
        _bursting = false;
    }
    //Change Fire mode
    private void CycleFireMode() => _fireMode = ((int) _fireMode < 2) ? _fireMode + 1 : 0;

}

}


