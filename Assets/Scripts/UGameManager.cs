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

	}

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Vector3 pos = GetMouseWorldPosition(table.transform.position);
			if (currentChecker != null)
			{
				if (!needToKick && currentChecker.Move(pos))
				{
					currentCircle.SetActive(false);
					currentChecker = null;
					NextStep();
					if (step % 2 == 1)
					{
						foreach (Checker c in table.whiteCheckers)
						{
							if (c.AbleKick())
							{
								needToKick = true;
								Debug.Log(c.name + " need kick");
							}
						}
					}
					else
					{
						foreach (Checker c in table.blackCheckers)
						{
							if (c.AbleKick())
							{
								needToKick = true;
								Debug.Log(c.name + " need kick");
							}
						}
					}
				}
				else if(needToKick)
				{
					Debug.Log("We need to Kick");

					if (currentChecker.Kick(pos))
					{
						needToKick = false;
						if (step % 2 == 1)
							foreach (Checker c in table.whiteCheckers)
								if (c.AbleKick())
									needToKick = true;

						if (step % 2 == 0)
							foreach (Checker c in table.blackCheckers)
								if (c.AbleKick())
									needToKick = true;

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

							if (table.whiteCheckers.Count == 0) WinBlack?.Invoke();
							if (table.blackCheckers.Count == 0) WinWhite?.Invoke();

							if (step % 2 == 1)
							{
								foreach (Checker c in table.whiteCheckers)
								{
									if (c.AbleKick())
									{
										needToKick = true;
										Debug.Log(c.name + " need kick");
									}
								}
							}
							else
							{
								foreach (Checker c in table.blackCheckers)
								{
									if (c.AbleKick())
									{
										needToKick = true;
										Debug.Log(c.name + " need kick");
									}
								}
							}
						}
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
	}
	public void ChooseCurrent(Vector3 pos)
	{
		int x = Mathf.RoundToInt(pos.x), y = Mathf.RoundToInt(pos.y);
		if(table[y, x] != null && table[y, x].isWhite == (step % 2 == 1))
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
