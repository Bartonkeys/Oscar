using System;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.MRIT.Core.Enums;

namespace Oscar.Mrit.Features.MRITIntegration.Common
{
    public class MritMapperFactory
    {
        private readonly OscarContext _oscarContext;

        public MritMapperFactory(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public IMritMapper Create(VwOnMusicFelixWorks felixWork)
        {
            var serialLevel = (SerialLevel) felixWork.SerialLevel;
            return serialLevel switch
            {
                SerialLevel.OneOff => new MritOneOffMapper(_oscarContext, felixWork),
                SerialLevel.SeriesHeader => new MritSeriesMapper(_oscarContext, felixWork),
                _ => throw new ArgumentOutOfRangeException(nameof(serialLevel), serialLevel, null),
            };
        }

    }

}
