using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class ActorSprite : MonoBehaviour
{
    public bool IsAnimating { private set; get; }

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

    public void AnimateAttack( Vector3 a_targetPos ) {
        StartCoroutine( AnimateAttackCoroutine( a_targetPos ) );
    }

    private IEnumerator AnimateAttackCoroutine( Vector3 a_targetPos ) {
        IsAnimating = true;

        var timeElapsed = 0f;
        var moveTime = 0.5f;
        var halfTime = moveTime / 2f;
        var attackTime = 0.1f;

        var peakY = 3f;

        while ( timeElapsed < halfTime ) {
            timeElapsed += Time.deltaTime;
            var t = timeElapsed / halfTime;

            var pos = Vector3.Lerp( m_startPos, a_targetPos, t );

            var yt = 0f;
            if ( t < 0.5f ) yt = t * 2f;
            else yt = ( 1f - t ) * 2f;

            pos.y = Mathf.Lerp( m_startPos.y, a_targetPos.y + peakY, yt );
            transform.position = pos;

            yield return null;
        }

        yield return new WaitForSeconds( attackTime );
        m_selectedTime -= attackTime;

        timeElapsed = 0f;
        while ( timeElapsed < halfTime ) {
            timeElapsed += Time.deltaTime;
            var t = timeElapsed / halfTime;

            var pos = Vector3.Lerp( a_targetPos, m_startPos, t );

            var yt = 0f;
            if ( t < 0.5f ) yt = t * 2f;
            else yt = ( 1f - t ) * 2f;

            pos.y = Mathf.Lerp( m_startPos.y, a_targetPos.y + peakY, yt );
            transform.position = pos;

            yield return null;
        }

        transform.position = m_startPos;

        IsAnimating = false;
    }

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
