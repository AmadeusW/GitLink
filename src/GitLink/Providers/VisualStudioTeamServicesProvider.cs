﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualStudioTeamServicesProvider.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Providers
{
    using System;
    using System.Text.RegularExpressions;
    using Catel;
    using GitTools.Git;

    public class VisualStudioTeamServicesProvider : ProviderBase
    {
        private static readonly Regex HostingUrlPattern = new Regex(@"(?<url>(?<companyurl>(?:https://)?(?<accountname>([a-zA-Z0-9\-\.]*)?)\.visualstudio\.com/)(?<project>[a-zA-Z0-9\-\.]*)/?_git/(?<repo>[^/]+))");

        public VisualStudioTeamServicesProvider()
            : base(new GitPreparer())
        {
        }

        public override string RawGitUrl => string.Empty;

        public override bool Initialize(string url)
        {
            var match = HostingUrlPattern.Match(url);

            if (!match.Success)
            {
                return false;
            }

            CompanyName = match.Groups["accountname"].Value;
            CompanyUrl = match.Groups["companyurl"].Value;

            ProjectName = match.Groups["project"].Value;
            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                ProjectName = match.Groups["repo"].Value;
            }

            // In the VSTS provider, the ProjectUrl will represent
            // the repository's name.
            ProjectUrl = match.Groups["repo"].Value;

            if (!CompanyUrl.StartsWithIgnoreCase("https://"))
            {
                CompanyUrl = String.Concat("https://", CompanyUrl);
            }

            return true;
        }
    }
}