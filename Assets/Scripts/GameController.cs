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
        RPSChoice enemyChoice = (RPSChoice)Random.Range(0, 3);
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
        }
        else if (result == -1)
        {
            // Enemy wins -> enemy happy
            chosen.happy.gameObject.SetActive(true);
            Debug.Log("Enemy Wins!");
            if (coffeeMeter != null)
                coffeeMeter.Decrease();
        }
        else
        {
            // Draw -> show draw sprite for 2s, then revert to neutral
            if (chosen.draw != null)
                StartCoroutine(ShowDrawTemporarily(chosen.draw, 1f));
            else if (enemyNeutral != null)
                ShowOnly(enemyNeutral);

            Debug.Log("Draw!");
        }
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
}
