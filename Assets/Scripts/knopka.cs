using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[Serializable]
public class PortalPair
{
    public AnomalyPortal portalA;
    public AnomalyPortal portalB;
}

public class knopka : NetworkBehaviour
{
    [SerializeField] private PortalPair[] portalPairs;
    [SerializeField] private Animator buttonAnimator;
    [SerializeField] private int minCoins = 0;
    [SerializeField] private int maxCoins = int.MaxValue;

    [SyncVar(hook = nameof(OnButtonStateChanged))]
    private bool syncIsActive = false;

    private readonly HashSet<Collider2D> objectsOnButton = new HashSet<Collider2D>();

    private void Start()
    {
        UpdateButtonVisual();
    }

    private void OnButtonStateChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"Button state changed to: {newValue}");
        UpdateButtonVisual();
        UpdatePortalsState(newValue);
    }

    private void UpdateButtonVisual()
    {
        if (buttonAnimator != null)
            buttonAnimator.SetBool("pressed", syncIsActive);
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        
        // Проверяем и игрока и коробку
        if (other.CompareTag("Player") || other.CompareTag("box"))
        {
            objectsOnButton.Add(other);
            Debug.Log($"{other.tag} entered button. Total objects: {objectsOnButton.Count}");
            
            // Проверяем, достаточно ли монет для активации
            CheckButtonActivation();
        }
    }

    [ServerCallback]
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;
        
        if (other.CompareTag("Player") || other.CompareTag("box"))
        {
            objectsOnButton.Remove(other);
            Debug.Log($"{other.tag} exited button. Total objects: {objectsOnButton.Count}");
            
            // Проверяем, нужно ли деактивировать кнопку
            CheckButtonActivation();
        }
    }

    [Server]
    private void CheckButtonActivation()
    {
        // Если на кнопке нет объектов, деактивируем
        if (objectsOnButton.Count == 0)
        {
            if (syncIsActive)
            {
                syncIsActive = false;
                Debug.Log("Button deactivated - no objects on button");
            }
            return;
        }

        // Проверяем, достаточно ли монет для активации
        bool hasEnoughCoins = CheckCoinsCondition();
        
        if (hasEnoughCoins && !syncIsActive)
        {
            syncIsActive = true;
            Debug.Log("Button activated - enough coins and objects on button");
        }
        else if (!hasEnoughCoins && syncIsActive)
        {
            syncIsActive = false;
            Debug.Log("Button deactivated - not enough coins");
        }
    }

    [Server]
    private bool CheckCoinsCondition()
    {
        if (CoinManager.Instance != null)
        {
            int coins = CoinManager.Instance.GetTotalCoins();
            bool hasEnoughCoins = coins >= minCoins && coins <= maxCoins;
            Debug.Log($"Coins check: {coins}, required: {minCoins}-{maxCoins}, result: {hasEnoughCoins}");
            return hasEnoughCoins;
        }
        
        Debug.LogWarning("CoinManager.Instance is null. Allowing activation without coin check.");
        return true; // Если CoinManager не существует, разрешаем активацию
    }

    [Server]
    private void UpdatePortalsState(bool active)
    {
        if (portalPairs == null) return;

        foreach (var pair in portalPairs)
        {
            if (pair == null) continue;
            
            if (pair.portalA != null)
            {
                pair.portalA.SetActive(active);
                Debug.Log($"Setting portalA {pair.portalA.name} to {active}");
            }
            
            if (pair.portalB != null)
            {
                pair.portalB.SetActive(active);
                Debug.Log($"Setting portalB {pair.portalB.name} to {active}");
            }
        }
    }

    // Обработка изменения количества монет
    private IEnumerator SubscribeWhenCoinManagerReady()
    {
        while (CoinManager.Instance == null)
            yield return null;
        CoinManager.OnTotalCoinsChanged += OnTotalCoinsChanged;
    }

    [Server]
    private void OnTotalCoinsChanged(int newTotal)
    {
        // При изменении количества монет проверяем состояние кнопки
        CheckButtonActivation();
    }

    private void OnEnable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.OnTotalCoinsChanged += OnTotalCoinsChanged;
        }
        else
        {
            StartCoroutine(SubscribeWhenCoinManagerReady());
        }
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
            CoinManager.OnTotalCoinsChanged -= OnTotalCoinsChanged;
    }
}