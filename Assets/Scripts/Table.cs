using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
	public Board board;
	public GameObject whiteChecker, blackChecker;

	private void Start()
	{
	}
	public void CreatBoard()
	{
		board = new Board();
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
		for(int i = 0; i < 8; i++)
		{
			for(int j = 0; j < 8; j++)
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

	public Board()
	{
		for (int y = 0; y < 8; y++)
		{
			for (int x = y % 2; x < 8; x += 2)
			{
				if(y < 3)
				{
					_board[y, x] = new Checker(new Vector3Int(x, y), true, false);
					whiteCheckers.Add(_board[y, x]);
				}
				else if (y > 4)
				{
					_board[y, x] = new Checker(new Vector3Int(x, y), false, false);
					blackCheckers.Add(_board[y, x]);
				}
			}
		}
		isEndGame = false;
		isWhiteMove = true;
	}
	public Board(List<Checker> allCheckers)
	{
		foreach(Checker c in allCheckers)
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
		for(int y = 0; y < 8; y++)
		{
			for(int x = y % 2; x < 8; x+=2)
			{
				if(b[y,x] != null)_board[y, x] = new Checker(b[y, x]);
				if(_board[y, x] != null)
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
			for(int i = 0; i < move.pos.Count - 1; i++)
			{
				if(Board.AbleKick(_board[move.pos[i].y, move.pos[i].x], move.pos[i+1], this))
				{
					Board.Kick(move.pos[i].y, move.pos[i].x, move.pos[i+1].x, move.pos[i+1].x, this);
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
		for(int y = 7; y >= 0; y--)
		{
			for(int x = 0; x < 8; x++)
			{
				res += (_board[y, x] != null) ? (_board[y, x].isWhite ? " W " : " B ") : "    " ;
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
		b[sy, sx] = null;
		c.position.x = ex; c.position.y = ey;
		if(visual)
		{
			//c.checkerControll.SetPosition();
			b[(sy + ey) / 2, (sx + ex) / 2].checkerControll.DestroyMe();
		}
		b[(sy + ey) / 2, (sx + ex) / 2] = null;
		b[ey, ex] = c;
		if (c.isWhite && c.position.y == 7) c.SetQueen(true);
		if (!c.isWhite && c.position.y == 0) c.SetQueen(true);
		return b;
	}
	public static Board RealiseMove(Move move, Board b, bool visual = false)
	{
		for(int i = 0; i < move.pos.Count - 1; i++)
		{
			Checker c = b[move.pos[i].y, move.pos[i].x];
			b[move.pos[i].y, move.pos[i].x] = null;
			c.position = move.pos[i + 1];
			if(visual)
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
		if(Mathf.Abs(ty - c.position.y) != 1 || Mathf.Abs(tx - c.position.x) != 1)
		{
			c.isAbleMove = false;
			return false;
		}
		c.isAbleMove = true;
		if (b.InField(pos) && (((ty - c.position.y > 0) == c.isWhite) || c.isQueen))
		{
			if (b.IsEmpty(pos)) return true;
		}

		c.isAbleMove = false;
		return false;
	}
	public static bool AbleMove(Checker c, Board b)
	{
		int tx, ty;
		if (b.isEndGame) return false;

		c.isAbleMove = true;
		for (ty = c.position.y - 1; ty < c.position.y + 2; ty += 2)
		{
			for (tx = c.position.x - 1; tx < c.position.x + 2; tx += 2)
			{
				if (b.InField(tx, ty) && (((ty - c.position.y > 0) == c.isWhite) || c.isQueen))
				{
					if (b.IsEmpty(tx, ty)) return true;
				}
			}
		}

		c.isAbleMove = false;
		return false;
	}
	public static bool AbleKick(Checker c, Vector3Int pos, Board b)
	{
		if (b.isEndGame) return false;
		c.isNeedAttack = true;
		c.isAbleMove = true;
		Vector3Int middle = (pos + c.position) / 2;

		if (Mathf.Abs(c.position.y - pos.y) != 2 || Mathf.Abs(c.position.x - pos.x) != 2) return false;

		if (b.InField(pos) && (((pos.y - c.position.y > 0) == c.isWhite) || c.isQueen))
		{
			if (b.IsEmpty(pos) && !b.IsEmpty(middle) && b[middle.y, middle.x].isWhite != c.isWhite) return true;
		}
		return false;
	}
	public static bool AbleKick(Checker c, Board b)
	{
		if (b.isEndGame) return false;
		int ex, ey, tx, ty;
		c.isNeedAttack = true;
		c.isAbleMove = true;
		for (ty = c.position.y - 2; ty < c.position.y + 3; ty += 4)
		{
			for (tx = c.position.x - 2; tx < c.position.x + 3; tx += 4)
			{
				ey = (ty + c.position.y) / 2; ex = (tx + c.position.x) / 2;
				if (b.InField(tx, ty) && ((ty - c.position.y > 0) == c.isWhite || c.isQueen))
				{
					if (b.IsEmpty(tx, ty) && !b.IsEmpty(ex, ey) && b[ey, ex].isWhite != c.isWhite) return true;
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

		foreach(Checker c in checkers)
		{
			allMoves.AddRange(Board.GetAbleKick(b[c], b));
		}

		if(allMoves.Count == 0)
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
		for (int ty = c.position.y - 1; ty < c.position.y + 2; ty += 2)
		{
			for (int tx = c.position.x - 1; tx < c.position.x + 2; tx += 2)
			{
				if (Board.AbleMove(b[c], new Vector3Int(tx, ty), b))
				{
					moves.Add(new Move(new List<Vector3Int> { c.position, new Vector3Int(tx, ty) }, new List<Vector3Int>()));
				}
			}
		}
		return moves;
	}
	public static List<Move> GetAbleKick(Checker c, Board b, Move move = null)
	{
		List<Move> moves = new List<Move>();
		for (int ty = c.position.y - 2; ty < c.position.y + 3; ty += 4)
		{
			for (int tx = c.position.x - 2; tx < c.position.x + 3; tx += 4)
			{
				if (Board.AbleKick(b[c], new Vector3Int(tx, ty), b))
				{
					Move addedMove = move;
					if (addedMove == null) addedMove = new Move(new List<Vector3Int> { c.position, new Vector3Int(tx, ty) }, 
																new List<Vector3Int> { new Vector3Int((tx + c.position.x) / 2, (ty + c.position.y) / 2) });
					else
					{
						addedMove = new Move(move);
						addedMove.pos.Add(new Vector3Int(tx, ty));
						addedMove.taken.Add(new Vector3Int((tx + c.position.x) / 2, (ty + c.position.y) / 2));
					}
					Board moveBoard = new Board(b);
					moveBoard = Board.Kick(moveBoard[c], new Vector3Int(tx, ty), moveBoard);
					moves.AddRange(GetAbleKick(moveBoard[addedMove.pos[^1].y, addedMove.pos[^1].x], moveBoard, addedMove));
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
	public static void IterativeDeepeningMinimax(Board board, int minDepth, int maxDepth, ref Move bestMove, ref int depth, bool isWhiteMove)
	{

		for (depth = minDepth; depth <= maxDepth; depth++)
		{
			(float eval, Move tempBestMove) = Minimax(board, depth, isWhiteMove);
			if (tempBestMove != null)
			{
				bestMove =  new Move(tempBestMove);
			}
			else
			{
				depth -= 1;
				break;
			}

			if (eval >= 1000 && isWhiteMove || eval <= -1000 && !isWhiteMove)
				break;
		}
	}
	public static (float, Move) MiniMax(Board board, int depth, bool isMaximizing)
	{
		if (depth == 0) return (Evaluate(board), null);
		if(isMaximizing)
		{
			List<Move> allMoves = Board.GetAllAbleMoves(new Board(board), 1);

			if(allMoves.Count == 0)
			{
				return (Evaluate(board), null);
			}
			Move bestMove = new Move(new List<Vector3Int>(), new List<Vector3Int>());
			float bestValue = -9999;

			foreach(Move m in allMoves)
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
	public static (float, Move) Minimax(Board board, int depth, bool maximizingPlayer)
	{
		List<Move> allMoves = Board.GetAllAbleMoves(board, maximizingPlayer ? 1 : 0);

		if (depth == 0)
		{
			float eval = Board.Evaluate(board);
			return (eval, null);
		}

		// Если ход единственный -- см. комментарии под кодом
		if (allMoves.Count == 1)
		{
			Board next_board = Board.RealiseMove(allMoves[0], board);

			float eval = Minimax(next_board, depth, !maximizingPlayer).Item1;
			return (eval, allMoves[0]);
		}

		// Ищем лучший ход (за белых)
		Move bestMove = null;
		if (maximizingPlayer)
		{
			float maxEval = -1000;
			foreach (Move move in allMoves)
			{
				Board next_board = Board.RealiseMove(move, new Board(board));

				(float eval, Move compMove) = Minimax(next_board, depth - 1, false);

				if (compMove == null)
					return (0, null);
				if (eval > maxEval)
				{
					maxEval = eval;
					bestMove = move;
				}
			}
			return (maxEval, bestMove);
		}
		else
		{
			float minEval = 1000;
			foreach (Move move in allMoves)
			{
				Board next_board = Board.RealiseMove(move, new Board(board));

				(float eval, Move compMove) = Minimax(next_board, depth - 1, true);

				if (compMove == null)
					return (0, null);
				if (eval < minEval)
				{
					minEval = eval;
					bestMove = move;
				}
			}
			return (minEval, bestMove);
		}
	}
}
