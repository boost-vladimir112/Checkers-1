using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGameManager : MonoBehaviour
{
	public GameObject currentCircle;
	public Checker currentChecker;
	public Table table;
	public int step = 1;
	public bool needToKick;
	public AIBot aiBot;

	public delegate void VoidFunc();
	public event VoidFunc WinWhite, WinBlack;
	private void Start()
	{
		WinWhite += () => { Debug.Log("White Win!!"); };
		WinBlack += () => { Debug.Log("Black Win!!"); };
	}

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Vector3Int pos = GetVector3Int(GetMouseWorldPosition(table.transform.position));
			if (currentChecker != null)
			{
				if (!needToKick && Board.AbleMove(currentChecker, pos, table.board))
				{
					Board.Move(currentChecker, pos, table.board);
					currentChecker.transform.position = currentChecker.position + table.transform.position;

					currentCircle.SetActive(false);
					currentChecker = null;
					NextStep();
					
				}
				else if(needToKick)
				{
					Debug.Log("We need to Kick " + pos);

					if (Board.AbleKick(currentChecker, pos, table.board))
					{
						Debug.Log("Kick " + pos);
						Board.Kick(currentChecker, pos, table.board);
						table.board.whiteCheckers.Clear();
						table.board.blackCheckers.Clear();
						table.board.SetCheckers();
						currentChecker.transform.position = new Vector3(currentChecker.position.x, currentChecker.position.y) + table.transform.position;
						needToKick = false;
						if(Board.AbleKick(currentChecker, table.board))
						{
							needToKick = true;
						}

						if (needToKick) Debug.Log("We also need to Kick");
						if (needToKick)
						{
							currentCircle.transform.position = currentChecker.transform.position;
						}
						else
						{
							currentCircle.SetActive(false);
							currentChecker = null;
							NextStep();
						}
					}
					else				
					{
						ChooseCurrent(pos);
					}
				}
				else
				{
					ChooseCurrent(pos);
				}
			}
			else
			{
				ChooseCurrent(pos);
			}
		}
	}

	public void NextStep()
	{
		step++;
		table.board.isWhiteMove = (step % 2 == 1);
		Debug.Log("Step: " + step);
		List<Checker> list = (step % 2 == 1) ? table.board.whiteCheckers : table.board.blackCheckers;

		if (list.Count == 0)
		{
			if (step % 2 == 1) WinBlack?.Invoke();
			else WinWhite?.Invoke();
		}
		bool isAbleMove = false;
		foreach (Checker c in list)
		{
			if (Board.AbleKick(c, table.board))
			{
				isAbleMove = true;
				needToKick = true;
				Debug.Log(c.name + " need kick");
			}
			if (!isAbleMove && Board.AbleMove(c, table.board))
			{
				isAbleMove = true;
			}
		}
		if(!isAbleMove)
		{
			if (step % 2 == 1) WinBlack?.Invoke();
			else WinWhite?.Invoke();
		}

		//Debug.Log(AIBot.GetMoves(table.squareArray, step % 2).Length);
	}
	public void ChooseCurrent(Vector3Int pos)
	{
		if(table.board.IsEmpty(pos) && table.board[pos.y, pos.x].isWhite == (step % 2 == 1) && table.board[pos.y, pos.x].isNeedAttack == needToKick)
		{
			currentChecker = table.board[pos.y, pos.x];

			currentCircle.SetActive(true);
			currentCircle.transform.position = currentChecker.transform.position;
		}
	}
	public static Vector3 GetMouseWorldPosition() => GetMouseWorldPosition(Vector3.zero, Input.mousePosition, Camera.main);
	public static Vector3 GetMouseWorldPosition(Vector3 nullCoordinate) => GetMouseWorldPosition(nullCoordinate, Input.mousePosition, Camera.main);
	public static Vector3 GetMouseWorldPosition(Vector3 nullCoordinate, Vector3 screenPosition, Camera worldCamera)
	{
		Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition) - nullCoordinate;
		worldPosition = new Vector3(worldPosition.x, worldPosition.y);
		return worldPosition;
	}
	public static Vector3Int GetVector3Int(Vector3 v)
	{
		return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
	}
}
