# Hollow: Cook Off

<div align="center">

🇺🇸 English | [🇰🇷 한국어](./README.ko.md)

</div>

An action side-scrolling shooter with FSM-based AI and core combat systems.  
Developed as a team project — responsible for overall programming.

[![Hollow: Cook Off Trailer](https://img.youtube.com/vi/cNTTR39F9BM/0.jpg)](https://www.youtube.com/watch?v=cNTTR39F9BM)

[▶ Steam Page](https://store.steampowered.com/app/2640380/Hollow_Cook_Off/)

| Item | Details |
|------|------|
| Genre | Action Side-Scrolling Shooter |
| Engine | Unity (C#) |
| Platform | PC (Steam) |
| Release Year | 2024 |

## Key Features

- **FSM-Based Enemy AI**: Designed combat behaviors for regular enemies and multi-phase bosses using a class-encapsulated FSM architecture.
- **Weapon System**: Modularized weapon types and firing logic for easy extensibility and designer-friendly configuration.
- **Player Special Abilities**: Implemented action-oriented combat systems including bullet parry and slash combos.
- **Data-Driven Dialogue System**: Utilized JSON data to load and play dialogues without code changes.

## Code Structure

```
Hollow-CookOff-Codes/
├── Enemy/                    # Enemy units and boss behaviors
├── UI/                       # UI systems
├── Util/                     # General utilities
├── JsonCogi/                 # JSON data management
├── MainCharacter.cs          # Player character controller
├── SpecialAbility.cs         # Player special abilities
├── BulletParry.cs            # Bullet parry system
├── SlashCombo.cs             # Slash combo system
├── ChatDB.cs                 # Dialogue database
├── ChatEventTriger.cs        # Dialogue event trigger
├── SaveDataManager.cs        # Save data management
└── BGMManager.cs             # Audio control
```

## Implementation Details

### 1. FSM-Based Enemy AI
- Each state (`Enter`, `Update`, `Exit`) is managed as an independent class, making maintenance and adding new states straightforward.
- For bosses, implemented phase transition logic based on HP thresholds to provide complex attack patterns.

### 2. Weapon System
- Designed parameters such as firing mode, bullet speed, and damage to be immediately adjustable within the Unity Inspector.
- Uses a `WeaponBox` placement system, allowing designers to arrange weapons in the scene according to their intent.

### 3. Combat Mechanics
- **Bullet Parry**: A defensive-offensive mechanic that parries projectiles with precise timing.
- **Slash Combo**: A system that determines and executes chained attacks based on input timing.
- **Bio-Energy**: A resource system required for using special abilities.
- **Special Ability**: Enables unique special attacks depending on the currently equipped weapon.

### 4. Dialogue System
- Dialogue content is managed via JSON, allowing non-programmers to edit scripts without touching the code.
- `ChatEventTrigger` fires dialogue sequences at specific locations or conditions within a scene.

## License

[MIT License](LICENSE)