using GadjIT.AppContext.GadjIT_App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Services.SessionState
{
    public interface IMappingSessionState
    {
        AppWorkTypes selectedWorkType { get; }
        bool showMapping { get; }
        Action ToggleMapping { get; }
        Action ToggleMappingOverviewScreen { get; }

        void SetSelectedWorkType(AppWorkTypes selectedWorkType);
        void SetToggleMappingAction(Action action);
        void SetToggleMappingOverviewAction(Action action);
        void ToggleMappingScreen();
    }

    public class MappingSessionState : IMappingSessionState
    {
        public bool showMapping { get; protected set; }

        public AppWorkTypes selectedWorkType { get; protected set; }

        public Action ToggleMapping { get; protected set; }

        public Action ToggleMappingOverviewScreen { get; protected set; }

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

        public void SetToggleMappingOverviewAction(Action action)
        {
            ToggleMappingOverviewScreen = action;
        }

    }
}
