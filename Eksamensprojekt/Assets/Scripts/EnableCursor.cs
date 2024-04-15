using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("EnableCursorr", 1);

    }

    void EnableCursorr()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
