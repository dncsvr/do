﻿using System.Security.Claims;

namespace Do;

public static class AuthorizationExtensions
{
    public static bool HasClaim(this ClaimsPrincipal claims, string claimType) =>
        claims.HasClaim(c => c.Type == claimType);
}