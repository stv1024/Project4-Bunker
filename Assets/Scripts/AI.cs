using System.Collections.Generic;
using Fairwood.Math;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// AI
/// </summary>
public class AI : MonoBehaviour
{
    public Unit Owner;

    void Start()
    {

    }

    private void Update()
    {
        if (!Owner) return;
        if (!Owner.Data.IsAlive) return;
        if (!Owner.isServer) return;

    }
}