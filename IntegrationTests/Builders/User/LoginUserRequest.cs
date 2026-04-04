namespace IntegrationTests.Builders.User;

public class LoginUserRequest
{
    private string _email = "";
    private string _password = "";

    public LoginUserRequest WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public LoginUserRequest WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public object Build() => new
    {
        email = _email,
        password = _password
    };
}
