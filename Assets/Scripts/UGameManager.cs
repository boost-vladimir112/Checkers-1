using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGameManager : MonoBehaviour
{
	public MenuManager menu;
	public GameObject currentCircle;
	public Checker currentChecker;
	public Table table;
	public int score = 0, step = 1;
	public bool needToKick, isEnd = false, isPause = true;
	public Player player1, player2;


	public delegate void VoidFunc();
	public event VoidFunc WinWhite, WinBlack;
	private void Start()
	{
		score = PlayerPrefs.GetInt("playerScore");
		WinWhite += () => 
		{ 
			Debug.Log("White Win!!"); 
			if(player2 != null)
			{
				score += player2.playerHard * 10;
				menu.scoreText.text = score.ToString();
				PlayerPrefs.SetInt("playerScore", score);
			}
			isEnd = true;
			isPause = true;
			table.Clear();
			table.board = new Board();
			menu.back.SetActive(true);
		};
		WinBlack += () =>
		{
			Debug.Log("Black Win!!");
			isEnd = true;
			isPause = true;
			table.Clear();
			table.board = new Board();
			menu.back.SetActive(true);
		};

	}

	private void Update()
	{
		if(!isEnd && !isPause && Input.GetMouseButtonDown(0) && player1 == null)
		{
			Vector3Int pos = GetVector3Int(GetMouseWorldPosition(table.transform.position));
			Debug.Log(pos);
			if (currentChecker != null)
			{
				if (!needToKick && Board.AbleMove(currentChecker, pos, table.board))
				{
					Board.Move(currentChecker, pos, table.board);
					currentChecker.checkerControll.transform.position = currentChecker.position + table.transform.position;

					currentCircle.SetActive(false);
					currentChecker = null;
					NextStep();
					
				}
				else if(needToKick)
				{

					if (Board.AbleKick(currentChecker, pos, table.board))
					{
						Board.Kick(currentChecker, pos, table.board, true);

						table.board.SetCheckers();
						currentChecker.checkerControll.transform.position = new Vector3(currentChecker.position.x, currentChecker.position.y) + table.transform.position;
						needToKick = false;
						if(Board.AbleKick(currentChecker, table.board))
						{
							needToKick = true;
						}

						if (needToKick)
						{
							currentCircle.transform.position = currentChecker.checkerControll.transform.position;
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
		else if(Input.GetMouseButtonDown(0) && player1 == null)
		{

		}
		else if(Input.GetMouseButtonDown(0) && player1 != null)
		{
			NextStep();
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
			return;
		}
		needToKick = false;
		bool isAbleMove = false;
		foreach (Checker c in list)
		{
			if (Board.AbleKick(c, table.board))
			{
				isAbleMove = true;
				needToKick = true;
			}
			if (!isAbleMove && Board.AbleMove(c, table.board))
			{
				isAbleMove = true;
			}
		}
		if (!isAbleMove)
		{
			if (step % 2 == 1) WinBlack?.Invoke();
			else WinWhite?.Invoke();
			return;
		}

		if (step % 2 == 1 && player1 != null)
		{
			Board.RealiseMove(Board.MiniMax(table.board, player1.playerHard * 2, false).Item2, table.board, true);
			NextStep();
		}
		if (step % 2 == 0 && player2 != null)
		{
			Board.RealiseMove(Board.MiniMax(table.board, player2.playerHard * 2, false).Item2, table.board, true);
			NextStep();
		}
	}
	public void ChooseCurrent(Vector3Int pos)
	{
		if(!table.board.IsEmpty(pos) && table.board[pos.y, pos.x].isWhite == (step % 2 == 1) && table.board[pos.y, pos.x].isNeedAttack == needToKick)
		{
			currentChecker = table.board[pos.y, pos.x];

			currentCircle.SetActive(true);
			currentCircle.transform.position = currentChecker.checkerControll.transform.position;
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
