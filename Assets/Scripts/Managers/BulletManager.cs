using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] GameObject pistolBullet, smgBullet, mgBullet, sniperBullet, shotgunBullet, rocketBullet, granadeBullet;
    [SerializeField] Transform pistolContainer, smgContainer, mgContainer, sniperContainer, shotgunContainer, rocketContainer, granadeContainer;
    [SerializeField] int pistolSize, smgSize, mgSize, shotgunSize, sniperSize, rocketSize, granadeSize;

    public static BulletManager instance;
    Dictionary<string, List<GameObject>> bulletsPools;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        bulletsPools = new Dictionary<string, List<GameObject>>();
        GenerateBulletPool();
    }

    void GenerateBulletPool()
    {
        if (pistolBullet && smgBullet && mgBullet && shotgunBullet && sniperBullet && rocketBullet && granadeBullet)
        {
            bulletsPools.Add(pistolBullet.name, new List<GameObject>(pistolSize));
            bulletsPools.Add(smgBullet.name, new List<GameObject>(smgSize));
            bulletsPools.Add(mgBullet.name, new List<GameObject>(mgSize));
            bulletsPools.Add(shotgunBullet.name, new List<GameObject>(shotgunSize));
            bulletsPools.Add(sniperBullet.name, new List<GameObject>(sniperSize));
            bulletsPools.Add(rocketBullet.name, new List<GameObject>(rocketSize));
            bulletsPools.Add(granadeBullet.name, new List<GameObject>(granadeSize));
            //aggiungere melee

            FillList(bulletsPools[pistolBullet.name], pistolSize, pistolBullet, pistolContainer);
            FillList(bulletsPools[smgBullet.name], smgSize, smgBullet, smgContainer);
            FillList(bulletsPools[mgBullet.name], mgSize, mgBullet, mgContainer);
            FillList(bulletsPools[shotgunBullet.name], shotgunSize, shotgunBullet, shotgunContainer);
            FillList(bulletsPools[sniperBullet.name], sniperSize, sniperBullet, sniperContainer);
            FillList(bulletsPools[rocketBullet.name], rocketSize, rocketBullet, rocketContainer);
            FillList(bulletsPools[granadeBullet.name], granadeSize, granadeBullet, granadeContainer);
            //aggiungere melee
        }
    }

    void FillList(List<GameObject> list, int size, GameObject obj, Transform container)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject newObj = Instantiate<GameObject>(obj);
            newObj.SetActive(false);
            list.Add(newObj);
            newObj.transform.parent = container;
        }
    }

    public GameObject GetBullet(string gunType)
    {
        List<GameObject> bulletList = bulletsPools[gunType];
        GameObject bulletToReturn = GetBullet(bulletList);
        if (bulletToReturn == null) return null;

        return bulletToReturn;

    }

    public GameObject GetBullet(List<GameObject> bulletList)
    {
        while (true)
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (!bulletList[i].activeInHierarchy)
                {
                    return bulletList[i];
                }
            }
        }       
    }
}
