using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Board board;
    public GameObject whiteChecker, blackChecker;
    //public int[,] startBoard =
    //{
    //	{ 1, 0, 1, 0, 0, 0, 0, 0},
    //	{ 0, 0, 0, 0, 0, 0, 0, 0},
    //	{ 0, 0, 0, 0, 0, 0, 0, 0},
    //	{ 0, 1, 0, 1, 0, 1, 0, 0},
    //	{ 1, 0, 1, 0, 1, 0, 0, 0},
    //	{ 0, 0, 0, 0, 0, 0, 0, 0},
    //	{ 2, 0, 0, 0, 0, 0, 0, 0},
    //	{ 0, 0, 0, 0, 0, 0, 0, 0},
    //};

    public int[,] startBoard =
    {
        { 1, 0, 1, 0, 1, 0, 1, 0},	//	(0, 0) ...... (7, 0)
		{ 0, 1, 0, 1, 0, 1, 0, 1},	//	.
		{ 1, 0, 1, 0, 1, 0, 1, 0},	//	.
		{ 0, 0, 0, 0, 0, 0, 0, 0},	//	.
		{ 0, 0, 0, 0, 0, 0, 0, 0},	//	.
		{ 0, 2, 0, 2, 0, 2, 0, 2},	//	.
		{ 2, 0, 2, 0, 2, 0, 2, 0},	//	.
		{ 0, 2, 0, 2, 0, 2, 0, 2},	//	(0, 7)
	};
    private void Start()
    {
    }
    public void CreatBoard()
    {
        board = new Board(startBoard);
        foreach (Checker c in board.whiteCheckers)
        {
            c.ChangeQueen += SetQueen;
            GameObject go = Instantiate(whiteChecker, c.position + transform.position, Quaternion.identity);
            CheckerControll cc = go.GetComponent<CheckerControll>();
            cc.table = this;
            cc.checker = c;
            c.checkerControll = cc;
            SetQueen(c, false);
        }
        foreach (Checker c in board.blackCheckers)
        {
            c.ChangeQueen += SetQueen;
            GameObject go = Instantiate(blackChecker, c.position + transform.position, Quaternion.identity);
            CheckerControll cc = go.GetComponent<CheckerControll>();
            cc.table = this;
            cc.checker = c;
            c.checkerControll = cc;
            SetQueen(c, false);
        }
    }
    public void Clear()
    {
        if (board == null) return;
        foreach (Checker c in board.whiteCheckers)
        {
            c.checkerControll.DestroyMe();
        }
        foreach (Checker c in board.blackCheckers)
        {
            c.checkerControll.DestroyMe();
        }
    }
    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Gizmos.color = board[j, i] != null ? Color.white : Color.red;
                Gizmos.DrawCube(new Vector3(i, j) + transform.position, Vector3.one);
            }
        }
    }
    public void SetQueen(Checker checker, bool queen)
    {
        checker.isQueen = queen;
        checker.checkerControll.crown.SetActive(checker.isQueen);
    }
}

public class Board
{
    private Checker[,] _board = new Checker[8, 8];
    public bool isWhiteMove, isEndGame;
    public List<Checker> whiteCheckers = new List<Checker>(), blackCheckers = new List<Checker>();

    public Board(int[,] intBoard)
    {
        whiteCheckers = new List<Checker>();
        blackCheckers = new List<Checker>();
        for (int y = 0; y < 8; y++)
        {
            for (int x = y % 2; x < 8; x += 2)
            {
                if (intBoard[y, x] == 1)
                {
                    _board[y, x] = new Checker(new Vector3Int(x, y), true, false);
                    whiteCheckers.Add(_board[y, x]);
                }
                else if (intBoard[y, x] == 2)
                {
                    _board[y, x] = new Checker(new Vector3Int(x, y), false, false);
                    blackCheckers.Add(_board[y, x]);
                }
            }
        }
        Debug.Log(whiteCheckers.Count);
        Debug.Log(blackCheckers.Count);
        isEndGame = false;
        isWhiteMove = true;
    }
    public Board(List<Checker> allCheckers)
    {
        whiteCheckers = new List<Checker>();
        blackCheckers = new List<Checker>();
        foreach (Checker c in allCheckers)
        {
            if (c.isWhite) whiteCheckers.Add(c);
            else blackCheckers.Add(c);
            allCheckers = whiteCheckers;
            allCheckers.AddRange(blackCheckers);

            _board[c.position.y, c.position.x] = c;
        }
        isEndGame = false;
        isWhiteMove = true;
    }
    public Board(Board b, Move move = null)
    {
        whiteCheckers = new List<Checker>();
        blackCheckers = new List<Checker>();
        for (int y = 0; y < 8; y++)
        {
            for (int x = y % 2; x < 8; x += 2)
            {
                if (b[y, x] != null) _board[y, x] = new Checker(b[y, x]);
                if (_board[y, x] != null)
                {
                    if (_board[y, x].isWhite) whiteCheckers.Add(_board[y, x]);
                    else blackCheckers.Add(_board[y, x]);
                }
            }
        }
        isEndGame = false;
        isWhiteMove = true;
        if (move != null)
        {
            for (int i = 0; i < move.pos.Count - 1; i++)
            {
                (bool, Vector3Int) kickStatus = Board.AbleKick(_board[move.pos[i].y, move.pos[i].x], move.pos[i + 1], this);
                if (kickStatus.Item1)
                {
                    Board.Kick(move.pos[i].y, move.pos[i].x, move.pos[i + 1].x, move.pos[i + 1].x, this);
                }
                else
                {
                    Board.Move(move.pos[i].x, move.pos[i].y, move.pos[i + 1].x, move.pos[i + 1].y, this);
                    isWhiteMove = !isWhiteMove;
                }
            }
        }
    }
    public Checker this[int y, int x]
    {
        get
        {
            return _board[y, x];
        }
        set
        {
            _board[y, x] = value;
        }
    }
    public Checker this[Checker c]
    {
        get
        {
            return new Checker(_board[c.position.y, c.position.x]);
        }
    }
    public bool IsEmpty(Vector3Int pos)
    {
        return InField(pos) && _board[pos.y, pos.x] == null;
    }
    public bool IsEmpty(int x, int y)
    {
        return InField(x, y) && _board[y, x] == null;
    }
    public bool InField(Vector3Int pos)
    {
        return pos.y >= 0 && pos.y < 8 && pos.x >= 0 && pos.x < 8;
    }
    public bool InField(int x, int y)
    {
        return y >= 0 && y < 8 && x >= 0 && x < 8;
    }
    public Checker[,] GetCheckersBoard()
    {
        return _board;
    }
    public override string ToString()
    {
        string res = "";
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                res += (_board[y, x] != null) ? (_board[y, x].isWhite ? " W " : " B ") : "    ";
            }
            res += "\n";
        }
        return res;
    }


    public void SetPosition(Checker checker, Vector3Int pos, Board board)
    {
        if (checker.isWhite && pos.y == 7) checker.SetQueen(true);
        if (!checker.isWhite && pos.y == 0) checker.SetQueen(true);

        checker.checkerControll.SetPosition();

        board[checker.position.y, checker.position.x] = null;
        checker.position.x = pos.x; checker.position.y = pos.y;
        board[checker.position.y, checker.position.x] = checker;

    }

    public static Board Move(Checker c, Vector3Int pos, Board b, bool visual = false)
    {
        return Move(c.position.x, c.position.y, pos.x, pos.y, b, visual);
    }
    public static Board Move(int sx, int sy, int ex, int ey, Board b, bool visual = false)
    {
        Checker c = b[sy, sx];
        b[sy, sx] = null;
        c.position.x = ex; c.position.y = ey;

        if (visual) c.checkerControll.SetPosition();

        b[ey, ex] = c;
        if (c.isWhite && c.position.y == 7) c.SetQueen(true);
        if (!c.isWhite && c.position.y == 0) c.SetQueen(true);
        return b;
    }
    public static Board Kick(Checker c, Vector3Int pos, Board b, bool visual = false, bool newBoard = false)
    {
        return Kick(c.position.x, c.position.y, pos.x, pos.y, b, visual, newBoard);
    }
    public static Board Kick(int sx, int sy, int ex, int ey, Board b, bool visual = false, bool newBoard = false)
    {
        Checker c = b[sy, sx];
        Vector3Int takenChecker = Board.AbleKick(c, new Vector3Int(ex, ey), b).Item2;

        if (newBoard) b = new Board(b);

        b[sy, sx] = null;
        c.position.x = ex; c.position.y = ey;
        if (visual)
        {
            //c.checkerControll.SetPosition();
            b[takenChecker.y, takenChecker.x].checkerControll.DestroyMe();
        }
        b[takenChecker.y, takenChecker.x] = null;
        b[ey, ex] = c;
        if (c.isWhite && c.position.y == 7) c.SetQueen(true);
        if (!c.isWhite && c.position.y == 0) c.SetQueen(true);
        return b;
    }
    public static Board RealiseMove(Move move, Board b, bool visual = false)
    {
        for (int i = 0; i < move.pos.Count - 1; i++)
        {
            Checker c = b[move.pos[i].y, move.pos[i].x];
            b[move.pos[i].y, move.pos[i].x] = null;
            c.position = move.pos[i + 1];
            if (visual)
            {
                //c.checkerControll.SetPosition();
                if (move.taken.Count > 0) b[move.taken[i].y, move.taken[i].x].checkerControll.DestroyMe();
            }
            if (move.taken.Count > 0)
            {
                b[move.taken[i].y, move.taken[i].x] = null;
            }
            b[move.pos[i + 1].y, move.pos[i + 1].x] = c;
            if (c.isWhite && c.position.y == 7) c.SetQueen(true);
            if (!c.isWhite && c.position.y == 0) c.SetQueen(true);
        }
        b.SetCheckers();
        return b;
    }
    public static bool AbleMove(Checker c, Vector3Int pos, Board b)
    {
        int tx, ty;
        ty = pos.y; tx = pos.x;
        if (b.isEndGame) return false;
        if (!b.InField(pos) || (Mathf.Abs(ty - c.position.y) != Mathf.Abs(tx - c.position.x)) || (!c.isQueen && Mathf.Abs(ty - c.position.y) > 1))
        {
            c.isAbleMove = false;
            return false;
        }
        c.isAbleMove = true;
        if (!c.isQueen && ((ty - c.position.y > 0) == c.isWhite))
        {
            if (b.IsEmpty(pos)) return true;
        }
        if (c.isQueen)
        {
            Vector3Int dictionary = new Vector3Int((pos.x - c.position.x) / Mathf.Abs(pos.x - c.position.x), (pos.y - c.position.y) / Mathf.Abs(pos.y - c.position.y));
            Vector3Int pos_target = c.position + dictionary;

            while (b.InField(pos_target) && pos_target - dictionary != pos)
            {
                if (!b.IsEmpty(pos_target))
                {
                    c.isAbleMove = false;
                    return false;
                }

                pos_target += dictionary;
            }
            return true;
        }

        c.isAbleMove = false;
        return false;
    }
    public static bool AbleMove(Checker c, Board b)
    {
        int tx, ty;
        if (b.isEndGame) return false;

        c.isAbleMove = true;
        if (!c.isQueen)
        {
            for (ty = -1; ty < 2; ty += 2)
            {
                for (tx = -1; tx < 2; tx += 2)
                {
                    Vector3Int dictionary = new Vector3Int(tx, ty);
                    Vector3Int pos_target = c.position + dictionary;

                    if (b.InField(pos_target) && b.IsEmpty(pos_target))
                    {
                        return true;
                    }
                    else if (b.InField(pos_target))
                    {
                        break;
                    }
                }
            }
        }
        if (c.isQueen)
        {
            for (ty = -1; ty < 2; ty += 2)
            {
                for (tx = -1; tx < 2; tx += 2)
                {
                    Vector3Int dictionary = new Vector3Int(tx, ty);
                    Vector3Int pos_target = c.position + dictionary;

                    if (b.InField(pos_target) && b.IsEmpty(pos_target))
                    {
                        return true;
                    }
                    else if (b.InField(pos_target))
                    {
                        break;
                    }
                }
            }
        }


        c.isAbleMove = false;
        return false;
    }
    public static (bool, Vector3Int) AbleKick(Checker c, Vector3Int pos, Board b)
    {
        if (b.isEndGame || !b.InField(pos)) return (false, -Vector3Int.one);
        c.isNeedAttack = true;
        c.isAbleMove = true;

        if (Mathf.Abs(c.position.y - pos.y) != Mathf.Abs(c.position.x - pos.x)) return (false, -Vector3Int.one);

        if (!c.isQueen)
        {
            // Kick to simple Checker
            if (Mathf.Abs(c.position.y - pos.y) != 2) return (false, -Vector3Int.one);

            Vector3Int middle = (pos + c.position) / 2;
            if (b.IsEmpty(pos) && !b.IsEmpty(middle) && b[middle.y, middle.x].isWhite != c.isWhite) return (true, middle);
        }
        if (c.isQueen)
        {
            // Kick to Queen Checker
            Vector3Int direction = new Vector3Int((pos.x - c.position.x) / Mathf.Abs(pos.x - c.position.x), (pos.y - c.position.y) / Mathf.Abs(pos.y - c.position.y));
            Vector3Int pos_target = c.position + direction;

            bool opp_found = false, kick_onWay = false;
            Vector3Int opp_pos = -Vector3Int.one;

            while (pos_target - direction != pos)
            {
                if (b.IsEmpty(pos_target))
                {
                    if (opp_found) // If we jumping to the target cell and also jump over the opponent's checker, then this is a capture
                        kick_onWay = true;
                }
                else if (b[pos_target.y, pos_target.x].isWhite == c.isWhite) // If we jump into our checker - that's all
                {
                    c.isNeedAttack = false;
                    c.isAbleMove = false;
                    return (false, -Vector3Int.one);
                }
                else
                {
                    if (!opp_found) // If pos_target have enemy checker then remember it
                    {
                        opp_found = true;
                        opp_pos = pos_target;
                    }
                    else // If we jump into second enemy checker - that's all
                    {
                        c.isNeedAttack = false;
                        c.isAbleMove = false;
                        return (false, -Vector3Int.one);
                    }
                }
                pos_target += direction;
            }
            if (kick_onWay)
                return (true, opp_pos);
            else
            {
                c.isNeedAttack = false;
                c.isAbleMove = false;
                return (false, -Vector3Int.one);
            }
        }
        c.isNeedAttack = false;
        c.isAbleMove = false;
        return (false, -Vector3Int.one);
    }
    public static bool AbleKick(Checker c, Board b)
    {
        if (b.isEndGame) return false;
        c.isNeedAttack = true;
        c.isAbleMove = true;
        if (!c.isQueen)
        {
            int ex, ey, tx, ty;
            for (ty = c.position.y - 2; ty < c.position.y + 3; ty += 4)
            {
                for (tx = c.position.x - 2; tx < c.position.x + 3; tx += 4)
                {
                    ey = (ty + c.position.y) / 2; ex = (tx + c.position.x) / 2;
                    if (b.InField(tx, ty))
                    {
                        // Kick to simple Checker
                        if (b.IsEmpty(tx, ty) && !b.IsEmpty(ex, ey) && b[ey, ex].isWhite != c.isWhite) return true;
                    }
                }
            }
        }
        else
        {
            for (int ty = -1; ty < 2; ty += 2)
            {
                for (int tx = -1; tx < 2; tx += 2)
                {
                    bool opp_found = false;

                    Vector3Int pos_target = c.position + new Vector3Int(tx, ty);
                    while (b.InField(pos_target))
                    {
                        if (b.IsEmpty(pos_target))
                        {
                            if (opp_found) // If we jumping to the target cell and also jump over the opponent's checker, then this is a capture
                                return true;
                        }
                        else if (b[pos_target.y, pos_target.x].isWhite == c.isWhite) // If we jump into our checker - that's all
                            break;
                        else
                        {
                            if (!opp_found) // If pos_target have enemy checker then remember it
                            {
                                opp_found = true;
                            }
                            else // If we jump into second enemy checker - that's all
                                break;
                        }
                        pos_target += new Vector3Int(tx, ty);
                    }
                }
            }
        }

        c.isNeedAttack = false;
        c.isAbleMove = false;
        return false;
    }
    public static List<Move> GetAllAbleMoves(Board b, int color = 0)
    {
        List<Checker> checkers = (color == 1) ? b.whiteCheckers : b.blackCheckers;
        List<Move> allMoves = new List<Move>();
        if (checkers.Count == 0) return allMoves;

        foreach (Checker c in checkers)
        {
            allMoves.AddRange(Board.GetAbleKick(b[c], b));
        }

        if (allMoves.Count == 0)
        {
            foreach (Checker c in checkers)
            {
                allMoves.AddRange(Board.GetAbleMoves(b[c], b));
            }
        }
        return allMoves;
    }
    public static List<Move> GetAbleMoves(Checker c, Board b)
    {
        List<Move> moves = new List<Move>();

        for (int ty = -1; ty < 2; ty += 2)
        {
            for (int tx = -1; tx < 2; tx += 2)
            {
                Vector3Int pos_target = c.position + new Vector3Int(tx, ty);
                while (b.InField(pos_target))
                {
                    if (Board.AbleMove(b[c], pos_target, b))
                    {
                        moves.Add(new Move(new List<Vector3Int> { c.position, pos_target }, new List<Vector3Int>()));
                    }
                    else
                        break;
                    pos_target += new Vector3Int(tx, ty);
                }

            }
        }
        return moves;
    }
    public static List<Move> GetAbleKick(Checker c, Board b, Move move = null)
    {
        List<Move> moves = new List<Move>();
        for (int ty = -1; ty < 2; ty += 2)
        {
            for (int tx = -1; tx < 2; tx += 2)
            {
                (bool, Vector3Int) kick_status;

                Vector3Int pos_target = c.position + new Vector3Int(tx, ty);
                while (b.InField(pos_target))
                {
                    kick_status = Board.AbleKick(b[c], pos_target, b);
                    if (kick_status.Item1)
                    {
                        Debug.Log(kick_status.Item1 + " " + c.position + " -> " + pos_target);
                        Move addedMove = move;
                        if (addedMove == null) addedMove = new Move(new List<Vector3Int> { c.position, pos_target },
                                                                    new List<Vector3Int> { kick_status.Item2 });
                        else
                        {
                            addedMove = new Move(move);
                            addedMove.pos.Add(pos_target);
                            addedMove.taken.Add(kick_status.Item2);
                        }
                        Debug.Log(addedMove);
                        Board movedBoard = new Board(b);
                        movedBoard = Board.Kick(movedBoard[c], pos_target, movedBoard);
                        moves.AddRange(GetAbleKick(movedBoard[addedMove.pos[^1].y, addedMove.pos[^1].x], movedBoard, addedMove));
                    }
                    pos_target += new Vector3Int(tx, ty);
                }

            }
        }
        if (moves.Count == 0 && move != null) moves.Add(move);
        return moves;
    }
    public void SetCheckers()
    {
        whiteCheckers.Clear();
        blackCheckers.Clear();
        for (int y = 0; y < 8; y++)
        {
            for (int x = y % 2; x < 8; x += 2)
            {
                if (!IsEmpty(x, y))
                {
                    if (_board[y, x].isWhite) whiteCheckers.Add(_board[y, x]);
                    else blackCheckers.Add(_board[y, x]);
                }
            }
        }
    }
    public static float Evaluate(Board board)
    {
        board.SetCheckers();
        List<Checker> allCheckers = board.whiteCheckers;
        allCheckers.AddRange(board.blackCheckers);
        float eval = 0;
        float[,] _squareBonus = new float[8, 4] // бонус клетки
		{
            { 1.2f, 1.2f, 1.2f, 1.2f },
            { 1.15f, 1.2f, 1.2f, 1.15f },
            { 1.15f, 1.2f, 1.2f, 1.13f },
            { 1.0f, 1.2f, 1.15f, 1.0f },
            { 1.0f, 1.2f, 1.2f, 1.0f },
            { 1.0f, 1.0f, 1.0f, 1.0f },
            { 1.0f, 1.0f, 1.0f, 1.0f },
            { 1.0f, 1.0f, 1.0f, 1.0f },
        };

        foreach (Checker c in allCheckers)
        {
            Vector3Int coord = c.position;
            eval += (c.isWhite ? 1.0f : -1.0f) * (c.isQueen ? 2.5f : 1.0f) * (c.isWhite ? _squareBonus[coord.y, coord.x / 2] : _squareBonus[7 - coord.y, 3 - coord.x / 2]);
        }
        return eval;
    }

    public static (float, Move) MiniMax(Board board, int depth, bool isMaximizing)
    {
        if (depth == 0) return (Evaluate(board), null);
        if (isMaximizing)
        {
            List<Move> allMoves = Board.GetAllAbleMoves(new Board(board), 1);

            if (allMoves.Count == 0)
            {
                return (Evaluate(board), null);
            }
            Move bestMove = new Move(new List<Vector3Int>(), new List<Vector3Int>());
            float bestValue = -9999;

            foreach (Move m in allMoves)
            {
                Board next_board = RealiseMove(m, new Board(board));
                float e = MiniMax(next_board, depth - 1, !isMaximizing).Item1;
                if (e > bestValue)
                {
                    bestValue = e;
                    bestMove = m;
                }
            }
            return (bestValue, bestMove);
        }
        else
        {
            List<Move> allMoves = Board.GetAllAbleMoves(board, 0);

            if (allMoves.Count == 0)
            {
                return (Evaluate(board), null);
            }
            Move bestMove = null;
            float bestValue = 9999;

            foreach (Move m in allMoves)
            {
                Board next_board = RealiseMove(m, new Board(board));
                float e = MiniMax(next_board, depth - 1, !isMaximizing).Item1;
                if (e < bestValue)
                {
                    bestValue = e;
                    bestMove = m;
                }
            }
            return (bestValue, bestMove);
        }
    }
}
