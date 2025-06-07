using Domain.Entities.DbCompileLog;

namespace Appilcation.IRepository;

public interface IDbCompileLog
{
    public  Task<IEnumerable<string>> GetDistinctList();
    public  Task<IEnumerable<CompileLog>> GetByName(string name);
    public  Task<CompileLog?> GetById(int id);
}