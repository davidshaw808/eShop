namespace DataLayer.Interface.General
{
    public interface IDatabaseChangeValidation
    {
        public (bool Generate, bool Update) Valid<T>(T t);
        IVisitorCaller
    }
}
