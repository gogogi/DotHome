﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DotHome.Config.Tools
{
    public class Command : ICommand
    {
        private static List<Command> commands = new List<Command>();

        private Func<bool> canExecute;
        private Func<Task<bool>> canExecuteTask;
        private Action executed;
        private Func<Task> executedTask;

        public event EventHandler CanExecuteChanged;

        public Command(Func<Task<bool>> canExecuteTask, Action executed)
        {
            this.canExecuteTask = canExecuteTask;
            this.executed = executed;
            commands.Add(this);
        }

        public Command(Func<bool> canExecute, Action executed)
        {
            this.canExecute = canExecute;
            this.executed = executed;
            commands.Add(this);
        }

        public Command(Func<bool> canExecute, Func<Task> executedTask)
        {
            this.canExecute = canExecute;
            this.executedTask = executedTask;
            commands.Add(this);
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null) return canExecute();
            else
            {
                var cet = canExecuteTask();
                cet.Wait();
                return cet.Result;
            }
        }

        public async void Execute(object parameter)
        {
            if (executed != null) executed();
            else await executedTask();
            ForceChanges();
        }

        public void ForceChange()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public static void ForceChanges()
        {
            foreach(var command in commands)
            {
                command.ForceChange();
            }
        }
    }
}
