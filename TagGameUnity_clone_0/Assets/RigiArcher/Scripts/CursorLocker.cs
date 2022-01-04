using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLocker : MonoBehaviour
{
    private void Start() {
        // Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.Singleton.GameEndEvent += (r) => {
            Cursor.lockState = CursorLockMode.None;
        };
    }
}
