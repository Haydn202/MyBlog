namespace IntegrationTests.Builders.User;

public class RegisterUserRequest
{
    private string _userName = "";
    private string _password = "";
    private string _email = "";

    public RegisterUserRequest WithName(string username)
    {
        _userName = username;
        return this;
    }

    public RegisterUserRequest WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public RegisterUserRequest WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public object Build() => new
    {
        userName = _userName,
        password = _password,
        email = _email
    };
}