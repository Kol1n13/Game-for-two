using UnityEngine;
using UnityEngine.UI;

public class EndPanelManager : MonoBehaviour
{
    public static EndPanelManager Instance { get; private set; }

    [Tooltip("Перетащите сюда саму EndPanel")]
    public GameObject endPanel;

    [Tooltip("Перетащите сюда DarkBackground (тот же стиль, что в PauseMenu)")]
    public GameObject darkBackground;

    [Tooltip("Кнопка выхода в меню")]
    public Button menuButton;

    [Header("Host behavior")]
    public bool stopHostWhenHostPressesMenu = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (endPanel != null) endPanel.SetActive(false);
        if (darkBackground != null) darkBackground.SetActive(false);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuPressed);
    }

    void OnDestroy()
    {
        if (menuButton != null)
            menuButton.onClick.RemoveListener(OnMenuPressed);
    }

    public void ShowEndPanel()
    {
        if (endPanel != null) endPanel.SetActive(true);
        if (darkBackground != null) darkBackground.SetActive(true);
    }

    public void OnMenuPressed()
    {
        MenuActions.GoToMenu(stopHostWhenHostPressesMenu, this);
    }
}
