using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : NetworkBehaviour
{
    [Header("Optional - leave empty to auto-find by name")]
    public GameObject pausePanel; // можно оставить пустым — скрипт попробует найти объект с именем "PausePanel"

    private Button continueButton;
    private Button menuButton;

    [Header("Host behavior")]
    [Tooltip("Если true — после того как сервер сменит сцену на Menu, он вызовет StopHost() (прекратит хостинг). Обычно не нужно.")]
    public bool stopHostWhenHostPressesMenu = false;

    void Start()
    {
        Debug.Log($"[PauseMenu] Start on '{gameObject.name}' isLocalPlayer={isLocalPlayer}");

        if (!isLocalPlayer)
        {
            Debug.Log("[PauseMenu] Not local player -> skipping Start.");
            return;
        }

        // Найдём панель (включая неактивные)
        if (pausePanel == null)
        {
            pausePanel = FindGameObjectInSceneByName("PausePanel");
            if (pausePanel != null) Debug.Log("[PauseMenu] Found PausePanel in scene.");
        }

        if (pausePanel == null)
        {
            Debug.LogWarning("[PauseMenu] PausePanel not assigned and not found in scene.");
            return;
        }

        pausePanel.SetActive(false);

        Transform contT = pausePanel.transform.Find("ContinueButton");
        Transform menuT = pausePanel.transform.Find("MenuButton");

        if (contT != null) continueButton = contT.GetComponent<Button>();
        if (menuT != null) menuButton = menuT.GetComponent<Button>();

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinuePressed);
            Debug.Log("[PauseMenu] Subscribed to ContinueButton.onClick");
        }
        else Debug.LogWarning("[PauseMenu] ContinueButton not found under PausePanel.");

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuPressed);
            Debug.Log("[PauseMenu] Subscribed to MenuButton.onClick");
        }
        else Debug.LogWarning("[PauseMenu] MenuButton not found under PausePanel.");
    }

    void OnDestroy()
    {
        if (continueButton != null) continueButton.onClick.RemoveListener(OnContinuePressed);
        if (menuButton != null) menuButton.onClick.RemoveListener(OnMenuPressed);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[PauseMenu] Escape pressed on local player.");
            TogglePauseUI();
        }
    }

    public void TogglePauseUI()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("[PauseMenu] TogglePauseUI called but pausePanel == null");
            return;
        }
        bool newState = !pausePanel.activeSelf;
        pausePanel.SetActive(newState);
        Debug.Log($"[PauseMenu] pausePanel active state set to {newState}");
    }

    public void OnContinuePressed()
    {
        if (!isLocalPlayer) return;
        Debug.Log("[PauseMenu] Continue pressed");
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void OnMenuPressed()
    {
        if (!isLocalPlayer) return;

        if (isServer)
        {
            // Если этот локальный игрок — сервер/хост, инициируем смену сцены для всех
            Debug.Log("[PauseMenu] Local player is server -> changing scene for everyone to 'Menu'.");
            if (NetworkManager.singleton != null)
            {
                NetworkManager.singleton.ServerChangeScene("Menu");

                // Опция: остановить хост после смены сцены (по желанию)
                if (stopHostWhenHostPressesMenu)
                {
                    StartCoroutine(StopHostDelayed());
                }
            }
            else
            {
                Debug.LogWarning("[PauseMenu] NetworkManager.singleton == null when trying to change scene as server.");
            }
        }
        else
        {
            // Клиент: отключаем только клиента и загружаем локальное меню
            Debug.Log("[PauseMenu] Local player is client -> disconnecting client and loading local Menu scene.");
            if (NetworkManager.singleton != null)
            {
                NetworkManager.singleton.StopClient();
            }
            // Загрузить локальную сцену Меню (у клиента)
            SceneManager.LoadScene("Menu");
        }
    }

    IEnumerator StopHostDelayed()
    {
        // Небольшая задержка: даём кадр для начала смены сцены у клиентов (по необходимости)
        yield return null;
        if (NetworkManager.singleton != null)
        {
            Debug.Log("[PauseMenu] Stopping host (StopHost).");
            NetworkManager.singleton.StopHost();
        }
    }

    // ---------- helper: ищет объект по имени в активной сцене, включая неактивные ----------
    GameObject FindGameObjectInSceneByName(string name)
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid()) return null;

        var roots = scene.GetRootGameObjects();
        foreach (var root in roots)
        {
            var found = FindInChildrenRecursively(root.transform, name);
            if (found != null) return found.gameObject;
        }
        return null;
    }

    Transform FindInChildrenRecursively(Transform t, string name)
    {
        if (t.name == name) return t;
        for (int i = 0; i < t.childCount; i++)
        {
            var r = FindInChildrenRecursively(t.GetChild(i), name);
            if (r != null) return r;
        }
        return null;
    }
}
