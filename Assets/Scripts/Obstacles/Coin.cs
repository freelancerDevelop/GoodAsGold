﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A coin has two states (invisible/visible)
/// Based on the current state, collecting a coin will either make it appear or dissapear
/// Collected coins increase the player's curse causing them to move/rotate/etc. slower 
/// </summary>
[RequireComponent(typeof(Renderer), typeof(RotateTransform))]
public class Coin : MonoBehaviour
{
    /// <summary>
    /// Different states the coin can be in
    /// </summary>
    public enum State
    {
        Visible,
        Invisible,
    }

    /// <summary>
    /// A reference to the renderer component
    /// </summary>
    Renderer m_renderer;
    Renderer CoinRenderer
    {
        get {
            if (m_renderer == null) {
                m_renderer = GetComponentInChildren<Renderer>();
            }
            return m_renderer;
        }
    }

    /// <summary>
    /// Current coin state
    /// </summary>
    [SerializeField]
    State m_state;

    /// <summary>
    /// Current coin state
    /// Changing the state of the coin will also change its appearance 
    /// </summary>
    public State CoinState
    {
        get { return m_state; }
        set {
            Material newMaterial = null;
            switch (value) {
                case State.Visible:
                    newMaterial = m_visibleStateMaterial;
                    break;
                case State.Invisible:
                    newMaterial = m_hiddenStateMaterial;
                    break;
            }

            // The accessor is used here as this script is called from the editor
            if (newMaterial != null) {
                CoinRenderer.material = newMaterial;
            }

            m_state = value;
        }
    }

    /// <summary>
    /// Materials that represent the coin being visible
    /// </summary>
    [SerializeField]
    Material m_visibleStateMaterial;

    /// <summary>
    /// Material that represents the coin being hidden
    /// </summary>
    [SerializeField]
    Material m_hiddenStateMaterial;

    /// <summary>
    /// A reference to the coin model to make it rotate
    /// </summary>
    [SerializeField]
    GameObject m_coinModel;

    /// <summary>
    /// A reference to the rotate transform component
    /// </summary>
    RotateTransform m_rotateTransform;
    RotateTransform RotateTransformComponent {
        get {
            if(m_rotateTransform == null) {
                m_rotateTransform = GetComponent<RotateTransform>();
            }

            return m_rotateTransform;
        }
    }


    /// <summary>
    /// Triggers rotation when true
    /// </summary>
    [SerializeField]
    bool m_hasRotation = true;

    /// <summary>
    /// Turns the coins local rotation ON/OFF
    /// </summary>
    public bool HasRotation
    {
        get { return m_hasRotation; }
        set {
            m_hasRotation = value;

            // This is to help editor scripts to find the component
            if (this.RotateTransformComponent != null) {
                if (value) {
                    this.RotateTransformComponent.StartRotation();
                } else {
                    this.RotateTransformComponent.StopRotation();
                }
            }
        }
    }

    /// <summary>
    /// Set references
    /// </summary>
    void Awake()
    {
        m_rotateTransform = GetComponent<RotateTransform>();

        if (m_rotateTransform == null) {
            Debug.LogErrorFormat("Coin Error: Missing Component!" +
                "Rotate Transform = {0}",
                m_rotateTransform
            );
        }
    }

    /// <summary>
    /// When the player collides with the coin it:
    ///     - Reveals hidden coins
    ///     - Triggers player to collect coin and destroys this object
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        // Ignore non-player collisions
        if (!other.CompareTag("Player")) {
            return;
        }

        PlayerManager playerManager = other.GetComponent<PlayerManager>();

        // Log error and destroy object
        if (playerManager == null) {
            Debug.LogErrorFormat("Coin OnTriggerEnter: " +
                "Failed to get PlayerManager component from {0}", 
                other.name
            );
            Destroy(gameObject);
        }

        switch (m_state) {
            case State.Visible:
                Collected(playerManager);
                break;
            case State.Invisible:
                Reveal();
                break;
        }
    }

    /// <summary>
    /// Notifies the player the coin has been collected and destroys itslef
    /// </summary>
    /// <param name="playerManager"></param>
    void Collected(PlayerManager playerManager)
    {
        // Ignore when dead
        if (playerManager.CursePercent >= 100f) {
            return;
        }

        playerManager.CoinsCollected++;
        Destroy(gameObject);
    }

    /// <summary>
    /// Triggers the coin to "reveal" itself by changing the state to visible
    /// </summary>
    void Reveal()
    {
        this.CoinState = State.Visible;
    }

    /// <summary>
    /// Triggers the coin to move towards the destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="speed"></param>
    /// <param name="destroyWhenDone"></param>
    public void MoveTowards(Vector3 destination, float speed, bool destroyWhenDone = true)
    {
        StartCoroutine(MoveTowardsRoutine(destination, speed, destroyWhenDone));
    }

    /// <summary>
    /// Moves the coin towards the destination destroying when it arrives if told to
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="speed"></param>
    /// <param name="destroyWhenDone"></param>
    /// <returns></returns>
    IEnumerator MoveTowardsRoutine(Vector3 destination, float speed, bool destroyWhenDone)
    {
        while(transform.position != destination) {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        if (destroyWhenDone) {
            Destroy(gameObject);
        }
    }
}
