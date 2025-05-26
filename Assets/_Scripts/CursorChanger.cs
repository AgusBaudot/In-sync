using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    [SerializeField] private Texture2D _cursorTexture;
    [SerializeField] private Vector2 _hotspot;
    [SerializeField] private CursorMode _cursorMode = CursorMode.Auto;

    private void Start()
    {
        Cursor.SetCursor(_cursorTexture, _hotspot, _cursorMode);
    }
}
