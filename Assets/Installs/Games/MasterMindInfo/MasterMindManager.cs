using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MasterMindManager : MonoBehaviour
{
    public int AmountOfCoins;
    public int AmountOfColors;
    public int AmountOfTries;
    public float ScoreMultiplier;

    public List<Color> AvailableColors;
    public Solution MasterCode = new Solution();
    public List<Solution> Solutions = new List<Solution>();

    public GameObject PlayTable;
    public GameObject PrefabCoin;
    public GameObject PrefabTable;
    public GameObject PrefabDot;
    public GameObject ButtonPrefab;
    public GameObject UIParent;
    public bool StartTheGame;

    bool GameIsBusy;
    int CurrentSolutionTry;
    int CurrentSolutionCoin;
    float PointNorm;

    public class Solution
    {
        public List<GameObject> CoinObject = new List<GameObject>();
        public List<int> ColorNumber = new List<int>();
    }

    private void OnEnable()
    {     
        StartGame();
    }

    public void StartGame()
    {
        GameIsBusy = true;

        // Destroys and reset values
        DestroyChilderen(PlayTable);
        CurrentSolutionTry = 0;
        CurrentSolutionCoin = 0;
        PointNorm = 1;
        Solutions.Clear();

        // Spawns the playtable
        GameObject Go = Instantiate(PrefabTable, PlayTable.transform);
        float thick = 0.2f;
        Go.transform.localScale = new Vector3(AmountOfCoins, thick, AmountOfTries + 1);
        Go.transform.localPosition = new Vector3(0, thick * 0.5f * 0.4f, 0);
        //PlayTable.transform.localPosition = new Vector3(0, 0.1f, 0);

        /*Go = Instantiate(PrefabTable, PlayTable.transform);
        Go.transform.localScale = new Vector3(AmountOfCoins, 0.1f, 1);
        Go.transform.localPosition = new Vector3(0,0.4f, -AmountOfTries/2f - 1f);
        Go.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);*/

        // Spawns the mastercode and enables the first try
        MasterCode = DrawMasterCode(PrefabCoin, PlayTable, 1f, -(AmountOfTries / 2f + 1f));
        Solutions.Add(new Solution());

        // Spawns the UI buttons;
        SpawnUIButtons();
    }

    public void SpawnUIButtons()
    {
        DestroyChilderen(UIParent);
        for (int i = 0; i < AmountOfColors; i++)
        {
            GameObject Button = Instantiate(ButtonPrefab, UIParent.transform);
            Button.GetComponent<Image>().color = AvailableColors[i];
            int j = i;
            Button.GetComponent<Button>().onClick.AddListener(() => { AddCoin(j); });
        }
    }

    public void AddCoin(int colorNumber)
    {
        // place and position the physical coin
        GameObject go = Instantiate(PrefabCoin, PlayTable.transform);
        go.transform.localPosition = new Vector3(-(AmountOfCoins / 2f - 0.5f) + CurrentSolutionCoin, 0.25f, (AmountOfTries / 2f) - CurrentSolutionTry);
        go.GetComponent<Renderer>().material.SetColor("_Color", AvailableColors[colorNumber]);

        // Store values
        Solutions[CurrentSolutionTry].CoinObject.Add(go);
        Solutions[CurrentSolutionTry].ColorNumber.Add(colorNumber);

        // Sets the locations number of the next coin
        CurrentSolutionCoin++;
        if (CurrentSolutionCoin >= AmountOfCoins)
        {
            CurrentSolutionCoin = 0;
            if (!ShowResults())
            {
                CurrentSolutionTry += 1;
                PointNorm = (AmountOfTries - CurrentSolutionTry) / 100f * AmountOfTries;
                Solutions.Add(new Solution());
            }
            else
            {
                Invoke("LevelFinished", 5f);
                print("WIN!");
                GameIsBusy = false;
            }
        }
        if (CurrentSolutionTry >= AmountOfTries)
        {
            StartGame();
            print("LOSE");
        }
    }

public void LevelFinished()
{
    GameManager.Instance.LevelFinished();
}

public void RemoveCoin()
    {
        if (CurrentSolutionCoin == 0)
            return;
        CurrentSolutionCoin--;
        Destroy(Solutions[CurrentSolutionTry].CoinObject[CurrentSolutionCoin]);
        Solutions[CurrentSolutionTry].CoinObject.RemoveAt(CurrentSolutionCoin);
        Solutions[CurrentSolutionTry].ColorNumber.RemoveAt(CurrentSolutionCoin);

    }

    public bool ShowResults()
    {
        Solution s = Solutions[CurrentSolutionTry];
        List<int> TestColors = new List<int>(s.ColorNumber);
        List<int> AllColors = new List<int>(MasterCode.ColorNumber);
        int WhitePins = 0;
        int BlackPins = 0;
        for (int i = 0; i < AmountOfCoins; i++)
        {
            if (s.ColorNumber[i] == MasterCode.ColorNumber[i])
            {
                GameObject go = Instantiate(PrefabDot, PlayTable.transform);
                go.transform.localPosition = new Vector3(AmountOfCoins / 2f + 1 + (BlackPins * 0.3f), 0, (AmountOfTries / 2f) - CurrentSolutionTry + 0.2f);
                go.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                BlackPins++;
                TestColors.Remove(s.ColorNumber[i]);
                AllColors.Remove(s.ColorNumber[i]);
            }
        }
        if (TestColors.Count == 0)
        {
            return true;
        }
        for (int i = 0; i < TestColors.Count; i++)
        {
            if (AllColors.Exists(x => x == TestColors[i]))
            {
                GameObject go = Instantiate(PrefabDot, PlayTable.transform);
                go.transform.localPosition = new Vector3(AmountOfCoins / 2f + 1 + (WhitePins * 0.3f), 0, (AmountOfTries / 2f) - CurrentSolutionTry - 0.2f);
                WhitePins++;
                AllColors.Remove(TestColors[i]);
            }
        }

        return false;
    }

    public void DestroyChilderen(GameObject Parent)
    {
        foreach (Transform child in Parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public Solution DrawMasterCode(GameObject SpawnObject, GameObject Parent, float XDistance, float ZStart)
    {
        float Pos = -(AmountOfCoins / 2f - 0.5f);
        Solution s = new Solution();
        for (int i = 0; i < AmountOfCoins; i++)
        {
            int color = Random.Range(0, AmountOfColors);
            //GameObject Dot = (Instantiate(SpawnObject, Parent.transform));
            //Dot.transform.localPosition = new Vector3(Pos, 0, ZStart);
            //Pos += XDistance;
            //Dot.GetComponent<Renderer>().material.SetColor("_Color", AvailableColors[color]);
            //s.CoinObject.Add(Dot);
            s.ColorNumber.Add(color);
        }
        return s;
    }

    // Update is called once per frame
    void Update()
    {

        if (StartTheGame)
        {
            StartTheGame = false;
            StartGame();
        }
    }
}
