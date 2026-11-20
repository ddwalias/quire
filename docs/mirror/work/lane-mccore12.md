# Quire.Mccore12 checkpoint

- Owner: AGY, Orca task `task_c71af2c74bba`.
- Branch: `feature/mccore12-agy`.
- Target: `src/Quire.Mccore12` (C++/CLI under `MixedMode/`, MSVC 19.51.36246, D19/D20).

## Landed Commits

- Pending Batch 1: Established original-language C++/CLI project split for `Quire.Mccore12` (`Quire.Mccore12.csproj` and `MixedMode/Quire.Mccore12.vcxproj`); mirrored `msclr._detail_class`, `msclr.lock`, and `Engine.DetectAlgos`.

## Release IL Report & Gates

- Batch 1: PASS, 106 checked, 104 exact, 0 failures, 289 Mathcad `<Module>` / unmanaged members not paired (`.mirror-cache/release-il-60e17d4869bc/release-il.json`).

## Remaining Members and Actionable Blockers

All remaining members in `mccore12.dll` are concretely blocked on native libraries (`engutils.dll`, `mcaduser.dll`, `mctranslator.dll`, `mshex.dll`, `msmpl.dll`, `utl.dll`) and dependent native engine types:
1. Native Project Dependencies:
   - `Quire.Native.Mctranslator` (`mctranslator.dll`): Not yet implemented / created (Wave 7). Provides translation facilities for `ArlTranslate` and `Engine.TranslateHelper`.
   - `Quire.Native.Msmpl` (`msmpl.dll`): Not yet implemented / created (Wave 4). Provides compiler/evaluator symbols for `MplCompile` and `Engine.ModuleEnv`.
   - Native unmanaged types: `Engine::RedefMap`, `Engine::AlgorithmMap`, `Engine::LabelMap`, `Engine::ModuleData`, `Engine::ModuleEnv`, `Hex::Names`, `Hex::Interpreter`, `Hex::Environment`, `Hex::Module`, `Hex::Heap`, `Hex::Signature`, `Hex::Value`, `Hex::Error`, `Attachment`, `utl::ReadWriteLock`, `utl::TimeIt`, `Engine::options`.
2. Unfinished Managed Engine Types:
   - `Engine.MCCore12`: Core singleton. Exposes `m_factory` of type `CoreFactory`, and calls unmanaged `Engine::ModuleEnv::LoadMplModuleRec`.
   - `Engine.CoreFactory`: Instantiates native `Hex::Names`, `Engine::Unmanaged::label_root`, `Engine::ModuleEnv`, `Hex::Interpreter`.
   - `Engine.DetectRedefs`: Exposes `FromUnmanaged(RedefMap*)` and creates `ResolveLabels`.
   - `Engine.ResolveLabels`: Depends on `add_labels` and `LabelTrie`.
   - `Engine.ArlTranslate`: Directly creates unmanaged `Hex::Module`, `Hex::Heap`, invokes `Hex::Interpreter`.
   - `Engine.MplCompile`: Compiles MPL AST into assembly, depending on native `ModuleEnv` and `HexEnv`.
   - `<Module>`: 289 unmanaged C++ functions and compiler CRT helpers will resolve automatically when native engine headers and compiler dependencies are integrated.
