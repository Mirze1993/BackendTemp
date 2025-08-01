﻿namespace Domain.Response.User;

public class UserInfoResp
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
}