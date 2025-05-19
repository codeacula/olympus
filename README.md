# Olympus

**Olympus** is a next-generation AI-powered tabletop platform that simulates immersive narrative campaigns through a modular, event-sourced architecture. Players interact via Discord while AI Game Masters generate responsive narratives, powered by Semantic Kernel, Redis state management, and a layered C# application.

---

## 🔧 Architecture

Olympus is built using:

- **CQRS + Event Sourcing** (via Marten/PostgreSQL) for domain modeling
- **Entity Component System (ECS)** using Redis for real-time world state
- **AI Integration** using Semantic Kernel plugins and prompt orchestration
- **Discord Bot** for player interaction
- **Vue.js Portal** (planned) for campaign and world admin

> See [`docs/architecture.md`](docs/architecture.md) and [`docs/ai.md`](docs/ai.md)

---

## 🚀 MVP: AI Narrative Loop

The initial MVP enables a single player to narrate actions via a Discord command. Olympus returns AI-generated narrative results based on short-term memory, chance, and character context.

> See [`docs/mvp-plan.md`](docs/mvp-plan.md) and [`docs/stories.md`](docs/stories.md)

---

## 🛠️ Development

- C#/.NET 9
- Redis
- PostgreSQL (via Marten)
- Semantic Kernel
- Discord.Net

> Setup instructions, dev guide, and contribution flow: [`CONTRIBUTING.md`](CONTRIBUTING.md)

---

## 🤝 Community

- Issues and feature requests welcome!
- Be kind, inclusive, and constructive.
- Join the conversation on **Discord**: [invite link here]
