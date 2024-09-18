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

    Card? EnemyLane1Card;
    Card? EnemyLane2Card;
    Card? EnemyTrapLaneCard;
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
    public void attackEnemy(float damage)
    {
        if (EnemyLane1Card == null && EnemyLane2Card == null)
        {
            enemy.takeHit(damage);
        }
    }
    public void attackPlayer(float damage)
    {
        if (PlayerLane1Card == null && PlayerLane2Card == null)
        {
            player.takeHit(damage);
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
    public void tryPlayCard(Card card, Hand hand, Lane lane)
    {
        if (hand.spellSlotCurrent - card.Cost >= 0)
        {
            if (lane.card == null)
            { //Card can be played
                if (!hand.cardsInHand.Remove(card))
                { // Remove card from parent hand
                    Debug.Log("Card failed to remove!");
                }

                lane.card = card;
                card.inHand = false;
                card.transform.Find("Card_Front").gameObject.SetActive(true);
                card.transform.Find("Card_Back").gameObject.SetActive(false);
                card.parentHand.numberCardsPlayed += 1; // TODO Take out
                card.sortingGroup.sortingLayerName = "cards_played";
                card.sortingGroup.sortingOrder = card.parentHand.numberCardsPlayed;
                card.fixText();
                card.transform.localScale = card.defaultScale;
            
                switch (lane.lane)
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
            else
            {
                Debug.Log("Lane already occupied!");
            }
        }
    }
    public void attackCard(Card attackingCard, Card defendingCard)
    {

        StartCoroutine(doCardAttack(attackingCard, defendingCard));
    }
    IEnumerator doCardAttack(Card attackingCard, Card defendingCard)
    {
        Debug.Log(attackingCard.CardName + " is attacking " + defendingCard.CardName);

        attackingCard.PreAttackEffect();
        //probably in future do pre defence effect


        //This does not handle tempHP TODO
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
            KillCard(attackingCard);
        }
        if (defendingCard.HP <= 0)
        {
            KillCard(attackingCard);
        }


        attackingCard.PostAttackEffect();
    }

    void KillCard(Card card)
    {
        card.lane.card = null;//??
        Destroy(card.gameObject);
    }

    IEnumerator cardAttackAnimation(Card attackingCard, Card defendingCard)
    {
        //swoosh BAM
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
                turnPhase = TurnPhase.ATTACK;
                yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true));
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
        while (gameRunning)
        {
            if (playerTurn)
            {
                if (turnPhase == TurnPhase.DRAW)
                {
                    yield return StartCoroutine(ui.handleTextDisplay(isEndOfTurn: true)); //ABIGAILS TURN
                    playerHand.adjustSpellSlotCurrent(0, max: true);
                    yield return StartCoroutine(ui.handleTextDisplay(isEndOfPhase: true)); //DRAW PHASE
                    playerHand.drawToMax();                                                //Draw max hand
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
                    yield return StartCoroutine(nextPhaseCoroutine());                      //SUMMON PHASE
                }
                else if (turnPhase == TurnPhase.SUMMON)
                {
                    yield return StartCoroutine(EnemySummon());
                    yield return StartCoroutine(nextPhaseCoroutine());                      //ATTACK PHASE       
                }
                else if (turnPhase == TurnPhase.ATTACK)
                {
                    yield return StartCoroutine(EnemyAttack());
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
    IEnumerator EnemySummon()
    {
        yield return new WaitForSeconds(1f);
        tryPlayCard(chooseRandomCardFromHand(enemyHand), enemyHand, ui.enemyMonLane1);
        yield return new WaitForSeconds(0.5f);
        tryPlayCard(chooseRandomCardFromHand(enemyHand), enemyHand, ui.enemyMonLane2);
        yield return new WaitForSeconds(0.5f);
        tryPlayCard(chooseRandomCardFromHand(enemyHand), enemyHand, ui.enemyTrapLane);
        yield return new WaitForSeconds(1f);
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
