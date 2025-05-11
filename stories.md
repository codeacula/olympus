You're right, Code, that's a perfect way to make these user stories even more actionable! Since we have a detailed architectural blueprint and file structure in mind, we can definitely map these MVP user stories to the specific projects, folders, and even key files/classes that would need to be created or significantly modified.

This will give a very clear "slice" through the architecture for each MVP feature.

Let's expand on those MVP user stories with the code/file context:

---

### MVP User Stories for Olympus (Expanded with Code/File Context)

**User Role: Player (Interacting via Discord)**

**1. Story: Initiate Narrative Interaction**

* **As a** Player on Discord,
* **I want to** send a natural language message describing my character's action or dialogue,
* **so that** the Olympus AI Game Master can process my turn and respond with a narrative outcome.

* **Code/Files Involved:**
  * **`clients/Olympus.Bot.Discord/`**
    * `Commands/SlashCommandModules/NarrativeModule.cs` (or similar): New slash command (e.g., `/interact <text>`) definition and handler.
    * `Commands/MessageHandlers/PlayerInputHandler.cs` (if supporting free text): Logic to parse messages in a game channel.
    * `Services/OlympusApiHttpClient.cs`: Method to call the `/api/ai/interact` endpoint, sending `ProcessPlayerNarrativeInputCommand` data.
    * `Services/DiscordMessageFormatter.cs`: Method to display the `NarrativeResponseDto` from the API.
  * **`src/Olympus.Api/`**
    * `Controllers/AiInteractionController.cs`: New POST endpoint (e.g., `/api/ai/interact`) to receive the request from the bot.
  * **`src/Olympus.Application/`**
    * `AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommand.cs`: Define the command record (input: `SessionId`, `PlayerId`, `PlayerInputText`).
    * `AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommandHandler.cs`: Core logic for this story resides here.
    * `AiDrivenFeatures/DTOs/NarrativeResponseDto.cs`: Define the response DTO (output: `GmTextOutput`).
  * **`src/Olympus.Infrastructure.Ai/`**
    * `KernelServices/SemanticKernelOrchestrator.cs`: Used by the command handler to invoke the SK function.
    * `Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/skprompt.txt` & `config.json`: The actual prompt and its SK configuration.

**2. Story: Experience Contextual AI Responses**

* **As a** Player on Discord,
* **I want** the AI GM's narrative responses to consider my previous actions and the AI's recent responses (short-term context),
* **so that** the interaction feels coherent and conversational.

* **Code/Files Involved:**
  * **`src/Olympus.Application/Abstractions/Ai/IGameSessionNarrativeContextService.cs`**: Ensure methods like `GetContextAsync` and `UpdateContextAsync` are well-defined.
  * **`src/Olympus.Application/Abstractions/Ai/NarrativeContext.cs` & `NarrativeExchange.cs`**: Define these record structures clearly.
  * **`src/Olympus.Application/AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommandHandler.cs`**:
    * Modify to call `_contextService.GetContextAsync()` before calling SK.
    * Modify to populate `KernelArguments` with `recentHistory`.
    * Modify to call `_contextService.UpdateContextAsync()` after receiving SK's response.
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/skprompt.txt`**:
    * Ensure the prompt includes placeholders and logic to effectively use `{{recentHistory}}`.
  * **`src/Olympus.Infrastructure.Caching.Redis/Services/RedisGameSessionNarrativeContextService.cs`**: (Covered by Developer Story #7, but directly supports this player story).

**3. Story: Encounter Narrative Chance**

* **As a** Player on Discord,
* **I want** the AI GM to be ableable to introduce a simple element of chance for my narratively uncertain actions,
* **so that** outcomes feel less predictable and can add interesting twists.

* **Code/Files Involved:**
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/skprompt.txt`**:
    * Modify the prompt to instruct the LLM on *when* and *how* to consider using the `SimpleChanceRoll_Evaluate` tool.
    * Instruct the LLM on how to weave the qualitative outcome from the tool into its narrative.
  * **`src/Olympus.Infrastructure.Ai/Plugins/CoreUtilityPlugin/NativeFunctions/SimpleChanceRoll.cs`**: (Covered by Developer Story #6, but directly supports this player story).
  * **`src/Olympus.Infrastructure.Ai/KernelServices/KernelFactory.cs`**: Ensure the `CoreUtilityPlugin` (containing `SimpleChanceRoll`) is registered with the Kernel so the `NarrativeGmPlugin` can find its functions.
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/config.json`**: Ensure `CoreUtilityPlugin` (or specifically `SimpleChanceRoll`) is allowed/listed in `feature_filters.plugins` if SK requires explicit cross-plugin calls.

**4. Story: Basic Session Awareness**

* **As a** Player on Discord,
* **I want** the system to recognize me and the ongoing narrative session when I interact,
* **so that** my actions are contextualized correctly.

* **Code/Files Involved:**
  * **`clients/Olympus.Bot.Discord/`**
    * `Commands/SlashCommandModules/*Module.cs` or `MessageHandlers/PlayerInputHandler.cs`: Logic to extract Discord `User.Id` and `Channel.Id` (or maintain a bot-level session concept).
    * `Services/OlympusApiHttpClient.cs`: Ensure it passes `PlayerId` (Discord User ID for now) and a `SessionId` (could be Discord Channel ID, or a GUID generated by the bot per "session") to the API.
  * **`src/Olympus.Domain/ValueObjects/StronglyTypedIDs/`**:
    * Consider if `SessionId.cs` and a simple `PlatformUserId.cs` (e.g., `PlatformUserId(string platformName, string id)`) are needed as VOs for type safety, even in MVP. For "ASAP," raw strings might be used initially in DTOs/Commands and then upgraded.
  * **`src/Olympus.Application/AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommand.cs`**: Ensure it accepts `SessionId` and `PlayerId`.
  * **`src/Olympus.Application/Abstractions/Ai/IGameSessionNarrativeContextService.cs`**: `InitializeContextAsync` method will be key.
  * **`src/Olympus.Application/AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommandHandler.cs`**: Call `_contextService.InitializeContextAsync()` if `GetContextAsync` returns `Option.None`. Pass static/default `sceneDescription` and `characterPersona` during initialization for MVP.

---

**User Role: Developer/System (Supporting the MVP)**

**5. Story: Configure Core AI GM Prompt**

* **As a** Developer,
* **I need to** define and implement the `GeneratePlayerTurnOutcome` semantic function for the `NarrativeGmPlugin`,
* **so that** the AI can generate appropriate narrative responses.

* **Code/Files Involved:**
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/skprompt.txt`**: Create/refine this file with the core system message, context placeholders, and instructions for the LLM.
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/config.json`**: Create/refine this file to define input variables, LLM completion settings (temperature, max tokens), and any allowed tools/plugins.

**6. Story: Implement Basic AI Tool**

* **As a** Developer,
* **I need to** implement and register the `SimpleChanceRoll` native function as an SK tool,
* **so that** the `NarrativeGmPlugin` can use it.

* **Code/Files Involved:**
  * **`src/Olympus.Infrastructure.Ai/Plugins/CoreUtilityPlugin/NativeFunctions/SimpleChanceRoll.cs`**: Create this C# class and the `EvaluateAsync` method with `[KernelFunction]` and `[Description]` attributes. Implement the simple random roll logic.
  * **`src/Olympus.Infrastructure.Ai/KernelServices/KernelFactory.cs`**: Add logic to instantiate and register the `CoreUtilityPlugin` (or `SimpleChanceRoll` class directly) with the `KernelBuilder` (e.g., `kernelBuilder.Plugins.AddFromType<SimpleChanceRoll>();`).

**7. Story: Enable Short-Term Narrative Context Persistence**

* **As a** Developer,
* **I need to** implement the `RedisGameSessionNarrativeContextService`,
* **so that** the AI GM has short-term conversational memory.

* **Code/Files Involved:**
  * **`src/Olympus.Application/Abstractions/Ai/IGameSessionNarrativeContextService.cs`**: Finalize the interface definition.
  * **`src/Olympus.Application/Abstractions/Ai/NarrativeContext.cs` & `NarrativeExchange.cs`**: Finalize these record definitions.
  * **`src/Olympus.Infrastructure.Caching.Redis/Services/RedisGameSessionNarrativeContextService.cs`**: Implement the methods using `StackExchange.Redis` to store/retrieve data (e.g., using Redis Hashes for context fields and Lists for history, with trimming).
  * **`src/Olympus.Infrastructure.Caching.Redis/DependencyInjection.cs`**: Register `RedisGameSessionNarrativeContextService` for `IGameSessionNarrativeContextService` and configure Redis connection.

---

This level of detail connects each MVP user story directly to the architectural components and even specific files we've planned. It should provide a much clearer roadmap for development.

Now that we've expanded these MVP user stories with their code/file context, do you feel this gives a good enough launchpad for the initial AI narrative loop? Or are there any specific files or interactions within this MVP scope you'd like to explore further before we move on to the Bot layer in more detail?
