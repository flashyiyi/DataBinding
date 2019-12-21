using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataBinding;

public class Tester : MonoBehaviour
{
    [MenuItem("Tools/BindTester")]
    static void Test()
    {
        var obj = new A();
        var obj2 = new A();
        new BindHandler().SetBindProperty(v => obj2.a = v, () => obj.a);
        new BindHandler().SetBindProperty(v => obj.a = v, () => obj2.a);
        obj.a = 1;
        Debug.Log(obj.a + "," + obj2.a);
        obj2.a = 2;
        Debug.Log(obj.a + "," + obj2.a);
        //new BindHandler().AddTarget(()=> obj.a + obj.b).SetAction(e => Debug.Log(e.propertyName));
        //obj.a = 1;
        //obj.b = 1;
    }
}

public class A:BindAble
{
    private int _a;
    public int a { get => GetProperty(ref _a); set => SetProperty(ref _a, value); }

    private int _b;
    public int b { get => GetProperty(ref _b); set => SetProperty(ref _b, value); }
}
