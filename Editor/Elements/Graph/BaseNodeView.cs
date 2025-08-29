// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

//using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphViewBase
{
    public class BaseNodeView : GraphElement
    {
#region Constructor

        public BaseNodeView()
        {
            // Root Container
            MainContainer = this;

            // Title Container
            TitleContainer = new() { pickingMode = PickingMode.Ignore };
            TitleContainer.AddToClassList("title-container");
            hierarchy.Add(TitleContainer);

            TitleLabel = new Label();
            TitleLabel.AddToClassList("title-label");
            TitleContainer.Add(TitleLabel);

            // Input Container
            InputContainer = new() { pickingMode = PickingMode.Ignore };
            InputContainer.AddToClassList("inputs");

            // Output Container
            OutputContainer = new() { pickingMode = PickingMode.Ignore };
            OutputContainer.AddToClassList("outputs");

            // ports Container 
            PortContainer = new() { pickingMode = PickingMode.Ignore };
            PortContainer.AddToClassList("port-container");
            PortContainer.Add(InputContainer);
            PortContainer.Add(OutputContainer);
            hierarchy.Add(PortContainer);

            // Style
            AddToClassList("node");

            // Capability
            Capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable |
                            Capabilities.Ascendable;
            usageHints = UsageHints.DynamicTransform;

            //Debug.Log(this.HasMouseCapture());
        }

#endregion

#region Properties

        protected Label TitleLabel { get; }
        //protected TextField EditableTitleLabel { get; }
        public VisualElement MainContainer { get; }
        public VisualElement TitleContainer { get; }
        public VisualElement PortContainer { get; }
        public VisualElement InputContainer { get; }
        public VisualElement OutputContainer { get; }
        public VisualElement ExtensionContainer { get; }

        public override string Title {
            get => TitleLabel != null ? TitleLabel.text : string.Empty;
            set {
                if (TitleLabel != null) {
                    TitleLabel.text = value;
                }
            }
        }

        public override bool Selected {
            get => base.Selected;
            set {
                if (base.Selected == value) {
                    return;
                }
                base.Selected = value;
                if (value) {
                    AddToClassList("node--selected");
                }
                else {
                    RemoveFromClassList("node--selected");
                }
            }
        }

#endregion

#region Position

        public override void SetPosition(Vector2 newPosition)
        {
            base.SetPosition(newPosition);
        }

#endregion

#region Ports

        public virtual void AddPort(BasePort port)
        {
            port.ParentNode = this;
            if (port.Direction == Direction.Input) {
                InputContainer.Add(port);
            }
            else {
                OutputContainer.Add(port);
            }
        }

#endregion

#region Drag Events

        [EventInterest(typeof(DragOfferEvent))]
#if UNITY_6000_0_OR_NEWER
        protected override void HandleEventBubbleUp(EventBase evt)
        {
            base.HandleEventBubbleUp(evt);
#else
        protected override void ExecuteDefaultActionAtTarget(EventBase evt) {
            base.ExecuteDefaultActionAtTarget(evt);
#endif
            if (evt.eventTypeId == DragOfferEvent.TypeId()) {
                OnDragOffer((DragOfferEvent)evt);
            }
        }

        private void OnDragOffer(DragOfferEvent e)
        {
            // Check if this is a node drag event, additionaly check if the current dragged element is an edge
            if (!IsNodeDrag(e) || !IsMovable() || e.GetDraggedElement() is BaseEdge) {
                return;
            }

            // Accept Drag
            e.AcceptDrag(this);
        }

        private bool IsNodeDrag<T>(DragAndDropEvent<T> e) where T : DragAndDropEvent<T>, new()
        {
            if ((MouseButton)e.button != MouseButton.LeftMouse) {
                return false;
            }
            if (!e.modifiers.IsNone()) {
                return false;
            }
            return true;
        }

#endregion
    }
}