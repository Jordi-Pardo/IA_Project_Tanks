using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUIController : MonoBehaviour
{
    [SerializeField] Shoot shootScript;
    public RechargeAction rechargeAction;

    public Color noAmmoColor;
    Color ammoColor;
    public List<Image> bulletSpriteList;
    // Start is called before the first frame update

    private void Awake()
    {
        shootScript.onShoot += RemoveBullet;
    }
    private void Start()
    {

        ammoColor = bulletSpriteList[0].color;
    }

    
    void RemoveBullet()
    {
        int bulletIndex = shootScript.ammo;
        bulletSpriteList[bulletIndex].color = noAmmoColor;
    }



    private void OnDestroy()
    {
        shootScript.onShoot -= RemoveBullet;
    }

    public void StartRechargeCO()
    {
        StartCoroutine(StartRecharge());
    }
    IEnumerator StartRecharge()
    {
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(shootScript.timeToRecharge / 5);
            bulletSpriteList[count].color = ammoColor;
            count++;
        }

        
    }
}
