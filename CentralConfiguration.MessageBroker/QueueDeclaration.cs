using System.Collections;
using System.Collections.Generic;

namespace CentralConfiguration.MessageBroker
{
    public class QueueDeclaration
    {
        public QueueDeclaration()
        {
            Args = new Dictionary<string, object>();
        }
        public string Name { get; set; }
        public bool IsDurable { get; set; }
        public bool IsExclusive { get; set; }
        public bool HasAutoDelete { get; set; }
        public IDictionary<string, object> Args { get; set; }
    }
}
