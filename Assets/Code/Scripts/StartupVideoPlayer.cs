using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StartupVideoPlayer : MonoBehaviour {
    public VideoPlayer videoPlayer;
    public VideoClip startupClip;

    void Start() {
        videoPlayer.clip = startupClip;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnClipEnded;
    }

    void OnClipEnded(VideoPlayer vp) {
        SceneManager.LoadScene("MainMenuScene");
    }
}