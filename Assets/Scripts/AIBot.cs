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

	public void CalculateBestMove()
	{
		Movement[] moves = GetMoves(baseCheckPos);
	}

	public static Movement[] GetMoves(Checker[,] area, int color = 0)
	{
		List<Movement> moves = new List<Movement>();

		List<Checker> owerCheck = new List<Checker>(), enemyCheck = new List<Checker>();
		for(int i = 0; i < 8; i++)
		{
			for(int j = 0; j < 8; j++)
			{
				if(area[i, j] != null)
				{
					if (area[i, j].isWhite == (color == 1)) owerCheck.Add(area[i, j]);
					else enemyCheck.Add(area[i, j]);
				}
			}
		}
		bool haveAttack = false;
		foreach (Checker c in owerCheck)
		{
			moves.AddRange(GetMove(c, area, ref haveAttack));
		}
		if(haveAttack)
		{
			List<Movement> newList = new List<Movement>();
			foreach(Movement m in moves)
			{
				if (m.value == 1) newList.Add(m);
			}
			moves = newList;
		}

		return moves.ToArray();
	}



	public static List<Movement> GetMove(Checker check, Checker[,] area, ref bool haveAttack)
	{
		List<Movement> moves = new List<Movement>();
		Vector3Int pos = new Vector3Int(check.positionX + 2, check.positionY + 2);
		if(CheckerControll.AbleKick(check, pos, area))
		{
			Movement m = new Movement(CheckerControll.Kick(check, pos, (Checker[,])area.Clone()), 1, true);
			m.simpleMoves.Add(new SimpleMove(check.positionX, check.positionY, pos.x, pos.y));
			haveAttack = true;
			moves.Add(m);
		}

		pos = new Vector3Int(check.positionX + 2, check.positionY - 2);
		if (CheckerControll.AbleKick(check, pos, area))
		{
			Movement m = new Movement(CheckerControll.Kick(check, pos, (Checker[,])area.Clone()), 1, true);
			m.simpleMoves.Add(new SimpleMove(check.positionX, check.positionY, pos.x, pos.y));
			haveAttack = true;
			moves.Add(m);
		}

		pos = new Vector3Int(check.positionX - 2, check.positionY - 2);
		if (CheckerControll.AbleKick(check, pos, area))
		{
			Movement m = new Movement(CheckerControll.Kick(check, pos, (Checker[,])area.Clone()), 1, true);
			m.simpleMoves.Add(new SimpleMove(check.positionX, check.positionY, pos.x, pos.y));
			haveAttack = true;
			moves.Add(m);
		}

		pos = new Vector3Int(check.positionX - 2, check.positionY + 2);
		if (CheckerControll.AbleKick(check, pos, area))
		{
			Movement m = new Movement(CheckerControll.Kick(check, pos, (Checker[,])area.Clone()), 1, true);
			m.simpleMoves.Add(new SimpleMove(check.positionX, check.positionY, pos.x, pos.y));
			haveAttack = true;
			moves.Add(m);
		}
		
		if(!haveAttack)
		{
			pos = new Vector3Int(check.positionX + 1, check.positionY + 1);
			if (CheckerControll.AbleMove(check, pos, area))
			{
				Movement m = new Movement(CheckerControll.Kick(check, pos, (Checker[,])area.Clone()), 0, true);
				m.simpleMoves.Add(new SimpleMove(check.positionX, check.positionY, pos.x, pos.y));
				moves.Add(m);
			}
			pos = new Vector3Int(check.positionX + 1, check.positionY - 1);
			if (CheckerControll.AbleMove(check, pos, area))
			{
				Movement m = new Movement(CheckerControll.Kick(check, pos, (Checker[,])area.Clone()), 0, true);
				m.simpleMoves.Add(new SimpleMove(check.positionX, check.positionY, pos.x, pos.y));
				moves.Add(m);
			}
			pos = new Vector3Int(check.positionX - 1, check.positionY - 1);
			if (CheckerControll.AbleMove(check, pos, area))
			{
				Movement m = new Movement(CheckerControll.Kick(check, pos, (Checker[,])area.Clone()), 0, true);
				m.simpleMoves.Add(new SimpleMove(check.positionX, check.positionY, pos.x, pos.y));
				moves.Add(m);
			}
			pos = new Vector3Int(check.positionX - 1, check.positionY + 1);
			if (CheckerControll.AbleMove(check, pos, area))
			{
				Movement m = new Movement(CheckerControll.Kick(check, pos, (Checker[,])area.Clone()), 0, true);
				m.simpleMoves.Add(new SimpleMove(check.positionX, check.positionY, pos.x, pos.y));
				moves.Add(m);
			}
		}
		return moves;
	}
	private void ContinueAttack(Movement move)
	{

	}

	private void SearchTailCount(List<Movement> list, ref int resalt)
	{
		if (list.Count == 0) resalt++;
		foreach(Movement m in list)
		{
		}
		
	}

}

public struct SimpleMove
{
	int startX, startY;
	int endX, endY;
	int destroyX, destroyY;
	public SimpleMove(int sx, int sy, int ex, int ey)
	{
		startX = sx;
		startY = sy;
		endX = ex;
		endY = ey;
		if(startX - endX == 2 && startY - endY == 2)
		{
			destroyX = (startX + endX) / 2;
			destroyY = (startY + endY) / 2;
		}
		else
		{
			destroyX = -1;
			destroyY = -1;
		}
	}
}
public class Movement
{
	public Checker[,] area;
	public int value;
	public List<SimpleMove> simpleMoves;
	public bool owerStep;

	public Movement(Checker[,] a, int v, bool os)
	{
		area = a;
		value = v;
		owerStep = os;
		simpleMoves = new List<SimpleMove>();
	}
}
