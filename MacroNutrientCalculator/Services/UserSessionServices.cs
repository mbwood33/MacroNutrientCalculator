using System;

namespace MacroNutrientCalc.Services
{
    // UserSessionService is responsible for storing and retrieving the currently logged-in user's information.
    // It is designed to be a simple in-memory session store.
    public class UserSessionServices
    {
        public int CurrentUserId { get; set; }  // Holds the current user's ID. It can be used by other components to retrieve user-specific data.
        public string CurrentUserName { get; set; } // Holds the current user's name for display purposes
        
        // Sets the current user's session data after a successful login
        public void SetCurrentUser(int userId, string userName)
        {
            CurrentUserId = userId;     // Store the user's ID
            CurrentUserName = userName; // Store the user's name
        }

        // Clears the session data, for example when the user logs out
        public void ClearSession()
        {
            CurrentUserId = 0;      // Reset the user ID
            CurrentUserName = string.Empty; // Reset the user's name
        }
    }
}
