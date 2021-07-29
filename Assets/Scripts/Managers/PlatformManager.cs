using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] float minTimeToFall, maxTimeToFall;
    [SerializeField] List<Platform> platformList;
    float timeToFall;
    bool canFall;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject stage = GameObject.Find("Stage");
        if (stage != null)
        {
            for (int i = 0; i < stage.transform.childCount; i++)
                platformList.Add(stage.transform.GetChild(i).GetComponent<Platform>());
        }
        else
            Debug.LogError("Stage not found");
        
        timeToFall = Random.Range(minTimeToFall, maxTimeToFall);
        canFall = true;
    }

    void Update()
    {
        if (canFall)
        {
            canFall = false;
            timeToFall = Random.Range(minTimeToFall, maxTimeToFall);
            StartCoroutine("StartFallCountdown");
        }
    }

    IEnumerator StartFallCountdown()
    {
        yield return new WaitForSeconds(timeToFall);
        if(platformList != null)
        {
            int index = Random.Range(0, platformList.Count);
            platformList[index].StartFalling();
            platformList.RemoveAt(index);
        }
        canFall = true;
    }
}
