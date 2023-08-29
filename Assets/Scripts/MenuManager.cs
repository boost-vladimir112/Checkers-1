using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class MenuManager : MonoBehaviour
{
    public Bot lv1, lv2, lv3, lv4;
	public Button butt1, butt2, butt3, buttPvP, buttEvE, buttQuit, buttMenu;
	public GameObject back, boxWinWhite, boxWinBlack;
	public Text scoreText;
    public UGameManager gameManager;

	private void Start()
	{
		int score = PlayerPrefs.GetInt("playerScore");
		scoreText.text = PlayerPrefs.GetInt("playerScore").ToString();
		YandexGame.NewLeaderboardScores("best", score);
		butt1.onClick.AddListener(() => 
		{
			gameManager.botWhite = null;
			gameManager.botBlack = lv1;
			gameManager.isPause = false;
			gameManager.isEnd = false;
			gameManager.table.CreatBoard();
			gameManager.step = 1;
			back.SetActive(false);
		});
		butt2.onClick.AddListener(() => 
		{ 
			gameManager.botWhite = null;
			gameManager.botBlack = lv2; 
			gameManager.isPause = false;
			gameManager.isEnd = false;
			gameManager.table.CreatBoard();
			gameManager.step = 1;
			back.SetActive(false);
		});
		butt3.onClick.AddListener(() => 
		{ 
			gameManager.botWhite = null;
			gameManager.botBlack = lv3; 
			gameManager.isPause = false;
			gameManager.isEnd = false;
			gameManager.table.CreatBoard();
			gameManager.step = 1;
			back.SetActive(false);
		});
		buttPvP.onClick.AddListener(() =>
		{
			gameManager.botWhite = null;
			gameManager.botBlack = null;
			gameManager.isPause = false;
			gameManager.isEnd = false;
			gameManager.table.CreatBoard();
			gameManager.step = 1;
			back.SetActive(false);
		});
		buttEvE.onClick.AddListener(() =>
		{
			gameManager.botWhite = lv4;
			gameManager.botBlack = lv3;
			gameManager.isPause = false;
			gameManager.isEnd = false;
			gameManager.table.CreatBoard();
			gameManager.step = 1;
			back.SetActive(false);
		});
		buttQuit.onClick.AddListener(() =>
		{
			Application.Quit();
		});
		buttMenu.onClick.AddListener(() =>
		{
			gameManager.Restart();
			back.SetActive(true);
		});
	}
}
