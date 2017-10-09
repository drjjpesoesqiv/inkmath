using System;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace InkMath
{
    class SpeechInput
    {
        public SpeechRecognizer recognizer;

        public SpeechInput()
        {
            recognizer = new SpeechRecognizer();
        }

        public async Task<string> Recognize()
        {
            string output = "";
            await recognizer.CompileConstraintsAsync();
            SpeechRecognitionResult res = await recognizer.RecognizeAsync();
            if (res.Status == SpeechRecognitionResultStatus.Success)
                output = res.Text;
            return output;
        }
    }
}
