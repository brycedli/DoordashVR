using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public static VideoManager main;

    VideoPlayer player;
    private void Start()
    {
        main = this;
        player = GetComponent<VideoPlayer>();
    }

    public void stopVideo ()
    {
        
        player.Stop();
    }

    public void resetVideo()
    {
        player.Play();
        player.frame = 0;
    }
}
