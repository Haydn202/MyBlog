namespace IntegrationTests.Builders.User;

public class UpdateRoleRequest
{
    private string _role = "";

    public UpdateRoleRequest WithRole(string role)
    {
        _role = role;
        return this;
    }

    public object Build() => new
    {
        role = _role
    };
}
