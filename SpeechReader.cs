using System;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace InkMath
{
    class SpeechReader
    {
        public static string defaultVoice = "Microsoft Zira";

        public static async void ReadText(string text)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.Voice = GetVoice(defaultVoice);

            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(text);

            MediaElement me = new MediaElement();
            me.SetSource(stream, stream.ContentType);
            me.Play();
        }

        public static VoiceInformation GetVoice(string displayName)
        {
            foreach (var voice in SpeechSynthesizer.AllVoices)
                if (voice.DisplayName == displayName)
                    return voice;
            return SpeechSynthesizer.AllVoices[0];
        }
    }
}
