﻿using PixiEditor.ChangeableDocument.Changes.Drawing;
using PixiEditor.ChangeableDocument.Enums;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.DrawingApi.Core.Surface.Vector;

namespace PixiEditor.ChangeableDocument.Changes.Selection.MagicWand;

internal class MagicWand_Change : Change
{
    private VectorPath? originalPath;
    private VectorPath path = new() { FillType = PathFillType.EvenOdd };
    private VecI point;
    private readonly Guid memberGuid;
    private readonly bool referenceAll;
    private readonly bool drawOnMask;
    private readonly SelectionMode mode;

    [GenerateMakeChangeAction]
    public MagicWand_Change(Guid memberGuid, VecI point, SelectionMode mode, bool referenceAll, bool drawOnMask)
    {
        path.MoveTo(point);
        this.mode = mode;
        this.memberGuid = memberGuid;
        this.referenceAll = referenceAll;
        this.drawOnMask = drawOnMask;
        this.point = point;
    }

    public override bool InitializeAndValidate(Document target)
    {
        originalPath = new VectorPath(target.Selection.SelectionPath);
        return true;
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> Apply(Document target, bool firstApply, out bool ignoreInUndo)
    {
        var image = DrawingChangeHelper.GetTargetImageOrThrow(target, memberGuid, drawOnMask);

        HashSet<Guid> membersToReference = new();
        if (referenceAll)
            target.ForEveryReadonlyMember(member => membersToReference.Add(member.GuidValue));
        else
            membersToReference.Add(memberGuid);
        path = MagicWandHelper.DoMagicWandFloodFill(point, membersToReference, target);

        ignoreInUndo = false;
        return CommonApply(target);
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> Revert(Document target)
    {
        (var toDispose, target.Selection.SelectionPath) = (target.Selection.SelectionPath, new VectorPath(originalPath!));
        toDispose.Dispose();
        return new Selection_ChangeInfo(new VectorPath(target.Selection.SelectionPath));
    }

    private Selection_ChangeInfo CommonApply(Document target)
    {
        var toDispose = target.Selection.SelectionPath;
        if (mode == SelectionMode.New)
        {
            var copy = new VectorPath(path);
            copy.Close();
            target.Selection.SelectionPath = copy;
        }
        else
        {
            target.Selection.SelectionPath = originalPath!.Op(path, mode.ToVectorPathOp());
        }
        toDispose.Dispose();

        return new Selection_ChangeInfo(new VectorPath(target.Selection.SelectionPath));
    }

    public override void Dispose()
    {
        path.Dispose();
        originalPath?.Dispose();
    }
}
