using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour {
    public static AchievementManager instance;
    
    private List<Achievement> achievements;
    /*public GameObject visualAchivementCompleted;
    public GameObject visualAchivementNew;*/
    private Achievement currentAchievment;
    public Canvas canvas;
    // Use this for initialization

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        achievements = new List<Achievement>();
        //InitList();
        int tmpAchivmt = GetActiveAchievement();
        currentAchievment = tmpAchivmt >= GetTotalNumOfAchievements() ? null : achievements[GetActiveAchievement()];
    }

    private void Start()
    {
        //if(currentAchievment != null)      
             //StartCoroutine(InitialAchivement());
    }
    void InitList()
    {
        achievements.Add(new Achievement(0, "Complete the tutorial", 1, 15, Achievement.ACHIEVMENT_TUTORIAL, false));
        achievements.Add(new Achievement(1, "Score 10 points ", 10, 15, Achievement.ACHIEVMENT_SCORE, true));
        achievements.Add(new Achievement(2, "Collect 5 coins", 5, 15, Achievement.ACHIEVMENT_COINS,true));
        achievements.Add(new Achievement(3, "Destroy a shield", 1, 15, Achievement.ACHIEVMENT_DESTROY_SHIELD, false));
        achievements.Add(new Achievement(4, "Destroy a enemy while invincible", 1, 15, Achievement.ACHIEVMENT_ENEMIES_INVINCIBLE, false));
        achievements.Add(new Achievement(5, "Collect a coin while equipped with Magnet", 1, 15, Achievement.ACHIEVMENT_COINS_MAGNET, false));
        achievements.Add(new Achievement(6, "Score a total of 20 points", 20, 15, Achievement.ACHIEVMENT_SCORE,true));

        achievements.Add(new Achievement(7, "Pickup 3 power-ups in a single Game", 3, 20, Achievement.ACHIEVMENT_POWERUPS, false));
        achievements.Add(new Achievement(8, "Collect 15 coins in a single game", 15, 20, Achievement.ACHIEVMENT_COINS, false));
        achievements.Add(new Achievement(9, "Play 3 games", 3, 20, Achievement.ACHIEVMENT_GAMES, true));
        achievements.Add(new Achievement(10, "Score 20 points in a single Game", 20, 20, Achievement.ACHIEVMENT_SCORE, false));
        achievements.Add(new Achievement(11, "Destroy 10 enemies while invincible", 10, 20, Achievement.ACHIEVMENT_ENEMIES_INVINCIBLE, true));
        achievements.Add(new Achievement(12, "Destroy 2 shields in a single Game", 2, 20, Achievement.ACHIEVMENT_DESTROY_SHIELD, false));
        achievements.Add(new Achievement(13, "Collect 40 coins", 40, 20, Achievement.ACHIEVMENT_COINS, true));
        achievements.Add(new Achievement(14, "Collect 10 coins while equipped with Magnet", 10, 20, Achievement.ACHIEVMENT_COINS_MAGNET, true));

        achievements.Add(new Achievement(15, "Pickup 10 power-ups", 10, 25, Achievement.ACHIEVMENT_POWERUPS, true));
        achievements.Add(new Achievement(16, "Play 5 games", 5, 25, Achievement.ACHIEVMENT_GAMES, true));
        achievements.Add(new Achievement(17, "Score 40 points in a single Game", 40, 25, Achievement.ACHIEVMENT_SCORE, false));
        achievements.Add(new Achievement(18, "Destroy 10 shields", 10, 25, Achievement.ACHIEVMENT_DESTROY_SHIELD, true));
        achievements.Add(new Achievement(19, "Collect 40 coins in a single game", 40, 25, Achievement.ACHIEVMENT_COINS, false));
        achievements.Add(new Achievement(20, "Destroy 20 enemies while invincible", 20, 25, Achievement.ACHIEVMENT_ENEMIES_INVINCIBLE, true));
        achievements.Add(new Achievement(21, "Pickup 20 power-ups", 20, 25, Achievement.ACHIEVMENT_POWERUPS, true));
        achievements.Add(new Achievement(22, "Collect 40 coins while equipped with Magnet", 40, 25, Achievement.ACHIEVMENT_COINS_MAGNET, true));

        achievements.Add(new Achievement(23, "Score 70 points in a single Game", 70, 30, Achievement.ACHIEVMENT_SCORE, false));
        achievements.Add(new Achievement(24, "Destroy 20 shields", 20, 30, Achievement.ACHIEVMENT_DESTROY_SHIELD, true));
        achievements.Add(new Achievement(25, "Collect 100 coins", 100, 30, Achievement.ACHIEVMENT_COINS, true));
        achievements.Add(new Achievement(26, "Destroy 15 enemies while invincible in a single game", 15, 30, Achievement.ACHIEVMENT_ENEMIES_INVINCIBLE, false));
        achievements.Add(new Achievement(27, "Pickup 40 power-ups", 40, 30, Achievement.ACHIEVMENT_POWERUPS, true));
        achievements.Add(new Achievement(28, "Play 10 games", 10, 30, Achievement.ACHIEVMENT_GAMES, true));
        achievements.Add(new Achievement(29, "Collect 60 coins while equipped with Magnet", 40, 30, Achievement.ACHIEVMENT_COINS_MAGNET, true));

        achievements.Add(new Achievement(30, "Score 200 points ", 200, 35, Achievement.ACHIEVMENT_SCORE, true));
        achievements.Add(new Achievement(31, "Destroy 8 shields in a single game", 8, 35, Achievement.ACHIEVMENT_DESTROY_SHIELD, false));
        achievements.Add(new Achievement(32, "Collect 200 coins", 200, 35, Achievement.ACHIEVMENT_COINS, true));
        achievements.Add(new Achievement(33, "Destroy 60 enemies while invincible", 60, 35, Achievement.ACHIEVMENT_ENEMIES_INVINCIBLE, true));
        achievements.Add(new Achievement(34, "Pickup 40 power-ups", 40, 35, Achievement.ACHIEVMENT_POWERUPS, true));
        achievements.Add(new Achievement(35, "Play 20 games", 20, 35, Achievement.ACHIEVMENT_GAMES, true));
        achievements.Add(new Achievement(36, "Collect 100 coins while equipped with Magnet", 100, 35, Achievement.ACHIEVMENT_COINS_MAGNET, true));

        achievements.Add(new Achievement(37, "Score 400 points ", 400, 40, Achievement.ACHIEVMENT_SCORE, true));
        achievements.Add(new Achievement(38, "Destroy 200 shields", 200, 40, Achievement.ACHIEVMENT_DESTROY_SHIELD, true));
        achievements.Add(new Achievement(39, "Collect 500 coins", 500, 40, Achievement.ACHIEVMENT_COINS, true));
        achievements.Add(new Achievement(40, "Destroy 120 enemies while invincible",120, 40, Achievement.ACHIEVMENT_ENEMIES_INVINCIBLE, true));
        achievements.Add(new Achievement(41, "Pickup 100 power-ups", 100, 40, Achievement.ACHIEVMENT_POWERUPS, true));
        achievements.Add(new Achievement(42, "Play 40 games", 40, 40, Achievement.ACHIEVMENT_GAMES, true));
        achievements.Add(new Achievement(43, "Collect 250 coins while equipped with Magnet", 250, 40, Achievement.ACHIEVMENT_COINS_MAGNET, true));

    }
    public int GetTotalNumOfAchievements()
    {
        return achievements.Count;
    }

	public void Reseted()
    {
        //reseted = true;
        currentAchievment = achievements[0];
        PlayerPrefs.SetInt("CurrentChallenge", 0);
        //StartCoroutine(ShowAchivement());
    }

    Achievement SetNewActiveAchivement(int newId)
    {
        newId++;
        if (newId >= achievements.Count)
        {
            //COMPLETED ALL
            //StartCoroutine(ShowAllChallengesCompleted());
            PlayerPrefs.SetInt("CurrentChallenge", 999);
            return null;
        }
        else
        {
            PlayerPrefs.SetInt("CurrentChallenge", newId);
            return achievements[newId];
        }
    }

    public Achievement GetAchievement()
    {
        return currentAchievment;
    } 

    public int GetActiveAchievement()
    {
        return PlayerPrefs.GetInt("CurrentChallenge", 0);
    }

    public void EarnAchievment(int id, int points)
    {
        if (currentAchievment == null)
            return;
        if (achievements[id].EarnAchievement(points))
        {
           // GameManager.instance.incrementCoinsReward(currentAchievment.Reward);
           // CompletedAchievmentVisual();
            currentAchievment = SetNewActiveAchivement(currentAchievment.Id);
            //StartCoroutine(HideAchivement());
        }
    }
    /*public void CompletedAchievmentVisual()
    {
        visualAchivementCompleted.SetActive(true);
        visualAchivementCompleted.transform.GetChild(0).GetComponent<Text>().text = "CHALLENGE " + (currentAchievment .Id + 1)+ " COMPLETED";
        visualAchivementCompleted.transform.GetChild(1).GetComponent<Text>().text = currentAchievment.Description;
        visualAchivementCompleted.transform.GetChild(2).GetComponent<Text>().text = "+"+currentAchievment.Reward;
    }*/

   /* public void NewAchievmentVisual()
    {
        visualAchivementNew.SetActive(true);
        visualAchivementNew.transform.GetChild(0).GetComponent<Text>().text = "CHALLENGE " + (currentAchievment.Id + 1);
        visualAchivementNew.transform.GetChild(1).GetComponent<Text>().text = currentAchievment.Description;
        visualAchivementNew.transform.GetChild(2).GetComponent<Text>().text = "NEW";
    }*/

    /*public void InitialAchievmentVisual()
    {
        visualAchivementNew.SetActive(true);
        visualAchivementNew.transform.GetChild(0).GetComponent<Text>().text = "CHALLENGE " + (currentAchievment.Id + 1);
        visualAchivementNew.transform.GetChild(1).GetComponent<Text>().text = currentAchievment.Description;
        visualAchivementNew.transform.GetChild(2).GetComponent<Text>().text = "CHALLENGE";
    }*/

    /*public void AllAchievementsCompletedVisual()
    {
        visualAchivementNew.SetActive(true);
        visualAchivementNew.transform.GetChild(0).GetComponent<Text>().text = "CONGRATULATIONS!";
        visualAchivementNew.transform.GetChild(1).GetComponent<Text>().text = "YOU COMPLETED ALL CHALLENGES!";
        visualAchivementNew.transform.GetChild(2).GetComponent<Text>().text = "AWESOME!";
    }*/

   /* public IEnumerator HideAchivement()
    {
        yield return new WaitForSeconds(3);
        visualAchivementCompleted.gameObject.SetActive(false);
        if (currentAchievment == null)
        {
            StartCoroutine(ShowAllChallengesCompleted());
        }
        else
        {
            StartCoroutine(ShowAchivement());
        }
        
    }
    public IEnumerator ShowAchivement()
    {
        NewAchievmentVisual();
        yield return new WaitForSeconds(3);
        visualAchivementNew.SetActive(false);
        
    }

    public IEnumerator InitialAchivement()
    {
        InitialAchievmentVisual();
        yield return new WaitForSeconds(3);
        visualAchivementNew.SetActive(false);

    }
    public IEnumerator ShowAllChallengesCompleted()
    {
        AllAchievementsCompletedVisual();
        yield return new WaitForSeconds(3);
        visualAchivementNew.SetActive(false);
        
    }*/
}
