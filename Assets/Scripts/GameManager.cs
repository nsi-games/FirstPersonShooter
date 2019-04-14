using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance = null;
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }
    #endregion


    private List<Vector3> hitPoints = new List<Vector3>();

    public void AddHitPoint(Vector3 hitPoint)
    {
        hitPoints.Add(hitPoint);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var hit in hitPoints)
        {
            Gizmos.DrawWireSphere(hit, .1f);
        }
    }

}
