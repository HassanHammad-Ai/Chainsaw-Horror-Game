using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public int maxAmmoInMag = 10;
    public int maxAmmoInStorage = 30;
    public float shootCooldown = 0.5f;
    public float reloadCooldown = 1f;
    public float shootRange = 100f;
    public int damage = 10;

    public ParticleSystem impactEffect;

    public int currentAmmoInMag;
    public int currentAmmoInStorage;

    private bool canShoot = true;
    private bool isReloading = false;
    private float shootTimer;

    public Transform cartridgeEjectionPoint;
    public GameObject cartridgePrefab;
    public float cartridgeEjectionForce = 5f;

    public Animator gun;
    public ParticleSystem muzzleFlash;
    public GameObject muzzleFlashLight;
    public AudioSource shootAudio;

    void Start()
    {
        currentAmmoInMag = maxAmmoInMag;
        currentAmmoInStorage = maxAmmoInStorage;

        muzzleFlashLight.SetActive(false);
    }

    void Update()
    {
        if (shootTimer > 0)
            shootTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void Shoot()
    {
        if (!canShoot || isReloading) return;

        if (currentAmmoInMag <= 0)
        {
            Debug.Log("Out of ammo in mag!");
            return;
        }

        if (shootTimer > 0) return;

        shootAudio.Play();
        muzzleFlash.Play();
        muzzleFlashLight.SetActive(true);
        gun.SetBool("shoot", true);

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootRange))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // 🔥 FIX: works even if collider is on child
            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        // Shell ejection
        if (cartridgePrefab != null && cartridgeEjectionPoint != null)
        {
            GameObject cartridge = Instantiate(cartridgePrefab, cartridgeEjectionPoint.position, cartridgeEjectionPoint.rotation);
            Rigidbody rb = cartridge.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(cartridgeEjectionPoint.right * cartridgeEjectionForce, ForceMode.Impulse);
            }
        }

        currentAmmoInMag--;
        shootTimer = shootCooldown;

        StartCoroutine(ResetShootAnim());
        StartCoroutine(ResetLight());

        Debug.Log("Ammo: " + currentAmmoInMag);
    }

    void Reload()
    {
        if (isReloading) return;
        if (currentAmmoInStorage <= 0) return;
        if (currentAmmoInMag == maxAmmoInMag) return;

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        canShoot = false;

        gun.SetBool("reload", true);

        yield return new WaitForSeconds(reloadCooldown);

        int needed = maxAmmoInMag - currentAmmoInMag;
        int toLoad = Mathf.Min(needed, currentAmmoInStorage);

        currentAmmoInMag += toLoad;
        currentAmmoInStorage -= toLoad;

        gun.SetBool("reload", false);

        isReloading = false;
        canShoot = true;
    }

    IEnumerator ResetShootAnim()
    {
        yield return new WaitForSeconds(0.1f);
        gun.SetBool("shoot", false);
    }

    IEnumerator ResetLight()
    {
        yield return new WaitForSeconds(0.1f);
        muzzleFlashLight.SetActive(false);
    }
}