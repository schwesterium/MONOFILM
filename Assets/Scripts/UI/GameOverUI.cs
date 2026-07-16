using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    //ボタンの指定
    [SerializeField]
    private Button restartBuuton = null;
    [SerializeField]
    private Button backTitleButton = null;

    //UnityEventの登録
    [SerializeField]
    private UnityEvent onRestartButtonClick = null;
    [SerializeField]
    private UnityEvent onBackTitleButtonClick = null;

    [SerializeField]
    private AudioClip crap;

    public UnityEvent OnRestartButtonClick => onRestartButtonClick;
    public UnityEvent OnBackTitleButtonClick => onBackTitleButtonClick;

    AudioSource audioSource;

    Animator animator;
    static readonly int introId = Animator.StringToHash("Intro");
    static readonly int outroId = Animator.StringToHash("Outro");


    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        //イベントの登録
        restartBuuton.onClick.AddListener(() =>
        {
            OnRestartButtonClick.Invoke();
            animator.SetTrigger(outroId);

        });

        backTitleButton.onClick.AddListener(() =>
        {
            OnBackTitleButtonClick.Invoke();
            animator.SetTrigger(outroId);

        });

        Hide();
    }

    //ゲームオーバーアニメーションの再生
    public void AnimPlay()
    {
        Show();
        animator.SetTrigger(introId);
        audioSource.Play();
        restartBuuton.Select();
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

    //効果音を鳴らす
    public void PlayClap()
    {
        audioSource.PlayOneShot(crap);
    }
}
