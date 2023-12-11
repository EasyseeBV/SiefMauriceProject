using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkGame : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        GameManager.Instance.LinkedObject = gameObject;
    }
    private void OnDisable()
    {
        GameManager.Instance.LinkedObject = null;
    }

}
