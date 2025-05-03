using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpriteLayerSwitcher2 : MonoBehaviour
{
    public Transform bingo;
    public Collider2D above;
    public Collider2D below;
    public float height;

    private Coroutine searchCoroutine;

    private void Awake()
    {
        LayerSwitcherManager.Instance?.RegisterSwitcher(this);
        FindLocalPlayer();
        height = transform.position.y;
    }

    private void OnDestroy()
    {
        if (LayerSwitcherManager.Instance != null)
        {
            LayerSwitcherManager.Instance.UnregisterSwitcher(this);
        }

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
    }
}