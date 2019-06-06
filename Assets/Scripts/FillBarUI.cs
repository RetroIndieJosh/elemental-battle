using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO also handle vertical fill
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class FillBarUI : MonoBehaviour
{
    [SerializeField] private Image m_fillImage = null;
    [SerializeField, Range(0f, 1f)] private float m_fillPercent = 1f;

    public Color FillColor {  set { m_fillImage.color = value; } }
    public float FillPercent {
        get { return m_fillPercent; }
        set { m_fillPercent = Mathf.Clamp( value, 0f, 1f ); } }

    private RectTransform m_rectTransform;

    private void Awake() {
        m_rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        var widthMax = m_rectTransform.sizeDelta.x;
        var width = m_fillPercent * widthMax;

        var height = m_fillImage.GetComponent<RectTransform>().sizeDelta.y;
        m_fillImage.GetComponent<RectTransform>().sizeDelta = new Vector2( width, height );
    }
}
