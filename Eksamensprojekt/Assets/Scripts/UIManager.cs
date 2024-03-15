using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image damageVignette;

    //Health bar
    [SerializeField] Transform healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image healthBarFill;
    [SerializeField] Gradient healthBarGradient;
    int startHealth;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovementAdvanced.onPlayerHit += PlayerMovementAdvanced_onPlayerHit;
        healthBarFill.color = healthBarGradient.Evaluate(1);
        startHealth = PlayerMovementAdvanced.instance.GetStartHealth();
        healthText.text = "HP " + startHealth;
    }

    private void PlayerMovementAdvanced_onPlayerHit(int health)
    {
        float fillAmount = (float)health / startHealth;

        float duration = 0.2f;
        healthBarFill.DOFillAmount(fillAmount, duration);
        healthBar.DOShakePosition(duration, 4, 30);
        healthBarFill.DOColor(healthBarGradient.Evaluate(fillAmount), duration);

        healthText.text = "HP " + health;

        damageVignette.DOFade(0.5f, 0.1f).OnComplete(() => damageVignette.DOFade(0f, 0.3f));
    }
}
