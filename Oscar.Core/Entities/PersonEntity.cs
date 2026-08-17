namespace Oscar.Core.Entities;

public class PersonEntity: BaseEntity
{
    private string _firstName;
    public string FirstName
    {
        get { return _firstName; }
        set
        {
            _firstName = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }
    }

    private string _lastName;
    public string LastName
    {
        get { return _lastName; }
        set
        {
            _lastName = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }
    }

    public ICollection<Works> Works { get; set; }
}

public class Producer : PersonEntity
{
}

public class Director : PersonEntity
{
}

public class Actor : PersonEntity
{
}

public class Distributor : PersonEntity
{
}

public class ScreenWriter : PersonEntity
{
}

public class ScriptWriter : PersonEntity
{
}