using Common.Interface;

namespace Common.Models.Immutable;

public record HistoryLog : IElement<HistoryLog>
{
    public int? Id { get; set; }
    public string LogEvent { get; set; }
    public bool Active { get; set; }
    public Guid? Key { get; set; }
    public DateTime DateAdded { get; set; }

    public U Visit<U>(IVisitor<HistoryLog, U> visitor)
    {
        return visitor.Visit(this);
    }
}
