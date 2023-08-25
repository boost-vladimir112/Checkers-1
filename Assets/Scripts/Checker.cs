using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker
{
	public Vector3Int position;
	public bool isWhite, isQueen, isNeedAttack, isAbleMove;
	public CheckerControll checkerControll;

	public delegate void VoidFunc(Checker c, bool queen);
	public event VoidFunc ChangeQueen;

	public Checker(Vector3Int pos, bool isW, bool isQ)
	{
		position = pos;
		isWhite = isW;
		isQueen = isQ;
		isNeedAttack = false;
		isAbleMove = false;
	}
	public Checker(Checker c)
	{
		this.position	= c.position;
		this.isWhite	= c.isWhite;
		this.isQueen	= c.isQueen;
		this.isNeedAttack = c.isNeedAttack;
		this.isAbleMove = c.isAbleMove;
	}
	public void SetQueen(bool queen)
	{
		isQueen = queen;
		ChangeQueen?.Invoke(this, queen);
	}
}
