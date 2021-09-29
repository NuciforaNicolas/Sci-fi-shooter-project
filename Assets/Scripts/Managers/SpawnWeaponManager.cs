using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Managers
{
    public class SpawnWeaponManager : MonoBehaviourPunCallbacks
    {
        [Header("Object Pooling Properties")]
        [SerializeField] GameObject drone;
        [SerializeField] Pool[] pools;
        [Header("Spawn Properties")]
        [SerializeField] List<Transform> spawnPositions;
        [SerializeField] float minTimeSpawn, maxTimeSpawn, minSpawnDegree, maxSpawnDegree;
        [SerializeField] string dronePrefabPath, weaponPrefabPath;


        Dictionary<string, Pool> weaponsPools; //object pool delle armi
        [SerializeField] Transform weaponContainer, droneContainer; //contiene le transform dei contenitori delle armi
        Dictionary<string, int> weaponSpawnedCounter; //Indica quante armi sono state spawnate
        float spawnDegree, spawnTime;
        public int slotsFull;
        GameObject droneInstance;
        [SerializeField] bool canSpawn;
        public static SpawnWeaponManager instance;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            weaponsPools = new Dictionary<string, Pool>();
            weaponSpawnedCounter = new Dictionary<string, int>();
            GenerateWeaponPool();
            CreateDrone();
            spawnTime = Random.Range(minTimeSpawn, maxTimeSpawn);
            canSpawn = true;
            slotsFull = 0;
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (canSpawn)
            {
                canSpawn = false;
                StartCoroutine(SpawnDroneCoroutine());
            }
        }

        IEnumerator SpawnDroneCoroutine()
        {
            yield return new WaitForSeconds(spawnTime);
            SpawnDrone();
        }

        void SpawnDrone()
        {
            GameObject weapon = GetRandomWeapon();
            if (weapon != null)
            {
                Transform spawnPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
                droneInstance.transform.position = spawnPos.position;
                droneInstance.transform.rotation = spawnPos.rotation;
                var degree = Random.Range(minSpawnDegree, maxSpawnDegree);
                droneInstance.transform.Rotate(0, spawnPos.transform.rotation.y + degree, 0);
                SetDroneWeapon(ref weapon);
                droneInstance.GetComponent<Drone>().ActiveDrone();
            }
        }

        void SetDroneWeapon(ref GameObject weapon)
        {
            Transform droneWeaponSLot = droneInstance.transform.GetChild(0).transform;
            weapon.transform.parent = droneWeaponSLot;
            weapon.transform.position = droneWeaponSLot.position;
            weapon.transform.rotation = droneWeaponSLot.rotation;
            weapon.GetComponent<Weapon>().ActiveWeapon();
        }

        void GenerateWeaponPool()
        {
            for(int i = 0; i < pools.Length; i++)
            {
                for(int j = 0; j < pools[i].listSize; j++)
                {
                    GameObject weapon = PhotonNetwork.Instantiate(Path.Combine(weaponPrefabPath, pools[i].prefab.name), Vector3.zero, Quaternion.identity, 0);
                    weapon.GetComponent<Weapon>().DeactiveWeapon();
                    pools[i].poolList.Add(weapon);
                    weapon.transform.parent = weaponContainer;
                }
                weaponSpawnedCounter.Add(pools[i].poolName, pools[i].listSize);
                weaponsPools.Add(pools[i].poolName, pools[i]);
            }
        }

        void CreateDrone()
        {
            droneInstance = PhotonNetwork.Instantiate(Path.Combine(dronePrefabPath, drone.name), Vector3.zero, Quaternion.identity, 0);
            droneInstance.GetComponent<Drone>().DeactiveDrone();
            droneInstance.transform.parent = droneContainer;
        }

        public GameObject GetRandomWeapon()
        {
            string selectedWeapon;
            while (slotsFull < weaponsPools.Count()) // Se tutte le armi sono state spawnate, evito di cercare armi
            {
                selectedWeapon = weaponsPools.ElementAt(Random.Range(0, weaponsPools.Count)).Key; //Restituisce la chiave di un weaponPool randomico
                if (weaponSpawnedCounter[selectedWeapon] == 0) continue; // Se il counter è 0, significa che ho raggiunto il limite massimo dell'arma selezionata in gioco 
                var weaponList = weaponsPools[selectedWeapon].poolList;
                for (int i = 0; i < weaponList.Count; i++)
                {
                    if (!weaponList[i].activeInHierarchy)
                    {
                        DecreaseWeaponCounter(selectedWeapon); //Decremento il contatore di armi selezionato, in modo da spawnarne un numero limitato
                        return weaponList[i];
                    }

                    if (weaponSpawnedCounter[selectedWeapon] == 0)
                    {
                        IncreaseWeaponCounter(selectedWeapon); 
                    }
                }
            }
            return null;
        }

        public void IncreaseWeaponCounter(string weaponType)
        {
            if (weaponSpawnedCounter[weaponType] == 0)
            {
                slotsFull--;
                if (slotsFull == weaponsPools.Count() - 1 && !canSpawn)
                {
                    canSpawn = true;
                    Debug.Log("Can spawn new weapon");
                }
            }
            weaponSpawnedCounter[weaponType]++;
        }

        public void DecreaseWeaponCounter(string weaponType)
        {
            weaponSpawnedCounter[weaponType]--;
            if (weaponSpawnedCounter[weaponType] == 0)
            {
                slotsFull++;
            }
        }

        public Transform GetWeaponContainer()
        {
            return weaponContainer;
        }

        public bool GetCanSpawn()
        {
            return canSpawn;
        }

        public void SetCanSpawn(bool canSpawn)
        {
            this.canSpawn = canSpawn;
        }
    }
}
