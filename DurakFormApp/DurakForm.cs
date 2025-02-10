/*
    Project: Durak Game made by Group3
    Description: This project contains classes representing a card game.
    Authors: Dhyey Patel, Bhavya Detroja, Pratham Patel, Dawa Sherpa
    Date: 6th April 2024
*/

// Shuffling cards using Durstenfeld's shuffle algorithm
// Reference: Durstenfeld, R. "Algorithm 235: Random permutation," Communications of the ACM, 1964.

// Initializing standard Durak game rules
// Reference: Parlett, David. "The Oxford Guide to Card Games," Oxford University Press, 1990.

// Using LINQ to query cards in a hand
// Reference: Albahari, Joseph and Ben. "C# 7.0 in a Nutshell: The Definitive Reference," O'Reilly Media, 2017.

// Designing the main game window layout
// Reference: Schell, Jesse. "The Art of Game Design: A Book of Lenses," CRC Press, 2008.

// Configuring ASP.NET Core services and middleware
// Reference: Freeman, Adam. "Pro ASP.NET Core 3," Apress, 2019.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using Ch10CardLib;
using System.IO;

namespace DurakFormApp
{
    public partial class DurakForm : Form
    {

        private string gameRules = @"1. A deck comprising 36 cards with the following denominations: 6, 7, 8, 9, 10, J, Q, K, A. (The deck is randomized prior to gameplay.)
2. Each player receives a hand consisting of six cards.
3. One card is drawn from the bottom of the deck, revealed to all players, and designated as the trump suit for the current match. This card is then removed from play.
4. The player holding the lowest-valued card in their hand becomes the first attacker.
5. The attacking player initiates the round by playing a card from their hand.
6. The defending player may opt to counter the attack by playing a card that meets one of the following criteria:
   a. Same suit but higher value.
   b. A card from the trump suit, regardless of value.
   c. A card of the same rank to exchange roles of attacker and defender, marking the completion of a round. This option may be exercised up to three times.
7. Upon successfully defending against an attack, the attacker may launch subsequent attacks using cards of values present on the field, up to a maximum of six attacks per round.
8. If the defending player is unable or unwilling to defend against the attack, they must pick up all cards involved in the attack.
9. Both players replenish their hands to a minimum of six cards from the deck, with the attacker drawing first in case of insufficient cards.
10. Upon successful defense or exhaustion of possible attacks, all cards involved in the attack are discarded, and a new round commences.
11. The attacker retains their role if successful; otherwise, the defender becomes the new attacker for the subsequent round.
12. Rounds persist until the deck is depleted.
13. After the deck is emptied, gameplay continues without card draws.
14. Players exit the game upon depletion of their cards or the exhaustion of the deck. The player with cards remaining at the end loses.
15. Each player maintains their individual log file and can reset statistics via the provided radio button.";

        private List<CardBox.CardBox> fieldCards = new List<CardBox.CardBox>();
        private GameDeck gameDeck = new GameDeck();
        private int indexOfPlayerCard = 0;
        private bool aiHandVisible = false;
        private int offsetPlayer = 215;
        private int offsetAI = 215;
        private AI aiPlayer = new AI("AI");
        private Player humanPlayer;
        private Player attackingPlayer;
        private Player defendingPlayer;
        private Player currPlayer;
        private List<CardBox.CardBox> playerCards = new List<CardBox.CardBox>();
        private List<CardBox.CardBox> aiCards = new List<CardBox.CardBox>();
        private bool gameEnder = false;
        private bool flag = false;
        private int countTurns = 0;
        private GameField gamePlayingField = new GameField();
        private GameCard cardTrump;
        private const string INITIALATTACK = "INITIALATTACK";
        private const string PLAYATTACKER = "PLAYATTACKER";
        private const string PLAYDEFENDER = "PLAYDEFENDER";
        private string currRound = INITIALATTACK;

        private int numGamesPlayed = 0;
        private int wonGames = 0;
        private int lostGames = 0;

        private int indexOfAICard = 0;
        private bool flagMatcher = false;
        private int fieldOffset = 0;
        private DateTime currDate = DateTime.Now;
        private const int ATTACKMAX = 6;
        private bool gameEnd = false;
        private bool skipped = true;



        // When the game form is created
        public DurakForm()
        {
            InitializeComponent(); // Set up the form and its components

            btnSkipTurn.Enabled = false; // The "Skip Turn" button is disabled initially

            // Log the start of the application and the game's start time
            gameLogWriter("Application has started" + "\n" + "Game Started At: " + currDate.ToString("F"));
        }

        // Method to create controls for the game
        private void ControlCreator()
        {
            // Create card boxes for the human player's hand
            for (int i = 0; i < humanPlayer.playerHand.gethandSize(); i++)
            {
                // Create a card box representing a card in the player's hand
                CardBox.CardBox newCardBox = new CardBox.CardBox(humanPlayer.playerHand.GetCard(i));

                // Add functionality when the card box is clicked
                newCardBox.Click += CardBox_Click;

                // Store the card box in the player's collection
                playerCards.Add(newCardBox);
            }

            // Create card boxes for the AI player's hand
            for (int i = 0; i < aiPlayer.playerHand.gethandSize(); i++)
            {
                // Create a card box representing a card in the AI player's hand
                CardBox.CardBox newCardBox = new CardBox.CardBox(aiPlayer.playerHand.GetCard(i));

                // Add the card box to the AI player's collection
                aiCards.Add(newCardBox);
            }

            // Display the number of cards remaining in the game deck
            lblDeckSizeValue.Text = gameDeck.getCardsRemaining().ToString();
        }

        // Method to display controls for the players' hands
        private void ControlDisplayer()
        {
            // Adjust offset for the player's hand based on its size
            offsetPlayer = offsetPlayer - (humanPlayer.playerHand.gethandSize() - 6) * 20 / 100;

            // Adjust offset for the AI player's hand based on its size
            offsetAI = offsetAI - (aiPlayer.playerHand.gethandSize() - 6) * 20 / 100;

            // Display card boxes for the human player's hand
            for (int i = humanPlayer.playerHand.gethandSize() - 1; i >= 0; i--)
            {
                // Position the card box horizontally based on index and offset
                playerCards[i].Left = (i * 20) + offsetPlayer;

                // Add the card box to the player's hand panel
                this.pnPlayerHand.Controls.Add(playerCards[i]);
            }

            // Display card boxes for the AI player's hand if there are any
            if (aiCards.Count > 0)
            {
                for (int i = aiPlayer.playerHand.gethandSize() - 1; i >= 0; i--)
                {
                    // Position the card box horizontally based on index and offset
                    aiCards[i].Left = (i * 20) + offsetAI;

                    // Set the visibility of the AI player's hand
                    aiCards[i].FaceUp = aiHandVisible;

                    // Add the card box to the AI player's hand panel
                    this.pnAIHand.Controls.Add(aiCards[i]);
                }
            }
        }


        // Method to handle the click event of a card box
        void CardBox_Click(object sender, EventArgs e)
        {
            // Get the clicked card box
            CardBox.CardBox aCardBox = sender as CardBox.CardBox;

            // Check if the card box exists
            if (aCardBox != null)
            {
                // Find the index of the clicked card in the human player's hand
                for (int i = 0; i < humanPlayer.playerHand.gethandSize(); i++)
                {
                    if (aCardBox.Card == humanPlayer.playerHand.GetCard(i))
                    {
                        indexOfPlayerCard = i;
                    }
                }

                // Display the selected card in a label
                lblCardSelected.Text = aCardBox.Card.ToString();
            }
        }

        // Method to toggle visibility of the AI player's hand
        private void chkAIHandToggle_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle the visibility of the AI player's hand
            aiHandVisible = !aiHandVisible;

            // Update the display of player and AI hands
            ControlDisplayer();
        }


        private void lblPlayerTurn_TextChanged(object sender, EventArgs e)
        {

        }

        // Method to write game logs
        public void gameLogWriter(string msg)
        {
            // Construct the file path for game logs
            string filePath = "../../logs/GameLogs/" + currDate.ToString("yyyy-M-dd--HH-mm-ss") + "-GameLog.txt";

            // Write the message to the file
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(msg);
            }
        }

        // Method to write player statistics logs
        public void statLogWriter(string msg)
        {
            // Construct the file path for player statistics logs
            string path = "../../logs/PlayerStats/" + humanPlayer.playerName + "-StatsLog.txt";

            // Clear the contents of the file (if any)
            File.WriteAllText(path, "");

            // Write the message to the file
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(msg);
            }
        }


        // Method to handle the click event when the "Start" button is clicked
        private void btnStart_Click(object sender, EventArgs e)
        {
            // Initialize game components and variables
            gameDeck = new GameDeck();
            countTurns = 0;
            gameEnder = false;
            playerCards = new List<CardBox.CardBox>();
            aiCards = new List<CardBox.CardBox>();
            fieldCards = new List<CardBox.CardBox>();
            currRound = INITIALATTACK;
            flagMatcher = false;
            pnPlayerHand.Controls.Clear();
            pnAIHand.Controls.Clear();
            playerCards.Clear();
            gamePlayingField = new GameField();
            pnPlayingField.Controls.Clear();

            // Enable AI hand toggle
            chkAIHandToggle.Enabled = true;

            // Reset game end flag
            gameEnd = false;

            // Reset offsets for player and AI hands
            offsetPlayer = 215;
            offsetAI = 215;

            // Reset game statistics
            numGamesPlayed = 0;
            wonGames = 0;
            lostGames = 0;

            // Hide reset stats button
            btnResetStats.Visible = false;

            // Shuffle the game deck
            gameDeck.Shuffle();

            // Create a new player with a default name if no name is provided
            if (String.IsNullOrEmpty(txtNameInput.Text))
            {
                humanPlayer = new Player("Player1");
            }
            else
            {
                humanPlayer = new Player(txtNameInput.Text.Trim());
                humanPlayer.playerName = humanPlayer.playerName.Replace(" ", "");
            }

            // Read previous statistics logs
            statLogReader();

            // Increment number of games played
            numGamesPlayed += 1;

            // Deal hands to players
            humanPlayer.playerHand = new GameHand(gameDeck);
            aiPlayer.playerHand = new GameHand(gameDeck);

            // Determine the trump card
            cardTrump = gameDeck.getTrumpcard();

            // Display the trump card
            lblTrumpCard.Text = "Trump Card: " + cardTrump.ToString();
            pnTrump.Controls.Clear();
            pnTrump.Controls.Add(new CardBox.CardBox(cardTrump));

            // Display the number of cards remaining in the game deck
            lblDeckSizeValue.Text = gameDeck.getCardsRemaining().ToString();

            // Display the selected card in the player's hand
            lblCardSelected.Text = humanPlayer.playerHand.GetCard(indexOfPlayerCard).ToString();

            // Write game log information
            gameLogWriter("Trump Card: " + cardTrump.ToString() + "\n" + humanPlayer.playerName + "'s hand:" + humanPlayer.playerHand.ToString()
                          + "\n" + aiPlayer.playerName + "'s hand:" + aiPlayer.playerHand.ToString());

            // Create controls for the game
            ControlCreator();

            // Display controls for the game
            ControlDisplayer();

            // Determine the initial attacker
            GameCard minOfPlayer = humanPlayer.playerHand.getMinOfSuite(cardTrump);
            GameCard minOfAI = aiPlayer.playerHand.getMinOfSuite(cardTrump);

            if (minOfPlayer == null)
            {
                attackingPlayer = aiPlayer;
            }
            else if (minOfAI == null)
            {
                attackingPlayer = humanPlayer;
            }
            else if ((int)minOfPlayer.rank < (int)minOfAI.rank)
            {
                attackingPlayer = humanPlayer;
            }
            else
            {
                attackingPlayer = aiPlayer;
            }

            // Set initial attacker and defender
            if (attackingPlayer == humanPlayer)
            {
                lblPlayerTurn.Text = humanPlayer.playerName + " is the initial attacker.";
                gameLogWriter(humanPlayer.playerName + " is the initial attacker.");
                defendingPlayer = aiPlayer;
                currRound = PLAYDEFENDER;
            }


            else
            {
                // Handle the case when it's the AI player's turn to play

                // Check if there are cards on the playing field
                if (fieldCards.Count > 0)
                {
                    // Determine the current round based on the defending player
                    if (aiPlayer == defendingPlayer)
                    {
                        currRound = PLAYDEFENDER;
                    }
                    else
                    {
                        currRound = PLAYATTACKER;
                    }
                }

                // AI player's turn cycle to decide which card to play
                indexOfAICard = aiPlayer.AITurnCycle(cardTrump, gamePlayingField, currRound, flag);

                // Execute AI player's turn
                playAITurn();

                // Display the defending player's name
                lblCardSelected.Text = humanPlayer.playerName + "is the defender;";

                // Set the current round to attacker's turn
                currRound = PLAYATTACKER;

                // Set the defending player to human player
                defendingPlayer = humanPlayer;

                // Write game log information
                gameLogWriter(aiPlayer.playerName + " is the initial attacker.");

                // Enable buttons and hide unnecessary elements
                btnPlayCard.Enabled = true;
                btnDiscardPile.Enabled = true;
                btnStart.Visible = false;
                txtNameInput.Visible = false;
                btnSkipTurn.Enabled = true;

                // Write player statistics log
                statLogWriter(humanPlayer.playerName + "\n" + "Games Played: " + numGamesPlayed + "\n" + "Games Won: " + wonGames + "\n" + "Games Lost: " + lostGames);
            }
        }


        // Method to display the "About" dialog
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the game rules in a message box
            MessageBox.Show(gameRules);
        }

        // Method to handle the "Exit" menu item click
        private void mnuExit_Click(object sender, EventArgs e)
        {
            // Close the application
            this.Close();
        }

        // Method called when the form is loaded
        private void frmDurak_Load(object sender, EventArgs e)
        {
            // Display the number of cards remaining in the game deck
            lblDeckSizeValue.Text = gameDeck.getCardsRemaining().ToString();

            // Create some example cards for demonstration
            GameCard theCard = new GameCard(GameSuit.Clubs, GameRank.Six, 1);
            theCard.FaceUp = false;
            GameCard theCard2 = new GameCard(GameSuit.Clubs, GameRank.Seven, 2);

            // Disable buttons and show/hide elements appropriately
            btnPlayCard.Enabled = false;
            btnDiscardPile.Enabled = false;
            btnStart.Visible = true;
            txtNameInput.Visible = true;
            chkAIHandToggle.Enabled = false;
        }

        // Method to display cards on the playing field
        private void FieldDisplayer()
        {
            // Adjust offset for the playing field based on its size
            fieldOffset = fieldOffset - (fieldCards.Count - 6) * 20 / 100;

            // Display card boxes for cards on the playing field
            for (int i = fieldCards.Count - 1; i >= 0; i--)
            {
                // Position the card box horizontally based on index and offset
                fieldCards[i].Left = (i * 20) + fieldOffset;

                // Add the card box to the playing field panel
                this.pnPlayingField.Controls.Add(fieldCards[i]);
            }
        }


        // Method to handle the "Play Card" button click
        private void btnPlayCard_Click(object sender, EventArgs e)
        {
            // Get the card to be played from the human player's hand
            GameCard aCard = humanPlayer.playerHand.GetCard(indexOfPlayerCard);

            // Check if the card can be played
            if (aCard.suit == cardTrump.suit || fieldCards.Count == 0 || ((int)aCard.rank > (int)gamePlayingField.getCurrentCard().rank && aCard.suit == gamePlayingField.getCurrentCard().suit))
            {
                // Remove the card from the player's hand and play it
                aCard = humanPlayer.playerHand.playCard(indexOfPlayerCard);

                // Create a card box for the played card
                CardBox.CardBox newCardBox = new CardBox.CardBox(aCard);

                // Update the game playing field
                gamePlayingField.cardPlayed(aCard);

                // Add the played card to the list of field cards
                fieldCards.Add(newCardBox);

                // Display the selected card in a label
                lblCardSelected.Text = aCard.ToString();

                // Clear player's card collection, update field display, and recreate player's hand
                playerCards.Clear();
                FieldDisplayer();
                ControlCreator();
                this.pnPlayerHand.Controls.Clear();
                ControlDisplayer();

                // Switch to the AI player's turn
                currPlayer = aiPlayer;

                // Determine the current round based on the defending player
                if (fieldCards.Count > 0)
                {
                    if (aiPlayer == defendingPlayer)
                    {
                        currRound = PLAYDEFENDER;
                    }
                    else
                    {
                        currRound = PLAYATTACKER;
                    }
                }

                // AI player's turn cycle to decide which card to play
                indexOfAICard = aiPlayer.AITurnCycle(cardTrump, gamePlayingField, currRound, flag);

                // Execute AI player's turn
                playAITurn();
            }
        }

        // Method to execute AI player's turn
        private void playAITurn()
        {
            // Check if AI player cannot play a card
            if (indexOfAICard == -1)
            {
                // Skip turn if no valid card to play
                btnSkipTurn_Click(null, null);
            }

            // Get the card to be played from the AI player's hand
            GameCard aCard = aiPlayer.playerHand.GetCard(indexOfAICard);

            // Check if the card can be played
            if (aCard.suit == cardTrump.suit && fieldCards.Count != 0 && gamePlayingField.getCurrentCard().suit == cardTrump.suit && (int)aCard.rank < (int)gamePlayingField.getCurrentCard().rank)
            {
                // Skip turn if a higher trump card is needed
                btnSkipTurn_Click(null, null);
            }
            else if (aCard.suit == cardTrump.suit || fieldCards.Count == 0 || ((int)aCard.rank > (int)gamePlayingField.getCurrentCard().rank && aCard.suit == gamePlayingField.getCurrentCard().suit))
            {
                // Remove the card from the AI player's hand and play it
                aCard = aiPlayer.playerHand.playCard(indexOfAICard);
                aCard.FaceUp = true;
                gamePlayingField.cardPlayed(aCard);

                // Create a card box for the played card
                CardBox.CardBox newCardBox = new CardBox.CardBox(aCard);
                fieldCards.Add(newCardBox);

                // Clear AI player's card collection, update field display, and recreate AI player's hand
                aiCards.Clear();
                FieldDisplayer();
                ControlCreator();
                this.pnAIHand.Controls.Clear();
                ControlDisplayer();

                // Switch to the human player's turn
                currPlayer = humanPlayer;
            }
            else
            {
                // Skip turn if no valid card to play
                btnSkipTurn_Click(null, null);
            }
        }

        // Method to handle the end of a winning attack
        private void WinningAttack()
        {
            // Pick up cards from the playing field
            ArrayList cardsToBePickedUp = gamePlayingField.pickupField();

            // Clear field cards and update field display
            fieldCards.Clear();
            this.pnPlayingField.Controls.Clear();
            FieldDisplayer();

            // Handle card pickup for the defending player
            if (attackingPlayer == humanPlayer)
            {
                humanPlayer.DrawCards(gameDeck);
                this.pnPlayerHand.Controls.Clear();
                playerCards.Clear();

                // Add drawn cards to the player's hand and update hand display
                for (int i = 0; i < humanPlayer.playerHand.gethandSize(); i++)
                {
                    CardBox.CardBox newCardBox = new CardBox.CardBox(humanPlayer.playerHand.GetCard(i));
                    newCardBox.Click += CardBox_Click;
                    playerCards.Add(newCardBox);
                }
                ControlDisplayer();
            }
            else
            {
                aiPlayer.DrawCards(gameDeck);
                this.pnAIHand.Controls.Clear();
                aiCards.Clear();

                // Add drawn cards to the AI player's hand and update hand display
                for (int i = 0; i < aiPlayer.playerHand.gethandSize(); i++)
                {
                    CardBox.CardBox newCardBox = new CardBox.CardBox(aiPlayer.playerHand.GetCard(i));
                    aiCards.Add(newCardBox);
                }
                ControlDisplayer();
            }

            // Check if the game has ended due to no remaining cards in the deck
            if (gameDeck.getCardsRemaining() == 0)
            {
                gameEnder = true;
            }

            // Check if the game has ended and handle game over state
            if (gameEnder)
            {
                isGameOver();
            }

            // Handle card pickup for the defending player
            if (defendingPlayer == humanPlayer)
            {
                string tempString = "";
                for (int i = 0; i < cardsToBePickedUp.Count; i++)
                {
                    humanPlayer.playerHand.addCard((GameCard)cardsToBePickedUp[i]);
                    tempString += " " + cardsToBePickedUp[i].ToString();
                }
                gameLogWriter(humanPlayer.playerName + " picked up:" + tempString);
                humanPlayer.DrawCards(gameDeck);

                this.pnPlayerHand.Controls.Clear();
                playerCards.Clear();

                // Add drawn cards to the player's hand and update hand display
                for (int i = 0; i < humanPlayer.playerHand.gethandSize(); i++)
                {
                    CardBox.CardBox newCardBox = new CardBox.CardBox(humanPlayer.playerHand.GetCard(i));
                    newCardBox.Click += CardBox_Click;
                    playerCards.Add(newCardBox);
                }
                ControlDisplayer();
            }
            else
            {
                string tempString = "";
                for (int i = 0; i < cardsToBePickedUp.Count; i++)
                {
                    aiPlayer.playerHand.addCard((GameCard)cardsToBePickedUp[i]);
                    tempString += " " + cardsToBePickedUp[i].ToString();
                }
                gameLogWriter(aiPlayer.playerName + " picked up:" + tempString);

                aiPlayer.DrawCards(gameDeck);

                this.pnAIHand.Controls.Clear();
                aiCards.Clear();

                // Add drawn cards to the AI player's hand and update hand display





                for (int i = 0; i < aiPlayer.playerHand.gethandSize(); i++)
                // Size(); i++)
                {
                    CardBox.CardBox newCardBox = new CardBox.CardBox(aiPlayer.playerHand.GetCard(i));
                    aiCards.Add(newCardBox);
                }
                ControlDisplayer();
            }

            // Check if the game has ended due to no remaining cards in the deck
            if (gameDeck.getCardsRemaining() == 0)
            {
                gameEnder = true;
            }

            // Check if the game has ended and handle game over state
            if (gameEnder)
            {
                isGameOver();
            }

            // Swap attacking and defending players
            if (attackingPlayer == humanPlayer)
            {
                defendingPlayer = aiPlayer;
                attackingPlayer = humanPlayer;
            }
            else if (attackingPlayer == aiPlayer)
            {
                defendingPlayer = humanPlayer;
                attackingPlayer = aiPlayer;
            }

            // Reset game variables for the next round
            countTurns = 0;
            currPlayer = attackingPlayer;
            currRound = INITIALATTACK;
            flag = false;

            // Update UI to reflect the attacker for the next round
            lblPlayerTurn.Text = currPlayer.playerName + " is still attacker.";
            gameLogWriter("Round End");
            gameLogWriter(currPlayer.playerName + " is still attacker.");

            // Enable skip turn button and reset index of selected player card
            btnSkipTurn.Enabled = true;
            indexOfPlayerCard = 0;
            lblCardSelected.Text = humanPlayer.playerHand.GetCard(indexOfPlayerCard).ToString();

            // Update the display of remaining cards in the deck
            lblDeckSizeValue.Text = gameDeck.getCardsRemaining().ToString();
        }


        // Method to handle the "Skip Turn" button click
        private void btnSkipTurn_Click(object sender, EventArgs e)
        {
            // Check if the current player is the defending player
            if (currPlayer == defendingPlayer)
            {
                // Log defending player's skip and end the round
                gameLogWriter(defendingPlayer.playerName + " skipped.");
                gameLogWriter("Round End");
                WinningAttack();
            }
            // Check if the current player is the attacking player
            else if (currPlayer == attackingPlayer)
            {
                // Log attacking player's skip and end the round
                gameLogWriter(attackingPlayer.playerName + " skipped.");
                gameLogWriter("Round End");
                WinningDefend();
            }

            // Clear error message label
            lblErrorMsg.Text = "";

            // Determine AI player's turn cycle and execute its turn if AI player's turn
            indexOfAICard = aiPlayer.AITurnCycle(cardTrump, gamePlayingField, currRound, flag);
            if (currPlayer == aiPlayer)
            {
                // Update current round based on AI player's role
                if (fieldCards.Count > 0)
                {
                    if (aiPlayer == defendingPlayer)
                    {
                        currRound = PLAYDEFENDER;
                    }
                    else
                    {
                        currRound = PLAYATTACKER;
                    }
                }

                // Determine AI player's turn cycle and execute its turn
                indexOfAICard = aiPlayer.AITurnCycle(cardTrump, gamePlayingField, currRound, flag);
                playAITurn();
            }

            // Set skipped flag to true
            skipped = true;
        }


        // Method to check if the game is over
        public void isGameOver()
        {
            // Check if AI player has no cards left
            if (aiPlayer.playerHand.gethandSize() == 0)
            {
                // Log AI player's victory, display game over message, and disable game buttons
                gameLogWriter(aiPlayer.playerName + " won the game!!");
                MessageBox.Show("GAME OVER.");
                btnPlayCard.Enabled = false;
                btnDiscardPile.Enabled = false;
                btnSkipTurn.Enabled = false;
                btnStart.Visible = true;
                txtNameInput.Visible = true;
                txtNameInput.Text = humanPlayer.playerName;

                // Reset game variables and controls
                gameDeck = new GameDeck();
                countTurns = 0;
                gameEnder = false;
                playerCards = new List<CardBox.CardBox>();
                aiCards = new List<CardBox.CardBox>();
                fieldCards = new List<CardBox.CardBox>();
                currRound = INITIALATTACK;
                flagMatcher = false;
                pnPlayerHand.Controls.Clear();
                pnAIHand.Controls.Clear();
                playerCards.Clear();
                gamePlayingField = new GameField();
                pnPlayingField.Controls.Clear();
                cbTrumpCard.Visible = true;
                cardBox1.Visible = true;
                chkAIHandToggle.Enabled = true;
                btnResetStats.Visible = true;
                gameEnd = true;
                lostGames += 1;

                // Write player statistics to the log file
                statLogWriter(humanPlayer.playerName + "\n" + "Games Played: " + numGamesPlayed + "\n" + "Games Won: " + wonGames + "\n" + "Games Lost: " + lostGames);
            }
            // Check if human player has no cards left
            else if (humanPlayer.playerHand.gethandSize() == 0)
            {
                // Display human player's victory message and update game controls
                MessageBox.Show(humanPlayer.playerName + " Wins!");
                gameLogWriter(humanPlayer.playerName + " won the game!!");
                btnPlayCard.Enabled = false;
                btnDiscardPile.Enabled = false;
                btnSkipTurn.Enabled = false;
                btnStart.Visible = true;
                txtNameInput.Visible = true;
                txtNameInput.Text = humanPlayer.playerName;
                pnPlayerHand.Controls.Clear();
                pnAIHand.Controls.Clear();
                pnPlayingField.Controls.Clear();
                gameEnd = true;
                btnResetStats.Visible = true;

                // Reset game variables and controls
                gameDeck = new GameDeck();
                humanPlayer.DrawCards(gameDeck);
                wonGames += 1;

                // Write player statistics to the log file
                statLogWriter(humanPlayer.playerName + "\n" + "Games Played: " + numGamesPlayed + "\n" + "Games Won: " + wonGames + "\n" + "Games Lost: " + lostGames);
            }
            //cardBox1.Visible = false;
        }

        // Method to handle the end of a winning defense
        private void WinningDefend()
        {
            // Discard cards from the playing field
            gamePlayingField.discardField();

            // Clear field cards and update field display
            fieldCards.Clear();
            this.pnPlayingField.Controls.Clear();
            FieldDisplayer();

            // Draw cards for the attacking player
            if (attackingPlayer == humanPlayer)
            {
                humanPlayer.DrawCards(gameDeck);
                this.pnPlayerHand.Controls.Clear();
                playerCards.Clear();

                // Add drawn cards to the player's hand and update hand display
                for (int i = 0; i < humanPlayer.playerHand.gethandSize(); i++)
                {
                    CardBox.CardBox newCardBox = new CardBox.CardBox(humanPlayer.playerHand.GetCard(i));
                    newCardBox.Click += CardBox_Click;
                    playerCards.Add(newCardBox);
                }
                ControlDisplayer();
            }
            else
            {
                aiPlayer.DrawCards(gameDeck);
                this.pnAIHand.Controls.Clear();
                aiCards.Clear();

                // Add drawn cards to the AI player's hand and update hand display
                for (int i = 0; i < aiPlayer.playerHand.gethandSize(); i++)
                {
                    CardBox.CardBox newCardBox = new CardBox.CardBox(aiPlayer.playerHand.GetCard(i));
                    aiCards.Add(newCardBox);
                }
                ControlDisplayer();
            }

            // Check if the game has ended due to no remaining cards in the deck
            if (gameDeck.getCardsRemaining() == 0)
            {
                gameEnder = true;
            }

            // Check if the game has ended and handle game over state
            if (gameEnder)
            {
                isGameOver();
            }

            // Draw cards for the defending player
            if (defendingPlayer == humanPlayer)
            {
                humanPlayer.DrawCards(gameDeck);
                this.pnPlayerHand.Controls.Clear();
                playerCards.Clear();

                // Add drawn cards to the player's hand and update hand display
                for (int i = 0; i < humanPlayer.playerHand.gethandSize(); i++)
                {

                    CardBox.CardBox newCardBox = new CardBox.CardBox(humanPlayer.playerHand.GetCard(i));
                    // ard(i));
                    newCardBox.Click += CardBox_Click;
                    playerCards.Add(newCardBox);
                }
                ControlDisplayer();
            }
            else
            {
                aiPlayer.DrawCards(gameDeck);
                this.pnAIHand.Controls.Clear();
                aiCards.Clear();

                // Add drawn cards to the AI player's hand and update hand display
                for (int i = 0; i < aiPlayer.playerHand.gethandSize(); i++)
                {
                    CardBox.CardBox newCardBox = new CardBox.CardBox(aiPlayer.playerHand.GetCard(i));
                    aiCards.Add(newCardBox);
                }
                ControlDisplayer();
            }

            // Check if the game has ended due to no remaining cards in the deck
            if (gameDeck.getCardsRemaining() == 0)
            {
                gameEnder = true;
            }

            // Check if the game has ended and handle game over state
            if (gameEnder)
            {
                isGameOver();
            }

            // Swap roles between attacking and defending players
            SwapRoles();
            gameLogWriter("Roles Swapped");

            // Reset game variables for the next round
            countTurns = 0;
            currPlayer = attackingPlayer;
            currRound = INITIALATTACK;
            flag = false;
            lblPlayerTurn.Text = currPlayer.playerName + " is the new attacker.";
            gameLogWriter(attackingPlayer.playerName + " is the new attacker.");

            // Update the display of remaining cards in the deck
            lblDeckSizeValue.Text = gameDeck.getCardsRemaining().ToString();
        }


        // Method to handle the "Discard Pile" button click
        private void btnDiscardPile_Click(object sender, EventArgs e)
        {
            // Open the DiscardForm to display the discard pile
            DiscardForm frmdiscard = new DiscardForm();
            DiscardForm.field = gamePlayingField;
            frmdiscard.ShowDialog();
        }

        // Method to swap roles between attacking and defending players
        public void SwapRoles()
        {
            if (defendingPlayer == humanPlayer)
            {
                defendingPlayer = aiPlayer;
                attackingPlayer = humanPlayer;
            }
            else if (defendingPlayer == aiPlayer)
            {
                defendingPlayer = humanPlayer;
                attackingPlayer = aiPlayer;
            }
        }

        // Method to handle the "Reset Stats" button click
        private void btnResetStats_Click(object sender, EventArgs e)
        {
            // Reset game statistics and write to the log file
            numGamesPlayed = 0;
            wonGames = 0;
            lostGames = 0;
            statLogWriter(humanPlayer.playerName + "\n" + "Games Played: " + numGamesPlayed + "\n" + "Games Won: "
                + wonGames + "\n" + "Games Lost: " + lostGames);
            btnResetStats.Visible = false;
        }

        // Method to handle the painting event of the AI player's hand panel
        private void pnAIHand_Paint(object sender, PaintEventArgs e)
        {
            // This method is currently empty
        }

        // Method to read player statistics from the log file
        public void statLogReader()
        {
            bool fileExists = false;
            string path = "../../logs/PlayerStats/" + humanPlayer.playerName + "-StatsLog.txt";

            // Check if the log file exists and read statistics if it does
            using (FileStream fs = File.Open(path, FileMode.Append, FileAccess.Write))
            {
                if (fs.Length > 0)
                {
                    fileExists = true;
                }
            }

            if (fileExists)
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string input = reader.ReadToEnd();

                    // Split the input string to extract statistics
                    string[] splitInput = input.Split(null);
                    numGamesPlayed = int.Parse(splitInput[3]);
                    wonGames = int.Parse(splitInput[6]);
                    lostGames = int.Parse(splitInput[9]);
                }
            }
        }
