using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Checker))]
public class CheckerControll : MonoBehaviour
{
	Checker checker;
	public Table table;

	private void Start()
	{
		checker = GetComponent<Checker>();
	}
	public void SetQueen(bool queen)
	{
		checker.isQueen = queen;
		checker.crown.SetActive(checker.isQueen);
	}
	public void SetPosition(int posX, int posY)
	{
		if (checker.isWhite && posY == 7) SetQueen(true);
		if (!checker.isWhite && posY == 0) SetQueen(true);

		table[checker.positionY, checker.positionX] = null;
		checker.positionX = posX; checker.positionY = posY;
		transform.position = new Vector3(checker.positionX, checker.positionY) + table.transform.position;
		table[checker.positionY, checker.positionX] = checker;

	}

	public static Checker[,] Move(Checker c, Vector3 pos, Checker[,] area)
	{
		return Move(c.positionX, c.positionY, Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), area);
	}
	public static Checker[,] Move(int sx, int sy, int ex, int ey, Checker[,] area)
	{
		Checker c = area[sy, sx];
		area[sy, sx] = null;
		c.positionX = ex; c.positionY = ey;
		//c.transform.position = new Vector3(c.positionX, c.positionY) + table.transform.position;
		area[ey, ex] = c;
		return area;
	}
	public static Checker[,] Kick(Checker c, Vector3 pos, Checker[,] area)
	{
		return Kick(c.positionX, c.positionY, Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), area);
	}
	public static Checker[,] Kick(int sx, int sy, int ex, int ey, Checker[,] area)
	{
		Checker c = area[sy, sx];
		area[sy, sx] = null;
		c.positionX = ex; c.positionY = ey;
		area[(sy + ey) / 2, (sx + ex) / 2] = null;
		area[ey, ex] = c;
		return area;
	}
	public static bool AbleMove(Checker c, Vector3 pos, Checker[,] area)
	{
		int tx, ty;
		ty = Mathf.RoundToInt(pos.y); tx = Mathf.RoundToInt(pos.x);

		c.isAbleMove = true;
		if (ty <= 7 && tx <= 7 && (c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null) return true;
		}

		c.isAbleMove = false;
		return false;
	}
	public static bool AbleMove(Checker c, Checker[,] area)
	{
		int tx, ty;
		c.isAbleMove = true;
		ty = c.positionY + 1; tx = c.positionX + 1;
		if (ty <= 7 && tx <= 7 && (c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null) return true;
		}

		ty = c.positionY - 1; tx = c.positionX + 1;
		if (ty >= 0 && tx <= 7 && (!c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null) return true;
		}

		ty = c.positionY - 1; tx = c.positionX - 1;
		if (ty >= 0 && tx >= 0 && (!c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null) return true;
		}

		ty = c.positionY + 1; tx = c.positionX - 1;
		if (ty <= 7 && tx >= 0 && (c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null) return true;
		}
		c.isAbleMove = false;
		return false;
	}
	public static bool AbleKick(Checker c, Vector3 pos, Checker[,] area)
	{
		int sx, sy, tx, ty;
		c.isNeedAttack = true;
		c.isAbleMove = true;
		ty = Mathf.RoundToInt(pos.y); tx = Mathf.RoundToInt(pos.x);
		sy = (ty + c.positionY) / 2; sx = (ty + c.positionY) / 2;
		if (Mathf.Abs(c.positionY - ty) != 2 || Mathf.Abs(c.positionX - tx) != 2) return false;

		if (ty >= 0 && ty <= 7 && tx >= 0 && tx <= 7 && ((c.positionY - sy < 0) == c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null && area[sy, sx] != null && area[sy, sx].isWhite != c.isWhite) return true;
		}
		return false;
	}
	public static bool AbleKick(Checker c, Checker[,] area)
	{
		int ex, ey, tx, ty;
		c.isNeedAttack = true;
		c.isAbleMove = true;
		ty = c.positionY + 2; tx = c.positionX + 2;
		ey = c.positionY + 1; ex = c.positionX + 1;
		if (ty <= 7 && tx <= 7 && (c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null && area[ey, ex] != null && area[ey, ex].isWhite != c.isWhite) return true;
		}

		ty = c.positionY - 2; tx = c.positionX + 2;
		ey = c.positionY - 1; ex = c.positionX + 1;
		if (ty >= 0 && tx <= 7 && (!c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null && area[ey, ex] != null && area[ey, ex].isWhite != c.isWhite) return true;
		}

		ty = c.positionY - 2; tx = c.positionX - 2;
		ey = c.positionY - 1; ex = c.positionX - 1;
		if (ty >= 0 && tx >= 0 && (!c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null && area[ey, ex] != null && area[ey, ex].isWhite != c.isWhite) return true;
		}

		ty = c.positionY + 2; tx = c.positionX - 2;
		ey = c.positionY + 1; ex = c.positionX - 1;
		if (ty <= 7 && tx >= 0 && (c.isWhite || c.isQueen))
		{
			if (area[ty, tx] == null && area[ey, ex] != null && area[ey, ex].isWhite != c.isWhite) return true;
		}
		c.isNeedAttack = false;
		c.isAbleMove = false;
		return false;
	}
	public bool AbleMove(int posX, int posY, Checker[,] temp_table)
	{
		if (posX < 0 || posX > 7 || posY < 0 || posY > 7) return false;
		if (Mathf.Abs(checker.positionX - posX) != 1 || Mathf.Abs(checker.positionY - posY) != 1 || temp_table[posY, posX] != null) return false;

		if (!checker.isQueen)
		{
			if (checker.positionY - posY < 0 != checker.isWhite) return false;
			if (checker.positionY - posY > 0 == checker.isWhite) return false;
		}

		return true;
	}
	public bool AbleKick(int posX, int posY, Checker[,] temp_table)
	{
		if (posX < 0 || posX > 7 || posY < 0 || posY > 7) return false;
		if (Mathf.Abs(checker.positionX - posX) != 2 || Mathf.Abs(checker.positionY - posY) != 2 || temp_table[posY, posX] != null) return false;

		if (!checker.isQueen)
		{
			if (checker.positionY - posY < 0 != checker.isWhite) return false;
			if (checker.positionY - posY > 0 == checker.isWhite) return false;
		}

		int ex = (checker.positionX + posX) / 2, ey = (checker.positionY + posY) / 2;
		if (temp_table[ey, ex] == null || temp_table[ey, ex].isWhite == checker.isWhite) return false;

		return true;
	}
	
}
