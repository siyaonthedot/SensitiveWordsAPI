using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SensitiveWordsAPI.Model;
using SensitiveWordsAPI.DAL.Utility;
using SensitiveWordsAPI.DAL.Repository;
using SensitiveWordsAPI.BL.Service;
using System;
using Microsoft.AspNetCore.Cors;
using SensitiveWordsAPI.DAL.Entities;
using AutoMapper;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

namespace SensitiveWordsAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/SensitiveWord")]

    public class SensitiveWordController : Controller
    {

        private readonly IOptions<AppSettings> appSettings;
        private readonly ISensiveWordsService _sensiveWordsService;
        private readonly ICacheService _cacheService;
        private readonly IWordsRepostitory _wordsRepostitory;
        private readonly IMapper _mapper;


        public SensitiveWordController(IOptions<AppSettings> appSettings,
            ISensiveWordsService sensiveWordsService, IWordsRepostitory wordsRepostitory,
            IMapper mapper, ICacheService cacheService)
        {
            this.appSettings = appSettings;
            this._sensiveWordsService = sensiveWordsService;
            this._wordsRepostitory = wordsRepostitory;
            this._mapper = mapper;
            this._cacheService = cacheService;
        }

        [HttpPost]
        [Route("MaskSensitiveWords")]
        public IActionResult MaskSensitiveWords(string textMessage)
        {
            var cacheData = _cacheService.GetData<IEnumerable<SensitiveWord>>("SensitiveWord");
            if (cacheData != null)
            {
               textMessage = _cacheService.CachedMaskSensitiveWords(textMessage, cacheData);
               return Ok(textMessage);
            }
            textMessage = _sensiveWordsService.MaskSensitiveWords(textMessage, appSettings.Value.DbConn);
            return Ok(textMessage);
        }
        
        [HttpGet]
        [Route("GetAllSensitiveWords")]
        [EnableCors("OriginsPolicy")]
        public IActionResult GetAllSensitiveWords()
        {
            var cacheData = _cacheService.GetData<IEnumerable<SensitiveWord>>("SensitiveWord");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }

            var data = _wordsRepostitory.GetAllSensitiveWords(appSettings.Value.DbConn);
            var expirationTime = DateTimeOffset.Now.AddMinutes(1.0);
            _cacheService.SetData<IEnumerable<SensitiveWord>>("SensitiveWord", data, expirationTime);

            return Ok(data);
        }
  
        [HttpGet]
        [Route("GetSensitiveWordByID")]
        [EnableCors("OriginsPolicy")]
        public IActionResult GetSensitiveWordByID(int id)
        {
            var data = _wordsRepostitory.GetSensitiveWord(id,appSettings.Value.DbConn);

            if (data != null) { return Ok(data); }

            return NotFound(data);
        }

        [HttpPost]
        [Route("SaveSensitiveWord")]
        [EnableCors("OriginsPolicy")]
        public IActionResult SaveSensitiveWord([FromBody] SensitiveWordsModel model)
        {
            var commandModel = _mapper.Map<SensitiveWord>(model);
            var data = _wordsRepostitory.SaveSensitiveWord(commandModel, appSettings.Value.DbConn);

            if (data == "C200")
            {
                return Ok(data);
            }
  
            return NotFound(data);
        }

        [HttpPut]
        [Route("UpdateSensitiveWord")]
        [EnableCors("OriginsPolicy")]
        public IActionResult UpdateSensitiveWord([FromBody] SensitiveWordsModel model)
        {
            var commandModel = _mapper.Map<SensitiveWord>(model);
            var data = _wordsRepostitory.UpdateSensitiveWord(commandModel, appSettings.Value.DbConn);
     
            return Ok(data);  
        }

        [HttpDelete]
        [Route("DeleteSensitiveWord")]
        [EnableCors("OriginsPolicy")]
        public IActionResult DeleteSensitiveWord(int id)
        {
            var data = _wordsRepostitory.DeleteSensitiveWord(id, appSettings.Value.DbConn);
            if (data == "C200")
            {
                return Ok(data);
            }
            return NotFound(data);
        }


    }
}
