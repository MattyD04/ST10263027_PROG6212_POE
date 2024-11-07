﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ST10263027_PROG6212_POE.Roles
{
    public class AuthorisingRoles : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public AuthorisingRoles(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var userType = httpContext.Session.GetString("UserType");

            if (!_allowedRoles.Contains(userType))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
            }
        }
    }
}
