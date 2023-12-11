using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBox : MonoBehaviour
{

    public int index = -1;                      // Index of the puzzle box.
    public int row = -1;                        // Row in coordinate for the puzzle box. [0, size-1]
    public int column = -1;                     // Column in coordinate for the puzzle box. [0, size-1]
    public bool smooth = false;                 // Indicates whether to use smooth movement.
    public float duration = 0.1f;               // Time used during smooth movement.
    private Vector3 velocity = Vector3.zero;    // Velocity during smooth movement.
    public Action<int, int> click = null;       // A delegate reference to click event.

    public void Init(int _index, int _row, int _col, Sprite _sprite, Action<int, int> _click, AudioClip _clip)
    {
        // Init index.
        this.index = _index;
        // Init coordinate and local position.
        Move(_row, _col);
        // Init sprite of the SpriteRenderer component.
        this.GetComponent<SpriteRenderer>().sprite = _sprite;
        // Init function reference.
        this.click = _click;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && click != null)
            click(row, column);
    }

    public void Move(int row, int col)
    {
        // Update coordinate parameters.
        this.row = row;
        this.column = col;
        // Teleport the box to the target local position when the smooth movement is disabled.
        if (!smooth) transform.localPosition = new Vector2(this.column, -this.row);
    }

    private void Update()
    {
        if (smooth)
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector2(this.column, -this.row), ref velocity, duration);
    }

}
