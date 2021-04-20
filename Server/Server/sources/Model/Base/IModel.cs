using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Model
{
    interface IModel { }

    abstract class AbstractModel<T> where T : class, IModel, new()
    {
        public static T Instance { get; private set; } = new();
    }
}