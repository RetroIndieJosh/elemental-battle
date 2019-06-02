using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent( typeof( TextMeshPro ) )]
public class FloatText : MonoBehaviour
{
    [SerializeField] private bool m_fade = false;
    [SerializeField] private bool m_shrink = false;
    [SerializeField] private Vector3 m_movement = Vector3.up;
    [SerializeField] private float m_stayTimeSec = 1f;

    private Vector3 m_startPos = Vector3.zero;
    private TextMeshPro m_textMesh = null;
    private float m_timeElapsed = 0f;
    private bool m_moving = false;

    public string Text { set { m_textMesh.text = value; } }

    public void Show() {
        m_moving = true;
        m_timeElapsed = 0f;
        m_startPos = transform.position;

        var color = m_textMesh.color;
        color.a = 0f;
        m_textMesh.color = color;
    }

    private void Awake() {
        m_textMesh = GetComponent<TextMeshPro>();
    }

    private void Start() {
        Hide();
    }

    private void Update() {
        if ( m_moving == false ) return;

        m_timeElapsed += Time.deltaTime;

        var t = m_timeElapsed / m_stayTimeSec;
        transform.position = Vector3.Lerp( m_startPos, m_startPos + m_movement, t );

        if ( m_fade ) {
            var color = m_textMesh.color;
            color.a = 1f - t;
            m_textMesh.color = color;
        }

        if( m_shrink )
            m_textMesh.transform.localScale = Vector3.one * ( 1f - t );

        if ( m_timeElapsed >= m_stayTimeSec ) {
            m_moving = false;
            Hide();
        }
    }

    private void Hide() {
        var color = m_textMesh.color;
        color.a = 0f;
        m_textMesh.color = color;
    }
}
