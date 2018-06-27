using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusPoa.MobileAppService.Models;
using BusPoa.Models;

namespace BusPoa.Models
{
    public class LocalizacaoRepository : ILocalizacaoRepository
	{
		private static ConcurrentDictionary<string, Localizacao> items =
			new ConcurrentDictionary<string, Localizacao>();

		public LocalizacaoRepository()
		{
			//Add(new Localizacao() { Id = Guid.NewGuid().ToString(), Text = "Item 1", Description = "This is an item description." });
			//Add(new Item { Id = Guid.NewGuid().ToString(), Text = "Item 2", Description = "This is an item description." });
			//Add(new Item { Id = Guid.NewGuid().ToString(), Text = "Item 3", Description = "This is an item description." });
		}

	    public Localizacao GetLast()
	    {
	        var ret = items.Values.OrderByDescending(o => o.Data).FirstOrDefault();
	        return ret;
	    }

        public IEnumerable<Localizacao> GetAll()
		{
			return items.Values.Take(50);
		}

		public void Add(Localizacao item)
		{
			item.Id = Guid.NewGuid().ToString();
			items[item.Id] = item;
		}

	}
}