using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaveBattle : MonoBehaviour
{
    public Button leaveButton;

    // This method will be called when the leave button is clicked
    public void Leave()
    {
        // Load the "SampleScene" when the button is clicked
        Loader.Load(Loader.Scene.SampleScene);
    }
}

