﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Services;

namespace CliFx.Tests.TestCommands
{
    [Command("concat", Description = "Concatenate strings.")]
    public class ConcatCommand : ICommand
    {
        [CommandOption('i', IsRequired = true, Description = "Input strings.")]
        public IReadOnlyList<string> Inputs { get; set; }

        [CommandOption('s', Description = "String separator.")]
        public string Separator { get; set; } = ""; 
        
        public Task ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine(string.Join(Separator, Inputs));
            return Task.CompletedTask;
        }
    }
}