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
            if (!m_ins)
                m_ins = new GameObject().AddComponent<GameManager>();
            return m_ins;
        }
    }
    public bool spawnAble;
    public bool isGameOver;
    
    [SerializeField] private GameObject _blockParticle;
    [SerializeField] private Tetromino[] _tetrominos;
    
    private static GameManager m_ins;
    private Tetromino m_nextTetromino;
    private Vector3 m_posNextTetromino;
    private int m_nextIndexTetromino;
    private Tetromino m_curTetromino;
    private void Start()
    {
        spawnAble = true;
        m_nextIndexTetromino = Random.Range(0, _tetrominos.Length);
        m_posNextTetromino = new Vector3(0.5f, 19.5f, 0);
    }

    private void Update()
    {
        if (isGameOver) return;
        if(spawnAble) SpawnBlock();
        HandleClick();
    }

    private void SpawnBlock()
    {
        if (_tetrominos.Length > 0)
        {
            _tetrominos[m_nextIndexTetromino].isNextTetromino = false;
            m_curTetromino = Instantiate(_tetrominos[m_nextIndexTetromino], new Vector3(5,16,0),Quaternion.identity);
            m_nextIndexTetromino = Random.Range(0, _tetrominos.Length);
            spawnAble = false;
            ShowNextTetromino();
        }
    }
    private void ShowNextTetromino()
    {
        if(m_nextTetromino)Destroy(m_nextTetromino.gameObject);
        _tetrominos[m_nextIndexTetromino].isNextTetromino = true;
        m_nextTetromino = Instantiate(_tetrominos[m_nextIndexTetromino],m_posNextTetromino,Quaternion.identity);
        m_nextTetromino.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        m_nextTetromino.enabled = false;
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
    public void TetrominoGhostOnClick(Transform ghost)
    {
        if (m_curTetromino)
        {
            m_curTetromino.OnClickGhost(ghost.position,ghost.rotation.eulerAngles.z);
        }
    }

    public void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit && hit.collider.gameObject.CompareTag("Tetromino Block"))
            {
                TetrominoGhostOnClick(hit.collider.gameObject.transform);
            }
        }
        
    }
}