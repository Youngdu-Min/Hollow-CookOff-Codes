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
| Engine | Unity (C#), Corgi Engine |
| Platform | PC (Steam) |
| Release Year | 2024 |

## Key Features

- **Enemy AI**: Extended Corgi Engine's AIBrain (state = actions + transition decisions) with custom actions and decisions to build combat behaviors for regular enemies and multi-phase bosses.
- **Boss Phases**: When a health segment is depleted the boss is stunned, and the player must break the block with a knife slash to advance to the next phase.
- **Weapon System**: Modularized weapon types and firing logic for easy extensibility and designer-friendly configuration.
- **Player Special Abilities**: Implemented action-oriented combat systems including knife dash, melee and bullet parry, slash combos, grappling hook and time stop.
- **Data-Driven Dialogue System**: Managed dialogue content as ScriptableObject assets so it can be edited and played without code changes, with Korean/English switching.
- **Balance Data Integration**: Weapon and enemy values are loaded from Google Sheets, so they can be tuned without a new build.

## Code Structure

```
Hollow-CookOff-Codes/
├── Enemy/                    # Enemy units and boss behaviors
├── UI/                       # UI systems
├── Util/                     # General utilities
├── JsonCogi/                 # Combat-zone camera lock, score tracking
├── MainCharacter.cs          # Player character controller
├── SpecialAbility.cs         # Player special abilities
├── BulletParry.cs            # Bullet parry system
├── SlashCombo.cs             # Slash combo system
├── ChatDB.cs                 # Dialogue database
├── ChatEventTrigger.cs       # Dialogue event trigger
├── SaveDataManager.cs        # Save data management
└── BGMManager.cs             # Audio control
```

## Implementation Details

### 1. Enemy AI
- Corgi Engine's AIBrain builds each state from `AIAction` (behavior) and `AIDecision` (transition condition) components. On top of it I added custom actions and decisions such as boss pattern rotation (no immediate repeats), per-phase object activation and an armor-support enemy.
- When a boss depletes a health segment, its health is clamped at the segment boundary and it is stunned (blocked). Breaking the block with a knife slash within 10 seconds advances the phase; otherwise the boss recovers part of its health.
- Armored enemies absorb damage with armor first and switch behavior patterns the moment the armor breaks.

### 2. Weapon System
- Designed parameters such as firing mode, bullet speed, and damage to be immediately adjustable within the Unity Inspector.
- Uses a `WeaponBox` placement system, allowing designers to arrange weapons in the scene according to their intent.

### 3. Combat Mechanics
- **Knife Dash**: Dashes in the aim direction up to the nearest wall while slashing. Closer hits cause stronger knockback, and holding the button launches enemies upward.
- **Bullet Parry**: A melee parry that stuns enemies when the knife lands during their attack wind-up, and a bullet parry that slows time and reflects the projectile with its original look.
- **Slash Combo**: A system that determines and executes chained attacks based on input timing.
- **Bio-Energy**: A resource system required for using special abilities.
- **Special Ability**: Enables unique special attacks depending on the currently equipped weapon.
- **Grappling Hook**: Swing from terrain or pull enemies toward the player.
- **Time Stop**: Freezes characters, projectiles and moving platforms inside an area for a set time.

### 4. Dialogue System
- Dialogue content is managed as ScriptableObject assets (`DialogueDB_Kor`, `DialogueDB_Eng`), allowing non-programmers to edit scripts without touching the code.
- `ChatEventTrigger` fires dialogue sequences at specific locations or conditions within a scene.
- Switching `LanguageData` swaps both the dialogue database and the font for Korean/English.

### 5. Balance Data
- Weapons, sub-weapons, player actions and enemy health/armor are managed in Google Sheets and loaded with UnityGoogleSheet.
- Designers can tune values in the sheet without rebuilding.

## Technical Document

Internal design details (enemy AI extensions, boss health segments and slash phases, knife / parry / grapple combat, dialogue system) are documented in the [technical document](docs/TechnicalDesign.md) (Korean).

## License

[MIT License](LICENSE)