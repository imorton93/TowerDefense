using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum gameStatus{
    next, play, gameover, win
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private Text totalMoneyLbl;
    [SerializeField] private Text currentWaveLbl;
    [SerializeField] private Text totalEscapedLabel;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private int totalEnemies = 3;
    [SerializeField] private int enemiesPerSpawn;

    [SerializeField] private Text playButtonLabel;

    [SerializeField] private Button playButton;

    private int waveNumber = 0;
    private int totalMoney = 10;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    private int whichEnemiesToSpawn = 0;
    private gameStatus currentState = gameStatus.play;

    private AudioSource audioSource;

    public List<Enemy> EnemyList = new List<Enemy>();

    const float spawnDelay = 0.5f;

    public Transform[] waypoints;

    public int TotalMoney{
        get{
            return totalMoney;
        }
        set{
            totalMoney = value;
            totalMoneyLbl.text = totalMoney.ToString();
        }
    }

    public AudioSource AudioSource {
        get{
            return audioSource;
        }
    }

    public int TotalEscaped{
        get{
            return totalEscaped;
        }
        set{
            totalEscaped = value;
        }
    }

    public int RoundEscaped{
        get{
            return roundEscaped;
        }
        set{
            roundEscaped = value;
        }
    }

    public int TotalKilled{
        get{
            return totalKilled; 
        }
        set{
            totalKilled = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        playButton.gameObject.SetActive(true);
        audioSource = GetComponent<AudioSource>();
        showMenu();
    }

    void Update() {
        handleEscape();
    }



    IEnumerator spawn()
    {
        if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies)
        {
            for(int i = 0; i < enemiesPerSpawn; i++)
            {
                if(EnemyList.Count < totalEnemies)
                {
                    GameObject newEnemy = Instantiate(enemies[0]) as GameObject;
                    newEnemy.transform.position = spawnPoint.transform.position;
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(spawn());
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        EnemyList.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DestroyAllEnemies()
    {
        foreach(Enemy enemy in EnemyList)
        {
            Destroy(enemy.gameObject);
        }

        EnemyList.Clear();
    }


    public void addMoney(int amount){
        TotalMoney += amount;
    }

    public void subtractMoney(int amount){
        TotalMoney -= amount;
    }

    public void isWaveOver(){
        totalEscapedLabel.text = "Escaped " + TotalEscaped + "/10";
        if((RoundEscaped + TotalKilled) == totalEnemies){
            setCurrentGameState();
            showMenu();        
        }
    }

    public void setCurrentGameState(){
        if(TotalEscaped >= 10){
            currentState = gameStatus.gameover;
        } else if(waveNumber == 0 && (TotalKilled + RoundEscaped) == 0){
            currentState = gameStatus.play;
        } else if(waveNumber >= totalWaves){
            currentState = gameStatus.win;
        } else{
            currentState = gameStatus.next;
        }
    }
    

    public void showMenu(){
        switch(currentState){
            case gameStatus.gameover:
                playButtonLabel.text = "Play Again!";
                //add gameover sounds later
                AudioSource.PlayOneShot(SoundManager.Instance.GameOver);
                break;
            case gameStatus.next:
                playButtonLabel.text = "Next Wave";
                break;
            case gameStatus.play:
                playButtonLabel.text = "Play";
                break;
            case gameStatus.win:
                playButtonLabel.text = "Play";
                break;
        }
        playButton.gameObject.SetActive(true);
    }

    public void playButtonPressed(){
        switch(currentState){
            case gameStatus.next:
                waveNumber += 1;
                totalEnemies += waveNumber;

                break;
            default:
                totalEnemies = 3;
                TotalEscaped = 0;
                TotalMoney = 10;
                TowerManager.Instance.DestroyAllTowers();
                TowerManager.Instance.RenameTagsBuildSites();
                totalMoneyLbl.text = TotalMoney.ToString();
                totalEscapedLabel.text = "Escaped " + TotalEscaped + "/10";
                audioSource.PlayOneShot(SoundManager.Instance.NewGame);
                break;
        }
        DestroyAllEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        currentWaveLbl.text = "Wave " + (waveNumber + 1);
        StartCoroutine(spawn());
        playButton.gameObject.SetActive(false);
    }

    private void handleEscape(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            TowerManager.Instance.disableDragSprite();
            TowerManager.Instance.towerBtnPressed = null;
        }
    }
}
