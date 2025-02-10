/*
    Project: Durak Game made by Group3
    Description: This project contains classes representing a card game.
    Authors: Dhyey Patel, Bhavya Detroja, Pratham Patel, Dawa Sherpa
    Date: 6th April 2024
*/

using System;
using System.IO;

namespace Ch10CardLib
{
    // Player class represents a player in the game
    public class Player
    {
        // Player's name
        public string playerName { get; set; }

        // Player's hand of cards
        public GameHand playerHand { get; set; }

        // Constructor to initialize the player with a name
        public Player(string name)
        {
            playerName = name;
        }

        // Method to draw cards from the deck
        public void DrawCards(GameDeck myDeck)
        {
            bool attackerDraw = true;
            string filePath = @"../../GameLog.txt";
            string tempString = "";

            // Continue drawing cards until the attacker's hand size is less than 6
            while (attackerDraw)
            {
                // Check if there are cards remaining in the deck
                if (myDeck.getCardsRemaining() > 0)
                {
                    int attackerHandSize = this.playerHand.gethandSize();
                    // Draw a card if the attacker's hand size is less than 6
                    if (attackerHandSize < 6)
                    {
                        GameCard drawnCard = myDeck.drawCard();
                        tempString += " " + drawnCard.ToString();
                        this.playerHand.addCard(drawnCard);
                    }
                    else
                    {
                        attackerDraw = false; // Stop drawing if hand size reaches 6
                    }
                }
                else
                {
                    attackerDraw = false; // Stop drawing if no cards remaining in the deck
                }
            }
            // Write drawn cards to the game log file
            if (tempString != "")
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(this.playerName + " drew:" + tempString);
                }
            }
        }
    }

    // AI class represents an AI player in the game, inheriting from Player class
    public class AI : Player
    {
        // Constant for a skipped turn
        const int TURNSKIPPED = -1;

        // Constructor to initialize the AI with a name
        public AI(string name) : base(name)
        {
            playerName = name;
        }

        // Method to execute AI's turn based on the current round
        public int AITurnCycle(GameCard TrumpCard, GameField PlayingField, string round, bool perevodnoyFlag)
        {
            const string ATTACKINITIAL = "INITIALATTACK";
            const string ATTACKERTURN = "PLAYATTACKER";
            const string DEFENDERTURN = "PLAYDEFENDER";

            // Decide AI's action based on the current round
            if (round == ATTACKINITIAL)
            {
                return AIAttackerInitialTurn(TrumpCard);
            }
            else if (round == ATTACKERTURN)
            {
                return AIAttackerTurn(PlayingField, TrumpCard);
            }
            else if (round == DEFENDERTURN)
            {
                return AIDefenderTurn(PlayingField, TrumpCard, perevodnoyFlag);
            }
            else
            {
                return TURNSKIPPED; // Skip turn if round is unknown
            }
        }

        // Method for AI's initial attack turn
        public int AIAttackerInitialTurn(GameCard trumpCard)
        {
            GameCard selectedCard = null;

            // Find the highest card that is not a trump
            foreach (GameSuit suit in Enum.GetValues(typeof(GameSuit)))
            {
                if (suit != trumpCard.suit)
                {
                    GameCard gameCard = this.playerHand.getMaxOfSuite(suit);

                    if (gameCard != null)
                    {
                        if (selectedCard == null)
                        {
                            selectedCard = gameCard;
                        }
                        else if ((int)selectedCard.rank < (int)gameCard.rank)
                        {
                            selectedCard = gameCard;
                        }
                    }
                }
            }

            if (selectedCard == null)
            {
                // All cards in hand are of the trump suit, select the lowest trump card
                selectedCard = this.playerHand.getMinOfSuite(trumpCard);
            }

            // Find the index of selected card
            for (int i = 0; i < this.playerHand.gethandSize(); i++)
            {
                if (this.playerHand.GetCard(i) == selectedCard)
                {
                    return i;
                }
            }
            // Couldn't select any card
            return -1;
        }

        // Method for AI's attack turn
        public int AIAttackerTurn(GameField playingField, GameCard trumpCard)
        {
            // Find the highest card of the required suit
            GameCard selectedCard = this.playerHand.getMaxOfSuite(playingField.getCurrentCard().suit);

            if (selectedCard == null)
            {
                // No cards of the required suit, select the lowest trump card
                selectedCard = this.playerHand.getMinOfSuite(trumpCard);
            }
            else if ((int)selectedCard.rank < (int)playingField.getCurrentCard().rank)
            {
                // If the highest card is lower than the current card on the field, select the lowest trump card
                selectedCard = this.playerHand.getMinOfSuite(trumpCard);
            }

            // Find the index of selected card
            for (int i = 0; i < this.playerHand.gethandSize(); i++)
            {
                if (this.playerHand.GetCard(i) == selectedCard)
                {
                    return i;
                }
            }
            // Couldn't select any card
            return -1;
        }

        // Method for AI's defense turn
        public int AIDefenderTurn(GameField playingField, GameCard trumpCard, bool perevodnoyFlag)
        {
            // Find the highest card of the required suit
            GameCard selectedCard = this.playerHand.getMaxOfSuite(playingField.getCurrentCard().suit);

            if (selectedCard == null)
            {
                // No cards of the required suit, select the lowest trump card
                selectedCard = this.playerHand.getMinOfSuite(trumpCard);
            }
            else if ((int)selectedCard.rank < (int)playingField.getCurrentCard().rank)
            {
                // If the highest card is lower than the current card on the field, select the lowest trump card
                selectedCard = this.playerHand.getMinOfSuite(trumpCard);
            }

            // Find the index of selected card
            for (int i = 0; i < this.playerHand.gethandSize(); i++)
            {
                if (this.playerHand.GetCard(i) == selectedCard)
                {
                    return i;
                }
            }
            // Couldn't select any card
            return -1;
        }
    }
}
