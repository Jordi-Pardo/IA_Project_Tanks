using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Action onShoot;
    public GameObject shellPrefab; // Add Shell game object in the Inspector.
    public GameObject shellSpawnPos; // Add Cube game object in the Inspector.
    public GameObject target; // Add Enemy game object in the Inspector.
    public GameObject parent; // Add Tank game object in the Inspector.
    public bool lowAngle = false;
    float speed = 15;
    float turnSpeed = 2;
    bool canShoot = true;
    [SerializeField] TankHealth tankHealth;
    [SerializeField] Complete.TankMovement tankMovement;
    [SerializeField] DetectionAreaScript detection;
    public int ammo;
    int maxAmmo = 5;
    public float timeToRecharge = 3f;
    public bool HasAmmo { get { return ammo > 0; } }

    private void Awake()
    {
        RechargeAmmo();
       
    }

    void CanShootAgain()
    {
        canShoot = true;
    }

    void Fire()
    {
        bool targetDead = target.GetComponent<TankHealth>().m_Dead;
        if (targetDead && target != null)
        {
            if (detection)
            {
                detection.RemoveTarget(target);
                detection.ChangeTarget();
            }
            else
            {
                target = null;
            }
        }
        if (canShoot && !targetDead && ammo > 0)
        {
            GameObject shell = Instantiate(shellPrefab, shellSpawnPos.transform.position, shellSpawnPos.transform.rotation);
            shell.GetComponent<Rigidbody>().velocity = speed * this.transform.forward; // Use 'forward' because it's the Z axis you want to shoot along.
            if (tankHealth)
            {
                shell.GetComponent<ShellExplosion>().team = tankHealth.team;
            }
            canShoot = false;
            Invoke("CanShootAgain", 0.8f); // Increase value to slow down rate of fire, decrease value to speed up rate of fire.
            if (tankHealth.team != Team.Boss)
            {
                ammo--;
                onShoot?.Invoke();
                if(ammo <= 0)
                {
                    tankMovement.needRecharge = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) return;

        Vector3 direction = (target.transform.position - parent.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        parent.transform.rotation = Quaternion.Slerp(parent.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);

        float? angle = RotateTurret();

        if(angle != null && Vector3.Angle(direction, parent.transform.forward) < 10) // When the angle is less than 10 degrees...
            Fire(); // ...start firing.
    }

    float? RotateTurret()
    {
        float? angle = CalculateAngle(lowAngle); // Set to false for upper range of angles, true for lower range.

        if (angle != null) // If we did actually get an angle...
        {
            this.transform.localEulerAngles = new Vector3(360f - (float)angle, 0f, 0f); // ... rotate the turret using EulerAngles because they allow you to set angles around x, y & z.
        }
        return angle;
    }

    float? CalculateAngle(bool low)
    {
        Vector3 targetDir = target.transform.position - this.transform.position;
        float y = targetDir.y;
        targetDir.y = 0f;
        float x = targetDir.magnitude;
        float gravity = 9.81f;
        float sSqr = speed * speed;
        float underTheSqrRoot = (sSqr * sSqr) - gravity * (gravity * x * x + 2 * y * sSqr);

        if (underTheSqrRoot >= 0f)
        {
            float root = Mathf.Sqrt(underTheSqrRoot);
            float highAngle = sSqr + root;
            float lowAngle = sSqr - root;

            if (low)
                return (Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg);
            else
                return (Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg);

        }
        else
            return null;
    }

    public void RechargeAmmo()
    {
        ammo = maxAmmo;
    }
}
