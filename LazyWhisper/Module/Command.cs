﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Net;

namespace LazyWhisper.Module
{
    public class Command : ModuleBase
    {
        private IDataRepository _dataRepository;

        private static readonly string[] defaultCommands = 
        {
            "add", "remove ", "edit", "list", "help"
        };

        public Command(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [Command("add")]
        public async Task AddCommandAsync(params string[] args)
        {
            if (args.Length < 2 || args[0].Contains('!'))
            {
                await ReplyAsync("!add CommandName Message");
                return;
            }

            if (defaultCommands.Contains(args[0]))
            {
                await ReplyAsync("Default commands cannot be registered");
                return;
            }
            var message = args.Skip(1).Aggregate((a, b) => $"{a} {b}");
            await _dataRepository.InsertAsync(args[0], message, Context.Channel.Id);
            await ReplyAsync($"The {args[0]} command has been registered");
        }

        [Command("edit")]
        public async Task EditCommandAsync(params string[] args)
        {
            if (args.Length < 2)
            {
                await ReplyAsync("!edit CommandName Message");
                return;
            }

            var result = await _dataRepository.FindAsync(args[0], Context.Channel.Id);
            if (result == null)
            {
                await ReplyAsync("The command is not registered.");
                return;
            }

            var message = args.Skip(1).Aggregate((a, b) => $"{a} {b}");
            await _dataRepository.UpdateAsync(args[0], message, Context.Channel.Id);
            await ReplyAsync($"The {args[0]} command has been changed");
        }
    }
}
