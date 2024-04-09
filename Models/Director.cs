namespace DevFlix.Models;

public class Director : Person
{
    public List<MediaDirector>? MediaDirectors { get; set; }
}
