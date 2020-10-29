using Gizmo.Context.Gizmo_Authentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services.SessionState
{
    public interface IMappingSessionState
    {
        AppWorkTypes selectedWorkType { get; }
        bool showMapping { get; }
        Action ToggleMapping { get; }

        void SetSelectedWorkType(AppWorkTypes selectedWorkType);
        void SetToggleMappingAction(Action action);
        void ToggleMappingScreen();
    }

    public class MappingSessionState : IMappingSessionState
    {
        public bool showMapping { get; protected set; }

        public AppWorkTypes selectedWorkType { get; protected set; }

        public Action ToggleMapping { get; protected set; }

        public MappingSessionState()
        {
            showMapping = false;
        }

        public void ToggleMappingScreen()
        {
            showMapping = !showMapping;
        }

        public void SetSelectedWorkType(AppWorkTypes selectedWorkType)
        {
            this.selectedWorkType = selectedWorkType;
        }

        public void SetToggleMappingAction(Action action)
        {
            ToggleMapping = action;
        }

    }
}
