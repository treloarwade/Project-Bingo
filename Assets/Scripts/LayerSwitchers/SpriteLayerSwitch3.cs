using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpriteLayerSwitch3 : MonoBehaviour
{
    public Transform bingo;
    public Collider2D above;
    public Collider2D above2;
    public Collider2D below;
    public Collider2D below2;
    public float height;

    private Coroutine searchCoroutine;

    private void Awake()
    {
        FindLocalPlayer();
        height = transform.position.y;
    }

    private void OnDestroy()
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
        }
    }

    public void FindLocalPlayer()
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
        }
        searchCoroutine = StartCoroutine(FindLocalPlayerRoutine());
    }

    private IEnumerator FindLocalPlayerRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f)); // Stagger searches

        // First try local player manager if available
        if (LocalPlayerManager.Instance != null && LocalPlayerManager.Instance.LocalPlayer != null)
        {
            bingo = LocalPlayerManager.Instance.LocalPlayer.transform;
            yield break;
        }

        // Fallback to tag search
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (player.GetComponent<NetworkObject>()?.IsOwner ?? false)
            {
                bingo = player.transform;
                break;
            }
        }
    }

    void Update()
    {
        if (bingo == null) return;

        bool isBelow = bingo.position.y < height;

        if (below != null) below.enabled = isBelow;
        if (above != null) above.enabled = !isBelow;
        if (below != null) below2.enabled = isBelow;
        if (above != null) above2.enabled = !isBelow;
    }
}
