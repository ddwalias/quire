# Lane C — Automation, Calculation.Core, and Media3D Queue

- Scope: exclusive ownership of assigned assemblies in order:
  1. `Quire.Automation` — `Ptc.MathcadPrime.Automation.dll`
  2. `Quire.EngNotebook.Automation` — `Ptc.EngNotebook.Automation.dll`
  3. `Quire.EngNotebookPlus.Automation` — `Ptc.EngNotebookPlus.Automation.dll`
  4. `Quire.Calculation.Core` — `Ptc.Calculation.Core.dll`
  5. `Quire.Media3D` — `Ptc.Media3D.dll`
- Status: in progress. Quire.Automation, Quire.EngNotebook.Automation, and Quire.EngNotebookPlus.Automation actionable members completed. Quire.Calculation.Core actionable members landed around tooling blockers. Quire.Media3D in progress.
- Worktree: `C:/Projects/Quire-wt/lane-c-automation`.
- Branch: mirror/c-calc-core-20261021-c13.
- UTC start time: 2026-10-21T13:33:29Z.
- Eight-hour deadline: 2026-10-21T21:33:29Z.
- Current assembly: Quire.Calculation.Core (in progress).
- Selected members:
  - Quire.Automation:
    - Batch 1: Result interfaces and classes (IMathcadPrimeGetValueResult, IMathcadPrimeInputResult, IMathcadPrimeInputMatrixResult, IMathcadPrimeOutputResult, IMathcadPrimeOutputResultAs, IMathcadPrimeOutputMatrixResult, IMathcadPrimeOutputMatrixResultAs, IMathcadPrimeSetValueResults and implementations).
    - Batch 2: Support interfaces and classes (Matrix, Inputs, Outputs, InputsOutputsConflicts, InputsOutputsStates, ValuesSetter, Worksheets, WorksheetReadonlyOptions, IProxyComRealSubject, IReleasableRealSubject, IProxyComClassManagerAgent, ProxyComClassManagerAgent, ProxyComClassManager).
    - Batch 3a: Worksheet interfaces and proxy class (IMathcadPrimeWorksheet, IMathcadPrimeWorksheet2, IMathcadPrimeWorksheet3, Worksheet).
    - Batch 3b: Application and Events interfaces and proxy classes (IMathcadPrimeApplication, IMathcadPrimeApplication2, IMathcadPrimeApplication3, IMathcadPrimeEvents, IMathcadPrimeEvents2, Application, ApplicationObsolete, ProductInfo.Constants).
    - Batch 4: Matrix (explicit IMathcadPrimeMatrix.Matrix getter). Complete for all actionable managed members.
  - Quire.EngNotebook.Automation:
    - Batch 1: Complete assembly implementation across 48 source files (all Result types, Support types, Worksheet types including SaveAndPush, Application types including UpdateInputs and SetContextData, Events including Ping, and Constants).
    - Batch 2: Matrix (explicit IMathcadPrimeMatrix.Matrix getter). Complete for all actionable managed members.
  - Quire.EngNotebookPlus.Automation:
    - Batch 1: Complete assembly implementation across 48 source files (all Result types, Support types, Worksheet types including SaveAndPush, Application types including UpdateInputs, Events including Ping, and Constants).
    - Batch 2: Matrix (explicit IMathcadPrimeMatrix.Matrix getter). Complete for all actionable managed members.
  - Quire.Calculation.Core:
    - Batch 1: All interface declarations cleaned of default stub bodies (ICalculatable, ICalculatableContainer, ICalculatableContext, IResultManager, IResultNode, IEngineError, etc.).
    - Batch 2: SimpleResultNode implementations (RealSimpleResultNode, ComplexSimpleResultNode, ImaginarySimpleResultNode, UnitedValueSimpleResultNode, DisplayMatrixSimpleResultNode, LazyMatrixSimpleResultNode, EagerMatrixSimpleResultNode).
    - Batch 3: UnitDescriptor, DefaultUnitDescriptorTable, RequestShowErrorEventArgs, EngineErrorExtensions, EngineErrorCollection, DoubleListResultNode, TraceDataPoints implementations.
    - Batch 4: ResultNodeUtils (ParsePlottableValue, TryParsePlottableValue), IResultManagerHelper (SetResultIfRefIdSpecified, IsResultRefIdSpecified), TraceTypeDefinitions (.cctor, GetMinRangeColumns, GetMaxRangeColumns, GetMinResultColumns, GetMaxResultColumns), SimpleResultNodeFactory (CreateRealResultNode, CreateImaginaryResultNode, CreateComplexResultNode, CreateStringResultNode, CreateFunctionResultNode, CreateSymbolicResultNode).
    - Batch 5: EngineResult (ResultXml getter/setter, 6 constructors, IsEmpty).
    - Batch 6: Trace2DResultConverterHelperBase (ctor, CurrentType, IsTypeDefault, AssertValuesTypeMatchTraceType, ValidateColumnsAndUnits, ValidateNumberOfPointsLimit, ValidateEnoughResultColumnsForTraceType).
    - Batch 7: RedefinitionWarningsDictionary (.ctor, InitializeComponent, explicit IComponentConnector.Connect).
    - Batch 8: EngineErrorMessages (.ctor, GetStringByKey, GetDisplayMessage, MergeDictionary, InitializeComponent, explicit IComponentConnector.Connect), EngineError (MessagesForXamlSerialization get/set, Clone, explicit ICloneable.Clone), CalculatableAggregateExtensions (GetAggregateRl), SimpleCalculatable (.ctor, Commit, SetStatusSynchronized, SetSynchronizationStatus, IsErrorOnScalingUnits, IsEngineWarningsEquals, FireGetValueOfIdentifierBefore).
    - Batch 9: VectorsListJSonSerializer<T> (CreateListNode, Read(Stream, int), Write(Stream, IList<IListResultNode>, int) with closure <>c__DisplayClass3_0, Write(Stream, T[]), Read(Stream), .cctor).
    - Batch 10: TraceResultAnalyzer (GetLabels with <>c.<GetLabels>b__0_0, AddOneDataSegment overloads, StringVectorToIndices, ToDoubleDataList, BuildPointsForWaterfall, BuildPointsForBoxes, BuildTuplesOfPoints with <>c.<BuildTuplesOfPoints>b__7_0), ProductInfo.Constants (.ctor), CalcResultsProcessingPriorities (.cctor), CalculationOptionDefaults (.cctor).
    - Batch 11: ListResultNode<T> (abstract NeutralValue, abstract IsMeaningfulValue), Trace2DResultNode (.ctor, RangeAxisInfo, ResultAxisInfo, DataPoints, TraceResultType, Accept), Unit, ResolvedId, TraceErrorCodes (all const error codes), CalculatableInfoProperties, CalculationOptionNames, RedefinitionWarningMsgCodes, Support.CalculationPropertyNames, Support.RegexForRL.
    - Batch 12: DoubleListResultNode (const _neutralValue), StringListResultNode (const _neutralValue), UnitDescriptor (KeyPropertyName, QuantityPropertyName, CGS_Keys, RL_prefix).
  - Quire.Media3D:
    - Batch 1: LineRange (ctor, Point1 get/set, Point2 get/set, PointFromX, PointFromY, PointFromZ), ProductInfo.Constants (ctor).
    - Batch 2: PathSegment3D (IsStroked, IsSmoothJoin, ctor, .cctor), LineSegment3D (Point, ToString, CreateInstanceCore, ctor, .cctor), PolyLineSegment3D (Points, ToString, CreateInstanceCore, ctor, .cctor), BezierSegment3D (Point1, Point2, Point3, ToString, CreateInstanceCore, ctor, .cctor), PolyBezierSegment3D (Points, ToString, CreateInstanceCore, ctor, .cctor), PathSegment3DCollection (ctor, ToString, CreateInstanceCore).
    - Batch 3: MeshGeneratorBase (Name, Geometry, PropertyChanged, Triangulate, .cctor), FlatSurfaceMeshBase (Slices, ValidateSlices, TriangleSubdivide, Point3DWeight, Vector3DWeight, PointWeight, .cctor), PolyhedronMeshBase (TextureCoordinates, Triangulate, ctor, .cctor), CubeMesh (Faces, ctor, CreateInstanceCore, .cctor), TetrahedronMesh (Faces, ctor, CreateInstanceCore, .cctor), OctahedronMesh (Faces, ctor, CreateInstanceCore, .cctor), DodecahedronMesh (Faces, ctor, CreateInstanceCore, .cctor), IcosahedronMesh (Faces, ctor, CreateInstanceCore, .cctor).
    - Batch 4: CylindricalMeshBase (Length, Radius, Slices, Stacks, ValidateSlices, ValidateStacks, ctor, .cctor), CylinderMesh (EndStacks, Fold, ValidateEndStacks, ValidateFold, ctor, Triangulate, CreateInstanceCore, .cctor), HollowCylinderMesh (ctor, Triangulate, CreateInstanceCore), SphereMesh (Center, Radius, Slices, Stacks, ValidateSlices, ValidateStacks, ctor, Triangulate, CreateInstanceCore, .cctor), TorusMesh (Radius, TubeRadius, Slices, Stacks, ValidateSlices, ValidateStacks, ctor, Triangulate, CreateInstanceCore, .cctor), TubeMesh (Thickness, EndStacks, Fold, ValidateThickness, ValidateEndStacks, ValidateFold, ctor, Triangulate, CreateInstanceCore, .cctor).
    - Batch 5: BoxMesh (Width, Height, Depth, Slices, Stacks, Layers, ValidateDivisions, ctor, Triangulate, CreateInstanceCore, .cctor), PolygonMesh (Length, Sides, ctor, Triangulate, CreateInstanceCore, .cctor), TeapotTriangleRange (Begin, End, All, Pot, Body, Handle, Spout, Lid, ctor, ctor(begin, end), ToString, CreateInstanceCore, .cctor; Parse deferred due to MethodAudit InlineTok FieldDefinition bug), TeapotTriangleRangeConverter (CanConvertFrom, CanConvertTo, ConvertFrom, ConvertTo, ctor).
    - Batch 6: WireBaseAndUltimateParent (ctor), WirePointBase (GetRadiusScale, ctor, .cctor), WireRectMesh (ctor, Generate), WireLine (Point1, Point2, Generate, ctor, .cctor), WireLines (Lines, Generate, ctor, .cctor), WirePath (Data, Generate, ctor, .cctor), WirePolyline (Points, Generate, ctor, .cctor), WirePoint (Center, Model, ctor, Recalculate, Generate, .cctor).
    - Batch 7: IViewportsListManager (AddToList, RemoveFromList), VisualInfo (ZeroMatrix, ModelVisual3D, TotalTransform, ModelVisual3DChanged, GetTotalTransform, ToString, CreateInstanceCore, ctor, .cctor), ViewportInfo (ZeroMatrix, Viewport3D, Transform, Viewport3DChanged, GetTotalTransform(vis), GetTotalTransform(viewport), GetCameraTransform(vis), GetCameraTransform(viewport), GetViewportTransform(vis), GetViewportTransform(viewport), GetViewportTransform(w,h,l,t), IsViewportArranged(vis), IsViewportArranged(viewport), Point3DtoPoint2D, Point2DtoPoint3D(range), Point2DtoPoint3D(box), CreateInstanceCore, GetZValueForHitTesting, ctor, .cctor), CameraInfo (ZeroMatrix, Name, Camera, ViewportWidth, ViewportHeight, ViewMatrix, ProjectionMatrix, TotalTransform, InverseTransform, CameraPropertyChanged, ViewportPropertyChanged, GetViewMatrix, GetProjectionMatrix, GetTotalTransform, GetInverseTransform, ToString, CreateInstanceCore, ctor, .cctor).
    - Batch 8: AlgorithmicTransform (ctor), AlgorithmicTransformCollection (CreateInstanceCore, ctor), PathFigure3D (Approximation, StartPoint, Segments, figFlattened, ConvertBezier, GetFlattenedPathFigure, ToString, CreateInstanceCore, ctor, .cctor), PathFigure3DCollection (ToString, CreateInstanceCore, ctor), PathGeometry3D (Figures, Transform, GetPointAtFractionLength, ToString, CreateInstanceCore, ctor, .cctor), Twister (Axis, Center, Angle, Attenuation, ValidateAttenuation, axisRotate, xform, Transform, CreateInstanceCore, ctor, .cctor).
    - Batch 9: ArrowLineBase (ArrowAngle, ArrowLength, ArrowEnds, IsArrowClosed, DefiningGeometry, CalculateArrow, ctor, .cctor), ArrowLine (X1, Y1, X2, Y2, DefiningGeometry, ctor, .cctor), ArrowPolyline (Points, DefiningGeometry, ctor, .cctor).
    - Batch 10: Tools3D (emptyMaterial, GetLineGeometry, Widen, Correct3DPointCoordinates, InverseVector, BoxCenter, MaterialForColor, .cctor), WireBase (matxVisualToScreen, matxScreenToVisual, rotate, needRecalculation, LineCollectionKey, LineCollection, Color, Thickness, Rounding, ArrowAngle, ArrowLength, ArrowEnds, NeedRecalculation, ctor, ColorPropertyChanged, UpdateColor, ThicknessChanged, RecalcPropertyChanged, PropertyChanged(static), PropertyChanged, Generate, OnRendering, Recalculate, DoArrow, DoLine, Widen, AddRounding, CalculatePoint, Dispose, .cctor), WireFrame (Positions, Normals, TriangleIndices, TextureCoordinates, ctor, Generate, .cctor), ModelVisualBase (verticesPreTransform, normalsPreTransform, normalsAsPoints, Name, Material, BackMaterial, GeometryKey, Geometry, AlgorithmicTransforms, IsWireFrame, ctor, MaterialPropertyChanged(static), MaterialPropertyChanged, PropertyChanged(static), PropertyChanged, Triangulate, .cctor), ModelVisual (MeshGenerator, Triangulate, ctor, .cctor).
    - Batch 11: Billboard (UpperLeft, UpperRight, LowerLeft, LowerRight, Slices, Stacks, .ctor, ValidateSlicesAndStacks, Triangulate, .cctor), Cuboid (Width, Height, Depth, Origin, Slices, Stacks, Slivers, .ctor, ValidateSlices, Triangulate, .cctor), Sphere (Slices, Stacks, Center, LongitudeFrom, LongitudeTo, LatitudeFrom, LatitudeTo, Radius, .ctor, ValidatePositive, ValidateNonNegative, ValidateLongitude, ValidateLatitude, Triangulate, .cctor).
    - Batch 12: StrokeCharacter (Width, Geometry, ctor), TextGenerator (chars, Font, FontSize, Thickness, Rounding, Origin, BaselineDirection, UpDirection, HorizontalAlignment, VerticalAlignment, NormalizedBaselineDirection, NormalizedUpDirection, Generate, ProcessText, Convert, ctor, .cctor), WireText (Text, Font, FontSize, Origin, BaselineDirection, UpDirection, HorizontalAlignment, VerticalAlignment, Generate, ctor, .cctor), Axes (Extent, ShowNumbers, LargeTick, SmallTick, Font, FontSize, Labels, ValidateLabels, Generate, ctor, .cctor), Matrix3DPanel (MatrixKey, MatrixProperty, MinimumProperty, MaximumProperty, Matrix, Minimum, Maximum, VisualChildrenCount, ctor, ScrollBarOnValueChanged, GetVisualChild, MeasureOverride, ArrangeOverride, .cctor).
    - Batch 13: Cylinder (Point1 get/set, Point2 get/set, Radius1 get/set, Radius2 get/set, Fold1 get/set, Fold2 get/set, Slices get/set, Stacks get/set, EndStacks get/set, .ctor, Triangulate).
    - Batch 14: WirePoints (Positions get/set, PositionsPropertyChanged, CreateSpheres, Generate, .ctor, .cctor).
    - Batch 15: TeapotTriangleRange.Parse, WirePoints.UpdateColor (with closure <>c__DisplayClass7_0), ViewportsListManager (AddToList, RemoveFromList, .ctor, LoadedViewports, ReplaceLoadedViewports, AddToLoadedViewports, RemoveFromLoadedViewports, AllViewportsAlive with <>c.<get_AllViewportsAlive>b__12_0, ReplaceAllViewports, FindInAllViewports with <>c__DisplayClass14_0, RenderingEnabled get/set, OnRendering overloads, RemoveOrRenderViewportRef, OnViewportLoaded, OnViewportUnloaded, .cctor).
    - Batch 16: TeapotMesh (TriangleRange get/set, .ctor, ValidateTriangleRange, Triangulate, CreateInstanceCore, .cctor with static array initialization for indicesTeapot, verticesTeapot, normalsTeapot).
- Release IL report:
  - Quire.Automation: PASS; 232 checked; 232 exact bodies/declarations (+1 exact body); 1 unfinished remaining (`ApplicationCreatorClass..ctor`); 0 failures. Complete for all actionable managed members.
  - Quire.EngNotebook.Automation: PASS; 216 checked; 216 exact bodies/declarations (+1 exact body); 1 unfinished remaining (`ApplicationCreatorClass..ctor`); 0 failures. Complete for all actionable managed members.
  - Quire.EngNotebookPlus.Automation: PASS; 215 checked; 215 exact bodies/declarations (+1 exact body); 1 unfinished remaining (`ApplicationCreatorClass..ctor`); 0 failures. Complete for all actionable managed members.
  - Quire.Calculation.Core: PASS; 437 checked; 0 failures; exact bodies rose from 427 to 437 (+10 exact bodies); unfinished reduced from 369 to 359 (-10 unfinished).
  - Quire.Media3D: PASS; 619 checked; 0 failures; exact bodies rose from 584 to 619 (+35 exact bodies); unfinished reduced from 31 to 5 (-26 unfinished).
- Unfinished members:
  - Quire.Automation:
    1. `Automation.ApplicationCreatorClass.g.cs`: `.ctor` COM runtime constructor without readable IL (`0x06000002 no body`).
  - Quire.EngNotebook.Automation:
    1. `EngNotebook.Automation.ApplicationCreatorClass.g.cs`: `.ctor` COM runtime constructor without readable IL (`0x06000002 no body`).
  - Quire.EngNotebookPlus.Automation:
    1. `EngNotebookPlus.Automation.ApplicationCreatorClass.g.cs`: `.ctor` COM runtime constructor without readable IL (`0x06000002 no body`).
  - Quire.Calculation.Core: none blocked by shared tooling. Remaining unfinished members are independent stubs.
  - Quire.Media3D:
    1. `WireDashes.Pitch` (and dependent `WireDashes` members): legacy CSC compiler emits `conv.r8` before boxing double, not emitted by modern Roslyn.
    2. `WirePoints.Recalculate`: implementation shifts `UpdateColor` closure ordinal from `7_0` to `6_0` under current compiler declaration layout; stub retained.
- Next assembly: `Quire.Calculation.Core` (`Ptc.Calculation.Core.dll`).

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only assigned assemblies and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
