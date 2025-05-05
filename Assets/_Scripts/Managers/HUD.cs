using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    //private bool _chip, _overcharged;
    private int _chip;
    private TextMeshProUGUI _healthText;
    [SerializeField] private GameObject _player;
    [SerializeField] private Sprite[] _baseFace;
    [SerializeField] private Sprite[] _overchargedFace;
    [SerializeField] private Sprite[] _attackedFace;
    [SerializeField] private Sprite[] _bar;
    [SerializeField] private Sprite[] _fullBar;
    [SerializeField] private Sprite[] _healthBar;
    [SerializeField] private Color[] _colors;
    [SerializeField] private Canvas _canvas;

    private enum UI
    {
        ChargedBar,
        Face,
        Bar,
        HealthBar,
        Health
    };

    private void Start()
    {
        _chip = 0;
        var playerController = _player.GetComponent<PlayerController>();
        playerController.OnSwap += Swap;
        playerController.OnOvercharged += Overcharged;
        _player.GetComponent<PlayerHealth>().OnAttacked += Attacked;
        _healthText = _canvas.transform.GetChild((int)UI.Health).GetComponent<TextMeshProUGUI>();
    }

    private void Swap()
    {
        _chip = (_chip == 0) ? 1 : 0; //Set chip to 0 if it's 1 and to 1 id it's 0.
        UpdateHUD();
    }

    private void Overcharged(bool state)
    {
        _canvas.transform.GetChild((int)UI.ChargedBar).gameObject.SetActive(state); //Enable overcharged bar if player is overcharged. Disable it otherwise.
        _canvas.transform.GetChild((int)UI.Face).GetComponent<Image>().sprite = (state) ? _overchargedFace[_chip] : _baseFace[_chip]; //Set face image to show overcharged face of character used.
    }

    private void Attacked()
    {
        _canvas.transform.GetChild((int)UI.Face).GetComponent<Image>().sprite = _attackedFace[_chip];
        _healthText.text = (_player.GetComponent<PlayerHealth>()._currentHp).ToString() + "%";
        StartCoroutine(Cooldown());
    }

    private void UpdateHUD()
    {
        //change state of each sprite to blue or orange.
        _canvas.transform.GetChild((int)UI.ChargedBar).GetComponent<Image>().sprite = _fullBar[_chip];
        _canvas.transform.GetChild((int)UI.Face).GetComponent<Image>().sprite = _baseFace[_chip];
        _canvas.transform.GetChild((int)UI.Bar).GetComponent<Image>().sprite = _bar[_chip];
        _canvas.transform.GetChild((int)UI.HealthBar).GetComponent<Image>().sprite = _healthBar[_chip];
        _canvas.transform.GetChild((int)UI.Health).GetComponent<TextMeshProUGUI>().color = _colors[_chip];
    }

    private IEnumerator Cooldown()
    {
        yield return Helpers.GetWait(0.25f);
        _canvas.transform.GetChild((int)UI.Face).GetComponent<Image>().sprite = _baseFace[_chip];
    }
}