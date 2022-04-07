using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines_test.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDutchmanAirlines_test
{
    internal class TestTools
    {
        public static FlyingDutchmanAirlinesContext InitializeInMemoryContext(string databaseName)
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
        new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase(databaseName).Options;
            return  new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

        }
    }
}
