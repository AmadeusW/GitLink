﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHubProvider.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Providers
{
    using System;
    using System.Text.RegularExpressions;
    using GitTools.Git;

    public class GitHubProvider : ProviderBase
    {
        private static readonly Regex HostingUrlPattern = new Regex(@"^(?<url>(?<companyurl>(?:https://)?github\.com/(?<company>[^/]+))/(?<project>[^/]+?)(\.git)?)$");

        public GitHubProvider()
            : base(new GitPreparer())
        {
        }

        public override string RawGitUrl => $"https://raw.github.com/{CompanyName}/{ProjectName}";

        public override bool Initialize(string url)
        {
            var match = HostingUrlPattern.Match(url);

            if (!match.Success)
            {
                return false;
            }

            CompanyName = match.Groups["company"].Value;
            CompanyUrl = match.Groups["companyurl"].Value;

            ProjectName = match.Groups["project"].Value;
            ProjectUrl = match.Groups["url"].Value;

            if (!CompanyUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                CompanyUrl = String.Concat("https://", CompanyUrl);
            }

            if (!ProjectUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                ProjectUrl = String.Concat("https://", ProjectUrl);
            }

            return true;
        }
    }
}