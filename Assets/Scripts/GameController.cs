using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RPSGameController : MonoBehaviour
{
    public enum RPSChoice { Rock, Paper, Scissors }

    [Header("Player Sprites")]
    public CGRPSEnlarger rockSprite;
    public CGRPSEnlarger paperSprite;
    public CGRPSEnlarger scissorsSprite;

    [System.Serializable]
    public class EnemyReaction
    {
        public SpriteRenderer happy;
        public SpriteRenderer mad;
        public SpriteRenderer draw;
    }

    [Header("Enemy Reactions")]
    public EnemyReaction rockEnemy;
    public EnemyReaction paperEnemy;
    public EnemyReaction scissorsEnemy;
    public SpriteRenderer enemyNeutral;

    [Header("Coffee Meter")]
    public CoffeeMeter coffeeMeter;

    [Header("Player Ability")]
public UnityEngine.UI.Button abilityButton;
    private int playerWinStreak = 0;
    private bool abilityReady = false;
    private bool abilityActive = false;
    public TMPro.TextMeshProUGUI probabilityText;

    private RPSChoice? lastPlayerChoice = null;


    void Start()
    {
        // Subscribe clicks
        rockSprite.OnClicked += () => PlayerChose(RPSChoice.Rock);
        paperSprite.OnClicked += () => PlayerChose(RPSChoice.Paper);
        scissorsSprite.OnClicked += () => PlayerChose(RPSChoice.Scissors);

        // Initialize visuals
        ShowOnly(enemyNeutral);

        // Optional: VN-style pre-game dialogue
        if (coffeeMeter != null)
        {
            coffeeMeter.OnMaxReached += () =>
            {
                Debug.Log("Coffee Meter Full! Trigger VN or mini-game.");
                // Trigger visual novel sequence here
            };
            coffeeMeter.OnMinReached += () =>
            {
                Debug.Log("Coffee Meter Empty! Trigger VN or mini-game.");
                // Trigger alternate sequence here
            };
        }
    }

    void PlayerChose(RPSChoice playerChoice)
{
    // Enemy chooses with adaptive AI
    RPSChoice enemyChoice = GetAdaptiveEnemyChoice();
    lastPlayerChoice = playerChoice;
    Debug.Log($"You: {playerChoice} | Enemy: {enemyChoice}");

    int result = DetermineWinner(playerChoice, enemyChoice);
    HideAll();

    EnemyReaction chosen = GetEnemyReaction(enemyChoice);

    if (result == 1)
    {
        // Player wins -> enemy mad
        chosen.mad.gameObject.SetActive(true);
        Debug.Log("You Win!");
        if (coffeeMeter != null)
            coffeeMeter.Increase();

        // --- Win streak logic ---
        playerWinStreak++;
        if (playerWinStreak >= 3)
        {
            abilityReady = true;
            if (abilityButton != null)
                abilityButton.interactable = true; // enable ability button
            Debug.Log("Ability unlocked! You can now see enemy probabilities next round.");
        }
    }
    else if (result == -1)
    {
        // Enemy wins -> enemy happy
        chosen.happy.gameObject.SetActive(true);
        Debug.Log("Enemy Wins!");
        if (coffeeMeter != null)
            coffeeMeter.Decrease();

        // Reset win streak if player loses
        playerWinStreak = 0;
    }
    else
    {
        // Draw -> show draw sprite for 2s, then revert to neutral
        if (chosen.draw != null)
            StartCoroutine(ShowDrawTemporarily(chosen.draw, 1f));
        else if (enemyNeutral != null)
            ShowOnly(enemyNeutral);

        Debug.Log("Draw!");

        // Reset win streak on draw
        playerWinStreak = 0;
    }
}

    RPSChoice GetAdaptiveEnemyChoice()
    {
        float rockWeight = 1f;
        float paperWeight = 1f;
        float scissorsWeight = 1f;

        if (lastPlayerChoice.HasValue)
        {
            switch (lastPlayerChoice.Value)
            {
                case RPSChoice.Rock: paperWeight = 2f; break;
                case RPSChoice.Paper: scissorsWeight = 2f; break;
                case RPSChoice.Scissors: rockWeight = 2f; break;
            }
        }

        // After enemy makes choice, reset ability
        if (abilityActive)
        {
            abilityActive = false;
            if (probabilityText != null)
                probabilityText.text = ""; // hide probabilities after next move
        }

        float total = rockWeight + paperWeight + scissorsWeight;
        float rand = Random.Range(0f, total);

        if (rand < rockWeight) return RPSChoice.Rock;
        else if (rand < rockWeight + paperWeight) return RPSChoice.Paper;
        else return RPSChoice.Scissors;
    }


    int DetermineWinner(RPSChoice player, RPSChoice enemy)
    {
        if (player == enemy) return 0;

        if ((player == RPSChoice.Rock && enemy == RPSChoice.Scissors) ||
            (player == RPSChoice.Paper && enemy == RPSChoice.Rock) ||
            (player == RPSChoice.Scissors && enemy == RPSChoice.Paper))
            return 1;

        return -1;
    }

    EnemyReaction GetEnemyReaction(RPSChoice choice)
    {
        return choice switch
        {
            RPSChoice.Rock => rockEnemy,
            RPSChoice.Paper => paperEnemy,
            RPSChoice.Scissors => scissorsEnemy,
            _ => null
        };
    }

    void HideAll()
    {
        if (enemyNeutral != null) enemyNeutral.gameObject.SetActive(false);

        rockEnemy.happy.gameObject.SetActive(false);
        rockEnemy.mad.gameObject.SetActive(false);
        if (rockEnemy.draw != null) rockEnemy.draw.gameObject.SetActive(false);

        paperEnemy.happy.gameObject.SetActive(false);
        paperEnemy.mad.gameObject.SetActive(false);
        if (paperEnemy.draw != null) paperEnemy.draw.gameObject.SetActive(false);

        scissorsEnemy.happy.gameObject.SetActive(false);
        scissorsEnemy.mad.gameObject.SetActive(false);
        if (scissorsEnemy.draw != null) scissorsEnemy.draw.gameObject.SetActive(false);
    }

    void ShowOnly(SpriteRenderer sprite)
    {
        HideAll();
        if (sprite != null)
            sprite.gameObject.SetActive(true);
    }

    IEnumerator ShowDrawTemporarily(SpriteRenderer drawSprite, float duration)
    {
        // Hide everything first
        HideAll();

        // Show the draw sprite
        if (drawSprite != null)
            drawSprite.gameObject.SetActive(true);

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        HideAll();

        // Show neutral
        if (enemyNeutral != null)
            enemyNeutral.gameObject.SetActive(true);
    }

    public void ActivateAbility()
    {
        if (!abilityReady) return;

        abilityActive = true;    // The ability will apply this round
        abilityReady = false;    // Disable until next 3-win streak

        if (abilityButton != null)
            abilityButton.interactable = false;

        ShowEnemyProbabilities();
    }


    void ShowEnemyProbabilities()
    {
        if (!abilityActive) return;

        float rockWeight = 1f;
        float paperWeight = 1f;
        float scissorsWeight = 1f;

        // Bias toward last player choice
        if (lastPlayerChoice.HasValue)
        {
            switch (lastPlayerChoice.Value)
            {
                case RPSChoice.Rock: paperWeight = 2f; break;
                case RPSChoice.Paper: scissorsWeight = 2f; break;
                case RPSChoice.Scissors: rockWeight = 2f; break;
            }
        }

        float total = rockWeight + paperWeight + scissorsWeight;
        float rockPercent = rockWeight / total * 100f;
        float paperPercent = paperWeight / total * 100f;
        float scissorsPercent = scissorsWeight / total * 100f;

        if (probabilityText != null)
            probabilityText.text = $"Rock: {rockPercent:F0}%\tPaper: {paperPercent:F0}%\tScissors: {scissorsPercent:F0}%";

        Debug.Log($"Enemy Probabilities:\nRock: {rockPercent}%\nPaper: {paperPercent}%\nScissors: {scissorsPercent}%");
    }

}
