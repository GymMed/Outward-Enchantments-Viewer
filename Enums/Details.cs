using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchanmentsViewer.Enums
{
    [Flags]
    public enum Details
    {
        None = 0,
        All = ~0,

        Vitals = 1 << 1,
        MaxVitals = 1 << 2,
        Needs = 1 << 3,
        Corruption = 1 << 4,
        RegenRates = 1 << 5,
        StatusEffects = 1 << 6,
        StatusCures = 1 << 7,
        Cooldown = 1 << 8,
        Costs = 1 << 9,
    }
}
