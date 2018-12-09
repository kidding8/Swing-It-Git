using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievement {
    public const int ACHIEVMENT_SCORE = 1;
    public const int ACHIEVMENT_COINS = 2;
    public const int ACHIEVMENT_POWERUPS = 3;
    public const int ACHIEVMENT_GAMES = 4;
    public const int ACHIEVMENT_COINS_MAGNET = 5;
    public const int ACHIEVMENT_ENEMIES_INVINCIBLE = 6;
    public const int ACHIEVMENT_DESTROY_SHIELD = 7;
    public const int ACHIEVMENT_TUTORIAL = 7;
    private int id;
    private string description;
    private int pointsToComplete;
    private int actualpoints;
    private bool isCompleted;
    private int reward;
    private int typeOfAchievement;
    private bool save;
    //private GameObject achievementRef;

    public Achievement (int id, string description, int pointsToComplete, int reward, int typeOfAchievment, bool save)
    {
        this.Id = id;
        this.Description = description;
        this.pointsToComplete = pointsToComplete;
        this.reward = reward;
        this.isCompleted = false;
        this.save = save;
        this.TypeOfAchievement = typeOfAchievment;
        //this.achievementRef = achievementRef;
        if(AchievementManager.instance.GetActiveAchievement() == id)
            GetAchievmentProgress();
    }

    #region GetsAndSets
    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }

        set
        {
            description = value;
        }
    }

    public int Points
    {
        get
        {
            return actualpoints;
        }

        set
        {
            actualpoints = value;
        }
    }

    public int PointsToComplete
    {
        get
        {
            return pointsToComplete;
        }

        set
        {
            pointsToComplete = value;
        }
    }

    public int Reward
    {
        get
        {
            return reward;
        }

        set
        {
            reward = value;
        }
    }

    public int TypeOfAchievement
    {
        get
        {
            return typeOfAchievement;
        }

        set
        {
            typeOfAchievement = value;
        }
    }
    #endregion

    public bool EarnAchievement(int points)
    {
        actualpoints += points;
        if (!isCompleted && actualpoints >= pointsToComplete)
        {
            isCompleted = true;
            ResetPoints();
            return true;
        }
        else
        {
            SaveAchievmentProgress();
        }
        return false;
    }

    public void GetAchievmentProgress()
    {
        if(save)
            actualpoints = PlayerPrefs.GetInt("Points", 0);
    }

    public void SaveAchievmentProgress() {
        if (save)
            PlayerPrefs.SetInt("Points", actualpoints);
    }
    public void ResetPoints()
    {
        actualpoints = 0;
        PlayerPrefs.SetInt("Points", 0);
    }
}
