namespace Appilcation.CustomerAttributes;

public class ReqRespLogAttribute:Attribute
{
    public ReqRespLogEnum ReqResp=ReqRespLogEnum.ReqResp;
    public int MaxReqSize = 4096;
    public int MaxRespSize = 4096;
}

public enum ReqRespLogEnum
{
    Request=1<<0,
    Response=1<<1,
    ReqResp=Request|Response,
}