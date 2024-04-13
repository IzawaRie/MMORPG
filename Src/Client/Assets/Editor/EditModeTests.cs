using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class EditModeTests {
    private MyClass mMyClass;

    [SetUp]
    public void SetUp()
    {
        mMyClass = new MyClass();
    }
    [Test]
    public void EditModeTestsSimplePasses() {
        // Use the Assert class to test conditions.
        int result = mMyClass.Add(1, 3);
        Debug.Log("Add=" + result);
        Assert.AreEqual(result, 4);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator EditModeTestsWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
