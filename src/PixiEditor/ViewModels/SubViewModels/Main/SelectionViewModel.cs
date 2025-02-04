﻿using System.Windows.Input;
using PixiEditor.ChangeableDocument.Enums;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.Models.Commands.Attributes.Commands;

namespace PixiEditor.ViewModels.SubViewModels.Main;

[Command.Group("PixiEditor.Selection", "Selection")]
internal class SelectionViewModel : SubViewModel<ViewModelMain>
{
    public SelectionViewModel(ViewModelMain owner)
        : base(owner)
    {
    }

    [Command.Basic("PixiEditor.Selection.SelectAll", "Select all", "Select everything", CanExecute = "PixiEditor.HasDocument", Key = Key.A, Modifiers = ModifierKeys.Control)]
    public void SelectAll()
    {
        var doc = Owner.DocumentManagerSubViewModel.ActiveDocument;
        if (doc is null)
            return;
        doc.Operations.SelectAll();
    }

    [Command.Basic("PixiEditor.Selection.Clear", "Clear selection", "Clear selection", CanExecute = "PixiEditor.Selection.IsNotEmpty", Key = Key.D, Modifiers = ModifierKeys.Control)]
    public void ClearSelection()
    {
        var doc = Owner.DocumentManagerSubViewModel.ActiveDocument;
        if (doc is null)
            return;
        doc.Operations.ClearSelection();
    }

    [Command.Basic("PixiEditor.Selection.InvertSelection", "Invert selection", "Invert the selected area", CanExecute = "PixiEditor.Selection.IsNotEmpty", Key = Key.I, Modifiers = ModifierKeys.Control)]
    public void InvertSelection()
    {
        Owner.DocumentManagerSubViewModel.ActiveDocument?.Operations.InvertSelection();
    }

    [Evaluator.CanExecute("PixiEditor.Selection.IsNotEmpty")]
    public bool SelectionIsNotEmpty()
    {
        return !Owner.DocumentManagerSubViewModel.ActiveDocument?.SelectionPathBindable?.IsEmpty ?? false;
    }

    [Evaluator.CanExecute("PixiEditor.Selection.IsNotEmptyAndHasMask")]
    public bool SelectionIsNotEmptyAndHasMask()
    {
        return SelectionIsNotEmpty() && (Owner.DocumentManagerSubViewModel.ActiveDocument?.SelectedStructureMember?.HasMaskBindable ?? false);
    }

    [Command.Basic("PixiEditor.Selection.TransformArea", "Transform selected area", "Transform selected area", CanExecute = "PixiEditor.Selection.IsNotEmpty", Key = Key.T, Modifiers = ModifierKeys.Control)]
    public void TransformSelectedArea()
    {
        Owner.DocumentManagerSubViewModel.ActiveDocument?.Operations.TransformSelectedArea(false);
    }

    [Command.Basic("PixiEditor.Selection.NudgeSelectedObjectLeft", "Nudge selected object left", "Nudge selected object left", Key = Key.Left, Parameter = new int[] { -1, 0 }, IconPath = "E76B", IconEvaluator = "PixiEditor.FontIcon", CanExecute = "PixiEditor.Selection.CanNudgeSelectedObject")]
    [Command.Basic("PixiEditor.Selection.NudgeSelectedObjectRight", "Nudge selected object right", "Nudge selected object right", Key = Key.Right, Parameter = new int[] { 1, 0 }, IconPath = "E76C", IconEvaluator = "PixiEditor.FontIcon", CanExecute = "PixiEditor.Selection.CanNudgeSelectedObject")]
    [Command.Basic("PixiEditor.Selection.NudgeSelectedObjectUp", "Nudge selected object up", "Nudge selected object up", Key = Key.Up, Parameter = new int[] { 0, -1 }, IconPath = "E70E", IconEvaluator = "PixiEditor.FontIcon", CanExecute = "PixiEditor.Selection.CanNudgeSelectedObject")]
    [Command.Basic("PixiEditor.Selection.NudgeSelectedObjectDown", "Nudge selected object down", "Nudge selected object down", Key = Key.Down, Parameter = new int[] { 0, 1 }, IconPath = "E70D", IconEvaluator = "PixiEditor.FontIcon", CanExecute = "PixiEditor.Selection.CanNudgeSelectedObject")]
    public void NudgeSelectedObject(int[] dist)
    {
        VecI distance = new(dist[0], dist[1]);
        Owner.DocumentManagerSubViewModel.ActiveDocument?.Operations.NudgeSelectedObject(distance);
    }

    [Command.Basic("PixiEditor.Selection.NewToMask", SelectionMode.New, "New mask from selection", "Selection to new mask", CanExecute = "PixiEditor.Selection.IsNotEmpty")]
    [Command.Basic("PixiEditor.Selection.AddToMask", SelectionMode.Add, "Add selection to mask", "Add selection to mask", CanExecute = "PixiEditor.Selection.IsNotEmpty")]
    [Command.Basic("PixiEditor.Selection.SubtractFromMask", SelectionMode.Subtract, "Subtract selection from mask", "Subtract selection from mask", CanExecute = "PixiEditor.Selection.IsNotEmptyAndHasMask")]
    [Command.Basic("PixiEditor.Selection.IntersectSelectionMask", SelectionMode.Intersect, "Intersect selection with mask", "Intersect selection with mask", CanExecute = "PixiEditor.Selection.IsNotEmptyAndHasMask")]
    [Command.Filter("PixiEditor.Selection.ToMaskMenu", "Selection to mask", "Selection to mask", Key = Key.M, Modifiers = ModifierKeys.Control)]
    public void SelectionToMask(SelectionMode mode)
    {
        Owner.DocumentManagerSubViewModel.ActiveDocument?.Operations.SelectionToMask(mode);
    }

    [Evaluator.CanExecute("PixiEditor.Selection.CanNudgeSelectedObject")]
    public bool CanNudgeSelectedObject(int[] dist) => Owner.DocumentManagerSubViewModel.ActiveDocument?.UpdateableChangeActive == true;
}
