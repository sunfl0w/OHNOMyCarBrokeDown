using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StartupVideoPlayer : MonoBehaviour {
    public VideoPlayer videoPlayer;
    public VideoClip startupClip;
    public VideoClip problemClip;
    // Start is called before the first frame update
    void Start() {
        videoPlayer.clip = startupClip;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnClipEnded;
    }

    void OnClipEnded(VideoPlayer vp) {
        if(vp.clip.name == startupClip.name) {
            videoPlayer.clip = problemClip;
            videoPlayer.Play();
        } else {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}