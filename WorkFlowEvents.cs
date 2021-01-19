using System.Collections.Generic;

namespace wf_core_sample
{
    public class WorkFlowEvents
    {
        private readonly List<string> _events = new List<string>();

        public void Add(string wf) => _events.Add(wf);

        public IEnumerable<string> Get() => _events;
    }
}
