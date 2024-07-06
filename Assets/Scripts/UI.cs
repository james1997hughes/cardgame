using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{

    public GameController controller;
    public GameObject monLane1GO;
    public GameObject monLane2GO;
    public GameObject trapLaneGO;
    public GameObject discardLaneGO;

    [HideInInspector]
    public Lane monLane1;
    [HideInInspector]
    public Lane monLane2;
    [HideInInspector]
    public Lane trapLane;
    [HideInInspector]
    public Lane discardLane;


    public GameObject enemyMonLane1GO;
    public GameObject enemyMonLane2GO;
    public GameObject enemyTrapLaneGO;
    public GameObject enemyDiscardLaneGO;
    [HideInInspector]
    public Lane enemyMonLane1;
    [HideInInspector]
    public Lane enemyMonLane2;
    [HideInInspector]
    public Lane enemyTrapLane;
    [HideInInspector]
    public Lane enemyDiscardLane;


    public GameObject enemyPortrait;
    public GameObject playerPortrait;
    public GameObject LargeText;
    public GameObject InfoBox;


    TextMeshProUGUI LargeTextComponent;
    TextMeshProUGUI infoBoxTextComponent;
    // Start is called before the first frame update
    void Start()
    {
        LargeTextComponent = LargeText.GetComponent<TextMeshProUGUI>();
        infoBoxTextComponent = InfoBox.transform.Find("Canvas").Find("TurnInfoText").GetComponent<TextMeshProUGUI>();

        //Convenient to store these here but maybe move to controller in future...
        monLane1 = monLane1GO.GetComponent<Lane>();
        monLane2 = monLane2GO.GetComponent<Lane>();
        trapLane = trapLaneGO.GetComponent<Lane>();
        discardLane = discardLaneGO.GetComponent<Lane>();

        enemyMonLane1 = enemyMonLane1GO.GetComponent<Lane>();
        enemyMonLane2 = enemyMonLane2GO.GetComponent<Lane>();
        enemyTrapLane = enemyTrapLaneGO.GetComponent<Lane>();
        enemyDiscardLane = enemyDiscardLaneGO.GetComponent<Lane>();

    }

    void Update()
    {
        updateInfoBox();
    }

    void updateInfoBox()
    {
        string name;
        if (controller.playerTurn)
        {
            name = controller.player.PlayerName;
        }
        else
        {
            name = controller.enemy.PlayerName;
        }
        string phase;
        switch (controller.turnPhase)
        {
            case GameController.TurnPhase.ATTACK:
                phase = "Attack";
                break;
            case GameController.TurnPhase.DRAW:
                phase = "Draw";
                break;
            case GameController.TurnPhase.SUMMON:
                phase = "Summon";
                break;
            default:
                phase = "???";
                break;
        }
        string info = "Turn:  " + controller.turn.ToString() + "\n"
                        + name + "'s Turn\n"
                        + phase + " Phase";
        infoBoxTextComponent.text = info;
    }

    public IEnumerator handleTextDisplay(bool isEndOfTurn = false, bool isEndOfPhase = false)
    {
        if (isEndOfPhase)
        {
            yield return StartCoroutine(endPhaseText());
        }
        if (isEndOfTurn)
        {
            yield return StartCoroutine(endTurnText());
        }
    }

    public IEnumerator endPhaseText()
    {
        if (controller.turnPhase == GameController.TurnPhase.DRAW)
        {
            LargeTextComponent.text = "DRAW PHASE";
        }
        if (controller.turnPhase == GameController.TurnPhase.SUMMON)
        {
            LargeTextComponent.text = "SUMMON PHASE";
        }
        if (controller.turnPhase == GameController.TurnPhase.ATTACK)
        {
            LargeTextComponent.text = "ATTACK PHASE";
        }
        //LargeTextComponent.color = new Color(LargeTextComponent.color.r, LargeTextComponent.color.g, LargeTextComponent.color.b, 0f);

        yield return StartCoroutine(fadeInText(LargeText.GetComponent<TextMeshProUGUI>(), 0.5f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(fadeOutText(LargeText.GetComponent<TextMeshProUGUI>(), 0.5f));
    }
    public IEnumerator endTurnText()
    {
        if (controller.playerTurn)
        {
            LargeTextComponent.text = controller.player.PlayerName + "'s Turn!";
        }
        else
        {
            LargeTextComponent.text = controller.enemy.PlayerName + "'s Turn!";
        }
        //LargeTextComponent.color = new Color(LargeTextComponent.color.r, LargeTextComponent.color.g, LargeTextComponent.color.b, 0f);

        yield return StartCoroutine(fadeInText(LargeText.GetComponent<TextMeshProUGUI>(), 1f));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(fadeOutText(LargeText.GetComponent<TextMeshProUGUI>(), 1f));
    }

    IEnumerator fadeInText(TextMeshProUGUI textComponent, float duration)
    {

        Color originalColor = textComponent.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Target alpha is 1
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            textComponent.color = Color.Lerp(originalColor, targetColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        textComponent.color = targetColor; // Ensure final color is set correctly
    }
    IEnumerator fadeOutText(TextMeshProUGUI textComponent, float duration)
    {

        Color originalColor = textComponent.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Target alpha is 1

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            textComponent.color = Color.Lerp(originalColor, targetColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textComponent.color = targetColor; // Ensure final color is set correctly

    }
}
