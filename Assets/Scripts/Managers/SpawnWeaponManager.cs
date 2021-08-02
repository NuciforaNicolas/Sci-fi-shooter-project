using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnWeaponManager : MonoBehaviour
{
    [Header("Object Pooling Properties")]
    [SerializeField] GameObject drone;
    [SerializeField] GameObject pistol, smg, mg, shotgun, sniper, rocket, granade; //aggiungere melee
    [SerializeField] int  pistolSize, smgSize, mgSize, shotgunSize, sniperSize, rocketSize, granadeSize;
    [SerializeField] Transform droneContainer, pistolContainer, smgContainer, mgContainer, shotgunContainer, sniperContainer, rocketContainer, granadeContainer;
    [Header("Spawn Properties")]
    [SerializeField] List<Transform> spawnPositions;
    [SerializeField] float minTimeSpawn, maxTimeSpawn, minSpawnDegree, maxSpawnDegree;


    Dictionary<string, List<GameObject>> weaponsPools;
    Dictionary<string, Transform> weaponContainer;
    float spawnDegree, spawnTime;
    GameObject droneInstance;
    public bool canSpawn { get; set; }
    public static SpawnWeaponManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        weaponsPools = new Dictionary<string, List<GameObject>>();
        weaponContainer = new Dictionary<string, Transform>();
        GenerateWeaponPool();
        CreateDrone();
        spawnTime = Random.Range(minTimeSpawn, maxTimeSpawn);
        canSpawn = true;
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
        Transform spawnPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
        droneInstance.transform.position = spawnPos.position;
        droneInstance.transform.rotation = spawnPos.rotation;
        var degree = Random.Range(minSpawnDegree, maxSpawnDegree);
        droneInstance.transform.Rotate(0, spawnPos.transform.rotation.y + degree,0);
        GameObject weapon = GetRandomWeapon();
        SetDroneWeapon(ref weapon);
        droneInstance.SetActive(true);
    }

    void SetDroneWeapon(ref GameObject weapon)
    {
        Transform droneWeaponSLot = droneInstance.transform.GetChild(0).transform;
        weapon.transform.parent = droneWeaponSLot;
        weapon.transform.position = droneWeaponSLot.position;
        weapon.transform.rotation = droneWeaponSLot.rotation;
        weapon.SetActive(true);
    }

    void GenerateWeaponPool()
    {
        if(pistol && smg && mg && shotgun && sniper && rocket && granade /*&& melee*/)
        {
            weaponsPools.Add(pistol.name, new List<GameObject>(pistolSize));
            weaponsPools.Add(smg.name, new List<GameObject>(smgSize));
            weaponsPools.Add(mg.name, new List<GameObject>(mgSize));
            weaponsPools.Add(shotgun.name, new List<GameObject>(shotgunSize));
            weaponsPools.Add(sniper.name, new List<GameObject>(sniperSize));
            weaponsPools.Add(rocket.name, new List<GameObject>(rocketSize));
            weaponsPools.Add(granade.name, new List<GameObject>(granadeSize));
            //aggiungere melee

            weaponContainer.Add(pistol.name, pistolContainer);
            weaponContainer.Add(smg.name, smgContainer);
            weaponContainer.Add(mg.name, mgContainer);
            weaponContainer.Add(shotgun.name, shotgunContainer);
            weaponContainer.Add(sniper.name, sniperContainer);
            weaponContainer.Add(rocket.name, rocketContainer);
            weaponContainer.Add(granade.name, granadeContainer);

            FillList(weaponsPools[pistol.name], pistolSize, pistol, pistolContainer);
            FillList(weaponsPools[smg.name], smgSize, smg, smgContainer);
            FillList(weaponsPools[mg.name], mgSize, mg, mgContainer);
            FillList(weaponsPools[shotgun.name], shotgunSize, shotgun, shotgunContainer);
            FillList(weaponsPools[sniper.name], sniperSize, sniper, sniperContainer);
            FillList(weaponsPools[rocket.name], rocketSize, rocket, rocketContainer);
            FillList(weaponsPools[granade.name], granadeSize, granade, granadeContainer);
            //aggiungere melee
        }
    }

    void FillList(List<GameObject> list, int size, GameObject obj, Transform container)
    {
        for(int i = 0; i < size; i++)
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
        while (true)
        {
            List<string> keys = Enumerable.ToList(weaponsPools.Keys);
            List<GameObject> selectedWeaponList = weaponsPools[keys[Random.Range(0, keys.Count)]];
            for (int i = 0; i < selectedWeaponList.Count; i++)
            {
                if (!selectedWeaponList[i].activeInHierarchy)
                {
                    return selectedWeaponList[i];
                }
            }
        }
    }

    public Transform GetWeaponContainer(string gunType)
    {
        return weaponContainer[gunType];
    }
}
