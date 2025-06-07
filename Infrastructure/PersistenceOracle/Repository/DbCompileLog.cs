using Appilcation.IRepository;
using Dapper;
using Domain.Entities.DbCompileLog;

namespace PersistenceOracle.Repository;

public class DbCompileLog:IDbCompileLog
{
    public async Task<IEnumerable<string>> GetDistinctList()
    {
        await using var db = new OracleDb();
        return await db.Connection.QueryAsync<string>(@"
                select  distinct NAME from COMPILE_LOG");
    }

    public async Task<IEnumerable<CompileLog>> GetByName(string name)
    {
        await using var db = new OracleDb();
        return await db.Connection.QueryAsync<CompileLog>(@"
                select  ID,NAME,TS from COMPILE_LOG where NAME=:Name", new { Name = name });
    }

    public async Task<CompileLog?> GetById(int id)
    {
        await using var db = new OracleDb();
        return await db.Connection.QueryFirstOrDefaultAsync<CompileLog>(@"
                select  ID,NAME,TS,BODY from COMPILE_LOG where ID=:ID", new { ID = id });
    }
}