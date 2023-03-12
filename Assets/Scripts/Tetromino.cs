using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tetromino : MonoBehaviour
{
    [System.Serializable] public class GhostData
    {
        public float rotateAngle;
        public int startPosX;
    }
    [Header("Ghost Data")]
    [SerializeField] private GhostData[] _ghostsData;
    [SerializeField] private GameObject _ghost;
    private List<Transform> m_ghostTop1Trans,m_ghostTop2Trans,m_ghostTop3Trans;
    private List<List<Transform>> m_ghostTransList;
    [SerializeField] private ValuesConst.Type _type;
    private int m_beginRowClean, m_amountRowClean,m_countShow = -1;
    private static Transform[,] m_board = new Transform[ValuesConst.ROW, ValuesConst.COL];
    private Transform[,] m_boardFake = new Transform[ValuesConst.ROW, ValuesConst.COL];
    private float m_timeMove;
    private bool m_isSpawnGhostCall, m_checkLimitHor, m_checkLimitVer;
    void Start()
    {
        m_ghostTop1Trans = new List<Transform>();
        m_ghostTop2Trans = new List<Transform>();
        m_ghostTop3Trans = new List<Transform>();
        m_ghostTransList = new List<List<Transform>>();
        m_timeMove = 0.1f;
        m_isSpawnGhostCall = false;
        m_boardFake = m_board;
    }
    void Update()
    {
        if (!m_isSpawnGhostCall)
        {
            m_isSpawnGhostCall = true;
            SpawnGhost();
            ShowGhost();
        }
    }
    
    //Thêm các block vào mảng
    private bool AddBoard()
    {
        foreach (Transform child in transform)
        {
            var position = child.position;
            var y = Mathf.RoundToInt(position.y);
            m_board[y, Mathf.RoundToInt(position.x)] = child;
            if (y == ValuesConst.ROW - 1) return false;
        }

        return true;
    }
    
    //Xoá các dòng đầy
    private void CleanAllFilledRow()
    {
        m_amountRowClean = 0;
        for (var row = 0; row < ValuesConst.ROW; row++)
        {
            if (IsEmptyRow(row)) break;
            if (!IsFilledRow(row)) continue;
            CleanRow(row);
            if (m_amountRowClean == 0)
                m_beginRowClean = row;
            m_amountRowClean++;
        }
    }

    //Di chuyển các dòng xuống 1 dòng khi dòng dưới trống
    private IEnumerator DropBlocks(float time, bool check = true)
    {
        yield return new WaitForSeconds(time);
        
        if (m_amountRowClean > 0)
        {
            m_amountRowClean--;
            for (int row = m_beginRowClean + 1; row < ValuesConst.ROW; row++)
            {
                for (int col = 0; col < ValuesConst.COL; col++)
                {
                    Transform trans = m_board[row, col];
                    if (trans)
                    {
                        trans.position += Vector3.down;
                        m_board[row - 1, col] = trans;
                        m_board[row, col] = null;
                    }
                }
                
            }
            StartCoroutine(DropBlocks(m_timeMove));
        }else if (check)
        {
            GameManager.Ins.spawnAble = true;
        }
    }

    private bool IsEmptyRow(int row)
    {
        for (int i = 0; i < ValuesConst.COL; i++)
        {
            if (m_board[row, i]) return false;
        }

        return true;
    }

    private bool IsFilledRow(int row)
    {
        for (int i = 0; i < ValuesConst.COL; i++)
        {
            if (!m_board[row, i]) return false;
        }

        return true;
    }

    private void CleanRow(int row)
    {
        for (int i = 0; i < ValuesConst.COL; i++)
        {
            m_board[row, i].gameObject.GetComponent<Block>().Death(_type);
            m_board[row, i] = null;
        } 
    } 
    
    //tạo các bản sao Ghost
    private void SpawnGhost()
    {
        // tạo bản sao ghost và thêm vào m_ghostTop3Trans
        foreach (var ghostData in _ghostsData)
        {
            int x = ghostData.startPosX;
            while (true)
            {
                var obj = Instantiate(_ghost, new Vector3(x, ValuesConst.START_BLOCK_POS_Y, 0),
                    quaternion.identity);
                obj.transform.Rotate(Vector3.forward * ghostData.rotateAngle);
                var trans = obj.transform;
                if (!Drop(ref trans))
                {
                    Destroy(obj);
                    if (m_checkLimitHor) break;
                    if (m_checkLimitVer)
                    {
                        x++;
                        continue;
                    }
                }
                obj.transform.position = trans.position;
                obj.SetActive(false);
                x++;
                if (IsTop1Trans(obj.transform))
                {
                    m_ghostTop1Trans.Add(obj.transform);
                    continue;
                }

                if (IsTop2Trans(obj.transform))
                {
                    m_ghostTop2Trans.Add(obj.transform);
                    continue;
                }
                m_ghostTop3Trans.Add(obj.transform);
            }
        }

        // trộn mảng và ẩn các phần tử của m_ghostTop3Trans
        var count = m_ghostTop3Trans.Count;
        while (count > 1)
        {
            count--;
            var k = Random.Range(0, count);
            (m_ghostTop3Trans[k], m_ghostTop3Trans[count]) = (m_ghostTop3Trans[count], m_ghostTop3Trans[k]);
        }
        
        
        //chuyển các phần tử của mảng m_ghostTop3Trans và m_ghostTransList

        while (m_ghostTop1Trans.Count > 0)
        {
            var ghostTrans = new List<Transform>();
            var ghostTransDelete = new List<Transform>();
            ghostTrans.Add(m_ghostTop1Trans[0]);
            m_ghostTop1Trans.RemoveAt(0);
            foreach (var trans in m_ghostTop1Trans)
            {
                if (ghostTrans.TrueForAll(trans2 => !IsGhostAdjacent(trans, trans2)))
                {
                    ghostTrans.Add(trans);
                    ghostTransDelete.Add(trans);
                }
            }
            m_ghostTop1Trans.RemoveAll(trans => ghostTransDelete.Contains(trans));
            if (ghostTrans.Count > 2)
            {
                m_ghostTransList.Add(ghostTrans);
                continue;
            }
            
            ghostTransDelete = new List<Transform>();
            
            //
            foreach (var trans in m_ghostTop2Trans)
            {
                if (ghostTrans.TrueForAll(trans2 => !IsGhostAdjacent(trans, trans2)))
                {
                    ghostTrans.Add(trans);
                    ghostTransDelete.Add(trans);
                }
            }
            m_ghostTop2Trans.RemoveAll(trans => ghostTransDelete.Contains(trans));
            if (ghostTrans.Count > 2)
            {
                m_ghostTransList.Add(ghostTrans);
                continue;
            }
            
            //
            ghostTransDelete = new List<Transform>();
            foreach (var trans in m_ghostTop3Trans)
            {
                if (ghostTrans.TrueForAll(trans3 => !IsGhostAdjacent(trans, trans3)))
                {
                    ghostTrans.Add(trans);
                    ghostTransDelete.Add(trans);
                }
            }
            m_ghostTransList.Add(ghostTrans);
            m_ghostTop3Trans.RemoveAll(trans => ghostTransDelete.Contains(trans));
        }

        while (m_ghostTop2Trans.Count > 0)
        {
            var ghostTrans = new List<Transform>();
            var ghostTransDelete = new List<Transform>();
            ghostTrans.Add(m_ghostTop2Trans[0]);
            m_ghostTop2Trans.RemoveAt(0);
            foreach (var trans in m_ghostTop2Trans)
            {
                if (ghostTrans.TrueForAll(trans2 => !IsGhostAdjacent(trans, trans2)))
                {
                    ghostTrans.Add(trans);
                    ghostTransDelete.Add(trans);
                }
            }
            
            m_ghostTop2Trans.RemoveAll(trans => ghostTransDelete.Contains(trans));
            if (ghostTrans.Count > 2)
            {
                m_ghostTransList.Add(ghostTrans);
                continue;
            }

            ghostTransDelete = new List<Transform>();
            
            foreach (var trans in m_ghostTop3Trans)
            {
                if (ghostTrans.TrueForAll(trans2 => !IsGhostAdjacent(trans, trans2)))
                {
                    ghostTrans.Add(trans);
                    ghostTransDelete.Add(trans);
                }
            }
            m_ghostTop3Trans.RemoveAll(trans => ghostTransDelete.Contains(trans));
            m_ghostTransList.Add(ghostTrans);
        }
        
        while (m_ghostTop3Trans.Count > 0)
        {
            var ghostTrans = new List<Transform>();
            var ghostTransDelete = new List<Transform>();
            var index = m_ghostTop3Trans.Count - 1;
            ghostTrans.Add(m_ghostTop3Trans[index]);
            m_ghostTop3Trans.RemoveAt(index);
            foreach (var trans in m_ghostTop3Trans)
            {
                var check = ghostTrans.All(trans2 => !IsGhostAdjacent(trans, trans2));

                if (check)
                {
                    ghostTrans.Add(trans);
                    ghostTransDelete.Add(trans);
                }
            }
            m_ghostTransList.Add(ghostTrans);
            m_ghostTop3Trans.RemoveAll(trans => ghostTransDelete.Contains(trans));
        }
    }

    private bool IsTop1Trans(Transform trans)
    {
        var checkFilled = false;
        foreach (Transform block in trans)
        {
            m_boardFake[Mathf.RoundToInt(block.position.y), Mathf.RoundToInt(block.position.x)] = transform;
        }
        for (int row = 0; row < ValuesConst.ROW; row++)
        {
            checkFilled = true;
            for (int col = 0; col < ValuesConst.COL; col++)
            {
                if (!m_boardFake[row, col])
                {
                    checkFilled = false;
                    break;
                }
            }

            if (checkFilled) break;
        }
        for (int row = 0; row < ValuesConst.ROW; row++)
        {
            for (int col = 0; col < ValuesConst.COL; col++)
            {
                if (m_boardFake[row, col] && m_boardFake[row, col].Equals(transform))
                {
                    m_boardFake[row, col] = null;
                }
            }
        }
        
        return checkFilled;
    }

    private bool IsTop2Trans(Transform trans)
    {
        bool check = false;
        var th2 = 0;
        foreach (Transform block in trans)
        {
            var x = Mathf.RoundToInt(block.position.x);
            var y = Mathf.RoundToInt(block.position.y);
            var checkAmount = 0;
            if (OfRangeHor(x- 1) && m_board[y, x - 1]) checkAmount++;
            if (OfRangeHor(x + 1) && m_board[y, x + 1]) checkAmount++;
            if (y - 1 >= 0 && m_board[y - 1, x]) checkAmount++;
            if (checkAmount > 0 && (x is 0 or ValuesConst.COL - 1 || y == 0)) checkAmount++;
            if (checkAmount > 1) return true;
            if (checkAmount == 1) th2++;
            if (th2 > 1) return true;
        }

        return false;
    }

    private bool OfRangeHor(float x)
    {
        return x >= 0 && x < ValuesConst.COL;
    }
    
    // Kiểm tra 2 tranform có sát nhau không
    private bool IsGhostAdjacent(Transform trans1, Transform trans2)
    {
        foreach (Transform child1 in trans1)
        {
            foreach (Transform child2 in trans2)
            {
                if (Mathf.Abs(Mathf.RoundToInt(child1.position.x) - Mathf.RoundToInt(child2.position.x)) < 2f) return true;
            }
        }

        return false;
    }
    
    //Hiển thị Ghost
    public void ShowGhost()
    {
        if (!m_isSpawnGhostCall || m_ghostTransList.Count <= 0) return;
        if (m_countShow != -1)
        {
            foreach (var ghostTrans in m_ghostTransList[m_countShow])
            {
                ghostTrans.gameObject.SetActive(false);
            }
        }
        m_countShow++;
        if (m_countShow == m_ghostTransList.Count) m_countShow = 0;
        foreach (var ghostTrans in m_ghostTransList[m_countShow])
        {
            ghostTrans.gameObject.SetActive(true);
        }
    }
    
    //di chuyển đối tượng xuống dưới đến khi chạm
    private bool Drop(ref Transform trans)
    {
        while (true)
        {
            trans.position += Vector3.down;
            if (!CheckBoundary(trans, false))
            {
                if (m_checkLimitHor) return false;
                trans.position += Vector3.up;
                break;
            }
        }
        
        return CheckBoundary(trans, true);
    }
    
    /// kiểm tra ranh giới
    private bool CheckBoundary(Transform tetrominoTrans,bool isDrop)
    {
        if (!isDrop)
        {
            m_checkLimitHor = false;
            foreach (Transform block in tetrominoTrans)
            {
                int x = Mathf.RoundToInt(block.position.x);
                int y = Mathf.RoundToInt(block.position.y);
                if (!OfRangeHor(x))
                {
                    m_checkLimitHor = true;
                    return false;
                }
                if (y < ValuesConst.ROW && (y < 0 || m_board[y, x])) return false;
            }
            return true;
        }
        m_checkLimitVer = false;
        foreach (Transform block in tetrominoTrans)
        {
            if (Mathf.RoundToInt(block.position.y) >= ValuesConst.ROW)
            {
                m_checkLimitVer = true;
                return false;
            }
        }
        return true;
    }
    
    //Khi click vào vị trí của ghost
    public void OnClickGhost(Vector3 pos,float rotateAngle)
    {
        transform.Rotate(Vector3.forward * rotateAngle);
        transform.position = pos;
        foreach (List<Transform> listTrans in m_ghostTransList)
        {
            foreach (Transform ghostTrans in listTrans)
            {
                Destroy(ghostTrans.gameObject);
            }
        }
        UpdateBoard();
    }
    
    // cập nhật lại board
    private void UpdateBoard()
    {
        if (AddBoard())
        {
            CleanAllFilledRow();
            GameManager.Ins.IncreaseScore(m_amountRowClean);
            StartCoroutine(DropBlocks(0.2f));
            return;
        }
        GameManager.Ins.GameOver();
        enabled = false;
    }

    public void TimeOut()
    {
        var trans = m_ghostTransList[0][0];
        foreach (var translist in m_ghostTransList)
        {
            if (translist[0].gameObject.activeSelf)
            {
                int index = Random.Range(0,translist.Count);
                trans = translist[index];
                break;
            }
        }
        AudioManager.Ins.PlayAudio(ValuesConst.AUDIO_DROP);
        OnClickGhost(trans.position,trans.rotation.eulerAngles.z);
    }
}                                                              
                                                               