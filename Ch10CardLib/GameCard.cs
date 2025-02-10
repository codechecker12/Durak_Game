/*
    Project: Durak Game made by Group3
    Description: This project contains classes representing a card game.
    Authors: Dhyey Patel, Bhavya Detroja, Pratham Patel, Dawa Sherpa
    Date: 6th April 2024
*/

using System.Drawing; // Importing the System.Drawing namespace for image handling

namespace Ch10CardLib
{
    // GameCard class represents a single playing card in the game
    public class GameCard
    {
        // Card's suit
        public GameSuit suit;

        // Card's rank
        public GameRank rank;

        // Card's value
        public readonly int value;

        // Static variable for the trump suit
        public static GameSuit trump;

        // Static variable for the trump rank
        public static GameRank trumpRank;

        // Whether the card is face up or not
        protected bool faceUp = true;

        // Property to get or set the face up status of the card
        public bool FaceUp
        {
            get { return faceUp; }
            set { faceUp = value; }
        }

        // Constructor to initialize a card with suit, rank, and value
        public GameCard(GameSuit newSuit, GameRank newRank, int newValue)
        {
            suit = newSuit;
            rank = newRank;
            value = newValue;
        }

        // Copy constructor to create a card with the same attributes as another card
        public GameCard(GameCard card)
        {
            suit = card.suit;
            rank = card.rank;
            value = card.value;
        }

        // Default constructor
        public GameCard() { }

        // Override ToString() method to represent the card as a string
        public override string ToString()
        {
            return "The " + rank + " of " + suit;
        }

        // Method to get the image of the card
        public Image GetCardImage()
        {
            string imageName;
            Image cardImage;

            // Determine the image name based on whether the card is face up or not
            if (!faceUp)
            {
                // If the card is face down, use a back image
                imageName = "purple_back";
            }
            else
            {
                // If the card is face up, use the appropriate suit and rank image
                imageName = suit.ToString() + "_" + rank.ToString();
            }

            // Retrieve the image from the resources using the image name
            cardImage = Properties.Resources.ResourceManager.GetObject(imageName) as Image;
            return cardImage;
        }
    }
}
