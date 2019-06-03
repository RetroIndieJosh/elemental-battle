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
            transform.rotation = Quaternion.identity;
        }
    }

    public bool IsVisible {
        get { return m_isVisible; }
        set {
            m_isVisible = value;
            if ( m_isVisible ) GetComponent<SpriteRenderer>().color = Color.white;
            else GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }

    private bool m_isVisible = true;

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
        var halfMoveTime = BattleManager.instance.AttackMoveTime / 2f;

        var peakY = 3f;

        while ( timeElapsed < halfMoveTime ) {
            timeElapsed += Time.deltaTime;
            var t = timeElapsed / halfMoveTime;

            var pos = Vector3.Lerp( m_startPos, a_targetPos, t );

            var yt = 0f;
            if ( t < 0.5f ) yt = t * 2f;
            else yt = ( 1f - t ) * 2f;

            pos.y = Mathf.Lerp( m_startPos.y, a_targetPos.y + peakY, yt );
            transform.position = pos;

            yield return null;
        }

        yield return new WaitForSeconds( BattleManager.instance.AttackStayTime );

        timeElapsed = 0f;
        while ( timeElapsed < halfMoveTime ) {
            timeElapsed += Time.deltaTime;
            var t = timeElapsed / halfMoveTime;

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

    public void Flash( Color a_color, float a_delaySec = 0f ) {
        StartCoroutine( FlashCoroutine( a_color, a_delaySec ) );
    }

    private IEnumerator FlashCoroutine( Color a_color, float a_delaySec = 0f ) {
        if ( a_delaySec > Mathf.Epsilon )
            yield return new WaitForSeconds( a_delaySec );

        if ( m_isVisible == false ) yield break;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = a_color;
        yield return new WaitForSeconds( 0.1f );

        if ( m_isVisible == false ) yield break;

        spriteRenderer.color = Color.white;
    }

    private void Start() {
        m_startPos = transform.position;
    }

    void Update() {
        if( m_isSelected )
            transform.Rotate( Vector3.up, -500f * Time.deltaTime );
    }
}
