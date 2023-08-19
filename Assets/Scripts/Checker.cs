using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
	public int positionX, positionY;
	public GameObject crown;
	public List<Checker> owerList;
	public Table table;
	public bool isWhite, isQueen;
	private void Start()
	{
		transform.position = table.transform.position + new Vector3(positionX, positionY);
		SetQueen(false);
	}

	public void SetQueen(bool queen)
	{
		isQueen = queen;
		crown.SetActive(isQueen);
	}
	public void SetPosition(int posX, int posY)
	{
		if (isWhite && posY == 7) SetQueen(true);
		if(!isWhite && posY == 0) SetQueen(true);

		table[positionY, positionX] = null;
		positionX = posX; positionY = posY;
		transform.position = new Vector3(positionX, positionY) + table.transform.position;
		table[positionY, positionX] = this;

	}
	public bool Move(Vector3 pos)
	{
		int x = Mathf.RoundToInt(pos.x), y = Mathf.RoundToInt(pos.y);

		return Move(x, y);
	}
	public bool Move(int posX, int posY)
	{
		if (Mathf.Abs(positionX - posX) != Mathf.Abs(positionY - posY) || table[posY, posX] != null) return false;

		if(!isQueen)
		{
			if (positionY - posY < 0 != isWhite) return false;
			if (positionY - posY > 0 == isWhite) return false;
		}
		if (Mathf.Abs(positionX - posX) == 1)
		{
			SetPosition(posX, posY);
			return true;
		}

		return false;
	}
	public bool Kick(Vector3 pos)
	{
		int x = Mathf.RoundToInt(pos.x), y = Mathf.RoundToInt(pos.y);

		return Kick(x, y);
	}
	public bool Kick(int posX, int posY)
	{
		if (Mathf.Abs(positionX - posX) != 2 || Mathf.Abs(positionY - posY) != 2 || table[posY, posX] != null) return false;

		if (!isQueen)
		{
			if (positionY - posY < 0 != isWhite) return false;
			if (positionY - posY > 0 == isWhite) return false;
		}

		
		int ex = (positionX + posX) / 2, ey = (positionY + posY) / 2;
		if (table[ey, ex] == null || table[(positionY + posY) / 2, (positionX + posX) / 2].isWhite == this.isWhite) return false;
		SetPosition(posX, posY);

		// Ñúåäàåì øàøêó
		Checker c = table[ey, ex];
		table[ey, ex] = null;
		c.owerList.Remove(c);

		Destroy(c.gameObject);
		return true;
	}
	public bool AbleMove()
	{
		int tx, ty;
		ty = positionY + 1; tx = positionX + 1;
		if (ty <= 7 && tx <= 7 && (isWhite || isQueen))
		{
			if (table[ty, tx] == null) return true;
		}

		ty = positionY - 1; tx = positionX + 1;
		if (ty >= 0 && tx <= 7 && (!isWhite || isQueen))
		{
			if (table[ty, tx] == null) return true;
		}

		ty = positionY - 1; tx = positionX - 1;
		if (ty >= 0 && tx >= 0 && (!isWhite || isQueen))
		{
			if (table[ty, tx] == null) return true;
		}
		
		ty = positionY + 1; tx = positionX - 1;
		if (ty <= 7 && tx >= 0 && (isWhite || isQueen))
		{
			if (table[ty, tx] == null) return true;
		}
		return false;
	}
	public bool AbleMove(int posX, int posY, Checker[,] temp_table)
	{
		if (posX < 0 || posX > 7 || posY < 0 || posY > 7) return false;
		if (Mathf.Abs(positionX - posX) != 1 ||  Mathf.Abs(positionY - posY) != 1 || temp_table[posY, posX] != null) return false;

		if (!isQueen)
		{
			if (positionY - posY < 0 != isWhite) return false;
			if (positionY - posY > 0 == isWhite) return false;
		}

		return true;
	}
	public bool AbleKick(int posX, int posY, Checker[,] temp_table)
	{
		if (posX < 0 || posX > 7 || posY < 0 || posY > 7) return false;
		if (Mathf.Abs(positionX - posX) != 2 || Mathf.Abs(positionY - posY) != 2 || temp_table[posY, posX] != null) return false;

		if (!isQueen)
		{
			if (positionY - posY < 0 != isWhite) return false;
			if (positionY - posY > 0 == isWhite) return false;
		}

		int ex = (positionX + posX) / 2, ey = (positionY + posY) / 2;
		if (temp_table[ey, ex] == null || temp_table[ey, ex].isWhite == this.isWhite) return false;

		return true;
	}
	public bool AbleKick()
	{
		int ex, ey, tx, ty;
		ty = positionY + 2; tx = positionX + 2;
		ey = positionY + 1; ex = positionX + 1; 
		if(ty <= 7 && tx <= 7 && (isWhite || isQueen))
		{
			if (table[ty, tx] == null && table[ey, ex] != null && table[ey, ex].isWhite != this.isWhite) return true;
		}

		ty = positionY - 2; tx = positionX + 2;
		ey = positionY - 1; ex = positionX + 1;
		if (ty >= 0 && tx <= 7 && (!isWhite || isQueen))
		{
			if (table[ty, tx] == null && table[ey, ex] != null && table[ey, ex].isWhite != this.isWhite) return true;
		}

		ty = positionY - 2; tx = positionX - 2;
		ey = positionY - 1; ex = positionX - 1;
		if (ty >= 0 && tx >= 0 && (!isWhite || isQueen))
		{
			if (table[ty, tx] == null && table[ey, ex] != null && table[ey, ex].isWhite != this.isWhite) return true;
		}

		ty = positionY + 2; tx = positionX - 2;
		ey = positionY + 1; ex = positionX - 1;
		if (ty <= 7 && tx >= 0 && (isWhite || isQueen))
		{
			if (table[ty, tx] == null && table[ey, ex] != null && table[ey, ex].isWhite != this.isWhite) return true;
		}

		return false;
	}
}
