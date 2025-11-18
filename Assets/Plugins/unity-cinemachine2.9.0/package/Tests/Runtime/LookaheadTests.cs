using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cinemachine;

namespace Tests.Runtime
{
    [TestFixture]
    public class LookaheadTests : CinemachineFixtureBase
    {
        CinemachineVirtualCamera m_VCam;
        CinemachineComposer m_Composer;
        CinemachineFramingTransposer m_FramingTransposer;
        Transform m_Target;

        [SetUp]
        public override void SetUp()
        {
            // Camera
            CreateGameObject("MainCamera", typeof(Camera), typeof(CinemachineBrain));
            m_Target = CreateGameObject("Target Object").transform;

            // Source vcam
            m_VCam = CreateGameObject("Source CM Vcam", typeof(CinemachineVirtualCamera)).GetComponent<CinemachineVirtualCamera>();
            m_VCam.Follow = m_Target;
            m_VCam.LookAt = m_Target;
            m_FramingTransposer = m_VCam.AddCinemachineComponent<CinemachineFramingTransposer>();
            m_Composer = m_VCam.AddCinemachineComponent<CinemachineComposer>();
            m_FramingTransposer.m_LookaheadSmoothing = m_Composer.m_LookaheadSmoothing = 0.3f;
            m_FramingTransposer.m_LookaheadTime = m_Composer.m_LookaheadTime = 10;

            base.SetUp();
        }

        [UnityTest]
        public IEnumerator TargetsChanged()
        {
            Assert.That(m_VCam.FollowTargetChanged, Is.False);
            Assert.That(m_VCam.LookAtTargetChanged, Is.False);

            yield return null;
            Assert.That(m_VCam.FollowTargetChanged, Is.False);
            Assert.That(m_VCam.LookAtTargetChanged, Is.False);
            
            var newTarget = CreateGameObject("Target Object 2").transform;
            m_VCam.LookAt = newTarget;

            yield return null;
            
            Assert.That(m_VCam.FollowTargetChanged, Is.False);
            Assert.That(m_VCam.LookAtTargetChanged, Is.True);

            m_VCam.Follow = newTarget;

            yield return null;
            
            Assert.That(m_VCam.FollowTargetChanged, Is.True);
            Assert.That(m_VCam.LookAtTargetChanged, Is.False);

            m_VCam.Follow = m_Target;
            m_VCam.LookAt = m_Target;

            yield return null;
            
            Assert.That(m_VCam.FollowTargetChanged, Is.True);
            Assert.That(m_VCam.LookAtTargetChanged, Is.True);
        }

        [UnityTest]
        public IEnumerator LookaheadDelta()
        {
            var delta = m_Composer.m_Predictor.PredictPositionDelta(m_Composer.m_LookaheadTime);
            Assert.That(delta.sqrMagnitude > 0, Is.False);
            
            delta = m_FramingTransposer.m_Predictor.PredictPositionDelta(m_FramingTransposer.m_LookaheadTime);
            Assert.That(delta.sqrMagnitude > 0, Is.False);
            
            m_Target.Translate(10, 0, 0);
            yield return null;
            
            delta = m_Composer.m_Predictor.PredictPositionDelta(m_Composer.m_LookaheadTime);
            Assert.That(delta.sqrMagnitude > 0, Is.True);
            
            delta = m_FramingTransposer.m_Predictor.PredictPositionDelta(m_FramingTransposer.m_LookaheadTime);
            Assert.That(delta.sqrMagnitude > 0, Is.True);
        }
    }
}