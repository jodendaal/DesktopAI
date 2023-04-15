class SpeechToText {
    constructor() {
        this.onTextReceived = null;
        this.recognition = new window.webkitSpeechRecognition();
        this.transScript = "";
        this.onRecognition = (event) => {
            for (let i = event.resultIndex; i < event.results.length; ++i) {
                if (event.results[i].isFinal) {
                    this.transScript += event.results[i][0].transcript;
                    if (this.onTextReceived) {
                        this.onTextReceived(this.transScript);
                    }
                }
                else if (this.onTextReceived) {
                    this.onTextReceived(event.results[i][0].transcript);
                }
            }
        };
        this.recognition.continuous = true;
        this.recognition.interimResults = true;
        this.recognition.onresult = this.onRecognition;
    }
    getTranscript() {
        return this.transScript;
    }
    startProxy(instance, callbackMethod) {
        this.start((data) => {
            instance.invokeMethodAsync(callbackMethod, data);
        });
    }
    start(callback) {
        this.transScript = "";
        this.onTextReceived = callback;
        this.recognition.start();
    }
    stop() {
        this.recognition.stop();
        if (this.onTextReceived) {
            this.onTextReceived(this.transScript);
        }
    }
}
export function createSpeechToText() {
    return new SpeechToText();
}
