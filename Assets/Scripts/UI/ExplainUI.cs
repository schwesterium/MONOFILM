using UnityEngine;
using UnityEngine.UI;

public class ExplainUI : MonoBehaviour
{
    [SerializeField]
    private Button backButton = null;

    [SerializeField]
    private GameObject ButtonsObj = null;
    private Button[] childButtons = null;

    private Animator animator;

    static readonly int showId = Animator.StringToHash("Show");
    static readonly int hideId = Animator.StringToHash("Hide");

    private void Awake()
    {
        animator = GetComponent<Animator>();

        childButtons = ButtonsObj.GetComponentsInChildren<Button>();
    }

    private void Start()
    {
        OnHide();
    }

    //UIをアニメーションを再生する
    public void Show()
    {
        animator.SetTrigger(showId);
    }

    public void Hide()
    {
        animator.SetTrigger(hideId);
    }

    // これらの関数はアニメーションから呼び出される

    //遊び方説明を表示する
    //その際背後にあるボタンのinteractableをfalseにする
    public void OnShow()
    {
        foreach (var b in childButtons) {b.interactable = false; }
        
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    //遊び方説明を表示する
    //その際背後にあるボタンのinteractableをtrueにする
    public void OnHide()
    {
        foreach (var b in childButtons) { b.interactable = true; }
        
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    //StartButtonを選択する
    public void SelectChildButtons()
    {
        childButtons[0].Select();
    }

    public void SelectBackButton()
    {
        backButton.Select();
    }
}
