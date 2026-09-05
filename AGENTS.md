# Repository Guidelines

## Project Structure & Module Organization
This repository is a Unity project (2022.3 LTS) centered on EX-GAS 2.0.

- `Assets/GAS/Runtime`: core gameplay ability system runtime (ECS/DOTS, tags, effects, abilities).
- `Assets/GAS/Editor`: custom tools (GAS Center, timeline editor, web editors).
- `Assets/GAS/General`: shared utilities/constants used by runtime and editor code.
- `Assets/DemoForESC`, `Assets/EXUI`, `Assets/_EXProceduralMachine`: demo/gameplay integration content.
- `EX_GAS_Config/ProjectConfigTable/exgas_config`: Luban Excel-to-JSON config source (`Datas/`, `gen.bat`, `gen.sh`).
- `Packages/`, `ProjectSettings/`: Unity package and project config.

Do not commit generated caches/build folders like `Library/`, `Temp/`, `Logs/`, or `obj/`.

## Build, Test, and Development Commands
- Open project with Unity `2022.3.16f1` (see `ProjectSettings/ProjectVersion.txt`).
- Regenerate config JSON from Excel:
  - Windows: `EX_GAS_Config\ProjectConfigTable\exgas_config\gen.bat`
  - macOS/Linux: `bash EX_GAS_Config/ProjectConfigTable/exgas_config/gen.sh`
- Run EditMode tests (batchmode):
  - `Unity.exe -batchmode -quit -projectPath . -runTests -testPlatform EditMode -testResults TestResults/EditMode.xml`
- Run PlayMode tests (batchmode):
  - `Unity.exe -batchmode -quit -projectPath . -runTests -testPlatform PlayMode -testResults TestResults/PlayMode.xml`

## Coding Style & Naming Conventions
- Language: C# with 4-space indentation, braces on new lines (match existing files).
- Keep namespaces consistent with folder intent (for GAS code, typically `GAS.Runtime`, `GAS.Editor`, `GAS.General`).
- Type naming follows project patterns:
  - `C*` for ECS components, `B*` for buffer elements, `S*` for systems, `Conf*` for config loaders.
- Preserve Unity `.meta` files for any moved/added assets.

## Testing Guidelines
- Primary framework: Unity Test Framework (`com.unity.test-framework`).
- Keep project-specific tests under `Assets/_Test` or dedicated test asmdefs; avoid mixing with plugin vendor tests.
- Name test classes by behavior and suffix with `Tests` where possible (for example, `GameplayCueTests`).

## Commit & Pull Request Guidelines
- Recent history shows short, task-focused messages in both Chinese and English (for example: `docs: ...`, `Update ...`, `修复...`).
- Prefer format: `<scope>: <imperative summary>` (examples: `runtime: fix effect disposal`, `docs: update GAS workflow`).
- PRs should include:
  - What changed and why.
  - Affected modules/paths (for example `Assets/GAS/Runtime/Effect`).
  - Validation evidence (test run, editor verification, or screenshots for tooling/UI changes).

## Project Memory (项目记忆)
- **Read `PROJECT_MEMORY.md` first** before making changes: it is the on-boarding memory for AI agents covering the ECS system topology, ASC/Spec facade pattern, ECS naming conventions (C*/B*/S*/MC*/Conf*), the Excel → Luban → JSON → codegen config pipeline, the Bean mapping spec (`BeanMappingSpec.md`), editor tooling (GASCenter / WebEditor / GASWatcher), known issues, and working conventions.
- Authoritative docs: `README.md` (system design & config tables), `BeanMappingSpec.md` (Bean mapping), `DemoFrameworkIntroduction.md` (Demo framework). Keep `PROJECT_MEMORY.md` in sync when the architecture or conventions change.
