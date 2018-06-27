using System.Collections.Generic;
using BusPoa.Models;

namespace BusPoa.MobileAppService.Models
{
    public interface ILocalizacaoRepository
    {
        Localizacao GetLast();
        IEnumerable<Localizacao> GetAll();
        void Add(Localizacao item);
    }
}