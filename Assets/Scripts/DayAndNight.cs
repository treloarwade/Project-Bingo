using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DayAndNight : NetworkBehaviour
{
    public NetworkVariable<bool> isNight = new NetworkVariable<bool>(false); // Replaces [SyncVar]
    private bool turningToLight;
    public static DayAndNight Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public override void OnNetworkSpawn() // Replaces OnStartClient
    {
        base.OnNetworkSpawn();
        SyncVisuals();
    }
    public void SyncVisuals()
    {
        if (!turningToLight)
        {
            if (isNight.Value)
            {
                ForceNightVisuals();
            }
            else
            {
                ForceDayVisuals();
            }
        }

    }
    public void SyncPlayer()
    {
        // Only run on clients
        // Get local player object safely
        if (NetworkManager.Singleton == null ||
            NetworkManager.Singleton.SpawnManager == null ||
            !NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning("Could not find local player object for sync");
            return;
        }

        GameObject playerObject = NetworkManager.Singleton.SpawnManager
            .GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).gameObject;

        // Get all sprite renderers (player + children)
        SpriteRenderer[] allSprites = playerObject.GetComponentsInChildren<SpriteRenderer>(true);
        Color targetColor = isNight.Value ?
            new Color(0.5f, 0.5f, 0.5f, 1f) :
            new Color(1f, 1f, 1f, 1f);

        // Apply to all sprites
        foreach (SpriteRenderer sprite in allSprites)
        {
            if (sprite != null) // Extra null check for safety
            {
                sprite.color = new Color(targetColor.r, targetColor.g, targetColor.b, sprite.color.a);
            }
        }

        Debug.Log($"Synced {allSprites.Length} sprites for player {NetworkManager.Singleton.LocalClientId}");
    }
    [ServerRpc(RequireOwnership = false)] // Replaces [Command]
    public void ToggleDayNightServerRpc()
    {
        if (isNight.Value)
        {
            DayClientRpc();
        }
        else
        {
            NightClientRpc();
        }
    }
    [ClientRpc]
    public void NightClientRpc()
    {
        isNight.Value = true;
        StopAllCoroutines();
        StartCoroutine(TurntoNight(10f));
    }

    [ClientRpc]
    public void DayClientRpc()
    {
        isNight.Value = false;
        StopAllCoroutines();
        StartCoroutine(EndNight());
    }
    public void ToggleDayNight()
    {
        ToggleDayNightServerRpc();
    }

    private void ForceNightVisuals()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            Color currentColor = sprite.color;
            Color grey = new Color(0.5f, 0.5f, 0.5f, currentColor.a);
            if (sprite.color != grey)
            {
                sprite.color = grey;

            }
        }

        if (NightLights.Instance == null)
            NightLights.Instance.ChangeColor(0.80f);
        NightLights.Instance.TurnLightsOnForAllMushroomHouses();
        StartCoroutine(DoubleCheck());
    }
    private IEnumerator DoubleCheck()
    {
        yield return null;

        //yield return new WaitForSeconds(0.05f);
        NightLights.Instance.ChangeColor(0.80f);
        NightLights.Instance.TurnLightsOnForAllMushroomHouses();

    }

    private void ForceDayVisuals()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            Color currentColor = sprite.color;
            sprite.color = new Color(1f, 1f, 1f, currentColor.a);
        }

        if (NightLights.Instance == null)

        NightLights.Instance.TurnLightsOffForAllMushroomHouses();
    }

    public void TurnToNightIf()
    {
        if (!isNight.Value && NetworkManager.Singleton.IsServer) // Only server can initiate this
        {
            isNight.Value = true;
            NightClientRpc();
        }
    }
    IEnumerator DayTime()
    {
        yield return new WaitForSeconds(670f);
        StartCoroutine(TurntoNight(10f));
        yield return null;
    }
    IEnumerator TurntoNight(float speed)
    {
        turningToLight = true;
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        isNight.Value = true;

        float[] greyLevels = { 0.95f, 0.90f, 0.85f, 0.80f, 0.75f, 0.70f, 0.65f, 0.60f, 0.55f, 0.50f };

        foreach (float greyLevel in greyLevels)
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                Color currentColor = sprite.color;
                sprite.color = new Color(greyLevel, greyLevel, greyLevel, currentColor.a);
            }
            yield return new WaitForSeconds(5f / speed);
        }
        turningToLight = false;
        StartCoroutine(NightTime());
        yield return null;
    }

    IEnumerator NightTime()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            Color currentColor = sprite.color;
            sprite.color = new Color(0.5f, 0.5f, 0.5f, currentColor.a);
        }
        NightLights.Instance.ChangeColor(0.80f);
        NightLights.Instance.TurnLightsOnForAllMushroomHouses();
        yield return new WaitForSeconds(670f);
        StartCoroutine(EndNight());
        yield return null;
    }
    IEnumerator EndNight()
    {
        NightLights.Instance.TurnLightsOffForAllMushroomHouses();
        StartCoroutine(TurntoDay(10f));
        yield return null;
    }
    IEnumerator TurntoDay(float speed)
    {
        turningToLight = true;

        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        isNight.Value = false;

        float[] greyLevels = { 0.55f, 0.60f, 0.65f, 0.70f, 0.75f, 0.80f, 0.85f, 0.90f, 0.95f, 1.0f };

        foreach (float greyLevel in greyLevels)
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                Color currentColor = sprite.color;
                sprite.color = new Color(greyLevel, greyLevel, greyLevel, currentColor.a);
            }
            yield return new WaitForSeconds(5f / speed);
        }
        turningToLight = false;

        StartCoroutine(DayTime());
        yield return null;
    }


}

