namespace Assets.Scripts.Constants
{
    public static class ErrorMessages
    {
        public const string InvalidEmailFormat = "Invalid email format. Please enter a valid email address.";
        public const string PasswordsDoNotMatch = "Passwords do not match.";
        public const string EmailAlreadyRegistered = "This email is already registered. Please use a different email or log in.";
        public const string SignUpError = "An error occurred during sign-up. Please try again.";
        public const string LoginError = "An unexpected error occurred. Please try again.";
        public const string AccountDoesNotExist = "The account does not exist. Please sign up first.";
        public const string IncorrectPassword = "Incorrect password. Please try again.";
        public const string UnableToLogin = "Unable to login. Please try again.";
        public const string AccountSwitchError = "Recovered account is null. Cannot switch accounts.";
        public const string FailedToRecoverAccount = "Failed to recover account. Please check your email and password.";
        public const string FieldsMustBeFilled = "All fields must be filled.";
    }
}