using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public enum FireMode {Semi, Burst, Auto}

public class Weapon : MonoBehaviour
{

    [Header("Weapon Inputs")]
    [SerializeField] private KeyCode _shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode _cycleFireModeKey = KeyCode.F;
    

    [Header("Ammo and Reloading")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 3f;
    public int AmmoBox = 4;
    public int AmmoPerBox;
    

    [Header("Weapons Logics")]
    [SerializeField ]private FireMode _fireMode = FireMode.Semi;
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
    public Text Ammo;
    public Text AmmoBoxText;

    private void Start() 
    {     
        AmmoPerBox = maxAmmo;
        currentAmmo = maxAmmo;
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


    void Update()
    {
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

        //Fire mode Text
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

        // Reload

        if(IsReloading)
            return;
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if(currentAmmo <= 0)
        {
            CantShoot();
        }

        Ammo.text = currentAmmo.ToString() + " / " + AmmoPerBox.ToString();
        AmmoBoxText.text = AmmoBox.ToString() + " AmmoBox";

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
       if(Input.GetKeyDown(_cycleFireModeKey))
       {
           CycleFireMode();
       }
    }

    //Shoot 
    private void FireGun()
    {
       Debug.DrawRay(firePoint.position, firePoint.right * 100, Color.red, 2f);
       Ray ray = new Ray(firePoint.position, firePoint.right);
       RaycastHit hitInfo;
       MuzzleFlash.Play();
       currentAmmo--;

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


    //Reload
    IEnumerator Reload()
    {
        IsReloading = true;
        AmmoBox--;
        Debug.Log("reloading");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        IsReloading = false;
    }
    
    //Burst
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


