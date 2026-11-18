using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;

    // Called when the Start button is clicked
    public void StartGame()
    {
        Debug.Log("Starting game: navigating to GameScene...");
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Awake()
    {
        EnsureEventSystem();
    }

    void Start()
    {
        EnsureEventSystem();
        HookUpStartButton();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void EnsureEventSystem()
    {
        #if UNITY_2023_1_OR_NEWER
        var es = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        #else
        var es = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        #endif
        if (es == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
    }

    private void HookUpStartButton()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(StartGame);
            startButton.onClick.AddListener(StartGame);
            return;
        }

        // Search for existing buttons with "Start" in name or text
        #if UNITY_2023_1_OR_NEWER
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        #else
        Button[] buttons = FindObjectsOfType<Button>();
        #endif

        foreach (var btn in buttons)
        {
            string name = btn.gameObject.name.ToLowerInvariant();
            TMP_Text tmp = btn.GetComponentInChildren<TMP_Text>();
            Text txt = btn.GetComponentInChildren<Text>();
            string textContent = (tmp != null ? tmp.text : (txt != null ? txt.text : "")).ToLowerInvariant();

            if (name.Contains("start") || textContent.Contains("start"))
            {
                startButton = btn;
                btn.onClick.RemoveListener(StartGame);
                btn.onClick.AddListener(StartGame);
                return;
            }
        }

        // If not found, look for candidate GameObjects in the Menu scene
        string[] candidates = new string[] { "result text", "Start", "Start Button", "StartButton", "RawImage (1)" };
        foreach (string cand in candidates)
        {
            GameObject go = GameObject.Find(cand);
            if (go != null)
            {
                Button btn = go.GetComponent<Button>();
                if (btn == null)
                {
                    btn = go.AddComponent<Button>();
                }
                startButton = btn;
                btn.onClick.RemoveListener(StartGame);
                btn.onClick.AddListener(StartGame);
                return;
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name.ToLowerInvariant().Contains("menu"))
        {
            #if UNITY_2023_1_OR_NEWER
            MenuController mc = Object.FindFirstObjectByType<MenuController>();
            #else
            MenuController mc = Object.FindObjectOfType<MenuController>();
            #endif
            if (mc == null)
            {
                GameObject menuGo = new GameObject("MenuManager");
                menuGo.AddComponent<MenuController>();
            }
        }
    }
}
