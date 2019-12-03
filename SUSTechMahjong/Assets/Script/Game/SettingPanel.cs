using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    //保存按钮
    private Button saveButton;
    //取消按钮
    private Button cancelButton;
    //控制音量的滑动条
    private Slider volumeControlSlider;
    //记录上一次保存的音量大小
    private float volume;

    //初始化
    public override void OnInit()
    {
        skinPath = "SettingPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        saveButton = skin.transform.Find("SaveBtn").GetComponent<Button>();
        cancelButton = skin.transform.Find("CancelBtn").GetComponent<Button>();
        volumeControlSlider = skin.transform.Find("Slider").GetComponent<Slider>();

        //监听
        saveButton.onClick.AddListener(Audio.ButtonClick);
        cancelButton.onClick.AddListener(Audio.ButtonClick);
        saveButton.onClick.AddListener(OnSaveClick);
        cancelButton.onClick.AddListener(OnCancelClick);
        volumeControlSlider.onValueChanged.AddListener((float value)=>AudioControlChanged(value,volumeControlSlider));

        volume = XMLManager.Volume;
        volumeControlSlider.value = volume;
    }

    private void OnSaveClick()
    {
        volume = volumeControlSlider.value;
        XMLManager.Volume = volumeControlSlider.value;
        Close();
    }

    private void OnCancelClick()
    {
        Audio.SetVolume(volume);
        Close();
    }

    private void AudioControlChanged(float value,Slider eventSlider)
    {
        Audio.SetVolume(value);
    }

    //关闭面板
    public override void OnClose()
    {

    }
}
