using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoordinateChecker : MonoBehaviour
{
    public Text text;
    public GameObject bingo;
    public void CheckCoordinates()
    {
        Debug.Log(bingo.transform.position);
        text.text = bingo.transform.position.ToString();
    }
}
