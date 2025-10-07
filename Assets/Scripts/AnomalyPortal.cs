using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnomalyPortal : NetworkBehaviour
{
    [Header("Portal Configuration")]
    [SerializeField] private AnomalyPortal pairPortal;
    [SerializeField] private Animator portalAnimator;
    
    [Header("Teleport cooldown (seconds)")]
    [SerializeField] private float entryCooldown = 0.5f;

    [SyncVar(hook = nameof(OnActiveStateChanged))]
    private bool syncIsActive = false;

    private readonly Dictionary<GameObject, Coroutine> blockedObjects = new Dictionary<GameObject, Coroutine>();

    private void Start()
    {
        UpdateVisualState();
    }

    private void OnActiveStateChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"Portal {name} state changed: {newValue}");
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (portalAnimator != null)
            portalAnimator.SetBool("isActive", syncIsActive);
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!syncIsActive) 
        {
            Debug.Log($"Portal {name} is inactive, ignoring trigger");
            return;
        }
        
        if (other == null) return;

        // Проверяем тег Player ИЛИ Box
        if (!other.CompareTag("Player") && !other.CompareTag("box")) 
        {
            Debug.Log($"Object {other.name} with tag {other.tag} is not Player or box, ignoring");
            return;
        }

        GameObject obj = other.attachedRigidbody != null ? 
            other.attachedRigidbody.gameObject : other.gameObject;

        // Если этот объект сейчас заблокирован — игнорируем вход
        if (IsBlocked(obj)) 
        {
            Debug.Log($"Object {obj.name} is blocked, ignoring");
            return;
        }

        Debug.Log($"Portal {name} activating teleport for {obj.name}");

        // Выполняем телепорт на сервере
        TeleportObject(obj);

        // Блокируем объект на парном портале
        if (pairPortal != null)
        {
            pairPortal.BlockObjectForSeconds(obj, entryCooldown);
        }
    }

    [ServerCallback]
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;
        
        if (!other.CompareTag("Player") && !other.CompareTag("box")) return;

        GameObject obj = other.attachedRigidbody != null ? 
            other.attachedRigidbody.gameObject : other.gameObject;

        // Блокируем на этом портале после выхода
        BlockObjectForSeconds(obj, entryCooldown);
    }

    [Server]
    private void TeleportObject(GameObject obj)
    {
        if (pairPortal == null)
        {
            Debug.LogWarning($"Portal '{name}' has no pair assigned — cannot teleport.");
            return;
        }

        Debug.Log($"Teleporting {obj.name} from {name} to {pairPortal.name}");

        // Перемещаем объект в позицию парного портала
        obj.transform.position = pairPortal.transform.position;

        // Обнуляем скорость Rigidbody2D
        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            Debug.Log($"Reset velocity for {obj.name}");
        }

        // Для игрока также нужно обновить позицию на всех клиентах
        if (obj.CompareTag("Player"))
        {
            var playerIdentity = obj.GetComponent<NetworkIdentity>();
            if (playerIdentity != null)
            {
                // Принудительно обновляем позицию игрока на всех клиентах
                RpcUpdatePlayerPosition(obj, pairPortal.transform.position);
            }
        }
    }

    [ClientRpc]
    private void RpcUpdatePlayerPosition(GameObject player, Vector3 newPosition)
    {
        if (player != null)
        {
            player.transform.position = newPosition;
            Debug.Log($"RPC updated player position to {newPosition}");
        }
    }

    [Server]
    public void SetActive(bool active)
    {
        syncIsActive = active;
        Debug.Log($"Server set portal {name} active: {active}");
    }

    // ---- Блокировка объектов ----
    private bool IsBlocked(GameObject obj)
    {
        if (obj == null) return false;
        return blockedObjects.ContainsKey(obj);
    }

    [Server]
    public void BlockObjectForSeconds(GameObject obj, float seconds)
    {
        if (obj == null) return;

        if (blockedObjects.TryGetValue(obj, out var existing))
        {
            StopCoroutine(existing);
        }

        Coroutine c = StartCoroutine(UnblockAfterSeconds(obj, seconds));
        blockedObjects[obj] = c;
    }

    private IEnumerator UnblockAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (obj != null && blockedObjects.ContainsKey(obj))
        {
            blockedObjects.Remove(obj);
            Debug.Log($"Unblocked {obj.name}");
        }
    }

    private void OnDisable()
    {
        foreach (var kv in blockedObjects)
        {
            if (kv.Value != null)
                StopCoroutine(kv.Value);
        }
        blockedObjects.Clear();
    }
}