﻿using System;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.UI.Infrastructure.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AnonymousOnlyAttribute : Attribute, IAuthorizeData
    {
        private const string PolicyName = "AnonymousOnly";

        public static void AddPolicy(AuthorizationOptions options) => options
            .AddPolicy(PolicyName, builder => builder
                .RequireAssertion(context =>
                {
                    if (context.User.Identity.IsAuthenticated)
                    {
                        context.Fail();
                        return false;
                    }

                    return true;
                }));

        string? IAuthorizeData.AuthenticationSchemes { get => null; set => throw new NotSupportedException(); }
        string? IAuthorizeData.Policy { get => PolicyName; set => throw new NotSupportedException(); }
        string? IAuthorizeData.Roles { get => null; set => throw new NotSupportedException(); }
    }
}
