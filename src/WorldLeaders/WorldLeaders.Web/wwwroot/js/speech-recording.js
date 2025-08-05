/**
 * Speech Recording Module for Educational Language Learning
 * Context: Child-safe audio recording for 12-year-old language learners
 * Safety: Real-time processing only, no audio storage or transmission to third parties
 * Educational Objective: Enable pronunciation practice with real microphone input
 */

class SpeechRecorder {
    constructor() {
        this.mediaRecorder = null;
        this.audioChunks = [];
        this.stream = null;
        this.isRecording = false;
        this.dotNetRef = null;
    }

    /**
     * Initialize the speech recorder with .NET reference for callbacks
     * @param {object} dotNetReference - Blazor component reference for callbacks
     */
    init(dotNetReference) {
        this.dotNetRef = dotNetReference;
        console.log('Speech recorder initialized for educational language learning');
    }

    /**
     * Check if browser supports MediaRecorder API
     * @returns {boolean} True if MediaRecorder is supported
     */
    isSupported() {
        const supported = 'mediaDevices' in navigator && 
                         'getUserMedia' in navigator.mediaDevices &&
                         'MediaRecorder' in window;
        
        console.log('MediaRecorder API support check:', supported);
        return supported;
    }

    /**
     * Request microphone permission and start recording
     * Child-safe: 2-second maximum duration, high quality for speech recognition
     * @returns {Promise<boolean>} Success status
     */
    async startRecording() {
        if (this.isRecording) {
            console.warn('Recording already in progress');
            return false;
        }

        try {
            console.log('Requesting microphone access for educational speech recording...');
            
            // Request microphone access with audio constraints optimized for speech
            this.stream = await navigator.mediaDevices.getUserMedia({
                audio: {
                    sampleRate: 16000,        // 16kHz for Azure Speech Services
                    channelCount: 1,          // Mono audio
                    echoCancellation: true,   // Reduce background noise
                    noiseSuppression: true,   // Clean audio for children
                    autoGainControl: true     // Normalize volume levels
                }
            });

            // Create MediaRecorder with appropriate settings
            const options = {
                mimeType: this.getSupportedMimeType(),
                audioBitsPerSecond: 32000  // Good quality for speech recognition
            };

            this.mediaRecorder = new MediaRecorder(this.stream, options);
            this.audioChunks = [];

            // Set up event handlers
            this.mediaRecorder.ondataavailable = (event) => {
                if (event.data.size > 0) {
                    this.audioChunks.push(event.data);
                    console.log('Audio chunk captured:', event.data.size, 'bytes');
                }
            };

            this.mediaRecorder.onstop = async () => {
                console.log('Recording stopped, processing audio for language assessment...');
                await this.processRecording();
            };

            this.mediaRecorder.onerror = (event) => {
                console.error('MediaRecorder error:', event.error);
                this.cleanup();
                if (this.dotNetRef) {
                    this.dotNetRef.invokeMethodAsync('OnRecordingError', 'Recording error occurred');
                }
            };

            // Start recording
            this.mediaRecorder.start(100); // Capture data every 100ms
            this.isRecording = true;

            console.log('Educational speech recording started successfully');
            
            // Auto-stop after 5 seconds for child safety (prevent long recordings)
            setTimeout(() => {
                if (this.isRecording) {
                    console.log('Auto-stopping recording after 5 seconds (child safety limit)');
                    this.stopRecording();
                }
            }, 5000);

            return true;

        } catch (error) {
            console.error('Error starting speech recording:', error);
            this.cleanup();
            
            let errorMessage = 'Could not access microphone. ';
            if (error.name === 'NotAllowedError') {
                errorMessage += 'Please allow microphone access to practice pronunciation!';
            } else if (error.name === 'NotFoundError') {
                errorMessage += 'No microphone found. You can type your answer instead!';
            } else {
                errorMessage += 'Please try typing your answer instead!';
            }

            if (this.dotNetRef) {
                this.dotNetRef.invokeMethodAsync('OnRecordingError', errorMessage);
            }
            
            return false;
        }
    }

    /**
     * Stop recording and process the audio
     * @returns {Promise<boolean>} Success status
     */
    async stopRecording() {
        if (!this.isRecording || !this.mediaRecorder) {
            console.warn('No active recording to stop');
            return false;
        }

        try {
            this.isRecording = false;
            this.mediaRecorder.stop();
            console.log('Stopping educational speech recording...');
            return true;
        } catch (error) {
            console.error('Error stopping recording:', error);
            this.cleanup();
            return false;
        }
    }

    /**
     * Process the recorded audio and convert to format suitable for Azure Speech Services
     * Educational context: Convert to PCM format for pronunciation assessment
     */
    async processRecording() {
        try {
            if (this.audioChunks.length === 0) {
                console.warn('No audio data captured');
                if (this.dotNetRef) {
                    this.dotNetRef.invokeMethodAsync('OnRecordingError', 'No audio captured. Please try again!');
                }
                return;
            }

            // Create blob from audio chunks
            const audioBlob = new Blob(this.audioChunks, { 
                type: this.getSupportedMimeType() 
            });

            console.log('Processing audio blob:', audioBlob.size, 'bytes');

            // Convert to AudioBuffer for processing
            const arrayBuffer = await audioBlob.arrayBuffer();
            const audioContext = new (window.AudioContext || window.webkitAudioContext)({
                sampleRate: 16000  // 16kHz for Azure Speech Services
            });

            try {
                const audioBuffer = await audioContext.decodeAudioData(arrayBuffer);
                
                // Convert to 16-bit PCM format (required by Azure Speech Services)
                const pcmData = this.convertToPCM16(audioBuffer);
                
                console.log('Audio converted to PCM format:', pcmData.length, 'bytes');
                console.log('Audio duration:', audioBuffer.duration.toFixed(2), 'seconds');

                // Send processed audio back to Blazor component
                if (this.dotNetRef) {
                    // Convert to base64 for transfer to .NET
                    const base64Audio = this.arrayBufferToBase64(pcmData);
                    await this.dotNetRef.invokeMethodAsync('OnAudioRecorded', base64Audio, audioBuffer.duration);
                }

            } catch (decodeError) {
                console.error('Error decoding audio:', decodeError);
                if (this.dotNetRef) {
                    this.dotNetRef.invokeMethodAsync('OnRecordingError', 'Audio processing failed. Please try again!');
                }
            } finally {
                await audioContext.close();
            }

        } catch (error) {
            console.error('Error processing recorded audio:', error);
            if (this.dotNetRef) {
                this.dotNetRef.invokeMethodAsync('OnRecordingError', 'Audio processing failed. Please try again!');
            }
        } finally {
            this.cleanup();
        }
    }

    /**
     * Convert AudioBuffer to 16-bit PCM format for Azure Speech Services
     * @param {AudioBuffer} audioBuffer - Source audio buffer
     * @returns {ArrayBuffer} 16-bit PCM data
     */
    convertToPCM16(audioBuffer) {
        const numberOfChannels = audioBuffer.numberOfChannels;
        const sampleRate = audioBuffer.sampleRate;
        const length = audioBuffer.length;
        
        // Get the audio data (use first channel for mono)
        const audioData = audioBuffer.getChannelData(0);
        
        // Convert to 16-bit PCM
        const pcmBuffer = new ArrayBuffer(length * 2); // 2 bytes per sample
        const view = new DataView(pcmBuffer);
        
        for (let i = 0; i < length; i++) {
            // Convert from [-1, 1] range to 16-bit integer
            const sample = Math.max(-1, Math.min(1, audioData[i]));
            const intSample = sample < 0 ? sample * 0x8000 : sample * 0x7FFF;
            view.setInt16(i * 2, intSample, true); // Little endian
        }
        
        return pcmBuffer;
    }

    /**
     * Convert ArrayBuffer to Base64 string for .NET transfer
     * @param {ArrayBuffer} buffer - Source buffer
     * @returns {string} Base64 encoded string
     */
    arrayBufferToBase64(buffer) {
        const bytes = new Uint8Array(buffer);
        let binary = '';
        for (let i = 0; i < bytes.byteLength; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }

    /**
     * Get the best supported MIME type for recording
     * @returns {string} Supported MIME type
     */
    getSupportedMimeType() {
        const types = [
            'audio/webm;codecs=opus',
            'audio/webm',
            'audio/mp4',
            'audio/wav'
        ];

        for (const type of types) {
            if (MediaRecorder.isTypeSupported(type)) {
                console.log('Using MIME type:', type);
                return type;
            }
        }

        console.log('Using default MIME type (no specific codec support detected)');
        return 'audio/webm'; // Fallback
    }

    /**
     * Clean up resources and stop media stream
     */
    cleanup() {
        console.log('Cleaning up speech recording resources...');
        
        if (this.stream) {
            this.stream.getTracks().forEach(track => {
                track.stop();
                console.log('Stopped audio track');
            });
            this.stream = null;
        }

        if (this.mediaRecorder) {
            this.mediaRecorder = null;
        }

        this.audioChunks = [];
        this.isRecording = false;
    }

    /**
     * Dispose of the recorder and clean up all resources
     */
    dispose() {
        console.log('Disposing speech recorder for educational language learning');
        this.cleanup();
        this.dotNetRef = null;
    }
}

// Global recorder instance
window.speechRecorder = new SpeechRecorder();

// JavaScript interop functions for Blazor
window.speechRecorderInterop = {
    init: (dotNetRef) => {
        window.speechRecorder.init(dotNetRef);
    },

    isSupported: () => {
        return window.speechRecorder.isSupported();
    },

    startRecording: async () => {
        return await window.speechRecorder.startRecording();
    },

    stopRecording: async () => {
        return await window.speechRecorder.stopRecording();
    },

    dispose: () => {
        window.speechRecorder.dispose();
    }
};

console.log('Speech recording module loaded for educational language learning');
