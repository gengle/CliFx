﻿using System.Threading.Tasks;
using CliFx.Services;

namespace CliFx.Tests.TestCommands
{
    public class NonAnnotatedCommand : ICommand
    {
        public Task ExecuteAsync(IConsole console) => Task.CompletedTask;
    }
}