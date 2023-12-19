using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{

    public GameObject LinkedObject;
    public List<VideoPlayer> allVideoPlayers = new List<VideoPlayer>();
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class Level {
        public GameObject LevelObject;
        public string TextToDisplay;
    }

    public List<Level> Levels = new List<Level>();

    public int CurrentLevel;

    public TextMeshProUGUI InfoUI;

    public List<string> AlmostCodes;
    public string Answer;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        CurrentLevel = PlayerPrefs.GetInt("CurrentLevel");
    }

    IEnumerator PlayClosestVideos()
    {
        while (true)
        {
            if (CurrentLevel == 0)
            {
                Transform cameraTransform = Camera.main.transform;

                allVideoPlayers.Sort((vp1, vp2) =>
                    Vector3.Distance(vp1.transform.position, cameraTransform.position).CompareTo(
                    Vector3.Distance(vp2.transform.position, cameraTransform.position))
                );

                int MaxVideos = 0;
                for (int i = 0; i < allVideoPlayers.Count; i++)
                {
                    if (MaxVideos < 9 && Vector3.Distance(allVideoPlayers[i].transform.position, cameraTransform.position) < 15f)
                    {
                        allVideoPlayers[i].Play();
                        MaxVideos++;
                    }
                    else
                    {
                        allVideoPlayers[i].Pause();
                    }
                }
            }
            yield return new WaitForSecondsRealtime (0.5f);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayClosestVideos());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetLevels();
        }if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (CurrentLevel < Levels.Count)
                LevelFinished();
        }
        if (LinkedObject != null && transform.parent != LinkedObject.transform)
        {
            GetComponent<AudioSource>().Play();
            transform.parent = LinkedObject.transform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = new Vector3(90, 0, 0);
            LoadLevel(PlayerPrefs.GetInt("CurrentLevel"));
        }       
    }

    public void ResetLevels()
    {
        if (CurrentLevel <= Levels.Count - 1)
        {
            Levels[CurrentLevel].LevelObject?.SetActive(false);
        }
        
        CurrentLevel = 0;
        if (Levels.Count > 0)
        {
            LoadLevel(CurrentLevel);
        }
        
    }

    public void LevelFinished()
    {
        if (CurrentLevel > Levels.Count - 1)
        {
            Debug.LogWarning("Level error, there is a level lost");
            return;
        }
        Levels[CurrentLevel].LevelObject?.SetActive(false);
        CurrentLevel++;
        LoadLevel(CurrentLevel);
    }
    public void LoadLevel(int Level)
    {
        if (Level > Levels.Count - 1)
        {
            Debug.LogWarning("Level error, there is a level lost");
            return;
        }
        Levels[Level].LevelObject?.SetActive(true);
        InfoUI.text = Levels[Level].TextToDisplay;
        PlayerPrefs.SetInt("CurrentLevel", Level);
    }

    public void GuessedCode()
    {
        foreach (var item in Levels)
        {
            item.LevelObject?.SetActive(false);
        }
        Levels[0].LevelObject?.SetActive(true);
        InfoUI.text = "Goed gespeeld! Stuur Jordi een bericht met de code en een datum (een vrij t/m Ma) \n Ps. Heb je al op iedereens gezichten geklikt?";
    }

    public void CheckCode(string code)
    {
        if (string.Equals(code, Answer, StringComparison.OrdinalIgnoreCase))
            GuessedCode();
        else if (AlmostCodes.Any(s => string.Equals(code, s, StringComparison.OrdinalIgnoreCase)))
        {
            StartCoroutine(ShortTextChange("Bijna", 3f));
        }
        else
        {
            StartCoroutine(ShortTextChange("Fout", 3f));
        }
        
    }

    public IEnumerator ShortTextChange(string info, float time)
    {
        string OldText = InfoUI.text;
        InfoUI.text = info;
        yield return new WaitForSeconds(time);
        if (InfoUI.text == info)
        {
            InfoUI.text = OldText;
        }
    }
}
