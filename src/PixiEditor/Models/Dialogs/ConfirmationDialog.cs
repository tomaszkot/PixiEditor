﻿using PixiEditor.Models.Enums;
using PixiEditor.Views;
using PixiEditor.Views.Dialogs;

namespace PixiEditor.Models.Dialogs;

internal static class ConfirmationDialog
{
    public static ConfirmationType Show(string message, string title)
    {
        ConfirmationPopup popup = new ConfirmationPopup
        {
            Title = title,
            Body = message,
            ShowInTaskbar = false
        };
        if (popup.ShowDialog().GetValueOrDefault())
        {
            return popup.Result ? ConfirmationType.Yes : ConfirmationType.No;
        }

        return ConfirmationType.Canceled;
    }
}
