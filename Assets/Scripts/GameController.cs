using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Rock - Paper - Scissors : starter script
// Follow the slides and complete every TODO.
// 1 = Rock, 2 = Paper, 3 = Scissors, 4 = Unknown ("?")
public class GameController : MonoBehaviour
{
    [SerializeField] Image imgYou;       // your selected image (rock, paper, scissor)
    [SerializeField] Image imgCom;       // computer selected image (rock, paper, scissor)

    [SerializeField] Text txtYou;        // the score you win
    [SerializeField] Text txtCom;        // the score computer win
    [SerializeField] Text txtResult;     // the result

    // Supports TextMeshProUGUI / TMP_Text if used in the Canvas
    [SerializeField] TMP_Text tmpYou;
    [SerializeField] TMP_Text tmpCom;
    [SerializeField] TMP_Text tmpResult;

    [SerializeField] float handScale = 3.28f; // scale for hand images so they appear nice and large

    int cntYou = 0;     // number you win
    int cntCom = 0;     // number computer win


    private void InitGame()
    {
        // TODO (slides 37-40): find the UI objects by name and get their components.
        // Example:
        // imgYou = GameObject.Find("ImgYou").GetComponent<Image>();
        // Also find: ImgCom, TxtYou, TxtCom, TxtResult_

        if (imgYou == null)
        {
            GameObject go = FindUIObject("player", "ImgYou", "imgYou", "Player", "Img_You");
            if (go != null) imgYou = go.GetComponent<Image>();
        }

        if (imgCom == null)
        {
            GameObject go = FindUIObject("computer", "ImgCom", "imgCom", "Computer", "Img_Com");
            if (go != null) imgCom = go.GetComponent<Image>();
        }

        if (txtYou == null && tmpYou == null)
        {
            GameObject go = FindUIObject("player score count", "TxtYou", "txtYou", "PlayerScoreCount", "player score");
            if (go != null)
            {
                txtYou = go.GetComponent<Text>() ?? go.GetComponentInChildren<Text>();
                tmpYou = go.GetComponent<TMP_Text>() ?? go.GetComponentInChildren<TMP_Text>();
            }
        }

        if (txtCom == null && tmpCom == null)
        {
            GameObject go = FindUIObject("computer score count", "TxtCom", "txtCom", "ComputerScoreCount", "computer score");
            if (go != null)
            {
                txtCom = go.GetComponent<Text>() ?? go.GetComponentInChildren<Text>();
                tmpCom = go.GetComponent<TMP_Text>() ?? go.GetComponentInChildren<TMP_Text>();
            }
        }

        if (txtResult == null && tmpResult == null)
        {
            GameObject go = FindUIObject("result text", "TxtResult_", "TxtResult", "txtResult", "ResultText");
            if (go != null)
            {
                txtResult = go.GetComponent<Text>() ?? go.GetComponentInChildren<Text>();
                tmpResult = go.GetComponent<TMP_Text>() ?? go.GetComponentInChildren<TMP_Text>();
            }
        }

        // Remove any accidental 3D TextMesh components on background boxes
        ClearStrayTextMeshes();

        // Center align the result text and expand width so it doesn't wrap or get cut off
        AlignResultText();

        // Fix score boxes: stretch inside background box with padding, center text, and enable auto-sizing
        FixScoreBox(txtYou, tmpYou);
        FixScoreBox(txtCom, tmpCom);

        // Set the initial text of txtResult to "Select the button below"
        SetText(txtResult, tmpResult, "Select the button below");

        // Initialize score counters
        cntYou = 0;
        cntCom = 0;
        SetText(txtYou, tmpYou, cntYou.ToString());
        SetText(txtCom, tmpCom, cntCom.ToString());

        // Keep the large hand scale, ensure correct facing rotation, and reset starting sprite to unknown ("?") img_4
        Sprite unknownSprite = Resources.Load<Sprite>("img_4");
        if (unknownSprite == null)
        {
            unknownSprite = Resources.Load("img_4", typeof(Sprite)) as Sprite;
        }

        if (imgYou != null)
        {
            float currentScale = Mathf.Abs(imgYou.transform.localScale.x);
            if (currentScale > 1.5f) handScale = currentScale;
            if (unknownSprite != null) imgYou.sprite = unknownSprite;
            imgYou.transform.localRotation = Quaternion.identity;
            imgYou.transform.localScale = new Vector3(handScale, handScale, handScale);
        }

        if (imgCom != null)
        {
            float currentScale = Mathf.Abs(imgCom.transform.localScale.x);
            if (currentScale > 1.5f) handScale = Mathf.Max(handScale, currentScale);
            if (unknownSprite != null) imgCom.sprite = unknownSprite;
            imgCom.transform.localRotation = Quaternion.identity;
            imgCom.transform.localScale = new Vector3(handScale, handScale, handScale);
        }

        // Auto-hook Rock, Paper, Scissors buttons if On Click () is not set in Inspector
        HookUpButtons();
    }

    void Start()
    {
        // init the game
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        // exit if press escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // Slides 24-31: link this function to the On Click () event of every button.
    // Pass the button itself as the GameObject parameter.
    public void OnButtonClick(GameObject buttonObject)
    {
        if (buttonObject == null) return;

        Debug.Log(buttonObject.name + " clicked ");

        // Slide 34: get the number from the first character of the button name
        int you = 0;
        if (buttonObject.name.Length > 0 && char.IsDigit(buttonObject.name[0]))
        {
            int.TryParse(buttonObject.name.Substring(0, 1), out you);
        }

        // Also supports custom button names (e.g. "Rock button", "paper button", "Scissor Button")
        if (you < 1 || you > 3)
        {
            string lower = buttonObject.name.ToLowerInvariant();
            if (lower.Contains("rock")) you = 1;
            else if (lower.Contains("paper")) you = 2;
            else if (lower.Contains("scissor")) you = 3;
        }

        if (you >= 1 && you <= 3)
        {
            CheckResult(you);
        }
    }

    // Overload for convenience if called with button index directly
    public void OnButtonClick(int choice)
    {
        if (choice >= 1 && choice <= 3)
        {
            CheckResult(choice);
        }
    }

    void CheckResult(int yourResult)
    {
        Debug.Log("Your result is: " + yourResult);

        // TODO (slide 33): game logic
        // 1. com = random number 1..3  ->  UnityEngine.Random.Range(1, 4)
        int com = UnityEngine.Random.Range(1, 4);

        // 2. k = you - com
        int k = yourResult - com;

        // 3. k == 0              -> "Draw."
        //    k == 1 or k == -2   -> "You win."       (cntYou++)
        //    otherwise           -> "Computer win."  (cntCom++)
        string resultStr;
        if (k == 0)
        {
            resultStr = "Draw";
        }
        else if (k == 1 || k == -2)
        {
            resultStr = "You Win";
            cntYou++;
        }
        else
        {
            resultStr = "Computer Win";
            cntCom++;
        }

        // Align result text center and update string
        AlignResultText();
        SetText(txtResult, tmpResult, resultStr);

        // 4. call SetResult(yourResult, comResult)
        SetResult(yourResult, com);
    }

    void SetResult(int you, int com)
    {
        // TODO (slide 41):
        // 1. change imgYou.sprite and imgCom.sprite
        //    Resources.Load("img_" + you, typeof(Sprite)) as Sprite
        Sprite spriteYou = Resources.Load<Sprite>("img_" + you);
        if (spriteYou == null)
        {
            spriteYou = Resources.Load("img_" + you, typeof(Sprite)) as Sprite;
        }
        if (imgYou != null && spriteYou != null)
        {
            imgYou.sprite = spriteYou;
        }

        Sprite spriteCom = Resources.Load<Sprite>("img_" + com);
        if (spriteCom == null)
        {
            spriteCom = Resources.Load("img_" + com, typeof(Sprite)) as Sprite;
        }
        if (imgCom != null && spriteCom != null)
        {
            imgCom.sprite = spriteCom;
        }

        // 2. Face each other: player faces right (+x) towards computer; computer flips (-x) towards player
        if (imgYou != null)
        {
            imgYou.transform.localRotation = Quaternion.identity;
            imgYou.transform.localScale = new Vector3(handScale, handScale, handScale);
        }
        if (imgCom != null)
        {
            imgCom.transform.localRotation = Quaternion.identity;
            imgCom.transform.localScale = new Vector3(-handScale, handScale, handScale);
        }

        // 3. update txtYou.text and txtCom.text with the scores
        SetText(txtYou, tmpYou, cntYou.ToString());
        SetText(txtCom, tmpCom, cntCom.ToString());

        // Keep scores perfectly centered and auto-sized inside the background box (handles 1, 2, or more digits)
        FixScoreBox(txtYou, tmpYou);
        FixScoreBox(txtCom, tmpCom);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void AlignResultText()
    {
        if (txtResult != null)
        {
            txtResult.alignment = TextAnchor.MiddleCenter;
            RectTransform rt = txtResult.rectTransform;
            rt.anchoredPosition = new Vector2(0f, rt.anchoredPosition.y);
            rt.sizeDelta = new Vector2(Mathf.Max(rt.sizeDelta.x, 600f), Mathf.Max(rt.sizeDelta.y, 80f));
        }
        if (tmpResult != null)
        {
            tmpResult.alignment = TextAlignmentOptions.Center;
            tmpResult.horizontalAlignment = HorizontalAlignmentOptions.Center;
            tmpResult.verticalAlignment = VerticalAlignmentOptions.Middle;
            tmpResult.margin = Vector4.zero;
            RectTransform rt = tmpResult.rectTransform;
            rt.anchoredPosition = new Vector2(0f, rt.anchoredPosition.y);
            rt.sizeDelta = new Vector2(Mathf.Max(rt.sizeDelta.x, 600f), Mathf.Max(rt.sizeDelta.y, 80f));
        }
    }

    private void FixScoreBox(Text uiText, TMP_Text tmpText)
    {
        if (uiText != null)
        {
            uiText.alignment = TextAnchor.MiddleCenter;
            RectTransform rt = uiText.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(10f, 10f);
            rt.offsetMax = new Vector2(-10f, -10f);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }
        if (tmpText != null)
        {
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            tmpText.verticalAlignment = VerticalAlignmentOptions.Middle;
            tmpText.margin = Vector4.zero;
            tmpText.textWrappingMode = TextWrappingModes.NoWrap;
            tmpText.enableAutoSizing = true;
            tmpText.fontSizeMin = 18;
            tmpText.fontSizeMax = 65;

            RectTransform rt = tmpText.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(10f, 10f);
            rt.offsetMax = new Vector2(-10f, -10f);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    private void ClearStrayTextMeshes()
    {
        string[] boxNames = new string[] { "Button", "Button (1)" };
        foreach (string bName in boxNames)
        {
            GameObject bGo = GameObject.Find(bName);
            if (bGo != null)
            {
                TextMesh tm = bGo.GetComponent<TextMesh>();
                if (tm != null) tm.text = "";
                MeshRenderer mr = bGo.GetComponent<MeshRenderer>();
                if (mr != null) mr.enabled = false;
            }
        }
    }

    private void SetText(Text uiText, TMP_Text tmpText, string value)
    {
        if (uiText != null) uiText.text = value;
        if (tmpText != null) tmpText.text = value;
    }

    private void HookUpButtons()
    {
        #if UNITY_2023_1_OR_NEWER
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        #else
        Button[] buttons = FindObjectsOfType<Button>();
        #endif

        foreach (Button btn in buttons)
        {
            if (btn == null) continue;
            string lower = btn.gameObject.name.ToLowerInvariant();
            bool isRpsButton = (btn.name.Length > 0 && btn.name[0] >= '1' && btn.name[0] <= '3')
                || lower.Contains("rock") || lower.Contains("paper") || lower.Contains("scissor");

            if (isRpsButton)
            {
                // Only auto-add listener if not already configured via persistent events
                if (btn.onClick.GetPersistentEventCount() == 0)
                {
                    GameObject targetGo = btn.gameObject;
                    btn.onClick.AddListener(() => OnButtonClick(targetGo));
                }
            }
        }
    }

    private GameObject FindUIObject(params string[] candidateNames)
    {
        foreach (string name in candidateNames)
        {
            GameObject go = GameObject.Find(name);
            if (go != null) return go;
        }

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var tr in root.GetComponentsInChildren<Transform>(true))
                {
                    foreach (string name in candidateNames)
                    {
                        if (string.Equals(tr.name.Trim(), name.Trim(), System.StringComparison.OrdinalIgnoreCase))
                        {
                            return tr.gameObject;
                        }
                    }
                }
            }
        }

        return null;
    }
}
