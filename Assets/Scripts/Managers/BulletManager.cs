using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager instance;
    List<GameObject> bulletList;
    [SerializeField] GameObject bullet;
    [SerializeField] int bulletListSize;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bulletList = new List<GameObject>();
        for(int i = 0; i < bulletListSize; i++)
        {
            GameObject bullet = (GameObject)Instantiate(this.bullet);
            bullet.SetActive(false);
            bulletList.Add(bullet);
            bullet.transform.SetParent(this.transform);
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletListSize; i++)
        {
            if (!bulletList[i].activeInHierarchy)
            {
                return bulletList[i];
            }
        }
        bulletListSize++;
        GameObject newBullet = (GameObject)Instantiate(this.bullet);
        newBullet.SetActive(false);
        bulletList.Add(newBullet);
        newBullet.transform.SetParent(this.transform);
        return newBullet;
    }
}
