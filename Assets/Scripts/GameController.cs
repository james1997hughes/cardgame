#nullable enable
#pragma warning disable CS8602 // Dereference of a possibly null reference.


using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{

    GameObject UIGO;
    UI ui;


    public GamePlayer? player;
    public GamePlayer? enemy;

    
    Card? PlayerLane1Card;
    Card? PlayerLane2Card;
    Card? PlayerTrapLaneCard;
    //Card PlayerSpellLaneCard;

    Card? enemyLane1Card;
    Card? enemyLane2Card;
    Card? enemyTrapLaneCard;
    //Card enemySpellLaneCard;

    GameObject phaseText;
    TextMeshProUGUI textComponent;

    GameObject turnText;
    TextMeshProUGUI turnTextComponent;

    public Hand? playerHand; //Assigned in editor
    public Hand? enemyHand; //Assigned in editor

    public int turn;
    public enum TurnPhase {DRAW, SUMMON, ATTACK}
    public TurnPhase turnPhase;
    public bool playerTurn;

    public bool gameRunning = false;
    public bool halfTurnOver = false;
    public bool AIThinking = false;

    IEnumerator FadeInPhaseText;
    IEnumerator FadeOutPhaseText;
    IEnumerator FadeInTurnText;
    IEnumerator FadeOutTurnText;
    bool handlingPhaseChange = false;
    

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.Find("PlayerGamePlayer").gameObject.GetComponent<GamePlayer>();
        player.controller = this;
        enemy = GameObject.Find("EnemyGamePlayer").gameObject.GetComponent<GamePlayer>();
        enemy.controller = this;
        player.updateHpBar();
        enemy.updateHpBar();
        UIGO = GameObject.Find("UI");
        ui = UIGO.GetComponent<UI>();
        ui.controller = this;





        if (enemy == null || enemyHand == null || playerHand == null){
            throw new Exception("Something is null yo!");
        }

        turn = 0;

        playerTurn = true;
        
    }
    public void setLane1Player(Card card){
        PlayerLane1Card = card;
    }
    public void setLane2Player(Card card){
        PlayerLane2Card = card;
    }
    public void setTrapLanePlayer(Card card){
        PlayerTrapLaneCard = card;
    }

    public void attackEnemy(float damage){
        if (enemyLane1Card == null && enemyLane2Card == null){
            enemy.takeHit(damage);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void nextPhase(){
        if (playerTurn){
            StartCoroutine(nextPhaseCoroutine());
        }
    }

    IEnumerator nextPhaseCoroutine(){
        if (!handlingPhaseChange){
            handlingPhaseChange = true;
            Debug.Log("Next phase");

            if (turnPhase == TurnPhase.DRAW){
                turnPhase = TurnPhase.SUMMON;
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true));
            } else if (turnPhase == TurnPhase.SUMMON){
                turnPhase = TurnPhase.ATTACK;
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true));                
            } else if (turnPhase == TurnPhase.ATTACK){
                turnPhase = TurnPhase.DRAW;
                handleEndOfTurn();
            }

            handlingPhaseChange = false;
        }
    }
    void handleEndOfTurn(){
        if (!halfTurnOver){
            halfTurnOver = true;
        } else if(halfTurnOver){
            halfTurnOver = false;
            turn +=1;
        }
        if (playerTurn){
            playerTurn = false;
        }else {
            playerTurn = true;
        }
    }

    IEnumerator handleGame(){
      while(gameRunning){
        if (playerTurn){
            if (turnPhase == TurnPhase.DRAW){
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfTurn: true)); //ABIGAILS TURN
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true)); //DRAW PHASE
                    playerHand.drawToMax();                                                //Draw max hand
                    yield return StartCoroutine(nextPhaseCoroutine());                   //SUMMON PHASE
            } else if(turnPhase == TurnPhase.SUMMON){
                //Do nothing, wait for player to summon
            } else if(turnPhase == TurnPhase.ATTACK){
                //Do nothing, wait for player to attack
            }
        }else{ // Enemy Turn
            if (turnPhase == TurnPhase.DRAW){
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfTurn: true)); //PIERRES TURN
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true)); //DRAW PHASE
                enemyHand.drawToMax();                                                  //Draw Max Hand
                yield return StartCoroutine(nextPhaseCoroutine());                      //SUMMON PHASE
            } else if(turnPhase == TurnPhase.SUMMON){
                yield return StartCoroutine(EnemySummon());
                yield return StartCoroutine(nextPhaseCoroutine());                      //ATTACK PHASE       
            } else if(turnPhase == TurnPhase.ATTACK){
                yield return StartCoroutine(EnemyAttack());
                yield return StartCoroutine(nextPhaseCoroutine());
            }
        }
        yield return null;
      }

    }
    IEnumerator EnemySummon()
    {
        yield return new WaitForSeconds(2f);
    }
    
    IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(1f);
    }

    public void startGame(){
        Debug.Log("Starting Game!");
        turn = 1;
        gameRunning = true;
        StartCoroutine(handleGame());
    }

    public void handlePlayerDeath(){
        Debug.Log("You Lose!");
        turn = 0;
    }
    public void handleEnemyDeath(){
        Debug.Log("You Win!");
        turn = 0;
    }
}
