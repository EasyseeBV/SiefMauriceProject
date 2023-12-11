using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{

    private PuzzleBox[,] boxes = new PuzzleBox[1005, 1005];     // Array for all puzzle boxes.
    private PuzzleBox empty;                                    // Reference to the empty puzzle box.
    private Transform container;                                // Reference to the child GameObject named container to hold all puzzle boxes.
    private GameState scriptGameState;                          // Reference to the GameState.cs script.
    private AudioSource audioSource;                            // Reference to the AudioSource component.
    public PuzzleBox prefab;                                    // Prefab for puzzle boxes.
    public int size;                                            // The size of the puzzle.
    public Material material;                                   // Material for reference image plane.
    public Sprite[] sprites;                                    // Array of sprites for puzzle boxes.
    public float delay = 1.0f;                                  // Delay time after puzzle finished to show the last one.
    public AudioClip audioClip;                                 // Sound that will be played when any puzzle box is clicked.

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        // Get reference to AudioSource component and apply AudioClip to it.
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = audioClip;

        // Get reference to GameState.cs script.
        scriptGameState = GetComponent<GameState>();

        // Get container from child list.
        container = this.transform.Find("Container");

        // Set material for reference plane.
        this.GetComponentInChildren<MeshRenderer>().material = material;

        // Call function to spawn all puzzle boxes and init them.
        Generate();

        // Get the reference to the empty puzzle box.
        empty = boxes[size - 1, size - 1];

        // Center all puzzle boxes by modifying the local position of the container.
        float offsets = (size - 1) / 2 + ((size - 1) % 2) * 0.5f;
        this.container.localPosition = new Vector2(-offsets, offsets);

        // Call function to shuffle all puzzle boxes randomly.
        Shuffle();

        // Enable smooth movement for all puzzle boxes.
        SetSmoothMovement(true);

        // Activate countdown timer in GameState.cs script.
        scriptGameState.StartCountdown();

        // Print debug information.
        Debug.LogWarning("Game Start");

        transform.localEulerAngles = Vector3.zero;
        transform.localScale = new Vector3(20.43f, 20.43f, 20.43f);
        gameObject.SetActive(false);
    }

    public void Generate()
    {
        // Spawn all puzzle boxes, init them and add them to the boxes array.
        for (int row = 0; row < size; row++)
            for (int col = 0; col < size; col++)
            {
                PuzzleBox box = Instantiate(prefab, new Vector2(row, col), Quaternion.identity);    // Spawn a puzzle box.
                int index = row * size + col;                                                       // Convert 2D coordinate to 1D index.
                box.Init(index, row, col, sprites[index], HandleClick, audioClip);                  // Call Init() function in PuzzleBox.cs.
                boxes[row, col] = box;                                                              // Add this puzzle box to the array.
                box.transform.parent = this.container;                                              // Move the puzzle box under container.
                box.transform.localEulerAngles = Vector3.zero;
            }

        // Remove the sprite of the last puzzle box with coord (size-1, size-1).
        boxes[size - 1, size - 1].GetComponent<SpriteRenderer>().sprite = null;
    }

    public void Shuffle()
    {
        /* METHOD 1

        // Shuffle the boxes in the array.
        Swap(empty.row, empty.column, size - 1, size - 1);
        int n = size * size + (size * size) % 2;
        for (int i = 0; i < n; i++)
        {
            // Make a random coordinate and make sure it won't be (0,0) or (size-1,size-1).
            int randomRow = UnityEngine.Random.Range(0, size);                                                              // It can be any row from 0 to size-1.
            int randomCol = UnityEngine.Random.Range((randomRow == 0 ? 1 : 0), (randomRow == size - 1 ? size - 1 : size));  // Should not get (0,0) or (size-1,size-1), [1,size) if row==0, [0,size-1) if row==size-1, or it will be [0,size).
            // Swap two boxes (randomRow, randomCol) and (0,0).
            Swap(randomRow, randomCol, 0, 0);
        }

        */

        /* METHOD 2 */

        // Shuffle the boxes in the array.
        int[,] dir = { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };      // Coordinate offsets for 4-direction search.
        int n = size * size * 4;                            // Times for shuffle random move.
        // Move the empty puzzle box around randomly.
        for (int i = 0; i < n; i++)
        {
            // Find all nearby puzzle boxes and add them to the nearby array.
            List<PuzzleBox> nearby = new List<PuzzleBox>();
            for (int d = 0; d < 4; d++)
                if (InBound(empty.row + dir[d, 0], empty.column + dir[d, 1]))
                    // Add this puzzle box to the nearby array if it is in bound.
                    nearby.Add(boxes[empty.row + dir[d, 0], empty.column + dir[d, 1]]);
            // Pick a random puzzle box from the nearby array.
            PuzzleBox random = nearby[UnityEngine.Random.Range(0, nearby.Count)];
            // Swap these two puzzle boxes.
            Swap(random.row, random.column, empty.row, empty.column);
        }
        // Move it back to the lower right corner.
        for (int r = empty.row; r < size - 1; r++)      // Downward first.
            Swap(empty.row + 1, empty.column, empty.row, empty.column);
        for (int c = empty.column; c < size - 1; c++)   // Rightward then.
            Swap(empty.row, empty.column + 1, empty.row, empty.column);
    }

    public void SetSmoothMovement(bool enabled)
    {
        // Toggle smooth movement for all puzzle boxes.
        for (int row = 0; row < size; row++)
            for (int col = 0; col < size; col++)
                boxes[row, col].smooth = enabled;
    }

    public void SetClickDelegate(Action<int, int> function)
    {
        for (int row = 0; row < size; row++)
            for (int col = 0; col < size; col++)
                boxes[row, col].click = function;
    }

    public bool InBound(int row, int col)
    {
        // We consider the puzzle box is in bound if and only if the row and column of its coordinate do not exceed the range of [0,size).
        return 0 <= row && row < size && 0 <= col && col < size;
    }

    public bool IsEmpty(int row, int col)
    {
        // We consider the index of the empty box is the last one of all boxes.
        return boxes[row, col].index == empty.index;
    }

    public void Swap(int fromRow, int fromCol, int toRow, int toCol)
    {
        // Make references to the two puzzle boxes.
        var source = boxes[fromRow, fromCol];
        var target = boxes[toRow, toCol];

        // Swap two boxes in the array.
        boxes[fromRow, fromCol] = target;
        boxes[toRow, toCol] = source;

        // Update local position parameters for these two boexes.
        source.Move(toRow, toCol);
        target.Move(fromRow, fromCol);

        // Update moves in game state and play click sound.
        if (boxes[fromRow, fromCol].smooth)
        {
            scriptGameState.IncreaseMoves();
            audioSource.Play();
        }
    }

    public void HandleClick(int row, int col)
    {
        // Can move upward
        if (InBound(row - 1, col) && IsEmpty(row - 1, col))
            Swap(row, col, row - 1, col);

        // Can move downward
        else if (InBound(row + 1, col) && IsEmpty(row + 1, col))
            Swap(row, col, row + 1, col);

        // Can move leftward
        else if (InBound(row, col - 1) && IsEmpty(row, col - 1))
            Swap(row, col, row, col - 1);

        // Can move rightward
        else if (InBound(row, col + 1) && IsEmpty(row, col + 1))
            Swap(row, col, row, col + 1);

        // Check whether the puzzle has been finished.
        if (CheckEnd())
            StartCoroutine(GameOver());
    }

    public bool CheckEnd()
    {
        for (int row = 0; row < size; row++)
            for (int col = 0; col < size; col++)
                // If any box is not in its original place, then we consider the puzzle is not done and game should be continued.
                if (boxes[row, col].index != row * size + col)
                    return false;
        // If all boxes are in place, then we consider the puzzle is finished and game is over.
        return true;
    }

    IEnumerator GameOver()
    {
        // Deactivate countdown timer in GameState.cs script.
        scriptGameState.StopCountdown();

        // Disable click events for all puzzle boxes.
        SetClickDelegate(null);

        // Wait for (wait)s after game finished.
        yield return new WaitForSeconds(delay);

        // Print debug information.
        Debug.LogWarning("Game Over");

        // Show the last sprite (wait)s after the player finishs the game.
        boxes[size - 1, size - 1].GetComponent<SpriteRenderer>().sprite = sprites[size * size - 1];

        Invoke("LevelFinished", 5f);
    }

    public void LevelFinished()
    {
        GameManager.Instance.LevelFinished();
    }

    public void Restart()
    {
        // Deactivate countdown timer, clear time and moves in GameState.cs script.
        scriptGameState.StopCountdown();
        scriptGameState.ClearTime();
        scriptGameState.ClearMoves();

        // Remove the sprite of the last puzzle box with coord (size-1, size-1).
        boxes[empty.row, empty.column].GetComponent<SpriteRenderer>().sprite = null;

        // Enable smooth movement for all puzzle boxes.
        SetSmoothMovement(false);

        // Call function to shuffle all puzzle boxes randomly.
        Shuffle();

        // Enanble click events for all puzzle boxes.
        SetClickDelegate(HandleClick);

        // Enable smooth movement for all puzzle boxes.
        SetSmoothMovement(true);

        // Call init and then activate countdown timer in GameState.cs script.
        scriptGameState.Init();
        scriptGameState.StartCountdown();

        // Print debug information.
        Debug.LogWarning("Game Restart");
    }

}
