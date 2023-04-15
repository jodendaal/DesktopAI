class SpeechToText {

    private onTextReceived: ((data: string) => void) | null = null;
    recognition = new window.webkitSpeechRecognition();
    transScript = "";

    constructor() {
        this.recognition.continuous = true; 
        this.recognition.interimResults = true;
        this.recognition.onresult = this.onRecognition;
    }

    onRecognition = (event: any) => {
        for (let i = event.resultIndex; i < event.results.length; ++i) {

            if (event.results[i].isFinal) {
                this.transScript += event.results[i][0].transcript;

                if (this.onTextReceived) {
                    this.onTextReceived(this.transScript)
                }
            }
            else if (this.onTextReceived) {
                this.onTextReceived(event.results[i][0].transcript)
            }
        }
    }

    getTranscript() {
        return this.transScript;
    }

    startProxy(instance, callbackMethod) {
        this.start((data: string) => {
            instance.invokeMethodAsync(callbackMethod, data);
        });
    }

    start(callback: (data: string) => void) {
        this.transScript = "";
        this.onTextReceived = callback;
        this.recognition.start();
    }

    stop() {
        this.recognition.stop();
        if (this.onTextReceived) {
            this.onTextReceived(this.transScript)
        }
    }
}

declare global {
    interface Window {
        webkitSpeechRecognition: any;
    }
}


export function createSpeechToText(): SpeechToText {
    return new SpeechToText();
}
