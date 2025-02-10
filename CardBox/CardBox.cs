/*
    Project: Durak Game made by Group3
    Description: This project contains classes representing a card game.
    Authors: Dhyey Patel, Bhavya Detroja, Pratham Patel, Dawa Sherpa
    Date: 6th April 2024
*/

using System; 
using System.Drawing; 
using System.Windows.Forms; 
using Ch10CardLib;

namespace CardBox
{
    // CardBox class represents a custom user control for displaying a playing card
    public partial class CardBox : UserControl
    {
        // Declaration of private fields
        private GameCard myCard; // Instance of the GameCard class representing the card
        private Orientation myOrientation; // Orientation of the card (Vertical or Horizontal)

        // Event handler for click event
        new public event EventHandler Click;

        // Event handler for card flipped event
        public event EventHandler CardFlipped;

        // Property for setting and getting the rank of the card
        public GameRank rank
        {
            set
            {
                Card.rank = value;
                UpdateCardImage();
            }
            get { return Card.rank; }
        }

        // CardBox constructor
        public CardBox()
        {
            InitializeComponent();
            myOrientation = Orientation.Vertical;
            myCard = new GameCard();
            UpdateCardImage();
        }

        // CardBox constructor with parameters
        public CardBox(GameCard card, Orientation orientation = Orientation.Vertical)
        {
            InitializeComponent();
            myOrientation = orientation;
            myCard = card;
        }

        // Method called when the CardBox control is loaded
        private void CardBox_Load(object sender, EventArgs e)
        {
            UpdateCardImage();
        }

        // Method to handle click event on the picture box
        private void pbMyPictureBox_Click(object sender, EventArgs e)
        {
            if (Click != null)
                Click(this, e);
        }

        // Property for setting and getting the face-up state of the card
        public bool FaceUp
        {
            set
            {
                if (myCard.FaceUp != value)
                {
                    myCard.FaceUp = value;
                    UpdateCardImage();

                    if (CardFlipped != null)
                        CardFlipped(this, new EventArgs());
                }
            }
            get { return Card.FaceUp; }
        }

        // Property for setting and getting the card
        public GameCard Card
        {
            set
            {
                myCard = value;
                UpdateCardImage();
            }
            get { return myCard; }
        }

        // Property for setting and getting the suit of the card
        public GameSuit Suit
        {
            set
            {
                Card.suit = value;
                UpdateCardImage();
            }
            get { return Card.suit; }
        }

        // Property for setting and getting the orientation of the card
        public Orientation CardOrientation
        {
            set
            {
                if (myOrientation != value)
                {
                    myOrientation = value;
                    this.Size = new Size(Size.Height, Size.Width);
                    UpdateCardImage();
                }
            }
            get { return myOrientation; }
        }

        // Method to update the card image based on its properties
        private void UpdateCardImage()
        {
            pbMyPictureBox.Image = myCard.GetCardImage();

            if (myOrientation == Orientation.Horizontal)
            {
                pbMyPictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
        }
    }
}
