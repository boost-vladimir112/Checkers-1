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
			Vector3 pos = GetMouseWorldPosition(table.transform.position);
			if (currentChecker != null)
			{
				if (!needToKick && CheckerControll.AbleMove(currentChecker, pos, table.squareArray))
				{
					CheckerControll.Move(currentChecker, pos, table.squareArray);
					currentChecker.transform.position = new Vector3(currentChecker.positionX, currentChecker.positionY) + table.transform.position;

					currentCircle.SetActive(false);
					currentChecker = null;
					NextStep();
					
				}
				else if(needToKick)
				{
					Debug.Log("We need to Kick");

					if (CheckerControll.AbleKick(currentChecker, pos, table.squareArray))
					{
						CheckerControll.Kick(currentChecker, pos, table.squareArray);
						currentChecker.transform.position = new Vector3(currentChecker.positionX, currentChecker.positionY) + table.transform.position;
						needToKick = false;
						if(CheckerControll.AbleKick(currentChecker, table.squareArray))
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
		Debug.Log("Step: " + step);
		if (step % 2 == 1)
		{
			if (table.whiteCheckers.Count == 0) WinBlack?.Invoke();
			bool isAbleMove = false;
			foreach (Checker c in table.whiteCheckers)
			{
				if (CheckerControll.AbleKick(c, table.squareArray))
				{
					isAbleMove = true;
					needToKick = true;
					Debug.Log(c.name + " need kick");
				}
				if(!isAbleMove && CheckerControll.AbleMove(c, table.squareArray))
				{
					isAbleMove = true;
				}
			}
			if(!isAbleMove)
			{
				WinBlack?.Invoke();
			}
		}
		else
		{
			if (table.blackCheckers.Count == 0) WinWhite?.Invoke();
			bool isAbleMove = false;
			foreach (Checker c in table.blackCheckers)
			{
				if (CheckerControll.AbleKick(c, table.squareArray))
				{
					isAbleMove = true;
					needToKick = true;
					Debug.Log(c.name + " need kick");
				}
				if (!isAbleMove && CheckerControll.AbleMove(c, table.squareArray))
				{
					isAbleMove = true;
				}
			}
			if (!isAbleMove)
			{
				WinWhite?.Invoke();
			}
		}
	}
	public void ChooseCurrent(Vector3 pos)
	{
		int x = Mathf.RoundToInt(pos.x), y = Mathf.RoundToInt(pos.y);
		if(table[y, x] != null && table[y, x].isWhite == (step % 2 == 1) && table[y, x].isNeedAttack == needToKick)
		{
			currentChecker = table[y, x];

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
}
