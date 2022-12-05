using Microsoft.Extensions.Options;
using SensitiveWordsAPI.DAL.Entities;
using SensitiveWordsAPI.DAL.Repository;
using System.Collections.Generic;
using SensitiveWordsAPI.DAL.Utility;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace SensitiveWordsAPI.BL.Service
{
    public class SensiveWordsService : ISensiveWordsService
    {
        private IWordsRepostitory _wordsRepostitory;
        private readonly IOptions<AppSettings> _appSettings;

        public SensiveWordsService(IWordsRepostitory wordsRepostitory, IOptions<AppSettings> appSettings)
        {
            _wordsRepostitory = wordsRepostitory;
            _appSettings = appSettings;
        }

        public string MaskSensitiveWords(string sensitiveText, string connction)
        {
            var sensitiveTextWords = sensitiveText.Split(" ");
            StringBuilder maskWord = new StringBuilder();
            var data = _wordsRepostitory.GetAllSensitiveWords(connction);

            List<string> wordsList = new List<string>();
            foreach (var item in sensitiveTextWords)
            {

                var word = data.Where(s => s.WordContext.ToUpper() == item.ToUpper()).FirstOrDefault()?.WordContext;

                if (word != null)
                {
                    maskWord.Append(word.Mask(0, word.Length));
                    maskWord.Append(" ");
                }
                else
                {                 
                    maskWord.Append(item);
                    maskWord.Append(" ");
                }                 
            }

            return maskWord.ToString();

        }
    }
}
