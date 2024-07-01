using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText; // Reference to the UI Text component
    public Button okButton; // Reference to the OK button
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    public GameObject enemy; // Reference to the enemy GameObject

    public VideoClip challengeVideo; // Video of challenging the enemy
    public VideoClip blockVideo; // Video of blocking the enemy's attack
    public VideoClip attackVideo; // Video of attacking the enemy

    public bool hasChallenged = false; // Tracks if the player has challenged the enemy
    public bool hasBlocked = false; // Tracks if the player has blocked an attack
    public bool hasAttacked = false; // Tracks if the player has attacked the enemy

    private enum TutorialStep { Start, Challenge, Block, Attack, Complete }
    private TutorialStep currentStep;

    void Start()
    {
        currentStep = TutorialStep.Start;
        ShowMessage("Welcome to the tutorial! Click OK to start.");
        okButton.onClick.AddListener(OnOkButtonClicked);
    }

    void Update()
    {
        switch (currentStep)
        {
            case TutorialStep.Challenge:
                if (hasChallenged)
                {
                    hasChallenged = false;
                    ShowMessage("Now block the enemy's attack. Click OK to continue.");
                    currentStep = TutorialStep.Block;
                }
                break;
            case TutorialStep.Block:
                if (hasBlocked)
                {
                    hasBlocked = false;
                    ShowMessage("Now attack the enemy while it's blocking. Click OK to continue.");
                    currentStep = TutorialStep.Attack;
                }
                break;
            case TutorialStep.Attack:
                if (hasAttacked)
                {
                    hasAttacked = false;
                    ShowMessage("Tutorial completed! Well done.");
                    currentStep = TutorialStep.Complete;
                }
                break;
        }

        // Check if the enemy is defeated
        if (enemy == null && currentStep != TutorialStep.Complete)
        {
            Debug.Log("completed");
            currentStep = TutorialStep.Complete;
        }
    }

    void OnOkButtonClicked()
    {
        switch (currentStep)
        {
            case TutorialStep.Start:
                StartCoroutine(PlayVideo(challengeVideo));
                ShowMessage("Watch how to challenge the enemy. Click OK to continue.");
                currentStep = TutorialStep.Challenge;
                break;
            case TutorialStep.Challenge:
                StartCoroutine(PlayVideo(blockVideo));
                break;
            case TutorialStep.Block:
                StartCoroutine(PlayVideo(attackVideo));
                break;
            case TutorialStep.Attack:
                tutorialText.text = "Attack the enemy to complete the tutorial.";
                break;
            case TutorialStep.Complete:
                tutorialText.gameObject.SetActive(false);
                okButton.gameObject.SetActive(false);
                break;
        }
    }

    void ShowMessage(string message)
    {
        tutorialText.text = message;
        tutorialText.gameObject.SetActive(true);
        okButton.gameObject.SetActive(true);
    }

    IEnumerator PlayVideo(VideoClip clip)
    {	
        videoPlayer.clip = clip;
        videoPlayer.Play();
        videoPlayer.isLooping = true;
        yield return new WaitForSeconds((float)clip.length);
        videoPlayer.Stop();
    }
}
