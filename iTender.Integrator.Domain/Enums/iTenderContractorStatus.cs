using System;
using System.Collections.Generic;
using System.Text;

namespace iTender.Integrator.Domain.Enums
{
    public enum iTenderContractorStatus
    {
        Active = 1,
        Suspended = 100000010,
        Expired = 100000006,
        DeRegistered = 100000005,
        Removed = 100000009
    }
}
