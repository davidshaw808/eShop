using Common;
using Common.Base;
using DataLayer.Interface.General;

namespace BusinessLayer.CommonVisitor.DatabaseChangeValidators;

public class DatabaseChangeValidationUser : IDatabaseChangeValidation
{
    public (bool Generate, bool Update) Valid<T>(T t) => t switch
    {
        Address => (true, true),
        Order => (false, false),
        FinancialTransaction => (false, false),
        Product => (false, false),
        AdminUser => (false, false),
        Customer => (true, true),
        Person => (true, true),
        Category => (false, false),
        IEnumerable<Category> => (false, false),
        HistoryLog => (true, false),
        Review => (true, true),
        _ => (false, false)
    };
}
