# The Olympus Advanced Blueprint (Final Draft - May 13, 2025)

## 1. Introduction & Vision

**Olympus** is envisioned as an **AI-Powered Tabletop RPG Platform**. Its core mission is to provide a flexible, immersive, and deeply interactive environment for playing various tabletop roleplaying games (e.g., D&D 5e, Shadowrun). It emphasizes leveraging Artificial Intelligence (powered by Semantic Kernel with OpenAI as the initial LLM) to assist Game Masters (GMs), enhance player experience through dynamic narrative, NPC interactions, and provide intelligent game management capabilities.

**Key Goals:**

* **Dynamic Narrative & Interaction:** AI will drive engaging storytelling, NPC personalities and dialogues, and respond intelligently to player actions.
* **Flexible Game System Support:** The architecture will support the integration of specific TTRPG rule systems.
* **Multi-Platform Accessibility:** Players will interact with Olympus via various clients, starting with chat platforms like Discord (and potentially Twitch), with a Vue.js web portal planned for management and broader access.
* **Robust & Advanced Backend:** Built with modern C# (.NET 9+), Clean Architecture, CQRS, Event Sourcing, and advanced programming patterns, targeting experienced developers.
* **Scalability & Maintainability:** Designed for growth, with considerations for future microservice extraction and increasing complexity.

## 2. Core Architectural Pillars & Principles

Olympus is built upon these foundational architectural pillars and design principles:

* **Clean Architecture**: A strict separation of concerns into `Domain`, `Application`, `Infrastructure`, and `Presentation/Clients` layers, ensuring high cohesion, low coupling, testability, and a dependency flow strictly inwards.
* **CQRS (Command Query Responsibility Segregation)**: Using MediatR for in-process dispatch, this separates operations that change state (Commands) from operations that retrieve data (Queries), allowing for optimized data handling and scalability.
* **Event Sourcing (ES) with Marten on PostgreSQL**: For key domain entities (Player Characters, Major NPCs, Campaigns), their state will be derived from an immutable sequence of domain events persisted by Marten. This provides a full audit trail, temporal query capabilities, and flexibility for building diverse read models.
* **Data-Oriented Programming (DOP) Influences & Entity Component System (ECS)**:
  * Aggregates, while encapsulating behavior, will have their internal state structured in a data-oriented manner (component-like Value Objects).
  * A core ECS will manage dynamic game world state (generic NPCs, environment, interactables), primarily operating on data cached in Redis, for simulation and performant querying.
* **Key Design Principles:**
  * **Immutability for Data Transfer**: Extensive use of C# records (`record class`, `readonly record struct`) for DTOs, Commands, Queries, Events, and Value Objects.
  * **Explicit Error Handling (Railway Oriented Programming - ROP)**: Application service methods and command handlers will return `Result<TSuccess, TError>` types.
  * **Explicit Optionality**: Use of `Option<T>` for values that may legitimately be absent.
  * **Pluggable Infrastructure**: Key infrastructure concerns (event publishing, AI LLM providers) are abstracted to allow for swapping implementations.
  * **Advanced C# Features**: Leveraging pattern matching, primary constructors, `required` members, static abstract members in interfaces, file-local types, source-generated logging, and modern async practices (`IAsyncEnumerable<T>`, `ValueTask<T>`).

## 3. Overall System Architecture (High-Level)

Olympus consists of several interconnected layers and components:

1. **Clients (Bots, Web Portal)**: User-facing applications that translate platform-specific interactions into API calls to the Olympus backend.
2. **`Olympus.Api` (ASP.NET Core)**: The central HTTP API gateway. It handles incoming requests, authentication/authorization, request validation, and routes calls to the Application Layer.
3. **`Olympus.Application` Layer**: Orchestrates application-specific use cases (CQRS handlers). It defines DTOs and contains application logic, using interfaces to interact with the Domain Layer and Infrastructure Layer.
4. **`Olympus.Domain` Layer**: The heart of the business logic. Contains domain entities (Aggregates), Value Objects, Domain Events, and repository interfaces, organized by domain concept. It is independent of other layers.
5. **`Olympus.Infrastructure.ECS`**: The Entity Component System. Manages the live, dynamic state of the game world (generic NPCs, environmental objects, etc.) primarily within Redis, running simulation logic via its Systems.
6. **`Olympus.Infrastructure.Ai` (Semantic Kernel based)**: Manages all LLM interactions, including prompt orchestration, tool use (native functions), memory, and specific AI model integrations (starting with OpenAI).
7. **`Olympus.Infrastructure.Persistence.Marten` & `Olympus.Infrastructure.Caching.Redis`**: Handle durable data storage (PostgreSQL via Marten for ES aggregates, snapshots) and active/cached game state (Redis for ECS, narrative context).
8. **`Olympus.Infrastructure.Messaging.*`**: Manages event propagation, initially in-process (MediatR) and designed for future distributed messaging.

**Basic Command Flow:** Client -> `Olympus.Api` -> `Olympus.Application` Command Handler -> (Interacts with `Olympus.Domain` Aggregate OR `Olympus.Infrastructure.ECS` via Application Service) -> `Olympus.Infrastructure.Persistence/Caching` -> Domain Events/World State Events Published -> Projections Updated (including ECS state in Redis and other read models) -> Response back through layers to Client.

## 4. Detailed Project & Folder Structure

```text
Olympus.sln
README.md
.editorconfig
.gitattributes
.gitignore

src/ // For the Olympus Backend API and Core Libraries
├── Olympus.Domain/ (Project: Olympus.Domain.csproj - Core business logic, organized by domain concept)
│   ├── Campaign/  // << Domain Concept: Campaign >>
│   │   ├── Campaign.cs (AggregateRoot)
│   │   ├── ICampaignRepository.cs
│   │   ├── ValueObjects/
│   │   │   ├── CampaignName.cs
│   │   │   └── CampaignSettings.cs
│   │   └── Events/
│   │       ├── CampaignCreatedEvent.cs
│   │       └── CampaignPhaseChangedEvent.cs
│   ├── Character/ // << Domain Concept: Character (Player or otherwise significant) >>
│   │   ├── Character.cs (AggregateRoot)
│   │   ├── ICharacterRepository.cs
│   │   ├── ValueObjects/
│   │   │   ├── CharacterStats.cs
│   │   │   ├── HealthPoints.cs // Could move to SharedKernel.Combat if truly generic
│   │   │   └── EquippedItems.cs // Internal to Character aggregate state
│   │   ├── Events/
│   │   │   ├── CharacterCreatedEvent.cs
│   │   │   ├── CharacterHealthReducedEvent.cs
│   │   │   └── CharacterLeveledUpEvent.cs
│   │   └── Enums/
│   │       └── CharacterAlignment.cs
│   ├── MajorNPC/ // << Domain Concept: Major NPC (if treated as an Aggregate) >>
│   │   ├── MajorNPC.cs (AggregateRoot)
│   │   ├── IMajorNPCRepository.cs
│   │   └── Events/
│   │       └── MajorNPCDialogueTriggeredEvent.cs
│   ├── UniqueItem/ // << Domain Concept: Unique Item (if treated as an Aggregate) >>
│   │   ├── UniqueItem.cs (AggregateRoot)
│   │   ├── IUniqueItemRepository.cs // Example
│   │   └── Events/
│   │       └── UniqueItemAbilityActivatedEvent.cs
│   ├── SharedKernel/ // << Truly cross-cutting domain elements >>
│   │   ├── ValueObjects/
│   │   │   ├── StronglyTypedIDs/ // Generic ID types or base
│   │   │   │   ├── UserId.cs
│   │   │   │   ├── NpcId.cs      // Could be used by MajorNPC or generic NPCs
│   │   │   │   ├── ItemId.cs
│   │   │   │   ├── EntityId.cs   // Generic for ECS if needed here
│   │   │   │   └── AggregateIdBase.cs
│   │   │   ├── Money.cs
│   │   │   ├── GameDateTime.cs
│   │   │   └── PositionVO.cs     // Logical position, distinct from ECS PositionComponent
│   │   ├── Events/
│   │   │   └── IDomainEvent.cs   // Marker interface
│   │   ├── Enums/
│   │   │   ├── DamageType.cs
│   │   │   └── MaterialType.cs
│   │   ├── Errors/
│   │   │   └── DomainErrorBase.cs
│   │   └── Abstractions/
│   │       └── IParsable.cs      // Example shared abstraction for VOs
│   ├── Services/               // Stateless domain services operating across aggregates/shared concepts
│   │   ├── IDamageResolutionService.cs
│   │   └── DamageResolutionService.cs
│   └── Common/                 // Base classes for domain elements if not in SharedKernel
│       ├── AggregateRoot.cs
│       └── Entity.cs

├── Olympus.Application/ (Project: Olympus.Application.csproj - Use cases, CQRS, DTOs, App Abstractions)
│   ├── Abstractions/
│   │   ├── IEventPublisher.cs
│   │   ├── ICurrentUserProvider.cs
│   │   ├── IDateTimeProvider.cs
│   │   ├── ECS/
│   │   │   └── IEcsQueryService.cs
│   │   ├── Ai/
│   │   │   ├── ISemanticKernelOrchestrator.cs
│   │   │   ├── IPersonalityComponentProvider.cs
│   │   │   ├── IAiCharacterProfileRepository.cs
│   │   │   ├── IGameSessionNarrativeContextService.cs
│   │   │   ├── IPromptProvider.cs
│   │   │   └── IPluginProvider.cs
│   │   └── Gateways/
│   │       └── IRemoteCampaignServiceGateway.cs
│   ├── Campaigns/              // Feature Slice - organized by vertical slice/use case
│   │   ├── CreateCampaign/
│   │   │   ├── CreateCampaignCommand.cs
│   │   │   ├── CreateCampaignCommandHandler.cs
│   │   │   └── CreateCampaignCommandValidator.cs
│   │   ├── AdvanceCampaignPhase/
│   │   │   ├── AdvanceCampaignPhaseCommand.cs
│   │   │   └── ...
│   │   ├── GetCampaignDetails/
│   │   │   ├── GetCampaignDetailsQuery.cs
│   │   │   └── ...
│   │   ├── Common/             // DTOs and Event Handlers specific to Campaigns feature
│   │   │   ├── DTOs/
│   │   │   │   └── CampaignDto.cs
│   │   │   └── EventHandlers/
│   │   │       └── CampaignCreatedEventHandler.cs
│   ├── Characters/             // Feature Slice - organized by vertical slice/use case
│   │   ├── CreatePlayerCharacter/
│   │   │   └── ...
│   │   ├── ApplyDamageToCharacter/
│   │   │   └── ...
│   │   ├── GetCharacterSheet/
│   │   │   └── ...
│   │   ├── Common/
│   │   │   ├── DTOs/
│   │   │   │   └── CharacterSheetDto.cs
│   │   │   └── EventHandlers/
│   ├── GameWorldInteractions/  // Feature Slice for generic ECS entities
│   │   ├── DamageWorldEntity/
│   │   │   └── ...
│   │   ├── GetWorldEntityDetails/
│   │   │   └── ...
│   │   ├── Common/
│   │   │   └── DTOs/
│   │   │       └── WorldEntityDto.cs
│   ├── AiDrivenFeatures/       // Feature Slice for AI-powered use cases
│   │   ├── ProcessPlayerNarrativeInput/
│   │   │   └── ...
│   │   ├── GenerateNpcPersonality/
│   │   │   └── ...
│   │   ├── PopulateCharacterCard/
│   │   │   └── ...
│   │   ├── Common/
│   │   │   └── DTOs/ (NarrativeResponseDto.cs, AiCharacterProfileDto.cs, etc.)
│   ├── Common/                 // Truly shared Application components
│   │   ├── Behaviors/ (ValidationBehavior.cs, LoggingBehavior.cs, UnitOfWorkBehavior.cs, ResultMappingBehavior.cs)
│   │   ├── Errors/ (Error.cs base record, ValidationError.cs, NotFoundError.cs, etc.)
│   │   ├── Types/ (Result.cs, Option.cs, Success.cs)
│   │   └── Mappings/ (DtoMapperProfiles.cs for libraries like AutoMapper/Mapster, or manual mapping helpers)
│   └── DependencyInjection.cs    // services.AddApplicationServices()

├── Olympus.Infrastructure/     // Parent folder for infrastructure implementations
│   ├── Olympus.Infrastructure.Persistence.Marten/ (Project)
│   │   ├── Repositories/       // Impls for ICharacterRepository, ICampaignRepository, etc.
│   │   ├── Projections/        // Marten read model projections (e.g., CampaignSummaryProjection.cs)
│   │   ├── EcsEntitySnapshots/ // Impl for IEcsSnapshotStore (e.g., MartenEcsSnapshotStore.cs)
│   │   ├── AiData/             // Impls for IAiCharacterProfileRepository, IPersonalityComponentProvider
│   │   │   ├── MartenAiCharacterProfileRepository.cs
│   │   │   └── MartenPersonalityComponentProvider.cs
│   │   │   └── Schemas/        // Folder for DB schema scripts or entity definitions for AI data if not all doc based
│   │   │       └── PersonalityComponentDefinition.cs // (If using Marten docs, this might be a simple record)
│   │   ├── Outbox/             // OutboxMessage.cs (if custom outbox table is used with Marten transactions)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Caching.Redis/ (Project)
│   │   ├── Abstractions/ (IActiveEcsCache.cs, INarrativeContextCache.cs - may move to Application if generic)
│   │   ├── Services/ (RedisActiveEcsCache.cs, RedisNarrativeContextCache.cs)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.ECS/ (Project)
│   │   ├── Core/ (ECS Framework integration or custom core: EntityId.cs, SystemBase.cs, ComponentBase.cs)
│   │   ├── Components/ (PositionComponent.cs, HealthComponent.cs, AIStateComponent.cs, etc. - records/structs)
│   │   ├── Systems/ (AISystem.cs, MovementSystem.cs, EcsCombatSystem.cs, etc. - operate on Redis cache via IActiveEcsCache)
│   │   ├── Events/ (World State Events emitted by ECS Systems, e.g., EcsEntityDamagedEvent.cs)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Ai/ (Project - Semantic Kernel Integration)
│   │   ├── KernelServices/ (KernelFactory.cs, SemanticKernelOrchestrator.cs, AiServiceSettings.cs)
│   │   ├── Plugins/ (SK Plugins: NarrativeGmPlugin, CoreUtilityPlugin, CharacterCardPlugin, PersonalityEnginePlugin, etc.)
│   │   │   ├── NarrativeGmPlugin/GeneratePlayerTurnOutcome/ (skprompt.txt, config.json)
│   │   │   ├── CoreUtilityPlugin/NativeFunctions/ (SimpleChanceRoll.cs, DiceRoller.cs)
│   │   │   └── ... (other plugins as detailed previously)
│   │   ├── PromptManagement/ (Optional: FileBasedPromptProvider.cs, PromptTemplateFiles/)
│   │   ├── Memory/ (SK Memory: SemanticMemoryConfiguration.cs, MemoryStoreFactory.cs)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Messaging.MediatR/ (Project - Initial IEventPublisher)
│   │   ├── MediatREventPublisher.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Messaging.RabbitMQ/ (Future Project)
│   ├── Olympus.Infrastructure.Clients.CampaignService/ (Future Project)
│   ├── Olympus.Infrastructure.CommonServices/ (Implementations for IDateTimeProvider, ICurrentUserProvider, etc.)

├── Olympus.Api/ (Project: Olympus.Api.csproj - ASP.NET Core Web API)
│   ├── Controllers/ (CampaignsController.cs, CharactersController.cs, AiInteractionController.cs, Admin...Controller.cs, etc.)
│   ├── DTOs/ (API-specific request DTOs if not using Commands directly from Application)
│   ├── Middleware/ (GlobalErrorHandlingMiddleware.cs, AuthMiddleware.cs)
│   ├── Program.cs (Composition Root, DI, Auth, CORS, Kestrel setup)
│   ├── appsettings.json
│   └── appsettings.Development.json

clients/ // Client Applications
├── Olympus.Bot.Discord/ (Project: Olympus.Bot.Discord.csproj - Discord Bot Worker Service)
│   ├── Core/ (BotWorkerService.cs - IHostedService for Discord client lifecycle)
│   ├── Interactions/ (DiscordInteractionController.cs or Minimal API endpoint in Program.cs for webhooks, InteractionValidationService.cs)
│   ├── Commands/ (SlashCommandModules/, PrefixCommandHandlers/ - if any)
│   ├── Services/ (OlympusApiHttpClient.cs, DiscordMessageFormatter.cs, UserProfileMapperService.cs - optional)
│   ├── Configuration/ (DiscordBotSettings.cs - POCO for settings)
│   ├── Program.cs (Bot host setup, DI, Discord client config, Kestrel for webhooks)
│   └── appsettings.json
├── Olympus.Bot.Twitch/ (Future Project - Example)
│   └── ... // Similar structure for Twitch-specific logic
└── Olympus.Web.VuePortal/ (Project: Vue.js SPA, potentially served by an ASP.NET Core host initially)
    ├── public/ (index.html, favicon, etc.)
    ├── src/ (Vue components, views, router, store, services/OlympusApiAccessService.ts)
    ├── package.json
    └── (vue.config.js or vite.config.js)
    └── (Olympus.Web.VuePortal.csproj if using `dotnet new vue` template for hosting)

tests/
├── Olympus.Tests.Domain/ (Project: Olympus.Tests.Domain.csproj)
│   ├── Campaign/ (Tests for Campaign aggregate, VOs, etc.)
│   │   └── CampaignTests.cs
│   └── Character/
│       └── CharacterTests.cs
├── Olympus.Tests.Application/ (Project: Olympus.Tests.Application.csproj)
│   ├── Campaigns/ (Tests for Campaign commands, queries, handlers)
│   │   └── CreateCampaign/
│   │       └── CreateCampaignCommandHandlerTests.cs
│   └── AiDrivenFeatures/ (...)
├── Olympus.Tests.Infrastructure/ (Project: Olympus.Tests.Infrastructure.csproj)
│   ├── Persistence/
│   │   └── MartenCharacterRepositoryTests.cs // Requires Testcontainers
│   ├── ECS/
│   │   └── AISystemTests.cs // Mock IActiveEcsCache or use in-memory Redis for tests
│   └── Ai/
│       └── SemanticKernelOrchestratorTests.cs
├── Olympus.Tests.Clients/ // Parent folder for client test projects
│   └── Olympus.Tests.Bot.Discord/ (Project: Olympus.Tests.Bot.Discord.csproj)
│       └── Commands/
│           └── GamePlayModuleTests.cs // Mocks OlympusApiHttpClient
├── Olympus.Tests.Integration/ (Project: Olympus.Tests.Integration.csproj)
│   ├── Api/
│   │   └── CampaignsControllerIntegrationTests.cs // Uses WebApplicationFactory, Testcontainers
│   └── Scenarios/
│       └── FullPlayerNarrativeInteractionScenarioTests.cs
└── Olympus.Tests.Architecture/ (Optional Project: Olympus.Tests.Architecture.csproj - Using NetArchTest)
    └── DependencyRuleTests.cs
    └── NamingConventionTests.cs
```

## 5. Layer & Component Responsibilities

* **`Olympus.Domain`**: The "Rulebook." Contains pure, application-agnostic business logic. Organized by domain concept (e.g., `Campaign/`, `Character/`), each housing its Aggregate Root, specific Value Objects, Domain Events, and its Repository Interface. A `SharedKernel/` contains truly cross-cutting domain elements. `Services/` holds stateless domain services.
* **`Olympus.Application`**: The "Orchestrator." Implements use cases via CQRS handlers, organized into feature slices (e.g., `Campaigns/`, `Characters/`), with use cases further broken down into vertical slices (e.g., `Campaigns/CreateCampaign/`). Defines DTOs and application-level interfaces for infrastructure.
* **`Olympus.Infrastructure.*`**: Provides concrete implementations for abstractions.
  * **`.Persistence.Marten`**: Handles event sourcing, aggregate persistence, Marten projections, snapshotting of generic ECS entities, and persistence for AI-generated data (profiles, personality components).
  * **`.Caching.Redis`**: Manages active ECS component data and short-term narrative context.
  * **`.ECS`**: Defines ECS components and systems. Systems operate on data in Redis (via `IActiveEcsCache`), can generate "World State Events," or trigger Commands.
  * **`.Ai`**: Integrates Semantic Kernel (with OpenAI initially). Manages SK `Kernel`, `Plugins` (native C# tools & semantic prompts for personality, narrative, character cards), prompt strategies, and SK `Memory`.
  * **`.Messaging.*`**: Implements `IEventPublisher` (MediatR initially, with Outbox pattern for future distributed bus).
  * **`.Clients.*`**: (Future) Gateways to other microservices.
  * **`.CommonServices`**: Implements general utility interfaces like `IDateTimeProvider`.
* **`Olympus.Api`**: The HTTP entry point. Handles web requests, authN/authZ (e.g., JWTs), CORS, serialization, and dispatches to `Olympus.Application` (MediatR). Composition root for Dependency Injection.
* **`clients/*`**: User-facing applications.
  * **`Olympus.Bot.Discord`**: Handles Discord interactions (slash commands via webhooks), translates to API calls, formats responses.
  * **`Olympus.Web.VuePortal`**: Vue.js SPA for management and potential player access, consuming the API.

## 6. Key Workflows Illustrated

* **A. Player Narrative Interaction via Bot (ASAP MVP Focus):**
    1. Discord Bot (`Olympus.Bot.Discord`) receives player text (e.g., `/interact "I search the room"`). The bot's HTTP endpoint (e.g., in `Interactions/DiscordInteractionController.cs`) handles the validated webhook from Discord.
    2. Bot calls `OlympusApiHttpClient.cs` to send request to `POST /api/ai/interact` in `Olympus.Api`.
    3. `AiInteractionController` in `Olympus.Api` dispatches `ProcessPlayerNarrativeInputCommand` to `Olympus.Application`.
    4. `ProcessPlayerNarrativeInputCommandHandler` uses `IGameSessionNarrativeContextService` (backed by `Olympus.Infrastructure.Caching.Redis`) to fetch/update conversational context.
    5. Handler calls `ISemanticKernelOrchestrator` (implemented by `SemanticKernelOrchestrator` in `Olympus.Infrastructure.Ai`).
    6. `SemanticKernelOrchestrator` uses SK `Kernel` to invoke the `NarrativeGmPlugin.GeneratePlayerTurnOutcome` semantic function. This prompt may use native tools like `CoreUtilityPlugin.SimpleChanceRoll`. OpenAI processes.
    7. Narrative response flows back to Bot, which uses `DiscordMessageFormatter.cs` to display to user.
* **B. Structured Game Action via PC Aggregate (Post-MVP):**
    1. Bot (e.g., `CharacterModule`) translates user action to a structured API call (e.g., `POST /api/v1/characters/{charId}/abilities/use`).
    2. API Controller dispatches corresponding `UseCharacterAbilityCommand`.
    3. Application Handler loads `Character` Aggregate (from `MartenCharacterRepository` in `Olympus.Infrastructure.Persistence.Marten`).
    4. `Character.UseAbility(...)` method executes its internal component-driven pipeline, possibly using `IDamageResolutionService` (from `Olympus.Domain`).
    5. Aggregate emits `Domain Event(s)`.
    6. Repository saves aggregate (Marten appends events to PostgreSQL).
    7. `IEventPublisher` (e.g., `MediatREventPublisher` + Outbox write) dispatches events.
    8. In-process projectors update Redis ECS cache (via `IActiveEcsCache`) and PostgreSQL read models (Marten Projections). An Outbox Relay (future) sends to external bus.
    9. Response (e.g., `Result<AbilityOutcomeDto, Error>`) flows back to Bot.
* **C. ECS Simulation Driving Change (Post-MVP):**
    1. An ECS System (e.g., `AISystem` in `Olympus.Infrastructure.ECS`, operating on component data in Redis via `IActiveEcsCache`) determines an NPC should act.
    2. The System generates a `Command` (e.g., `NpcPerformActionCommand(npcEntityId, targetEntityId, actionType)`).
    3. This command is dispatched to `Olympus.Application`.
    4. The command handler proceeds:
        * If the target is an Aggregate (e.g., PC), it loads the Aggregate, calls its methods (Flow B).
        * If the target is another generic ECS entity, it might orchestrate further ECS System interactions or directly update components in Redis (via `IActiveEcsCache`) and emit a "World State Event."
* **D. Admin Action via Web Portal (Post-MVP):**
    1. Admin in Vue Portal (`Olympus.Web.VuePortal`) interacts with a management UI.
    2. Vue app makes authenticated HTTP call to `Olympus.Api` (e.g., `PUT /api/v1/admin/aicontent/personality-components/{id}`).
    3. `AdminAiConfigurationController` dispatches `UpdatePersonalityComponentCommand`.
    4. Handler validates, uses `IPersonalityComponentProvider` (whose impl uses `Olympus.Infrastructure.Persistence.Marten`) to update the definition in PostgreSQL.
    5. `Result<Success, Error>` flows back to Web Portal.

## 7. Plan for Accomplishing an MVP

**MVP Definition:**
The core "ASAP AI Narrative Campaign Loop" allowing a single player to interact with an AI GM via the Discord bot for a simple, narrative-driven scenario. The AI provides descriptive responses, manages basic conversational context (short-term memory via Redis), and uses a simple chance mechanism (`SimpleChanceRoll` SK tool).

**Core Components to Build for MVP:**

1. **`Olympus.Api`**:
    * Endpoint: `POST /api/ai/interact`.
    * Basic bot authentication (API key). DI setup.
2. **`Olympus.Application`**:
    * Interfaces: `ISemanticKernelOrchestrator`, `IGameSessionNarrativeContextService`.
    * Command/Handler: `ProcessPlayerNarrativeInputCommand` & `Handler`.
    * DTOs: `NarrativeResponseDto`, `NarrativeContext`, `NarrativeExchange`.
    * Core types: `Result<T,E>`, `Option<T>`, `Error`, `Success`.
    * Interface: `IEventPublisher`.
3. **`Olympus.Infrastructure.Ai`**:
    * `KernelFactory` (configured for OpenAI).
    * `SemanticKernelOrchestrator` implementation.
    * `AiServiceSettings` (OpenAI config).
    * `NarrativeGmPlugin` (`GeneratePlayerTurnOutcome` skprompt.txt & config.json).
    * `CoreUtilityPlugin` (`SimpleChanceRoll.cs` native function).
    * DI registration.
4. **`Olympus.Infrastructure.Caching.Redis`**:
    * `RedisNarrativeContextCache` (implements caching part of `IGameSessionNarrativeContextService`).
    * DI registration.
5. **`Olympus.Infrastructure.Messaging.MediatR`**:
    * `MediatREventPublisher` implementation.
    * DI registration.
6. **`clients/Olympus.Bot.Discord`**:
    * `BotWorkerService` (Discord connection, command registration).
    * HTTP endpoint for Discord interactions (e.g., in `Program.cs` or `Interactions/DiscordInteractionController.cs`).
    * Simple slash command (e.g., `/interact <text>`).
    * `OlympusApiHttpClient` (to call `/api/ai/interact`).
    * Basic `DiscordMessageFormatter`.
    * Configuration for bot token & API URL.
7. **Minimal `Olympus.Domain`**: Primarily for shared Value Objects if any DTOs in the MVP flow use them (e.g., `SessionId`, `UserId` if strongly typed early). No complex aggregates for this initial MVP.

**What's OUT of MVP Scope (but planned for subsequent phases):**
Full ES Aggregate persistence for PCs/Campaigns; complex ECS simulation; DB persistence for AI personalities/character cards; advanced LLM tools; Semantic Kernel Memory for RAG; Vue.js admin portal; multi-platform bot support; detailed game system rules; full Outbox pattern for eventing; robust RBAC.

**Phased Approach Post-MVP (High-Level):**
As detailed in the previous response (Enhance AI Narrative -> Core Game Entities & State -> Deeper AI & ECS -> Game Systems & Admin Portal -> Expansion).
