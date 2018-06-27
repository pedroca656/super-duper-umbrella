using System.Collections.Generic;
using BusPoa.Models;

namespace BusPoa.MobileAppService.Models
{
    public interface ILocalizacaoRepository
    {
        Localizacao GetLast(string linha);
        IEnumerable<Localizacao> GetAll();
        void Add(Localizacao item);
    }
}