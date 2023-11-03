﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedSelectionCommand.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a command that raises an event returning the hit models at the mouse location when the mouse button is clicked.
//   Provides a command that shows a rectangle when the mouse is dragged and raises an event returning the models contained in the rectangle
//   when the mouse button is released.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Provides a command that raises an event returning the hit models at the mouse location when the mouse button is clicked <br/>
    /// Provides a command that shows a rectangle when the mouse is dragged and raises an event returning the models contained in the rectangle
    /// when the mouse button is released.
    /// </summary>
    public class CombinedSelectionCommand : SelectionCommand
    {
        /// <summary>
        /// The selection rectangle.
        /// </summary>
        private Rect selectionRect;

        /// <summary>
        /// The rectangle adorner.
        /// </summary>
        private RectangleAdorner rectangleAdorner;

        /// <summary>
        /// Allow auto detect SelectionHitMode by mouse position<br/>
        ///  Default value is true
        ///</summary>
        /// <remarks>
        /// If mouse dragged from left to right: SelectionHitMode = SelectionHitMode.Inside<br/>
        /// Other SelectionHitMode = SelectionHitMode.Touch<br/>
        /// </remarks>    
        public bool AllowAutoSetSelectionHitMode { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleSelectionCommand" /> class.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="modelsSelectedEventHandler">The selection event handler.</param>
        public CombinedSelectionCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> modelsSelectedEventHandler)
            : base(viewport, modelsSelectedEventHandler, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleSelectionCommand" /> class.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="visualsSelectedEventHandler">The selection event handler.</param>
        public CombinedSelectionCommand(Viewport3D viewport, EventHandler<VisualsSelectedEventArgs> visualsSelectedEventHandler)
            : base(viewport, null, visualsSelectedEventHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleSelectionCommand" /> class.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="modelsSelectedEventHandler">The selection event handler.</param>
        /// <param name="visualsSelectedEventHandler">The selection event handler.</param>
        public CombinedSelectionCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> modelsSelectedEventHandler, EventHandler<VisualsSelectedEventArgs> visualsSelectedEventHandler)
            : base(viewport, modelsSelectedEventHandler, visualsSelectedEventHandler)
        {
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        protected override void Started(ManipulationEventArgs e)
        {
            base.Started(e);
            this.selectionRect = new Rect(this.MouseDownPoint, this.MouseDownPoint);
            this.ShowRectangle();
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        protected override void Delta(ManipulationEventArgs e)
        {
            base.Delta(e);
            this.selectionRect = new Rect(this.MouseDownPoint, e.CurrentPosition);
            this.UpdateRectangle();
        }

        /// <summary>
        /// The customized complete operation when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        protected override void Completed(ManipulationEventArgs e)
        {
            this.HideRectangle();
            if (AllowAutoSetSelectionHitMode)
            {
                if (MouseDownPoint.X < e.CurrentPosition.X) //left -> right => Inside
                {
                    SelectionHitMode = SelectionHitMode.Inside;
                }
                else
                {
                    SelectionHitMode = SelectionHitMode.Touch;
                }
            }
            var res = this.Viewport.FindHits(this.selectionRect, this.SelectionHitMode);
            var selectedModels = res.Select(hit => hit.Model).ToList();

            this.OnModelsSelected(new ModelsSelectedByRectangleEventArgs(selectedModels, this.selectionRect));
            var selectedVisuals = res.Select(hit => hit.Visual).ToList();
            this.OnVisualsSelected(new VisualsSelectedByRectangleEventArgs(selectedVisuals, this.selectionRect));
        }

        /// <summary>
        /// Gets the cursor for the gesture.
        /// </summary>
        /// <returns>
        /// A cursor.
        /// </returns>
        protected override Cursor GetCursor()
        {
            return Cursors.Arrow;
        }

        /// <summary>
        /// Hides the selection rectangle.
        /// </summary>
        private void HideRectangle()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (myAdornerLayer == null) { return; }
            if (this.rectangleAdorner != null)
            {
                myAdornerLayer.Remove(this.rectangleAdorner);
            }
            this.rectangleAdorner = null;
            this.Viewport.InvalidateVisual();
        }

        /// <summary>
        /// Updates the selection rectangle.
        /// </summary>
        private void UpdateRectangle()
        {
            if (this.rectangleAdorner == null)
            {
                return;
            }
            this.rectangleAdorner.Rectangle = this.selectionRect;
            this.rectangleAdorner.InvalidateVisual();
        }

        /// <summary>
        /// Shows the selection rectangle.
        /// </summary>
        private void ShowRectangle()
        {
            if (this.rectangleAdorner != null)
            {
                return;
            }

            var adornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (adornerLayer == null) { return; }
            this.rectangleAdorner = new RectangleAdorner(this.Viewport, this.selectionRect, Colors.LightGray, Colors.Black, 1, 1, 0, DashStyles.Dash);
            adornerLayer.Add(this.rectangleAdorner);
        }
    }
}
