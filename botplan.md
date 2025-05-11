Excellent! Let's switch gears and dive into the **Bot Layer**. You've correctly identified its core purpose: the Bot is the "thing that handles communicating" with specific platforms like Discord, Twitch, etc., acting as the interactive frontend for Olympus.

The key architectural principle here is that each bot implementation (e.g., one for Discord, one for Twitch) will be a **separate client application**. These client applications will communicate with the backend `Olympus.Api` you're designing, typically via HTTP requests. They will *not* directly reference projects like `Olympus.Domain` or `Olympus.Application`.

Let's focus on designing a robust structure for this layer, using **`Olympus.Bot.Discord`** as our primary example.

---

### Bot Layer Architecture & `Olympus.Bot.Discord` Detailed Plan

**Core Responsibilities of any Olympus Bot Client:**

1. **Platform Connection & Lifecycle Management:** Connect to the specific platform (Discord Gateway, Twitch IRC/API), handle disconnections, and manage the bot's online presence.
2. **Input Processing:** Receive user inputs (slash commands, prefix commands, button clicks, direct messages, chat messages).
3. **Command Parsing & Translation:** Interpret platform-specific inputs and translate them into structured requests for the `Olympus.Api`.
4. **API Communication:** Make HTTP calls to `Olympus.Api` endpoints.
5. **Response Handling:** Process responses from `Olympus.Api`.
6. **Output Formatting & Delivery:** Format data received from the API into messages suitable for the specific platform and send them to the user/channel.
7. **User Mapping (Potentially):** Associate platform-specific user IDs (e.g., Discord User ID) with Olympus User IDs.

---

**1. Project Structure for `clients/Olympus.Bot.Discord/`**

This project would typically be a .NET Worker Service or Console Application.

```text
clients/
└── Olympus.Bot.Discord/ (Project: Olympus.Bot.Discord.csproj - .NET Worker Service)
    ├── Core/
    │   └── BotHostService.cs         // IHostedService: Manages Discord client login, command registration, event handling.
    ├── Commands/                     // Logic for handling Discord interactions
    │   ├── SlashCommandModules/      // Modules for Discord.Net InteractionService (or equivalent)
    │   │   ├── NarrativeInteractionModule.cs // e.g., /say, /act, /describe (sends to API's /ai/interact)
    │   │   ├── GameMechanicsModule.cs    // e.g., /roll, /check-skill (might call specific API endpoints or /ai/interact)
    │   │   └── CharacterModule.cs        // e.g., /character-sheet (fetches data from API)
    │   ├── PrefixCommandHandlers/      // Optional: For traditional !command style
    │   │   ├── PrefixCommandRegistry.cs
    │   │   └── GeneralTextCommandHandler.cs // Parses messages starting with '!'
    │   └── InteractionProcessingService.cs // Common logic after parsing, before calling API client
    ├── Services/
    │   ├── OlympusApiHttpClient.cs       // Typed HttpClient for reliable communication with Olympus.Api
    │   ├── DiscordMessageFormatter.cs    // Helper service to format API responses into Discord embeds, styled text
    │   └── UserProfileMapperService.cs   // Optional: Maps Discord User IDs to Olympus User IDs (might call an API endpoint for this)
    ├── Configuration/
    │   ├── DiscordBotSettings.cs       // POCO for Discord token, API base URL, default prefix, etc.
    │   └── BotCommandSettings.cs       // Configuration for specific commands, if needed
    ├── Program.cs                      // Bot host setup: DI, logging, Discord client configuration and startup.
    └── appsettings.json                // Configuration: Discord token, Olympus API URL
```

**2. Key Components & Their Roles:**

* **`BotHostService.cs` (IHostedService):**
  * **Responsibilities:**
    * Initializes and configures the Discord client library (e.g., `DiscordSocketClient` from Discord.Net).
    * Logs into Discord using the bot token.
    * Registers slash command modules with Discord.
    * Subscribes to Discord gateway events (e.g., `MessageReceived`, `InteractionCreated`).
    * Handles graceful shutdown.
  * **Why:** Provides a clean entry point and lifecycle management for the bot within the .NET host.

* **`Commands/SlashCommandModules/` (e.g., `NarrativeInteractionModule.cs`):**
  * **Responsibilities:**
    * Define slash commands recognized by Discord (e.g., `/say <message>`, `/act <action_description>`).
    * Receive interaction data from Discord when a slash command is used.
    * Extract parameters provided by the user.
    * Call the `InteractionProcessingService` or directly the `OlympusApiHttpClient` to forward the intent/data to the backend.
  * **Why:** Leverages modern Discord interaction features and library support for structured commands. Most Discord libraries provide a framework (like InteractionService in Discord.Net) that simplifies this.

* **`Commands/PrefixCommandHandlers/` (Optional):**
  * **Responsibilities:** If you support `!command` style:
    * `GeneralTextCommandHandler` (or similar) would listen to `MessageReceived` events.
    * Parse messages to check for your bot's prefix (e.g., `!`).
    * Identify the command and extract arguments.
    * Dispatch to specific logic or the `InteractionProcessingService`.
  * **Why:** Provides an alternative input method if desired. Requires more manual parsing.

* **`Commands/InteractionProcessingService.cs` (Optional but Recommended):**
  * **Responsibilities:** Acts as an intermediary between the raw Discord command handlers (slash or prefix) and the `OlympusApiHttpClient`.
    * Performs any bot-side validation or pre-processing common across command types.
    * Constructs the appropriate request DTOs for the Olympus API.
    * Calls the relevant methods on `OlympusApiHttpClient`.
  * **Why:** Decouples the raw platform interaction from the API call logic, making command handlers cleaner and promoting reuse.

* **`Services/OlympusApiHttpClient.cs`:**
  * **Responsibilities:** Encapsulates all HTTP communication with your `Olympus.Api` backend.
    * Uses `HttpClientFactory` to create and manage `HttpClient` instances.
    * Defines methods for each API endpoint the bot needs to call (e.g., `Task<NarrativeResponseDto> SendNarrativeInputAsync(NarrativeInputRequestDto request)`).
    * Handles serialization/deserialization of request/response DTOs (which should match those defined/expected by `Olympus.Api`).
    * Manages authentication with the API if the bot uses an API key.
    * Implements basic error handling and retry logic for HTTP calls if necessary.
  * **Why:** Centralizes API communication, makes it testable (can mock this service), and keeps API call details out of command handlers.

* **`Services/DiscordMessageFormatter.cs`:**
  * **Responsibilities:** Takes data returned from the `OlympusApiHttpClient` (e.g., a `NarrativeResponseDto`, `CharacterSheetDto`) and formats it into user-friendly Discord messages. This includes creating embeds, using Markdown, handling long messages (pagination), etc.
  * **Why:** Separates presentation logic for Discord from the core bot logic. Makes it easier to customize how information is displayed on Discord.

* **`Services/UserProfileMapperService.cs` (Optional):**
  * **Responsibilities:** If you need to associate a Discord User ID with an internal Olympus `UserId`.
    * Might involve a one-time `/link-account` command.
    * Could call an Olympus API endpoint like `/api/users/map-platform-id`.
    * Caches mappings locally in the bot or relies on the backend to resolve them on each request.
  * **Why:** Necessary if Olympus needs to track users across different platforms or if user-specific data is stored against an Olympus `UserId`.

* **`Configuration/DiscordBotSettings.cs`:**
  * A POCO class to bind settings from `appsettings.json` (Discord Token, Olympus API Base URL, default command prefix, etc.) using the Options pattern.

* **`Program.cs`:**
  * Sets up the .NET generic host (`Host.CreateDefaultBuilder`).
  * Configures logging, dependency injection (registers Discord client, `BotHostService`, `OlympusApiHttpClient`, `DiscordMessageFormatter`, settings, command modules/handlers).
  * Builds and runs the host.

**3. Handling Multiple Bot Implementations (Discord, Twitch, etc.):**

You're exactly right – you'd have separate projects for each platform:

* `clients/Olympus.Bot.Discord/`
* `clients/Olympus.Bot.Twitch/`
* `clients/Olympus.Bot.Slack/` (etc.)

Each would:

* Use the appropriate SDK/library for its platform (e.g., TwitchLib for C# for Twitch).
* Implement its own platform-specific command parsing and message formatting.
* But importantly, they would all likely use a **similar pattern to call the `Olympus.Api`**.
  * They could each have their own `OlympusApiHttpClient.cs` (could be some code duplication here for the HTTP call logic).
  * **Better for reuse:** You could create a small, shared **client library (NuGet package)**, say `Olympus.ApiClient`, that both `Olympus.Bot.Discord` and `Olympus.Bot.Twitch` (and any other future clients like a web UI) could reference. This library would contain the DTOs for API interaction and the typed `HttpClient` logic for calling the `Olympus.Api`. This reduces duplication and ensures all clients speak the same "language" to your API.

**4. Interaction with Olympus API:**

The Bot layer will primarily:

* Send player narrative input to an endpoint like `POST /api/ai/interact` (which triggers the `ProcessPlayerNarrativeInputCommandHandler`).
* Call endpoints to fetch data for display (e.g., `GET /api/characters/{characterId}/sheet`).
* Call endpoints for more structured game actions if you choose to implement them beyond pure natural language for AI processing (e.g., `POST /api/game/roll-dice`).
* Potentially call user management endpoints (`POST /api/users/link-discord-id`).

**5. Authentication & User Identity:**

* **Bot to API:** The bot client itself might use an API key to authenticate with the `Olympus.Api`. This key would be configured in the bot's `appsettings.json`.
* **Player to Olympus User:**
    1. The bot receives an interaction from a platform user (e.g., Discord `Context.User.Id`).
    2. When making an API call to `Olympus.Api` on behalf of that user, the bot needs to tell the API *which platform user* this is.
    3. The `Olympus.Api` backend (likely an authentication/user service) would then be responsible for mapping this `PlatformUserId` (e.g., "Discord:123456789012345678") to an internal Olympus `UserId`.
    4. This mapping might happen via:
        * A one-time account linking process initiated by the user.
        * Automatic account creation on first interaction (if allowed).
    5. The Olympus `UserId` is then used by the Application layer for all internal operations.

This structure allows each bot to be tailored to its platform's strengths and unique features while ensuring they all interact with a consistent, well-defined backend API. The API remains the central orchestrator of game logic and AI processing.

This detailed plan for the Bot Layer, starting with Discord, should give you a solid foundation. The key is the separation of platform-specific concerns within each bot project and standardized communication with the `Olympus.Api`.
