using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] int bulletListSize;
    [SerializeField] GameObject pistolBullet, smgBullet, mgBullet, sniperBullet, shotgunBullet, rocketBullet, granadeBullet;
    [SerializeField] Transform pistol, smg, mg, sniper, shotgun, rocket, granade;

    public static BulletManager instance;
    List<GameObject> pistolBulletList, smgBulletList, mgBulletList, sniperBulletList, shotgunBulletList, rocketBulletList, granadeBulletList;
    int pistolCounter, smgCounter, mgCounter, sniperCounter, shotgunCounter, rocketCounter, granadeCounter;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        pistolBulletList = new List<GameObject>(); 
        smgBulletList = new List<GameObject>(); 
        mgBulletList = new List<GameObject>(); 
        sniperBulletList = new List<GameObject>(); 
        shotgunBulletList = new List<GameObject>(); 
        rocketBulletList = new List<GameObject>(); 
        granadeBulletList = new List<GameObject>();
        for (int i = 0; i < bulletListSize; i++)
        {
            GameObject pistolBullet = (GameObject)Instantiate(this.pistolBullet);
            GameObject smgBullet = (GameObject)Instantiate(this.smgBullet);
            GameObject mgBullet = (GameObject)Instantiate(this.mgBullet);
            GameObject sniperBullet = (GameObject)Instantiate(this.sniperBullet);
            GameObject shotgunBullet = (GameObject)Instantiate(this.shotgunBullet);
            GameObject rocketBullet = (GameObject)Instantiate(this.rocketBullet);
            GameObject granadeBullet = (GameObject)Instantiate(this.granadeBullet);
            pistolBullet.SetActive(false); smgBullet.SetActive(false); mgBullet.SetActive(false); sniperBullet.SetActive(false); shotgunBullet.SetActive(false); rocketBullet.SetActive(false); granadeBullet.SetActive(false);
            pistolBulletList.Add(pistolBullet); smgBulletList.Add(smgBullet); mgBulletList.Add(mgBullet); sniperBulletList.Add(sniperBullet); shotgunBulletList.Add(shotgunBullet); rocketBulletList.Add(rocketBullet); granadeBulletList.Add(granadeBullet);
            pistolBullet.transform.SetParent(pistol); smgBullet.transform.SetParent(smg); mgBullet.transform.SetParent(mg); sniperBullet.transform.SetParent(sniper); shotgunBullet.transform.SetParent(shotgun); rocketBullet.transform.SetParent(rocket); granadeBullet.transform.SetParent(granade);
        }
    }

    public GameObject GetBullet(string gunType)
    {
        GameObject bulletToReturn = null;
        switch (gunType)
        {
            case "Pistol":
                {
                    Debug.Log("Spawn " + gunType + " Bullet");
                    bulletToReturn = GetBullet(ref pistolBulletList, ref pistolCounter); 
                    break;
                }
                
            case "SMG":
                {
                    Debug.Log("Spawn " + gunType + " Bullet");
                    bulletToReturn = GetBullet(ref smgBulletList, ref smgCounter); 
                    break;
                }
            case "MG":
                {
                    Debug.Log("Spawn " + gunType + " Bullet");
                    bulletToReturn = GetBullet(ref mgBulletList, ref mgCounter); 
                    break;
                }
            case "Sniper":
                {
                    Debug.Log("Spawn " + gunType + " Bullet");
                    bulletToReturn = GetBullet(ref sniperBulletList, ref sniperCounter);
                    break;
                }
            case "Shotgun":
                {
                    Debug.Log("Spawn " + gunType + " Bullet");
                    bulletToReturn = GetBullet(ref shotgunBulletList, ref shotgunCounter); 
                    break;
                }
            case "RocketLauncher":
                {
                    Debug.Log("Spawn " + gunType + " Bullet");
                    bulletToReturn = GetBullet(ref rocketBulletList, ref rocketCounter); 
                    break;
                }
            case "GranadeLauncher":
                {
                    Debug.Log("Spawn " + gunType + " Bullet");
                    bulletToReturn = GetBullet(ref granadeBulletList, ref granadeCounter); 
                    break;
                }
        }
        if (bulletToReturn == null) return null;

        return bulletToReturn;

    }

    public GameObject GetBullet(ref List<GameObject> bulletList, ref int bulletCounter)
    {
        while (true)
        {
            for (int i = bulletCounter; i < bulletListSize; i++)
            {
                if (!bulletList[i].activeInHierarchy)
                {
                    bulletCounter++;
                    return bulletList[i];
                }
                if (bulletCounter == bulletListSize) bulletCounter = 0;
            }
            if (bulletCounter == bulletListSize) bulletCounter = 0;
        }       
    }
}
