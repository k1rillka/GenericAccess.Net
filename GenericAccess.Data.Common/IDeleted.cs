using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAccess.Data.Common
{
    public interface IDeleted
    {
        bool IsDeleted { get; set; }
    }
}
