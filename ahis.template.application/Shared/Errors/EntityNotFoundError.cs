using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.application.Shared.Errors
{
    public class EntityNotFoundError : Error
    {
        public EntityNotFoundError(string entityName, object key)
            : base($"Entity '{entityName}' with key '{key}' was not found.")
        {
            EntityName = entityName;
            Key = key;
        }
        public string EntityName { get; }
        public object Key { get; }
        public override string ToString()
        {
            return $"{base.ToString()} (Entity: {EntityName}, Key: {Key})";
        }
    }
}
