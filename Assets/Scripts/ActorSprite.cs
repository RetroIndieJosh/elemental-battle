using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class ActorSprite : MonoBehaviour
{
    public bool IsSelected {
        set {
            m_isSelected = value;
            if ( m_isSelected )
                m_startPos = transform.position;
            else transform.position = m_startPos;
        }
    }

    public Color Color {
        set { GetComponent<SpriteRenderer>().color = value; }
    }

    public Sprite Sprite {
        get { return GetComponent<SpriteRenderer>().sprite; }
        set { GetComponent<SpriteRenderer>().sprite = value; }
    }

    private bool m_isSelected = false;
    private Vector3 m_startPos = Vector3.zero;
    private float m_selectedTime = 0f;

    void Update() {
        if( m_isSelected ) {
            m_selectedTime += Time.deltaTime;
            var t = 0f;
            if ( m_selectedTime < 1f )
                t = m_selectedTime;
            else t = 1f - ( m_selectedTime - 1f );
            transform.position = Vector3.Lerp( m_startPos, m_startPos + Vector3.up, t );

            if ( m_selectedTime >= 2f )
                m_selectedTime = 0f;
        }
    }
}
