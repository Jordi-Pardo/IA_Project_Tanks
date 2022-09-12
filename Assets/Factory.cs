using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    public Team team;
    [SerializeField] Transform spawnPoint;
    [Space]
    [SerializeField] Transform tankToCreate;
    [SerializeField] Material tankMaterial;
    public Transform rechargePoint;
    float timeToCreate = 2f;
    GameObject lastTankCreated;

    private void Start()
    {
        CreateTank();
    }

    public void CreateTank()
    {
        StartCoroutine(CO_CreateTank());
    }

    IEnumerator CO_CreateTank()
    {
        yield return new WaitForSecondsRealtime(timeToCreate);
        lastTankCreated = Instantiate(tankToCreate, spawnPoint).gameObject;
        TankHealth tankHealthScript = lastTankCreated.GetComponent<TankHealth>();
        tankHealthScript.factory = this;
        tankHealthScript.team = team;

        lastTankCreated.GetComponent<MaterialManager>().ChangeMaterial(tankMaterial);
    }

}
