using Assets.Senet.Scripts.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FAQList : MonoBehaviour
{
    [SerializeField]
    private VerticalLayoutGroup _verticalLayout;
    [SerializeField]
    private GameObject _expanderItemPrefab;
    [SerializeField]
    private List<HeaderAndContent> _headerAndContent;

    void Start()
    {
        foreach (var (data, index) in _headerAndContent.WithIndex())
        {
            var point = index + 1;
            _expanderItemPrefab.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{point}. {data._header}<indent=6%></indent><br><br>";
            _expanderItemPrefab.transform.GetChild(1).GetComponent<TMP_Text>().text = $"<indent=6%>{data._content}</indent>";
            Instantiate(_expanderItemPrefab, _verticalLayout.transform);
        }
    }
}
