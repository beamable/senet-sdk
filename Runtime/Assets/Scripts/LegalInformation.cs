using Assets.Senet.Scripts.Extensions;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class HeaderAndContent
{
    public string _header;
    public string _content;
}

public class LegalInformation : MonoBehaviour
{
    [SerializeField]
    private List<HeaderAndContent> _headerAndContent;
    [SerializeField]
    private TMP_Text _tmpText;

    void Start()
    {
        foreach (var (data, index) in _headerAndContent.WithIndex())
        {
            var point = index + 1;
            _tmpText.text += $"<b><size=120%>{point}. {data._header}</size></b>\r\n<size=90%><indent=7%>{data._content}</indent></size><br><br>";
        }
    }
}
