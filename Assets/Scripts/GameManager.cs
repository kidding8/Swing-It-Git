using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static int MENU = 0;
    private static int STOPPED = 1;
    private static int PLAYING = 2;
    //private static int RESTART = 3;
    //private static bool isTutorial = true;
    public static int gameState = 0;
    public static GameManager instance;

    
    [Space(10)]
    [Header("Public Variables")]
    public TextMeshProUGUI textScore;
    //public Slider pointsSlider;

    [Space(10)]
    [Header("Public Objects")]
    public ObjectPooler heartPool;
    public GameObject heartPanel;
    
    public int maxLifes = 3;
    private GameObject player;

    [Space(10)]
    [Header("Main Menu")]
    public GameObject mainMenu;
    public Text bestScoreMainMenuText;
    

    [Space(10)]
    [Header("Death Menu")]
    public GameObject deathMenu;
    public Text deathMenuScoreText;
    public Text deathMenuBestScoreText;

    [Space(10)]
    [Header("Continue Menu")]
    public GameObject continueMenu;

    [Space(10)]
    [Header("Mission Menu")]
    public GameObject missionIngameMenu;

    [Space(10)]
    [Header("Combo")]
    public TextMeshProUGUI comboCountText;
    public TextMeshProUGUI comboScoreText;
    public Slider comboTimerSlider;
    private int comboCount = 0;
    private int comboScore = 0;
    public float comboTimeToDie = 2f;
    private float comboTimer;
    Coroutine comboCoroutine;


    int coins = 0;
    int coinsGained = 0;
    int score = 0;
    int lifes = 3;

    //Coroutine myCoroutine;
    private SpawnHookManager SHM;
    private AuxManager AM;
    private PlayerManager PM;
   

    public bool invincible = false;
    public bool destroyRope = true;
    private float invinsibleTime = 1.5f;

    private int distance = 0;

    private List<GameObject> heartList;


    private void Start()
    {
        SHM = SpawnHookManager.instance;
        AM = AuxManager.instance;
        PM = PlayerManager.instance;
        player = AM.GetPlayer();
        transform.position = player.transform.position;
        ShowMainMenu();
        //bestScoreMainMenuText.text = "Best Score: " + GetBestScore();
        heartList = new List<GameObject>();
        
    }

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

    }

    private void Update()
    {
        int currentdistance = (int)( player.transform.position.x - transform.position.x);
        if(currentdistance > distance && distance >= 0 )
        {
            distance = currentdistance;
            textScore.text = distance.ToString();
        }

       // comboTimer -= Time.deltaTime;
        
    }

    public void CheckIfCombo()
    {
        comboCount++;
        UpdateComboText();
        /*
        if(comboCount <= 1)
        {
            comboCount++;
            //comboTimer = comboTimeToDie;
            UpdateComboText();
        }
        else
        {
            comboCount = 1;
            UpdateComboText();
        }*/
    }

    public void AddCombo(int amount)
    {
        comboCount++;
        comboScore += amount * comboCount;
        comboScoreText.text = comboScore.ToString();
        //comboTimer = comboTimeToDie;
        UpdateComboText();
    }

    public void RemoveCombo()
    {
        comboCount = 0;
        UpdateComboText();
        if (comboCoroutine != null)
            StopCoroutine(comboCoroutine);
        comboTimerSlider.gameObject.SetActive(false);
    }


    private void UpdateComboText()
    {
        //Debug.Log("COMBO " + combo);
        if(comboCount >= 2)
        {
            comboCountText.gameObject.SetActive(true);
            comboCountText.text = comboCount + "x";
            if(comboCoroutine != null)
            StopCoroutine(comboCoroutine);
            comboCoroutine = StartCoroutine(ComboSliderOverTime(comboTimeToDie));
        }
        else
        {
            comboCountText.gameObject.SetActive(false);
            comboTimerSlider.gameObject.SetActive(false);
        }
            
            

    }

    private void PopulateHearts()
    {
        for (int i = 0; i < maxLifes; i++)
        {
            AddHeartImage();
        }
        lifes = maxLifes;
    }

    private void AddHeartImage()
    {
        GameObject life = heartPool.GetPooledObject();
        life.SetActive(true);
        life.transform.SetParent(heartPanel.transform, false);
        heartList.Add(life);
    }

    private void RemoveHeartImage()
    {
        //int total = heartPanel.transform.childCount;
        GameObject obj = heartList[heartList.Count - 1];
        obj.gameObject.SetActive(false);
        heartList.Remove(obj);
    }



    public void AddLife()
    {
        if(lifes+1 <= maxLifes)
        {
            lifes++;
            Debug.Log("Added life: " + lifes);
            AddHeartImage();
        }
    }


    public void RemoveLife()
    {
        if (PM.invincible)
        {
           
            return;
        }

        if (lifes <= 1)
        {
            Debug.Log("DEAD");
            OnDeath();
        }
        else
        {
            
            lifes--;
            Debug.Log("Removed life: " + lifes);
            RemoveHeartImage();
            StartCoroutine(InvicibleTimer(invinsibleTime));
        }
    }

    public void StartNormalGame()
    {
        gameState = PLAYING;
        PM.SetPlayerState(States.STATE_NORMAL);
        Time.timeScale = 1f;
        SHM.StartGame();
        ShowPlayer();
        PopulateHearts();
        HideMainMenu();
        UpdateComboText();
    }

    public void StartRopeGame()
    {
        StartNormalGame();
       /* PM.playerPower = Power.POWER_ROPE;
        PM.playerHability = Hability.HABILITY_JUMP;*/
    }

    /*public void StartSpringGame()
    {
        StartNormalGame();
        PM.playerPower = Power.POWER_SPRING;
    }
    public void StartGrappleGame()
    {
        StartNormalGame();
        PM.playerPower = Power.POWER_GRAPPLE;
    }
    public void StartTeleporterGame()
    {
        StartNormalGame();
        PM.playerPower = Power.POWER_TELEPORT;
    }
    public void StartMagnetGame()
    {
        StartNormalGame();
        PM.playerPower = Power.POWER_MAGNET;
    }

    public void StartDashingGame()
    {
        StartNormalGame();
        PM.playerPower = Power.POWER_ROPE;
        PM.playerHability = Hability.HABILITY_DASH;
    }*/

    private void HidePlayer()
    {
        player.SetActive(false);
        textScore.gameObject.SetActive(false);
        comboScoreText.gameObject.SetActive(false);
        //pointsSlider.gameObject.SetActive(false);
    }

    private void ShowPlayer()
    {
        player.SetActive(true);
        textScore.gameObject.SetActive(true);
        comboScoreText.gameObject.SetActive(true);
        //pointsSlider.gameObject.SetActive(true);
    }

    private void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        HidePlayer();
        gameState = MENU;
        Time.timeScale = 0.0f;

        int bestScore = GetBestScore();
        if (bestScore != 0)
        {
            bestScoreMainMenuText.text = "Best: " + bestScore;
        }
        else
        {
            bestScoreMainMenuText.gameObject.SetActive(false);
        }
    }

    private void ShowMissionMenuIngame()
    {
        missionIngameMenu.SetActive(true);
    }

    private void HideMissionMenuIngame()
    {
        missionIngameMenu.SetActive(false);
    }

    private void HideMainMenu()
    {
        mainMenu.SetActive(false);
    }

    private void SetNewHighScore(int newScore)
    {
        PlayerPrefs.SetInt("BestScore", newScore);
    }

    private void ShowDeathMenu()
    {
        gameState = MENU;
        Time.timeScale = 0f;
        deathMenu.SetActive(true);
        int bestScore = GetBestScore();
        deathMenuBestScoreText.text = "Best Score: " + bestScore;
        deathMenuScoreText.text = "Score: " + GetScore();
        
    }

    private void HideDeathMenu()
    {
        deathMenu.SetActive(false);
    }

    private void ShowContinueMenu()
    {
        gameState = MENU;
        continueMenu.SetActive(true);
    }

    private void HideContinueMenu()
    {
        continueMenu.SetActive(false);
    }

    public void OnDeath()
    {
        /*int bestScore = GetBestScore();
        if (score > bestScore)
        {
            SetNewHighScore(score);
        }
        ShowDeathMenu();*/
        /*if (invincible)
        {
            Debug.Log("Invicible noob");
            return;
        }*/
        gameState = STOPPED;
        ShowContinueMenu();
        Time.timeScale = 0f;
    }


    public void OnContinue()
    {
        Time.timeScale = 1f;
        /*HideMainMenu();
        HideDeathMenu();*/
        StartCoroutine(InvicibleTimer(invinsibleTime));
        gameState = PLAYING;
        HideContinueMenu();
        PM.OnContinue();
    }

    int GetBestScore()
    {
        return PlayerPrefs.GetInt("BestScore", 0);
    }

    public bool isPlaying()
    {
        return gameState == PLAYING;
    }

    public int GetScore()
    {
        return score;
    }


    public int GetGameState()
    {
        return gameState;
    }

    private IEnumerator InvicibleTimer(float waitTime)
    {
        invincible = true;
        yield return new WaitForSeconds(waitTime);
        invincible = false;
    }

    private IEnumerator WaitToDie(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        OnDeath();
    }

    IEnumerator ComboSliderOverTime(float seconds)
    {
        comboTimerSlider.gameObject.SetActive(true);
        float animationTime = 0f;
        while (animationTime < seconds)
        {
            animationTime += Time.deltaTime;
            float lerpValue = animationTime / seconds;
            comboTimerSlider.value = Mathf.Lerp(100f, 0f, lerpValue);
            yield return null;
        }

        RemoveCombo();
        
        //comboCount = 0;
        //DeathMenu();

    }

    public void Reset()
    {
        //gameState = 2;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    /* public void ClearScreen()
     {
         Transform[] allChildren = spawwnedObjts.GetComponentsInChildren<Transform>();
         foreach (Transform child in allChildren)
         {
             if (child.gameObject.activeInHierarchy)
             {
                 SimpleEnemyAI enemyAI = child.GetComponent<SimpleEnemyAI>();
                 if (enemyAI != null)
                     enemyAI.Death();
             }
         }
     }*/

   
    public void IncrementScore(int newScore, Transform pos)
    {
        score += newScore;
        textScore.text = score.ToString();
        //GenerateText(newScore, pos);
    }

    public void IncrementCoins(int coin)
    {
        coinsGained += coin;
        coins += coin;
    }

    /* public int incrementCoins()
     {
         int newcoins = Random.Range((int)coinsMax.x, (int)coinsMax.y);
         coins += newcoins;
         coinsGained += newcoins;
         PlayerPrefs.SetInt("coins", coins);
         return newcoins;
     }

     public bool removeCoins(int numCoins)
     {
         if (coins - numCoins <= 0)
             return false;
         coins -= numCoins;
         PlayerPrefs.SetInt("coins", coins);
         return true;
     }*/


    /* IEnumerator AnimateSliderOverTime(float seconds)
     {
         mainSlider.gameObject.SetActive(true);
         float animationTime = 0f;
         while (animationTime < seconds)
         {
             animationTime += Time.deltaTime;
             float lerpValue = animationTime / seconds;
             mainSlider.value = Mathf.Lerp(100f, 0f, lerpValue);
             yield return null;
         }

         OnDeath();
         //DeathMenu();

     }*/

    /*public void StopCoroutineTime()
    {
        StopCoroutine(myCoroutine);
    }*/

   


}
