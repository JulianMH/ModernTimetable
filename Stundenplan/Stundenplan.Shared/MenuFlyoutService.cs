﻿using System;
using System.Windows;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Stundenplan
{
    /// <summary>
    /// Provides the system implementation for displaying a MenuFlyout.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public static class MenuFlyoutService
    {
        /// <summary>
        /// Gets the value of the MenuFlyout property of the specified object.
        /// </summary>
        /// <param name="element">Object to query concerning the MenuFlyout property.</param>
        /// <returns>Value of the MenuFlyout property.</returns>
        public static MenuFlyout GetMenuFlyout(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (MenuFlyout)element.GetValue(MenuFlyoutProperty);
        }

        /// <summary>
        /// Sets the value of the MenuFlyout property of the specified object.
        /// </summary>
        /// <param name="element">Object to set the property on.</param>
        /// <param name="value">Value to set.</param>
        public static void SetMenuFlyout(DependencyObject element, MenuFlyout value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(MenuFlyoutProperty, value);
        }

        /// <summary>
        /// Identifies the MenuFlyout attached property.
        /// </summary>
        public static readonly DependencyProperty MenuFlyoutProperty = DependencyProperty.RegisterAttached(
            "MenuFlyout",
            typeof(MenuFlyout),
            typeof(MenuFlyoutService),
            new PropertyMetadata(null, OnMenuFlyoutChanged));

        private static void OnElementHolding(object sender, HoldingRoutedEventArgs args)
        {
            // this event is fired multiple times. We do not want to show the menu twice
            if (args.HoldingState != HoldingState.Started) return;

            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            // If the menu was attached properly, we just need to call this handy method
            FlyoutBase.ShowAttachedFlyout(element);
        }

        /// <summary>
        /// Handles changes to the MenuFlyout DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnMenuFlyoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = o as FrameworkElement;

            if (null != element)
            {
                // just in case we were here before and there is no new menu
                element.Holding -= OnElementHolding;
                element.RightTapped -= OnElementRightTap;

                MenuFlyout oldMenuFlyout = e.OldValue as MenuFlyout;

                if (null != oldMenuFlyout)
                {
                    // Remove previous attachment
                    element.SetValue(FlyoutBase.AttachedFlyoutProperty, null);
                }

                MenuFlyout newMenuFlyout = e.NewValue as MenuFlyout;

                if (null != newMenuFlyout)
                {
                    // attach using FlyoutBase to easier show the menu
                    element.SetValue(FlyoutBase.AttachedFlyoutProperty, newMenuFlyout);

                    // need to show it
                    element.Holding += OnElementHolding;
                    element.RightTapped += OnElementRightTap;
                }
            }
        }

        static void OnElementRightTap(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            // If the menu was attached properly, we just need to call this handy method
            FlyoutBase.ShowAttachedFlyout(element);
        }
    }
}

