using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed, range, damage;

    private void OnBecameVisible()
    {
        StartCoroutine("DisableCountdown");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    IEnumerator DisableCountdown()
    {
        yield return new WaitForSeconds(range);
        gameObject.SetActive(false);
    }

    public float GetDamage() { return damage; }
}
