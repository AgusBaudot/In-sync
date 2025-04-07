using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour, IAttackable
{
    private Animator _anim;
    [SerializeField] private GameObject _dmgTextPrefab;

    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    public void OnAttacked(int damageReceived)
    {
        _anim.SetTrigger((Random.value < 0.5f) ? "Right" : "Left");
        var dmgText = Instantiate(_dmgTextPrefab, transform.GetChild(1));
        dmgText.GetComponent<DamageUI>().ShowDamage(damageReceived);
        //GetComponent<DamageUI>().ShowDamage(damageReceived);
        //if (TryGetComponent<DamageUI>(out DamageUI script))
        //{
        //    script.ShowDamage();
        //}
        //_respawnCo = StartCoroutine(Respawn());
    }

    public void OnDeath()
    {

    }

    private IEnumerator Respawn()
    {
        yield return Helpers.GetWait(1);
        //_respawnCo = null;
    }
}
