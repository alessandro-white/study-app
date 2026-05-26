

class Topics
{
    public string Title { get; set; }
    public string Body { get; set; }
    public int ID { get; set; }
}

class notes : Topics
{
    public string note { get; set; }
}

class account
{
    private string Username { get; set; }
    private string Email { get; set; }
    private string Password { get; set; }
}

