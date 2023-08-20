using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
	public int positionX, positionY;
	public GameObject crown;
	public List<Checker> owerList;
	public bool isWhite, isQueen, isNeedAttack, isAbleMove;
	public CheckerControll checkerControll;
	private void Start()
	{
		transform.position = checkerControll.table.transform.position + new Vector3(positionX, positionY);
		checkerControll.SetQueen(false);
	}

}
