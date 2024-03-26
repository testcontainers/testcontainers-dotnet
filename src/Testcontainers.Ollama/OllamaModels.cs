namespace Testcontainers.Ollama
{
    /// <summary>
    /// A selection of OLLAMA models from the readme.
    /// </summary>
    /// <remarks>
    /// See: https://github.com/ollama/ollama?tab=readme-ov-file#model-library
    /// </remarks>
    public static class OllamaModels
    {
        /// <summary>
        /// Llama 2: 7B parameters, Size: 3.8GB, Command: ollama run llama2
        /// </summary>
        public const string Llama2 = "llama2";

        /// <summary>
        /// Mistral: 7B parameters, Size: 4.1GB, Command: ollama run mistral
        /// </summary>
        public const string Mistral = "mistral";

        /// <summary>
        /// Dolphin Phi: 2.7B parameters, Size: 1.6GB, Command: ollama run dolphin-phi
        /// </summary>
        public const string DolphinPhi = "dolphin-phi";

        /// <summary>
        /// Phi-2: 2.7B parameters, Size: 1.7GB, Command: ollama run phi
        /// </summary>
        public const string Phi2 = "phi";

        /// <summary>
        /// Neural Chat: 7B parameters, Size: 4.1GB, Command: ollama run neural-chat
        /// </summary>
        public const string NeuralChat = "neural-chat";

        /// <summary>
        /// Starling: 7B parameters, Size: 4.1GB, Command: ollama run starling-lm
        /// </summary>
        public const string Starling = "starling-lm";

        /// <summary>
        /// Code Llama: 7B parameters, Size: 3.8GB, Command: ollama run codellama
        /// </summary>
        public const string CodeLlama = "codellama";

        /// <summary>
        /// Llama 2 Uncensored: 7B parameters, Size: 3.8GB, Command: ollama run llama2-uncensored
        /// </summary>
        public const string Llama2Uncensored = "llama2-uncensored";

        /// <summary>
        /// Llama 2 13B: 13B parameters, Size: 7.3GB, Command: ollama run llama2:13b
        /// </summary>
        public const string Llama213B = "llama2:13b";

        /// <summary>
        /// Llama 2 70B: 70B parameters, Size: 39GB, Command: ollama run llama2:70b
        /// </summary>
        public const string Llama270B = "llama2:70b";

        /// <summary>
        /// Orca Mini: 3B parameters, Size: 1.9GB, Command: ollama run orca-mini
        /// </summary>
        public const string OrcaMini = "orca-mini";

        /// <summary>
        /// Vicuna: 7B parameters, Size: 3.8GB, Command: ollama run vicuna
        /// </summary>
        public const string Vicuna = "vicuna";

        /// <summary>
        /// LLaVA: 7B parameters, Size: 4.5GB, Command: ollama run llava
        /// </summary>
        public const string LLaVA = "llava";
    }
}