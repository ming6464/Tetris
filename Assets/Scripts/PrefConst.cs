using UnityEngine;

public class PrefConst : MonoBehaviour
{
    public static PrefConst Ins 
    {
        get
        {
            m_ins = GameObject.FindObjectOfType<PrefConst>();
            if (!m_ins) m_ins = new GameObject().AddComponent<PrefConst>();
            return m_ins;
        }
    }
    private static PrefConst m_ins;

    public float MusicVol
    {
        get => PlayerPrefs.GetFloat(ValuesConst.MUSIC_VOL, -1f);
        set => PlayerPrefs.SetFloat(ValuesConst.MUSIC_VOL, value);
    }

    public float SfxVol
    {
        get => PlayerPrefs.GetFloat(ValuesConst.SFX_VOL, -1f);
        set => PlayerPrefs.SetFloat(ValuesConst.SFX_VOL, value);
    }
}