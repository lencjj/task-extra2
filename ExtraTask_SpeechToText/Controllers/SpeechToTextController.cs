using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExtraTask_SpeechToText.Controllers
{
    [Route("api/SpeechToText")]
    [ApiController]
    public class SpeechToTextController : ControllerBase
    {
        private IHostingEnvironment _env;

        public SpeechToTextController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        [Route("Convert")]
        public string Convert(IFormFile file)
        {
            var dir = _env.ContentRootPath;
            string path = Path.Combine(dir, file.FileName);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                SampleRateHertz = 44100,
                LanguageCode = "en",
            }, RecognitionAudio.FromFile(path));
            string message = "";
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                    message += alternative.Transcript;
                }
            }
            return message;
        }
    }
}