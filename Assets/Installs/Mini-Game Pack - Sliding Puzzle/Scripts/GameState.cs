using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{

    public Text txtTime;        // Reference to the UI text for time.
    public Text txtMoves;       // Reference to the UI text for moves.
    public Button btnRestart;   // Reference to the UI button for restart.

    Coroutine timer = null;     // Reference to the timer coroutine.
    private int time = 0;       // Used to record game time, the unit for time variable is seconds.
    private int moves = 0;      // Used to record clicks and moves for puzzle boxes.

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        // Init UI texts if valid.
        if (txtTime != null)
            txtTime.text = "Time: " + time + "s";
        if (txtMoves != null)
            txtMoves.text = "Moves: " + moves;
        // Init UI button if valid.
        if (btnRestart != null)
            btnRestart.onClick.AddListener(delegate ()
            {
                GetComponent<Puzzle>().Restart();
            });
    }

    IEnumerator Countdown()
    {
        // Wait for one second.
        yield return new WaitForSeconds(1);
        // Add one second to the time variable.
        IncreaseTime();
        // Restart countdown event.
        StopCountdown();
        StartCountdown();
    }

    public void StartCountdown()
    {
        // Start timer and count for one second.
        timer = StartCoroutine(Countdown());
    }

    public void StopCountdown()
    {
        // Clear timer.
        StopCoroutine(timer);
    }

    public void IncreaseTime()
    {
        // Add one second to the variable.
        time++;
        // Update the UI text if valid.
        if (txtTime != null)
            txtTime.text = "Time: " + time + "s";
    }

    public void ClearTime()
    {
        // Set the time variable back to 0s.
        time = 0;
    }

    public int GetTime()
    {
        return time;
    }

    public void IncreaseMoves()
    {
        // Add one step to the variable.
        moves++;
        // Update the UI text if valid.
        if (txtMoves != null)
            txtMoves.text = "Moves: " + moves;
    }

    public void ClearMoves()
    {
        // Set the moves variable back to 0.
        moves = 0;
    }

    public int GetMoves()
    {
        return moves;
    }

}
