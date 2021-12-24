﻿using PixiEditor.Helpers;
using PixiEditor.Models.Controllers;
using PixiEditor.Models.Controllers.Shortcuts;
using PixiEditor.Models.Tools.Tools;
using System;
using System.Windows.Input;

namespace PixiEditor.ViewModels.SubViewModels.Main
{
    public class IoViewModel : SubViewModel<ViewModelMain>
    {
        public RelayCommand MouseMoveCommand { get; set; }

        public RelayCommand MouseDownCommand { get; set; }

        public RelayCommand PreviewMouseMiddleButtonCommand { get; set; }

        public RelayCommand MouseUpCommand { get; set; }

        public RelayCommand KeyDownCommand { get; set; }

        public RelayCommand KeyUpCommand { get; set; }

        private bool restoreToolOnKeyUp = false;

        private MouseInputFilter filter = new();

        public IoViewModel(ViewModelMain owner)
            : base(owner)
        {
            MouseDownCommand = new RelayCommand(filter.MouseDown);
            MouseMoveCommand = new RelayCommand(filter.MouseMove);
            MouseUpCommand = new RelayCommand(filter.MouseUp);
            PreviewMouseMiddleButtonCommand = new RelayCommand(OnPreviewMiddleMouseButton);
            GlobalMouseHook.OnMouseUp += filter.MouseUp;
            KeyDownCommand = new RelayCommand(OnKeyDown);
            KeyUpCommand = new RelayCommand(OnKeyUp);

            filter.OnMouseDown += OnMouseDown;
            filter.OnMouseMove += OnMouseMove;
            filter.OnMouseUp += OnMouseUp;
        }

        private void OnKeyDown(object parameter)
        {
            KeyEventArgs args = (KeyEventArgs)parameter;
            var key = args.Key;
            if (key == Key.System)
                key = args.SystemKey;

            ProcessShortcutDown(args.IsRepeat, key);

            if (Owner.BitmapManager.ActiveDocument != null)
            {
                Owner.BitmapManager.InputTarget.OnKeyDown(key);
            }
        }

        private void ProcessShortcutDown(bool isRepeat, Key key)
        {
            //if (isRepeat && !restoreToolOnKeyUp && Owner.ShortcutController.LastShortcut != null &&
            //    Owner.ShortcutController.LastShortcut.Command == Owner.ToolsSubViewModel.SelectToolCommand)
            //{
            //    restoreToolOnKeyUp = true;
            //    ShortcutController.BlockShortcutExecution = true;
            //}

            Owner.ShortcutController.KeyPressed(key, Keyboard.Modifiers);
        }

        private void OnKeyUp(object parameter)
        {
            KeyEventArgs args = (KeyEventArgs)parameter;
            var key = args.Key;
            if (key == Key.System)
                key = args.SystemKey;

            if (Owner.BitmapManager.ActiveDocument != null)
                Owner.BitmapManager.InputTarget.OnKeyUp(key);
        }

        private void OnMouseDown(object sender, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                BitmapManager bitmapManager = Owner.BitmapManager;
                var activeDocument = bitmapManager.ActiveDocument;
                if (activeDocument == null)
                    return;

                bitmapManager.InputTarget.OnLeftMouseButtonDown(activeDocument.MouseXOnCanvas, activeDocument.MouseYOnCanvas);
            }
        }

        private void OnPreviewMiddleMouseButton(object sender)
        {
            Owner.ToolsSubViewModel.SetActiveTool<MoveViewportTool>();
        }

        private void OnMouseMove(object sender, EventArgs args)
        {
            var activeDocument = Owner.BitmapManager.ActiveDocument;
            if (activeDocument == null)
                return;
            Owner.BitmapManager.InputTarget.OnMouseMove(activeDocument.MouseXOnCanvas, activeDocument.MouseYOnCanvas);
        }

        private void OnMouseUp(object sender, MouseButton button)
        {
            if (Owner.BitmapManager.ActiveDocument == null)
                return;
            if (button == MouseButton.Left)
            {
                Owner.BitmapManager.InputTarget.OnLeftMouseButtonUp();
            }
            else if (button == MouseButton.Middle)
            {
                if (Owner.ToolsSubViewModel.LastActionTool != null)
                    Owner.ToolsSubViewModel.SetActiveTool(Owner.ToolsSubViewModel.LastActionTool);
            }
        }
    }
}
