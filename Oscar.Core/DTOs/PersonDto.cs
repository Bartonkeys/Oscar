namespace Oscar.Core.DTOs
{

    public record PersonDto: IDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }

    public record ProducerDto : PersonDto
    {
    }

    public record DirectorDto : PersonDto
    {
    }

    public record ActorDto : PersonDto
    {
    }

    public record DistributorDto : PersonDto
    {
    }

    public record ScreenWriterDto : PersonDto
    {
    }

    public record ScriptWriterDto : PersonDto
    {
    }

}