using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance = null;
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }
    #endregion

    public GameObject floatingTextPrefab;
    public Transform panelFloating;

    public void SpawnFloatingText(string text, Transform target)
    {
        // Spawn new Floating Text
        GameObject clone = Instantiate(floatingTextPrefab, panelFloating);
        // Text Mesh Pro
        TextMeshProUGUI textMeshPro = clone.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = text;
        // UI Follow Target
        UIFollowTarget follow = clone.GetComponentInChildren<UIFollowTarget>();
        follow.target = target;
    }
}
