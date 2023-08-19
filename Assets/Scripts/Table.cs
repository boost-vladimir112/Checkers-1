using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
	public Checker[,] squareArray;
	public List<Checker> whiteCheckers, blackCheckers;

	private void Start()
	{
		squareArray = new Checker[8, 8];

		foreach (Checker c in whiteCheckers)
		{
			c.SetPosition(c.positionX, c.positionY);
			c.owerList = whiteCheckers;
		}
		foreach (Checker c in blackCheckers)
		{
			c.SetPosition(c.positionX, c.positionY);
			c.owerList = blackCheckers;
		}
	}
	private void OnDrawGizmosSelected()
	{
		for(int i = 0; i < 8; i++)
		{
			for(int j = 0; j < 8; j++)
			{
				Gizmos.color = squareArray[j, i] != null ? Color.white : Color.red;
				Gizmos.DrawCube(new Vector3(i, j) + transform.position, Vector3.one);
			}
		}
	}
	public Checker this[int y, int x]
	{
		get
		{
			return squareArray[y, x];
		}
		set
		{
			squareArray[y, x] = value;
		}
	}
}
