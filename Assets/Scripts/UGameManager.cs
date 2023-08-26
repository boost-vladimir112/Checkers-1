using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGameManager : MonoBehaviour
{
	public MenuManager menu;
	public GameObject currentCircle;
	public Checker currentChecker, movingChecker;
	public Table table;
	public int score = 0, step = 1;
	public bool needToKick, isEnd = false, isPause = true, isMovePause = false;
	public Player player1, player2;
	Vector3 target = Vector3.zero;
	Vector3Int startPos = Vector3Int.zero;
	Move botVisualMove = null;
	int botStepMove = 0;
	Checker botVisualChecker = null;
	AudioSource audio;

	public delegate void VoidFunc();
	public event VoidFunc WinWhite, WinBlack;
	private void Start()
	{
		audio = GetComponent<AudioSource>();
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

		};
		WinBlack += () =>
		{
			Debug.Log("Black Win!!");
			isEnd = true;

		};

	}
	public void Restart()
	{
		isEnd = true;
		isPause = true;
		table.Clear();
		table.board = new Board();
		menu.back.SetActive(true);
	}

	private void Update()
	{
		if(!isEnd && !isPause && Input.GetMouseButtonDown(0) && player1 == null)
		{
			Vector3Int pos = GetVector3Int(GetMouseWorldPosition(table.transform.position));
			if (currentChecker != null)
			{
				if (!needToKick && Board.AbleMove(currentChecker, pos, table.board))
				{
					Board.Move(currentChecker, pos, table.board);
					target = pos + table.transform.position;
					movingChecker = currentChecker;

					currentCircle.SetActive(false);
					currentChecker = null;
					
				}
				else if(needToKick)
				{

					if (Board.AbleKick(currentChecker, pos, table.board))
					{
						startPos = currentChecker.position;
						Board.Kick(currentChecker, pos, table.board, true);

						target = pos + table.transform.position;

						table.board.SetCheckers();

						movingChecker = table.board[pos.y, pos.x];

						needToKick = Board.AbleKick(currentChecker, table.board);
						
						if(!needToKick)
						{
							currentCircle.SetActive(false);
							currentChecker = null;
						}
					}
					else				
					{
						ChooseCurrent(pos);
						target = currentChecker.checkerControll.transform.position;
					}

				}
				else
				{
					ChooseCurrent(pos);
					target = currentChecker.checkerControll.transform.position;
				}
			}
			else
			{
				if(ChooseCurrent(pos))	target = currentChecker.checkerControll.transform.position;
			}
		}
		else if (isEnd && Input.GetMouseButtonDown(0) && player1 == null)
		{
			Restart();
		}
		else if (Input.GetMouseButtonDown(0) && player1 != null)
		{
			NextStep();
		}

		// player visual of checkers move
		if (movingChecker != null && (Mathf.Abs((movingChecker.checkerControll.transform.position - target).magnitude) > 0.1f))
		{
			movingChecker.checkerControll.transform.position += (target - movingChecker.checkerControll.transform.position).normalized * 5 * Time.deltaTime;
			if (needToKick)
			{
				currentCircle.transform.position = movingChecker.checkerControll.transform.position;
			}
		}
		else if (movingChecker != null)
		{
			movingChecker.checkerControll.transform.position = target;
			audio.Play();
			if (needToKick)
			{
				currentCircle.transform.position = movingChecker.checkerControll.transform.position;
			}
			movingChecker = null;
			if (!needToKick) NextStep();
		}

		// bot visual of checkers move
		if(botVisualMove != null && botVisualChecker != null)
		{
			Debug.Log(botVisualMove);
			Debug.Log(botVisualChecker.position);
			Debug.Log("->" + target);
			if ((Mathf.Abs((botVisualChecker.checkerControll.transform.position - target).magnitude) > 0.1f))
			{
				botVisualChecker.checkerControll.transform.position += (target - botVisualChecker.checkerControll.transform.position).normalized * 5 * Time.deltaTime;
				if (needToKick)
				{
					currentCircle.transform.position = botVisualChecker.checkerControll.transform.position;
				}
			}
			else if(botStepMove < botVisualMove.pos.Count-1)
			{
				botVisualChecker.checkerControll.transform.position = target;
				audio.Play();
				target = botVisualMove.pos[++botStepMove] + table.transform.position;
			}
			else
			{
				botVisualChecker.checkerControll.transform.position = target;
				botVisualChecker = null;
				//NextStep();
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
			botVisualMove = Board.MiniMax(table.board, player2.playerHard * 2, false).Item2;
			botVisualChecker = table.board[botVisualMove.pos[0].y, botVisualMove.pos[0].x];
			target = botVisualMove.pos[1] + table.transform.position;
			botStepMove = 0;
			Board.RealiseMove(botVisualMove, table.board, true);
			NextStep();
		}
	}
	public bool ChooseCurrent(Vector3Int pos)
	{
		if (pos.x > 7 || pos.x < 0 || pos.y > 7 || pos.y < 0) return false;
		if(!table.board.IsEmpty(pos) && table.board[pos.y, pos.x].isWhite == (step % 2 == 1) && table.board[pos.y, pos.x].isNeedAttack == needToKick)
		{
			currentChecker = table.board[pos.y, pos.x];

			currentCircle.SetActive(true);
			currentCircle.transform.position = currentChecker.checkerControll.transform.position;
			return true;
		}
		return false;
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
