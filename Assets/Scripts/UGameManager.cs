using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGameManager : MonoBehaviour
{
	public GameObject currentCircle;
	public Checker currentChecker;
	public Table table;
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

				if (currentChecker.Move(pos))
				{
					currentCircle.SetActive(false);
					currentChecker = null;
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

	public void ChooseCurrent(Vector3 pos)
	{
		int x = Mathf.RoundToInt(pos.x), y = Mathf.RoundToInt(pos.y);
		currentChecker = table[y, x];
		if(currentChecker != null)
		{
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
