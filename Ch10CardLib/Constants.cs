/*
    Project: Durak Game made by Group3
    Description: This project contains classes representing a card game.
    Authors: Dhyey Patel, Bhavya Detroja, Pratham Patel, Dawa Sherpa
    Date: 6th April 2024
*/

namespace Ch10CardLib
{
    // GameRank enum represents the ranks of playing cards in the game
    public enum GameRank
    {
        Six = 1,    // Rank Six
        Seven,      // Rank Seven
        Eight,      // Rank Eight
        Nine,       // Rank Nine
        Ten,        // Rank Ten
        Jack,       // Rank Jack
        Queen,      // Rank Queen
        King,       // Rank King
        Ace         // Rank Ace
    }

    // GameSuit enum represents the suits of playing cards in the game
    public enum GameSuit
    {
        Diamonds,   // Diamonds suit
        Clubs,      // Clubs suit
        Hearts,     // Hearts suit
        Spades      // Spades suit
    }
}
