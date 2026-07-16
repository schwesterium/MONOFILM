using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StageClearUI : MonoBehaviour
{
    //ボタンの指定
    [SerializeField]
    private Button nextBuuton = null;
    [SerializeField]
    private Button backTitleButton = null;
    [SerializeField]
    private Button quitButton = null;

    //UnityEventの登録
    [SerializeField]
    private UnityEvent onNextButtonClick = null;
    [SerializeField]
    private UnityEvent onBackTitleButtonClick = null;
    [SerializeField]
    private UnityEvent onQuitButtonClick = null;
    public UnityEvent OnNextButtonClick => onNextButtonClick;
    public UnityEvent OnBackTitleButtonClick => onBackTitleButtonClick;
    public UnityEvent OnQuitButtonClick => onQuitButtonClick;

    private AudioSource audioSource;

    private Animator animator;
    static readonly int introId = Animator.StringToHash("Intro");
    static readonly int outroId = Animator.StringToHash("Outro");


    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        nextBuuton.onClick.AddListener(() =>
        {
            OnNextButtonClick.Invoke();
            animator.SetTrigger(outroId);

        });

        backTitleButton.onClick.AddListener(() =>
        {
            OnBackTitleButtonClick.Invoke();
            animator.SetTrigger(outroId);

        });

        quitButton.onClick.AddListener(() =>
        {
            OnQuitButtonClick.Invoke();
            animator.SetTrigger(outroId);

        });

        Hide();
    }
    
    //ステージクリアアニメーションの再生
    public void AnimPlay()
    {
        Show();
        animator.SetTrigger(introId);
        audioSource.Play();
        nextBuuton.Select();
    }

    //UIの表示
    public void Show()
    {
        gameObject.SetActive(true);
    }

    //UIの非表示
    //アニメーションでも呼び出している
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
