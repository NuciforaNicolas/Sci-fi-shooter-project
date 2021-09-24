using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed, range, damage;

    public enum BulletType
    {
        PistolBullet, SMGBullet, MGBullet, ShotgunBullet, SniperBullet, RocketBullet, GranadeBullet
    }
    [SerializeField] BulletType bulletType;

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Move(speed);
    }

    IEnumerator DisableCountdown()
    {
        yield return new WaitForSeconds(range);
        gameObject.SetActive(false);
    }

    public float GetDamage() { return damage; }

    public string GetBulletType()
    {
        return bulletType.ToString();
    }

    private void OnEnable()
    {
        StartCoroutine("DisableCountdown");
    }
}
