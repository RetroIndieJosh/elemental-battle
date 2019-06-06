using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using EditorGUITable;

[DisallowMultipleComponent]
public class UiManager : MonoBehaviour
{
    static public UiManager instance = null;

    [SerializeField] private Sprite m_buttonImageA = null;
    [SerializeField] private Sprite m_buttonImageB = null;
    [SerializeField] private Sprite m_buttonImageX = null;
    [SerializeField] private Sprite m_buttonImageY = null;
    [SerializeField] private Sprite m_buttonImageLB = null;

    public Sprite GetSpriteFor(ButtonControl a_button ) {
        if ( a_button == null ) return null;

        if ( a_button.displayName == "A" ) return m_buttonImageA;
        else if ( a_button.displayName == "B" ) return m_buttonImageB;
        else if ( a_button.displayName == "X" ) return m_buttonImageX;
        else if ( a_button.displayName == "Y" ) return m_buttonImageY;
        else if ( a_button.displayName == "LB" ) return m_buttonImageLB;
        return null;
    }

    private void Awake() {
        instance = this;
    }
}
