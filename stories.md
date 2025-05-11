### Olympus MVP User Stories (with Code/File Context from Final Blueprint)

**MVP Definition Recap:**
*The core "ASAP AI Narrative Campaign Loop" allowing a single player to interact with an AI GM via the Discord bot for a simple, narrative-driven scenario. The AI should provide descriptive responses, manage basic conversational context (short-term memory), and use a simple chance mechanism (`SimpleChanceRoll` tool).*

---

**User Role: Player (Interacting via Discord)**

**1. Story: Initiate Narrative Interaction**

* **As a** Player on Discord,
* **I want to** send a natural language message (e.g., using a specific slash command like `/interact <text>`) describing my character's action or dialogue,
* **so that** the Olympus AI Game Master can process my turn and respond with a narrative outcome.

* **Code/Files Involved:**
  * **`clients/Olympus.Bot.Discord/`**
    * `Interactions/DiscordInteractionController.cs` (or Minimal API in `Program.cs`): Receives the webhook POST from Discord for slash commands.
    * `Interactions/InteractionValidationService.cs`: (Or logic within the controller/library) Validates Discord interaction signature.
    * `Commands/SlashCommandModules/NarrativeModule.cs`: Defines and handles the `/interact` slash command, extracts user input.
    * `Services/OlympusApiHttpClient.cs`: Method to construct and send a request (e.g., `ProcessPlayerNarrativeApiRequest`) to the `/api/ai/interact` endpoint of `Olympus.Api`.
    * `Services/DiscordMessageFormatter.cs`: Method to display the `NarrativeResponseDto` (containing AI GM's text) back to the Discord channel.
    * `Program.cs`: Configures Kestrel for the webhook endpoint and registers Discord.Net's `InteractionService` to route to `NarrativeModule.cs`.
  * **`src/Olympus.Api/`**
    * `Controllers/AiInteractionController.cs`: Defines the `POST /api/ai/interact` endpoint; maps the incoming API request to `ProcessPlayerNarrativeInputCommand`.
  * **`src/Olympus.Application/`**
    * `AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommand.cs`: The MediatR command record (input: `SessionId`, `PlatformPlayerId`, `PlayerInputText`).
    * `AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommandHandler.cs`: The handler that orchestrates this use case.
    * `AiDrivenFeatures/DTOs/NarrativeResponseDto.cs`: The DTO record for the AI GM's textual response.
  * **`src/Olympus.Infrastructure.Ai/`**
    * `KernelServices/SemanticKernelOrchestrator.cs`: Used by the command handler to invoke the Semantic Kernel function. (Implements `ISemanticKernelOrchestrator` from Application).
    * `Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/skprompt.txt` & `config.json`: The core prompt and SK configuration for the AI GM.

**2. Story: Experience Contextual AI Responses**

* **As a** Player on Discord,
* **I want** the AI GM's narrative responses to consider my previous actions and the AI's recent responses (short-term context),
* **so that** the interaction feels coherent and the story flows logically.

* **Code/Files Involved:**
  * **`src/Olympus.Application/Abstractions/Ai/IGameSessionNarrativeContextService.cs`**: Define the interface (methods: `GetContextAsync`, `UpdateContextAsync`, `InitializeContextAsync`).
  * **`src/Olympus.Application/Abstractions/Ai/NarrativeContext.cs` & `NarrativeExchange.cs`**: Define these record structures for holding context.
  * **`src/Olympus.Application/AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommandHandler.cs`**:
    * Modified to call `_contextService.GetContextAsync()` before invoking SK.
    * Populates `KernelArguments` with `recentHistory` from the fetched `NarrativeContext`.
    * Calls `_contextService.UpdateContextAsync()` after receiving SK's response to store the new exchange.
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/skprompt.txt`**:
    * Ensure the prompt includes placeholders and iterative logic (e.g., `{{#each recentHistory}}`) to display and utilize the `recentHistory` input variable.
  * **`src/Olympus.Infrastructure.Caching.Redis/Services/RedisNarrativeContextCache.cs`**: (Implementation for Developer Story #7, but directly supports this). Implements `IGameSessionNarrativeContextService` (or its caching backend `INarrativeContextCache`).

**3. Story: Encounter Narrative Chance**

* **As a** Player on Discord,
* **I want** the AI GM to be able to introduce a simple element of chance for my narratively uncertain actions,
* **so that** outcomes feel less predictable and can add interesting twists.

* **Code/Files Involved:**
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/skprompt.txt`**:
    * Update prompt to instruct the LLM on scenarios where invoking the `SimpleChanceRoll_EvaluateAsync` tool is appropriate.
    * Guide LLM on how to incorporate the qualitative tool output (e.g., "Success") into its narrative.
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/config.json`**:
    * Ensure the `execution_settings` allow or list the `CoreUtilityPlugin` (or specifically `SimpleChanceRoll_EvaluateAsync`) if SK requires explicit cross-plugin function calling permissions in the prompt's config.
  * **`src/Olympus.Infrastructure.Ai/Plugins/CoreUtilityPlugin/NativeFunctions/SimpleChanceRoll.cs`**: (Implementation for Developer Story #6, but directly supports this).
  * **`src/Olympus.Infrastructure.Ai/KernelServices/KernelFactory.cs`**: Ensure `CoreUtilityPlugin` (or the class containing `SimpleChanceRoll`) is registered with the SK `Kernel`.

**4. Story: Basic Session Awareness**

* **As a** Player on Discord,
* **I want** the system to recognize me and the ongoing narrative session when I interact,
* **so that** my actions are contextualized correctly.

* **Code/Files Involved:**
  * **`clients/Olympus.Bot.Discord/`**
    * `Commands/SlashCommandModules/NarrativeModule.cs` (and other modules): Logic to obtain the Discord `User.Id` and `Channel.Id` (or a bot-generated session ID) from the interaction context.
    * `Services/OlympusApiHttpClient.cs`: Modified to include `PlatformPlayerId` (e.g., `"discord:" + user.Id`) and `SessionId` in requests to `/api/ai/interact`.
  * **`src/Olympus.Application/AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommand.cs`**: Ensure it accepts `SessionId` (string or `SessionId` VO) and `PlatformPlayerId` (string).
  * **`src/Olympus.Application/Abstractions/Ai/IGameSessionNarrativeContextService.cs`**: Ensure `InitializeContextAsync` method is defined.
  * **`src/Olympus.Application/AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/ProcessPlayerNarrativeInputCommandHandler.cs`**: Logic to call `_contextService.InitializeContextAsync(command.SessionId, defaultScene, defaultPersona)` if `GetContextAsync` indicates no existing context for the session.
  * **(Optional for MVP, but good VOs) `src/Olympus.Domain/ValueObjects/StronglyTypedIDs/`**: `SessionId.cs`, `PlatformIdentifierVO.cs` (to wrap platform name + ID).

---

**User Role: Developer/System (Supporting the MVP)**

**5. Story: Configure Core AI GM Prompt**

* **As a** Developer,
* **I need to** define and implement the `GeneratePlayerTurnOutcome` semantic function for the `NarrativeGmPlugin`,
* **so that** the AI can generate appropriate narrative responses.

* **Code/Files Involved:**
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/skprompt.txt`**: Create this file with the detailed system message, context placeholders (for `sceneDescription`, `characterPersona`, `recentHistory`, `playerInput`), and instructions for the LLM on narrative style and tool usage.
  * **`src/Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/config.json`**: Create this file defining input variables, OpenAI model completion settings (temperature, max tokens, etc.), and any permitted plugins/functions for SK function calling.

**6. Story: Implement Basic AI Tool**

* **As a** Developer,
* **I need to** implement and register the `SimpleChanceRoll.EvaluateAsync` native function as a Semantic Kernel tool,
* **so that** the `NarrativeGmPlugin` can use it.

* **Code/Files Involved:**
  * **`src/Olympus.Infrastructure.Ai/Plugins/CoreUtilityPlugin/NativeFunctions/SimpleChanceRoll.cs`**: Create this C# class with the `EvaluateAsync` method, annotated with `[KernelFunction]` and `[Description]`. Implement the internal random roll logic returning a qualitative string.
  * **`src/Olympus.Infrastructure.Ai/KernelServices/KernelFactory.cs`**: Modify to add the `CoreUtilityPlugin` (or the class `SimpleChanceRoll` directly) to the `KernelBuilder.Plugins` collection so SK can discover and use this native function.

**7. Story: Enable Short-Term Narrative Context Persistence**

* **As a** Developer,
* **I need to** implement the `RedisNarrativeContextCache` (fulfilling `IGameSessionNarrativeContextService`'s caching needs),
* **so that** the AI GM has short-term conversational memory.

* **Code/Files Involved:**
  * **`src/Olympus.Application/Abstractions/Ai/IGameSessionNarrativeContextService.cs`**: Ensure the interface and its associated DTOs (`NarrativeContext.cs`, `NarrativeExchange.cs`) are finalized.
  * **`src/Olympus.Infrastructure.Caching.Redis/Services/RedisNarrativeContextCache.cs`**: Create and implement this class. It will use `StackExchange.Redis` to connect to Redis and implement methods for:
    * Storing `NarrativeContext` (e.g., `characterPersona`, `sceneDescription` in a Redis Hash).
    * Storing `recentHistory` (e.g., as a Redis List, using `LPUSH` and `LTRIM` to maintain size).
    * Retrieving the context.
    * Handling initialization of context.
  * **`src/Olympus.Infrastructure.Caching.Redis/DependencyInjection.cs`**: Add DI registration for `RedisNarrativeContextCache` as `IGameSessionNarrativeContextService` (or as the implementation for an `INarrativeContextCache` if `IGameSessionNarrativeContextService` is a higher-level service in Application that *uses* `INarrativeContextCache`). For MVP, direct implementation is fine. Configure Redis connection services.
