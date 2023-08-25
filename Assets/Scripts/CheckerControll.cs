using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerControll : MonoBehaviour
{
	public Table table;
	public Checker checker;
	public Board board;
	public GameObject crown;

	private void Start()
	{
		transform.position = table.transform.position + checker.position;
	}
	public void SetPosition()
	{
		transform.position = new Vector3(checker.position.x, checker.position.y) + table.transform.position;
	}
	public void DestroyMe()
	{
		Destroy(gameObject);
	}

}
