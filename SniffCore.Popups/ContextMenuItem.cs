//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SniffCore.Popups
{
    /// <summary>
    ///     Brings an easy to use MenuItem to be used in the ContextMenu no matter if its in an own VisualTree or not.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <TextBlock Text="{Binding Name}">
    ///     <TextBlock.ContextMenu>
    ///         <ContextMenu>
    ///             <Toolkit:ContextMenuItem Header="Edit Global"
    ///                                      Command="{Binding EditCommand}"
    ///                                      CommandParameter="{Binding ItemDataContext, RelativeSource={RelativeSource Self}}" />
    ///             <Toolkit:ContextMenuItem Header="Edit Directly"
    ///                                      IsBindToSelf="True"
    ///                                      Command="{Binding EditCommand}" />
    ///         </ContextMenu>
    ///     </TextBlock.ContextMenu>
    /// </TextBlock>
    /// ]]>
    /// </code>
    /// </example>
    public class ContextMenuItem : MenuItem
    {
        /// <summary>
        ///     The DependencyProperty for the Title ItemDataContext.
        /// </summary>
        public static readonly DependencyProperty ItemDataContextProperty =
            DependencyProperty.Register("ItemDataContext", typeof(object), typeof(ContextMenuItem), new UIPropertyMetadata(null));

        /// <summary>
        ///     The DependencyProperty for the Title ElementHolder.
        /// </summary>
        public static readonly DependencyProperty ElementHolderProperty =
            DependencyProperty.Register("ElementHolder", typeof(object), typeof(ContextMenuItem), new UIPropertyMetadata(OnElementHolderChanged));

        /// <summary>
        ///     The DependencyProperty for the Title IsBindToSelf.
        /// </summary>
        public static readonly DependencyProperty IsBindToSelfProperty =
            DependencyProperty.Register("IsBindToSelf", typeof(bool), typeof(ContextMenuItem), new UIPropertyMetadata(false));

        /// <summary>
        ///     Gets or sets the item in the DataContext of the own visual tree.
        /// </summary>
        public object ItemDataContext
        {
            get => GetValue(ItemDataContextProperty);
            set => SetValue(ItemDataContextProperty, value);
        }

        /// <summary>
        ///     Proxy to catch up the original visual tree.
        /// </summary>
        public object ElementHolder
        {
            get => GetValue(ElementHolderProperty);
            set => SetValue(ElementHolderProperty, value);
        }

        /// <summary>
        ///     Keeps the DataContext on the original. Its the same value as in <see cref="ItemDataContext" />.
        /// </summary>
        public bool IsBindToSelf
        {
            get => (bool) GetValue(IsBindToSelfProperty);
            set => SetValue(IsBindToSelfProperty, value);
        }

        /// <summary>
        ///     Initialized the <see cref="ContextMenuItem" />.
        /// </summary>
        /// <param name="e">The parameter.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var binding = new Binding
            {
                Path = new PropertyPath("PlacementTarget"),
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ContextMenu), 1)
            };
            SetBinding(ElementHolderProperty, binding);
        }

        private static void OnElementHolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = (ContextMenuItem) d;
            var parent = VisualTreeHelper.GetParent((DependencyObject) e.NewValue);
            while (parent != null)
            {
                if (parent is Window || parent is UserControl)
                    break;

                if (control.ItemDataContext == null && parent is FrameworkElement element)
                    control.ItemDataContext = element.DataContext;

                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent != null)
                control.DataContext = ((FrameworkElement) parent).DataContext;

            if (control.IsBindToSelf)
                control.DataContext = control.ItemDataContext;
        }
    }
}