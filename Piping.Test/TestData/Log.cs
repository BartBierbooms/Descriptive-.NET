using Piping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Test.TestData
{
    public class Log : IValueAndSupplementExtension, IExposeLog
    {
        private List<string> logs = new List<string>();
        public List<string> Logs { get => logs; }

        public string LogTitle { get; private set; }

        string IExposeLog.Log => LogTitle;

        public void PostProcess(IExpose val) {
            if (val is IExposeLog l) {
                logs.Add(l.Log);
            }
        }

        public Log setLogTitle(string title) {
            this.LogTitle = title;
            return this;
        }
    }
}
