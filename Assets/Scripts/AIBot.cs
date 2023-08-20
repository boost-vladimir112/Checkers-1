using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBot : MonoBehaviour
{
	public Table table;
	public UGameManager gameManager;
	public List<Movement> moveList;
	public Checker[,] baseCheckPos;
	public List<Checker> owerCheckers;
	public List<Checker> enemyCheckers;
	public List<Checker[,]> predictionCheckPos;
	public int color; // 1 - white, 0 - black

	public void SetBaseChecks(Checker[,] baseCheckPos, int color)
	{
		this.baseCheckPos = (Checker[,])baseCheckPos.Clone();
		this.color = color;
		for(int y = 0; y < 8; y++)
		{
			for(int x = 0; x < 8; x++)
			{
				if (baseCheckPos[y, x] != null)
				{
					if (baseCheckPos[y, x].isWhite == (color == 1)) owerCheckers.Add(baseCheckPos[y, x]);
					else enemyCheckers.Add(baseCheckPos[y, x]);
				}
			}
		}
	}

	// RENAME FUNC
	public void FuncA()
	{
		foreach(Checker c in owerCheckers)
		{
			FuncC(c, baseCheckPos);
		}
	}

	// RENAME FUNC
	public bool FuncB(Checker c, Vector3 pos, Checker[,] area, Movement move = null)
	{
		Checker[,] newArea;
		bool isHaveAttack = false;
		if (CheckerControll.AbleKick(c, pos, area))
		{
			newArea = CheckerControll.Kick(c, pos, (Checker[,])area.Clone());
			Movement newMove = new Movement(newArea, move != null ? move.value + 2 : 2, true, move);
			Checker nc = newArea[Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.x)];
			isHaveAttack = true;
			FuncC(nc, newArea, newMove);
		}
		else
		{
			moveList.Add(move);
		}
		return isHaveAttack;
	}

	// RENAME FUNC
	public bool FuncC(Checker c, Checker[,] area, Movement move = null)
	{
		Checker[,] newArea;
		Vector3 pos;
		bool haveAttack = false;
		pos = new Vector3(c.positionX + 2, c.positionY + 2);
		if (FuncB(c, pos, area, move)) haveAttack = true;

		pos = new Vector3(c.positionX - 2, c.positionY + 2);
		if (FuncB(c, pos, area, move)) haveAttack = true;

		pos = new Vector3(c.positionX + 2, c.positionY - 2);
		if (FuncB(c, pos, area, move)) haveAttack = true;

		pos = new Vector3(c.positionX - 2, c.positionY - 2);
		if (FuncB(c, pos, area, move)) haveAttack = true;

		return haveAttack;
	}


	//public void SetAttackMovementToChecker(Checker checker)
	//{
	//	int tx, ty;
	//	bool haveAttack = false;
	//	tx = checker.positionX - 2; ty = checker.positionY + (color == 1 ? 2 : -2);
	//	if (checker.checkerControll.AbleKick(tx, ty, baseCheckPos))
	//	{
	//		moveList.Add(CreateMovement(checker.positionX, checker.positionY, tx, ty, 1, null, true));
	//		haveAttack = true;
	//	}

	//	tx = checker.positionX + 2; ty = checker.positionY + (color == 1 ? 2 : -2);
	//	if (checker.checkerControll.AbleKick(tx, ty, baseCheckPos))
	//	{
	//		moveList.Add(CreateMovement(checker.positionX, checker.positionY, tx, ty, 1, null, true));
	//		haveAttack = true;
	//	}
	//	if(!haveAttack)
	//	{
	//		SetMoveMovementToChecker(checker);
	//	}
	//}
	//public void SetMoveMovementToChecker(Checker checker)
	//{
	//	int tx, ty;
	//	tx = checker.positionX - 2; ty = checker.positionY + (color == 1 ? 2 : -2);

	//	tx = checker.positionX - 1; ty = checker.positionY + (color == 1 ? 1 : -1);
	//	if (checker.checkerControll.AbleMove(tx, ty, baseCheckPos)) moveList.Add(CreateMovement(checker.positionX, checker.positionY, tx, ty, 0, null, true));

	//	tx = checker.positionX + 1; ty = checker.positionY + (color == 1 ? 1 : -1);
	//	if (checker.checkerControll.AbleMove(tx, ty, baseCheckPos)) moveList.Add(CreateMovement(checker.positionX, checker.positionY, tx, ty, 0, null, true));
	//}
	//public Movement CreateAttackMovement(int sx, int sy, int ex, int ey, int v, Movement nm, bool os)
	//{
	//	Movement move = new Movement(sx, sy, ex, ey, v, nm, os);
	//	Checker[,] newArea = (Checker[,])baseCheckPos.Clone();
	//	Checker c = newArea[sy, sx];
	//	newArea[sy, sx] = null;
	//	newArea[ey, ex] = c;
	//	//newArea[(sy + ey) / 2, (sx + ex) / 2].isWhite == (color == 1) ? 
	//	//newArea[(sy + ey) / 2, (sx + ex) / 2] = null;

	//	return move;
	//}
	//public Movement CreateMovement(int sx, int sy, int ex, int ey, int v, Movement nm, bool os)
	//{
	//	Movement move = new Movement(sx, sy, ex, ey, v, nm, os);


	//	return move;
	//}
	//public void SetMoveList()
	//{
	//	List<Checker> checkList = color == 1 ? table.whiteCheckers : table.blackCheckers;

	//	foreach(Checker c in checkList)
	//	{
	//		int tx, ty;
	//		tx = c.positionX - 2; ty = c.positionY + (color == 1 ? 2 : -2);
	//		if (c.checkerControll.AbleKick(tx, ty, baseCheckPos)) moveList.Add(new Movement(c.positionX, c.positionY, tx, ty, 1, null, true));

	//		tx = c.positionX + 2; ty = c.positionY + (color == 1 ? 2 : -2);
	//		if (c.checkerControll.AbleKick(tx, ty, baseCheckPos)) moveList.Add(new Movement(c.positionX, c.positionY, tx, ty, 1, null, true));

	//		tx = c.positionX - 1; ty = c.positionY + (color == 1 ? 1 : -1);
	//		if (c.checkerControll.AbleMove(tx, ty, baseCheckPos)) moveList.Add(new Movement(c.positionX, c.positionY, tx, ty, 0, null, true));

	//		tx = c.positionX + 1; ty = c.positionY + (color == 1 ? 1 : -1);
	//		if (c.checkerControll.AbleMove(tx, ty, baseCheckPos)) moveList.Add(new Movement(c.positionX, c.positionY, tx, ty, 0, null, true));
	//	}
		
	//}
	//public Checker[,] PredictMovement(Movement move)
	//{
	//	Checker[,] predCheckPos = (Checker[,])baseCheckPos.Clone();
	//	if(move.value == 0)
	//	{
	//		Checker c = predCheckPos[move.startY, move.startX];
	//		predCheckPos[move.startY, move.startX] = null;
	//		c.positionX = move.endX; c.positionY = move.endY;
	//		predCheckPos[move.endY, move.endX] = c;
	//	}
	//	if(move.value == 2)
	//	{
	//		Checker c = predCheckPos[move.startY, move.startX];
	//		predCheckPos[move.startY, move.startX] = null;
	//		predCheckPos[(move.startY + move.endY) / 2, (move.startX + move.endX) / 2] = null;
	//		c.positionX = move.endX; c.positionY = move.endY;
	//		predCheckPos[move.endY, move.endX] = c;
	//	}
	//	return predCheckPos;
	//}

}

public class Movement
{
	public Checker[,] area;
	public int value;
	public Movement nextMove;
	public bool owerStep;

	public Movement(Checker[,] a, int v, bool os, Movement nm = null)
	{
		area = a;
		value = v;
		owerStep = os;
		nextMove = nm;
	}
}
