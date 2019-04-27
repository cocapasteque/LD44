using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Util class to manage pooled objects. When creating a pool, it creates a GameObject _pool_{GUID or name}
/// That can be used to pool objects.
/// </summary>
/// <typeparam name="T">Type of the pooled MonoBehaviour</typeparam>
public class Pool<T> where T : MonoBehaviour
{
    private T[] _poolObjects;
    private Transform _pool;
    private int _poolIndex;

    /// <summary>
    /// Creates a new pool.
    /// </summary>
    /// <param name="size">The size of the pool.</param>
    /// <param name="name">(optional) The name of the pool</param>
    public Pool(int size, string name = "")
    {
        if (string.IsNullOrEmpty(name)) name = Guid.NewGuid().ToString();

        _pool = new GameObject($"_pool_{name}").GetComponent<Transform>();
        _pool.position = Vector3.zero;
        _pool.rotation = Quaternion.identity;

        _poolObjects = new T[size];
    }

    public List<T> AsList()
    {
        return _poolObjects.ToList();
    }

    /// <summary>
    /// Returns the next object from the pool in a circular way.
    /// </summary>
    /// <returns>The next object from the pool.</returns>
    public T Next()
    {
        T obj;

        // Get or create the pooled object
        if (_poolObjects[_poolIndex] == null)
        {
            obj = new GameObject($"_pooled_{_poolIndex}").AddComponent<T>();
            obj.transform.SetParent(_pool);
            _poolObjects[_poolIndex] = obj;
        }
        else
        {
            obj = _poolObjects[_poolIndex];
        }

        // Cicle the pool
        if (++_poolIndex >= _poolObjects.Length) _poolIndex = 0;

        return obj;
    }
}

