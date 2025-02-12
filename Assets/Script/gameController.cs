﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameController : MonoBehaviour
{
	public Text questionDisplayText;
    public Text scoreDisplayText;
    public Text timeRemainingDisplayText;
	public simpleObjectPool answerButtonObjectPool;
	public Transform answerButtonParent;
    public GameObject questionDisplay;
    public GameObject roundEndDisplay;

	private dataController dataController;
	private roundData currentRoundData;
	private questionData[] questionPool;

	private bool isRoundActive;
	private float timeRemaining;
	private int questionIndex;
    private int playerScore;
	private List<GameObject> answerButtonGameObjects = new List<GameObject>();

    // class ini buat controller yang dipanggil saat masuk halaman quiz
    void Start()
    {
    	dataController = FindObjectOfType<dataController> ();
    	currentRoundData = dataController.GetCurrentRoundData();
    	questionPool = currentRoundData.question; 
    	timeRemaining = currentRoundData.timeLimitInSeconds;
        UpdateTimeRemainingDisplay();

    	playerScore = 0;
    	questionIndex = 0;

    	ShowQuestion();
    	isRoundActive = true;  
    }

    private void ShowQuestion(){
    	RemoveAnswerButtons();
    	questionData questionData = questionPool[questionIndex];
    	questionDisplayText.text = questionData.questionText;

    	for (int i = 0; i < questionData.answer.Length; i++){
    		GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();
    		answerButtonGameObject.transform.SetParent(answerButtonParent);
    		answerButtonGameObjects.Add(answerButtonGameObject);

    		answerButton answerButton = answerButtonGameObject.GetComponent<answerButton>();
    		answerButton.Setup(questionData.answer[i]);

    		
    	}

    }

    private void RemoveAnswerButtons(){
    	while (answerButtonGameObjects.Count > 0){
    		answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
    		answerButtonGameObjects.RemoveAt(0);
    	}
    }

    public void AnswerButtonClicked(bool isCorrect){
    	if(isCorrect){
    		playerScore += currentRoundData.pointAddedForCorrectAnswer;
            scoreDisplayText.text = "Score: " + playerScore.ToString();
    	}

        if(questionPool.Length > questionIndex + 1)
        {
            questionIndex++;
            ShowQuestion();
        }
        else
        {
            EndRound();
        }
    }

    public void EndRound()
    {
        isRoundActive = false;
        questionDisplay.SetActive(false);
        roundEndDisplay.SetActive(true);
    }

    public void ToMenu()
    {
        Application.LoadLevel("mainMenuScene");
    }

    private void UpdateTimeRemainingDisplay()
    {
        timeRemainingDisplayText.text = "Waktu: " + Mathf.Round(timeRemaining).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRoundActive)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeRemainingDisplay();

            if(timeRemaining <= 0f)
            {
                EndRound();
            }
        }
    }
}
