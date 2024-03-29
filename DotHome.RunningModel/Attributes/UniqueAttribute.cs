﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel.Attributes
{
    /// <summary>
    /// Marks <see cref="Block"/> parameter as unique. That means that two blocks cannot have the same value of this actual property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : ValidationAttribute
    {
        // There can hardly be anything as we must see the whole project to perform validation
        // It only acts as a markup
    }
}
