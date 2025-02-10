/*
    Project: Durak Game made by Group3
    Description: This project contains classes representing a card game.
    Authors: Dhyey Patel, Bhavya Detroja, Pratham Patel, Dawa Sherpa
    Date: 6th April 2024
*/

using System; 
using System.Collections.Generic; 
using System.Collections; 
using System.Windows.Forms; 
using Ch10CardLib; 

namespace DurakFormApp
{
    // DiscardForm class represents a form for displaying the discarded cards
    public partial class DiscardForm : Form
    {
        // List to hold CardBox controls representing discarded cards
        public List<CardBox.CardBox> discardPile = new List<CardBox.CardBox>();

        // Static property to hold the game field
        public static GameField field { get; set; }

        // DiscardForm constructor
        public DiscardForm()
        {
            InitializeComponent();
        }

        // Method called when the DiscardForm is loaded
        private void frmDiscard_Load(object sender, EventArgs e)
        {
            DisplayDiscardPile();
        }

        // Method to display the discard pile on the form
        private void DisplayDiscardPile()
        {
            int counter = 0; // Counter for tracking the number of cards displayed
            int topOffset = 10; // Offset for the top position of each row of cards
            int displayCounter = 10; // Counter for controlling the horizontal position of each card

            // Get the discarded cards from the game field
            ArrayList discardedCards = field.getDiscard();

            // Create CardBox controls for each discarded card and add them to the discardPile list
            for (int i = 0; i < discardedCards.Count; i++)
            {
                CardBox.CardBox newCardBox = new CardBox.CardBox((GameCard)discardedCards[i]);
                discardPile.Add(newCardBox);
            }

            // Display the CardBox controls on the form
            for (int i = discardedCards.Count - 1; i >= 0; i--)
            {
                // Set the position of the CardBox control
                discardPile[i].Left = (displayCounter * 20) + 100;
                discardPile[i].Top = topOffset;

                // Increment counters
                counter++;
                displayCounter--;

                // Add the CardBox control to the form's panel
                this.pnDiscard.Controls.Add(discardPile[i]);

                // If 10 cards have been displayed in a row, adjust topOffset and reset counters
                if (counter > 9)
                {
                    topOffset = topOffset + 120;
                    counter = 0;
                    displayCounter = 10;
                }
            }
        }
    }
}
