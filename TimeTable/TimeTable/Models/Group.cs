namespace TimeTable.Models;

public class Group
{
    public string Name { get; set; }
    public string Type { get; set; }
    
    public Group(string name, string type)
    {
        Name = name;
        Type = type;
    }
}