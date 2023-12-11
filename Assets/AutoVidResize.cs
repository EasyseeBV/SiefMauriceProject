using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class AutoVidResize : MonoBehaviour
{
    public bool Resize;
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.Prepare();
    }


    // Update is called once per frame
    void Update()
    {
     if (Resize)
        {
            Resize = false;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x / videoPlayer.clip.width * videoPlayer.clip.height, transform.localScale.z);
        }   
    }
}
