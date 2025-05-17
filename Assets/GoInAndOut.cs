using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoInAndOut : MonoBehaviour
{
    public static GoInAndOut Instance { get; private set; }
    public bool transitioning = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
}
