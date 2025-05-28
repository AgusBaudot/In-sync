using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth _attackable;
    [Range(0.01f, 0.2f)] [SerializeField] private float _timeShowed;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        gameObject.SetActive(false);
        _attackable.OnAttackedEvent += ShowUI;
    }

    private void ShowUI()
    {
        gameObject.SetActive(true);
        _anim.SetTrigger("Damaged");
        StartCoroutine(ShowUICooldown());
    }

    private IEnumerator ShowUICooldown()
    {
        yield return Helpers.GetWait(0.16f);
        gameObject.SetActive(false);
    }
}
