using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private void OnDestroy()
    {
        GameManager.instance.keyItem = true;
    }
}
