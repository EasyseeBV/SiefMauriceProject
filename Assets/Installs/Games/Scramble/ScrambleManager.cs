using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ScrambleManager : MonoBehaviour
{
    string CurrentWord;
    string ScrambledWord;
    public List<string> Words;
    List<string> _Words;
    public GameObject PlayTable;
    public GameObject LetterBox;
    public TextAsset dict;
    List<GameObject> AllLetterObjects = new List<GameObject>();
    public bool FindRandomWord;
    public int WordLength;
    public int AmountOfWords;
    public bool Reset;
    public bool EasyMode;
    List<int> LettersSelected = new List<int>();
    List<WordList> WordLists = new List<WordList>();
    float TotalScore;

    [Serializable]
    public class WordList
    {
        public List<string> AllWords = new List<string>();
    }

    
    public void Start()
    {
        string[] separators = new[] { "\r\n", "\t" };
        string[] wordArray = dict.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        foreach(string word in wordArray)
        {
            while (word.Length > WordLists.Count)
            {
                WordLists.Add(new WordList());
            }

            WordLists[word.Length - 1].AllWords.Add(word);
        }

        InitializeWords();
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        TotalScore -= Time.fixedDeltaTime;
        if (TotalScore < 10)
            TotalScore = 10;

        if (Reset)
        {
            Reset = false;
            InitializeWords();
        }
    }

    public void InitializeWords()
    {
        DestroyChilderen(PlayTable);
        if (FindRandomWord)
        {
            _Words = new List<string>();
            for (int i = 0; i < AmountOfWords; i++)
            {
                _Words.Add(WordLists[WordLength-1].AllWords[UnityEngine.Random.Range(0, WordLists[WordLength-1].AllWords.Count)].ToUpper());
            }
        }
        else
        {
            _Words = new List<string>();
            foreach (string word in Words)
                _Words.Add(word.ToUpper());
        }
        SpawnNewWord();
    }

    public void DestroyChilderen(GameObject Parent)
    {
        foreach (Transform child in Parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public string ScrambleWord (string Word)
    {
        List<char> s = Word.ToList();
        string ScrambledWord = "";
        while(s.Count != 0) {
            int Rand = UnityEngine.Random.Range(0, s.Count);
            ScrambledWord += s[Rand];
            s.RemoveAt(Rand);
        }
        return ScrambledWord;
    }

    public void SpawnNewWord ()
    {
        CurrentWord = _Words[UnityEngine.Random.Range(0, _Words.Count)].ToUpper();
        _Words.Remove(CurrentWord);
        int tries = 0;
        do
        {
            ScrambledWord = ScrambleWord(CurrentWord);
            tries++;
        } while (tries < 100 && ScrambledWord == CurrentWord);

        for (int i = 0; i < AllLetterObjects.Count; i++)
        {
            Destroy(AllLetterObjects[i]);
        }

        AllLetterObjects.Clear();
        LettersSelected.Clear();

        for (int i = 0; i < ScrambledWord.Length; i++)
        {
            AllLetterObjects.Add(Instantiate(LetterBox, PlayTable.transform));
            AllLetterObjects[i].transform.localPosition = new Vector3(-(CurrentWord.Length / 2 *0.5f) + (i * 0.5f), 0, 0);
            WordInfoComponent info = AllLetterObjects[i].GetComponent<WordInfoComponent>();
            info.Index = i;
            info.SetText(ScrambledWord.Substring(i,1));
        }
        if (EasyMode)
            CheckAllLetters();
    }

    public void AddSelectedWord(int WordNumber)
    {
        if (LettersSelected.Contains(WordNumber))
        {
            AllLetterObjects[WordNumber].GetComponent<WordInfoComponent>().UnSelect();
            LettersSelected.Remove(WordNumber);
        }
        else
        {
            AllLetterObjects[WordNumber].GetComponent<WordInfoComponent>().Select();
            LettersSelected.Add(WordNumber);
        }
        if (LettersSelected.Count == 2)
        {            
            StartCoroutine(WordsAnimator(LettersSelected[0], LettersSelected[1]));
            AllLetterObjects[LettersSelected[0]].GetComponent<WordInfoComponent>().Clickable = false;
            AllLetterObjects[LettersSelected[1]].GetComponent<WordInfoComponent>().Clickable = false;
            LettersSelected.Clear();
        }
    }

    IEnumerator WordsAnimator(int WordOne, int WordTwo)
    {
        WordInfoComponent LetterOne = AllLetterObjects[WordOne].GetComponent<WordInfoComponent>();
        WordInfoComponent LetterTwo = AllLetterObjects[WordTwo].GetComponent<WordInfoComponent>();

        float Percent = 0;
        Vector3 First = AllLetterObjects[WordOne].transform.localPosition, Second = AllLetterObjects[WordTwo].transform.localPosition;
        float Starttime = Time.realtimeSinceStartup;
        while (Percent < 1f)
        {
            Percent = Time.realtimeSinceStartup - Starttime * 2f;
            float SinusPercent = Percent >= 0.5f ? 1 - Percent : Percent;
            AllLetterObjects[WordOne].transform.localPosition = Vector3.Lerp(First, Second, Percent);
            AllLetterObjects[WordOne].transform.localPosition += new Vector3(0, 0, Mathf.Lerp(0, 1, SinusPercent));

            AllLetterObjects[WordTwo].transform.localPosition = Vector3.Lerp(Second, First, Percent);
            AllLetterObjects[WordTwo].transform.localPosition += new Vector3(0, 0, Mathf.Lerp(0, -1, SinusPercent));
            yield return new WaitForSecondsRealtime(0.01f);
        }

        string NewString = ScrambledWord;

        if (LetterOne.Index > LetterTwo.Index)
        {
            NewString =
         NewString.Substring(0, LetterTwo.Index) +
         LetterOne.Text +
         NewString.Substring(LetterTwo.Index+1, LetterOne.Index - (LetterTwo.Index+1)) +
         LetterTwo.Text +
         NewString.Substring(LetterOne.Index+1);
        }
        else
        {
            NewString =
        NewString.Substring(0, LetterOne.Index) +
        LetterTwo.Text +
        NewString.Substring(LetterOne.Index+1, LetterTwo.Index - (LetterOne.Index+1)) +
        LetterOne.Text +
        NewString.Substring(LetterTwo.Index+1);
        }
        AllLetterObjects[LetterOne.Index] = LetterTwo.gameObject;
        AllLetterObjects[LetterTwo.Index] = LetterOne.gameObject;
        int l = LetterOne.Index;
        LetterOne.Index = LetterTwo.Index;
        LetterTwo.Index = l;       

        ScrambledWord = NewString;
        if (EasyMode && CheckLetter(LetterOne.Index)) {
                AllLetterObjects[LetterOne.Index].GetComponent<WordInfoComponent>().LockObject();
        }
        else
        {
            LetterOne.UnSelect();
            LetterOne.Clickable = true;
        }
        if (EasyMode && CheckLetter(LetterTwo.Index))
        {
            AllLetterObjects[LetterTwo.Index].GetComponent<WordInfoComponent>().LockObject();
        }
        else
        {
            LetterTwo.UnSelect();
            LetterTwo.Clickable = true;
        }
        

        if (ScrambledWord == CurrentWord)
            GoToNextWord();

        yield return null;
    }

    public void CheckAllLetters()
    {
        for (int i = 0; i < ScrambledWord.Length; i++)
        {
            if (CurrentWord.Substring(i,1) == ScrambledWord.Substring(i, 1)) {
                AllLetterObjects[i].GetComponent<WordInfoComponent>().LockObject();
                AllLetterObjects[i].GetComponent<WordInfoComponent>().Clickable = false;
            }
        }
    }
    public bool CheckLetter(int LetterNumber)
    {
        if (CurrentWord.Substring(LetterNumber, 1) == ScrambledWord.Substring(LetterNumber, 1))         
            return true;
        return false;
    }

    public void GoToNextWord()
    {
        CheckAllLetters();
        if (_Words.Count == 0)
        {
            Invoke("LevelFinished", 5f);
            //Content.GameFinished(TotalScore);
            //Content.GameFinished(TotalScore, true);
            return;
        }
        else
        {
            Invoke("SpawnNewWord", 2f);
        }       
    }

    public void LevelFinished()
    {
        GameManager.Instance.LevelFinished();
    }
}
