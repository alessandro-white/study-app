class Module
{
    public string Name { get; set; }
    public int OwnerID { get; set; }
    public int ID { get; set; }
}

class Note
{
    public int ID { get; set; }
    public int ModuleID { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime LastReviewed { get; set; }
    public int Interval { get; set; }
    public int Repetitions { get; set; }
    public double Easiness { get; set; }
}

class Account
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public int ID { get; private set; }
}



