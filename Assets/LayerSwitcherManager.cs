using System.Collections.Generic;
using UnityEngine;

public class LayerSwitcherManager : MonoBehaviour
{
    public static LayerSwitcherManager Instance;

    private List<SpriteLayerSwitcher2> allSwitchers = new List<SpriteLayerSwitcher2>();
    private bool playerReady;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterSwitcher(SpriteLayerSwitcher2 switcher)
    {
        if (!allSwitchers.Contains(switcher))
        {
            allSwitchers.Add(switcher);

            // If player is already ready, refresh immediately
            if (playerReady)
            {
                switcher.FindLocalPlayer();
            }
        }
    }

    public void UnregisterSwitcher(SpriteLayerSwitcher2 switcher)
    {
        if (allSwitchers.Contains(switcher))
        {
            allSwitchers.Remove(switcher);
        }
    }

    // Call this when local player is ready/joined
    public void OnLocalPlayerReady()
    {
        playerReady = true;
        RefreshAllSwitchers();
    }

    private void RefreshAllSwitchers()
    {
        foreach (var switcher in allSwitchers)
        {
            if (switcher != null)
            {
                switcher.FindLocalPlayer();
            }
        }
    }
}