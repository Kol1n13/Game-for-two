using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    public int maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChanged))] private int currentHealth;
    private bool isTouchingToxicMolly = false;
    public int damagePerTick = 1;
    public float damageInterval = 0.1f;
    private float damageTimer;

    public Slider healthSlider;
    public CanvasGroup healthBarCanvasGroup; // ��������� CanvasGroup ��� �������� �������
    public float fadeSpeed = 5f; // �������� ������������ health bar

    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        // ������� ��� ��������� CanvasGroup ���� �� ���������� � ����������
        if (healthBarCanvasGroup == null)
        {
            healthBarCanvasGroup = healthSlider.GetComponentInParent<CanvasGroup>();
            if (healthBarCanvasGroup == null)
            {
                healthBarCanvasGroup = healthSlider.gameObject.AddComponent<CanvasGroup>();
            }
        }

        // ������ ���������� health bar ��� ���������� ������, �� ��������� ���������� ����� �����
        healthSlider.gameObject.SetActive(isLocalPlayer);

        // ����� �������� ���� �������� ������
        UpdateHealthBarVisibility();
    }

    // ��� ��� ������������� health bar ��� ��������� ��������
    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        healthSlider.value = newHealth;
        UpdateHealthBarVisibility();

        if (newHealth <= 0 && isLocalPlayer)
        {
            CmdPlayerDied();
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (isTouchingToxicMolly)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                // ��������� �������� ����� ������� ��� �������������
                CmdTakeDamage(damagePerTick);
                damageTimer = 0f;
            }
        }

        // ������� ���������� ���������� health bar
        SmoothHealthBarVisibility();
    }

    // ���������� ��������� health bar
    private void UpdateHealthBarVisibility()
    {
        if (healthBarCanvasGroup != null)
        {
            // ���� �������� �� ������ - ����������, ����� ��������
            bool shouldShow = currentHealth < maxHealth;

            // ��������� ������������� ����� ��� ��������
            healthBarCanvasGroup.alpha = shouldShow ? 1f : 0f;
            healthBarCanvasGroup.interactable = shouldShow;
            healthBarCanvasGroup.blocksRaycasts = shouldShow;
        }
    }

    // ������� ��������� ��������� health bar
    private void SmoothHealthBarVisibility()
    {
        if (healthBarCanvasGroup != null)
        {
            bool shouldShow = currentHealth < maxHealth;
            float targetAlpha = shouldShow ? 1f : 0f;

            // ������� ��������� ������������
            healthBarCanvasGroup.alpha = Mathf.Lerp(
                healthBarCanvasGroup.alpha,
                targetAlpha,
                fadeSpeed * Time.deltaTime
            );

            // ��������� �������������� ����� ��������� ������
            if (Mathf.Approximately(healthBarCanvasGroup.alpha, 0f))
            {
                healthBarCanvasGroup.interactable = false;
                healthBarCanvasGroup.blocksRaycasts = false;
            }
            else if (Mathf.Approximately(healthBarCanvasGroup.alpha, 1f))
            {
                healthBarCanvasGroup.interactable = true;
                healthBarCanvasGroup.blocksRaycasts = true;
            }
        }
    }

    [Command]
    private void CmdTakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isLocalPlayer) return;

        if (other.CompareTag("ToxicMolly"))
        {
            isTouchingToxicMolly = true;
            damageTimer = damageInterval; // �������� ����������� �����
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isLocalPlayer) return;

        if (other.CompareTag("ToxicMolly"))
        {
            isTouchingToxicMolly = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isLocalPlayer) return;

        if (other.CompareTag("ToxicMolly"))
        {
            isTouchingToxicMolly = false;
            damageTimer = 0f;
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
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        }
    }

    // ����� ��� ������� ������
    [Command]
    public void CmdHeal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    // ����� ��� ��������� �������� ��������
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // ����� ��� ��������, ��� �� �����
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}