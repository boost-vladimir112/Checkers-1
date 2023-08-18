using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
	public Checker[,] squareArray;
	public List<Checker> whiteChecker, blackChecker;

	private void Start()
	{
		squareArray = new Checker[8, 8];

		foreach (Checker c in whiteChecker)
		{
			c.SetPosition(c.positionX, c.positionY);
		}
		foreach (Checker c in blackChecker)
		{
			c.SetPosition(c.positionX, c.positionY);

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
