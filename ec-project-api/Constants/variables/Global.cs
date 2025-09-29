namespace ec_project_api.Constants.variables
{
    public static class GlobalVariables
    {
        public const string UsernameRegex = @"^[a-zA-Z0-9_]{3,50}$";
        public const string EmailRegex = @"^[\w\.-]+@[\w\.-]+\.\w{2,}$";
        public const string PasswordStrongRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        public const string ImageUrlRegex = @"^(https?:\/\/.*\.(?:png|jpg|jpeg|gif|webp))$";
        public const string FullNameRegex = @"^[\p{L}\s]{1,100}$";
        public const string PhoneRegex = @"^\+?[0-9]{8,15}$";
        public const string GenderRegex = @"^(Male|Female|Other|M|F)?$";
    }
}
