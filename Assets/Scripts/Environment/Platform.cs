using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float timeToChangeColor, fallSpeed;
    [SerializeField] Color colorA, colorB;
    [SerializeField] int maxHealth;
    int currentHealth;
    Renderer renderer;
    float timerChangeColor;
    bool isDestroyed;
    float colorAddictive;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        currentHealth = maxHealth;
        colorAddictive = 1.0f / maxHealth;
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

    void DecreaseHealth()
    {
        currentHealth--;
        colorA = new Color(colorA.r, colorA.g - colorAddictive, colorA.b - colorAddictive);
        renderer.material.color = colorA;
        if (currentHealth <= 0) StartCoroutine("FallPlatform");
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
        if (other.CompareTag("DeathBorder"))
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
        if (other.gameObject.layer.Equals("Explosive"))
        {
            DecreaseHealth();
        }
    }
}
