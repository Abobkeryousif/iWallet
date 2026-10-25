namespace iWallet.Application.Errors
{
    public static class UserErrors
    {
        public static ServiceError UserAlreadyExist() =>
            new(
                "User.Register.Error",
                "User Already Exist",
                ErrorType.NotFound
                );
        public static ServiceError InValidEmailOrPassword() =>
            new(
                "User.Login.Error",
                "Invalid Email or Password",
                ErrorType.NotFound
                );
    }
}
