using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _resumBtn;
    [SerializeField] private GameObject _dialog;
    [SerializeField] private Slider _musícSlider, _sfxSlider,_timeSlider;
    [SerializeField] private TextMeshProUGUI _dialogTitle,_textMulti,_textScoreToast;
    [SerializeField] private GameObject _toast;

    private float timeLifeToast;
    public static UIManager Ins
    {
        get
        {
            m_ins = FindObjectOfType<UIManager>();
            if (!m_ins) m_ins = new GameObject().AddComponent<UIManager>();
            return m_ins;
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;
    
    private static UIManager m_ins;
    private bool m_isSetUpVol;
    private float timeLeft;
    private bool isRunTime;
    private void Start()
    {
        m_isSetUpVol = false;
        if (_timeSlider) timeLeft = _timeSlider.maxValue;
    }

    private void Update()
    {
        if (!m_isSetUpVol)
        {
            float musicVol = PrefConst.Ins.MusicVol;
            float sfxVol = PrefConst.Ins.SfxVol;
            if (musicVol == -1) musicVol = 0.4f;
            if (sfxVol == -1) sfxVol = 0.3f;
            PrefConst.Ins.MusicVol = musicVol;
            PrefConst.Ins.SfxVol = sfxVol;
            AudioManager.Ins.AudioVol(musicVol, false);
            AudioManager.Ins.AudioVol(sfxVol);
            if (_musícSlider) _musícSlider.value = musicVol;
            if (_sfxSlider) _sfxSlider.value = sfxVol;
            m_isSetUpVol = true;
        }

        if (isRunTime)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = 0f;
                isRunTime = false;
                GameManager.Ins.TimeOut();
            }
            _timeSlider.value = timeLeft;
        }
        
        
    }

    public void SetScoreText(int score)
    {
        scoreText!.text = score.ToString();
    }

    public void ChangeVolSfx()
    {
        if(_sfxSlider) AudioManager.Ins.AudioVol(_sfxSlider.value);
    }

    public void ChangeVolMusic()
    {
        if(_musícSlider) AudioManager.Ins.AudioVol(_musícSlider.value,false);
    }

    public void PauseGame()
    {
        AudioManager.Ins.PauseMusic();
        AudioManager.Ins.PlayAudio(ValuesConst.AUDIO_PAUSE);
        Time.timeScale = 0f;
        _dialog.SetActive(true);
    }

    public void ResumeGame()
    {
        AudioManager.Ins.PlayAudio(ValuesConst.AUDIO_RESUME);
        AudioManager.Ins.PauseMusic(false);
        _dialog.SetActive(false);
        Time.timeScale = 1f;
    }

    public void NewGame()
    {
        Time.timeScale = 1f;
        AudioManager.Ins.PlayAudio(ValuesConst.AUDIO_NEWGAME);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        AudioManager.Ins.PlayAudio(ValuesConst.AUDIO_GAMEOVER);
        _resumBtn.SetActive(false);
        PauseGame();
        if (_dialogTitle) _dialogTitle.text = "Game Over";
    }

    public void RunTime()
    {
        timeLeft = _timeSlider.maxValue;
        isRunTime = true;
    }
    
    public void Show_scoreToast(int multi)
    {
        if (!_toast || multi <= 0) return;

        int score = multi * multi * ValuesConst.SCOREROW;
        _textMulti.text = "X " +  multi;
        _textScoreToast.text = score + " Score";
        _toast.GetComponent<Animator>().SetTrigger("isActive");
        GameManager.Ins.IncreaseScore(score);
    }
}
