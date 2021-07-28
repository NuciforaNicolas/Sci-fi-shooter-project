using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;

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
}
