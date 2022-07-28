﻿using ChunkyImageLib.DataHolders;
using PixiEditor.ChangeableDocument.Actions;
using PixiEditor.Models.Enums;
using PixiEditor.ViewModels.SubViewModels.Document;
using PixiEditor.ViewModels.SubViewModels.Tools.Tools;
using PixiEditor.ViewModels.SubViewModels.Tools.ToolSettings.Toolbars;

namespace PixiEditor.Models.DocumentModels.UpdateableChangeExecutors;
#nullable enable
internal class PenToolExecutor : UpdateableChangeExecutor
{
    private Guid guidValue;
    private SKColor color;
    private int toolSize;
    private bool drawOnMask;
    private bool pixelPerfect;

    public override ExecutionState Start()
    {
        ViewModelMain? vm = ViewModelMain.Current;
        StructureMemberViewModel? member = document!.SelectedStructureMember;
        PenToolViewModel? penTool = (PenToolViewModel?)(vm?.ToolsSubViewModel.GetTool<PenToolViewModel>());
        PenToolbar? toolbar = penTool?.Toolbar as PenToolbar;
        if (vm is null || penTool is null || member is null || toolbar is null)
            return ExecutionState.Error;
        drawOnMask = member is LayerViewModel layer ? layer.ShouldDrawOnMask : true;
        if (drawOnMask && !member.HasMaskBindable)
            return ExecutionState.Error;
        if (!drawOnMask && member is not LayerViewModel)
            return ExecutionState.Error;

        guidValue = member.GuidValue;
        color = vm.ColorsSubViewModel.PrimaryColor;
        toolSize = toolbar.ToolSize;
        pixelPerfect = toolbar.PixelPerfectEnabled;

        vm.ColorsSubViewModel.AddSwatch(color);
        IAction? action = pixelPerfect switch
        {
            false => new LineBasedPen_Action(guidValue, color, controller!.LastPixelPosition, toolSize, false, drawOnMask),
            true => new PixelPerfectPen_Action(guidValue, controller!.LastPixelPosition, color, drawOnMask)
        };
        helpers!.ActionAccumulator.AddActions(action);

        return ExecutionState.Success;
    }

    public override void OnPixelPositionChange(VecI pos)
    {
        IAction? action = pixelPerfect switch
        {
            false => new LineBasedPen_Action(guidValue, color, pos, toolSize, false, drawOnMask),
            true => new PixelPerfectPen_Action(guidValue, pos, color, drawOnMask)
        };
        helpers!.ActionAccumulator.AddActions(action);
    }

    public override void OnLeftMouseButtonUp()
    {
        IAction? action = pixelPerfect switch
        {
            false => new EndLineBasedPen_Action(),
            true => new EndPixelPerfectPen_Action()
        };

        helpers!.ActionAccumulator.AddFinishedActions(action);
        onEnded?.Invoke(this);
    }

    public override void ForceStop()
    {
        IAction? action = pixelPerfect switch
        {
            false => new EndLineBasedPen_Action(),
            true => new EndPixelPerfectPen_Action()
        };
        helpers!.ActionAccumulator.AddFinishedActions(action);
    }
}
