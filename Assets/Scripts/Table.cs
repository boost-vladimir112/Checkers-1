using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
	public Board board;

	private void Start()
	{

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

}

public class Board
{
	private Checker[,] _board = new Checker[8, 8];
	public bool isWhiteMove;
	public List<Checker> whiteCheckers = new List<Checker>(), blackCheckers = new List<Checker>();

	public Board(Board b)
	{
		_board = b.GetCheckersBoard();
		whiteCheckers = b.whiteCheckers;
		blackCheckers = b.blackCheckers;
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
		return b;
	}
	public static Board Kick(Checker c, Vector3Int pos, Board b, bool visual = false)
	{
		return Kick(c.position.x, c.position.y, pos.x, pos.y, b, visual);
	}
	public static Board Kick(int sx, int sy, int ex, int ey, Board b, bool visual = false)
	{
		Checker c = b[sy, sx];
		b[sy, sx] = null;
		c.position.x = ex; c.position.y = ey;
		if(visual)
		{
			c.checkerControll.SetPosition();
			b[(sy + ey) / 2, (sx + ex) / 2].checkerControll.DestroyMe();
		}
		b[(sy + ey) / 2, (sx + ex) / 2] = null;
		b[ey, ex] = c;
		return b;
	}
	public static bool AbleMove(Checker c, Vector3Int pos, Board b)
	{
		int tx, ty;
		ty = pos.y; tx = pos.x;

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
		c.isNeedAttack = true;
		c.isAbleMove = true;
		Vector3Int middle = pos + c.position / 2;
		if (Mathf.Abs(c.position.y - pos.y) != 2 || Mathf.Abs(c.position.x - pos.x) != 2) return false;

		if (b.InField(pos) && (((pos.y - c.position.y > 0) == c.isWhite) || c.isQueen))
		{
			if (b.IsEmpty(pos) && !b.IsEmpty(middle) && b[pos.y, pos.x].isWhite != c.isWhite) return true;
		}
		return false;
	}
	public static bool AbleKick(Checker c, Board b)
	{
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
	public void SetCheckers()
	{
		for(int y = 0; y < 8; y++)
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
}
