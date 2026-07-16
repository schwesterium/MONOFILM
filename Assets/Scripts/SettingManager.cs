using UnityEngine;
using UnityEngine.Audio;
public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;

    [SerializeField]
    private AudioMixer audioMixer;

    public AudioMixer AudioMixer { get { return audioMixer; } }

    //BGMのデシベルを格納する
    public float decibelBGM = -10f;
    //SEのデシベルを格納する
    public float decibelSE = -10f;
    //カメラ感度値(m_MaxSpeed)を格納する
    public float camSensi = 1.0f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
