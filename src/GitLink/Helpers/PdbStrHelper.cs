﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbStrHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink
{
    using System.Diagnostics;
    using Catel;
    using Catel.Logging;

    internal static class PdbStrHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        internal static void Execute(string pdbStrFileName, string projectPdbFile, string pdbStrFile)
        {
            Argument.IsNotNullOrWhitespace(() => projectPdbFile);
            Argument.IsNotNullOrWhitespace(() => pdbStrFile);

            var processStartInfo = new ProcessStartInfo(pdbStrFileName)
            {
                Arguments = string.Format("-w -s:srcsrv -p:\"{0}\" -i:\"{1}\"", projectPdbFile, pdbStrFile),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var process = new Process();
            bool errorsPrinted = false;
            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    Log.Info(e.Data);
                }
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    Log.Error(e.Data);
                    errorsPrinted = true;
                }
            };
            process.EnableRaisingEvents = true;
            process.StartInfo = processStartInfo;
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            var processExitCode = process.ExitCode;
            if (processExitCode != 0)
            {
                throw Log.ErrorAndCreateException<GitLinkException>("PdbStr exited with unexpected error code '{0}'", processExitCode);
            }

            // PdbStr can print errors and still return 0 for its exit code.
            if (errorsPrinted)
            {
                throw Log.ErrorAndCreateException<GitLinkException>("PdbStr printed errors.");
            }
        }
    }
}