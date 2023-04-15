class TextToSpeech {

    speak(text: string, voiceUri: string = "") {
        return new Promise<void>((resolve) => {
            let voices = speechSynthesis.getVoices();
            let voice;
            if (voiceUri === "" && voices.filter(i => i.lang == "en-GB").length > 0) {
                voice = voices.filter(i => i.lang == "en-GB")[0];
            } else {
                voice = voiceUri === "" ? voices.filter(i => i.default)[0] : voices.filter(i => i.voiceURI === voiceUri)[0];
            }

            let utterance = new SpeechSynthesisUtterance();
            utterance.text = text;
            utterance.lang = "en"
            utterance.voice = voice;
            speechSynthesis.speak(utterance);
            utterance.addEventListener("end", () => {
                resolve();
            });
        });
    }

    cancel() {
        speechSynthesis.cancel();
    }

    getVoices() {
        return new Promise((resolve) => {

            let voices = speechSynthesis.getVoices();

            if (voices.length == 0) {
                speechSynthesis.onvoiceschanged = () => {

                    voices = speechSynthesis.getVoices();
                    let mappedVoices = voices.map((voice) => ({ Name: voice.name, Lang: voice.lang, VoiceURI: voice.voiceURI }));
                    resolve(mappedVoices);

                }
            } else {
                let mappedVoices = voices.map((voice) => ({ Name: voice.name, Lang: voice.lang, VoiceURI: voice.voiceURI }));
                resolve(mappedVoices);
            }

        });
    }
}

export function createTextToSpeech(): TextToSpeech {
    return new TextToSpeech();
}
