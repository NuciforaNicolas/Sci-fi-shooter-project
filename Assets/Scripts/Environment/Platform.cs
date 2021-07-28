using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float minTimeToFall, maxTimeToFall, timeToChangeColor, fallSpeed;
    [SerializeField] Color colorA, colorB;
    Renderer renderer;
    float timeToFall, timerFall, timerChangeColor;
    bool isDestroyed;

    private void Awake()
    {
        timeToFall = Random.Range(minTimeToFall, maxTimeToFall);
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Start()
    {
        StartCoroutine("StartFallCountdown");
    }

    IEnumerator StartFallCountdown()
    {
        yield return new WaitForSeconds(timeToFall);
        StartCoroutine("LerpColorPlatform");
    }

    IEnumerator LerpColorPlatform()
    {
        timerChangeColor = 0;
         while (true)
        {
            timerChangeColor += Time.deltaTime;
            renderer.material.color = Color.Lerp(colorA, colorB, timerChangeColor / timeToChangeColor);
            yield return null;
            if (timerChangeColor >= timeToChangeColor)
            {
                StartCoroutine("FallPlatform");
                yield break;
            }
        }
    }

    IEnumerator FallPlatform()
    {
        while (!isDestroyed)
        {
            transform.position -= Vector3.up * fallSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("DeathBorder"))
        {
            Debug.Log("Destroyed");
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
