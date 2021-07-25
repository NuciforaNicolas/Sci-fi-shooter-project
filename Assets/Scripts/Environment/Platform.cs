using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float timeToDestroy, timeToChangeColor;
    [SerializeField] Material material;
    [SerializeField] Color colorA, colorB;
    float timerDestroy = 0, timerChangeColor = 0;

    // Update is called once per frame
    void Update()
    {
        timerDestroy += Time.deltaTime / timeToDestroy;
        Debug.Log("Timer Destroy: " + timerDestroy);
        if(timerDestroy >= timeToDestroy)
        {
            Debug.Log("Destroing platform");
            DestroyPlatform();
        }
    }

    void DestroyPlatform()
    {
         while(true)
        {
            timerChangeColor += Time.deltaTime / timeToChangeColor;
            material.color = Color.Lerp(colorA, colorB, timerChangeColor);
            if (timerChangeColor >= timeToChangeColor) Destroy(gameObject);
        }
    }
}
