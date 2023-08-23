using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Checker))]
public class CheckerControll : MonoBehaviour
{
	public Table table;
	Checker checker;
	public Board board;

	private void Start()
	{
		checker = GetComponent<Checker>();
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
