using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{

    public GameController controller;
    public float Health = 5f;
    Transform hpBarTransform;
    public string PlayerName;
    public enum PlayerType { PLAYER, ENEMY }
    public PlayerType playerType;
    public Hand hand; //Assigned in editor

    void Start()
    {
        hpBarTransform = transform.Find("HealthBarContainer");
        TextMeshProUGUI nameTextComponent = transform.Find("Name").Find("NameCanvas").Find("NameText").GetComponent<TextMeshProUGUI>();
        nameTextComponent.text = PlayerName;
        hand.player = this; // Terrible and should be changed to initialize Cards, Hands, and Players properly
    }

    public void updateHpBar()
    {
        float scaleVal = Health + (0.07f * Health);
        if (Health <= 0) { scaleVal = 0f; }
        hpBarTransform.localScale = new Vector3(scaleVal, hpBarTransform.localScale.y, hpBarTransform.localScale.z);
    }

    public void takeHit(float damage)
    {
        if (Health - damage > 0)
        {
            Health = Health - damage;
            updateHpBar();
        }
        else
        {
            Debug.Log(PlayerName + "has died!");
            Health = 0f;
            updateHpBar();
            switch (playerType)
            {
                case PlayerType.PLAYER:
                    controller.handlePlayerDeath();
                    break;
                case PlayerType.ENEMY:
                    controller.handleEnemyDeath();
                    break;
                default:
                    Debug.LogWarning($"Unhandled PlayerType: {playerType}");
                    break;
            }
        }
    }
}
