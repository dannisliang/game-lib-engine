using System;
using System.Collections;
using System.IO;
using UnityEngine;

public static class AssetBundleExtensions {

    //public static AssetBundleRequest LoadAsync<T>(this AssetBundle inst, string name) {
    //    return inst.LoadAsync(name, typeof(T));
    //}

    /// <summary>
    /// Instantiate the main asset of the bundle
    /// </summary>
    /// <param name="inst">Instance</param>
    /// <returns>Instantiated main asset</returns>
    ///
    public static UnityEngine.Object InstantiateMainAsset(this AssetBundle inst) {
        return UnityEngine.Object.Instantiate(inst.mainAsset);
    }

    /// <summary>
    /// Instantiate the main asset of the bundle at the given
    /// position and rotation.
    /// </summary>
    /// <param name="inst">Instance</param>
    /// <returns>Instantiated main asset</returns>
    ///
    public static UnityEngine.Object InstantiateMainAsset(this AssetBundle inst, Vector3 position, Quaternion rotation) {
        return UnityEngine.Object.Instantiate(inst.mainAsset, position, rotation);
    }

    /// <summary>
    /// Instantiate the main asset of the bundle and return the component
    /// specified in type parameter
    /// </summary>
    /// <param name="inst">Instance</param>
    /// <returns>Selected component of instantiated main asset</returns>
    ///
    public static T InstantiateMainAsset<T>(this AssetBundle inst)
        where T : Component {
        return (UnityEngine.Object.Instantiate(inst.mainAsset) as GameObject).GetComponent<T>();
    }

    /// <summary>
    /// Instantiate the main asset of the bundle and return the component
    /// specified in type parameter at the given position and rotation
    /// </summary>
    /// <param name="inst">Instance</param>
    /// <returns>Selected component of instantiated main asset</returns>
    ///
    public static T InstantiateMainAsset<T>(this AssetBundle inst, Vector3 position, Quaternion rotation)
        where T : Component {
        return (UnityEngine.Object.Instantiate(inst.mainAsset, position, rotation) as GameObject).GetComponent<T>();
    }
}