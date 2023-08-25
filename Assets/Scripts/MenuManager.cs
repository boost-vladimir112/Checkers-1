using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Player lv1, lv2, lv3, lv4;
	public Button butt1, butt2, butt3, buttPvP, buttEvE, buttQuit;
	public GameObject back;
	public Text scoreText;
    public UGameManager gameManager;

	private void Start()
	{
		butt1.onClick.AddListener(() => 
		{
			gameManager.player1 = null;
			gameManager.player2 = lv1;
			gameManager.isPause = false;
			gameManager.isEnd = false;
			gameManager.table.CreatBoard();
			gameManager.step = 1;
			back.SetActive(false);
		});
		butt2.onClick.AddListener(() => 
		{ 
			gameManager.player1 = null;
			gameManager.player2 = lv2; 
			gameManager.isPause = false;
			gameManager.isEnd = false;
			gameManager.table.CreatBoard();
			gameManager.step = 1;
			back.SetActive(false);
		});
		butt3.onClick.AddListener(() => 
		{ 
			gameManager.player1 = null;
			gameManager.player2 = lv3; 
			gameManager.isPause = false;
			gameManager.isEnd = false;
			gameManager.table.CreatBoard();
			gameManager.step = 1;
			back.SetActive(false);
		});
		buttPvP.onClick.AddListener(() =>
		{
			gameManager.player1 = null;
			gameManager.player2 = null;
			gameManager.isPause = false;
			gameManager.isEnd = false;
			gameManager.table.CreatBoard();
			gameManager.step = 1;
			back.SetActive(false);
		});
		buttEvE.onClick.AddListener(() =>
		{
			gameManager.player1 = lv4;
			gameManager.player2 = lv3;
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
	}
}
