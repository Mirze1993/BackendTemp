namespace ExternalServices.Models;

public class AsanFinanceResp
{
    public string DocNumber { get; set; }
    public string FullName { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public DateTime BirthDate { get; set; }
    public string BirthAdress { get; set; }
    public int Gender { get; set; }
    public string RegAdress { get; set; }
    public DateTime ExpirseDate { get; set; }
    public DateTime GivenDate { get; set; }
    public int MaritalStatus { get; set; }
    public string GivenOrg { get; set; }
    public string Citizensgip { get; set; }
    public string Image { get; set; }
}