Here is the current "Olympus Advanced Blueprint" folder and file structure, incorporating Clean Architecture, CQRS/ES with Marten, the advanced C# patterns, the core ECS Game World Simulation, active game state management with Redis, pluggable event publishing, the Semantic Kernel-based AI integration, and the initial structure for Bot clients.

This is the "mostly finalized" version we can use as a baseline before we dive deeper specifically into the AI and Bot layer implementations.

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
│   │   │   ├── IPromptProvider.cs
│   │   │   └── IPluginProvider.cs      // For discovering Semantic Kernel plugins
│   │   └── Gateways/
│   │       └── IRemoteCampaignServiceGateway.cs // Example for future microservice
│   ├── Campaigns/
│   │   ├── Commands/
│   │   │   └── CreateCampaign/
│   │   │       ├── CreateCampaignCommand.cs
│   │   │       └── CreateCampaignCommandHandler.cs
│   │   │   └── AdvanceCampaignPhase/
│   │   │       ├── AdvanceCampaignPhaseCommand.cs
│   │   │       └── AdvanceCampaignPhaseCommandHandler.cs
│   │   ├── Queries/
│   │   │   └── GetCampaignDetails/
│   │   │       ├── GetCampaignDetailsQuery.cs
│   │   │       └── GetCampaignDetailsQueryHandler.cs
│   │   ├── DTOs/
│   │   │   ├── CampaignDto.cs
│   │   │   └── CampaignPhaseDto.cs
│   │   └── EventHandlers/
│   │       └── CampaignCreatedEventHandler.cs
│   ├── Characters/
│   │   ├── Commands/
│   │   │   └── CreatePlayerCharacter/
│   │   │   │   ├── CreatePlayerCharacterCommand.cs
│   │   │   │   └── CreatePlayerCharacterCommandHandler.cs
│   │   │   └── ApplyDamageToCharacter/
│   │   │       ├── ApplyDamageToCharacterCommand.cs
│   │   │       └── ApplyDamageToCharacterCommandHandler.cs
│   │   ├── Queries/
│   │   │   └── GetCharacterSheet/
│   │   │       ├── GetCharacterSheetQuery.cs
│   │   │       └── GetCharacterSheetQueryHandler.cs
│   │   ├── DTOs/
│   │   │   └── CharacterSheetDto.cs
│   │   └── EventHandlers/
│   ├── GameWorldInteractions/
│   │   ├── Commands/
│   │   │   └── DamageWorldEntity/
│   │   │       ├── DamageWorldEntityCommand.cs
│   │   │       └── DamageWorldEntityCommandHandler.cs
│   │   ├── Queries/
│   │   │   └── GetWorldEntityDetails/
│   │   │       ├── GetWorldEntityDetailsQuery.cs
│   │   │       └── GetWorldEntityDetailsQueryHandler.cs
│   │   ├── DTOs/
│   │   │   └── WorldEntityDto.cs
│   │   └── EventHandlers/
│   │       └── DoorDamagedEventHandler.cs
│   ├── AiDrivenFeatures/
│   │   ├── Commands/
│   │   │   └── InterpretPlayerIntent/
│   │   │       ├── InterpretPlayerIntentCommand.cs
│   │   │       └── InterpretPlayerIntentCommandHandler.cs
│   │   │   └── GenerateDynamicNpcResponse/
│   │   │       ├── GenerateDynamicNpcResponseCommand.cs
│   │   │       └── GenerateDynamicNpcResponseCommandHandler.cs
│   │   ├── Queries/
│   │   │   └── GetAiGeneratedSceneDetails/
│   │   │       ├── GetAiGeneratedSceneDetailsQuery.cs
│   │   │       └── GetAiGeneratedSceneDetailsQueryHandler.cs
│   │   ├── DTOs/
│   │   │   ├── PlayerIntentAnalysisDto.cs
│   │   │   ├── NpcDialogueResponseDto.cs
│   │   │   └── GeneratedSceneElementsDto.cs
│   │   └── EventHandlers/
│   ├── Common/
│   │   ├── Behaviors/
│   │   │   ├── ValidationBehavior.cs
│   │   │   ├── LoggingBehavior.cs
│   │   │   ├── UnitOfWorkBehavior.cs
│   │   │   └── ResultMappingBehavior.cs
│   │   ├── Errors/
│   │   │   ├── Error.cs
│   │   │   ├── ValidationError.cs
│   │   │   ├── NotFoundError.cs
│   │   │   └── ConflictError.cs
│   │   ├── Types/
│   │   │   ├── Result.cs
│   │   │   ├── Option.cs
│   │   │   └── Success.cs
│   │   └── Mappings/
│   │       └── DtoMapperProfiles.cs
│   └── DependencyInjection.cs

├── Olympus.Infrastructure/
│   ├── Olympus.Infrastructure.Persistence.Marten/
│   │   ├── Repositories/
│   │   │   ├── MartenCharacterRepository.cs
│   │   │   ├── MartenCampaignRepository.cs
│   │   │   └── MartenMajorNPCRepository.cs
│   │   ├── Projections/
│   │   │   └── CampaignSummaryProjection.cs
│   │   ├── EcsEntitySnapshots/
│   │   │   ├── IEcsSnapshotStore.cs
│   │   │   └── MartenEcsSnapshotStore.cs
│   │   ├── Outbox/
│   │   │   └── OutboxMessage.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Caching.Redis/
│   │   ├── Abstractions/
│   │   │   └── IActiveEcsCache.cs
│   │   ├── Services/
│   │   │   └── RedisActiveEcsCache.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.ECS/
│   │   ├── Core/
│   │   ├── Components/
│   │   │   ├── PositionComponent.cs
│   │   │   ├── VelocityComponent.cs
│   │   │   ├── HealthComponent.cs
│   │   │   ├── AIStateComponent.cs
│   │   │   ├── MaterialComponent.cs
│   │   │   ├── DestructibleComponent.cs
│   │   │   └── PlayerControlledComponent.cs
│   │   ├── Systems/
│   │   │   ├── AISystem.cs
│   │   │   ├── MovementSystem.cs
│   │   │   ├── PhysicsSystem.cs
│   │   │   ├── EcsCombatSystem.cs
│   │   │   ├── WorldRuleSystem.cs
│   │   │   └── EcsEventEmissionSystem.cs
│   │   ├── Events/
│   │   │   ├── EcsEntityDamagedEvent.cs
│   │   │   ├── EcsEntityDestroyedEvent.cs
│   │   │   └── EcsPositionChangedEvent.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Ai/ (Project: Olympus.Infrastructure.Ai.csproj - Semantic Kernel Integration)
│   │   ├── KernelServices/
│   │   │   ├── KernelFactory.cs
│   │   │   ├── SemanticKernelOrchestrator.cs // Implements ISemanticKernelOrchestrator
│   │   │   └── AiServiceSettings.cs
│   │   ├── Plugins/                        // Semantic Kernel Plugins (Skills)
│   │   │   ├── OlympusCorePlugin/
│   │   │   │   ├── NativeFunctions/
│   │   │   │   │   ├── DiceRoller.cs
│   │   │   │   │   └── CharacterLookUp.cs
│   │   │   │   └── Prompts/
│   │   │   │       └── GetGameRuleInfo/
│   │   │   │           ├── skprompt.txt
│   │   │   │           └── config.json
│   │   │   ├── GameMasterAssistantPlugin/
│   │   │   │   ├── Prompts/
│   │   │   │   │   └── GenerateSceneDescription/
│   │   │   │   │   └── InterpretPlayerActionIntent/
│   │   │   │   └── NativeFunctions/
│   │   │   │       └── WorldStateQueries.cs
│   │   │   └── PluginRegistration.cs
│   │   ├── PromptManagement/
│   │   │   ├── FileBasedPromptProvider.cs
│   │   │   └── PromptTemplateFiles/
│   │   ├── Memory/
│   │   │   ├── SemanticMemoryConfiguration.cs
│   │   │   └── MemoryStoreFactory.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Messaging.MediatR/
│   │   ├── MediatREventPublisher.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Messaging.RabbitMQ/ (Future Project)
│   │   ├── RabbitMQOutboxEventPublisher.cs
│   │   ├── RabbitMQRelayWorker.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.Clients.CampaignService/ (Future Project)
│   │   ├── CampaignServiceHttpGateway.cs
│   │   └── DependencyInjection.cs
│   ├── Olympus.Infrastructure.CommonServices/
│   │   ├── DateTimeProvider.cs
│   │   ├── HttpUserProvider.cs
│   │   └── DependencyInjection.cs

├── Olympus.Api/ (Project: Olympus.Api.csproj - ASP.NET Core Web API)
│   ├── Controllers/
│   │   ├── CampaignsController.cs
│   │   ├── CharactersController.cs
│   │   ├── GameWorldController.cs
│   │   └── AiInteractionController.cs
│   ├── DTOs/
│   │   └── CreateCampaignApiRequest.cs
│   ├── Middleware/
│   │   └── GlobalErrorHandlingMiddleware.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json

clients/ // << CLIENT APPLICATIONS INTERACTING WITH OLYMPUS.API >>
├── Olympus.Bot.Discord/ (Project: Olympus.Bot.Discord.csproj - Example Discord Bot Client)
│   ├── Core/
│   │   └── BotWorkerService.cs           // Main IHostedService for the bot
│   ├── Commands/                       // Handling Discord interactions
│   │   ├── SlashCommandModules/
│   │   │   └── GamePlayModule.cs
│   │   └── MessageHandlers/
│   │       └── PlayerInputHandler.cs
│   ├── Services/
│   │   ├── OlympusApiHttpClient.cs       // Typed HTTP client for Olympus.Api
│   │   └── DiscordMessageFormatter.cs
│   ├── Configuration/
│   │   └── DiscordBotSettings.cs
│   ├── Program.cs
│   └── appsettings.json
├── Olympus.Bot.Twitch/ (Future Project - Example Twitch Bot Client)
│   └── ... // Similar structure for Twitch-specific logic and API interaction
└── Olympus.Web.BlazorUI/ (Future Project - Example Web UI Client)
    └── ... // Structure for a Blazor Web App client

tests/
├── Olympus.Tests.Domain/
│   ├── Aggregates/
│   │   └── CharacterTests.cs
│   └── ValueObjects/
│       └── HealthPointsTests.cs
├── Olympus.Tests.Application/
│   ├── Campaigns/ (...)
│   ├── Characters/ (...)
│   ├── AiDrivenFeatures/
│   │   └── InterpretPlayerIntentCommandHandlerTests.cs
│   └── Common/ (...)
├── Olympus.Tests.Infrastructure/
│   ├── Persistence/ (...)
│   ├── ECS/ (...)
│   ├── Ai/
│   │   ├── KernelServices/
│   │   │   └── SemanticKernelOrchestratorTests.cs
│   │   └── Plugins/ (...)
│   └── Messaging/ (...)
├── Olympus.Tests.Clients/
│   └── Olympus.Tests.Bot.Discord/
│       └── Commands/
│           └── GamePlayModuleTests.cs
├── Olympus.Tests.Integration/
│   ├── Api/
│   │   └── CampaignsControllerIntegrationTests.cs
│   └── Scenarios/
│       └── FullCharacterDamageScenarioTests.cs
└── Olympus.Tests.Architecture/ (Optional Project)
    └── DependencyRuleTests.cs
```

This should give you a comprehensive snapshot of the planned structure!

You're absolutely right about the **Bot layer** – it's the interface to specific platforms like Discord, Twitch, etc. Each bot implementation (`Olympus.Bot.Discord`, `Olympus.Bot.Twitch`) will be its own client application. It will:

1. Handle platform-specific communication (receiving messages/commands from Discord, Twitch chat, etc.).
2. Parse those platform inputs.
3. Translate them into appropriate calls (likely HTTP requests) to the `Olympus.Api` backend. This is where it converts "Discord slash command `/roll 2d6`" or "Twitch chat message `!cast fireball @target`" into a structured request for your API.
4. Receive the response from `Olympus.Api`.
5. Format that response for the specific platform and send it back to the user.

Each bot project in the `clients/` directory would be self-contained regarding its platform interaction logic but would use a shared way of communicating with your `Olympus.Api` (e.g., via the `OlympusApiHttpClient` which would understand the API's DTOs and endpoints).

Now, let's focus on the AI and Bot layers in more detail as you suggested! Where would you like to start with that deep dive? AI architecture with Semantic Kernel, or Bot architecture and its interaction patterns?
