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
    public CanvasGroup healthBarCanvasGroup; // Добавляем CanvasGroup для плавного скрытия
    public float fadeSpeed = 5f; // Скорость исчезновения health bar

    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        // Находим или добавляем CanvasGroup если не установлен в инспекторе
        if (healthBarCanvasGroup == null)
        {
            healthBarCanvasGroup = healthSlider.GetComponentInParent<CanvasGroup>();
            if (healthBarCanvasGroup == null)
            {
                healthBarCanvasGroup = healthSlider.gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Всегда показывать health bar для локального игрока, но управлять видимостью через альфу
        healthSlider.gameObject.SetActive(isLocalPlayer);

        // Сразу скрываем если здоровье полное
        UpdateHealthBarVisibility();
    }

    // Хук для синхронизации health bar при изменении здоровья
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
                // Уменьшаем здоровье через команду для синхронизации
                CmdTakeDamage(damagePerTick);
                damageTimer = 0f;
            }
        }

        // Плавное управление видимостью health bar
        SmoothHealthBarVisibility();
    }

    // Обновление видимости health bar
    private void UpdateHealthBarVisibility()
    {
        if (healthBarCanvasGroup != null)
        {
            // Если здоровье не полное - показываем, иначе скрываем
            bool shouldShow = currentHealth < maxHealth;

            // Мгновенно устанавливаем альфу без анимации
            healthBarCanvasGroup.alpha = shouldShow ? 1f : 0f;
            healthBarCanvasGroup.interactable = shouldShow;
            healthBarCanvasGroup.blocksRaycasts = shouldShow;
        }
    }

    // Плавное изменение видимости health bar
    private void SmoothHealthBarVisibility()
    {
        if (healthBarCanvasGroup != null)
        {
            bool shouldShow = currentHealth < maxHealth;
            float targetAlpha = shouldShow ? 1f : 0f;

            // Плавное изменение прозрачности
            healthBarCanvasGroup.alpha = Mathf.Lerp(
                healthBarCanvasGroup.alpha,
                targetAlpha,
                fadeSpeed * Time.deltaTime
            );

            // Отключаем взаимодействие когда полностью скрыто
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
            damageTimer = damageInterval; // Начинаем повреждение сразу
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

    // Метод для лечения игрока
    [Command]
    public void CmdHeal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    // Метод для получения текущего здоровья
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Метод для проверки, жив ли игрок
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}