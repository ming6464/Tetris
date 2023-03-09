using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public bool isActive;
    private float timeMove, timeStartMove;
    private float timeDownMove, TimeStartDownMove;
    private static Transform[,] board = new Transform[19, 7];
    void Start()
    {
        isActive = true;
        timeMove = 0.2f;
        timeDownMove = 0.5f;
        TimeStartDownMove = timeDownMove;
    }
    void Update()
    {
        if (!isActive) return;
        Move();
        if (Input.GetKeyDown(KeyCode.UpArrow))
            BlockRotate();
    }

    private void Move()
    {
        TimeStartDownMove -= Time.deltaTime;
        if (TimeStartDownMove <= 0)
        {
            MoveWithDirect(ValuesConst.MOVE_DOWN);
            TimeStartDownMove = timeDownMove;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveWithDirect(ValuesConst.MOVE_LEFT);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveWithDirect(ValuesConst.MOVE_RIGHT);
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            timeStartMove -= Time.deltaTime;
            if (timeStartMove <= 0)
            {
                MoveWithDirect(ValuesConst.MOVE_DOWN);
                timeStartMove = timeMove;
            }
            
        }
            
    }

    public void MoveWithDirect(int state)
    {
        isActive = CheckCanMove();
        if (!isActive)
        {
            AddToboard();
            GameManager.Ins.spawnAble = true;
            return;
        }
        Vector3 dir = Vector3.zero;
        switch (state)
        {
            case ValuesConst.MOVE_DOWN:
                dir = Vector3.down;
                break;
            case ValuesConst.MOVE_LEFT:
                dir = Vector3.left;
                break;
            case ValuesConst.MOVE_RIGHT:
                dir = Vector3.right;
                break;
        }
        transform.position += dir;
        if (!CheckPos()) transform.position -= dir;
    }

    public void BlockRotate()
    {
        if (!isActive) return;
        transform.Rotate(Vector3.forward,90);
        if(!CheckPos()) transform.Rotate(Vector3.forward,-90);
    }

    private bool CheckPos()
    {
        bool check = true;
        int index = 0;
        foreach (Transform block in transform)
        {
            index++;
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            if (x < 0 || x > 6) check = false;
            else if (board[y, x]) check = false;
        }
        if(index == 0) Destroy(gameObject);
        return check && isActive;
    }

    private bool CheckCanMove()
    {
        foreach (Transform block in transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            if (y == 0) return false;
            if (board[y-1, x]) return false;
        }
        return true;
    }

    private void AddToboard()
    {
        foreach (Transform child in transform)
        {
            board[Mathf.RoundToInt(child.position.y), Mathf.RoundToInt(child.position.x)] = child;
        }

        Updateboard();
    }

    private void Updateboard()
    {
        int beginRowClean = -1;
        int amountRowClean = 1;
        bool checkFull;
        bool checkEmpty;
        for (int row = 0; row < 17; row++)
        {
            checkEmpty = true;
            checkFull = true;
            for (int col = 0; col < 7; col++)
            {
                if (!board[row, col])
                {
                    checkFull = false;
                    break;
                }
                
                checkEmpty = false;
            }
            if (checkEmpty) break;
            if (checkFull)
            {
                CleanRow(row);
                if(beginRowClean == -1)
                    beginRowClean = row;
            }
        }

        if (beginRowClean >= 0 && beginRowClean <= 16)
        {
            for (int row = beginRowClean + 1; row < 17; row++)
            {
                checkEmpty = true;
                for (int col = 0; col < 7; col++)
                {
                    if (board[row, col])
                    {
                        board.SetValue(board[row,col],row - amountRowClean,col);
                        board[row, col] = null;
                        board[row - amountRowClean,col].transform.position += Vector3.down * amountRowClean;
                        checkEmpty = false;
                    }
                }
                if (checkEmpty) amountRowClean++;
            }
        }
    }

    private void CleanRow(int row)
    {
        for (int col = 0; col < 7; col++)
        {
            Destroy(board[row, col].gameObject);
            board[row, col] = null;
        }
    }
}
