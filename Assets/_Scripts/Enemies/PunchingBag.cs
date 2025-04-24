using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBag : MonoBehaviour, IAttackable
{
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _dmgTextPrefab;

    public void OnAttacked(int damageReceived)
    {
        _anim.SetTrigger((Random.value < 0.5f) ? "Right" : "Left"); //Set trigger flag on right or left at random.
        var dmgText = Instantiate(_dmgTextPrefab, transform.parent.GetChild(1)); //Instantiate damage text.
        dmgText.GetComponent<DamageUI>().ShowDamage(damageReceived); //Call show damage method with damage received from player.
    }

    public void OnDeath()
    {

    }
}
