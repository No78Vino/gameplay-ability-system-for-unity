using NUnit.Framework;
using Cinemachine.Editor;
using System.IO;

namespace Tests.Editor
{
    [TestFixture]
    public class ScriptableObjectUtilityTests
    {
        [Test]
        public void CinemachineInstallPathIsValid()
        {
            var pathToCmLogo = Path.Combine(ScriptableObjectUtility.CinemachineInstallPath, "Editor/EditorResources/cm_logo_sm.png");
            Assert.That(File.Exists(pathToCmLogo));
        }

        [Test]
        public void CinemachineInstallRelativePathIsValid()
        {
            var relativePathToCmLogo = Path.Combine(ScriptableObjectUtility.CinemachineRealativeInstallPath, "Editor/EditorResources/cm_logo_sm.png");
            var pathToCmLogo = Path.GetFullPath(relativePathToCmLogo);
            Assert.That(File.Exists(pathToCmLogo));
        }
    }
}