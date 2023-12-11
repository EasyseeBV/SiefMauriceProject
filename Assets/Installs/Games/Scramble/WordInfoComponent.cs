using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MText;

public class WordInfoComponent : MonoBehaviour
{
    public int Index;
    public string Text;
    public Modular3DText T;
    Color c;
    public bool Clickable = true;

    public void Start()
    {
        c = GetComponent<Renderer>().material.GetColor("_Color");
    }

    public void OnMouseDown()
    {
        if (Clickable)
            FindObjectOfType<ScrambleManager>().AddSelectedWord(Index);
    }

    public void SetText(string Letters)
    {
        Letters = Letters;
        Text = Letters;
        T.text = Letters;
    }
    public void Select()
    {
        GetComponent<Renderer>().material.SetColor("_Color", c * c);
    }

    public void UnSelect()
    {
        GetComponent<Renderer>().material.SetColor("_Color", c);
    }

    public void LockObject()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.green);
    }
}
