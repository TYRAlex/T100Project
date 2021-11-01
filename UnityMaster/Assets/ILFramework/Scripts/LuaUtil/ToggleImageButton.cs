using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleImageButton : MonoBehaviour
{

    public Sprite playImage;
    public Sprite pauseImage;
    public Sprite disableImage;
    public bool isPlayshow = true;//是否初始显示play图片
    Button _playButton;

    private void Awake()
    {
        _playButton = GetComponent<Button>();
    }

    public void toggleImage()
    {
        isPlayshow = !isPlayshow;
        _playButton.image.sprite = isPlayshow ? playImage : pauseImage;
    }

    public void setPlayImage(bool isShow)
    {
        _playButton.image.sprite = isShow ? playImage : pauseImage;
        isPlayshow = isShow;
    }

    public void setDisable(bool isOn)
    {
        _playButton.image.sprite = isOn ? disableImage : pauseImage;
        _playButton.enabled = !isOn;
    }

}
