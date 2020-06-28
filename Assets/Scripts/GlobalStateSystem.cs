using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GlobalStateSystem : MonoBehaviour
{
    private readonly Dictionary<string, object> PropertyListeners = new Dictionary<string, object>();

    private bool isMoving;
    private Vector3 position;

    public bool IsMoving
    {
        get => isMoving;
        set
        {
            isMoving = value;
            NotifyPropertyChanged(nameof(IsMoving), value);
        }
    }

    public Vector3 Posititon
    {
        get => position;
        set
        {
            position = value;
            NotifyPropertyChanged(nameof(Posititon), value);
        }
    }

    public void ListenTo<T>(Expression<Func<GlobalStateSystem, T>> property, Action<T> listener)
    {
        if (property.Body.NodeType != ExpressionType.MemberAccess)
        {
            throw new ArgumentException("Only member access expressions are allowed in GlobalStateSystem.ListenTo");
        }

        string propName = ((MemberExpression) property.Body).Member.Name;
        if (PropertyListeners.ContainsKey(propName))
        {
            ((List<Action<T>>) PropertyListeners[propName]).Add(listener);
        }
        else
        {
            PropertyListeners.Add(propName, new List<Action<T>> {listener});
        }
    }

    private void NotifyPropertyChanged<T>(string property, T value)
    {
        var listeners = (List<Action<T>>) PropertyListeners[property];
        foreach (var listener in listeners)
        {
            listener(value);
        }
    }
}