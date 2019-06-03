using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private void Update() {
        if ( Gamepad.current.aButton.wasPressedThisFrame || Keyboard.current.anyKey.isPressed )
            SceneManager.LoadScene( "main" );
    }
}
