using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnWeaponManager : MonoBehaviour
{
    [Header("Object Pooling Properties")]
    [SerializeField] GameObject drone;
    [SerializeField] GameObject pistol, smg, mg, shotgun, sniper, rocket, granade; //aggiungere melee
    [SerializeField] int pistolSize, smgSize, mgSize, shotgunSize, sniperSize, rocketSize, granadeSize;
    [SerializeField] Transform droneContainer, pistolContainer, smgContainer, mgContainer, shotgunContainer, sniperContainer, rocketContainer, granadeContainer;
    [Header("Spawn Properties")]
    [SerializeField] List<Transform> spawnPositions;
    [SerializeField] float minTimeSpawn, maxTimeSpawn, minSpawnDegree, maxSpawnDegree;


    Dictionary<string, List<GameObject>> weaponsPools; //object pool delle armi
    Dictionary<string, Transform> weaponContainer; //contiene le transform dei contenitori delle armi
    Dictionary<string, int> weaponSpawnedCounter; //Indica quante armi sono state spawnate
    List<string> weaponPoolKeys;
    float spawnDegree, spawnTime;
    public int slotsFull;
    GameObject droneInstance;
    public bool canSpawn { get; set; }
    public static SpawnWeaponManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        weaponsPools = new Dictionary<string, List<GameObject>>();
        weaponContainer = new Dictionary<string, Transform>();
        weaponSpawnedCounter = new Dictionary<string, int>();
        GenerateWeaponPool();
        CreateDrone();
        spawnTime = Random.Range(minTimeSpawn, maxTimeSpawn);
        canSpawn = true;
        slotsFull = 0;
    }

    private void Update()
    {
        if (canSpawn)
        {
            canSpawn = false;
            StartCoroutine("SpawnDrone");
        }
    }

    IEnumerator SpawnDrone()
    {
        yield return new WaitForSeconds(spawnTime);
        GameObject weapon = GetRandomWeapon();
        if (weapon != null)
        {
            Transform spawnPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            droneInstance.transform.position = spawnPos.position;
            droneInstance.transform.rotation = spawnPos.rotation;
            var degree = Random.Range(minSpawnDegree, maxSpawnDegree);
            droneInstance.transform.Rotate(0, spawnPos.transform.rotation.y + degree, 0);
            SetDroneWeapon(ref weapon);
            droneInstance.SetActive(true);
        }
    }

    void SetDroneWeapon(ref GameObject weapon)
    {
        Transform droneWeaponSLot = droneInstance.transform.GetChild(0).transform;
        weapon.transform.parent = droneWeaponSLot;
        weapon.transform.position = droneWeaponSLot.position;
        weapon.transform.rotation = droneWeaponSLot.rotation;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.SetActive(true);
    }

    void GenerateWeaponPool()
    {
        if (pistol && smg && mg && shotgun && sniper && rocket && granade /*&& melee*/)
        {
            //Aggiunge le liste per ogni arma
            weaponsPools.Add(pistol.name, new List<GameObject>(pistolSize));
            weaponsPools.Add(smg.name, new List<GameObject>(smgSize));
            weaponsPools.Add(mg.name, new List<GameObject>(mgSize));
            weaponsPools.Add(shotgun.name, new List<GameObject>(shotgunSize));
            weaponsPools.Add(sniper.name, new List<GameObject>(sniperSize));
            weaponsPools.Add(rocket.name, new List<GameObject>(rocketSize));
            weaponsPools.Add(granade.name, new List<GameObject>(granadeSize));
            //aggiungere melee

            //aggiunge i container per ogni mappa
            weaponContainer.Add(pistol.name, pistolContainer);
            weaponContainer.Add(smg.name, smgContainer);
            weaponContainer.Add(mg.name, mgContainer);
            weaponContainer.Add(shotgun.name, shotgunContainer);
            weaponContainer.Add(sniper.name, sniperContainer);
            weaponContainer.Add(rocket.name, rocketContainer);
            weaponContainer.Add(granade.name, granadeContainer);

            //aggiunge i contatori per ogni arma
            weaponSpawnedCounter.Add(pistol.name, pistolSize);
            weaponSpawnedCounter.Add(smg.name, smgSize);
            weaponSpawnedCounter.Add(mg.name, mgSize);
            weaponSpawnedCounter.Add(shotgun.name, shotgunSize);
            weaponSpawnedCounter.Add(sniper.name, sniperSize);
            weaponSpawnedCounter.Add(rocket.name, rocketSize);
            weaponSpawnedCounter.Add(granade.name, granadeSize);

            //riempie le liste com le proprie armi
            FillWeaponList(weaponsPools[pistol.name], pistolSize, pistol, pistolContainer);
            FillWeaponList(weaponsPools[smg.name], smgSize, smg, smgContainer);
            FillWeaponList(weaponsPools[mg.name], mgSize, mg, mgContainer);
            FillWeaponList(weaponsPools[shotgun.name], shotgunSize, shotgun, shotgunContainer);
            FillWeaponList(weaponsPools[sniper.name], sniperSize, sniper, sniperContainer);
            FillWeaponList(weaponsPools[rocket.name], rocketSize, rocket, rocketContainer);
            FillWeaponList(weaponsPools[granade.name], granadeSize, granade, granadeContainer);
            //aggiungere melee

            weaponPoolKeys = Enumerable.ToList(weaponsPools.Keys);
        }
    }

    void FillWeaponList(List<GameObject> list, int size, GameObject obj, Transform container)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject newObj = Instantiate<GameObject>(obj);
            newObj.SetActive(false);
            list.Add(newObj);
            newObj.transform.parent = container;
        }
    }

    void CreateDrone()
    {
        droneInstance = Instantiate<GameObject>(drone);
        droneInstance.SetActive(false);
        droneInstance.transform.parent = droneContainer.transform;
    }

    public GameObject GetRandomWeapon()
    {
        string selectedWeapon;
        List<GameObject> selectedWeaponList;
        while (slotsFull <= weaponPoolKeys.Count())
        {
            selectedWeapon = weaponPoolKeys[Random.Range(0, weaponPoolKeys.Count)];
            if (weaponSpawnedCounter[selectedWeapon] <= 0) { slotsFull++; continue; }
            selectedWeaponList = weaponsPools[selectedWeapon];
            for (int i = 0; i < selectedWeaponList.Count; i++)
            {
                if (!selectedWeaponList[i].activeInHierarchy)
                {
                    if (weaponSpawnedCounter[selectedWeapon] == 1) slotsFull -= Mathf.Clamp(slotsFull, 0, 1);
                    weaponSpawnedCounter[selectedWeapon]--;
                    return selectedWeaponList[i];
                }
                else
                {
                    weaponSpawnedCounter[selectedWeapon]++;
                }
            }
        }
        return null;
    }

    public Transform GetWeaponContainer(string gunType)
    {
        return weaponContainer[gunType];
    }
}
