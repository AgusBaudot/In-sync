using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class FlashWhiteAndBlack : MonoBehaviour
{
    [SerializeField] private Material _whiteMat, _blackMat;
    private Material _normalMaterial;
    private Renderer _renderer;

    private void Start()
    {
        transform.parent.parent.TryGetComponent(out IAttackable attackableInterface);
        if (attackableInterface != null)
            attackableInterface.OnAttackedEvent += StartFlash;
        else
        {
            transform.parent.parent.parent.TryGetComponent(out IAttackable otherAttackableInterface);
            otherAttackableInterface.OnAttackedEvent += StartFlash;
        }
            _renderer = GetComponent<Renderer>();
        _normalMaterial = _renderer.material;
    }

    private void StartFlash()
    {
        if (_renderer.gameObject.transform.parent.gameObject.activeSelf) //Check if parent gameobject is active.
            StartCoroutine(FlashCooldown());
    }

    private IEnumerator FlashCooldown()
    {
        _renderer.material = _whiteMat; //Instantly when hit.
        yield return Helpers.GetWait(0.08f);
        _renderer.material = _blackMat; //Start hit animation at the same time.
        yield return Helpers.GetWait(0.08f);
        _renderer.material = _normalMaterial; //End hit animation shortly after changing back to normal.
    }

    public void ResetMaterials()
    {
        _renderer.material = _normalMaterial;
    }
}
