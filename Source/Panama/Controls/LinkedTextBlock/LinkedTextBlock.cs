using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Represents a text block that has a command
    /// </summary>
    public class LinkedTextBlock : TextBlock, ICommandSource
    {
        #region Private
        private Brush originalForeground;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Defines a dependency property for the command.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register
            (
                nameof(Command), typeof(ICommand), typeof(LinkedTextBlock), new PropertyMetadata(null, OnCommandChanged)
            );

        /// <summary>
        /// Gets or sets the command parameter
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Defines a dependency property for the command parameter.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register
            (
                nameof(CommandParameter), typeof(object), typeof(LinkedTextBlock), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the brush used for rollover.
        /// </summary>
        public Brush RolloverBrush
        {
            get => (Brush)GetValue(RolloverBrushProperty);
            set => SetValue(RolloverBrushProperty, value);
        }

        /// <summary>
        /// The target element on which to fire the command.
        /// </summary>
        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for Target property
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register
            (
                nameof(CommandTarget), typeof(IInputElement), typeof(LinkedTextBlock), new PropertyMetadata(null)
            );

        /// <summary>
        /// Defines a dependency property for the rollover brush
        /// </summary>
        public static readonly DependencyProperty RolloverBrushProperty = DependencyProperty.Register
            (
                nameof(RolloverBrush), typeof(Brush), typeof(LinkedTextBlock), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedTextBlock"/> class.
        /// </summary>
        public LinkedTextBlock()
        {
            RolloverBrush = new SolidColorBrush(Colors.DarkRed);
            Cursor = Cursors.Hand;
            TextDecorations = System.Windows.TextDecorations.Underline;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Occurs when the mouse enters the control.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            originalForeground = Foreground;
            Foreground = RolloverBrush;
        }

        /// <summary>
        /// Occurs when the mouse leaves the control.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Foreground = originalForeground;
        }

        /// <summary>
        /// Occurs when a mouse button is released.
        /// </summary>
        /// <param name="e">The event args</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.ChangedButton == MouseButton.Left)
            {
                if (Command != null && Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinkedTextBlock t)
            {
                t.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
            }
        }

        private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                CanExecuteChangedEventManager.RemoveHandler(oldCommand, OnCanExecuteChanged);
            }
            if (newCommand != null)
            {
                CanExecuteChangedEventManager.AddHandler(newCommand, OnCanExecuteChanged);
            }
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            if (Command != null)
            {
                if (Command is RoutedCommand rc)
                {
                    IsEnabled = rc.CanExecute(CommandParameter, CommandTarget);
                }
                else
                {
                    IsEnabled = Command.CanExecute(CommandParameter);
                }
            }
        }
        #endregion
    }
}
