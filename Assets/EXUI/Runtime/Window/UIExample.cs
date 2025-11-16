/// <summary>
/// 账号子视图模型
/// </summary>
public class AccountViewModel : ObservableObject
{
    private int id;
    private string username;
    private string password;
    private string email;
    private DateTime birthday;
    private readonly ObservableProperty<string> address = new ObservableProperty<string>();

    public int ID
    {
        get { return this.id; }
        set { this.Set<int>(ref this.id, value, "ID"); }
    }

    public string Username
    {
        get { return this.username; }
        set { this.Set<string>(ref this.username, value, "Username"); }
    }

    public string Password
    {
        get { return this.password; }
        set { this.Set<string>(ref this.password, value, "Password"); }
    }

    public string Email
    {
        get { return this.email; }
        set { this.Set<string>(ref this.email, value, "Email"); }
    }

    public DateTime Birthday
    {
        get { return this.birthday; }
        set { this.Set<DateTime>(ref this.birthday, value, "Birthday"); }
    }

    public ObservableProperty<string> Address
    {
        get { return this.address; }
    }
}


/// <summary>
/// 数据绑定示例的视图模型
/// </summary>
public class DatabindingViewModel : ViewModelBase
{
    private AccountViewModel account;
    private bool remember;
    private string username;
    private string email;
    private ObservableDictionary<string, string> errors = new ObservableDictionary<string, string>();

    public AccountViewModel Account
    {
        get { return this.account; }
        set { this.Set<AccountViewModel>(ref account, value, "Account"); }
    }

    public string Username
    {
        get { return this.username; }
        set { this.Set<string>(ref this.username, value, "Username"); }
    }

    public string Email
    {
        get { return this.email; }
        set { this.Set<string>(ref this.email, value, "Email"); }
    }

    public bool Remember
    {
        get { return this.remember; }
        set { this.Set<bool>(ref this.remember, value, "Remember"); }
    }

    public ObservableDictionary<string, string> Errors
    {
        get { return this.errors; }
        set { this.Set<ObservableDictionary<string, string>>(ref this.errors, value, "Errors"); }
    }

    public void OnUsernameValueChanged(string value)
    {
        Debug.LogFormat("Username ValueChanged:{0}", value);
    }

    public void OnEmailValueChanged(string value)
    {
        Debug.LogFormat("Email ValueChanged:{0}", value);
    }

    public void OnSubmit()
    {
        if (string.IsNullOrEmpty(this.Username) || !Regex.IsMatch(this.Username, "^[a-zA-Z0-9_-]{4,12}$"))
        {
            this.errors["errorMessage"] = "Please enter a valid username.";
            return;
        }

        if (string.IsNullOrEmpty(this.Email) || !Regex.IsMatch(this.Email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
        {
            this.errors["errorMessage"] = "Please enter a valid email.";
            return;
        }

        this.errors.Clear();
        this.Account.Username = this.Username;
        this.Account.Email = this.Email;
    }
}
