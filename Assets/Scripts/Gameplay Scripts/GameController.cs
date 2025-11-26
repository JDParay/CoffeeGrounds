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
public CGRPSEnlarger ProbabilityButton;
public CGRPSEnlarger EliminationButton;
public CGRPSEnlarger BlessingButton;
    private int playerWinStreak = 4;
    private int playerDrawStreak = 2; //testing
    private int playerFailStreak = 3;
    private bool abilityReady = false;
    private bool abilityActive = false; 
    private bool eliminationAbilityReady = false; 
    private bool eliminationAbilityActive = false;
    private bool winStreakReady = false;
    private bool winStreakActive = false;

    public TMPro.TextMeshProUGUI probabilityText;
    private RPSChoice nextEnemyChoice;

    [Header("Blocker PNGs (separate inputs)")]
    public GameObject[] blockerPNGs;

    private RPSChoice? lastPlayerChoice = null;


    void Start()
    {
        // Subscribe clicks
        rockSprite.OnClicked += () => PlayerChose(RPSChoice.Rock);
        paperSprite.OnClicked += () => PlayerChose(RPSChoice.Paper);
        scissorsSprite.OnClicked += () => PlayerChose(RPSChoice.Scissors);

        if (ProbabilityButton != null)
    {
        ProbabilityButton.OnClicked += () =>
        {
            Debug.Log("Probability skill clicked!");
            ActivatePredictionAbility();
        };

        ProbabilityButton.boxCollider.enabled = true;
    }

    if (EliminationButton != null)
    {
        EliminationButton.OnClicked += () =>
        {
            Debug.Log("Elimination skill clicked!");
            ActivateEliminationAbility();
        };

        EliminationButton.boxCollider.enabled = true;
    }

    if (BlessingButton != null)
    {
        BlessingButton.OnClicked += () =>
        {
            Debug.Log("Blessing skill clicked!");
            ActivateBlessingAbility();
        };

        BlessingButton.boxCollider.enabled = true;
    }

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
    if (abilityActive && probabilityText != null)
{
    probabilityText.text = "";
    abilityActive = false;
    Animator anim = ProbabilityButton.GetComponent<Animator>();
    if (anim != null)
        anim.speed = 1f;
    ProbabilityButton.boxCollider.enabled = true;
}

    if (eliminationAbilityActive)
    {
    if (EliminationButton != null)
        EliminationButton.boxCollider.enabled = true;
        Animator anim = EliminationButton.GetComponent<Animator>();
        if (anim != null)
        anim.speed = 1f;  
    }

    if (!eliminationAbilityActive)
    nextEnemyChoice = GetAdaptiveEnemyChoice();

    RPSChoice enemyChoice = nextEnemyChoice;

    if (winStreakActive)
    {
        if (BlessingButton != null)
        BlessingButton.boxCollider.enabled = true;
    }
    lastPlayerChoice = playerChoice;
    Debug.Log($"You: {playerChoice} | Enemy: {enemyChoice}");

    int result = DetermineWinner(playerChoice, enemyChoice);
    HideAll();

    EnemyReaction chosen = GetEnemyReaction(enemyChoice);

    //WINSTREAK LOGIC
    if (result == 1)
    {
    chosen.mad.gameObject.SetActive(true);

        if (coffeeMeter != null)
            coffeeMeter.Increase();

        playerWinStreak++;

        if (playerWinStreak >= 5 && !winStreakReady)
        {
            winStreakReady = true;

            if (BlessingButton != null)
            {
                BlessingButton.boxCollider.enabled = true;
                BlessingButton.NotifySkillUnlocked();
            }

            Debug.Log("Win Streak Reward Ready! (5 wins)");
        }
        
        playerDrawStreak = 0;
        playerFailStreak = 0;

    }
    //ELIMINATION LOGIC
    else if (result == -1)
    {
        chosen.happy.gameObject.SetActive(true);
        if (coffeeMeter != null)
            coffeeMeter.Decrease();

        playerFailStreak++;

    if (playerFailStreak >= 4 && !eliminationAbilityReady) 
    {
        eliminationAbilityReady = true;

        if (EliminationButton != null)
            {EliminationButton.boxCollider.enabled = true;
            EliminationButton.NotifySkillUnlocked();}
        Debug.Log("Elimination Ability Ready (4 fails).");
    }
        playerWinStreak = 0;
        playerDrawStreak = 0;
    }  
    else
    {   //DRAW LOGIC!!!! 
        StartCoroutine(ShowDrawTemporarily(chosen.draw, 1f));

        playerDrawStreak++;

        if (playerDrawStreak >= 3 && !abilityReady)
        {
            abilityReady = true;

            if (ProbabilityButton != null)
                { ProbabilityButton.boxCollider.enabled = true;
                ProbabilityButton.NotifySkillUnlocked(); }

            Debug.Log("Ability Unlocked! (3 Draws in a row)");
        }

        playerWinStreak = 0;
        playerFailStreak = 0;
    }
    Invoke(nameof(ResetChoices), 0.2f);

    if (eliminationAbilityActive)
    {
    eliminationAbilityActive = false;
    if (EliminationButton != null)
        EliminationButton.boxCollider.enabled = true;
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

        if (abilityActive) 
        { 
            abilityActive = false; 
            if (probabilityText != null) 
            probabilityText.text = "";}

        float total = rockWeight + paperWeight + scissorsWeight;
        float rand = Random.Range(0f, total);

        if (rand < rockWeight) return RPSChoice.Rock;
        else if (rand < rockWeight + paperWeight) return RPSChoice.Paper;
        else return RPSChoice.Scissors;
    }

    void DisableWrongChoices(RPSChoice enemyChoice)
    {
        RPSChoice correct = GetCounterChoice(enemyChoice);

        CGRPSEnlarger[] all = { rockSprite, paperSprite, scissorsSprite };

        foreach (var c in all)
        {
            bool isCorrect = (c.choiceType == correct);

            c.boxCollider.enabled = isCorrect;

            if (c.blockerPNG != null)
                c.blockerPNG.SetActive(!isCorrect);
        }
    }

    void ResetChoices()
    {
        CGRPSEnlarger[] all = { rockSprite, paperSprite, scissorsSprite };

        for (int i = 0; i < all.Length; i++)
        {
            var c = all[i];
            c.boxCollider.enabled = true;

            if (c.blockerPNG != null)
                c.blockerPNG.SetActive(false);

            if (blockerPNGs != null && blockerPNGs.Length == 3 && blockerPNGs[i] != null)
                blockerPNGs[i].SetActive(false);
        }

        // clear probability text if needed
        if (probabilityText != null)
            probabilityText.text = "";
    }


    public static RPSChoice GetCounterChoice(RPSChoice enemy)
    {
        return enemy switch
        {
            RPSChoice.Rock => RPSChoice.Paper,
            RPSChoice.Paper => RPSChoice.Scissors,
            RPSChoice.Scissors => RPSChoice.Rock,
            _ => enemy
        };
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

    public void ActivatePredictionAbility()
    {
        Debug.Log("ActivatePredictionAbility called, abilityReady=" + abilityReady);

        if (!abilityReady) return;

        abilityActive = true;
        abilityReady = false;

        if (ProbabilityButton != null)
        {
            ProbabilityButton.boxCollider.enabled = true;

            Animator anim = ProbabilityButton.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play(0, 0, 0f);
            anim.speed = 0f;
        }
        }    

    ShowEnemyProbabilities();
    }

    public void ActivateEliminationAbility()
{
    Debug.Log("ActivateEliminationAbility called, eliminationAbilityReady=" + eliminationAbilityReady);

    if (!eliminationAbilityReady) return;

    eliminationAbilityReady = false;
    eliminationAbilityActive = true;

    nextEnemyChoice = GetAdaptiveEnemyChoice();

    if (probabilityText != null)
        probabilityText.text = $"{nextEnemyChoice} : 100%";

    RPSChoice correctPlayerChoice = GetCounterChoice(nextEnemyChoice);

    CGRPSEnlarger[] all = { rockSprite, paperSprite, scissorsSprite };
    for (int i = 0; i < all.Length; i++)
    {
        var c = all[i];
        bool isCorrect = (c.choiceType == correctPlayerChoice);

        c.boxCollider.enabled = isCorrect;

        if (blockerPNGs != null && blockerPNGs.Length == 3)
        {
            if (blockerPNGs[i] != null)
                blockerPNGs[i].SetActive(!isCorrect);
        }
        else
        {
            if (c.blockerPNG != null)
                c.blockerPNG.SetActive(!isCorrect);
        }
    }

    Animator anim = EliminationButton.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play(0, 0, 0f);
            anim.speed = 0f;
        }
}

    public void ActivateBlessingAbility()
    {
        if (!winStreakReady) return;

        winStreakReady = false;
        winStreakActive = true;

        if (coffeeMeter != null)
            coffeeMeter.Increase();

        if (BlessingButton != null)
            BlessingButton.boxCollider.enabled = true;

        Debug.Log("Win Streak Power-Up Activated! Coffee increased!");

        playerWinStreak = 0;

        Animator anim = BlessingButton.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play(0, 0, 0f);
        }
    }


    void ShowEnemyProbabilities()
    {
        if (!abilityActive) return;

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

        float total = rockWeight + paperWeight + scissorsWeight;
        float rockPercent = rockWeight / total * 100f;
        float paperPercent = paperWeight / total * 100f;
        float scissorsPercent = scissorsWeight / total * 100f;

        if (probabilityText != null)
            probabilityText.text = $"Rock: {rockPercent:F0}%\tPaper: {paperPercent:F0}%\tScissors: {scissorsPercent:F0}%";

        Debug.Log($"Enemy Probabilities:\nRock: {rockPercent}%\nPaper: {paperPercent}%\nScissors: {scissorsPercent}%");
    }

    void ShowForcedChoices()
    {
        RPSChoice forcedEnemyChoice = GetAdaptiveEnemyChoice();
        
        DisableWrongChoices(forcedEnemyChoice);
        ShowEnemyProbabilities();
    }

}
