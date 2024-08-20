using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ButtonHover : MonoBehaviour
{
    public Color highlight;
    private TMP_Text _text;
    private Color _default;

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
        _default = _text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = highlight;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = _default;
    }
}
