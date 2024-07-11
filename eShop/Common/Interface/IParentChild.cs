using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interface
{
    public interface IParentChild
    {
        public IParentChild? Parent { get; set; }
        public IList<IParentChild>? Children { get; set; }
    }
}
