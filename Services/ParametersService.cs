using Database;
using Database.Model;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public class ParametersService
    {
        private readonly StackGameContext stackGameContext;

        public ParametersService(StackGameContext stackGameContext)
        {
            this.stackGameContext = stackGameContext;
        }

        public ParameterDAO GetParameter(string key)
        {
            return stackGameContext.Parameters.FirstOrDefault(p => p.Key == key);
        }
    }
}
