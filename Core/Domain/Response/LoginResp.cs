﻿namespace Domain.Response;

public class LoginResp
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}