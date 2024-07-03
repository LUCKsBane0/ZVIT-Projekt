using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

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

    private int blockCount = 0; // Tracks the number of blocks
    private enum TutorialStep { Start, Challenge, Block, Attack, Complete }
    private TutorialStep currentStep;

    void Start()
    {
        currentStep = TutorialStep.Start;
        ShowMessage("Welcome to the tutorial! Click OK to start.");
        //okButton.onClick.AddListener(OnOkButtonClicked);
    }

    void Update()
    {
        switch (currentStep)
        {
            case TutorialStep.Challenge:
                if (hasChallenged)
                {
                    hasChallenged = false;
                    ShowMessage("Now block the enemy's attack. Watch the video and follow the instructions.");
                    StartCoroutine(PlayVideo(blockVideo, TutorialStep.Block));
                }
                break;
            case TutorialStep.Block:
                if (hasBlocked)
                {
                    blockCount++;
                    if (blockCount >= 2)
                    {
                        hasBlocked = false;
                        ShowMessage("Now attack the enemy while it's blocking. Watch the video and follow the instructions.");
                        StartCoroutine(PlayVideo(attackVideo, TutorialStep.Attack));
                    }
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

    public void OnOkButtonClicked()
    {
        switch (currentStep)
        {
            case TutorialStep.Start:
                ShowMessage("Watch how to challenge the enemy.");
                StartCoroutine(PlayVideo(challengeVideo, TutorialStep.Challenge));
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
    }

    IEnumerator PlayVideo(VideoClip clip, TutorialStep nextStep)
    {
        videoPlayer.clip = clip;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
        yield return new WaitForSeconds((float)clip.length);
        currentStep = nextStep;
    }
}
