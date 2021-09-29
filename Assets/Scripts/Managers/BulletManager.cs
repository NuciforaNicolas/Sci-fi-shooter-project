using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Managers{
    public class BulletManager : MonoBehaviourPunCallbacks
    {
        public static BulletManager instance;
        [SerializeField] Pool[] pools;
        [SerializeField] Transform container;
        Dictionary<string, Pool> bulletPools;
        [SerializeField] string bulletPrefabPath;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            bulletPools = new Dictionary<string, Pool>();
            GenerateBulletPool();
        }

        void GenerateBulletPool()
        {
            for(int i = 0; i < pools.Length; i++){
                for(int j = 0; j < pools[i].listSize; j++){
                    GameObject bullet = PhotonNetwork.Instantiate(Path.Combine(bulletPrefabPath, pools[i].prefab.name), Vector3.zero, Quaternion.identity, 0);
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
            GameObject newBullet = PhotonNetwork.Instantiate(Path.Combine(bulletPrefabPath, bulletPools[poolName].prefab.name), Vector3.zero, Quaternion.identity, 0);
            bulletPools[poolName].poolList.Add(newBullet);
            newBullet.transform.parent = container;
            return newBullet;
        }
    }
}
