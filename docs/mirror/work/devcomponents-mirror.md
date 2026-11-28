# DevComponents.WpfRibbon / WpfDock mirror

Owner: devcomponents-mirror worker. Branch: `feature/devcomponents-mirror-20261128`.

Batch 1 (MemberGen, D33): Dock 242 mechanical members, Ribbon 559. Release IL PASS
(Dock 350 checked, Ribbon 782 checked, 0 failures). MemberGen now resolves `source-layout.json`,
treats both projects as assigned assemblies, and leaves explicit-interface-implementation
stubs and incomplete types to manual work.

Remaining: Dock ~233 stubs, Ribbon ~500 stubs (Ribbon, ButtonDropDown, NavigationPane,
RibbonBackgroundChrome, WinApi, RibbonWindow, ...); XAML InitializeComponent/Connect pending.
Consumer switch (Quire, Controls*, Plots) from install references to ProjectReference pending.
