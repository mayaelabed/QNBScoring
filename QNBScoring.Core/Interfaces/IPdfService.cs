<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QNBScoring.Core.Interfaces
{
    // Change the access modifier of the IPdfService interface to 'public'  
    public interface IPdfService
    {
        //byte[] GenerateReport(ScoringResult result);
=======
﻿// Core/Interfaces/IPdfService.cs

using QNBScoring.Core.DTOs;

namespace QNBScoring.Core.Interfaces
{
    public interface IPdfService
    {
        string GenerateReport(List<ClientRequestDto> results);
>>>>>>> 42f6f51 (additionnal fuctionnality)
    }
}
