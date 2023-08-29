using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UGameManager : MonoBehaviour
{
	public MenuManager menu;
	public GameObject currentCircle;
	public Checker currentChecker, playerMovingChecker;
	public Table table;
	public int score = 0, step = 1;
	public float speedChecker = 5;
	public bool needToKick, isEnd = false, isPause = true, isMovePause = false;
	public Bot botWhite, botBlack;

	Vector3 playerTargetPos = Vector3.zero, botTargetPos = Vector3.zero, playerTakenPos = Vector3.zero, botTakenPos = Vector3.zero;
	Checker playerTackenChecker = null, botTackenChecker = null;
	Checker playerVisualChecker = null, botVisualChecker = null; 
	List<Checker> botTackenCheckers = new List<Checker>();
	Move botVisualMove = null;
	int botStepMove = 0;

	AudioSource audioSrc;

	public delegate void VoidFunc();
	public event VoidFunc WinWhite, WinBlack;
	private void Start()
	{
		audioSrc = GetComponent<AudioSource>();
		score = PlayerPrefs.GetInt("playerScore");
		WinWhite += () => 
		{ 
			Debug.Log("White Win!!"); 
			if(botBlack != null)
			{
				score += botBlack.botHard * 10;
				menu.scoreText.text = score.ToString();
				PlayerPrefs.SetInt("playerScore", score);
			}
			menu.boxWinWhite.SetActive(true);
			isEnd = true;

		};
		WinBlack += () =>
		{
			Debug.Log("Black Win!!");
			isEnd = true;
			menu.boxWinBlack.SetActive(true);
		};
	}
	public void Restart()
	{
		isEnd = true;
		isPause = true;
		needToKick = false;
		table.Clear();
		table.board = new Board();
		menu.back.SetActive(true);
	}

	private void Update()
	{

		if(!isEnd && !isMovePause && Input.GetMouseButtonDown(0) && botWhite == null)
		{
			PlayerMove();
		}
		else if (isEnd && Input.GetMouseButtonDown(0) && botWhite == null)
		{
			menu.boxWinWhite.SetActive(false);
			menu.boxWinBlack.SetActive(false);
			Restart();
		}

		// bot visual of checkers move
		if (botVisualMove != null && botVisualChecker != null)
		{
			if (botTackenCheckers.Count > 0 && (Mathf.Abs((botVisualChecker.checkerControll.transform.position - botTakenPos).magnitude) < 0.1f))
			{
				if (botTackenChecker.checkerControll != null)
				{
					Debug.Log("I");
					Debug.Log(botVisualChecker.checkerControll.transform.position + " " + botTakenPos);
					botTackenChecker.checkerControll.DestroyMe();

				}
			}

			if ((Mathf.Abs((botVisualChecker.checkerControll.transform.position - botTargetPos).magnitude) > 0.1f))
			{
				botVisualChecker.checkerControll.transform.position += (botTargetPos - botVisualChecker.checkerControll.transform.position).normalized * speedChecker * Time.deltaTime;
				if (needToKick)
				{
					currentCircle.transform.position = botVisualChecker.checkerControll.transform.position;
				}
			}
			else if (botStepMove < botVisualMove.pos.Count - 1)
			{
				botVisualChecker.checkerControll.transform.position = botTargetPos;
				audioSrc.Play();
				if(botVisualMove.taken.Count > 0)
				{
					botTakenPos = botVisualMove.taken[botStepMove] + table.transform.position;
					botTackenChecker = botTackenCheckers[botStepMove];
					
					Debug.Log("botTakenPos: " + botTakenPos);
					Debug.Log("botTackenChecker: " + botTackenChecker.position);
				}
	
				botTargetPos = botVisualMove.pos[++botStepMove] + table.transform.position;
				Debug.Log("botStepMove: " + botStepMove);
			}
			else
			{
				botVisualChecker.checkerControll.transform.position = botTargetPos;
				audioSrc.Play();
				botVisualChecker = null;
				isMovePause = false;
				NextStep();
			}
		}

		// player visual of checkers move
		if (playerVisualChecker != null && playerVisualChecker.checkerControll != null)
		{
			Debug.Log(playerVisualChecker.checkerControll);
			if (playerTackenChecker != null && (Mathf.Abs((playerVisualChecker.checkerControll.transform.position - playerTakenPos).magnitude) < 0.1f))
			{
				if (playerTackenChecker.checkerControll != null)
				{
					Debug.Log("I");
					playerTackenChecker.checkerControll.DestroyMe();

				}
			}

			if (playerMovingChecker != null && (Mathf.Abs((playerMovingChecker.checkerControll.transform.position - playerTargetPos).magnitude) > 0.1f))
			{
				playerMovingChecker.checkerControll.transform.position += (playerTargetPos - playerMovingChecker.checkerControll.transform.position).normalized * speedChecker * Time.deltaTime;
				currentCircle.transform.position = playerMovingChecker.checkerControll.transform.position;
			
			}
			else if (playerMovingChecker != null)
			{
				playerMovingChecker.checkerControll.transform.position = playerTargetPos;
				audioSrc.Play();
				currentCircle.transform.position = playerMovingChecker.checkerControll.transform.position;
			
				playerMovingChecker = null;
				isMovePause = false;
				if (!needToKick)
				{
					NextStep();
				}
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

		// bot1 move
		if (step % 2 == 1 && botWhite != null)
		{
			Board.RealiseMove(Board.MiniMax(table.board, botWhite.botHard * 2, false).Item2, table.board, true);
			NextStep();
		}
		//bot2 move
		if (step % 2 == 0 && botBlack != null)
		{
			BotThinkMove(botBlack);
		}
	}
	public void BotThinkMove(Bot bot)
	{
		Debug.Log("BotThinkMoveAsync: ");
		botVisualMove = Board.MiniMax(table.board, bot.botHard * 2, bot == botWhite).Item2;
		botVisualChecker = table.board[botVisualMove.pos[0].y, botVisualMove.pos[0].x];
		botStepMove = 0;
		botTargetPos = botVisualMove.pos[botStepMove+1] + table.transform.position;
		//botTackenCheckers = new List<Checker>();
		if (botVisualMove.taken.Count > 0)
		{
			Debug.Log("Takens");
			botTackenCheckers.Clear();
			foreach(Vector3Int v in botVisualMove.taken)
			{
				botTackenCheckers.Add(table.board[v.y, v.x]);
			}
			botTakenPos = botVisualMove.taken[botStepMove] + table.transform.position;
			botTackenChecker = botTackenCheckers[botStepMove];
			Debug.Log("botTakenPos: " + botTakenPos);
			Debug.Log("botTackenChecker: " + botTackenChecker.position);
		}
		isMovePause = true;
		Board.RealiseMove(botVisualMove, table.board);
			
		//NextStep();
	}
	public void PlayerMove()
	{
		Vector3Int pos = GetVector3Int(GetMouseWorldPosition(table.transform.position));
		if (currentChecker != null)
		{
			playerVisualChecker = currentChecker;
			Debug.Log(playerVisualChecker.position);

			if (!needToKick && Board.AbleMove(currentChecker, pos, table.board))
			{
				Board.Move(currentChecker, pos, table.board);
				playerTargetPos = pos + table.transform.position;
				playerMovingChecker = currentChecker;

				currentCircle.SetActive(false);
				currentChecker = null;

			}
			else if (needToKick)
			{
				(bool, Vector3Int) kiskStatus = Board.AbleKick(currentChecker, pos, table.board);
				if (kiskStatus.Item1)
				{
					playerTakenPos = kiskStatus.Item2 + table.transform.position;
					playerTackenChecker = table.board[kiskStatus.Item2.y, kiskStatus.Item2.x];

					Board.Kick(currentChecker, pos, table.board);

					playerTargetPos = pos + table.transform.position;
					isMovePause = true;

					table.board.SetCheckers();

					playerMovingChecker = table.board[pos.y, pos.x];

					needToKick = Board.AbleKick(currentChecker, table.board);

					if (!needToKick)
					{
						currentCircle.SetActive(false);
						currentChecker = null;
					}
				}
				else
				{
					ChooseCurrent(pos);
					playerTargetPos = currentChecker.checkerControll.transform.position;
				}

			}
			else
			{
				ChooseCurrent(pos);
				playerTargetPos = currentChecker.checkerControll.transform.position;
			}
		}
		else
		{
			ChooseCurrent(pos);
		}
	}
	public bool ChooseCurrent(Vector3Int pos)
	{
		if (pos.x > 7 || pos.x < 0 || pos.y > 7 || pos.y < 0) return false;
		if (table.board == null) return false;
		Debug.Log(table);
		Debug.Log(table.board);
		if (!table.board.IsEmpty(pos) && table.board[pos.y, pos.x].isWhite == (step % 2 == 1) && table.board[pos.y, pos.x].isNeedAttack == needToKick)
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
