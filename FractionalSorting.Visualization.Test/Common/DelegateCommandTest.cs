using FractionalSorting.Visualization.Common;
using System;
using System.Windows.Input;
using Xunit;

namespace FractionalSorting.Visualization.Test.Common
{
    public class DelegateCommandTest
    {
        [Fact]
        public void ExecuteCouldBeInvoked()
        {
            bool isExecuted = false;
            ICommand cmd = new DelegateCommand(() => isExecuted = true);
            cmd.Execute(null);
            Assert.True(cmd.CanExecute(null));
            Assert.True(isExecuted);
        }

        [Fact]
        public void FuncReturnFalse_CannotExecute()
        {
            bool isExecuted = false;
            DelegateCommand cmd = new DelegateCommand(() => isExecuted = true, () => false);
            cmd.Execute(null);
            Assert.False(cmd.CanExecute(null));            
            Assert.False(isExecuted);
        }

        [Fact]
        public void CanRaiseEvent()
        {
            bool eventRaised = false;
            DelegateCommand cmd = new DelegateCommand(() => { });
            cmd.CanExecuteChanged += (s, e) => eventRaised = true;
            cmd.RaiseCanExecuteChanged();
            Assert.True(eventRaised);
        }
    }
}
