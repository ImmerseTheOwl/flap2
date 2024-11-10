/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Oculus.Interaction.OVR.Editor
{
    [InitializeOnLoad]
    internal static class ProjectSetupTasks
    {
        static ProjectSetupTasks()
        {
            CheckHandTrackingSupportMode();
        }


        private static void CheckHandTrackingSupportMode()
        {
            OVRProjectSetup.AddTask(
                level: OVRProjectSetup.TaskLevel.Recommended,
                group: OVRProjectSetup.TaskGroup.Compatibility,
                isDone: _ =>
                {
                    OVRManager ovrManager = OVRProjectSetupUtils.FindComponentInScene<OVRManager>();
                    if (ovrManager == null)
                    {
                        return true;
                    }

                    var projectConfig = OVRProjectConfig.CachedProjectConfig;
                    return projectConfig.handTrackingSupport != OVRProjectConfig.HandTrackingSupport.ControllersOnly;

                },
                message: "Hand tracking support is set to \"Controllers Only\", hand tracking will not work in this mode.",
                fix: _ =>
                {
                    var projectConfig = OVRProjectConfig.CachedProjectConfig;
                    projectConfig.handTrackingSupport = OVRProjectConfig.HandTrackingSupport.ControllersAndHands;
                    OVRProjectConfig.CommitProjectConfig(projectConfig);
                },
                fixMessage: "Set hand tracking support mode to \"Controllers And Hands\" in the OVR Manager."
            );
        }
    }
}
