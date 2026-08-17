using System.Threading.Tasks;
using Oscar.Core.Entities;
using Oscar.Data.Context;

namespace Oscar.Mrit.Features.MRITIntegration.Common
{
    public class MritOneOffMapper: MritMapper
    {
        public MritOneOffMapper(OscarContext oscarContext, VwOnMusicFelixWorks felixWork): base(oscarContext, felixWork)
        {
            
        }

        protected override async Task Process()
        {
            MritProductionModel.IsOneOff = true;
            MritProductionModel.People = FelixWork.GetPeopleFrom();
        }
    }
}
