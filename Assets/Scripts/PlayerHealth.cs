using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    public int maxHealth = 100;
    [SyncVar] private int currentHealth;
    private bool isTouchingToxicMolly = false;
    public int damagePerTick = 1;
    public float damageInterval = 0.1f;
    private float damageTimer;

    public Slider healthSlider;

    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        healthSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (isTouchingToxicMolly)
        {
            if (!healthSlider.gameObject.activeSelf)
            {
                healthSlider.gameObject.SetActive(true);
            }

            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                currentHealth -= damagePerTick;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                healthSlider.value = currentHealth;
                damageTimer = 0f;

                if (currentHealth <= 0)
                {
                    CmdPlayerDied();
                }
            }
        }
        else
        {
            if (healthSlider.gameObject.activeSelf)
            {
                healthSlider.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("ToxicMolly"))
        {
            isTouchingToxicMolly = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ToxicMolly"))
        {
            isTouchingToxicMolly = false;
        }
    }

    [Command]
    private void CmdPlayerDied()
    {
        RpcRestartLevel();
    }

    [ClientRpc]
    private void RpcRestartLevel()
    {
        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene("DeathScene");
        }
    }
}
