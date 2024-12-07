using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yagg_vhf.Contract;

namespace yagg_vhf.Output
{
    internal abstract class Output
    {
        protected CompleteResultSet completeResults;
        protected Output(CompleteResultSet complete)
        {
            this.completeResults = complete;
        }

        public abstract void Produce();

    }
}
