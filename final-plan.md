## The Olympus Advanced Blueprint

### 1. Introduction & Vision

**Olympus** is envisioned as an **AI-Powered Tabletop RPG Platform**. Its core mission is to provide a flexible, immersive, and deeply interactive environment for playing various tabletop roleplaying games, with a strong emphasis on leveraging Artificial Intelligence to assist Game Masters (GMs) and enhance player experience.

**Key Goals:**

* **Dynamic Narrative & Interaction:** AI will drive engaging storytelling, NPC interactions, and respond intelligently to player actions.
* **Flexible Game System Support:** While initially focusing on narrative, the architecture will support the integration of specific TTRPG rule systems (e.g., D&D 5e, Shadowrun).
* **Multi-Platform Accessibility:** Players will interact with Olympus via various clients, starting with chat platforms like Discord and potentially Twitch, with a web portal for management and broader access.
* **Robust & Advanced Backend:** Built with modern C# (.NET 9+), Clean Architecture, CQRS, Event Sourcing, and advanced programming patterns, targeting experienced developers.
* **Scalability & Maintainability:** Designed for growth, with considerations for future microservice extraction and increasing complexity.

### 2. Core Architectural Pillars & Principles

Olympus stands on several foundational architectural pillars:

* **Clean Architecture:** A strict separation of concerns into `Domain`, `Application`, `Infrastructure`, and `Presentation/Clients` layers, ensuring high cohesion, low coupling, and testability with dependencies flowing inwards.
* **CQRS (Command Query Responsibility Segregation):** Using MediatR for in-process dispatch, this separates operations that change state (Commands) from operations that retrieve data (Queries), allowing for optimized handling of each.
* **Event Sourcing (ES) with Marten on PostgreSQL:** For key domain entities (Player Characters, Major NPCs, Campaigns), their state will be derived from an immutable sequence of domain events persisted by Marten. This provides a full audit trail, temporal query capabilities, and flexibility for building diverse read models.
* **Data-Oriented Programming (DOP) Influences & Entity Component System (ECS):**
  * While aggregates encapsulate behavior, their internal state will be data-oriented.
  * A core ECS will manage dynamic game world state (NPCs, environment, interactables), particularly for simulation and performant querying, primarily operating on data cached in Redis.
* **Key Design Principles:**
  * **Immutability for Data Transfer:** Extensive use of C# records for DTOs, Commands, Queries, Events, and Value Objects.
  * **Explicit Error Handling (Railway Oriented Programming - ROP):** Services and handlers return `Result<TSuccess, TError>` to make success/failure paths explicit.
  * **Explicit Optionality:** Use of `Option<T>` for values that may legitimately be absent.
  * **Pluggable Infrastructure:** Key infrastructure concerns (event publishing, AI LLM providers) are abstracted to allow for swapping implementations.

### 3. Overall System Architecture (High-Level)

Olympus consists of:

1. **Clients (Bots, Web Portal):** User-facing applications that translate platform-specific interactions into API calls.
2. **`Olympus.Api` (ASP.NET Core):** The central HTTP API gateway, handling requests, authentication, and routing to the Application Layer.
3. **`Olympus.Application` Layer:** Orchestrates use cases (CQRS handlers), defines DTOs, and contains application-specific logic. It uses interfaces to interact with Domain and Infrastructure.
4. **`Olympus.Domain` Layer:** Contains core business logic, aggregates, value objects, domain events, and repository interfaces.
5. **`Olympus.Infrastructure.ECS`:** Manages the live game world state via components and systems, interacting heavily with Redis.
6. **`Olympus.Infrastructure.Ai` (Semantic Kernel based):** Handles all LLM interactions, prompt management, tool use, and AI-driven content generation.
7. **`Olympus.Infrastructure.Persistence.Marten` & `Olympus.Infrastructure.Caching.Redis`:** Manage data storage (durable and active/cached).
8. **`Olympus.Infrastructure.Messaging.*`:** Handles event propagation, initially in-process, designed for future distributed messaging.

**Basic Command Flow:** Client -> API -> Application Command Handler -> (Domain Aggregate OR Application Service interacting with ECS) -> Persistence/Cache -> Events Published -> Projections Updated -> Response to Client.

### 4. Detailed Project & Folder Structure

```text
Olympus.sln
README.md
.editorconfig
.gitattributes
.gitignore

src/ // For the Olympus Backend API and Core Libraries
├── Olympus.Domain/ (Project: Olympus.Domain.csproj)
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
│   │   │   └── EntityId.cs // Generic ID for ECS entities if needed as a VO
│   │   ├── Combat/
│   │   │   ├── HealthPoints.cs
│   │   │   ├── DamageInstance.cs
│   │   │   ├── ArmorVO.cs
│   │   │   └── ResistanceVO.cs
│   │   ├── General/
│   │   │   ├── Money.cs
│   │   │   ├── GameDateTime.cs
│   │   │   └── PositionVO.cs // Logical position, distinct from ECS PositionComponent
│   │   └── CampaignName.cs
│   ├── Enums/
│   │   ├── DamageType.cs
│   │   ├── MaterialType.cs
│   │   ├── AbilityType.cs
│   │   └── CharacterClass.cs
│   ├── Services/               // Stateless domain services
│   │   ├── IDamageResolutionService.cs
│   │   └── DamageResolutionService.cs
│   ├── Repositories/           // Interfaces for aggregate persistence
│   │   ├── ICharacterRepository.cs
│   │   ├── ICampaignRepository.cs
│   │   └── IMajorNPCRepository.cs
│   └── Errors/
│       └── DomainError.cs      // Base record for critical domain validation errors

├── Olympus.Application/ (Project: Olympus.Application.csproj)
│   ├── Abstractions/           // Interfaces defined by Application, implemented by Infrastructure
│   │   ├── IEventPublisher.cs
│   │   ├── ICurrentUserProvider.cs
│   │   ├── IDateTimeProvider.cs
│   │   ├── ECS/
│   │   │   └── IEcsQueryService.cs // To query ECS read models from application services
│   │   ├── Ai/
│   │   │   ├── ISemanticKernelOrchestrator.cs
│   │   │   ├── IPersonalityComponentProvider.cs // For AI personality building blocks
│   │   │   ├── IAiCharacterProfileRepository.cs // For storing generated AI profiles
│   │   │   ├── IGameSessionNarrativeContextService.cs // For short-term AI narrative context
│   │   │   ├── IPromptProvider.cs         // Optional: For dynamic prompt template retrieval
│   │   │   └── IPluginProvider.cs         // Optional: For discovering Semantic Kernel plugins
│   │   └── Gateways/
│   │       └── IRemoteCampaignServiceGateway.cs // Example for future microservice client
│   ├── Campaigns/              // Feature Slice
│   │   ├── Commands/ Queries/ DTOs/ EventHandlers/ (as previously detailed)
│   ├── Characters/             // Feature Slice for Player Characters & Major NPCs (Aggregates)
│   │   ├── Commands/ Queries/ DTOs/ EventHandlers/ (as previously detailed)
│   ├── GameWorldInteractions/  // For commands/queries targeting generic ECS-managed entities
│   │   ├── Commands/ Queries/ DTOs/ EventHandlers/ (as previously detailed)
│   ├── AiDrivenFeatures/       // Application logic for features powered by AI
│   │   ├── Commands/
│   │   │   └── ProcessPlayerNarrativeInput/
│   │   │   │   ├── ProcessPlayerNarrativeInputCommand.cs
│   │   │   │   └── ProcessPlayerNarrativeInputCommandHandler.cs
│   │   │   └── InterpretPlayerIntent/
│   │   │   └── GenerateNpcPersonality/
│   │   │       ├── GenerateNpcPersonalityCommand.cs
│   │   │       └── GenerateNpcPersonalityCommandHandler.cs
│   │   ├── Queries/
│   │   │   └── GetAiGeneratedSceneDetails/
│   │   │   └── PopulateCharacterCard/
│   │   │       ├── PopulateCharacterCardQuery.cs
│   │   │       └── PopulateCharacterCardQueryHandler.cs
│   │   ├── DTOs/
│   │   │   ├── NarrativeResponseDto.cs
│   │   │   ├── AiCharacterProfileDto.cs
│   │   │   └── FormattedCharacterCardDto.cs
│   │   └── EventHandlers/
│   ├── Common/                 // Shared application components
│   │   ├── Behaviors/ (MediatR Pipeline Behaviors: Validation, Logging, UnitOfWork, ResultMapping)
│   │   ├── Errors/ (Error.cs base record, ValidationError, NotFoundError etc.)
│   │   ├── Types/ (Result.cs, Option.cs, Success.cs)
│   │   └── Mappings/ (DtoMapperProfiles.cs)
│   └── DependencyInjection.cs    // services.AddApplicationServices()

├── Olympus.Infrastructure/     // Parent folder
│   ├── Olympus.Infrastructure.Persistence.Marten/ (Project - PostgreSQL via Marten)
│   │   ├── Repositories/ (Implementations for ICharacterRepository, etc.)
│   │   ├── Projections/ (Marten read model projections)
│   │   ├── EcsEntitySnapshots/ (IEcsSnapshotStore impl for generic ECS entities)
│   │   ├── AiData/             // Specific store for AI-generated profiles & personality components
│   │   │   ├── MartenAiCharacterProfileRepository.cs
│   │   │   └── MartenPersonalityComponentProvider.cs // Implements IPersonalityComponentProvider
│   │   ├── Outbox/ (OutboxMessage.cs for reliable eventing)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Caching.Redis/ (Project - Active game state, ECS cache)
│   │   ├── Abstractions/
│   │   │   ├── IActiveEcsCache.cs
│   │   │   └── INarrativeContextCache.cs
│   │   ├── Services/
│   │   │   ├── RedisActiveEcsCache.cs
│   │   │   └── RedisNarrativeContextCache.cs // Implements IGameSessionNarrativeContextService cache part
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.ECS/ (Project - ECS framework, components, systems)
│   │   ├── Core/ (ECS framework integration or custom core)
│   │   ├── Components/ (PositionComponent.cs, HealthComponent.cs, etc. - records/structs)
│   │   ├── Systems/ (AISystem.cs, MovementSystem.cs, EcsCombatSystem.cs, etc. - operate on Redis cache)
│   │   ├── Events/ (World State Events emitted by ECS Systems, e.g., EcsEntityDamagedEvent.cs)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Ai/ (Project - Semantic Kernel Integration)
│   │   ├── KernelServices/ (KernelFactory.cs, SemanticKernelOrchestrator.cs, AiServiceSettings.cs)
│   │   ├── Plugins/ (SK Plugins: NarrativeGmPlugin, CoreUtilityPlugin, CharacterCardPlugin, PersonalityEnginePlugin)
│   │   │   ├── NarrativeGmPlugin/GeneratePlayerTurnOutcome/ (skprompt.txt, config.json)
│   │   │   ├── CoreUtilityPlugin/NativeFunctions/ (SimpleChanceRoll.cs, DiceRoller.cs)
│   │   │   ├── PersonalityEnginePlugin/ (NativeFunctions like PersonalityComponentSelector.cs, Prompts like SynthesizeProfile/)
│   │   │   └── CharacterCardPlugin/ (NativeFunctions like CharacterDataProvider.cs, Prompts like FormatCharacterCard/)
│   │   ├── PromptManagement/ (Optional: FileBasedPromptProvider.cs, PromptTemplateFiles/)
│   │   ├── DataProviders/ (Moved DbPersonalityComponentProvider here, if it's SK-specific data access not covered by App->Persistence)
│   │   ├── Memory/ (SK Memory: SemanticMemoryConfiguration.cs, MemoryStoreFactory.cs)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Messaging.MediatR/ (Project - Initial IEventPublisher)
│   │   ├── MediatREventPublisher.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Messaging.RabbitMQ/ (Future Project)
│   ├── Olympus.Infrastructure.Clients.CampaignService/ (Future Project)
│   ├── Olympus.Infrastructure.CommonServices/ (DateTimeProvider.cs, HttpUserProvider.cs, etc.)

├── Olympus.Api/ (Project: Olympus.Api.csproj - ASP.NET Core Web API)
│   ├── Controllers/ (CampaignsController.cs, CharactersController.cs, AiInteractionController.cs, Admin...Controller.cs)
│   ├── DTOs/ (API-specific request DTOs if not using Commands directly)
│   ├── Middleware/ (GlobalErrorHandlingMiddleware.cs)
│   ├── Program.cs (Composition Root, DI, Auth, CORS)
│   ├── appsettings.json
│   └── appsettings.Development.json

clients/ // Client Applications
├── Olympus.Bot.Discord/ (Project: Olympus.Bot.Discord.csproj - Discord Bot)
│   ├── Core/ (BotWorkerService.cs)
│   ├── Interactions/ (InteractionEndpointController.cs for webhooks, InteractionHandlerService.cs)
│   ├── Commands/ (SlashCommandModules/, PrefixCommandHandlers/)
│   ├── Services/ (OlympusApiHttpClient.cs, DiscordMessageFormatter.cs)
│   ├── Configuration/ (DiscordBotSettings.cs)
│   ├── Program.cs
│   └── appsettings.json
├── Olympus.Bot.Twitch/ (Future Project - Example)
│   └── ...
└── Olympus.Web.VuePortal/ (Project: Vue.js SPA - Management & Potential Player Access)
    ├── public/
    ├── src/ (components/, views/, router/, store/, services/OlympusApiAccessService.ts, etc.)
    ├── package.json
    └── (vue.config.js or vite.config.js)

tests/
├── Olympus.Tests.Domain/ (...)
├── Olympus.Tests.Application/ (...)
├── Olympus.Tests.Infrastructure/ (...)
├── Olympus.Tests.Clients/
│   └── Olympus.Tests.Bot.Discord/ (...)
├── Olympus.Tests.Integration/ (...)
└── Olympus.Tests.Architecture/ (Optional)
```

### 5. Layer & Component Responsibilities (Summary)

* **`Olympus.Domain`**: The "Rulebook." Contains pure business logic, entities (Aggregates like `Character` that orchestrate internal component-data pipelines), Value Objects, Domain Events, and repository interfaces. No external dependencies.
* **`Olympus.Application`**: The "Game Master's Screen & Adventure Orchestrator." Implements use cases via CQRS handlers, defines DTOs, and application service interfaces (e.g., `IEventPublisher`, `ISemanticKernelOrchestrator`). Orchestrates domain logic and infrastructure services.
* **`Olympus.Infrastructure.*`**: Provides concrete implementations for abstractions defined in Application/Domain.
  * **`.Persistence.Marten`**: Handles event sourcing for aggregates, snapshotting for generic ECS entities, and stores AI-generated profiles and personality components in PostgreSQL.
  * **`.Caching.Redis`**: Manages active ECS component data and narrative context for high-speed access.
  * **`.ECS`**: Defines ECS components and systems. Systems operate on data in Redis, can generate "World State Events" or trigger commands back to the Application layer.
  * **`.Ai`**: Integrates Semantic Kernel with OpenAI. Manages SK Kernel, Plugins (native C# tools like DiceRoller, data fetchers; semantic prompts for generation), prompt strategies, and SK Memory.
  * **`.Messaging.*`**: Implements `IEventPublisher` for in-process (MediatR) and future distributed eventing (Outbox to RabbitMQ/Kafka).
  * **`.Clients.*`**: (Future) Gateways to other microservices.
* **`Olympus.Api`**: The HTTP entry point. Handles web requests, auth, serialization, and dispatches to Application layer (MediatR). Composition root for DI.
* **`clients/*`**: User-facing applications.
  * **`Olympus.Bot.Discord`**: Handles Discord interactions, translates them to API calls, formats responses. Hosts an HTTP endpoint for Discord interaction webhooks.
  * **`Olympus.Web.VuePortal`**: Vue.js SPA for management and potential player access, consuming the API.

### 6. Key Workflows Illustrated

* **A. Player Narrative Interaction (Bot -> API -> AI -> Bot):**
    1. Discord Bot receives player text (e.g., "/say I greet the innkeeper").
    2. Bot calls `POST /api/ai/interact` with text and session context.
    3. `AiInteractionController` dispatches `ProcessPlayerNarrativeInputCommand`.
    4. Handler fetches narrative context from `IGameSessionNarrativeContextService` (Redis).
    5. Handler calls `ISemanticKernelOrchestrator` to invoke `NarrativeGmPlugin.GeneratePlayerTurnOutcome` (SK function).
        * SK Plugin uses player input, context, and potentially tools like `SimpleChanceRoll`.
        * OpenAI generates narrative response.
    6. Handler updates narrative context in Redis, returns response DTO.
    7. API sends narrative back to Bot, Bot displays to user.
* **B. Structured Game Action by PC Aggregate (Bot -> API -> App -> Domain -> Persistence -> Events -> Projections):**
    1. Discord Bot receives command (e.g., `/use-ability fireball target Goblin1`).
    2. Bot calls `POST /api/character/{charId}/abilities/{abilityId}/use` with target info.
    3. Controller dispatches `UseCharacterAbilityCommand`.
    4. Handler loads `Character` Aggregate (from Marten/PostgreSQL).
    5. `character.UseAbility(fireball, goblinEntityId)` method is called.
        * Aggregate validates (mana, cooldowns), orchestrates internal component-data pipeline (e.g., applies effects to self), uses `IDamageResolutionService` for target effects.
        * Emits `CharacterAbilityUsedEvent`, `TargetDamagedEvent`, `CharacterManaReducedEvent`.
    6. Repository saves aggregate (appends events to Marten).
    7. `IEventPublisher` dispatches events.
    8. Projectors update Redis ECS cache (e.g., Goblin1's health component) and other PostgreSQL read models.
    9. Handler returns `Result<AbilityUsageOutcomeDto, Error>`.
    10. API sends outcome to Bot.
* **C. ECS Simulation Driving Change (ECS -> Command -> App -> Domain -> ...):**
    1. `AISystem` in `Olympus.Infrastructure.ECS` (operating on Redis data) determines an NPC should attack a PC.
    2. `AISystem` generates an `NpcAttackCharacterCommand(npcId, pcId, attackType)`.
    3. This command is dispatched (internally, or via a message queue if ECS is very decoupled) to `Olympus.Application`.
    4. `NpcAttackCharacterCommandHandler` loads the `MajorNPC` (attacker) and `Character` (defender) aggregates.
    5. Orchestrates the attack: `attacker.PerformAttack(...)`, then `defender.ReceiveDamage(...)` (as per flow B).
    6. Events are emitted, ECS projections are updated.
* **D. Admin Action via Web Portal (Vue -> API -> App -> Persistence):**
    1. Admin in Vue Portal edits an AI personality component definition.
    2. Vue app calls `PUT /api/admin/ai/personality-components/{id}`.
    3. `AdminAiConfigurationController` dispatches `UpdatePersonalityComponentCommand`.
    4. Handler validates, uses `IPersonalityComponentProvider`'s underlying repository (via Marten) to update the definition in PostgreSQL.
    5. Returns `Result<Success, Error>`.

### 7. Plan for Accomplishing an MVP

**MVP Definition:**

* The core "ASAP AI Narrative Campaign Loop" allowing a single player to interact with an AI GM via the Discord bot for a simple, narrative-driven scenario. The AI should provide descriptive responses, manage basic conversational context, and use a simple chance mechanism.

**Core Components to Build for MVP:**

1. **`Olympus.Api`:**
    * Minimal endpoint: `POST /api/ai/interact` (for narrative input).
    * Basic bot authentication (e.g., shared secret/API key).
    * DI setup for MVP services.
2. **`Olympus.Application`:**
    * `ISemanticKernelOrchestrator`, `IGameSessionNarrativeContextService` interfaces.
    * `ProcessPlayerNarrativeInputCommand` and its `Handler`.
    * `NarrativeResponseDto`, `NarrativeContext`, `NarrativeExchange` records.
    * `Result<T,E>`, `Option<T>`, `Error` base types.
    * `IEventPublisher` interface (even if just MediatR impl for now).
3. **`Olympus.Infrastructure.Ai`:**
    * `KernelFactory` (basic, configured for OpenAI).
    * `SemanticKernelOrchestrator` implementation.
    * `AiServiceSettings` (for OpenAI key/model).
    * `NarrativeGmPlugin` with `GeneratePlayerTurnOutcome` prompt function (`skprompt.txt`, `config.json`).
    * `CoreUtilityPlugin` with `SimpleChanceRoll` native function.
    * DI registration for these.
4. **`Olympus.Infrastructure.Caching.Redis`:**
    * `RedisGameSessionNarrativeContextService` implementation (storing last N exchanges).
    * DI registration.
5. **`Olympus.Infrastructure.Messaging.MediatR`:**
    * `MediatREventPublisher` implementation.
    * DI registration.
6. **`clients/Olympus.Bot.Discord`:**
    * Basic bot connection to Discord (`BotWorkerService`).
    * Handler for a single slash command (e.g., `/say <text>` or `/interact <text>`) or just parsing raw message content.
    * `OlympusApiHttpClient` to call the `/api/ai/interact` endpoint.
    * Basic `DiscordMessageFormatter` to display the AI's text response.
    * Configuration for bot token and API URL.
7. **Minimal `Olympus.Domain`:** Only if strictly needed by any VOs used in DTOs (e.g., `SessionId`, `UserId` if passed around, but could be strings for MVP). No complex aggregates for MVP.

**What's OUT of MVP Scope (but planned later):**

* Full ES Aggregate persistence for PCs, Campaigns, Major NPCs (Marten setup will be there, but not heavily used).
* Complex ECS simulation and detailed component management in Redis beyond simple narrative context.
* Database storage for AI personalities, character cards, or personality component definitions.
* Advanced LLM tools beyond `SimpleChanceRoll` (like full DiceRoller).
* Semantic Kernel Memory for RAG or long-term NPC memory.
* Vue.js admin portal.
* Multi-platform bot support (focus on Discord first).
* Detailed game system rule integration (D&D 5e, Shadowrun).
* Sophisticated error handling, validation, logging across all layers (basic will be there).
* Outbox pattern for eventing.

**Phased Approach Post-MVP:**

1. **Phase 1.5 (Enhance AI Narrative):**
    * Implement basic AI Character Profile storage (`IAiCharacterProfileRepository`, Marten impl).
    * Rudimentary Personality Generation (`PersonalityEnginePlugin` using hardcoded or simple file-based components).
    * Allow `NarrativeGmPlugin` to fetch/use basic NPC persona if an NPC is interacted with.
    * Integrate a full `DiceRoller` tool.
2. **Phase 2 (Core Game Entities & State):**
    * Implement `PlayerCharacter` Aggregate with basic ES (name, HP, core stats).
    * Basic command handlers for character creation and modification.
    * Start building out ECS components in Redis for these PCs (position, status).
    * Project PC domain events to update these ECS components.
3. **Phase 3 (Deeper AI & ECS):**
    * Full DB-driven Personality Component system.
    * Character Card generation.
    * More ECS systems (basic movement, interaction).
    * Semantic Kernel Memory for better context.
4. **Phase 4 (Game Systems & Admin):**
    * Begin integrating specific TTRPG rule mechanics.
    * Develop the Vue.js Admin Portal for managing game data, AI components.
5. **Phase 5+ (Expansion):**
    * Other bot clients (Twitch).
    * Advanced SK Planners.
    * Community features, etc.

---

This provides a comprehensive blueprint from the high-level architecture down to a pragmatic MVP and phased rollout. It's a complex system, but this structured approach, leveraging advanced patterns and C# features, should provide a powerful and flexible foundation for Olympus.

We can now consider this overall planning phase largely complete unless you have further refinements. The next step would typically be to start building out the MVP components. What are your thoughts?
