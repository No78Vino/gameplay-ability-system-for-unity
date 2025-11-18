using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cinemachine;
using Tests.Runtime;

namespace Tests.Runtime
{
    [TestFixture]
    public class CamerasBlendingTests : CinemachineFixtureBase
    {
        private const float BlendingTime = 1;

        private CinemachineBrain m_Brain;
        private CinemachineVirtualCamera m_TargetVCam;

        [SetUp]
        public override void SetUp()
        {
            // Camera
            var cameraHolder = CreateGameObject("MainCamera", typeof(Camera), typeof(CinemachineBrain));
            m_Brain = cameraHolder.GetComponent<CinemachineBrain>();

            // Blending
            m_Brain.m_DefaultBlend = new CinemachineBlendDefinition(
                CinemachineBlendDefinition.Style.Linear,
                BlendingTime);

            var followObject = CreateGameObject("Follow Object");

            // Source vcam
            var sourceVCam = CreateGameObject("Source CM Vcam", typeof(CinemachineVirtualCamera)).GetComponent<CinemachineVirtualCamera>();
            sourceVCam.Priority = 2;
            sourceVCam.Follow = followObject.transform;

            // target vcam
            m_TargetVCam = CreateGameObject("Target CM Vcam", typeof(CinemachineVirtualCamera)).GetComponent<CinemachineVirtualCamera>();
            m_TargetVCam.Priority = 1;
            m_TargetVCam.Follow = followObject.transform;

            base.SetUp();
        }

        [UnityTest]
        public IEnumerator BlendingBetweenCameras()
        {
            m_TargetVCam.Priority = 3;
            yield return null;

            yield return new WaitForSeconds(BlendingTime + 0.01f);
            Assert.That(m_Brain.IsBlending, Is.False);
        }

        [UnityTest]
        public IEnumerator InterruptedBlendingBetweenCameras()
        {
            // Start blending
            m_TargetVCam.Priority = 3;
            yield return null;

            // Wait for 90% of blending duration
            yield return new WaitForSeconds(BlendingTime * 0.9f);

            // Blend back to source
            m_TargetVCam.Priority = 1;
            yield return null;
            yield return new WaitForSeconds(BlendingTime * 0.1f);

            // Quickly blend to target again
            m_TargetVCam.Priority = 3;
            yield return null;

            // We went 90%, then got 10% back, it means we are 20% away from the target
            yield return new WaitForSeconds(BlendingTime * 0.25f);

            Assert.That(m_Brain.IsBlending, Is.False);

            // Start blending
            m_TargetVCam.Priority = 3;
            yield return null;

            // Wait for 90% of blending duration
            yield return new WaitForSeconds(BlendingTime * 0.9f);

            // Blend back to source
            m_TargetVCam.Priority = 1;
            yield return null;
            yield return new WaitForSeconds(BlendingTime * 0.1f);

            // Quickly blend to target again
            m_TargetVCam.Priority = 3;
            yield return null;

            // We went 90%, then got 10% back, it means we are 20% away from the target - wait only 10% worth
            yield return new WaitForSeconds(BlendingTime * 0.1f);

            // Blend back to source
            m_TargetVCam.Priority = 1;
            yield return null;
            yield return new WaitForSeconds(BlendingTime * 0.1f);

            // Quickly blend to target again
            m_TargetVCam.Priority = 3;
            yield return null;

            // We went 90%, then got 10% back, it means we are 20% away from the target
            yield return new WaitForSeconds(BlendingTime * 0.25f);

            Assert.That(m_Brain.IsBlending, Is.False);
        }

        [UnityTest]
        public IEnumerator DoesInterruptedBlendingBetweenCamerasTakesDoubleTime()
        {
            // Start blending
            m_TargetVCam.Priority = 3;
            yield return null;

            // Wait for 90% of blending duration
            yield return new WaitForSeconds(BlendingTime * 0.9f);

            // Blend back to source
            m_TargetVCam.Priority = 1;
            yield return null;
            yield return new WaitForSeconds(BlendingTime * 0.1f);

            // Quickly blend to target again
            m_TargetVCam.Priority = 3;
            yield return null;

            // We went 90%, then got 10% back, it means we are 20% away from the target
            yield return new WaitForSeconds(BlendingTime + 0.01f);

            Assert.That(m_Brain.IsBlending, Is.False);
        }

        [UnityTest]
        public IEnumerator SetActiveBlend()
        {
            // Start blending
            m_TargetVCam.Priority = 3;

            // Wait 5 frames
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;

            var blend = m_Brain.ActiveBlend;
            float percentComplete = blend.TimeInBlend / blend.Duration;

            // Freeze the blend
            blend.TimeInBlend -= CinemachineCore.UniformDeltaTimeOverride;
            m_Brain.ActiveBlend = blend;

            // Wait a frame and check that TimeInBlend is the same
            yield return null;
            blend = m_Brain.ActiveBlend;
            Assert.That(percentComplete == blend.TimeInBlend / blend.Duration);

            // Force the blend to complete
            blend.Duration = 0;
            m_Brain.ActiveBlend = blend;

            // Wait a frame and check that blend is finished
            yield return null;
            Assert.That(m_Brain.ActiveBlend == null);

            // Blend back to source
            m_TargetVCam.Priority = 1;

            // Wait 5 frames
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            blend = m_Brain.ActiveBlend;
            Assert.That(percentComplete == blend.TimeInBlend / blend.Duration);

            // Kill the blend
            m_Brain.ActiveBlend = null;

            // Wait a frame and check that blend is finished
            yield return null;
            Assert.That(m_Brain.ActiveBlend == null);
        }
    }
}