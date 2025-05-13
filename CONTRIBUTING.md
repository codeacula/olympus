# Contributing to Olympus

Thanks for your interest in improving Olympus! Here's how to get started:

## 🧭 Before You Begin

1. **Search existing issues** to avoid duplicates.
2. **Open an issue** before submitting a Pull Request — let's discuss!
3. Join us on **Discord** to chat ideas, blockers, or collab opportunities.

## 🛠️ Dev Setup

1. Clone the repo
2. Run setup scripts (TBD)
3. Use `.editorconfig` and style rules to stay consistent

> NOTE: This project uses **event sourcing**, **CQRS**, and **modular domain layers**. Take time to understand the architecture before modifying aggregates or services.

## ✨ Code Guidelines

- Follow the existing patterns (value objects, aggregates, MediatR commands).
- Keep AI prompts and plugin config declarative.
- Structure new features like a vertical slice.

## ✅ Pull Request Checklist

- [ ] Related issue exists
- [ ] Feature or fix is MVP-compatible or isolated
- [ ] Tests or manual validation steps included
- [ ] Does not break bot or API

## 🧪 Test & Validate

> Testing details TBD — currently manual interaction via Discord is main loop.
