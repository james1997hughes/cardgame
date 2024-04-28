using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{

    public GameController controller;
    public GameObject monLane1;
    public GameObject monLane2;
    public GameObject trapLane;
    public GameObject discardLane;
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
        
    }

    void Update()
    {
        updateInfoBox();
    }

    void updateInfoBox(){
        string name;
        if (controller.playerTurn){
            name = controller.player.PlayerName;
        } else{
            name = controller.enemy.PlayerName;
        }
        string phase;
        switch (controller.turnPhase) {
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
        string info =   "Turn:  "+controller.turn.ToString()+"\n"
                        +name+"'s Turn\n"
                        +phase+" Phase";
        infoBoxTextComponent.text = info;
        

    }

    public IEnumerator handleTextDisplay(bool isEndOfTurn = false, bool isEndOfPhase = false){
        if (isEndOfPhase){
            yield return StartCoroutine(endPhaseText());
        }
        if (isEndOfTurn){
            yield return StartCoroutine(endTurnText());
            yield return StartCoroutine(endPhaseText());
        }
    }

    public IEnumerator endPhaseText(){
        if (controller.turnPhase == GameController.TurnPhase.DRAW){
            LargeTextComponent.text = "DRAW PHASE";
        }
        if (controller.turnPhase == GameController.TurnPhase.SUMMON){
            LargeTextComponent.text = "SUMMON PHASE";
        }
        if (controller.turnPhase == GameController.TurnPhase.ATTACK){
            LargeTextComponent.text = "ATTACK PHASE";
        }
        //LargeTextComponent.color = new Color(LargeTextComponent.color.r, LargeTextComponent.color.g, LargeTextComponent.color.b, 0f);
        
        yield return StartCoroutine(fadeInText(LargeText.GetComponent<TextMeshProUGUI>(), 0.5f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(fadeOutText(LargeText.GetComponent<TextMeshProUGUI>(), 0.5f));
    }
    public IEnumerator endTurnText(){
        if (controller.playerTurn){
            LargeTextComponent.text = controller.player.PlayerName + "'s Turn!";
        }else{
            LargeTextComponent.text = controller.enemy.PlayerName + "'s Turn!";
        }
        //LargeTextComponent.color = new Color(LargeTextComponent.color.r, LargeTextComponent.color.g, LargeTextComponent.color.b, 0f);
        
        yield return StartCoroutine(fadeInText(LargeText.GetComponent<TextMeshProUGUI>(), 1f));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(fadeOutText(LargeText.GetComponent<TextMeshProUGUI>(), 1f));
    }
    
     IEnumerator fadeInText(TextMeshProUGUI textComponent, float duration)
    {
        Debug.Log("Fade IN Text");
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
        Debug.Log("Fade OUT Text");
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
