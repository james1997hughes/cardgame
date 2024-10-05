
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class GameState
{
    // Dumb AI to start - Plays immediate best move, doesn't look ahead

    public GamePlayer friendlyPlayer {get; set;}
    public GamePlayer enemyPlayer {get; set;}
    public float friendlyPlayerHealth {get; set;} //Just use friendlyPlayer.Health you idiot <- AI autocompleted that insult
    public float enemyPlayerHealth {get; set;} 

    public List<Card>? friendlyCardsInHand {get; set;}
    public List<Card>? enemyCardsInHand {get; set;}
    public Card? friendlyMonLane1Card {get; set;}
    public Card? friendlyMonLane2Card {get; set;}
    public Card? enemyMonLane1Card {get; set;}
    public Card? enemyMonLane2Card {get; set;}
    public float friendlySpellSlotCurrent {get; set;}
    public float enemySpellSlotCurrent {get; set;}

    public UI ui {get; set;}
    public GameController controller {get; set;}

    List<Move> playedMoves {get; set;}

    public GameState(GamePlayer player, GamePlayer enemy, GameController controller){
        this.controller = controller;
        ui = controller.ui;
        playedMoves = new List<Move>();
        friendlyPlayer = player;
        enemyPlayer = enemy;
        friendlyPlayerHealth = friendlyPlayer.Health;
        enemyPlayerHealth = enemyPlayer.Health;
        friendlyCardsInHand = friendlyPlayer.hand.cardsInHand;
        enemyCardsInHand = enemyPlayer.hand.cardsInHand;
        friendlyMonLane1Card = null;
        friendlyMonLane2Card = null;
        enemyMonLane1Card = null;
        enemyMonLane2Card = null;
    }   

    GameState copyGameState(){
        GameState copy = new GameState(friendlyPlayer, enemyPlayer, controller);
        copy.friendlyPlayerHealth = friendlyPlayerHealth;
        copy.enemyPlayerHealth = enemyPlayerHealth;
        copy.friendlyCardsInHand = friendlyCardsInHand;
        copy.enemyCardsInHand = enemyCardsInHand;
        copy.friendlyMonLane1Card = friendlyMonLane1Card;
        copy.friendlyMonLane2Card = friendlyMonLane2Card;
        copy.enemyMonLane1Card = enemyMonLane1Card;
        copy.enemyMonLane2Card = enemyMonLane2Card;
        copy.friendlySpellSlotCurrent = friendlySpellSlotCurrent;
        copy.enemySpellSlotCurrent = enemySpellSlotCurrent;

        return copy;
    }

    public void addMove(Move move){
        playedMoves.Add(move);
        updateStateWithMove(move);
    }
    public void updateGameStateFromController(){
        friendlyPlayer = controller.player;
        enemyPlayer = controller.enemy;
        friendlyPlayerHealth = friendlyPlayer.Health;
        enemyPlayerHealth = enemyPlayer.Health;
        friendlyCardsInHand = friendlyPlayer.hand.cardsInHand;
        enemyCardsInHand = enemyPlayer.hand.cardsInHand;
        friendlyMonLane1Card = controller.PlayerLane1Card;
        friendlyMonLane2Card = controller.PlayerLane2Card;
        enemyMonLane1Card = controller.EnemyLane1Card;
        enemyMonLane2Card = controller.EnemyLane2Card;
        friendlySpellSlotCurrent = friendlyPlayer.hand.spellSlotCurrent;
        enemySpellSlotCurrent = enemyPlayer.hand.spellSlotCurrent;
    }

    void updateStateWithMove(Move move){
        // Update game state variables with move
        // Emulates the behaviour of gameController.executeMove but on the state
        // Should be combined in the future to manage this easier
        if (move.moveType == MoveType.SUMMON){
            if (move.laneTarget == Lanes.MONSTER_LANE_1){
                if(move.player.playerType == GamePlayer.PlayerType.PLAYER && friendlyMonLane1Card == null){
                    friendlyMonLane1Card = move.card;
                }
                if(move.player.playerType == GamePlayer.PlayerType.ENEMY && enemyMonLane1Card == null){
                    enemyMonLane1Card = move.card;
                }
            }
            else if (move.laneTarget == Lanes.MONSTER_LANE_2){
                if(move.player.playerType == GamePlayer.PlayerType.PLAYER && friendlyMonLane2Card == null){
                    friendlyMonLane2Card = move.card;
                }
                if(move.player.playerType == GamePlayer.PlayerType.ENEMY && enemyMonLane2Card == null){
                    enemyMonLane2Card = move.card;
                }
            }
        }
        else if (move.moveType == MoveType.ATTACK_MONSTER){ // First 2 ifs are repeating from the controller, should be a function 
            if (move.card.MonAtk + move.card.StatModifiers["MonAtk"] >= move.cardTarget.Def + move.cardTarget.StatModifiers["Def"])
                {
                    move.cardTarget.HP -= move.card.MonAtk + move.card.StatModifiers["MonAtk"];
                    // This affects live cards
                }

            if (move.card.Def + move.card.StatModifiers["Def"] <= move.cardTarget.MonAtk + move.cardTarget.StatModifiers["MonAtk"])
                {
                    move.card.HP -= move.cardTarget.MonAtk + move.cardTarget.StatModifiers["MonAtk"];
                    // This affects live cards
                }
            if (move.card.HP <= 0){
                if (move.sourceLane == SourceLane.MON_LANE_1 && move.player.playerType == GamePlayer.PlayerType.PLAYER){
                    friendlyMonLane1Card = null;
                }
                else if (move.sourceLane == SourceLane.MON_LANE_1 && move.player.playerType == GamePlayer.PlayerType.ENEMY){
                    enemyMonLane1Card = null;
                }
                else if (move.sourceLane == SourceLane.MON_LANE_2 && move.player.playerType == GamePlayer.PlayerType.PLAYER){
                    friendlyMonLane2Card = null;
                }
                else if (move.sourceLane == SourceLane.MON_LANE_2 && move.player.playerType == GamePlayer.PlayerType.ENEMY){
                    enemyMonLane2Card = null;
                }
            }
            if (move.cardTarget.HP <= 0){
                if (move.sourceLane == SourceLane.MON_LANE_1 && move.player.playerType == GamePlayer.PlayerType.PLAYER){
                    enemyMonLane1Card = null;
                }
                else if (move.sourceLane == SourceLane.MON_LANE_1 && move.player.playerType == GamePlayer.PlayerType.ENEMY){
                    friendlyMonLane1Card = null;
                }
                else if (move.sourceLane == SourceLane.MON_LANE_2 && move.player.playerType == GamePlayer.PlayerType.PLAYER){
                    enemyMonLane2Card = null;
                }
                else if (move.sourceLane == SourceLane.MON_LANE_2 && move.player.playerType == GamePlayer.PlayerType.ENEMY){
                    friendlyMonLane2Card = null;
                }
            }
        }
        else if (move.moveType == MoveType.ATTACK_PLAYER){
            if (move.playerTarget.playerType == GamePlayer.PlayerType.PLAYER){
                friendlyPlayerHealth -= move.card.MonAtk + move.card.StatModifiers["MonAtk"];
                //This does not affect live player health
            }
            else{
                enemyPlayerHealth -= move.card.MonAtk + move.card.StatModifiers["MonAtk"];
                //This does not affect live player health
            }
        }
        else if (move.moveType == MoveType.SPELL){
            //More complex than it seems as we're holding references to the live cards, not copies
            // This is probably the cause of the high damage bug.

            //Doing the following affects the live cards as we are not creating a deep copy of cards in the state
            //move.card.SpellEffect(move.cardTarget);

            // Next time I need to create deep copy logic for the cards
            // Making sure that all references to cards in GameState are to the GameState's own (copied) cards
        }

    }

    public IEnumerator<Move> getImmediateBestMove(GamePlayer player, System.Action<Move> callback){
        // For all possible current moves, return move with highest strength outcome
        // To make recursive, recurse this function up to a depth, then finally call evaluateGameState
        // Keeping track of the best overall move after depth recursions 

        List<Move> possibleMoves = getPossibleMoves(player, this);
        Move bestMove = null;
        float bestMoveStrengthDiff = -999f;
        foreach (Move move in possibleMoves){
            GameState copyState = copyGameState();
            copyState.addMove(move); // Not necessary. For future recursion
            copyState.updateStateWithMove(move);
            float moveStrengthDiff = evaluateGameState(player, copyState);
            if (moveStrengthDiff > bestMoveStrengthDiff || bestMove == null){
                bestMoveStrengthDiff = moveStrengthDiff;
                bestMove = move;
            }
        }
        yield return null;
        callback(bestMove);
    }


    float evaluateGameState(GamePlayer player, GameState state)
    {
        // Returns player game strength relative to enemy game strength
        float friendlyStrength = calculatePlayerStrength(friendlyPlayer, state);
        float enemyStrength = calculatePlayerStrength(enemyPlayer, state);

        if (player == friendlyPlayer)
        {
            return friendlyStrength - enemyStrength;
        }
        else
        {
            return enemyStrength - friendlyStrength;
        }
    }

    float calculatePlayerStrength(GamePlayer player, GameState state)
    {
        // Returns value representing HP + cards in hand + cards in lanes

        float strength = player == friendlyPlayer ? state.friendlyPlayerHealth : state.enemyPlayerHealth;
        List<Card> playerCardsInHand = player == friendlyPlayer ? state.friendlyCardsInHand : state.enemyCardsInHand;

        foreach (Card card in playerCardsInHand)
        {
            strength += card.Cost * 0.5f;
        }

        // Add strength for cards in lanes
        strength += calculateLaneStrength(player, state);

        return strength;
    }

    float calculateLaneStrength(GamePlayer player, GameState state)
    {
        //sum up a value for total card strength in lanes
        float strength = 0;

        // Reference to the relevant UI lanes based on the player
        Card? monLane1 = player == friendlyPlayer ? state.friendlyMonLane1Card : state.enemyMonLane1Card;
        Card? monLane2 = player == friendlyPlayer ? state.friendlyMonLane2Card : state.enemyMonLane2Card;

        if (monLane1 != null)
        {
            strength += monLane1.HP + monLane1.StatModifiers["HP"]
                        + monLane1.MonAtk + monLane1.StatModifiers["MonAtk"]
                        + monLane1.PlayerAtk + monLane1.StatModifiers["PlayerAtk"]
                        + monLane1.Def + monLane1.StatModifiers["Def"];
        }

        if (monLane2 != null)
        {
            strength += monLane2.HP + monLane2.StatModifiers["HP"]
                        + monLane2.MonAtk + monLane2.StatModifiers["MonAtk"]
                        + monLane2.PlayerAtk + monLane2.StatModifiers["PlayerAtk"]
                        + monLane2.Def + monLane2.StatModifiers["Def"];
        }

        return strength;
    }

    List<Move> getPossibleMoves(GamePlayer player, GameState state){
        // TODO: Add trap cards, spell cards
        List<Move> possibleMoves = new List<Move>();
        List<Card> playableCards = player.playerType == GamePlayer.PlayerType.PLAYER ? state.friendlyCardsInHand : state.enemyCardsInHand;

        GamePlayer opponentPlayer = player.playerType == GamePlayer.PlayerType.PLAYER ? state.enemyPlayer : state.friendlyPlayer;
        Card ownLane1Card = player.playerType == GamePlayer.PlayerType.PLAYER ? state.friendlyMonLane1Card : state.enemyMonLane1Card;
        Card ownLane2Card = player.playerType == GamePlayer.PlayerType.PLAYER ? state.friendlyMonLane2Card : state.enemyMonLane2Card;
        //Lane ownTrapLane = player == friendlyPlayer ? ui.trapLane : ui.enemyTrapLane;

        Card opponentLane1Card = player.playerType == GamePlayer.PlayerType.PLAYER ? state.enemyMonLane1Card : state.friendlyMonLane1Card;
        Card opponentLane2Card = player.playerType == GamePlayer.PlayerType.PLAYER ? state.enemyMonLane2Card : state.friendlyMonLane2Card;
        //Lane enemyTrapLane = player == friendlyPlayer ? ui.enemyTrapLane : ui.trapLane;

        // 1. Playing cards from hand
        if (controller.turnPhase == GameController.TurnPhase.SUMMON && playableCards.Count > 0){
            foreach (Card card in playableCards)
            {
                if (card.Cost <= (player == friendlyPlayer ? state.friendlySpellSlotCurrent : state.enemySpellSlotCurrent) && card.isMonster)
                {
                        // Monster cards can be played in empty lanes
                        if (player == friendlyPlayer){
                            if (state.friendlyMonLane1Card == null){
                                possibleMoves.Add(new Move(MoveType.SUMMON, card, Lanes.MONSTER_LANE_1, player));
                            }
                            if (state.friendlyMonLane2Card == null){
                                possibleMoves.Add(new Move(MoveType.SUMMON, card, Lanes.MONSTER_LANE_2, player));
                            }
                        }
                        else if (player == enemyPlayer){
                            if (state.enemyMonLane1Card == null){
                                possibleMoves.Add(new Move(MoveType.SUMMON, card, Lanes.MONSTER_LANE_1, player));
                            }
                            if (state.enemyMonLane2Card == null){
                                possibleMoves.Add(new Move(MoveType.SUMMON, card, Lanes.MONSTER_LANE_2, player));
                            }
                        }
                }
            }
        }

        // 2. Attacking with monsters in lanes
        if (controller.turnPhase == GameController.TurnPhase.ATTACK){
            if (ownLane1Card != null)
            {
                if (opponentLane1Card != null){
                    possibleMoves.Add(new Move(MoveType.ATTACK_MONSTER, ownLane1Card, opponentLane1Card, SourceLane.MON_LANE_1, player));
                }
                else if (opponentLane2Card != null){
                    possibleMoves.Add(new Move(MoveType.ATTACK_MONSTER, ownLane1Card, opponentLane2Card, SourceLane.MON_LANE_1, player));
                }
                else{
                    possibleMoves.Add(new Move(MoveType.ATTACK_PLAYER, ownLane1Card, opponentPlayer, SourceLane.MON_LANE_1, player));
                }
            }

            if (ownLane2Card != null)
            {
                if (opponentLane1Card != null){
                    possibleMoves.Add(new Move(MoveType.ATTACK_MONSTER, ownLane2Card, opponentLane1Card, SourceLane.MON_LANE_2, player));
                }
                else if (opponentLane2Card != null){
                    possibleMoves.Add(new Move(MoveType.ATTACK_MONSTER, ownLane2Card, opponentLane2Card, SourceLane.MON_LANE_2, player));
                }
                else{
                    possibleMoves.Add(new Move(MoveType.ATTACK_PLAYER, ownLane2Card, opponentPlayer, SourceLane.MON_LANE_2, player));
                }
            }
        }

        // 3. Use spells
        if (controller.turnPhase == GameController.TurnPhase.ATTACK || controller.turnPhase == GameController.TurnPhase.SUMMON && playableCards.Count > 0){
            foreach (Card card in playableCards) {
                if (card.isSpell && card.Cost <= (player == friendlyPlayer ? state.friendlySpellSlotCurrent : state.enemySpellSlotCurrent)){
                    if (ownLane1Card != null){
                        possibleMoves.Add(new Move(MoveType.SPELL, card, ownLane1Card, player));
                    }
                    if (ownLane2Card != null){
                        possibleMoves.Add(new Move(MoveType.SPELL, card, ownLane2Card, player));
                    }
                }
            }
        }

        possibleMoves.Add(new Move(MoveType.PASS, player));

        return possibleMoves;

    }




}
