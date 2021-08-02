using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float maxHealth;
    float health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("DeathBorder"))
        {
            gameObject.SetActive(false);
        }
    }
}
