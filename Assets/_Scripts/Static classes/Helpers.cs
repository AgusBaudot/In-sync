using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0)); //Store new isometric navigation in matrix.
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);

    private static Camera _camera;
    public static Camera Camera //Main camera reference.
    {
        get
        {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }

    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWait(float time) //Non-allocating waitforseconds for garbage collector efficiency.
    {
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;
    public static bool IsOverUi() //Get whether mouse is over UI or not. 
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }

    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element) //Returns world position of canvas.
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(element, element.position, Camera, out var result);
        return result;
    }

    public static void DeleteChildren(this Transform t) //Destroy all child gameObjects.
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }
}
