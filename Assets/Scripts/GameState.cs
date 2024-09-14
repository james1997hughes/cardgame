using System.Collections.Generic;

public class GameState
{
    // Dumb AI to start - Plays immediate best move, doesn't look ahead

    public GamePlayer friendlyPlayer {get; set;}
    public GamePlayer enemyPlayer {get; set;}
    public float friendlyPlayerHealth {get; set;} //Just use friendlyPlayer.Health you idiot <- AI autocompleted that insult
    public float enemyPlayerHealth {get; set;} 

    public List<Card> friendlyCardsInHand {get; set;}
    public List<Card> enemyCardsInHand {get; set;}
    public Card friendlyMonLane1Card {get; set;}
    public Card friendlyMonLane2Card {get; set;}
    public Card enemyMonLane1Card {get; set;}
    public Card enemyMonLane2Card {get; set;}
    public float friendlySpellSlotCurrent {get; set;}
    public float enemySpellSlotCurrent {get; set;}







    public UI ui {get; set;}
    public GameController controller {get; set;}

    List<Move> playedMoves {get; set;}


    GameState copyGameState(){
        GameState copy = new GameState();
        copy.friendlyPlayer = friendlyPlayer;
        copy.enemyPlayer = enemyPlayer;
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
        if (move.moveType == MoveType.SUMMON){
            if (move.laneTarget == ui.monLane1){
                friendlyMonLane1Card = move.card;
            }
            else if (move.laneTarget == ui.monLane2){
                friendlyMonLane2Card = move.card;
            }
        }
        else if (move.moveType == MoveType.ATTACK_MONSTER){ // First 2 ifs are repeating from the controller, should be a function 
            if (move.card.MonAtk + move.card.StatModifiers["MonAtk"] >= move.cardTarget.Def + move.cardTarget.StatModifiers["Def"])
                {
                    move.cardTarget.HP -= move.card.MonAtk + move.card.StatModifiers["MonAtk"];
                }

            if (move.card.Def + move.card.StatModifiers["Def"] <= move.cardTarget.MonAtk + move.cardTarget.StatModifiers["MonAtk"])
                {
                    move.card.HP -= move.cardTarget.MonAtk + move.cardTarget.StatModifiers["MonAtk"];
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
            }
            else{
                enemyPlayerHealth -= move.card.MonAtk + move.card.StatModifiers["MonAtk"];
            }
        }

        //For spells, we only need to update the spellslots and any stat mods etc.

    }



    /*Move getBestMove(GamePlayer player, float depth){
        // Generate all possible current moves
        // for each possible current move, run a simulation of all possible best followup moves up to depth turns
        // if the current move creates a chain leading to a greater win chance, set this is the candidate
        // once all possible current moves have been analysed, return best move
        List<Move> possibleMoves = getPossibleMoves(player);
        Move bestMove = null;
        float bestMoveStrength = -1;

        foreach(Move move in possibleMoves){
            float moveStrength = calculateWinProbability(player, depth);
            if (moveStrength > bestMoveStrength){
                bestMoveStrength = moveStrength;
                bestMove = move;
            }
        }

        return bestMove;
    }*/

    public Move getImmediateBestMove(GamePlayer player){
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
        return bestMove;
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
        Card monLane1 = player == friendlyPlayer ? state.friendlyMonLane1Card : state.enemyMonLane1Card;
        Card monLane2 = player == friendlyPlayer ? state.friendlyMonLane2Card : state.enemyMonLane2Card;

        if (monLane1 != null)
        {
            strength += monLane1.HP + monLane1.MonAtk + monLane1.Def;
        }

        if (monLane2 != null)
        {
            strength += monLane2.HP + monLane2.MonAtk + monLane2.Def;
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

        Card enemyLane1Card = opponentPlayer.playerType == GamePlayer.PlayerType.PLAYER ? state.enemyMonLane1Card : state.friendlyMonLane1Card;
        Card enemyLane2Card = opponentPlayer.playerType == GamePlayer.PlayerType.PLAYER ? state.enemyMonLane2Card : state.friendlyMonLane2Card;
        //Lane enemyTrapLane = player == friendlyPlayer ? ui.enemyTrapLane : ui.trapLane;

        // 1. Playing cards from hand
        if (controller.turnPhase == GameController.TurnPhase.SUMMON){
            foreach (Card card in playableCards)
            {
                if (card.Cost <= (player == friendlyPlayer ? state.friendlySpellSlotCurrent : state.enemySpellSlotCurrent))
                {
                        // Monster cards can be played in empty lanes
                        if (player == friendlyPlayer ? state.friendlyMonLane1Card == null : state.enemyMonLane1Card == null){
                            possibleMoves.Add(new Move(MoveType.SUMMON, card, ownLane1Card, SourceLane.MON_LANE_1, player));
                        }
                        if (player == friendlyPlayer ? state.friendlyMonLane2Card == null : state.enemyMonLane2Card == null){
                            possibleMoves.Add(new Move(MoveType.SUMMON, card, ownLane2Card, SourceLane.MON_LANE_2, player));
                        }
                }
            }
        }

        // 2. Attacking with monsters in lanes
        if (controller.turnPhase == GameController.TurnPhase.ATTACK){
            if (ownLane1Card != null)
            {
                if (enemyLane1Card != null){
                    possibleMoves.Add(new Move(MoveType.ATTACK_MONSTER, ownLane1Card, enemyLane1Card, SourceLane.MON_LANE_1, player));
                }
                else if (enemyLane2Card != null){
                    possibleMoves.Add(new Move(MoveType.ATTACK_MONSTER, ownLane1Card, enemyLane2Card, SourceLane.MON_LANE_1, player));
                }
                else{
                    possibleMoves.Add(new Move(MoveType.ATTACK_PLAYER, ownLane1Card, opponentPlayer, SourceLane.MON_LANE_1, player));
                }
            }

            if (ownLane2Card != null)
            {
                if (enemyLane1Card != null){
                    possibleMoves.Add(new Move(MoveType.ATTACK_MONSTER, ownLane2Card, enemyLane1Card, SourceLane.MON_LANE_2, player));
                }
                else if (enemyLane2Card != null){
                    possibleMoves.Add(new Move(MoveType.ATTACK_MONSTER, ownLane2Card, enemyLane2Card, SourceLane.MON_LANE_2, player));
                }
                else{
                    possibleMoves.Add(new Move(MoveType.ATTACK_PLAYER, ownLane2Card, opponentPlayer, SourceLane.MON_LANE_2, player));
                }
            }
        }

        possibleMoves.Add(new Move(MoveType.PASS, player));

        return possibleMoves;

    }




}
