using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBot : MonoBehaviour
{
	public Table table;
	public UGameManager gameManager;
	public List<Movement> moveList;
	public Checker[,] baseCheckPos;
	public List<Checker> baseCheckers;
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
				if (baseCheckPos[y, x] != null && baseCheckPos[y, x].isWhite == (color == 1)) baseCheckers.Add(baseCheckPos[y, x]);
			}
		}
	}
	public void SetMoveList()
	{
		List<Checker> checkList = color == 1 ? table.whiteCheckers : table.blackCheckers;

		foreach(Checker c in checkList)
		{
			int tx, ty;
			tx = c.positionX - 2; ty = c.positionY + (color == 1 ? 2 : -2);
			if (c.AbleKick(tx, ty, baseCheckPos)) moveList.Add(new Movement(c.positionX, c.positionY, tx, ty, 1));

			tx = c.positionX + 2; ty = c.positionY + (color == 1 ? 2 : -2);
			if (c.AbleKick(tx, ty, baseCheckPos)) moveList.Add(new Movement(c.positionX, c.positionY, tx, ty, 1));

			tx = c.positionX - 1; ty = c.positionY + (color == 1 ? 1 : -1);
			if (c.AbleMove(tx, ty, baseCheckPos)) moveList.Add(new Movement(c.positionX, c.positionY, tx, ty, 0));

			tx = c.positionX + 1; ty = c.positionY + (color == 1 ? 1 : -1);
			if (c.AbleMove(tx, ty, baseCheckPos)) moveList.Add(new Movement(c.positionX, c.positionY, tx, ty, 0));
		}
		
	}
	public Checker[,] PredictMovement(Movement move)
	{
		Checker[,] predCheckPos = (Checker[,])baseCheckPos.Clone();
		if(move.value == 0)
		{
			Checker c = predCheckPos[move.startY, move.startX];
			predCheckPos[move.startY, move.startX] = null;
			c.positionX = move.endX; c.positionY = move.endY;
			predCheckPos[move.endY, move.endX] = c;
		}
		if(move.value == 2)
		{
			Checker c = predCheckPos[move.startY, move.startX];
			predCheckPos[move.startY, move.startX] = null;
			predCheckPos[(move.startY + move.endY) / 2, (move.startX + move.endX) / 2] = null;
			c.positionX = move.endX; c.positionY = move.endY;
			predCheckPos[move.endY, move.endX] = c;
		}
		return predCheckPos;
	}

}

public struct Movement
{
	public int startX, startY, endX, endY;
	public int value;
	public Movement(int sx, int sy, int ex, int ey, int v)
	{
		startX = sx; startY = sy;
		endX = ex; endY = ey;
		value = v;
	}
}
