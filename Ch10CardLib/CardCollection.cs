/*
    Project: Durak Game made by Group3
    Description: This project contains classes representing a card game.
    Authors: Dhyey Patel, Bhavya Detroja, Pratham Patel, Dawa Sherpa
    Date: 6th April 2024
*/

using System; 
using System.Collections; 
using System.Text; 

namespace Ch10CardLib
{
    // GameField class represents the field where cards are played and discarded
    public class GameField
    {
        private ArrayList field = new ArrayList(); // ArrayList to hold cards on the field
        private ArrayList discard = new ArrayList(); // ArrayList to hold discarded cards

        // GameField constructor
        public GameField()
        {
        }

        // Method to get cards currently on the field
        public ArrayList getField()
        {
            ArrayList fieldCards = new ArrayList();

            if (field.Count > 0)
            {
                for (int i = 0; i < field.Count; i++)
                {
                    fieldCards.Add((GameCard)field[i]);
                }
            }
            else
            {
                Console.WriteLine("Field is empty.");
            }
            return fieldCards;
        }

        // Method to add a card to the field
        public void cardPlayed(GameCard card)
        {
            field.Add(card);
        }

        // Method to get the current card on the field
        public GameCard getCurrentCard()
        {
            return ((GameCard)field[field.Count - 1]);
        }

        // Method to get a card from the field by index
        public GameCard getIndexCard(int index)
        {
            return ((GameCard)field[index]);
        }

        // Method to get discarded cards
        public ArrayList getDiscard()
        {
            ArrayList discardCards = new ArrayList();
            if (discard.Count > 0)
            {
                for (int i = 0; i < discard.Count; i++)
                {
                    discardCards.Add((GameCard)discard[i]);
                }
            }
            else
            {
                Console.WriteLine("discard is empty.");
            }
            return discardCards;
        }

        // Method to discard all cards from the field
        public void discardField()
        {
            if (field.Count > 0)
            {
                for (int i = 0; i < field.Count; i++)
                {
                    discard.Add((GameCard)field[i]);
                }
            }
            else
            {
                Console.WriteLine("Field is empty.");
            }
            field.Clear();
        }

        // Method to pick up all cards from the field
        public ArrayList pickupField()
        {
            ArrayList pickupCards = new ArrayList();
            pickupCards = getField();
            field.Clear();
            return pickupCards;
        }

        // Method to display the cards currently on the field
        public void displayField()
        {
            for (int i = 0; i < this.getField().Count; i++)
            {
                GameCard tempCard = (GameCard)this.getField()[i];
                Console.Write(tempCard.ToString());
                if (i != this.getField().Count - 1)
                {
                    Console.Write(", ");
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }

        // Method to display the discarded cards
        public void displayDiscarded()
        {
            for (int i = 0; i < this.getDiscard().Count; i++)
            {
                GameCard tempCard = (GameCard)this.getDiscard()[i];
                Console.Write(tempCard.ToString());
                if (i != this.getDiscard().Count - 1)
                {
                    Console.Write(", ");
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }
    }

    // GameHand class represents a player's hand of cards
    public class GameHand
    {
        public static int defaultHandSize = 6; // Default size of a player's hand
        private ArrayList hand = new ArrayList(defaultHandSize); // ArrayList to hold cards in the hand

        // GameHand constructor
        public GameHand(GameDeck deck)
        {
            // Draw cards from the deck to fill the hand
            for (int i = 0; i < defaultHandSize; i++)
            {
                hand.Add(deck.drawCard());
            }
        }

        private GameHand()
        {
        }

        // Method to get a card from the hand by index
        public GameCard GetCard(int cardNum)
        {
            if (cardNum >= 0 && cardNum <= hand.Count - 1)
            {
                return (GameCard)hand[cardNum];
            }
            else
            {
                throw new ArgumentOutOfRangeException("cardNum", cardNum, "Value must be between 0 and hand size - 1");
            }
        }

        // Method to get the size of the hand
        public int gethandSize()
        {
            return hand.Count;
        }

        // Method to play a card from the hand
        public GameCard playCard(int cardNum)
        {
            if (cardNum >= 0 && cardNum <= hand.Count - 1)
            {
                GameCard playedCard = (GameCard)hand[cardNum];
                hand.RemoveAt(cardNum);
                return playedCard;
            }
            else
            {
                throw new ArgumentOutOfRangeException("cardNum", cardNum, "Value must be between 0 and hand size - 1");
            }
        }

        // Method to select a card from the hand without removing it
        public GameCard selectCard(int cardNum)
        {
            if (cardNum >= 0 && cardNum <= hand.Count - 1)
            {
                return (GameCard)hand[cardNum];
            }
            else
            {
                throw new ArgumentOutOfRangeException("cardNum", cardNum, "Value must be between 0 and hand size - 1");
            }
        }

        // Method to remove all cards from the hand
        public void removeHand()
        {
            hand.Clear();
        }

        // Method to add a card to the hand
        public void addCard(GameCard card)
        {
            hand.Add(card);
        }

        // Method to display the cards in the hand
        public void displayHand()
        {
            for (int i = 0; i < this.gethandSize(); i++)
            {
                GameCard tempCard = this.GetCard(i);
                Console.Write(tempCard.ToString());
                Console.Write((i != this.gethandSize() - 1) ? ", " : "\n");
            }
        }

        // Override ToString() to provide a string representation of the hand
        public override string ToString()
        {
            StringBuilder tempString = new StringBuilder();
            for (int i = 0; i < this.gethandSize(); i++)
            {
                GameCard tempCard = this.GetCard(i);
                tempString.Append(" " + tempCard.ToString());
            }
            return tempString.ToString();
        }

        // Method to get the minimum ranked card of a specific suit from the hand
        public GameCard getMinOfSuite(GameCard gameCard)
        {
            int minOfSuite = -1;
            GameCard minCard = null;

            for (int i = 0; i < this.gethandSize(); i++)
            {
                if (gameCard.suit == this.GetCard(i).suit)
                {
                    if (minOfSuite == -1)
                    {
                        minOfSuite = (int)this.GetCard(i).rank;
                        minCard = this.GetCard(i);
                    }
                    else if (minOfSuite > (int)this.GetCard(i).rank)
                    {
                        minOfSuite = (int)this.GetCard(i).rank;
                        minCard = this.GetCard(i);
                    }
                }
            }
            return minCard;
        }

        // Method to get the maximum ranked card of a specific suit from the hand
        public GameCard getMaxOfSuite(GameSuit suit)
        {
            int maxOfSuite = -1;
            GameCard maxCard = null;

            for (int i = 0; i < this.gethandSize(); i++)
            {
                if (suit == this.GetCard(i).suit)
                {
                    if (maxOfSuite == -1)
                    {
                        maxOfSuite = (int)this.GetCard(i).rank;
                        maxCard = this.GetCard(i);
                    }
                    else if (maxOfSuite < (int)this.GetCard(i).rank)
                    {
                        maxOfSuite = (int)this.GetCard(i).rank;
                        maxCard = this.GetCard(i);
                    }
                }
            }
            return maxCard;
        }
    }

    // GameDeck class represents a deck of cards
    public class GameDeck
    {
        public static int cardsInDeck = 36; // Total number of cards in the deck
        private ArrayList cards = new ArrayList(cardsInDeck); // ArrayList to hold cards in the deck

        // GameDeck constructor to initialize the deck with cards
        public GameDeck()
        {
            int cardValue = 1;
            int initialValue = 1;
            for (int suitVal = 0; suitVal < 4; suitVal++)
            {
                cardValue = initialValue;
                initialValue++;
                for (int rankVal = 1; rankVal < 10; rankVal++)
                {
                    cards.Add(new GameCard((GameSuit)suitVal, (GameRank)rankVal, cardValue));
                    cardValue = cardValue + 4;
                }
            }
        }

        // Method to get a card from the deck by index
        public GameCard GetCard(int cardNum)
        {
            if (cardNum >= 0 && cardNum <= cardsInDeck - 1)
            {
                return (GameCard)cards[cardNum];
            }
            else
            {
                throw (new System.ArgumentOutOfRangeException("cardNum", cardNum, "Value must be between 0 and 36."));
            }
        }

        // Method to shuffle the deck
        public void Shuffle()
        {
            GameCard[] newDeck = new GameCard[cardsInDeck];
            bool[] assigned = new bool[cardsInDeck];
            Random sourceGen = new Random();

            for (int i = 0; i < cardsInDeck; i++)
            {
                int destCard = 0;
                bool foundCard = false;
                while (foundCard == false)
                {
                    destCard = sourceGen.Next(cardsInDeck);
                    if (assigned[destCard] == false)
                    {
                        foundCard = true;
                    }
                }
                assigned[destCard] = true;
                newDeck[destCard] = (GameCard)cards[i];
            }
            cards = new ArrayList(newDeck);
        }

        // Method to get the number of cards remaining in the deck
        public int getCardsRemaining()
        {
            int cardsRemaining = cards.Count;
            return cardsRemaining;
        }

        // Method to get the trump card from the deck
        public GameCard getTrumpcard()
        {
            GameCard trumpCard = new GameCard((GameCard)cards[0]);

            this.cards.RemoveAt(0);

            return trumpCard;
        }

        // Method to draw a card from the deck
        public GameCard drawCard()
        {
            GameCard drawnCard = new GameCard((GameCard)cards[cards.Count - 1]);

            cards.RemoveAt(cards.Count - 1);

            return drawnCard;
        }

        // Method to display the cards in the deck
        public void displayDeck(GameDeck myDeck)
        {
            for (int i = 0; i < myDeck.getCardsRemaining(); i++)
            {
                GameCard tempCard = myDeck.GetCard(i);
                Console.Write(tempCard.ToString());
                if (i != myDeck.getCardsRemaining() - 1)
                {
                    Console.Write(", ");
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }
    }
}
