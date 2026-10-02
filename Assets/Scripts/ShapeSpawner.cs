using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Drawing;

[Serializable] public class Shapes
{
    public bool controlled;
    public bool main;
    public Vector2Int pos;
    public int color;
    public int category;
    public int direction;
}

public class ShapeSpawner : MonoBehaviour
{
    public List<Shapes> squares;
    public List<Shapes> queue;
    public List<int> colors;
    public List<int> toclear;
    public List<int> cleared;
    public TileBase[] tiles;
    public Tilemap tilemap;
    public Tilemap tilemap2;
    public Interface game;
    InputAction moveAction;
    InputAction rotateLeft;
    InputAction rotateRight;
    bool playerTurn = false;
    const int totalColors = 5;
    const int reqTiles = 4;
    int ticks = 0;
    float timer = 0f;
    float moveTimer = 0f;
    float moveTimer2 = 0f;
    public float speed = 0.1f;
    public int sizex = 25;
    public int sizey = 25;

    // SETTINGS VALUES
    const int perLanded = 1;
    const int perMatch = 5;
    const int perExtraMatch = 2;
    const float speedPerLevel = 0.5f;
    const int speedPlusLevel = 3;

    public void ClearBoard()
    {
        squares.Clear();
        //if (squares.Count <= 0)
        //{
           //squares.Add(new Shapes { controlled = false, main = false, pos = new Vector2Int(-999, -999), color = -1, category = 0, direction = 0 });
           //squares[squares.Length] = new Shapes { controlled = false, pos = new Vector2Int(-999, -999), color = -1, category = 0, direction = 0 };
        //}
        //for (int i = 0; i < squares.Length; i++)
        //{
            //Array.Clear(squares, 0, squares.Length);
            //squares[i] = new Shapes { controlled = false, pos = new Vector2Int(-999, -999), color = -1, category = 0, direction = 0 };
        //}
        /*for (int i = 0; i < 25; i++)
        {

            for (int j = 0; j < 25; j++)
            {
                squares[i, j] = new Shapes { controlled = false, color = -1, category = 0, direction = 0 };
            }
        }*/
    }
    public bool CheckCollision(int sq, int way)
    {
        int coll = -1;
        switch ((squares[sq].direction + way) % 4)
        {
            case 0:
                coll = FindPos(new Vector2Int(squares[sq].pos.x, squares[sq].pos.y - 1));
                if (coll < 0 && !(squares[sq].pos.y <= 0)) return false;
                else if (coll >= 0) return !squares[coll].controlled || squares[sq].pos.y <= 0;
                else return squares[sq].pos.y <= 0;
            case 1:
                coll = FindPos(new Vector2Int(squares[sq].pos.x - 1, squares[sq].pos.y));
                if (coll < 0 && !(squares[sq].pos.x <= 0)) return false;
                else if (coll >= 0) return !squares[coll].controlled || squares[sq].pos.x <= 0;
                else return squares[sq].pos.x <= 0;
            case 2:
                coll = FindPos(new Vector2Int(squares[sq].pos.x, squares[sq].pos.y + 1));
                if (coll < 0 && !(squares[sq].pos.y >= sizey - 1)) return false;
                else if (coll >= 0) return !squares[coll].controlled || squares[sq].pos.y >= sizey - 1;
                else return squares[sq].pos.y >= sizey - 1;
            case 3:
                coll = FindPos(new Vector2Int(squares[sq].pos.x + 1, squares[sq].pos.y));
                if (coll < 0 && !(squares[sq].pos.x >= sizex - 1)) return false;
                else if (coll >= 0) return !squares[coll].controlled || squares[sq].pos.x >= sizex - 1;
                else return squares[sq].pos.x >= sizex - 1;
            default:
                return false;
        }
    }
    public void MoveInput(int dir)
    {
        bool canMove = true;
        for (int k = 0; k < squares.Count; k++)
        {
            if (squares[k].controlled)
            {
                if (CheckCollision(k, dir))
                {
                    canMove = false;
                }
            }
        }
        if (canMove)
        {
            for (int k = 0; k < squares.Count; k++)
            {
                if (squares[k].controlled)
                {
                    squares[k].pos = MoveSquare(k, dir);
                }
            }
        }
    }
    public Vector2Int MoveSquare(int sq, int way) // 0 = down, 1 = left, 2 = up, 3 = right
    {
        if (CheckCollision(sq, way))
        {
            return squares[sq].pos;
        }
        else
        {
            switch ((squares[sq].direction + way) % 4)
            {
                case 0:
                    return new Vector2Int(squares[sq].pos.x, squares[sq].pos.y - 1);
                case 1:
                    return new Vector2Int(squares[sq].pos.x - 1, squares[sq].pos.y);
                case 2:
                    return new Vector2Int(squares[sq].pos.x, squares[sq].pos.y + 1);
                case 3:
                    return new Vector2Int(squares[sq].pos.x + 1, squares[sq].pos.y);
                default:
                    return squares[sq].pos;
            }
        }
        
    }
    public void PopulateQueue(int length)
    {
        while (queue.Count < length)
        {
            if (colors.Count <= 0)
            {
                for (int i = 0; i <= totalColors; i++)
                {
                    colors.Add(i);
                }
            }
            int colorPick = UnityEngine.Random.Range(0, colors.Count);
            queue.Add(new Shapes {color = colors[colorPick], category = 0, direction = UnityEngine.Random.Range(1, 4) });
            colors.RemoveAt(colorPick);
        }
    }
    public void DrawBoard()
    {
        tilemap.ClearAllTiles();
        for (int i = 0; i < squares.Count; i++)
        {
            Vector3Int ps = new Vector3Int(squares[i].pos.x, squares[i].pos.y, 0);
            if (squares[i].color < 0)
            {
                if (tilemap.GetTile(ps) != null) tilemap.SetTile(ps, null);
            }
            else tilemap.SetTile(ps, tiles[squares[i].color + 1]);
        }
        for (int i = 0; i < 6; i++)
        {
            tilemap.SetTile(new Vector3Int(i * 3 + 36, 16, 0), tiles[queue[i].color + 1]);
            tilemap.SetTile(new Vector3Int(i * 3 + 36, 15, 0), tiles[queue[i].color + 1]);
            switch (queue[i].direction)
            {
                case 2:
                    tilemap.SetTile(new Vector3Int(i * 3 + 36, 17, 0), tiles[queue[i].color + 1]);
                    break;
                case 3:
                    tilemap.SetTile(new Vector3Int(i * 3 + 37, 16, 0), tiles[queue[i].color + 1]);
                    break;
                default:
                    break;
            }
        }
        /*for (int i = -5; i < sizex + 5; i++)
        {
            for (int j = -5; j < sizey + 5; j++)
            {
                Vector3Int ps = new Vector3Int(i, j, 0);
                int find = FindPos(new Vector2Int(i, j));
                if (find == -1)
                {
                    if (tilemap.GetTile(ps) != null) tilemap.SetTile(ps, null);
                }
                else tilemap.SetTile(ps, tiles[squares[find].color + 1]);
            }
        }*/
    }
    public int FindMain()
    {
        int found = -1;
        for (int i = 0; i < squares.Count; i++)
        {
            if (squares[i].main) found = i;
        }
        return found;
    }
    public int FindPos(Vector2Int p)
    {
        int found = -1;
        for (int i = 0; i < squares.Count; i++)
        {
            if (squares[i].pos == p) found = i;
        }
        //Debug.Log(found);
        return found;
    }
    public Vector2Int FindRotate(Vector2Int mainPos, int sq, int dir) // 0 = left, 1 = right
    {
        Vector2Int targetPos = new Vector2Int(squares[sq].pos.x, squares[sq].pos.y);
        Vector2Int targetPos2 = new Vector2Int(targetPos.x, targetPos.y);
        switch (targetPos.x - mainPos.x)
        {
            case -1: // left of main square
                targetPos2.x += 1;
                targetPos2.y -= ((dir == 0) ? 1 : -1);
                break;
            case 0:
                switch (targetPos.y - mainPos.y)
                {
                    case -1: // below main square
                        targetPos2.x += ((dir == 0) ? 1 : -1);
                        targetPos2.y += 1;
                        break;
                    case 1: // above main square
                        targetPos2.x += ((dir == 1) ? 1 : -1);
                        targetPos2.y -= 1;
                        break;
                    default:
                        return targetPos;
                }
                break;
            case 1: // right of main square
                targetPos2.x -= 1;
                targetPos2.y -= ((dir == 1) ? 1 : -1);
                break;
            default:
                return targetPos;
        }
        Debug.Log(targetPos + ", " + targetPos2);
        return targetPos2;
        /*int coll = FindPos(targetPos2);
        if (coll < 0) return targetPos2;
        else if (squares[coll].controlled) return targetPos2;
        else return targetPos;*/
    }
    public void RotateSquares(int dir) // 0 = left, 1 = right
    {
        bool canRotate = true;
        int main = FindMain();
        if (main < 0) return;
        Vector2Int mainPos = new Vector2Int(squares[main].pos.x, squares[main].pos.y);
        for (int k = main; k < squares.Count; k++)
        {
            if (squares[k].controlled)
            {
                Vector2Int targetPos = FindRotate(mainPos, k, dir);
                int coll = FindPos(targetPos);
                if (coll >= 0)
                {
                    if (!squares[coll].controlled)
                    {
                        canRotate = false;
                    }
                }
                if (ticks > 5 && (targetPos.x < 0 || targetPos.x > sizex - 1 || targetPos.y < 0 || targetPos.y > sizey - 1)) canRotate = false;
            }
        }
        if (canRotate)
        {
            for (int k = main; k < squares.Count; k++)
            {
                if (squares[k].controlled)
                {
                    squares[k].pos = FindRotate(mainPos, k, dir);
                }
            }
        }
    }
    public bool CheckAddClear(int color, Vector2Int pos)
    {
        if (toclear.Count == 0) return true;
        else return squares[FindPos(pos)].color == color;
    }
    public void CheckClear()
    {
        if (toclear.Count >= reqTiles)
        {
            while (toclear.Count > 0)
            {
                if (!cleared.Contains(toclear[0])) cleared.Add(toclear[0]);
                Debug.Log("added tile #" + toclear[0]);
                toclear.RemoveAt(0);
            }
        }
    }
    public int CheckAcross(int color, int px, int py)
    {
        toclear.Clear();
        int ct = 0;
        int cc = color;
        while (cc == color && cc >= 0 && ct <= 25 && FindPos(new Vector2Int(px + ct, py)) >= 0)
        {
            //if (FindPos(new Vector2Int(px + ct, py)) >= 0)
            //{
                cc = squares[FindPos(new Vector2Int(px + ct, py))].color;
                if (CheckAddClear(cc, new Vector2Int(px + ct - 1, py))) toclear.Add(FindPos(new Vector2Int(px + ct, py)));
                //Debug.Log("found tile #" + FindPos(new Vector2Int(px + ct, py)) + " across #" + ct);
            //}
            ct++;
        }
        CheckClear();
        return cleared.Count;
    }
    public int CheckDown(int color, int px, int py)
    {
        toclear.Clear();
        int ct = 0;
        int cc = color;
        while (cc == color && cc >= 0 && ct <= 25 && FindPos(new Vector2Int(px, py + ct)) >= 0)
        {
            //if (FindPos(new Vector2Int(px, py + ct)) >= 0)
            //{
                cc = squares[FindPos(new Vector2Int(px, py + ct))].color;
                if (CheckAddClear(cc, new Vector2Int(px, py + ct - 1))) toclear.Add(FindPos(new Vector2Int(px, py + ct)));
                //Debug.Log("found tile #" + FindPos(new Vector2Int(px, py + ct)) + " down #" + ct);
            //}
            ct++;
        }
        CheckClear();
        return cleared.Count;
    }
    public int CheckDiagonalLeft(int color, int px, int py)
    {
        toclear.Clear();
        int ct = 0;
        int cc = color;
        while (cc == color && cc >= 0 && ct <= 25 && FindPos(new Vector2Int(px + ct, py - ct)) >= 0)
        {
            //if (FindPos(new Vector2Int(px + ct, py - ct)) >= 0)
            //{
                cc = squares[FindPos(new Vector2Int(px + ct, py - ct))].color;
                if (CheckAddClear(cc, new Vector2Int(px + ct - 1, py - ct + 1))) toclear.Add(FindPos(new Vector2Int(px + ct, py - ct)));
            //}
            ct++;
        }
        CheckClear();
        return cleared.Count;
    }
    public int CheckDiagonalRight(int color, int px, int py)
    {
        toclear.Clear();
        int ct = 0;
        int cc = color;
        while (cc == color && cc >= 0 && ct <= 25 && FindPos(new Vector2Int(px + ct, py + ct)) >= 0)
        {
            //if (FindPos(new Vector2Int(px + ct, py + ct)) >= 0)
            //{
                cc = squares[FindPos(new Vector2Int(px + ct, py + ct))].color;
                if (CheckAddClear(cc, new Vector2Int(px + ct - 1, py + ct - 1))) toclear.Add(FindPos(new Vector2Int(px + ct, py + ct)));
            //}
            ct++;
        }
        CheckClear();
        return cleared.Count;
    }
    public int CheckMatching()
    {
        int dx = -1;
        int dy = -1;
        int find = -1;
        for (int x = 0; x <= sizex; x++)
        {
            for (int y = 0; y <= sizey; y++)
            {
                find = FindPos(new Vector2Int(x, y));
                if (find >= 0)
                {
                    if (squares[find].color >= 0)
                    {
                        dx = squares[find].pos.x;
                        dy = squares[find].pos.y;
                        toclear.Clear();
                        cleared.Clear();
                        //if (
                        CheckAcross(squares[find].color, dx, dy); // >= reqTiles; ||
                        CheckDown(squares[find].color, dx, dy); // >= reqTiles ||
                        CheckDiagonalLeft(squares[find].color, dx, dy); // >= reqTiles ||
                        CheckDiagonalRight(squares[find].color, dx, dy); // >= reqTiles
                            //)
                        //{
                            Debug.Log(cleared.Count + " tiles found");
                        //}
                        if (cleared.Count >= reqTiles) break;
                    }
                }
            }
            if (cleared.Count >= reqTiles) break;
        }
        return cleared.Count;
    }
    public int CheckOutOfBounds()
    {
        int amount = 0;
        for (int i = 0; i < squares.Count; i++)
        {
            if (squares[i].pos.x < 0 || squares[i].pos.y < 0 || squares[i].pos.x >= sizex || squares[i].pos.y >= sizey)
            {
                game.lives--;
                squares.RemoveAt(i);
                timer = 1f / speed;
                amount++;
                i--;
            }
        }
        return amount;
    }
    public void SpawnSquares()
    {
        if (queue.Count <= 8) PopulateQueue(12);
        else
        {
            int color = queue[0].color;
            int shape = queue[0].direction; // 1 = two bar, 2 = three bar, 3 = l shape
            int dir = queue.Count % 4; // 0 = down, 1 = left, 2 = up, 3 = right
            Vector2Int[] positions = { new Vector2Int(12, 29), new Vector2Int(29, 12), new Vector2Int(12, -5), new Vector2Int(-5, 12) };

            squares.Add(new Shapes { controlled = true, main = true, pos = positions[dir], color = color, category = 0, direction = dir });
            squares.Add(new Shapes { controlled = true, main = false, pos = new Vector2Int(positions[dir].x + 1, positions[dir].y), color = color, category = 0, direction = dir });

            switch (shape) {
                case 2:
                    squares.Add(new Shapes { controlled = true, main = false, pos = new Vector2Int(positions[dir].x - 1, positions[dir].y), color = color, category = 0, direction = dir });
                    break;
                case 3:
                    squares.Add(new Shapes { controlled = true, main = false, pos = new Vector2Int(positions[dir].x, positions[dir].y + 1), color = color, category = 0, direction = dir });
                    break;
                default:
                    break;
            }
            queue.RemoveAt(0);
        }
        
    }
    public void CheckControlledCollisions()
    {
        for (int k = 0; k < squares.Count; k++)
        {
            if (squares[k].controlled)
            {
                if (CheckCollision(k, 0))
                {
                    game.score += perLanded * game.level;
                    playerTurn = false;
                }
            }
        }
    }
    void Start()
    {
        ClearBoard();
        ticks = 0;
        moveAction = InputSystem.actions.FindAction("Move");
        rotateLeft = InputSystem.actions.FindAction("Rotate Left");
        rotateRight = InputSystem.actions.FindAction("Rotate Right");
        game = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Interface>();
    }
    void Update()
    {
        if (game.lives > 0)
        {
            speed = (float)(game.level + speedPlusLevel) * speedPerLevel;
            if (timer <= 0f)
            {
                if (FindMain() >= 0)
                {
                    if (playerTurn) CheckControlledCollisions();
                    for (int k = 0; k < squares.Count; k++)
                    {
                        if (squares[k].controlled)
                        {
                            if (!playerTurn)
                            {
                                squares[k].controlled = false;
                                squares[k].main = false;
                            }
                            else squares[k].pos = MoveSquare(k, 0);
                            if (squares[k].main) ticks++;
                        }
                    }
                }
                else
                {
                    CheckMatching();
                    //if (squares.Count >= 1 && CheckMatching() >= 1)
                    //{
                        if (cleared.Count > 0)
                        {
                            int clearBonus = perMatch;
                            playerTurn = false;
                            //while (cleared.Count > 0)
                            for (int i = squares.Count - 1; i >= 0; i--)
                            {
                                if (cleared.Contains(i))
                                {
                                    game.score += clearBonus * game.level;
                                    if (clearBonus == perMatch) game.score += perMatch;
                                    clearBonus *= perExtraMatch;
                                    cleared.RemoveAt(cleared.IndexOf(i));
                                    squares.RemoveAt(i);
                                    Debug.Log("Removed tile #" + i);
                                }
                                timer = 1f / speed;
                            }
                        }
                    //}
                    else if (CheckOutOfBounds() == 0 && cleared.Count <= 0)
                    {
                        playerTurn = true;
                        ticks = 0;
                        SpawnSquares();
                    }
                }
                DrawBoard();
                timer = 1f / speed;
            }
            else
            {
                timer -= Time.deltaTime;
            }
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            if (moveValue.x == 0 && moveValue.y == 0) moveTimer = 0f;
            else
            {
                if (moveTimer <= 0f)
                {
                    int d = squares[FindMain()].direction;
                    int d2 = 0;
                    if (moveValue.y > 0)
                    {
                        d2 = (2 - d) % 4;
                        if (d != 0) MoveInput(d2);
                    }
                    if (moveValue.x > 0)
                    {
                        d2 = (3 - d) % 4;
                        if (d != 1) MoveInput(d2);
                    }
                    if (moveValue.y < 0)
                    {
                        d2 = (0 - d) % 4;
                        if (d != 2) MoveInput(d2);
                    }
                    if (moveValue.x < 0)
                    {
                        d2 = (1 - d) % 4;
                        if (d != 3) MoveInput(d2);
                    }
                    if (d2 == 0) ticks++;
                    DrawBoard();
                }
                moveTimer += Time.deltaTime;
            }
            if (!rotateLeft.IsPressed() && !rotateRight.IsPressed()) moveTimer2 = 0f;
            else
            {
                if (moveTimer2 <= 0f)
                {
                    if (rotateLeft.IsPressed()) RotateSquares(0);
                    if (rotateRight.IsPressed()) RotateSquares(1);
                    DrawBoard();
                }
                moveTimer2 += Time.deltaTime;
            }
        }
    }
}
