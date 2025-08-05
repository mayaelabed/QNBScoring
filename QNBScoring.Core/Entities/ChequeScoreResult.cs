using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Core.Entities
{
    
        public class ChequeScoreResult
        {
            public decimal SumLast12Months { get; set; }
            public decimal AvgMonthly { get; set; }
            public decimal GlobalAverage { get; set; }
            public decimal StdDev { get; set; }
            public decimal AvgCountPerMonth { get; set; }
        }
    

}
