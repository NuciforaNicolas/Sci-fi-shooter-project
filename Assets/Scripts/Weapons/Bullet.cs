using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks
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
        if (!PhotonNetwork.IsMasterClient) return;
        Move();
    }

    void Move()
    {
        transform.Move(speed);
    }

    IEnumerator DisableCountdown()
    {
        yield return new WaitForSeconds(range);
        DeactiveBullet();
    }

    public void DeactiveBullet()
    {
        photonView.RPC(nameof(DeactiveBulletRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DeactiveBulletRPC()
    {
        gameObject.SetActive(false);
    }

    public float GetDamage() { return damage; }

    public string GetBulletType()
    {
        return bulletType.ToString();
    }

    public void ActiveBullet()
    {
        Debug.Log("Bullet: calling ActiveBulletRPC");
        photonView.RPC(nameof(ActiveBulletRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ActiveBulletRPC()
    {
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        StartCoroutine("DisableCountdown");
    }
}
