using Common;
using Common.Base;
using DataLayer.Interface.General;

namespace BusinessLayer.CommonVisitor.DatabaseChangeValidators;

public class DatabaseChangeValidationAdmin : IDatabaseChangeValidation
{
    public (bool Generate, bool Update) Valid<T>(T t) => t switch
    {
        Address => (true, true),
        Order => (true, true),
        FinancialTransaction => (true, false),
        Product => (true, true),
        AdminUser => (true, true),
        Customer => (true, true),
        Person => (true, true),
        Category => (true, true),
        IEnumerable<Category> => (true, true),
        HistoryLog => (true, true),
        Review => (true, true),
        _ => (false, false)
    };
}
