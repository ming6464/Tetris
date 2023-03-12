using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Ins
    {
        get
        {
            m_ins = FindObjectOfType<GameManager>();
            if (!m_ins) m_ins = new GameObject().AddComponent<GameManager>();
            return m_ins;
        }
    }
    public bool spawnAble;
    
    
    [System.Serializable]
    private class TetrominoInfo
    {
        public GameObject obj;
        public ValuesConst.Type type;
    }
    
    
    [SerializeField] private GameObject _blockParticle;
    [SerializeField] private TetrominoInfo[] _tetrominos;
    [SerializeField] private TetrominoInfo[] _nextOrCurTetrominos;


    private readonly Vector3 m_curPosTetrominoFake = new(1.2f, 22.2f, 0f),
        m_nextPosTetromino1 = new(10.5f, 17.5f, 0f),
        m_nextPosTetromino2 = new(10.5f, 15f, 0f),
        m_nextPosTetromino3 = new(10.5f, 12.5f, 0f),
        m_curPostTetrominoRead = new(99f, 0f, 0f),
        m_scaleCurTetromino = new(ValuesConst.SCALE_TETROMINO_FAKE, ValuesConst.SCALE_TETROMINO_FAKE, 0),
        m_scaleNextTetromino = new (ValuesConst.SCALE_TETROMINO_NEXT, ValuesConst.SCALE_TETROMINO_NEXT, 0);

    private TetrominoInfo[] m_nextAndCurTetromino = new TetrominoInfo[4];
    private static GameManager m_ins;
    private GameObject m_curTetromino;
    private int m_score;
    private bool m_isGameOver, m_isSettedScore;

    private void Start()
    {
        spawnAble = true;
        if (_nextOrCurTetrominos.Length > 0)
        {
            m_nextAndCurTetromino[1] = GenerateRandomTetrominoInfo(m_nextPosTetromino1,m_scaleNextTetromino);
            m_nextAndCurTetromino[2] = GenerateRandomTetrominoInfo(m_nextPosTetromino2,m_scaleNextTetromino);
            m_nextAndCurTetromino[3] = GenerateRandomTetrominoInfo(m_nextPosTetromino3,m_scaleNextTetromino);
        }
    }

    private void Update()
    {
        if (m_isGameOver) return;
        if (!m_isSettedScore)
        {
            UIManager.Ins.SetScoreText(m_score);
            m_isSettedScore = true;
        }
        if(spawnAble) SpawnTetromino();
        if(Input.GetKeyDown(KeyCode.UpArrow)) ChangeGhost();
        HandleClick();
    }

    private void SpawnTetromino()
    {
        if(_tetrominos.Length <= 0) return;
        //
        if (m_nextAndCurTetromino[0] != null)
        {
            Destroy(m_nextAndCurTetromino[0].obj);
        }
        
        //
        m_nextAndCurTetromino[0] = m_nextAndCurTetromino[1];
        m_nextAndCurTetromino[1] = m_nextAndCurTetromino[2];
        m_nextAndCurTetromino[2] = m_nextAndCurTetromino[3];
        m_nextAndCurTetromino[3] = GenerateRandomTetrominoInfo(m_nextPosTetromino3,m_scaleNextTetromino);
        //cập nhật lại vị trí các phần tử
        m_nextAndCurTetromino[0].obj.transform.position = m_curPosTetrominoFake;
        m_nextAndCurTetromino[1].obj.transform.position = m_nextPosTetromino1;
        m_nextAndCurTetromino[2].obj.transform.position = m_nextPosTetromino2;
        //
        m_nextAndCurTetromino[0].obj.transform.localScale = m_scaleCurTetromino;
        //Nhân bản tetromino
        GameObject curObj = _tetrominos.FirstOrDefault(x => x.type == m_nextAndCurTetromino[0].type)!.obj;
        m_curTetromino = CloneObject(curObj, m_curPostTetrominoRead);
        spawnAble = false;
    }
    public void RunBlockParticle(Vector3 pos, ValuesConst.Type type)
    {
        Color color;
        switch (type)
        {
            case ValuesConst.Type.I:
                color = new Color(0.643f, 0.353f, 0.863f, 1f);
                break;
            case ValuesConst.Type.J:
                color = new Color(0.858f,0.353f,0.859f);
                break;
            case ValuesConst.Type.L:
                color = new Color(0.918f,0.322f,0.478f);
                break;
            case ValuesConst.Type.O:
                color = new Color(0.933f, 0.510f, 0.643f);
                break;
            case ValuesConst.Type.S:
                color = new Color(0.945f, 0.000f, 0.506f);
                break;
            case ValuesConst.Type.T:
                color = new Color(0.784f, 0.055f, 0.600f);
                break;
            default:
                color = new Color(0.444f, 0.310f, 0.871f);
                break;
        }

        GameObject newPart = Instantiate(_blockParticle, pos, quaternion.identity);
        ParticleSystem partComp = newPart.GetComponent<ParticleSystem>();
        partComp.startColor = color;
        Destroy(newPart,partComp.startLifetime);
    }

    public void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit && hit.collider.gameObject.CompareTag("Tetromino Block"))
            {
                AudioManager.Ins.PlayAudio(ValuesConst.AUDIO_DROP);
                Transform ghost = hit.collider.gameObject.transform;
                m_curTetromino.GetComponent<Tetromino>()!.OnClickGhost(ghost.position,ghost.rotation.eulerAngles.z);
            }
        }
    }

    public void ChangeGhost()
    {
        if (!m_curTetromino) return;
        AudioManager.Ins.PlayAudio(ValuesConst.AUDIO_ROTATE);
        m_curTetromino.GetComponent<Tetromino>().ShowGhost();
    }
    
    private GameObject CloneObject(GameObject obj, Vector3 pos)
    {
        return Instantiate(obj, pos, Quaternion.identity);
    }

    private TetrominoInfo GenerateRandomTetrominoInfo(Vector3 pos,Vector3 scale)
    {
        var count =  Random.Range(0, _nextOrCurTetrominos.Length);
        TetrominoInfo tetInfo = new TetrominoInfo();
        tetInfo.type = _nextOrCurTetrominos[count].type;
        tetInfo.obj = CloneObject(_nextOrCurTetrominos[count].obj, pos);
        tetInfo.obj.transform.localScale = scale;
        return tetInfo;
    }

    public void IncreaseScore(int row)
    {
        m_score += row * ValuesConst.SCOREROW;
        UIManager.Ins.SetScoreText(m_score);
    }

    public void GameOver()
    {
        UIManager.Ins.GameOver();
        m_isGameOver = true;
    }
}