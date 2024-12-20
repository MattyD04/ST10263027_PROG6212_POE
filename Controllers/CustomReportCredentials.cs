﻿using Microsoft.Reporting.WebForms;
using System.Net;
using System.Security.Principal;
//file recommended by the IDE to handle the customer credentials for report generation
public class CustomReportCredentials : IReportServerCredentials
{
    public WindowsIdentity ImpersonationUser => null; // Use current user's identity

    public ICredentials NetworkCredentials => CredentialCache.DefaultNetworkCredentials;

    public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
    {
        authCookie = null;
        user = password = authority = null;
        return false; // Not using forms authentication
    }
}
