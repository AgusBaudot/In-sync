using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageUI : MonoBehaviour
{
    private TextMeshProUGUI _dmgText;

    private void Update()
    {
        if (_dmgText != null)
            _dmgText.transform.parent.rotation = Quaternion.LookRotation(_dmgText.transform.parent.position - Helpers.Camera.transform.position);
    }

    public void ShowDamage(int dmg)
    {
        GetComponent<TextMeshProUGUI>().text = dmg.ToString();
        //StartCoroutine(FadeUI(_dmgText, 0, 0.75f));
    }

    private IEnumerator FadeUI(TextMeshProUGUI sr, float endValue, float duration) //Change it to a static method (inside helper maybe?)
    {
        float elapsedTime = 0;
        float startValue = sr.color.a;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void DestroyGameObjectAfterAnimation()
    {
        Destroy(gameObject);
    }
}
