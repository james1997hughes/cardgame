#nullable enable

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
        if (turn > 0){
            handleGame();
        }
    }

    public void nextPhase(){
        AIThinking = false;
        if (!handlingPhaseChange){
            StartCoroutine(nextPhaseCoroutine());
        }
    }
    IEnumerator nextPhaseCoroutine(){
        handlingPhaseChange = true;
        Debug.Log("Next phase");


        
        if (turnPhase == TurnPhase.DRAW){
            if (!playerTurn){
                enemyHand.drawToMax();
            }
            turnPhase = TurnPhase.SUMMON;
            yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true));
            
        } else
        if (turnPhase == TurnPhase.SUMMON){
            turnPhase = TurnPhase.ATTACK;
            yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true));
            
        } else
        if (turnPhase == TurnPhase.ATTACK){
            if (!halfTurnOver){
                halfTurnOver = true;
            } else if(halfTurnOver){
                halfTurnOver = false;

                Debug.Log("Round "+turn.ToString()+" Over");
                turn +=1;
            }
            turnPhase = TurnPhase.DRAW;
            if (playerTurn){
                Debug.Log("Turn Over! Switching to Enemy Turn");
                playerTurn = false;
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfTurn: true));
                
            }else {
                Debug.Log("Turn Over! Switching to Player Turn");
                playerTurn = true;
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfTurn: true));
            }
            
            

        }
        handlingPhaseChange = false;
    }



    IEnumerator endofRound(){
        turnPhase = TurnPhase.DRAW;
        float time = 0f;
        while(time < 1f){
            float t = time / 1;;
            time += Time.deltaTime;
            yield return null;
        }
    }

    void handleGame(){
        if (playerTurn){
            if (turnPhase == TurnPhase.DRAW){
                playerHand.drawToMax();
                nextPhase();
            } else if(turnPhase == TurnPhase.SUMMON){
                //Do nothing, wait for player to summon
            } else if(turnPhase == TurnPhase.ATTACK){
                //Do nothing, wait for player to attack
            }


        }else{ // Enemy Turn
            if (turnPhase == TurnPhase.DRAW){
                nextPhase();
                
            } else if(turnPhase == TurnPhase.SUMMON && !AIThinking){
                AIThinking = true;
                StartCoroutine(EnemySummon());
                
            } else if(turnPhase == TurnPhase.ATTACK && !AIThinking){
                AIThinking = true;
                StartCoroutine(EnemyAttack());
            }
        }

    }
    IEnumerator EnemySummon()
    {
        yield return new WaitForSeconds(1f);
        nextPhase();
    }
    
    IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(1f);
        nextPhase();
    }

    public void startGame(){
        Debug.Log("Starting Game!");
        turn = 1;
        gameRunning = true;
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
