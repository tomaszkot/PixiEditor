﻿using PixiEditor.ChangeableDocument.ChangeInfos.Properties;

namespace PixiEditor.ChangeableDocument.Changes.Properties;
internal class LayerLockTransparency_Change : Change
{
    private readonly Guid layerGuid;
    private bool originalValue;
    private readonly bool newValue;

    [GenerateMakeChangeAction]
    public LayerLockTransparency_Change(Guid layerGuid, bool newValue)
    {
        this.layerGuid = layerGuid;
        this.newValue = newValue;
    }

    public override OneOf<Success, Error> InitializeAndValidate(Document target)
    {
        var member = target.FindMember(layerGuid);
        if (member is not Layer layer)
            return new Error();
        originalValue = layer.LockTransparency;
        if (originalValue == newValue)
            return new Error();
        return new Success();
    }

    public override IChangeInfo? Apply(Document target, out bool ignoreInUndo)
    {
        ((Layer)target.FindMemberOrThrow(layerGuid)).LockTransparency = newValue;
        ignoreInUndo = false;
        return new LayerLockTransparency_ChangeInfo() { GuidValue = layerGuid };
    }

    public override IChangeInfo? Revert(Document target)
    {
        ((Layer)target.FindMemberOrThrow(layerGuid)).LockTransparency = originalValue;
        return new LayerLockTransparency_ChangeInfo() { GuidValue = layerGuid };
    }

    public override bool IsMergeableWith(Change other)
    {
        return other is LayerLockTransparency_Change;
    }
}
