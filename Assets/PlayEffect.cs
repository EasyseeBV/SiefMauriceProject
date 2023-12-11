using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Audio;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]
public class PlayEffect : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    public VideoClip BasicFace;
    public VideoClip KissingFace;
    public AudioClip SpecialAudio;

    public void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();
        if (BasicFace == null)
        {
            Debug.LogError("Video clip is not assigned in VideoPlayer!");
            Debug.LogError(gameObject.name);
            return;
        }
        StartOriginalVideo();

        if (TryGetComponent(out Interact interact))
        {
            interact.OnClickEvent.AddListener(StartKissingVideo);
        }
    }

    public void StartOriginalVideo()
    {
        videoPlayer.clip = BasicFace;
        double randomStartTime = Random.Range(0f, (float)videoPlayer.clip.length);
        videoPlayer.time = randomStartTime;
        videoPlayer.Play();
        videoPlayer.isLooping = true;
    }

    public void StartKissingVideo()
    {
        if (videoPlayer.clip == KissingFace)
        {
            Debug.Log("Already Playing");
            return;
        }
        if (KissingFace != null)
        {
            videoPlayer.clip = KissingFace;      
            videoPlayer.Play();
            videoPlayer.loopPointReached += EndReached;
        }
        else
        {
            Debug.LogError("Video clip is not assigned in VideoPlayer!");
        }

        if (SpecialAudio != null)
        {
            audioSource.clip = SpecialAudio;
            audioSource.Play();
            audioSource.loop = false;
        }
        else
        {
            Debug.LogWarning("Audio clip is not assigned in AudioPlayer!");
        }
    }

    void EndReached(VideoPlayer vp)
    {
        StartOriginalVideo();
        videoPlayer.loopPointReached -= EndReached;
    }
}
