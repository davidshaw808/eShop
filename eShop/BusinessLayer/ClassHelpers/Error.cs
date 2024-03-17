namespace BusinessLayer.ClassHelpers
{
    public record Error
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
