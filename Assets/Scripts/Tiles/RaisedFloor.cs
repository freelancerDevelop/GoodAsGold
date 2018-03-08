﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaisedFloor : BaseTile
{
    /// <summary>
    /// A reference to the wall prefab
    /// </summary>
    [SerializeField]
    GameObject m_wallPrefab;

    /// <summary>
    /// A reference to the barrier prefab
    /// </summary>
    [SerializeField]
    GameObject m_barrierPrefab;

    /// <summary>
    /// Spawns the wall tile
    /// Raised floors have a barrier for every side that borders a lower floor or the side of a ramp    /// 
    /// </summary>
    protected override void SpawnComponent()
    {
        Instantiate(m_wallPrefab, transform);

        // Spawn barriers for each side adjacent to a floor or ramp tile
        // Excludes other raised floors
        foreach(Vector3 point in GameManager.cardinalPoints) {
            GameObject tile = m_generator.GetTileAt(m_index + point);
            bool isFloorTile = (tile != null && tile.CompareTag("Floor"));

            GameObject objectOnTile = m_generator.GetObjectAt(m_index + point);
            bool isRamp = (objectOnTile != null && objectOnTile.CompareTag("Ramp"));

            if (isFloorTile) {                
                // We want to avoid blocking off ramps that are rotated to align with this tile
                // This means that if the ramp's forward is in the same direction or opposite to the
                // current cardinal point, then the ramp is connecting with this tile so we skip it
                if (isRamp){
                    Vector3 rampForwardVector = transform.InverseTransformDirection(objectOnTile.transform.forward);
                    if (rampForwardVector == point || rampForwardVector == -point) {
                        continue;
                    }
                }

                GameObject barrier = Instantiate(m_barrierPrefab, transform);
                barrier.transform.LookAt(tile.transform);
            }
        }
    }
}
