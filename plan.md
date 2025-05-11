# The Olympus Advanced Blueprint (Final Draft - May 11, 2025)

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
3. **`Olympus.Application` Layer**: Orchestrates application-specific use cases (CQRS command/query handlers). It defines DTOs and contains application logic, using interfaces to interact with the Domain Layer and Infrastructure Layer.
4. **`Olympus.Domain` Layer**: The heart of the business logic. Contains domain entities (Aggregates), Value Objects, Domain Events, and repository interfaces. It is independent of other layers.
5. **`Olympus.Infrastructure.ECS`**: The Entity Component System. Manages the live, dynamic state of the game world (generic NPCs, environmental objects, etc.) primarily within Redis, running simulation logic via its Systems.
6. **`Olympus.Infrastructure.Ai` (Semantic Kernel based)**: Manages all LLM interactions, including prompt orchestration, tool use (native functions), memory, and specific AI model integrations (starting with OpenAI).
7. **`Olympus.Infrastructure.Persistence.Marten` & `Olympus.Infrastructure.Caching.Redis`**: Handle durable data storage (PostgreSQL via Marten for ES aggregates, snapshots) and active/cached game state (Redis for ECS, narrative context).
8. **`Olympus.Infrastructure.Messaging.*`**: Manages event propagation, starting with in-process (MediatR) and designed for future extension to distributed message buses (e.g., RabbitMQ with an Outbox pattern).

**Key Data Flow for a Command:**
Client -> `Olympus.Api` -> `Olympus.Application` Command Handler -> (Interacts with `Olympus.Domain` Aggregate OR `Olympus.Infrastructure.ECS` via Application Service) -> `Olympus.Infrastructure.Persistence/Caching` -> Domain Events/World State Events Published -> Projections Updated (including ECS state in Redis and other read models) -> Response back through layers to Client.

## 4. Detailed Project & Folder Structure

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

├── Olympus.Application/ (Project: Olympus.Application.csproj)
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
│   ├── Campaigns/ (Feature Slice - Commands/, Queries/, DTOs/, EventHandlers/)
│   ├── Characters/ (Feature Slice - Commands/, Queries/, DTOs/, EventHandlers/)
│   ├── GameWorldInteractions/ (Feature Slice for generic ECS entities - Commands/, Queries/, DTOs/, EventHandlers/)
│   ├── AiDrivenFeatures/ (Feature Slice - Commands/, Queries/, DTOs/, EventHandlers/)
│   │   ├── Commands/
│   │   │   ├── ProcessPlayerNarrativeInput/
│   │   │   │   ├── ProcessPlayerNarrativeInputCommand.cs
│   │   │   │   └── ProcessPlayerNarrativeInputCommandHandler.cs
│   │   │   ├── InterpretPlayerIntent/
│   │   │   └── GenerateNpcPersonality/
│   │   ├── Queries/
│   │   │   ├── GetAiGeneratedSceneDetails/
│   │   │   └── PopulateCharacterCard/
│   │   └── DTOs/ (NarrativeResponseDto.cs, AiCharacterProfileDto.cs, etc.)
│   ├── Common/
│   │   ├── Behaviors/ (ValidationBehavior.cs, LoggingBehavior.cs, UnitOfWorkBehavior.cs, ResultMappingBehavior.cs)
│   │   ├── Errors/ (Error.cs, ValidationError.cs, NotFoundError.cs, etc.)
│   │   ├── Types/ (Result.cs, Option.cs, Success.cs)
│   │   └── Mappings/ (DtoMapperProfiles.cs)
│   └── DependencyInjection.cs

├── Olympus.Infrastructure/
│   ├── Olympus.Infrastructure.Persistence.Marten/ (Project)
│   │   ├── Repositories/ (MartenCharacterRepository.cs, etc.)
│   │   ├── Projections/ (CampaignSummaryProjection.cs)
│   │   ├── EcsEntitySnapshots/ (IEcsSnapshotStore impl: MartenEcsSnapshotStore.cs)
│   │   ├── AiData/ (MartenAiCharacterProfileRepository.cs, MartenPersonalityComponentProvider.cs)
│   │   ├── Outbox/ (OutboxMessage.cs)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Caching.Redis/ (Project)
│   │   ├── Abstractions/ (IActiveEcsCache.cs, INarrativeContextCache.cs)
│   │   ├── Services/ (RedisActiveEcsCache.cs, RedisNarrativeContextCache.cs)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.ECS/ (Project)
│   │   ├── Core/ (ECS Framework integration/custom core)
│   │   ├── Components/ (PositionComponent.cs, HealthComponent.cs, AIStateComponent.cs, etc.)
│   │   ├── Systems/ (AISystem.cs, MovementSystem.cs, EcsCombatSystem.cs, etc.)
│   │   ├── Events/ (EcsEntityDamagedEvent.cs, etc. - "World State Events")
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Ai/ (Project - Semantic Kernel Integration)
│   │   ├── KernelServices/ (KernelFactory.cs, SemanticKernelOrchestrator.cs, AiServiceSettings.cs)
│   │   ├── Plugins/ (SK Plugins: NarrativeGmPlugin, CoreUtilityPlugin, CharacterCardPlugin, PersonalityEnginePlugin, etc.)
│   │   │   ├── NarrativeGmPlugin/GeneratePlayerTurnOutcome/ (skprompt.txt, config.json)
│   │   │   ├── CoreUtilityPlugin/NativeFunctions/ (SimpleChanceRoll.cs, DiceRoller.cs)
│   │   │   └── ... (other plugins as detailed previously)
│   │   ├── PromptManagement/ (FileBasedPromptProvider.cs, PromptTemplateFiles/)
│   │   ├── DataProviders/ (DbPersonalityComponentProvider.cs - if different from Persistence.AiData)
│   │   ├── Memory/ (SemanticMemoryConfiguration.cs, MemoryStoreFactory.cs)
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Messaging.MediatR/ (Project)
│   │   ├── MediatREventPublisher.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Messaging.RabbitMQ/ (Future Project)
│   ├── Olympus.Infrastructure.Clients.CampaignService/ (Future Project)
│   ├── Olympus.Infrastructure.CommonServices/ (DateTimeProvider.cs, HttpUserProvider.cs, etc.)

├── Olympus.Api/ (Project: Olympus.Api.csproj - ASP.NET Core Web API)
│   ├── Controllers/ (CampaignsController.cs, CharactersController.cs, AiInteractionController.cs, Admin...Controller.cs, etc.)
│   ├── DTOs/ (API-specific request DTOs if not using Commands directly)
│   ├── Middleware/ (GlobalErrorHandlingMiddleware.cs)
│   ├── Program.cs (Composition Root, DI, Auth, CORS)
│   ├── appsettings.json
│   └── appsettings.Development.json

clients/ // Client Applications
├── Olympus.Bot.Discord/ (Project: Olympus.Bot.Discord.csproj - Discord Bot)
│   ├── Core/ (BotWorkerService.cs)
│   ├── Interactions/ (DiscordInteractionController.cs / Minimal API for webhooks, InteractionValidationService.cs)
│   ├── Commands/ (SlashCommandModules/, PrefixCommandHandlers/)
│   ├── Services/ (OlympusApiHttpClient.cs, DiscordMessageFormatter.cs, UserProfileMapperService.cs)
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

## 5. Layer & Component Responsibilities (Summary - As detailed in prior responses)

* **Domain**: Pure business logic, entities, VOs, ES event definitions, repository interfaces.
* **Application**: Use cases (CQRS), DTOs, orchestration, application-level interfaces (event publishing, AI orchestration, context services).
* **Infrastructure**: Concrete implementations for persistence (Marten), caching (Redis), ECS (systems, components), AI (Semantic Kernel, plugins, LLM connectors), messaging (MediatR, future bus clients), and other external concerns.
* **API**: HTTP interface, request handling, auth, DI composition.
* **Clients**: Platform-specific frontends (Discord Bot, Vue Web Portal) that consume the API.

## 6. Key Workflows Illustrated (Summary - As detailed in prior responses)

* **Player Narrative Interaction via Bot**: Input -> API -> App Handler -> AI Orchestrator (SK) -> Narrative Context (Redis) -> SK Plugin (Prompt + Tool) -> LLM -> Response -> Context Update -> API -> Bot -> Player.
* **Structured Game Action via PC Aggregate**: Input -> API -> App Handler -> Load PC Aggregate (Marten) -> Aggregate Method (internal component pipeline + Domain Services) -> Domain Events Emitted -> Save Aggregate (Marten appends events) -> Events Published -> Projections Updated (ECS in Redis, other read models) -> Response -> Bot -> Player.
* **ECS Simulation Driving Change**: ECS System (on Redis data) -> Generates Command -> App Handler -> Targets Aggregate (as above) OR interacts with generic ECS entity state.
* **Admin Action via Web Portal**: Vue UI -> API -> Admin App Handler -> Persistence (Marten for aggregates, AI data, ECS snapshots) -> Response.

## 7. Entity Archetypes & State Management Strategy

* **Player Characters (PCs)**: Full DDD/ES Aggregates (Marten). Active state projected to Redis for ECS.
* **Major NPCs/Quest-Critical Entities**: Likely DDD/ES Aggregates. Active state projected to Redis.
* **Generic NPCs & Common Interactables (Doors, Chests)**: State primarily in ECS components (Redis). Persistence via snapshots to PostgreSQL (Marten documents). Logic by ECS Systems & Application Services. Dynamically instantiated.

## 8. Aggregate Design in an ECS-Influenced World

* Aggregates are transactional boundaries and authoritative sources of their domain events.
* Internal state uses "component-like" Value Objects (e.g., `HealthPointsVO`, `ActiveEffectsVOs`).
* Aggregate methods orchestrate internal data pipelines, applying logic from these VOs and shared Domain Services (like `IDamageResolutionService`) to determine outcomes and events. This avoids complex conditionals directly in aggregate methods.

## 9. ECS Core: Game World Simulation & Read Models

* **Simulation**: Actively manages and simulates NPCs, environment, physics, ongoing effects in Redis. ECS Systems drive this logic.
* **Read Models**: Serves as a high-performance read model, populated by Domain Events (from Aggregates) and World State Events (from ECS Systems).
* **Interaction Loop**: Commands affect Aggregates OR generic ECS entities. Aggregates emit Domain Events. ECS Systems change components and emit World State Events. All events update the ECS read model state in Redis, potentially triggering further ECS System logic or new commands.

## 10. Active Game State Management (Redis & PostgreSQL)

* **Redis**: Primary store for active ECS component data and short-term narrative context.
* **PostgreSQL (with Marten)**: Authoritative store for ES Aggregate event streams and snapshots; durable store for snapshots of persistent non-aggregate ECS entities; stores static game data and long-term read models.
* **Flows**: Hydration (PG to Redis on session start), Live Operations (ECS on Redis; Aggregates to PG, then events update Redis), Dehydration/Checkpointing (Redis snapshots to PG for persistent generic entities).

## 11. Event Propagation Strategy (Internal & External)

* `IDomainEvent` marker interface. `IEventPublisher` abstraction (in `Olympus.Application`).
* **Initial:** `Olympus.Infrastructure.Messaging.MediatR` for in-process dispatch. The publisher can write events to an Outbox table (in PostgreSQL) *and* publish locally to MediatR.
* **Future:** Dedicated infrastructure projects (e.g., `Olympus.Infrastructure.Messaging.RabbitMQ`) with an `OutboxRelayWorker` to publish from Outbox to an external bus. Idempotent consumers are key.

## 12. Command & Query Dispatch Strategy (Internal & Future Microservices)

* **In-Process (Modular Monolith):** Direct use of `IMediator` (MediatR library). Handlers in `Olympus.Application`.
* **Future Microservices:** When a bounded context is extracted:
  * The new microservice exposes its own API (REST, gRPC, or message queues for async commands).
  * Calling services use dedicated **Service Gateway interfaces** (defined in their Application layer) with implementations in their Infrastructure layer that handle the remote communication.

## 13. Advanced C# Feature Usage (Summary)

* Records, Primary Constructors, `required` members (for DTOs, Commands, VOs, Events).
* Strongly-Typed IDs & Value Objects (`readonly record struct`).
* `Result<TSuccess, TError>` and `Option<T>`.
* Static abstract members in interfaces (e.g., `IParsable` on VOs).
* File-local types for encapsulation.
* Extensive Pattern Matching.
* Source-Generated Logging (`LoggerMessageAttribute`).
* Modern Async: `IAsyncEnumerable<T>`, `ValueTask<T>`.

## 14. AI Layer (Semantic Kernel based)

* **`Olympus.Application/Abstractions/Ai/ISemanticKernelOrchestrator.cs`**: Main app interface to AI.
* **`Olympus.Infrastructure.Ai/`**:
  * **`KernelServices/`**: `KernelFactory` configures SK `Kernel` with OpenAI connector, plugins, memory. `SemanticKernelOrchestrator` implements the abstraction.
  * **`Plugins/`**: Houses SK Plugins (mix of native C# functions and semantic prompt functions).
    * `NarrativeGmPlugin`: For core narrative generation (e.g., `GeneratePlayerTurnOutcome` semantic function).
    * `CoreUtilityPlugin`: Tools like `SimpleChanceRoll` and `DiceRoller` (native functions).
    * `PersonalityEnginePlugin`: Native functions to fetch personality components (from `IPersonalityComponentProvider` / DB) and semantic functions to synthesize full personalities.
    * `CharacterCardPlugin`: Native function to gather character data (ECS & AI Profile), semantic function to format the card.
  * **`DataProviders/`**: `DbPersonalityComponentProvider` fetches traits, quirks, etc., from PostgreSQL.
  * **`Memory/`**: Placeholder for SK Memory integration (e.g., RAG for lore, conversational history).
  * Database storage (via `IAiCharacterProfileRepository` in App, impl in Persistence) for generated NPC personalities and for personality building blocks.

## 15. Bot Layer (Discord Example - `clients/Olympus.Bot.Discord/`)

* Separate process, acts as an HTTP client to `Olympus.Api`.
* **`Core/BotWorkerService.cs`**: Manages Discord client lifecycle.
* **`Interactions/DiscordInteractionController.cs` (or Minimal API in `Program.cs`):** Receives and validates Discord Interaction Webhooks (POST requests).
* **`Commands/SlashCommandModules/`**: Handles specific slash commands, using Discord.Net `InteractionService` for dispatch.
* **`Services/OlympusApiHttpClient.cs`**: Typed client for all communication with `Olympus.Api` (sends commands like `ProcessPlayerNarrativeInputCommand`, handles API responses/errors).
* **`Services/DiscordMessageFormatter.cs`**: Formats API responses into Discord messages (text, embeds).
* User identity involves passing platform user ID to API for backend resolution to Olympus `UserId`.

## 16. Web Portal Client (Vue.js Example - `clients/Olympus.Web.VuePortal/`)

* Separate SPA, acts as an HTTP client to `Olympus.Api`.
* Primarily for management tasks (campaigns, users, AI data, game systems).
* Requires `Olympus.Api` to have robust AuthN/AuthZ (e.g., JWTs, RBAC) and CORS.
* Drives the need for admin-specific CQRS commands/queries and API endpoints.

## 17. Plan for Accomplishing an MVP

**MVP Definition:**
The core "ASAP AI Narrative Campaign Loop" allowing a single player to interact with an AI GM via the Discord bot for a simple, narrative-driven scenario. The AI provides descriptive responses, manages basic conversational context (short-term memory), and uses a simple chance mechanism (`SimpleChanceRoll` tool).

**Core Components to Build for MVP:**

1. **`Olympus.Api`**:
    * Endpoint: `POST /api/ai/interact`.
    * Basic bot authentication (e.g., API key validation).
    * DI for MVP services.
2. **`Olympus.Application`**:
    * Interfaces: `ISemanticKernelOrchestrator`, `IGameSessionNarrativeContextService`.
    * Command/Handler: `ProcessPlayerNarrativeInputCommand` & `Handler`.
    * DTOs: `NarrativeResponseDto`, `NarrativeContext`, `NarrativeExchange`.
    * Utility types: `Result<T,E>`, `Option<T>`, `Error`.
    * Interface: `IEventPublisher` (with MediatR implementation for now).
3. **`Olympus.Infrastructure.Ai`**:
    * `KernelFactory` (configured for OpenAI).
    * `SemanticKernelOrchestrator` implementation.
    * `AiServiceSettings` (OpenAI key, model).
    * `NarrativeGmPlugin` (`GeneratePlayerTurnOutcome` skprompt.txt & config.json).
    * `CoreUtilityPlugin` (`SimpleChanceRoll.cs` native function).
    * DI registration.
4. **`Olympus.Infrastructure.Caching.Redis`**:
    * `RedisGameSessionNarrativeContextService` implementation.
    * DI registration.
5. **`Olympus.Infrastructure.Messaging.MediatR`**:
    * `MediatREventPublisher` implementation.
    * DI registration.
6. **`clients/Olympus.Bot.Discord`**:
    * `BotWorkerService` (basic Discord connection).
    * HTTP endpoint for Discord interactions (slash command webhook).
    * A simple slash command (e.g., `/interact <text>`).
    * `OlympusApiHttpClient` (to call `/api/ai/interact`).
    * `DiscordMessageFormatter` (basic text display).
    * Configuration for bot token & API URL.
7. **Minimal `Olympus.Domain`**: VOs like `SessionId`, `UserId` if strictly needed (can be strings for MVP if passed from bot). No complex aggregates or ES for this initial MVP.

**What's OUT of MVP Scope (but planned for subsequent phases):**

* Full Event Sourcing for Player Characters, Campaigns, Major NPCs.
* Complex ECS simulation and detailed component management beyond basic narrative context.
* Database persistence for AI-generated personalities, character cards, or personality component definitions.
* Advanced LLM tools beyond `SimpleChanceRoll` (e.g., full `DiceRoller`, game state query tools).
* Semantic Kernel Memory for RAG or long-term NPC/conversation memory.
* Vue.js admin portal.
* Multi-platform bot support (focus on Discord first).
* Detailed game system rule integration (D&D 5e, Shadowrun).
* Full-fledged Outbox pattern for eventing (though `IEventPublisher` is ready for it).
* Robust, granular authorization and comprehensive error handling across all layers.

**Phased Approach Post-MVP (High-Level):**

1. **Enhance AI Narrative & Context:** Implement basic AI Character Profile storage; rudimentary Personality Generation; allow `NarrativeGmPlugin` to use basic NPC personas.
2. **Core Game Entities:** Implement `PlayerCharacter` Aggregate with basic ES; command handlers for creation/modification; project PC events to ECS components in Redis.
3. **Deeper AI & ECS Integration:** Full DB-driven Personality Component system; Character Card generation; more ECS systems (movement, basic interactions); Semantic Kernel Memory.
4. **Game Systems & Admin Portal:** Begin integrating specific TTRPG rule mechanics; develop the Vue.js Admin Portal.
5. **Expansion:** Other bot clients, advanced SK Planners, community features, etc.
