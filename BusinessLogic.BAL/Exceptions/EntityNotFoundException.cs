using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entity, int id)
            : base($"Entity {entity} with an id of {id} was not found in the system.")
        {

        }
    }
}
