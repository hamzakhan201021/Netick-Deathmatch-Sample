using UnityEngine;
using Netick.Unity;
using Netick;
using TMPro;

public class PlayerHealthController : NetworkBehaviour
{
    [Header("Health")]
    public int StartHealth = 100;
    [Networked] public int PlayerHealth { get; set; } = 100;
    [Space]
    [Header("UI")]
    [SerializeField] private GameObject _healthUI;
    [SerializeField] private TMP_Text _healthText;

    public override void NetworkStart()
    {
        PlayerHealth = StartHealth;

        UpdateHealthText();

        _healthUI.SetActive(IsInputSource);
    }

    public void ChangeHealth(int amount)
    {
        if (!IsServer)
        {
            Debug.LogWarning("Called change health from NOT Server, will not perform action (ignoring)");

            return;
        }

        PlayerHealth += amount;
    }

    public override void NetworkFixedUpdate()
    {
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        _healthText.text = "Health " + PlayerHealth;
    }
}