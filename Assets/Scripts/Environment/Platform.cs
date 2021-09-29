using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Platform : MonoBehaviourPunCallbacks, IPunObservable
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
        if (!photonView) Debug.LogError("Platform: Photonview component missing");
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
        if (other.gameObject.layer.Equals("Explosive"))
        {
            DecreaseHealth();
        }
    }

    public void DestroyPlatform()
    {
        isDestroyed = true;
        photonView.RPC(nameof(DestroyPlatformRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyPlatformRPC()
    {
        Debug.Log("DestroyPlatformRPC called: destroying platform");
        gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            var tmpColor = new Vector3(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b);
            stream.SendNext(tmpColor);
        }
        else
        {
            var tmpColor = (Vector3) stream.ReceiveNext();
            renderer.material.color = new Color(tmpColor.x, tmpColor.y, tmpColor.z, 1f);
        }
    }
}
