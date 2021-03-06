﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Core.Services.Commands;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;

namespace NxPlx.Core.Services
{
    public static class AdminCommandService
    {
        private static readonly Dictionary<string, CommandBase> Commands = new List<CommandBase>
        {
            new DeleteWatchingProgressCommand(),
            new DeleteSubtitlePreferencesCommand(),
        }.ToDictionary(c => c.Name);

        public static IEnumerable<CommandDto> AvailableCommands()
        {
            return ResolveContainer.Default.Resolve<IDtoMapper>().Map<CommandBase, CommandDto>(Commands.Values);
        }

        public static async Task<string?> InvokeCommand(string command, string[] args)
        {
            if (Commands.TryGetValue(command, out var cmd))
            {
                return await cmd.Execute(args);
            }

            return null;
        }
    }
}