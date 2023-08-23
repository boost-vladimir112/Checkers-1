using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
	public Vector3Int position;
	public GameObject crown;
	public List<Checker> owerList;
	public bool isWhite, isQueen, isNeedAttack, isAbleMove;
	public CheckerControll checkerControll;
	private void Start()
	{
		transform.position = checkerControll.table.transform.position + position;
		SetQueen(false);

	}
	public void SetQueen(bool queen)
	{
		isQueen = queen;
		crown.SetActive(isQueen);
	}
}
