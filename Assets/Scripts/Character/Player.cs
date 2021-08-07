using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float maxHealth;
    Rigidbody rigidbody;
    float health;
    bool isStunned;

    // Start is called before the first frame update
    void Awake()
    {
        health = maxHealth;
        rigidbody = GetComponent<Rigidbody>();
        isStunned = false;
    }

    public void Heal(float healValue)
    {
        if (health >= maxHealth) return;

        health += healValue;
        if (health > maxHealth) health = maxHealth;
    }

    public void GetDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            GetStunned();
        }
    }

    void GetStunned()
    {
        if (!isStunned)
        {
            isStunned = true;
            InputManager.instance.SetEnableControl(false);
            rigidbody.isKinematic = true;
            //TO REMOVE
            Debug.Log("STUNNED");
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
        }
    }

    public void Unstun()
    {
        isStunned = false;
        InputManager.instance.SetEnableControl(true);
        rigidbody.isKinematic = false;
        Debug.Log("UNSTUNNED");
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(0, 255, 0);
    }

    public bool GetIsStunned() { return isStunned; }

    public void SetIsStunned(bool isStunned) { this.isStunned = isStunned; }
}
