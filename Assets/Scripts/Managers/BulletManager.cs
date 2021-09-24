using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers{
    public class BulletManager : MonoBehaviour
    {
        public static BulletManager instance;
        [SerializeField] Pool[] pools;
        [SerializeField] Transform container;
        Dictionary<string, Pool> bulletPools;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            bulletPools = new Dictionary<string, Pool>();
            GenerateBulletPool();
        }

        void GenerateBulletPool()
        {
            for(int i = 0; i < pools.Length; i++){
                for(int j = 0; j < pools[i].listSize; j++){
                    GameObject bullet = Instantiate<GameObject>(pools[i].prefab);
                    bullet.SetActive(false);
                    pools[i].poolList.Add(bullet);
                    bullet.transform.parent = container;
                }
                bulletPools.Add(pools[i].poolName, pools[i]);
            }
        }

        public GameObject GetBullet(string poolName)
        {
            var bulletList = bulletPools[poolName].poolList;
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (!bulletList[i].activeInHierarchy) return bulletList[i];
            }
            GameObject newBullet = Instantiate<GameObject>(bulletPools[poolName].prefab);
            bulletPools[poolName].poolList.Add(newBullet);
            newBullet.transform.parent = container;
            return newBullet;
        }
    }
}
