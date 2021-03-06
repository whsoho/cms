﻿using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;

namespace SSCMS.Cli.Jobs
{
    public class PluginUnPublishJob : IJobService
    {
        public string CommandName => "plugin-unpublish";

        private bool _isHelp;

        private readonly IApiService _apiService;
        private readonly OptionSet _options;

        public PluginUnPublishJob(IApiService apiService)
        {
            _apiService = apiService;
            _options = new OptionSet
            {
                {
                    "h|help", "命令说明",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms-cli {CommandName} <pluginId>");
            Console.WriteLine("Summary: unpublishes a plugin. Example plugin id: sscms.hits");
            Console.WriteLine("Options:");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            if (context.Extras == null || context.Extras.Length == 0)
            {
                await CliUtils.PrintErrorAsync("missing required pluginId");
                return;
            }

            var (success, _, failureMessage) = _apiService.GetStatus();
            if (!success)
            {
                await CliUtils.PrintErrorAsync(failureMessage);
                return;
            }

            (success, failureMessage) = _apiService.UnPluginsPublish(context.Extras[0]);
            if (success)
            {
                await CliUtils.PrintSuccessAsync($"Unpublished {context.Extras[0]}.");
            }
            else
            {
                await CliUtils.PrintErrorAsync(failureMessage);
            }
        }
    }
}
