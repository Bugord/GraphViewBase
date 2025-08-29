using UnityEngine;
using static GraphViewBase.ShortcutHandler;

namespace GraphViewBase
{
    public class KeyAction
    {
        public class Handler
        {
            public readonly SpecialKey[] Checks;
            public readonly Actions ActionType;

            public Handler(SpecialKey[] checks, Actions actionType)
            {
                Checks = checks;
                ActionType = actionType;
            }
        }

        private readonly KeyCode keyCode;
        public readonly Handler handler;
        public readonly Handler altHandler;
        private string displayName;

        public string DisplayName => displayName;

        public KeyAction(KeyCode keyCode, Handler handler, Handler altHandler = null)
        {
            this.handler = handler;
            this.altHandler = altHandler;
            this.keyCode = keyCode;
            CreateDisplayName();
        }

        private void CreateDisplayName()
        {
            void CreateDisplayCodeFromHandler(Handler handler, ref string displayCode)
            {
                foreach (var specialKey in handler.Checks) {
                    if (specialKeyDisplayNameLookup.TryGetValue(specialKey, out var specialKeyDisplayName)) {
                        displayCode += specialKeyDisplayName + "+";
                    }
                }
            }

            var displayCode = "";
            CreateDisplayCodeFromHandler(handler, ref displayCode);
            if (altHandler != null) {
                displayCode += "/ ";
                CreateDisplayCodeFromHandler(altHandler, ref displayCode);
            }
            displayName = $"{handler.ActionType} ({displayCode + keyCode})";
        }

        private Actions ExecuteBase(EventModifiers modifiers, Handler handler, Handler alternativeHandler = null)
        {
            if (AreChecksValid(modifiers, handler.Checks)) {
                return handler.ActionType;
            }
            
            if (alternativeHandler != null && AreChecksValid(modifiers, alternativeHandler.Checks)) {
                return alternativeHandler.ActionType;
            }
            
            return Actions.NoAction;
        }

        public Actions Execute(EventModifiers modifiers)
        {
            return ExecuteBase(modifiers, handler, altHandler);
        }

        private bool AreChecksValid(EventModifiers eventModifiers, SpecialKey[] checks)
        {
            for (var i = 0; i < checks.Length; i++) {
                if (!specialKeyChecks[checks[i]](eventModifiers)) {
                    return false;
                }
            }
            return true;
        }
    }
}