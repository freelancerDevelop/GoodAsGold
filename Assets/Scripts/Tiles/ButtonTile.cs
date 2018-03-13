﻿using System;
using UnityEngine;

/// <summary>
/// Handles the creation of buttons with the appropriate floor underneath
/// </summary>
public class ButtonTile : BaseTile
{
    /// <summary>
    /// A reference to the button prefab
    /// </summary>
    [SerializeField]
    GameObject m_buttonPrefab;

    /// <summary>
    /// Spawns the button placing ontop of the floor it is currently on
    /// </summary>
    protected override void SpawnComponent()
    {
        GameObject button = Instantiate(m_buttonPrefab, transform);
        MatchFloorHeight();
    }
}
