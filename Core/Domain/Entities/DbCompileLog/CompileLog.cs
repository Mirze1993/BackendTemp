namespace Domain.Entities.DbCompileLog;

public class CompileLog
{
    public  int Id { get; set; }
    public  DateTime Ts { get; set; }
    public  string? Name { get; set; }
    public  string? Body { get; set; }
}