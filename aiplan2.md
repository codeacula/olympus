Okay, Code, you want the full AI vision laid out before we move on – I like it! This means detailing how we'll tackle:

1. **Full NPC Personality Generation System**
2. **Character Card Generation**
3. **Advanced LLM Tooling**
4. **Semantic Kernel Memory Integration**
5. **Dynamic Prompt Management** (if needed beyond SK's standard plugin structure)
6. **Semantic Kernel Planners** (for complex, multi-step AI behaviors)

Let's architect solutions for these within the Semantic Kernel framework, focusing on how they'd integrate into the `Olympus.Infrastructure.Ai` project and be used by `Olympus.Application`.

---

**A. Full NPC Personality Generation System (Detailed)**

This expands on our previous sketch, making it more concrete.

* **Goal:** To dynamically generate rich, varied, and persistent personalities for NPCs based on archetypes and categorized "personality components" stored in a database.
* **Components & Flow:**
    1. **Data Storage (`Olympus.Infrastructure.Persistence.Marten/`):**
        * **`PersonalityComponentDefinition` Document/Table:**
            * `Id: Guid`
            * `ComponentType: string` (e.g., "Trait," "Quirk," "Motivation," "BackstoryHook")
            * `Name: string` (e.g., "Brave," "Collects Spoons," "Seeks Vengeance")
            * `Description: string` (Detailed text for the component)
            * `Category: string` (e.g., for Traits: "Combat," "Social," "Mental"; for Quirks: "Physical," "Verbal")
            * `ArchetypeAffinities: List<string>` (e.g., ["VeteranSoldier", "MysticHermit"])
            * `Tags: List<string>` (e.g., ["Positive", "Negative", "Humorous"])
        * **`NpcArchetypeDefinition` Document/Table:**
            * `Id: string` (e.g., "VeteranSoldier")
            * `DisplayName: string`
            * `CoreDescription: string` (For LLM context)
            * `SuggestedTraitCategories: List<string>`
            * `SuggestedQuirkCategories: List<string>`
    2. **Data Provider (`Olympus.Application/Abstractions/Ai/IPersonalityComponentProvider.cs`):**
        * Methods: `Task<Result<IReadOnlyList<PersonalityComponentDefinition>, Error>> GetComponentsAsync(ComponentType type, string archetype, string? category, int count, IEnumerable<Guid>? excludeIds);`
    3. **Implementation (`Olympus.Infrastructure.Ai/DataProviders/DbPersonalityComponentProvider.cs`):**
        * Uses Marten to query the personality component collections based on filters, perform random selection.
    4. **Semantic Kernel Plugin: `PersonalityEnginePlugin` (`Olympus.Infrastructure.Ai/Plugins/`):**
        * **Native Functions (C#):**
            * `[KernelFunction] public async Task<string> SelectPersonalityComponentsAsync(KernelArguments args, [Description("NPC Archetype ID")] string archetypeId, [Description("JSON string defining component types, categories, and counts needed, e.g., { \"Trait\": [{ \"Category\": \"Core\", \"Count\": 3 }, { \"Category\": \"Social\", \"Count\": 2 }], \"Quirk\": [{ \"Category\": null, \"Count\": 2 }] }")] string selectionCriteriaJson)`:
                * Parses `selectionCriteriaJson`.
                * Calls `IPersonalityComponentProvider` multiple times to fetch components.
                * Returns a structured JSON string of the selected components (their names, descriptions).
        * **Semantic Function: `SynthesizeProfileFromComponents`**
            * **`Prompts/SynthesizeProfileFromComponents/skprompt.txt`**:

                ```
                System:
                You are an expert character writer for a high-fantasy TTRPG.
                Given an NPC archetype and a set of selected personality components (traits, quirks, motivations, backstory hooks),
                craft a cohesive and engaging personality profile.

                NPC Archetype: {{archetypeDescription}}
                Selected Components (JSON):
                {{selectedComponentsJson}}

                Generate the following, ensuring consistency and depth:
                1.  **Personality Summary (2-4 sentences):** Capture the essence of the character.
                2.  **Key Motivations (elaborate on 1-2):** What truly drives them?
                3.  **Notable Quirks/Mannerisms (elaborate on 1-2):** How do their quirks manifest in behavior or speech?
                4.  **Brief Backstory (3-5 sentences):** Weave the selected backstory hook(s) into a concise narrative.
                5.  **Likely Dialogue Style/Tone (1-2 sentences):** How would they typically speak?

                Output the profile in clear Markdown with headings for each section.
                ```

            * **`config.json`**: Input variables: `archetypeDescription`, `selectedComponentsJson`.
    5. **AI Character Profile Data (`Olympus.Application/AiDrivenFeatures/DTOs/AiCharacterProfileDto.cs`):**
        * As defined before: `EntityId`, `Archetype`, `PersonalitySummary`, `ElaboratedMotivations`, `ManifestedQuirks`, `BackstorySnippet`, `DialogueStyleNotes`, `SourceComponentIds` (List of Guids of the `PersonalityComponentDefinition`s used).
    6. **Profile Persistence (`IAiCharacterProfileRepository` & `MartenAiCharacterProfileRepository`):** Stores the `AiCharacterProfileDto` (or a persistent domain version) linked to an `NpcId`.
    7. **Orchestration (`ISemanticKernelOrchestrator` & Application Command Handler):**
        * `GenerateNpcPersonalityCommand(NpcId npcId, string archetypeId, PersonalityGenerationCriteria criteria)`
        * Handler uses orchestrator. Orchestrator uses `Kernel` to:
            1. (Optional) Call a function to get `archetypeDescription` from `IPersonalityComponentProvider`.
            2. Call `PersonalityEnginePlugin.SelectPersonalityComponentsAsync` (passing `archetypeId` and `criteria`).
            3. Call `PersonalityEnginePlugin.SynthesizeProfileFromComponents` (passing `archetypeDescription` and the JSON from step 2).
            4. Parse the LLM's Markdown output into `AiCharacterProfileDto`.
            5. Return the DTO, which the handler then saves using `IAiCharacterProfileRepository`.

---

**B. Character Card Generation (Detailed)**

* **Goal:** Generate formatted text or structured data representing a "character card" for PCs or NPCs, using both their game stats and AI-enhanced personality/backstory.
* **Components & Flow:**
    1. **`CharacterCardPlugin` (`Olympus.Infrastructure.Ai/Plugins/`):**
        * **Native Function: `GatherCharacterCardContextAsync`**
            * `[KernelFunction, Description("Gathers all necessary data for generating a character card.")]`
            * `public async Task<string> GetCharacterCardContextAsync(KernelArguments args, [Description("The ID of the character or NPC.")] string entityIdStr)`
                * Parses `entityIdStr` to `EntityId` (could be `CharacterId` or `NpcId`).
                * **Fetch Core Stats:** Uses `IEcsQueryService` to get relevant components from Redis (e.g., `StatsComponent`, `HealthComponent`, `ClassLevelComponent`, `EquippedItemsComponent` for the entity).
                * **Fetch AI Profile:** Uses `IAiCharacterProfileRepository.GetProfileAsync(entityId)` to get the `AiCharacterProfileDto`.
                * **Fetch Key Domain Info (if needed beyond ECS):** For PCs, might involve an application query to get specific aggregate-mastered data not fully projected to ECS (e.g., specific known spells list if not in ECS components).
                * **Aggregate & Format:** Combines all this data into a well-structured JSON string or a detailed multi-line text block designed for an LLM to easily parse.
        * **Semantic Function: `FormatCharacterCard`**
            * **`Prompts/FormatCharacterCard/skprompt.txt`**:

                ```
                System:
                You are a heraldic scribe creating official character dossiers.
                Use the provided character data to format a compelling and informative character card.

                Character Data:
                {{GatherCharacterCardContext.GetCharacterCardContext entityId=$entityId}}

                Format the output using Markdown. Include the following sections clearly:
                - Name & Title/Archetype
                - Core Attributes (e.g., Strength, Dexterity, etc.)
                - Combat Stats (e.g., HP, Armor, Main Attack)
                - Key Skills / Proficiencies
                - Personality Highlights (from AI Profile data)
                - Brief Backstory Snippet (from AI Profile data)
                - Notable Equipment (if available in data)

                Be concise yet evocative.
                ```

            * **`config.json`**: Input variable `entityId`. Enables the `GatherCharacterCardContext` function.
    2. **Orchestration (`ISemanticKernelOrchestrator` & Application Query Handler):**
        * `GetCharacterCardQuery(EntityId entityId)`
        * Handler uses orchestrator to invoke `CharacterCardPlugin.FormatCharacterCard` with the `entityId`.
        * SK handles calling `GetCharacterCardContextAsync` first, then feeding its output to `FormatCharacterCard`.
        * Returns the formatted Markdown string as `FormattedCharacterCardDto`.

---

**C. Advanced LLM Tooling (Native SK Functions)**

* **Goal:** Provide the LLM with robust tools to interact with Olympus's game mechanics and query game state.
* **Location:** Within relevant SK Plugins in `Olympus.Infrastructure.Ai/Plugins/`.
* **Examples:**
  * **`OlympusUtilitiesPlugin/DiceRoller.cs`**:
    * `[KernelFunction, Description("Rolls dice based on standard dice notation (e.g., 2d6, 1d20+5, 4d6kh3 for keep highest 3) and returns the total result and individual rolls.")]`
    * `public Task<string> RollDiceAsync([Description("Dice notation string.")] string notation)`:
      * Implementation uses a robust dice notation parsing library.
      * Returns a structured string or JSON: `"{ \"notation\": \"2d6\", \"rolls\": [3, 5], \"total\": 8 }"`.
  * **`WorldStatePlugin/EcsQuerier.cs`** (injects `IEcsQueryService`):
    * `[KernelFunction, Description("Lists notable items visible in the specified character's current location.")]`
    * `public async Task<string> ListItemsInLocationAsync([Description("The ID of the character whose location is to be queried.")] string characterIdStr)`: Uses `IEcsQueryService` to find character's location, then query items in that location from Redis ECS. Returns a comma-separated list or JSON array.
    * `[KernelFunction, Description("Checks if a specific condition or game state flag is true.")]`
    * `public async Task<string> CheckGameStateFlagAsync([Description("The name of the game state flag to check, e.g., 'IsNightTime', 'IsAlarmRaisedInTown'.")] string flagName)`: Queries game state (could be ECS, could be Campaign aggregate state via an app query). Returns "true" or "false".
* **Registration:** All `[KernelFunction]` methods in classes added to SK `Plugins` collection are automatically available to the LLM if the prompt and SK configuration allow function calling.
* **Prompt Engineering for Tool Use:** Prompts that orchestrate complex AI behavior (like in the `NarrativeGmPlugin` or a future `AiGmPlannerPlugin`) will be written to instruct the LLM when and how to consider using these available tools.

---

**D. Semantic Kernel Memory Integration**

* **Goal:** Provide LLMs with persistent and relevant context for more coherent and knowledgeable interactions.
* **Location:** `Olympus.Infrastructure.Ai/Memory/`
* **Setup (`KernelFactory.cs`):**
  * The `KernelFactory` will be responsible for creating and configuring an `IMemoryStore`.
  * Initial/Simple: `VolatileMemoryStore` (in-memory, not persistent).
  * Production: Connectors for persistent vector stores (e.g., Qdrant, Chroma, Azure AI Search) or Redis (for simpler text memory). An `IMemoryStore` implementation would be registered.
  * An embedding generator service (e.g., `OpenAITextEmbeddingGenerationService`) is also configured in the Kernel.
* **Usage Patterns:**
    1. **Conversational Memory (Short-term for a session):**
        * The `ISemanticKernelOrchestrator` or a specific SK plugin handling dialogue could automatically save summaries of player/AI exchanges to SK memory, tagged with a `SessionId`.
        * `await kernel.Memory.SaveInformationAsync(collection: sessionId, text: "Player: ... AI: ...", id: Guid.NewGuid().ToString());`
        * Prompts can then include a step like: "Recall relevant information from our recent conversation about X: {{recall 'topic X' from_collection=$sessionId}}". (SK has mechanisms for this kind of memory recall in prompts, often by augmenting with search results from memory).
    2. **RAG for Game Lore & Rules (Long-term, knowledge base):**
        * **Pre-processing:** Game lore documents, rulebook sections, campaign notes are chunked, embedded (using an embedding model via SK), and stored in a persistent vector store (e.g., Qdrant) with SK.
        * **Runtime:** When a player asks a question about lore or a rule, or when an AI GM needs this info:
            * An SK native function (`KnowledgeBaseQuerier.SearchLoreAsync(string query)`) searches the memory: `var results = await kernel.Memory.SearchAsync(collectionName: "olympusLore", query: query, limit: 3);`
            * The relevant retrieved text chunks are injected into the prompt for the LLM to synthesize an answer.
    3. **NPC-Specific Memory:**
        * Each significant NPC could have its own memory collection (e.g., `collectionName: $"npcMemory_{npcId}"`).
        * Key facts learned during interactions ("Player gave me a healing potion," "Player threatened me") can be saved to their memory by AI-driven plugins.
        * Prompts for generating that NPC's dialogue can then be augmented with relevant memories, making the NPC more consistent and aware.

---

**E. Dynamic Prompt Management (If Needed Beyond SK Plugin Files)**

* **Default:** SK primarily uses `.skprompt.txt` and `config.json` files within plugin directories. This is great for version control and developer-managed prompts.
* **For GM/User-Customizable Prompts:** If you want GMs to be able to create/edit prompt templates via an Olympus UI:
    1. **`IPromptProvider` (`Olympus.Application/Abstractions/Ai/`)**: `Task<Result<PromptTemplateData, Error>> GetPromptTemplateAsync(string templateName);` (where `PromptTemplateData` holds the prompt string and SK config).
    2. **`DbPromptProvider` (`Olympus.Infrastructure.Ai/PromptManagement/`)**: Implements `IPromptProvider`, fetches from a PostgreSQL table (`PromptTemplates`).
    3. **`SemanticKernelOrchestrator` / `KernelFactory`**:
        * When a specific prompt function is needed that *might* be dynamic:
            * First, try to get it from the `Kernel`'s existing loaded plugins.
            * If not found, or if a "dynamic" flag is set, use `IPromptProvider` to fetch the template string and config.
            * Register it with the kernel on-the-fly: `var dynamicFunction = kernel.CreateFunctionFromPrompt(templateString, promptTemplateConfig);`
            * Then invoke `dynamicFunction`.
  * This adds complexity but offers maximum flexibility if needed. For most cases, file-based prompts within version-controlled plugins are simpler and more robust.

---

**F. Semantic Kernel Planners (For Complex, Multi-Step AI Behaviors)**

* **Goal:** Allow the AI to achieve high-level goals by dynamically creating and executing a sequence of steps using available plugins/tools.
* **When to Use:** For truly emergent AI GM behavior, complex player intent resolution, or procedural content generation that requires multiple distinct capabilities. This is likely a more advanced feature to implement after core functionalities are stable.
* **Integration:**
  * `ISemanticKernelOrchestrator` would expose a method like `Task<Result<PlanExecutionResult, Error>> ExecuteGoalAsync(string goal, PlannerOptions options)`.
  * The implementation would use an SK planner (e.g., `HandlebarsPlanner`, or newer agentic frameworks in SK if they supersede these).

        ```csharp
        // Inside SemanticKernelOrchestrator
        // Kernel already has plugins (tools) registered
        // var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions { AllowLoops = true }); // Or other planner
        // var plan = await planner.CreatePlanAsync(kernel, goal);
        // var result = await plan.InvokeAsync(kernel, arguments);
        ```

  * The `goal` would be a high-level objective, e.g., "Generate a suitable random encounter for a party of 3 level 5 adventurers resting in a dark forest at night, considering they recently fought undead."
  * The planner would try to use available plugins (DiceRoller, ECS Querier for party/environment context, NPC/Monster knowledge plugin, narrative generator plugin) to achieve this.

---

This set of proposals for the "rest of the AI parts" leverages Semantic Kernel to provide a structured, powerful, and extensible AI layer for Olympus. It balances native C# code for deterministic logic and data access with LLM prompts for creative generation and language understanding.

We can consider the AI architectural planning sufficiently detailed for this stage. Next, we'll move on to the Bot layer when you're ready!
