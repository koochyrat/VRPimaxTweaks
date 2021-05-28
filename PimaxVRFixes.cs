using System;
using System.IO;
using HarmonyLib;
using QModManager.API.ModLoading;
using UnityEngine;
using UnityEngine.XR;
using System.Reflection;
using Valve.VR;

namespace VRPimaxTweaks
{
    [QModCore]
    public static class Loader
    {
        [QModPatch]
        public static void Initialize()
        {
            //I guess if they don't want to play in vr they don't have to.
            if (!XRSettings.enabled)
            {
                return;
            }
            File.AppendAllText("VRPimaxTweaksLog.txt", "Initializing" + Environment.NewLine);

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "VRPimaxTweaks");

            File.AppendAllText("VRPimaxTweaksLog.txt", "Done Initializing" + Environment.NewLine);
        }
    }

    [HarmonyPatch(typeof(ShaderGlobals), nameof(ShaderGlobals.Start))]
    class PimaxCullingInit
    {
        public static bool isCantedFov = false;
        public static Matrix4x4 projectionMatrix;
        public static float cantingAngle;

        [HarmonyPostfix]
        static void Start(ShaderGlobals __instance)
        {
            try
            {
                Camera m_Camera = Camera.main; //__instance.camera;
                HmdMatrix34_t eyeToHeadL = OpenVR.System.GetEyeToHeadTransform(Valve.VR.EVREye.Eye_Left);
                if (eyeToHeadL.m0 < 1)  //m0 = 1 for parallel projections
                {
                    isCantedFov = true;
                    float l_left = 0.0f, l_right = 0.0f, l_top = 0.0f, l_bottom = 0.0f;
                    OpenVR.System.GetProjectionRaw(EVREye.Eye_Left, ref l_left, ref l_right, ref l_top, ref l_bottom);
                    float r_left = 0.0f, r_right = 0.0f, r_top = 0.0f, r_bottom = 0.0f;
                    OpenVR.System.GetProjectionRaw(EVREye.Eye_Right, ref r_left, ref r_right, ref r_top, ref r_bottom);
                    Vector2 tanHalfFov = new Vector2(
                        Mathf.Max(-l_left, l_right, -r_left, r_right),
                        Mathf.Max(-l_top, l_bottom, -r_top, r_bottom));
                    float eyeYawAngle = Mathf.Acos(eyeToHeadL.m0);  //since there are no x or z rotations, this is y only. 10 deg on Pimax
                    cantingAngle = eyeYawAngle;
                    float eyeHalfFov = Mathf.Atan(tanHalfFov.x);
                    float tanCorrectedEyeHalfFovH = Mathf.Tan(eyeYawAngle + eyeHalfFov);

                    //increase horizontal fov by the eye rotation angles
                    projectionMatrix.m00 = 1 / tanCorrectedEyeHalfFovH;  //m00 = 0.1737 for Pimax

                    //because of canting, vertical fov increases towards the corners. calculate the new maximum fov otherwise culling happens too early at corners
                    float eyeFovLeft = Mathf.Atan(-l_left);
                    float tanCorrectedEyeHalfFovV = tanHalfFov.y * Mathf.Cos(eyeFovLeft) / Mathf.Cos(eyeFovLeft + eyeYawAngle);
                    projectionMatrix.m11 = 1 / tanCorrectedEyeHalfFovV;   //m11 = 0.3969 for Pimax

                    //set the near and far clip planes
                    projectionMatrix.m22 = -(m_Camera.farClipPlane + m_Camera.nearClipPlane) / (m_Camera.farClipPlane - m_Camera.nearClipPlane);
                    projectionMatrix.m23 = -2 * m_Camera.farClipPlane * m_Camera.nearClipPlane / (m_Camera.farClipPlane - m_Camera.nearClipPlane);
                    projectionMatrix.m32 = -1;
                    File.AppendAllText("VRPimaxTweaksLog.txt", "Pimax culling fix " + (eyeYawAngle * Mathf.Rad2Deg) + " deg" + Environment.NewLine);
                }
                else
                    isCantedFov = false;
            }
            catch (Exception e)
            {
                File.AppendAllText("VRPimaxTweaksLog.txt", "Pimax error " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
            }
        }
    }

    [HarmonyPatch(typeof(ShaderGlobals), nameof(ShaderGlobals.OnPreCull))]
    class PimaxCullingFix
    {
        [HarmonyPostfix]
        static void OnPreCull(ShaderGlobals __instance)
        {
            if (PimaxCullingInit.isCantedFov)
            {
                Camera.main.cullingMatrix = PimaxCullingInit.projectionMatrix * Camera.main.worldToCameraMatrix;
            }
        }
    }

    [HarmonyPatch(typeof(SNCameraRoot), nameof(SNCameraRoot.UpdateVR))]
    public class SNCameraRoot_Patch
    {
        static bool Prefix(SNCameraRoot __instance)
        {
            if (!XRSettings.enabled)
                return false;

            float num = __instance.mainCamera.stereoSeparation; //this is ipd returned from SteamVR
            if (Mathf.Abs(__instance.stereoSeparation - num) < 1E-05f)
            {
                return false;
            }
            //the camera's left and right projection matrix will be canted as returned from SteamVR
            //UI requires parallel projection, so we cancel out the canting angle so that both left and right UI will line up perfectly
            float yawAngle = PimaxCullingInit.cantingAngle * Mathf.Rad2Deg; //10 deg
            __instance.stereoSeparation = num;
            __instance.matrixLeftEye = Matrix4x4.TRS(__instance.stereoSeparation * 0.5f * Vector3.right, Quaternion.AngleAxis(-yawAngle, Vector3.up), new Vector3(1, 1, -1));
            __instance.matrixRightEye = Matrix4x4.TRS(-__instance.stereoSeparation * 0.5f * Vector3.right, Quaternion.AngleAxis(yawAngle, Vector3.up), new Vector3(1, 1, -1));

            __instance.guiCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, __instance.matrixLeftEye);
            __instance.guiCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, __instance.matrixRightEye);
            __instance.imguiCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, __instance.matrixLeftEye);
            __instance.imguiCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, __instance.matrixRightEye);
            return false;
        }
    }
}
