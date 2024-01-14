using Content.Client.CryoSleep;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Systems.Ghost.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;
using Robust.Shared.Configuration;
using Content.Shared.CCVar;
using Content.Shared.NF14.CCVar;

namespace Content.Client.UserInterface.Systems.Ghost.Widgets;

[GenerateTypedNameReferences]
public sealed partial class GhostGui : UIWidget
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;

    private TimeSpan? _timeOfDeath;
    private float _minTimeToRespawn;

    public GhostTargetWindow TargetWindow { get; }
    public GhostRespawnRulesWindow RulesWindow { get; }
    public CryosleepWakeupWindow CryosleepWakeupWindow { get; } // Frontier

    public event Action? RequestWarpsPressed;
    public event Action? ReturnToBodyPressed;
    public event Action? GhostRolesPressed;
    public event Action? GhostRespawnPressed;

    public GhostGui()
    {
        RobustXamlLoader.Load(this);

        TargetWindow = new GhostTargetWindow();
        RulesWindow = new GhostRespawnRulesWindow();
        CryosleepWakeupWindow = new CryosleepWakeupWindow();
        RulesWindow.RespawnButton.OnPressed += _ => GhostRespawnPressed?.Invoke();

        MouseFilter = MouseFilterMode.Ignore;

        GhostWarpButton.OnPressed += _ => RequestWarpsPressed?.Invoke();
        ReturnToBodyButton.OnPressed += _ => ReturnToBodyPressed?.Invoke();
        GhostRolesButton.OnPressed += _ => GhostRolesPressed?.Invoke();
        GhostRespawnButton.OnPressed += _ => RulesWindow.OpenCentered();
        CryosleepReturnButton.OnPressed += _ => CryosleepWakeupWindow.OpenCentered(); // Frontier
    }

    public void Hide()
    {
        TargetWindow.Close();
        Visible = false;
    }

    public void UpdateRespawn(TimeSpan? todd)
    {
        if (todd != null)
        {
            _timeOfDeath = todd;
            _minTimeToRespawn = _configurationManager.GetCVar(NF14CVars.RespawnTime);
        }
    }

    public void Update(int? roles, bool? canReturnToBody, TimeSpan? timeOfDeath, float minTimeToRespawn, bool canUncryo)
    {
        ReturnToBodyButton.Disabled = !canReturnToBody ?? true;
        _timeOfDeath = timeOfDeath;
        _minTimeToRespawn = minTimeToRespawn;

        if (roles != null)
        {
            GhostRolesButton.Text = Loc.GetString("ghost-gui-ghost-roles-button", ("count", roles));
            if (roles > 0)
            {
                GhostRolesButton.StyleClasses.Add(StyleBase.ButtonCaution);
            }
            else
            {
                GhostRolesButton.StyleClasses.Remove(StyleBase.ButtonCaution);
            }
        }

        TargetWindow.Populate();

        CryosleepReturnButton.Disabled = !canUncryo;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        if (_timeOfDeath is null)
        {
            GhostRespawnButton.Text = Loc.GetString("ghost-gui-respawn-button-denied", ("time", "disabled"));
            GhostRespawnButton.Disabled = true;
            return;
        }

        var delta = (_minTimeToRespawn - _gameTiming.CurTime.Subtract(_timeOfDeath.Value).TotalSeconds);
        if (delta <= 0)
        {
            GhostRespawnButton.Text = Loc.GetString("ghost-gui-respawn-button-allowed");
            GhostRespawnButton.Disabled = false;
        }
        else
        {
            GhostRespawnButton.Text = Loc.GetString("ghost-gui-respawn-button-denied", ("time", $"{delta:f1}"));
            GhostRespawnButton.Disabled = true;
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            TargetWindow.Dispose();
        }
    }
}
