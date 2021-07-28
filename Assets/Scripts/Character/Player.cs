using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float maxHealth;
    float health;
    public static Player instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        health = maxHealth;
    }


}
