using Assets.Senet.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HeaderAndContent
{
    [TextArea(15, 20)]
    public string _header;
    [TextArea(15, 20)]
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
            _tmpText.text += Regex.Unescape($"<b><size=120%>{point}. {data._header}</size></b>\r\n<size=90%><indent=7%>{data._content}</indent></size><br><br>");
        }
    }
}
