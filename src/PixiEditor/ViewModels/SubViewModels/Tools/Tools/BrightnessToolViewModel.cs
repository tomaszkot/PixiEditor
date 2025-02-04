﻿using System.Windows.Input;
using ChunkyImageLib.DataHolders;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.Models.Commands.Attributes.Commands;
using PixiEditor.Models.Enums;
using PixiEditor.ViewModels.SubViewModels.Tools.ToolSettings.Settings;
using PixiEditor.ViewModels.SubViewModels.Tools.ToolSettings.Toolbars;
using PixiEditor.Views.UserControls.Overlays.BrushShapeOverlay;

namespace PixiEditor.ViewModels.SubViewModels.Tools.Tools;

[Command.Tool(Key = Key.U)]
internal class BrightnessToolViewModel : ToolViewModel
{
    private readonly string defaultActionDisplay = "Draw on pixels to make them brighter. Hold Ctrl to darken.";

    public BrightnessToolViewModel()
    {
        ActionDisplay = defaultActionDisplay;
        Toolbar = ToolbarFactory.Create<BrightnessToolViewModel, BasicToolbar>();
    }

    public override string Tooltip => $"Makes pixels brighter or darker ({Shortcut}). Hold Ctrl to make pixels darker.";

    public override BrushShape BrushShape => BrushShape.Circle;

    [Settings.Inherited]
    public int ToolSize => GetValue<int>();
    
    [Settings.Float("Strength", 5)]
    public float CorrectionFactor => GetValue<float>();

    [Settings.Enum("Mode")]
    public BrightnessMode BrightnessMode => GetValue<BrightnessMode>();
    
    public bool Darken { get; private set; } = false;

    public override void UpdateActionDisplay(bool ctrlIsDown, bool shiftIsDown, bool altIsDown)
    {
        if (!ctrlIsDown)
        {
            ActionDisplay = defaultActionDisplay;
            Darken = false;
        }
        else
        {
            ActionDisplay = "Draw on pixels to make them darker. Release Ctrl to brighten.";
            Darken = true;
        }
    }

    public override void OnLeftMouseButtonDown(VecD pos)
    {
        ViewModelMain.Current?.DocumentManagerSubViewModel.ActiveDocument?.Tools.UseBrightnessTool();
    }
}
