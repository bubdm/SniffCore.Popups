//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Windows;
using SniffCore.Popups.Internal;

namespace SniffCore.Popups
{
    /// <summary>
    ///     This objects help to determine when a popup has to be closed. This can be by clicking somewhere else, clicking in
    ///     the title bar or moving the window.
    /// </summary>
    /// <example>
    ///     <code lang="csharp">
    /// <![CDATA[
    /// public class Control : ContentControl
    /// {
    ///     private PopupHandler _popupHandler;
    /// 
    ///     public override void OnApplyTemplate()
    ///     {
    ///         var popup = GetTemplateChild("PART_Popup") as Popup;
    ///         if (popup == null)
    ///             return;
    /// 
    ///         _popupHandler = new PopupHandler();
    ///         _popupHandler.AutoClose(popup, OnPopupClosed);
    ///     }
    /// 
    ///     private void OnPopupClosed()
    ///     {
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class PopupHandler
    {
        private Action _closeMethod;
        private UIElement _observedControl;
        private WindowObserver _observer;

        /// <summary>
        ///     Starts an observing of the window which contains the control to determine when the item has to be closed.
        /// </summary>
        /// <param name="observedControl">The control which owner window has to be observed.</param>
        /// <param name="closeMethod">
        ///     The close callback. This gets invoked when the owner window has send notifications to close
        ///     the popup.
        /// </param>
        /// <exception cref="ArgumentNullException">observedControl is null.</exception>
        /// <exception cref="ArgumentNullException">closeMethod is null.</exception>
        public void AutoClose(UIElement observedControl, Action closeMethod)
        {
            _observedControl = observedControl ?? throw new ArgumentNullException(nameof(observedControl));
            _closeMethod = closeMethod ?? throw new ArgumentNullException(nameof(closeMethod));

            var ownerWindow = Window.GetWindow(observedControl);
            if (ownerWindow != null)
            {
                _observer = new WindowObserver(ownerWindow);
                _observer.AddCallbackFor(0xA1, p => CallMethod()); // WM_NCLBUTTONDOWN
                _observer.AddCallbackFor(0x201, p => CallMethod()); // WM_LBUTTONDOWN
                _observer.AddCallbackFor(0x08, p => CallMethod()); // WM_KILLFOCUS
            }
        }

        private void CallMethod()
        {
            if (!_observedControl.IsMouseOver)
                _closeMethod();
        }
    }
}