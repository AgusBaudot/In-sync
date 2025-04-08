using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour, IAttackable
{
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _dmgTextPrefab;

    public void OnAttacked(int damageReceived)
    {
        _anim.SetTrigger((Random.value < 0.5f) ? "Right" : "Left");
        var dmgText = Instantiate(_dmgTextPrefab, transform.parent.GetChild(1));
        dmgText.GetComponent<DamageUI>().ShowDamage(damageReceived);
    }

    public void OnDeath()
    {

    }

    private IEnumerator Respawn()
    {
        yield return Helpers.GetWait(1);
    }
}
