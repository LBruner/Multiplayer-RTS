using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    private void Awake()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    private void OnMouseOver()
    {
        Debug.Log("faopjfoasjf");
    }

    private void OnMouseEnter()
    {
        Debug.Log("Foasj");
        healthBarParent.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        Debug.Log("5");
        healthBarParent.gameObject.SetActive(false);
    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
