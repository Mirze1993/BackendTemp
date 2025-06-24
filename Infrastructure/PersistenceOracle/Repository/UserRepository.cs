using System.Data;
using System.Text;
using Appilcation.IRepository;
using Dapper;
using Domain.DTO.User;
using Domain.Entities.User;
using Domain.Enums;
using Domain.Request;
using Domain.Request.User;
using Domain.Response.User;
using Oracle.ManagedDataAccess.Client;

namespace PersistenceOracle.Repository;

public class UserRepository : IUserRepository
{
    public async Task<int> Register(RegisterUserDto dto)
    {
        await using var db = new OracleDb();

        OracleParameter result = new("result", OracleDbType.Int32, ParameterDirection.Output);
        var pr = new List<OracleParameter>
        {
            result,
            new("P_EMAIL", OracleDbType.NVarchar2, dto.Email, ParameterDirection.Input),
            new("P_PASSWORD", OracleDbType.NVarchar2, dto.Password, ParameterDirection.Input)
        };


        await db.NonQueryAsync(@"
            begin
                  :result :=PKG_APP_USER.ADD_APP_USER (
												 P_EMAIL => :P_EMAIL,
                                                 P_PASSWORD => :P_PASSWORD);
            end;"
            , pr);
        var id = Convert.ToInt32(result.Value?.ToString());
        await SetClaim(id, UserClaimType.Role, "user");
        await SetClaim(id, UserClaimType.Name, dto.Name);
        return id;
    }

    public async Task<GetUserDto?> GetUserByEmail(string email)
    {
        await using var db = new OracleDb();
        var query = @"
SELECT Id,       
       Email,
       Password,
       FAILED_LOGIN_COUNT FailedCount ,
       LAST_FAILED_LOGIN LastFailedLogin
FROM APP_USER
WHERE Email = :P_EMAIL
";
        return await db.Connection.QueryFirstOrDefaultAsync<GetUserDto>(query, new
        {
            P_EMAIL = email,
        });
    }


    public async Task<GetUserDto> GetByRefreshToken(string refToken, int id)
    {
        await using var db = new OracleDb();
        var query = @"
SELECT Id,       
       Email,
       Password,
       FAILED_LOGIN_COUNT,
       LAST_FAILED_LOGIN
FROM App_user
WHERE Refresh_Token = :P_ref_tok AND Id = :P_Id";
        return await db.Connection.QueryFirstAsync<GetUserDto>(query, new
        {
            Refresh_Token = refToken,
            P_Id = id
        });
    }

    public async Task<bool> SuccessLogin(string refToken, int id)
    {
        await using var db = new OracleDb();

        var pr = new List<OracleParameter>
        {
            new("P_Id", OracleDbType.Int32, id, ParameterDirection.Input),
            new("P_ref_token", OracleDbType.NVarchar2, refToken, ParameterDirection.Input)
        };
        var t = await db.NonQueryAsync(@"
            begin
                  PKG_APP_USER.Success_Login (P_Id => :P_Id,
												 P_ref_token => :P_ref_token );
            end;", pr);

        return true;
    }

    public async Task<bool> FailedLogin(int id)
    {
        await using var db = new OracleDb();

        var pr = new List<OracleParameter>
        {
            new("P_Id", OracleDbType.Int32, id, ParameterDirection.Input)
        };
        var t = await db.NonQueryAsync(@"
            begin
                  PKG_APP_USER.Failed_Login (P_Id => :P_Id);
            end;", pr);
        return t > 0;
    }

    public async Task<List<UserClaims>> GetClaims(int userId, string claimType)
    {
        await using var db = new OracleDb();
        var pr = new DynamicParameters();
        pr.Add("P_AppUserId", userId);
        StringBuilder query = new(@"SELECT Id,
       CreateDate,
       LeastUpdate,
       Type,
       Value,
       ValueType,
       Issuer,
       AppUserId
FROM UserClaims
WHERE AppUserId = :P_AppUserId");
        if (!string.IsNullOrEmpty(claimType))
        {
            query.Append(" and Type=:P_Type ");
            pr.Add("P_Type", claimType);
        }

        return (await db.Connection.QueryAsync<UserClaims>(
            query.ToString(),
            pr
        )).ToList();
    }

    private async Task<int> SetClaim(int userId, string claimType, params string[] value)
    {
        return await SetClaim(new SetClaimReq(userId, claimType, value));
    }

    public async Task<int> SetClaim(SetClaimReq req)
    {
        await using var db = new OracleDb();

        OracleParameter values = new("P_Value", OracleDbType.NVarchar2, ParameterDirection.Input);
        values.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
        values.Value = req.Value;

        var pr = new List<OracleParameter>
        {
            new("P_Type", OracleDbType.NVarchar2, req.ClaimType, ParameterDirection.Input),
            values,
            new("P_ValueType", OracleDbType.NVarchar2, "string", ParameterDirection.Input),
            new("P_Issuer", OracleDbType.NVarchar2, "d", ParameterDirection.Input),
            new("P_AppUserId", OracleDbType.Int32, req.UserId, ParameterDirection.Input)
        };


        var r = await db.NonQueryAsync(@"
            begin
                  PKG_APP_USER.AddClaim (P_Type => :P_Type,
												 P_Value => :P_Value,
                                                 P_ValueType => :P_ValueType,
                                                 P_Issuer => :P_Issuer,
                                                 P_AppUserId => :P_AppUserId);
            end;"
            , pr);
        return r;
    }

    public async Task<bool> RemoveClaim(int id)
    {
        await using var db = new OracleDb();

        var pr = new List<OracleParameter>
        {
            new("P_Id", OracleDbType.Int32, id, ParameterDirection.Input)
        };
        return await db.NonQueryAsync(@"
            begin
                  :result :=PKG_APP_USER.RemoveClaim (P_Id => :P_Id);
            end;", pr) > 0;
    }

    public async Task<int> RemoveClaimByType(SetClaimReq req)
    {
        await using var db = new OracleDb();

        OracleParameter values = new("P_Values", OracleDbType.NVarchar2, ParameterDirection.Input);
        values.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
        values.Value = req.Value;

        var pr = new List<OracleParameter>
        {
            values, new("P_Type", OracleDbType.NVarchar2, req.ClaimType, ParameterDirection.Input),
            new("P_UserId", OracleDbType.Int32, req.UserId, ParameterDirection.Input)
        };


        var r = await db.NonQueryAsync(@"
            begin
                  PKG_APP_USER.RemoveClaimByType (
												 P_Values => :P_Values,
                                                 P_Type=>:P_Type,
                                                 P_UserId => :P_UserId);
            end;"
            , pr);
        return r;
    }

    public async Task<bool> EditClaim(int claimId, string value)
    {
        await using var db = new OracleDb();

        var pr = new List<OracleParameter>
        {
            new("P_Id", OracleDbType.Int32, claimId, ParameterDirection.Input),
            new("P_Value", OracleDbType.NVarchar2, value, ParameterDirection.Input)
        };
        return await db.NonQueryAsync(@"
            begin
                  PKG_APP_USER.EditClaim (P_Id => :P_Id,
                                          P_Value=>:P_Value);
            end;", pr) > 0;
    }

    public async Task<bool> EditProfile(EditProfilReq req)
    {
        switch (req.EditType)
        {
            case EditType.Insert:
                if (!req.UserId.HasValue)
                    throw new Exception("userid is null");
                return await SetClaim(req.UserId.Value, req.ColumnName, req.NewValue) > 0;

            case EditType.Update:
                if (!req.OldId.HasValue)
                    throw new Exception("OldId is null");
                return await EditClaim(req.OldId.Value, req.NewValue);
            case EditType.Delete:
                if (!req.OldId.HasValue)
                    throw new Exception("OldId is null");
                return await RemoveClaim(req.OldId.Value);
        }

        return false;
    }

    public async Task<List<SearchUserResp>> SearchUsers(string name, int userId)
    {
        await using var db = new OracleDb();


        return (await db.Connection.QueryAsync<SearchUserResp>(@"
                    SELECT Value AS Name, c.AppUserId,
                           GET_USER_VALUE(AppUserId, 'Picture') AS Photo,
                           u.Email
                    FROM UserClaims c
                    LEFT JOIN App_user u ON c.AppUserId = u.Id
                    WHERE Type = 'Name'
                    ",
            new { P_userName = name })).ToList();
    }

    public async Task<List<RoleValueDto>> GetRoleValue()
    {
        await using var db = new OracleDb();
        return (await db.Connection.QueryAsync<RoleValueDto>(@"
                                select rv.ID, rv.NAME,
                                    rv.TYPE_ID, rt.NAME as TypeName from ROLE_VALUE rv
                                    left join ROLE_TYPE rt on rv.TYPE_ID = rt.ID"
        )).ToList();
    }


    public async Task<List<NotifResp>> GetNotif(int userId)
    {
        await using var db = new OracleDb();
        return (await db.Connection.QueryAsync<NotifResp>(@"
                                  select ID,
                                         TITLE,
                                         BODY,
                                         IS_READ IsRead,
                                         CREATE_DATE CreateDate,
                                         ICON,
                                         TYPE,
                                         USER_ID UserId
                                  from NOTIF WHERE USER_ID=:P_USER_ID
                                  ", new { P_USER_ID = userId })).ToList();
    }

    public async Task<int> GetUnReadNotifCount(int userId)
    {
        await using var db = new OracleDb();

        return await db.Connection.ExecuteScalarAsync<int>(@"select count(*) as count
                    from NOTIF WHERE USER_ID=:P_USER_ID and IS_READ=0", new { P_USER_ID = userId });
    }

    public async Task ReadNotif(IntListReq ids)
    {
       
        await using var db = new OracleDb();
        OracleParameter values = new("P_Ids", OracleDbType.Int32, ParameterDirection.Input);
        values.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
        values.Value = ids.Values.ToArray();
        var pr = new List<OracleParameter> { values };
        await db.NonQueryAsync(@"
            begin
                  PKG_APP_USER.ReadNotif (P_Ids => :P_Ids);
            end;"
            , pr);
    }


    public async Task InstPosition(PositionReq req)
    {
        await using var db = new OracleDb();
        var pr = new List<OracleParameter>
        {
            new("P_ID", OracleDbType.Int32, req.Id, ParameterDirection.Input),
            new("P_PARENT_ID", OracleDbType.Int32, req.ParentId.HasValue ? req.ParentId : DBNull.Value,
                ParameterDirection.Input),
            new("P_NAME", OracleDbType.NVarchar2, req.Name, ParameterDirection.Input),
            new("P_DESCRIPTION", OracleDbType.NVarchar2, req.Description, ParameterDirection.Input)
        };
        await db.NonQueryAsync(@"
          begin
    PKG_APP_USER.INSPOSITION(
            P_ID => :P_ID,
            P_PARENT_ID => :P_PARENT_ID,
            P_NAME => :P_NAME,
            P_DESCRIPTION => :P_DESCRIPTION
    );
end;"
            , pr);
    }

    public async Task DeletePosition(int id)
    {
        await using var db = new OracleDb();
        var pr = new List<OracleParameter>
        {
            new("P_ID", OracleDbType.Int32, id, ParameterDirection.Input)
        };
        await db.NonQueryAsync(@"
           begin
                MY_BLOG.PKG_APP_USER.DELETEPOSITION(P_ID => :P_ID);
           end;"
            , pr);
    }


    public async Task<List<PositionReq>> GetPosition()
    {
        await using var db = new OracleDb();

        return (await db.Connection.QueryAsync<PositionReq>(@"select ID, PARENT_ID  parentId, NAME, DESCRIPTION
                from POSITION")).ToList() ;
    }
}