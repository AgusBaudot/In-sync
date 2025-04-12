using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageUI : MonoBehaviour
{
    private TextMeshProUGUI _dmgText; //Text that will show damage numbers.

    private void Update()
    {
        if (_dmgText != null) //If damage text exists:
            _dmgText.transform.parent.rotation = Quaternion.LookRotation(_dmgText.transform.parent.position - Helpers.Camera.transform.position); //Make it face the camera.
    }

    public void ShowDamage(int dmg) //Show damage in text
    {
        GetComponent<TextMeshProUGUI>().text = dmg.ToString(); //Show damage.
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

    public void DestroyGameObjectAfterAnimation() //Method called when animation of text is finished.
    {
        Destroy(gameObject);
    }
}
