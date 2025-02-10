/*
    Project: Durak Game made by Group3
    Description: This project contains classes representing a card game.
    Authors: Dhyey Patel, Bhavya Detroja, Pratham Patel, Dawa Sherpa
    Date: 6th April 2024
*/

using System; 
using System.Windows.Forms; 

namespace DurakFormApp
{
    // Program class contains the entry point of the application
    static class Program
    {
        // Main method, entry point of the application
        [STAThread] // Specifies that the COM threading model for the application is single-threaded apartment (STA)
        static void Main()
        {
            // Enable visual styles for the application to apply styles to controls
            Application.EnableVisualStyles();

            // Set the application's default text rendering to compatible mode
            Application.SetCompatibleTextRenderingDefault(false);

            // Run the application, starting with the DurakForm form
            Application.Run(new DurakForm());
        }
    }
}
