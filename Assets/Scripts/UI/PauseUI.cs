using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
// ポーズUIの進行制御を管理します。
public class PauseUI : MonoBehaviour
{
    // Resume Button が押されたときに発生する UnityEvent
    public UnityEvent onResumeButtonClick;
    // Restart Button が押されたときに発生する UnityEvent
    public UnityEvent onTitleButtonClick;

    // Resume Button を指定します。
    [SerializeField]
    private Button resumeButton = null;
    // Retry Button を指定します。
    [SerializeField]
    private Button TitleButton = null;

    [SerializeField]
    private OptionUI optionUI = null;

    WaitForSeconds waitTime = new WaitForSeconds(0.01f);


    void Awake()
    {
        //audioSource = GetComponent<AudioSource>();
        // UnityEvent を追加
        resumeButton.onClick.AddListener(() => { onResumeButtonClick.Invoke(); });
        TitleButton.onClick.AddListener(() => { onTitleButtonClick.Invoke(); });

        StartCoroutine(CheckCanHideUI());
        
    }

    // このUIを表示します。
    public void Show()
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    }

    //optionUIをhideして自身も非表示にする
    public void Hide()
    {
        optionUI.Hide();
        gameObject.SetActive(false);
    }

    //optionUIでInitialize()が完了後にUIを非表示にする
    IEnumerator CheckCanHideUI()
    {
        while (!optionUI.CanHide)
        {
            yield return waitTime;
        }

        Hide();
    }
}