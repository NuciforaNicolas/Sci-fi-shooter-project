using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float timeToChangeColor, fallSpeed;
    [SerializeField] Color colorA, colorB;
    Renderer renderer;
    float timerChangeColor;
    bool isDestroyed;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void StartFalling()
    {
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
