#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class Presenter<ModelType, ViewType>
{
    [MirrorVerified("PtcPresenter2MechanicalDifferentialTests")]
    public Presenter() { }

    [MirrorVerified("PtcPresenter2MechanicalDifferentialTests")]
    private ViewType _view;

    [MirrorVerified("PtcPresenter2MechanicalDifferentialTests")]
    public virtual ViewType View { get => _view; set => _view = value; }

    [MirrorVerified("PtcPresenter2MechanicalDifferentialTests")]
    private ModelType _model;

    [MirrorVerified("PtcPresenter2MechanicalDifferentialTests")]
    public virtual ModelType Model { get => _model; set => _model = value; }
}
