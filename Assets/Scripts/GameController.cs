#nullable enable
#pragma warning disable CS8602 // Dereference of a possibly null reference.


using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{

    GameObject UIGO;
    public UI ui;


    public GamePlayer? player;
    public GamePlayer? enemy;


    public Card? PlayerLane1Card;
    public Card? PlayerLane2Card;
    public Card? PlayerTrapLaneCard;
    //Card PlayerSpellLaneCard;

    public Card? EnemyLane1Card;
    public Card? EnemyLane2Card;
    public Card? EnemyTrapLaneCard;
    //Card enemySpellLaneCard;

    public Hand? playerHand; //Assigned in editor
    public Hand? enemyHand; //Assigned in editor

    public int turn;
    public enum TurnPhase { DRAW, SUMMON, ATTACK }
    public TurnPhase turnPhase;
    public bool playerTurn;

    public bool gameRunning = false;
    public bool halfTurnOver = false;
    public bool AIThinking = false;

    bool handlingPhaseChange = false;

    GameState gameState;


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

        turn = 0;
        playerTurn = true;
    }
    public void setLane1Player(Card card)
    {
        PlayerLane1Card = card;
    }
    public void setLane2Player(Card card)
    {
        PlayerLane2Card = card;
    }
    public void setTrapLanePlayer(Card card)
    {
        PlayerTrapLaneCard = card;
    }
    public void setLane1Enemy(Card card)
    {
        EnemyLane1Card = card;
    }
    public Card getLane1Enemy()
    {
        return EnemyLane1Card;
    }
    public void setLane2Enemy(Card card)
    {
        EnemyLane2Card = card;
    }
    public Card getLane2Enemy()
    {
        return EnemyLane2Card;
    }
    public void setTrapLaneEnemy(Card card)
    {
        EnemyTrapLaneCard = card;
    }

    //Attack enemyplayer, not card
    public void attackGamePlayer(GamePlayer playerToAttack, float damage)
    {
        if (playerToAttack == player && PlayerLane1Card == null && PlayerLane2Card == null)
        {
            playerToAttack.takeHit(damage);
        } 
        else if (playerToAttack == enemy && EnemyLane1Card == null && EnemyLane2Card == null)
        {
            playerToAttack.takeHit(damage);
        }
    }
    public void tryDiscard(Card card, Hand hand)
    {
        card.transform.Find("Card_Front").gameObject.SetActive(false);
        card.transform.Find("Card_Back").gameObject.SetActive(true);
        card.restPosition = card.parentHand.playerControlled ? ui.discardLaneGO.transform.position : ui.enemyDiscardLaneGO.transform.position;
        card.DiscardEffect();

        if (!hand.cardsInHand.Remove(card))
        {
            Debug.Log("Card failed to remove!");
        }
        hand.drawCards(1);
    }
    public void tryPlayCard(Card card, Hand hand, Lanes lane)
    {
        if (hand.spellSlotCurrent - card.Cost >= 0)
        {
            if (hand.playerControlled){
                if (lane == Lanes.MONSTER_LANE_1){
                    if (PlayerLane1Card == null){
                        PlayerLane1Card = card;
                        card.lane = ui.monLane1;
                        playCard(card, lane, hand);
                    }
                }
                else if (lane == Lanes.MONSTER_LANE_2){
                    if (PlayerLane2Card == null){
                        PlayerLane2Card = card;
                        card.lane = ui.monLane2;
                        playCard(card, lane, hand);
                    }
                }
                else if (lane == Lanes.TRAP_LANE){
                    if (PlayerTrapLaneCard == null){
                        PlayerTrapLaneCard = card;
                        card.lane = ui.trapLane;
                        playCard(card, lane, hand);
                    }
                }
            } else if (hand.playerControlled == false){ // Enemy move
                if (lane == Lanes.MONSTER_LANE_1){
                    if (EnemyLane1Card == null){
                        EnemyLane1Card = card;
                        card.lane = ui.enemyMonLane1;
                        playCard(card, lane, hand);
                    }
                }
                else if (lane == Lanes.MONSTER_LANE_2){
                    if (EnemyLane2Card == null){
                        EnemyLane2Card = card;
                        card.lane = ui.enemyMonLane2;
                        playCard(card, lane, hand);
                    }
                }
                else if (lane == Lanes.TRAP_LANE){
                    if (EnemyTrapLaneCard == null){
                        EnemyTrapLaneCard = card;
                        card.lane = ui.enemyTrapLane;
                        playCard(card, lane, hand);
                    }
                }
            }
        }
    }
    public void tryDoSpell(Card card, Card cardTarget, Hand hand){
            Debug.Log($"Trying spell. Card: {card.CardName}, target: {cardTarget.CardName}");
            if (hand.spellSlotCurrent - card.Cost >= 0 && cardTarget != null)
            {
                if (!hand.cardsInHand.Remove(card))
                { // Remove card from parent hand
                    Debug.Log("Card failed to remove!");
                }
                hand.adjustSpellSlotCurrent(card.Cost);
                card.SpellEffect(cardTarget);
            }

    }
    private void playCard(Card card, Lanes lane, Hand hand){
            if (!hand.cardsInHand.Remove(card))
            { // Remove card from parent hand
                Debug.Log("Card failed to remove!");
            }

            card.inHand = false;
            card.transform.Find("Card_Front").gameObject.SetActive(true);
            card.transform.Find("Card_Back").gameObject.SetActive(false);
            card.parentHand.numberCardsPlayed += 1; // TODO Take out
            card.sortingGroup.sortingLayerName = "cards_played";
            card.sortingGroup.sortingOrder = card.parentHand.numberCardsPlayed;
            card.fixText();
            card.transform.localScale = card.defaultScale;
            
            switch (lane)
            {
                case Lanes.MONSTER_LANE_1:
                    card.inLane = true;
                    card.restPosition = card.parentHand.playerControlled ? ui.monLane1GO.transform.position : ui.enemyMonLane1GO.transform.position;
                    card.parentHand.adjustSpellSlotCurrent(card.Cost);
                    card.PlayEffect();
                    break;
                case Lanes.MONSTER_LANE_2:
                    card.inLane = true;
                    card.restPosition = card.parentHand.playerControlled ? ui.monLane2GO.transform.position : ui.enemyMonLane2GO.transform.position;
                    card.parentHand.adjustSpellSlotCurrent(card.Cost);
                    card.PlayEffect();
                    break;
                case Lanes.TRAP_LANE:
                    card.inLane = true;
                    card.restPosition = card.parentHand.playerControlled ? ui.trapLaneGO.transform.position : ui.enemyTrapLaneGO.transform.position;
                    card.parentHand.adjustSpellSlotCurrent(card.Cost);
                    card.PlayEffect(); // Maybe trapeffect in future
                    break;
            }
            StartCoroutine(card.returnCardAnim());
    }
    public void attackCard(Card attackingCard, Card defendingCard)
    {

        StartCoroutine(doCardAttack(attackingCard, defendingCard));
    }
    
    public IEnumerator executeMove(Move move){
        Debug.Log("Executing move\n"+move.ToString());
        switch (move.moveType){
            case MoveType.SUMMON:
                tryPlayCard(move.card, move.player.hand, move.laneTarget);
                break;
            case MoveType.ATTACK_MONSTER:
                yield return StartCoroutine(doCardAttack(move.card, move.cardTarget));
                break;
            case MoveType.ATTACK_PLAYER:
                float playerDamage = move.card.MonAtk + move.card.StatModifiers["MonAtk"];
                attackGamePlayer(move.playerTarget, playerDamage);
                break;
            case MoveType.SPELL:
                tryDoSpell(move.card, move.cardTarget, move.player.hand);
                break;
            case MoveType.PASS:
                break;
            default:
                break;
        }
        gameState.addMove(move);
        gameState.updateGameStateFromController();
        yield return null;
    }
    IEnumerator doCardAttack(Card attackingCard, Card defendingCard)
    {
        Debug.Log(attackingCard.CardName + " is now attacking " + defendingCard.CardName);

        attackingCard.PreAttackEffect();
        //probably in future do pre defence effect


        if (attackingCard.MonAtk + attackingCard.StatModifiers["MonAtk"] >= defendingCard.Def + defendingCard.StatModifiers["Def"])
        {
            defendingCard.HP -= attackingCard.MonAtk + attackingCard.StatModifiers["MonAtk"];
            Debug.Log("Successful attack! Defending card hp: " + defendingCard.HP);
        }

        if (attackingCard.Def + attackingCard.StatModifiers["Def"] <= defendingCard.MonAtk + defendingCard.StatModifiers["MonAtk"])
        {
            attackingCard.HP -= defendingCard.MonAtk + defendingCard.StatModifiers["MonAtk"];
            Debug.Log("Successful defence! Attacking card hp: " + attackingCard.HP);
        }

        yield return StartCoroutine(cardAttackAnimation(attackingCard, defendingCard));

        attackingCard.updateStatBars();
        defendingCard.updateStatBars();

        if (attackingCard.HP <= 0)
        {
            yield return StartCoroutine(KillCard(attackingCard));
        }
        if (defendingCard.HP <= 0)
        {
            yield return StartCoroutine(KillCard(defendingCard));
        }


        attackingCard.PostAttackEffect();
        yield return null;
    }

    public IEnumerator KillCard(Card card)
    {
        Debug.Log("Killing "+card.CardName);
        if (card.lane != null){
            if (card.parentHand.playerControlled){
                if (card.lane.lane == Lanes.MONSTER_LANE_1){
                    gameState.friendlyMonLane1Card = null;
                }
                if (card.lane.lane == Lanes.MONSTER_LANE_2){
                    gameState.friendlyMonLane2Card = null;
                }
            }else{
                if (card.lane.lane == Lanes.MONSTER_LANE_1){
                    gameState.enemyMonLane1Card = null;
                }
                if (card.lane.lane == Lanes.MONSTER_LANE_2){
                    gameState.enemyMonLane2Card = null;
                }
            }
            card.lane.card = null;
        }
        try {
            Destroy(card.gameObject);
            // Live card stuff, such as playing an animation or sound.
            // Should instead check if the card has a gameObject somehow
        } catch (Exception e){
        }
        yield return null;
    }

    IEnumerator cardAttackAnimation(Card attackingCard, Card defendingCard)
    {
        //swoosh BAM
        Debug.Log($"Attack animation: {attackingCard.CardName} attacks {defendingCard.CardName}");
        yield return null;
    }


    public void nextPhase()
    {
        if (playerTurn)
        {
            StartCoroutine(nextPhaseCoroutine());
        }
    }

    IEnumerator nextPhaseCoroutine()
    {
        if (!handlingPhaseChange)
        {
            handlingPhaseChange = true;
            Debug.Log("Next phase");

            if (turnPhase == TurnPhase.DRAW)
            {
                turnPhase = TurnPhase.SUMMON;
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true));
            }
            else if (turnPhase == TurnPhase.SUMMON)
            {
                if (turn > 1){
                    turnPhase = TurnPhase.ATTACK;
                    yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true));
                }
                else{
                    turnPhase = TurnPhase.DRAW;
                    handleEndOfTurn();
                }
                
            }
            else if (turnPhase == TurnPhase.ATTACK)
            {
                turnPhase = TurnPhase.DRAW;
                handleEndOfTurn();
            }

            handlingPhaseChange = false;
        }
    }
    void handleEndOfTurn()
    {
        if (!halfTurnOver)
        {
            halfTurnOver = true;
        }
        else if (halfTurnOver)
        {
            halfTurnOver = false;
            turn += 1;
        }
        if (playerTurn)
        {
            playerTurn = false;
        }
        else
        {
            playerTurn = true;
        }
    }

    IEnumerator handleGame()
    {
        gameState = new GameState(player, enemy, this);
        while (gameRunning)
        {
            if (playerTurn)
            {
                if (turnPhase == TurnPhase.DRAW)
                {                    
                    gameState.updateGameStateFromController();
                    yield return StartCoroutine(ui.handleTextDisplay(isEndOfTurn: true)); //ABIGAILS TURN
                    playerHand.adjustSpellSlotCurrent(0, max: true);
                    yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true)); //DRAW PHASE
                    playerHand.drawToMax();                                                //Draw max hand
                    if (turn == 1){
                        enemyHand.drawToMax();
                    }
                    yield return StartCoroutine(nextPhaseCoroutine());                   //SUMMON PHASE
                }
                else if (turnPhase == TurnPhase.SUMMON)
                {
                    //Do nothing, wait for player to summon
                }
                else if (turnPhase == TurnPhase.ATTACK)
                {
                    //Do nothing, wait for player to attack
                }
            }
            else
            { // Enemy Turn
                if (turnPhase == TurnPhase.DRAW)
                {

                    yield return StartCoroutine(ui.handleTextDisplay(isEndOfTurn: true)); //PIERRES TURN
                    enemyHand.adjustSpellSlotCurrent(0, max: true);
                    yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true)); //DRAW PHASE
                    enemyHand.drawToMax();                                                  //Draw Max Hand
                    if (turn ==1){
                        enemyHand.drawCards(1);
                    }
                    yield return StartCoroutine(nextPhaseCoroutine());                      //SUMMON PHASE
                }
                else if (turnPhase == TurnPhase.SUMMON)
                {
                    yield return new WaitForSeconds(1.5f);
                    yield return StartCoroutine(EnemyMove());
                    yield return StartCoroutine(nextPhaseCoroutine());                      //ATTACK PHASE       
                }
                else if (turnPhase == TurnPhase.ATTACK)
                {
                    //yield return StartCoroutine(EnemyAttack());
                    yield return new WaitForSeconds(1.5f);
                    yield return StartCoroutine(EnemyMove());
                    yield return StartCoroutine(nextPhaseCoroutine());
                }
            }
            yield return null;
        }

    }

    Card chooseRandomCardFromHand(Hand hand)
    {
        int randomIndex = UnityEngine.Random.Range(0, hand.cardsInHand.Count);
        return hand.cardsInHand[randomIndex];
    }
    IEnumerator EnemyMove()
    {
        int maxMoves = 3;
        int movesMade = 0;
        while (enemyHand.spellSlotCurrent > 0 && movesMade < maxMoves){
            Move moveToMake = null;
            movesMade += 1;
            Debug.Log("Finding Best Move");
            yield return StartCoroutine(gameState.getImmediateBestMove(enemy, (move) => {
                if (move != null){
                    moveToMake = move;
                }else{
                    return;
                }
                
            }));
            if (moveToMake != null){
                Debug.Log(moveToMake.ToString());
                yield return executeMove(moveToMake);
                yield return new WaitForSeconds(1);
            }
        }

        yield return null;
    }

    IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(1f);
    }

    public void startGame()
    {
        Debug.Log("Starting Game!");
        turn = 1;
        gameRunning = true;
        StartCoroutine(handleGame());
    }

    public void handlePlayerDeath()
    {
        Debug.Log("You Lose!");
        turn = 0;
    }
    public void handleEnemyDeath()
    {
        Debug.Log("You Win!");
        turn = 0;
    }
}
