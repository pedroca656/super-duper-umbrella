using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusPoa.MobileAppService.Models;
using BusPoa.Models;
using Microsoft.AspNetCore.Mvc;

namespace BusPoa.MobileAppService.Controllers
{
    [Route("api/[controller]")]
    public class LocalizacaoController : Controller
    {
        private readonly ILocalizacaoRepository ItemRepository;

        public LocalizacaoController(ILocalizacaoRepository itemRepository)
        {
            ItemRepository = itemRepository;
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(ItemRepository.GetAll());
        }

        [HttpGet("{Id}")]
        public IActionResult Get(string id)
        {
            return Ok(ItemRepository.GetLast());
        }


        [HttpPost]
        public IActionResult Create([FromBody]Localizacao item)
        {
            try
            {
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid State");
                }

                ItemRepository.Add(item);

            }
            catch (Exception)
            {
                return BadRequest("Error while creating");
            }
            return Ok(item);
        }
    }
}
