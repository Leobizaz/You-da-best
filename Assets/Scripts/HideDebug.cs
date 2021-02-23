using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideDebug : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
