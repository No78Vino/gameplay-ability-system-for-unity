using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cinemachine;
using UnityEngine.TestTools.Utils;

namespace Tests.Runtime
{
    public class CameraPositionTests : CinemachineFixtureBase
    {
        CinemachineVirtualCamera m_Vcam;
        GameObject m_FollowObject;

        [SetUp]
        public override void SetUp()
        {
            CreateGameObject("MainCamera", typeof(Camera), typeof(CinemachineBrain));
            m_Vcam = CreateGameObject("CM Vcam", typeof(CinemachineVirtualCamera)).GetComponent<CinemachineVirtualCamera>();
            m_Vcam.Priority = 100;
            m_FollowObject = CreateGameObject("Follow Object");
            
            base.SetUp();
        }

        [UnityTest]
        public IEnumerator DoNothing()
        {
            m_Vcam.Follow = m_FollowObject.transform;
            var oldPos = m_Vcam.transform.position;
            m_FollowObject.transform.position += new Vector3(2, 2, 2);
            yield return null;
            Assert.That(m_Vcam.State.FinalPosition, Is.EqualTo(oldPos).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator ThirdPerson()
        {
            m_Vcam.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
            m_Vcam.Follow = m_FollowObject.transform;
            m_FollowObject.transform.position += new Vector3(10, 0, 0);
            yield return null;
            Assert.That(m_Vcam.State.FinalPosition, Is.EqualTo(m_FollowObject.transform.position).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator FramingTransposer()
        {
            var component = m_Vcam.AddCinemachineComponent<CinemachineFramingTransposer>();
            component.m_XDamping = 0;
            component.m_YDamping = 0;
            component.m_ZDamping = 0;
            component.m_CameraDistance = 1f;
            m_Vcam.Follow = m_FollowObject.transform;
            m_FollowObject.transform.position += new Vector3(10, 0, 0);
            yield return null;
            Assert.That(m_Vcam.State.FinalPosition, Is.EqualTo(new Vector3(10, 0, -component.m_CameraDistance)).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator HardLockToTarget()
        {
            m_Vcam.AddCinemachineComponent<CinemachineHardLockToTarget>();
            m_Vcam.Follow = m_FollowObject.transform;
            m_FollowObject.transform.position += new Vector3(10, 0, 0);
            yield return null;
            Assert.That(m_Vcam.State.FinalPosition, Is.EqualTo(m_FollowObject.transform.position).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator OrbTransposer()
        {
            var component = m_Vcam.AddCinemachineComponent<CinemachineOrbitalTransposer>();
            component.m_XDamping = 0;
            component.m_YDamping = 0;
            component.m_ZDamping = 0;
            component.m_FollowOffset = new Vector3(0, 0, 0);
            m_Vcam.Follow = m_FollowObject.transform;
            m_FollowObject.transform.position += new Vector3(10, 0, 0);
            yield return null;
            Assert.That(m_Vcam.State.FinalPosition, Is.EqualTo(m_FollowObject.transform.position).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator TrackedDolly()
        {
            m_Vcam.AddCinemachineComponent<CinemachineTrackedDolly>();
            m_Vcam.Follow = m_FollowObject.transform;
            var oldPos = m_Vcam.transform.position;
            m_FollowObject.transform.position += new Vector3(2, 2, 2);
            yield return null;
            Assert.That(m_Vcam.State.FinalPosition, Is.EqualTo(oldPos).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator Transposer()
        {
            var component = m_Vcam.AddCinemachineComponent<CinemachineTransposer>();
            component.m_XDamping = 0;
            component.m_YDamping = 0;
            component.m_ZDamping = 0;
            component.m_FollowOffset = new Vector3(0, 0, 0);
            m_Vcam.Follow = m_FollowObject.transform;
            m_FollowObject.transform.position += new Vector3(10, 0, 0);
            yield return null;
            Assert.That(m_Vcam.State.FinalPosition, Is.EqualTo(m_FollowObject.transform.position).Using(Vector3EqualityComparer.Instance));
        }
    }
}