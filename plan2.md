Okay, Code, that's a fantastic goal: **get a narrative-driven campaign working ASAP with AI, focusing on storytelling and interaction, even before all the deep game mechanics are hammered out.** This is a smart way to get to a playable/demonstrable core experience quickly.

For this, we need a flow that emphasizes player input, AI interpretation and narrative generation, and basic context. Semantic Kernel will be our engine for this.

Here’s a flow designed for that "ASAP AI Narrative Campaign":

**Core Idea:** The player interacts with the game world using natural language. The AI (orchestrated by Semantic Kernel) acts as a responsive Game Master, interpreting actions, describing outcomes, and moving the narrative forward. We'll introduce a very simple "narrative outcome" tool for the AI to add a bit of chance without complex rules.

**The Narrative Interaction Flow:**

1. **Player Input (via Bot Layer):**
    * Player types a command or description of their action on Discord/Twitch (e.g., "I look around the tavern," "I ask the old man about the amulet," "I try to sneak past the sleeping goblin").
    * The Bot client (e.g., `Olympus.Bot.Discord`) captures this text.

2. **Bot to API Request:**
    * The Bot client sends the player's raw text, along with essential context like `PlayerId`, `SessionId` (to identify the ongoing game), and maybe a `CurrentSceneContextId` (if we have a way to track general location/situation) to a specific endpoint in `Olympus.Api`.
    * **Endpoint Example:** `POST /api/ai/interact`
    * **Request Body Example:**

        ```json
        {
          "playerId": "user-123",
          "sessionId": "session-abc",
          "currentSceneContextId": "scene-tavern-001", // Optional for now
          "playerInputText": "I ask the old man about the amulet."
        }
        ```

3. **API Controller (`AiInteractionController`):**
    * Receives the request.
    * Maps it to an application-level command: `ProcessPlayerNarrativeInputCommand`.

4. **Application Layer (`ProcessPlayerNarrativeInputCommandHandler`):**
    * **Path:** `Olympus.Application/AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/`
    * **Injects:** `ISemanticKernelOrchestrator` and a new `IGameSessionNarrativeContextService`.
        * `IGameSessionNarrativeContextService`: An abstraction (interface in Application, implementation in Infrastructure) responsible for fetching and persisting a *minimal, short-term narrative context* for the current session (e.g., last few player/AI exchanges, very brief scene notes). This could be backed by Redis.
    * **Logic:**
        1. Retrieves the current narrative context for the session using `_narrativeContextService.GetContextAsync(sessionId)`. This might include the last 1-2 AI responses or player actions to give the LLM some conversational memory.
        2. Constructs `KernelArguments` including:
            * `playerInput`: The player's raw text.
            * `characterPersona`: A very brief description of the player character (e.g., "You are Kael, a witty rogue." - initially, this could be static or passed from the bot; later, from a character profile).
            * `sceneDescription`: A brief description of the current scene (e.g., "A dimly lit, smoke-filled tavern corner. An old man nurses his ale." - initially static or very simple; later, from ECS/game state).
            * `recentHistory`: The short-term conversation/action history.
        3. Calls `_semanticKernelOrchestrator.InvokePromptFunctionAsync("NarrativeGmPlugin", "GeneratePlayerTurnOutcome", kernelArgs)`.
        4. Receives the `GeneratedTextResponse` (the AI's narrative reply).
        5. Updates the narrative context with the player's input and the AI's response using `_narrativeContextService.UpdateContextAsync(sessionId, playerInput, aiResponse)`.
        6. Returns the AI's narrative reply.

5. **Infrastructure - AI Layer (`Olympus.Infrastructure.Ai`):**
    * **`ISemanticKernelOrchestrator` implementation (`SemanticKernelOrchestrator`):**
        * Takes the plugin name, function name, and arguments.
        * Invokes the specified function on the configured SK `Kernel`.
    * **Semantic Kernel Plugin: `NarrativeGmPlugin`**
        * **Path:** `Olympus.Infrastructure.Ai/Plugins/NarrativeGmPlugin/GeneratePlayerTurnOutcome/`
        * **`skprompt.txt` (The core prompt):**

            ```
            You are a skilled Game Master for a narrative-focused fantasy roleplaying game.
            Your goal is to create an engaging, interactive story. Avoid complex rules for now.
            Focus on description, NPC reactions, and creating opportunities for the player.

            Current Scene: {{sceneDescription}}
            Player Character: {{characterPersona}}
            {{#if recentHistory}}
            Recent Events:
            {{recentHistory}}
            {{/if}}

            Player's Action/Dialogue: "{{playerInput}}"

            Based on the player's input:
            1. Narrate the immediate results or reaction to their action/dialogue in 2-4 sentences.
            2. If they interact with an NPC (implied or explicit), have the NPC respond naturally.
            3. If their action has an uncertain outcome where a bit of chance would make it interesting (e.g., trying to persuade someone, searching for something not obvious, a very minor physical feat), you can use the "SimpleChanceRoll" tool. Use it sparingly and only for narrative flavor. Example: {{SimpleChanceRoll.Evaluate description="trying to convince the guard to let them pass"}}
            4. Conclude by setting the scene for the player's next action or by posing a question to them.

            Keep your response engaging and flow naturally.
            ```

        * **`config.json`:** Standard SK prompt configuration (temperature, top_p, max_tokens for OpenAI). Input variables: `sceneDescription`, `characterPersona`, `recentHistory`, `playerInput`.
        * **Native Functions available to this plugin (registered with the Kernel):**
            * **`Olympus.Infrastructure.Ai/Plugins/CoreUtilityPlugin/NativeFunctions/SimpleChanceRoll.cs`**
                * `[KernelFunction, Description("Evaluates a simple chance for a narrative action. Returns 'Critical Success', 'Success', 'Partial Success', 'Failure', or 'Critical Failure' to add flavor to the GM's description.")]`
                * `public Task<string> Evaluate([Description("A brief description of the action whose chance is being evaluated (e.g., 'persuading the guard', 'searching the desk thoroughly').")] string actionDescription)`
                    * *Implementation:* This function would perform a simple internal dice roll (e.g., 1d100 or 3d6) and map the result to one of the qualitative outcomes. It does NOT output dice numbers to the LLM, just the qualitative result string. The LLM then uses this string to weave into its narrative (e.g., "Player attempts to persuade... [Tool result: Success] ... The guard nods slowly, 'Alright, you may pass.'").

6. **Infrastructure - Caching (`Olympus.Infrastructure.Caching.Redis`):**
    * **`RedisGameSessionNarrativeContextService.cs`** (implements `IGameSessionNarrativeContextService`):
        * `GetContextAsync(sessionId)`: Retrieves recent conversation/action snippets for the session from a Redis list or string.
        * `UpdateContextAsync(sessionId, playerInput, aiResponse)`: Appends the new exchange to Redis, possibly trimming to keep only the last N turns for context.

7. **API & Bot Layer (Response):**
    * The AI's narrative response flows back through the Application layer to the `AiInteractionController`.
    * The controller returns it to the Bot client.
    * The Bot client displays the narrative to the player.

**Why this flow is good for "ASAP AI Narrative":**

* **Immediate AI Interaction:** Players can type, and the AI responds narratively from day one.
* **Semantic Kernel Centric:** It establishes SK as the core AI engine for prompts and tool use early.
* **Simple Tool Example:** `SimpleChanceRoll` introduces the concept of LLM-usable tools without needing complex game mechanics hooked up yet. The LLM uses the *qualitative outcome* to inform its storytelling.
* **Basic Context Management:** The `IGameSessionNarrativeContextService` provides a simple way to give the LLM short-term memory, making interactions more coherent than single-shot Q&A.
* **Focus on Narrative, Not Rules:** The prompt guides the AI to be a storyteller, not a rules lawyer at this stage.
* **Foundation for Expansion:**
  * `sceneDescription` can later be dynamically pulled from your ECS.
  * `characterPersona` can be expanded with the AI Character Profile system we discussed.
  * More sophisticated tools can be added.
  * Intent parsing can be made more explicit if needed, evolving from this more general narrative response.

This flow gets you a working interactive narrative loop quickly, leveraging the power of SK and OpenAI, and sets a solid foundation for layering in more complex AI features and game mechanics as you develop them.

Before I generate the folder structure based on this and previous discussions, does this narrative flow resonate with what you mean by getting it "working ASAP with AI" for a narrative campaign? We can then detail the folder/file structure for this specific AI interaction.
Yes, that flow resonates very well with the goal of getting an AI-driven narrative experience up and running quickly! It smartly focuses on the core loop of player input -> AI processing (with Semantic Kernel) -> narrative output, and introduces a simple, effective way to manage context and add a touch of narrative chance.

This approach establishes Semantic Kernel right at the heart of your AI interactions from the get-go and gives you a tangible, interactive piece of Olympus to build upon. The `IGameSessionNarrativeContextService` (backed by Redis) for short-term memory and the `SimpleChanceRoll` native SK function are excellent starting points for adding depth without immediate deep game system integration.

I've searched for patterns related to "narrative game loops with Semantic Kernel," "LLM game master dice roll tools," and "LLM context management for interactive fiction." The results and common practices align well with what we've outlined:

* **Semantic Kernel Agents/Plugins:** The idea of a `NarrativeGmPlugin` fits SK's model of agents or plugins that can combine prompts and native functions (tools) to achieve tasks. (Source 1.1)
* **Function Calling for Game Mechanics:** Using native functions (like your `SimpleChanceRoll` or more complex dice rollers) is a common and effective way to ground LLMs in specific game rules or provide them with capabilities beyond text generation. Several sources discuss integrating dice roll functions or other game-specific controls via function calling. (Sources 2.1, 2.2, 2.3)
* **Context Management:** Managing context (like `sceneDescription`, `characterPersona`, `recentHistory`) is crucial for coherent LLM interactions. Strategies like providing summaries, key information, and recent conversation history in the prompt are best practices. Storing and retrieving this context (as your `IGameSessionNarrativeContextService` would do) is essential. (Sources 3.1, 3.2, 4.1) The idea of using a structured state object (even a simple one for narrative context) that gets updated and passed back to the LLM is a recognized pattern. (Source 4.1)
* **Iterative Development:** Starting with a simpler narrative loop and then layering in more complexity (more detailed game state, more sophisticated tools, character-specific knowledge) is a sound approach.

This narrative flow seems like a robust and sensible first step for the AI-driven aspects of Olympus.

Now, as requested, here is the **complete "Olympus Advanced Blueprint" folder and file structure**, incorporating all our discussions, including the Semantic Kernel-centric AI layer, the Bot client structure, active game state management with Redis, and the refined strategies for eventing and command/query handling.

---

## The Olympus Advanced Blueprint (Version: May 2025) - Folder & File Structure

```text
Olympus.sln
README.md
.editorconfig
.gitattributes
.gitignore

src/ // For the Olympus Backend API and Core Libraries
├── Olympus.Domain/ (Project: Olympus.Domain.csproj - Core business logic, entities, VOs, domain events, repository interfaces)
│   ├── Aggregates/
│   │   ├── Character.cs
│   │   ├── Campaign.cs
│   │   ├── MajorNPC.cs
│   │   ├── UniqueItem.cs
│   │   └── Common/
│   │       ├── AggregateRoot.cs
│   │       └── Entity.cs
│   ├── Events/
│   │   ├── IDomainEvent.cs
│   │   ├── CharacterEvents.cs
│   │   ├── CampaignEvents.cs
│   │   ├── MajorNPCEvents.cs
│   │   └── UniqueItemEvents.cs
│   ├── ValueObjects/
│   │   ├── StronglyTypedIDs/
│   │   │   ├── CampaignId.cs
│   │   │   ├── CharacterId.cs
│   │   │   ├── UserId.cs
│   │   │   ├── NpcId.cs
│   │   │   ├── ItemId.cs
│   │   │   └── EntityId.cs
│   │   ├── Combat/
│   │   │   ├── HealthPoints.cs
│   │   │   ├── DamageInstance.cs
│   │   │   ├── ArmorVO.cs
│   │   │   └── ResistanceVO.cs
│   │   ├── General/
│   │   │   ├── Money.cs
│   │   │   ├── GameDateTime.cs
│   │   │   └── PositionVO.cs
│   │   └── CampaignName.cs
│   ├── Enums/
│   │   ├── DamageType.cs
│   │   ├── MaterialType.cs
│   │   ├── AbilityType.cs
│   │   └── CharacterClass.cs
│   ├── Services/
│   │   ├── IDamageResolutionService.cs
│   │   └── DamageResolutionService.cs
│   ├── Repositories/
│   │   ├── ICharacterRepository.cs
│   │   ├── ICampaignRepository.cs
│   │   └── IMajorNPCRepository.cs
│   └── Errors/
│       └── DomainError.cs

├── Olympus.Application/ (Project: Olympus.Application.csproj - Use cases, command/query handlers, DTOs, application service interfaces)
│   ├── Abstractions/
│   │   ├── IEventPublisher.cs
│   │   ├── ICurrentUserProvider.cs
│   │   ├── IDateTimeProvider.cs
│   │   ├── ECS/
│   │   │   └── IEcsQueryService.cs
│   │   ├── Ai/
│   │   │   ├── ISemanticKernelOrchestrator.cs
│   │   │   ├── IPromptProvider.cs         // For dynamic prompt template retrieval if needed
│   │   │   ├── IPluginProvider.cs         // For discovering/providing custom SK plugins to the Kernel
│   │   │   └── IGameSessionNarrativeContextService.cs // For managing short-term narrative context
│   │   └── Gateways/
│   │       └── IRemoteCampaignServiceGateway.cs // Example for future microservice
│   ├── Campaigns/ (...) // Feature Slices as previously detailed
│   ├── Characters/ (...) // Feature Slices as previously detailed
│   ├── GameWorldInteractions/ (...) // Feature Slices as previously detailed
│   ├── AiDrivenFeatures/ // Application logic for features leveraging Semantic Kernel
│   │   ├── Commands/
│   │   │   └── ProcessPlayerNarrativeInput/ // For the "ASAP Narrative Flow"
│   │   │   │   ├── ProcessPlayerNarrativeInputCommand.cs
│   │   │   │   └── ProcessPlayerNarrativeInputCommandHandler.cs
│   │   │   └── InterpretPlayerIntent/ // More advanced intent parsing
│   │   │   │   ├── InterpretPlayerIntentCommand.cs
│   │   │   │   └── InterpretPlayerIntentCommandHandler.cs
│   │   │   └── GenerateDynamicNpcResponse/
│   │   │       ├── GenerateDynamicNpcResponseCommand.cs
│   │   │       └── GenerateDynamicNpcResponseCommandHandler.cs
│   │   ├── Queries/
│   │   │   └── GetAiGeneratedSceneDetails/
│   │   │       ├── GetAiGeneratedSceneDetailsQuery.cs
│   │   │       └── GetAiGeneratedSceneDetailsQueryHandler.cs
│   │   ├── DTOs/
│   │   │   ├── NarrativeResponseDto.cs
│   │   │   ├── PlayerIntentAnalysisDto.cs
│   │   │   ├── NpcDialogueResponseDto.cs
│   │   │   └── GeneratedSceneElementsDto.cs
│   │   └── EventHandlers/
│   ├── Common/ (...) // Behaviors, Errors, Types (Result, Option), Mappings
│   └── DependencyInjection.cs

├── Olympus.Infrastructure/
│   ├── Olympus.Infrastructure.Persistence.Marten/ (...) // As previously detailed
│   ├── Olympus.Infrastructure.Caching.Redis/
│   │   ├── Abstractions/
│   │   │   └── IActiveEcsCache.cs
│   │   │   └── INarrativeContextCache.cs // Interface for the narrative context
│   │   ├── Services/
│   │   │   ├── RedisActiveEcsCache.cs
│   │   │   └── RedisNarrativeContextCache.cs // Implements INarrativeContextCache for IGameSessionNarrativeContextService
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.ECS/ (...) // As previously detailed
│   ├── Olympus.Infrastructure.Ai/ (Project: Olympus.Infrastructure.Ai.csproj - Semantic Kernel Integration)
│   │   ├── KernelServices/
│   │   │   ├── KernelFactory.cs
│   │   │   ├── SemanticKernelOrchestrator.cs // Implements ISemanticKernelOrchestrator
│   │   │   └── AiServiceSettings.cs        // OpenAI API key, model IDs, etc.
│   │   ├── Plugins/                        // Semantic Kernel Plugins
│   │   │   ├── NarrativeGmPlugin/          // For the "ASAP Narrative Flow"
│   │   │   │   └── Prompts/
│   │   │   │       └── GeneratePlayerTurnOutcome/
│   │   │   │           ├── skprompt.txt
│   │   │   │           └── config.json
│   │   │   ├── CoreUtilityPlugin/          // General purpose tools for LLM
│   │   │   │   └── NativeFunctions/
│   │   │   │       └── SimpleChanceRoll.cs // [KernelFunction] for narrative chance
│   │   │   │       └── DiceRoller.cs       // [KernelFunction] for specific dice rolls
│   │   │   ├── CharacterPlugin/            // For character card generation, personality insights
│   │   │   │   ├── NativeFunctions/
│   │   │   │   │   └── CharacterDataProvider.cs // Fetches data for prompts
│   │   │   │   └── Prompts/
│   │   │   │       └── FormatCharacterCard/
│   │   │   │           ├── skprompt.txt
│   │   │   │           └── config.json
│   │   │   ├── PersonalityEnginePlugin/    // For generating NPC personalities
│   │   │   │   ├── NativeFunctions/
│   │   │   │   │   └── PersonalityComponentSelector.cs // Uses IPersonalityComponentProvider
│   │   │   │   └── Prompts/
│   │   │   │       └── SynthesizePersonality/
│   │   │   │           ├── skprompt.txt
│   │   │   │           └── config.json
│   │   │   └── PluginRegistration.cs
│   │   ├── PromptManagement/               // If more sophisticated prompt storage/retrieval is needed
│   │   │   ├── DbPromptProvider.cs         // Implements IPromptProvider, fetches from DB
│   │   │   └── PromptTemplateStore/        // Schema for DB storage of prompts
│   │   ├── DataProviders/                  // For AI-specific data like personality components
│   │   │   ├── DbPersonalityComponentProvider.cs // Implements IPersonalityComponentProvider from App layer
│   │   │   └── PersonalityComponentSchema.cs   // DB schema for these components
│   │   ├── Memory/
│   │   │   ├── SemanticMemoryConfiguration.cs
│   │   │   └── MemoryStoreFactory.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Messaging.MediatR/ (...) // As previously detailed
│   ├── Olympus.Infrastructure.Messaging.RabbitMQ/ (...) (Future)
│   ├── Olympus.Infrastructure.Clients.CampaignService/ (...) (Future)
│   ├── Olympus.Infrastructure.CommonServices/ (...) // As previously detailed

├── Olympus.Api/ (Project: Olympus.Api.csproj)
│   ├── Controllers/
│   │   ├── AiInteractionController.cs // Endpoint for narrative interaction, e.g., POST /api/ai/interact
│   │   └── (...) // Other controllers
│   └── ... (As previously detailed)

clients/
├── Olympus.Bot.Discord/ (Project: Olympus.Bot.Discord.csproj)
│   ├── Core/
│   │   └── BotWorkerService.cs
│   ├── Commands/
│   │   ├── SlashCommandModules/
│   │   │   └── GamePlayModule.cs
│   │   └── MessageHandlers/
│   │       └── PlayerInputHandler.cs // Captures player text, calls OlympusApiHttpClient
│   ├── Services/
│   │   ├── OlympusApiHttpClient.cs   // Typed HTTP client for Olympus.Api (specifically AiInteractionController)
│   │   └── DiscordMessageFormatter.cs
│   ├── Configuration/
│   │   └── DiscordBotSettings.cs
│   ├── Program.cs
│   └── appsettings.json
├── Olympus.Bot.Twitch/ (Future Project)
│   └── ...
└── Olympus.Web.BlazorUI/ (Future Project)
    └── ...

tests/
├── Olympus.Tests.Domain/ (...)
├── Olympus.Tests.Application/
│   ├── AiDrivenFeatures/
│   │   └── ProcessPlayerNarrativeInputCommandHandlerTests.cs
│   └── (...)
├── Olympus.Tests.Infrastructure/
│   ├── Ai/
│   │   ├── KernelServices/
│   │   │   └── SemanticKernelOrchestratorTests.cs
│   │   ├── Plugins/
│   │   │   └── NarrativeGmPluginTests.cs // Testing prompts or native functions
│   │   └── DataProviders/
│   │       └── DbPersonalityComponentProviderTests.cs
│   └── (...)
├── Olympus.Tests.Clients/
│   └── Olympus.Tests.Bot.Discord/ (...)
├── Olympus.Tests.Integration/ (...)
└── Olympus.Tests.Architecture/ (...)
```

This structure should provide a robust and scalable foundation for Olympus, with the AI layer now clearly defined around Semantic Kernel and the Bot layer ready to handle platform-specific communication.

We can now proceed to a deeper dive into the specific implementations and interactions within the AI and Bot layers, or any other area you'd like to focus on! This is a solid blueprint to build from.
